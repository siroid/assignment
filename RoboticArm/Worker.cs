using Confluent.Kafka;

namespace RoboticArmService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IProducer<Null, string> _producer;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = $"Robot arm status update at {DateTimeOffset.Now}";
            _logger.LogInformation(message);

            await _producer.ProduceAsync("robotic-arm-events", new Message<Null, string> { Value = message });

            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _producer.Flush(cancellationToken);
        _producer.Dispose();
        await base.StopAsync(cancellationToken);
    }
}
