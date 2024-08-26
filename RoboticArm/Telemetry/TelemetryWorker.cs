namespace RoboticArm;

public class TelemetryWorker : BackgroundService
{
    private readonly ILogger<TelemetryWorker> _logger;
    private readonly ITelemetryService _telemetryService;

    public TelemetryWorker(ILogger<TelemetryWorker> logger, ITelemetryService telemetryService)
    {
        _logger = logger;
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