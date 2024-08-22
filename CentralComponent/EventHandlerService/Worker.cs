using Confluent.Kafka;

namespace EventHandlerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<Null, string> _consumer;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            GroupId = "consumer-group",
            BootstrapServers = "cdkafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _consumer.Subscribe("robotic-arm-events"); // Subscribe to the Kafka topic
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
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