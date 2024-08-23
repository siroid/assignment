namespace RoboticArm;

public class TaskHandler : ITaskHandler
{
    private readonly ILogger<TaskHandler> _logger;

    public TaskHandler(ILogger<TaskHandler> logger)
    {
        _logger = logger;
    }

    public async Task<ArmTask> GetNextTaskAsync(CancellationToken cancellationToken)
    {
        // Simulate fetching the next task from a queue or database
        await Task.Delay(500, cancellationToken); // Simulate async work
        return new ArmTask { Id = Guid.NewGuid().ToString(), Command = "MoveToPosition", Parameters = "X:10,Y:20" };
    }

    public async Task HandleTaskAsync(ArmTask task, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Handling task: {task.Id} with command: {task.Command} and parameters: {task.Parameters}");

        // Simulate performing the task
        await Task.Delay(1000, cancellationToken); // Simulate async work

        // Log task completion
        _logger.LogInformation($"Task {task.Id} completed.");
    }
}