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
        _logger.LogInformation("Monitoring Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var statuses = await _monitoringService.CheckStatusAsync(stoppingToken);

                foreach (var status in statuses)
                {
                    _logger.LogInformation($"Robotic arm {status.ArmId} status: {status.Status}");

                    if (status.Status == "Error")
                    {
                        _logger.LogWarning($"Robotic arm {status.ArmId} is in error state!");
                    }
                }

                await Task.Delay(10000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Monitoring Service.");
            }
        }

        _logger.LogInformation("Monitoring Service is stopping.");
    }
}
