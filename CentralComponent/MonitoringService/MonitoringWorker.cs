namespace MonitoringService;

public class MonitoringWorker : BackgroundService
{
    private readonly ILogger<MonitoringWorker> _logger;
    private readonly IMonitoringService _monitoringService;

    public MonitoringWorker(ILogger<MonitoringWorker> logger, IMonitoringService monitoringService)
    {
        _logger = logger;
        _monitoringService = monitoringService;
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
                await _monitoringService.ProcessEventsAsync(stoppingToken);

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
