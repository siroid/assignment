namespace RoboticArm;

public class RoboticArmWorker : BackgroundService
{
    private readonly ILogger<RoboticArmWorker> _logger;
    private readonly ITaskHandler _taskHandler;
    private readonly ITelemetryService _telemetryService;

    public RoboticArmWorker(ILogger<RoboticArmWorker> logger, ITaskHandler taskHandler, ITelemetryService telemetryService)
    {
        _logger = logger;
        _taskHandler = taskHandler;
        _telemetryService = telemetryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Robotic Arm Service is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Robotic Arm Service is checking for new tasks.");

                // Get and handle the next task
                var task = await _taskHandler.GetNextTaskAsync(stoppingToken);
                if (task != null)
                {
                    await _taskHandler.HandleTaskAsync(task, stoppingToken);
                }

                // Send telemetry data
                await _telemetryService.SendTelemetryDataAsync(stoppingToken);

                // Wait before checking for new tasks or sending telemetry data again
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Robotic Arm Service.");
            }
        }

        _logger.LogInformation("Robotic Arm Service is stopping.");
    }
}