using Confluent.Kafka;

namespace TaskCoordinatorService;

public class KafkaMessageBroker : IMessageBroker
{
    private readonly ILogger<KafkaMessageBroker> _logger;
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaMessageBroker(ILogger<KafkaMessageBroker> logger, string brokerList, string topic)
    {
        _logger = logger;
        _topic = topic;

        var config = new ProducerConfig
        {
            BootstrapServers = brokerList
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task SendTaskAsync(TaskModel task, CancellationToken cancellationToken)
    {
        try
        {
            var message = new Message<Null, string> { Value = task.Description };
            var deliveryResult = await _producer.ProduceAsync(_topic, message, cancellationToken);

            _logger.LogInformation($"Task {task.Id} sent to Kafka topic {_topic}. Partition: {deliveryResult.Partition}, Offset: {deliveryResult.Offset}");
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, $"Failed to send task {task.Id} to Kafka.");
        }
    }
}
