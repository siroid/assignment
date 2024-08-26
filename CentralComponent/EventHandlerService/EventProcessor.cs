using Confluent.Kafka;
using Polly;
using Polly.Retry;

namespace EventHandlerService;

/// <summary>
/// Processes regular events, aggregates the data and saves it to the DB
/// </summary>
public class EventProcessor : IEventProcessor, IDisposable
{
    private readonly ILogger<EventProcessor> _logger;
    private readonly string _bootstrapServers;
    private readonly RetryPolicy _retryPolicy;
    private IConsumer<Null, string> _consumer;

    public EventProcessor(ILogger<EventProcessor> logger)
    {
        _logger = logger;
        _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER"); // Kafka broker address

        // Define a retry policy with exponential backoff
        _retryPolicy = Policy
            .Handle<ConsumeException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                });

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP_ID"),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // Disable auto-commit for more control over offset handling
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _consumer.Subscribe(Environment.GetEnvironmentVariable("KAFKA_TELEMETRY_TOPIC"));
    }

    public async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _retryPolicy.Execute(async () =>
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                    // Process the message
                    await ProcessMessageAsync(consumeResult.Message.Value);

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
    }

    private async Task ProcessMessageAsync(string message)
    {
        _logger.LogInformation($"Processing message: {message}");

        // Simulate some asynchronous work
        await Task.Delay(500);
    }
}