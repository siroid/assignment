using Confluent.Kafka;
using Polly;
using Polly.Retry;

namespace RoboticArm;

public class TaskHandler : ITaskHandler
{
    private readonly ILogger<TaskHandler> _logger;
    private readonly IAppState appState;
    private readonly IConsumer<Null, string> _consumer;
    private readonly RetryPolicy _retryPolicy;

    public TaskHandler(ILogger<TaskHandler> logger, IAppState appState)
    {
        _logger = logger;
        this.appState = appState;
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER"),
            GroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP_ID"),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // Disable auto-commit for more control over offset handling
        };

        _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();

        _retryPolicy = Policy
            .Handle<ConsumeException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
            });

        _consumer.Subscribe(Environment.GetEnvironmentVariable("KAFKA_TASKS_TOPIC"));
    }

    public async Task<ArmTask> GetNextTaskAsync(CancellationToken cancellationToken)
    {
        ArmTask? result = null;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _retryPolicy.Execute(async () =>
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                    // Process the message
                    result = System.Text.Json.JsonSerializer.Deserialize<ArmTask>(consumeResult.Message.Value);

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


        return result;

    }

    public async Task HandleTaskAsync(ArmTask task, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Handling task: {task.Id} with command: {task.Type} and parameters: {task.Parameters}");

        if (task.Type == ArmTaskTypes.ToggleOperationState)
        {
            appState.IsInitialized = !appState.IsInitialized;
        }

        // Log task completion
        _logger.LogInformation($"Task {task.Id} completed.");
    }
}