using System.Security.Authentication;
using Confluent.Kafka;
using Grpc.Net.Client;
using StorageConnector;

namespace EventHandlerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly IStorageConnector _storageConnector;

    public Worker(ILogger<Worker> logger, IStorageConnector storageConnector)
    {
        _logger = logger;
        _storageConnector = storageConnector;

        var config = new ConsumerConfig
        {
            GroupId = "consumer-group",
            BootstrapServers = "kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _consumer.Subscribe("robotic-arm-events"); // Subscribe to the Kafka topic
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var grpcServerAddress = Environment.GetEnvironmentVariable("GRPC_SERVER_ADDRESS");
                    if (string.IsNullOrEmpty(grpcServerAddress))
                    {
                        grpcServerAddress = "http://storage-middleware-api:80"; // Fallback
                    }

                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    httpClientHandler.SslProtocols = SslProtocols.Tls12;

                    var httpClient = new HttpClient(httpClientHandler);
                    httpClient.DefaultRequestVersion = new Version(2, 0); // Ensure HTTP/2

                    var channel = GrpcChannel.ForAddress(grpcServerAddress, new GrpcChannelOptions
                    {
                        HttpClient = httpClient
                    });

                    var client = new StorageService.StorageServiceClient(channel);
                    try
                    {
                        var f = await client.CreateDataAsync(new CreateDataRequest() { Key = "1", Value = "2" });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var d = consumeResult.Message.Key;
                    //   var f = await  _storageConnector.CreateDataAsync(consumeResult.Message.Timestamp.ToString(), consumeResult.Message.Value);
                    _logger.LogInformation($"Consumed message: {consumeResult.Message.Value}");
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error consuming message: {ex.Message}");
                }
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}