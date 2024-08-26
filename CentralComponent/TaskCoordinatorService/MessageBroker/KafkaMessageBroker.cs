using Confluent.Kafka;
using RoboticArm;

namespace TaskCoordinatorService;

public class KafkaMessageBroker : IMessageBroker
{
    private readonly ILogger<KafkaMessageBroker> _logger;
    private readonly string _topic;
    private readonly IProducer<Null, ArmTask> _producer;

    public KafkaMessageBroker(ILogger<KafkaMessageBroker> logger, string brokerList, string topic)
    {
        _logger = logger;
        _topic = topic;

        var config = new ProducerConfig
        {
            BootstrapServers = brokerList
        };

        _producer = new ProducerBuilder<Null, ArmTask>(config).Build();
    }

    public async Task SendTaskAsync(TaskModel task, CancellationToken cancellationToken, int armId)
    {
        try
        {
            var message = new Message<Null, ArmTask> { Value = new ArmTask{Id = task.Id.ToString(),  Type = ArmTaskTypes.ToggleOperationState} };
            var deliveryResult = await _producer.ProduceAsync(_topic+armId, message, cancellationToken);

            _logger.LogInformation($"Task {task.RoboticArmId} sent to Kafka topic {_topic}. Partition: {deliveryResult.Partition}, Offset: {deliveryResult.Offset}");
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, $"Failed to send task {task.Id} to Kafka.");
        }
    }
}
