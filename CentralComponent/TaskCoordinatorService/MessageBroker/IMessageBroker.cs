namespace TaskCoordinatorService;

public interface IMessageBroker
{
    Task SendTaskAsync(TaskModel task, CancellationToken cancellationToken, int armId);
}