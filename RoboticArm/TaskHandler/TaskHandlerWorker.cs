namespace RoboticArm;

public class TaskHandlerWorker : BackgroundService
{
    private readonly ILogger<TaskHandlerWorker> _logger;
    private readonly ITaskHandler _taskHandler;
    private readonly IAppState _appState;

    public TaskHandlerWorker(ILogger<TaskHandlerWorker> logger, ITaskHandler taskHandler, IAppState appState)
    {
        _logger = logger;
        _taskHandler = taskHandler;
        _appState = appState;
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