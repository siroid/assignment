namespace EventHandlerService
{
    public class EventHandlerWorker : BackgroundService
    {
        private readonly ILogger<EventHandlerWorker> _logger;
        private readonly IEventProcessor _eventProcessor;

        public EventHandlerWorker(ILogger<EventHandlerWorker> logger, IEventProcessor eventProcessor)
        {
            _logger = logger;
            _eventProcessor = eventProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Event Handler Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Event Handler Service is checking for new events.");

                    // Process incoming events
                    await _eventProcessor.ProcessEventsAsync(stoppingToken);

                    // Wait before checking for new events again
                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Event Handler Service.");
                }
            }

            _logger.LogInformation("Event Handler Service is stopping.");
        }
    }
}
