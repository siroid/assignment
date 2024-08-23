namespace TaskCoordinatorService;

public class TaskCoordinatorWorker : BackgroundService
{
    private readonly ILogger<TaskCoordinatorWorker> _logger;
    private readonly IMessageBroker _messageBroker;

    public TaskCoordinatorWorker(ILogger<TaskCoordinatorWorker> logger, IMessageBroker messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task Coordinator Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var newTask = FetchNewTask();

                if (newTask != null)
                {
                    _logger.LogInformation($"Assigning task {newTask.Id} to a robotic arm.");
                    await _messageBroker.SendTaskAsync(newTask, stoppingToken);
                    _logger.LogInformation($"Task {newTask.Id} assigned.");
                }
                else
                {
                    _logger.LogInformation("No new tasks to assign.");
                }

                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Task Coordinator Service.");
            }
        }

        _logger.LogInformation("Task Coordinator Service is stopping.");
    }

    private TaskModel FetchNewTask()
    {
        return new TaskModel { Id = Guid.NewGuid().ToString(), Description = "Sample Task" };
    }
}