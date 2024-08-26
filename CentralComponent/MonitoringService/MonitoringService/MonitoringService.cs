using Confluent.Kafka;
using DataContracts;
using Polly;
using Polly.Retry;
using StorageConnector;

namespace MonitoringService;

// <summary>
// Processes emergency events, checks the situation, and decides if it requires stopping the arm. 
// If it does, this task is delegated to the TaskCoordinatorService by sending a message to a specific Kafka topic, to which the TaskCoordinator is subscribed.
// </summary>
public class MonitoringService : IMonitoringService
{
    private readonly ILogger<MonitoringService> _logger;
    private readonly IStorageConnector _storageConnector;
    private readonly IProducer<Null, string> _producer;
    private  IConsumer<Null, string> _consumer;

    private readonly string _eventTopic; // Robotic Arms --> Monitoring 
    private readonly string _taskCoordinatorTopic; // Monitoring --> TaskCoordinator
    private readonly RetryPolicy _retryPolicy;

    public MonitoringService(ILogger<MonitoringService> logger, IStorageConnector storageConnector)
    {
        _logger = logger;
        _storageConnector = storageConnector;
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER"),
            GroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP_ID"),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // Disable auto-commit for more control over offset handling
        };

        _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER")
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _eventTopic = Environment.GetEnvironmentVariable("KAFKA_EMERGENCY_TOPIC");
        _taskCoordinatorTopic = Environment.GetEnvironmentVariable("KAFKA_TASKCOORDINATOR_TOPIC");

        // Define a retry policy with exponential backoff
        _retryPolicy = Policy
            .Handle<ConsumeException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                });
    }

    public async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_eventTopic);
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _retryPolicy.Execute(async () =>
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                    // Process the message
                    await ProcessMessageAsync(consumeResult.Message.Value, cancellationToken);

                    // Commit the offset after successful processing
                    _consumer.Commit(consumeResult);
                });
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing events.");
        }
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();

        _producer.Flush(TimeSpan.FromSeconds(10)); // Ensure all messages are sent before disposing
        _producer.Dispose();
    }

    private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        var telemetry = System.Text.Json.JsonSerializer.Deserialize<TelemetryData>(message);

        if (telemetry.Temperature > 90) // Alarm! This is very high temperature, redirect this data to TaskCoordinator to allow him to decide what to do with this
        {
            try
            {
                string armTemperatureJson = System.Text.Json.JsonSerializer.Serialize(new RoboticArmTemperature { Temperature = telemetry.Temperature, ArmId = telemetry.ArmId });
                await _producer.ProduceAsync(_taskCoordinatorTopic, new Message<Null, string> { Value = armTemperatureJson }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing events.");
            }
        }

        // If this is a regular error, just process it and save to the db

        // Here is some procesing ...
        await _storageConnector.CreateDataAsync($"{telemetry.ArmId}+{telemetry.Timestamp}", message);
    }
}