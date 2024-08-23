using Confluent.Kafka;
using Polly;
using Polly.Retry;

namespace EventHandlerService;

public class EventProcessor : IEventProcessor
{
    private readonly ILogger<EventProcessor> _logger;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly RetryPolicy _retryPolicy;

    public EventProcessor(ILogger<EventProcessor> logger)
    {
        _logger = logger;
        _bootstrapServers = "localhost:9092"; // Kafka broker address
        _topic = "event-topic"; // Kafka topic name

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
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "event-handler-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // Disable auto-commit for more control over offset handling
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _retryPolicy.Execute(async () =>
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                        // Process the message
                        await ProcessMessageAsync(consumeResult.Message.Value);

                        // Commit the offset after successful processing
                        consumer.Commit(consumeResult);
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
            finally
            {
                consumer.Close();
            }
        }
    }

    private async Task ProcessMessageAsync(string message)
    {
        
        _logger.LogInformation($"Processing message: {message}");

        // Simulate some asynchronous work
        await Task.Delay(500);

        
    }
}

