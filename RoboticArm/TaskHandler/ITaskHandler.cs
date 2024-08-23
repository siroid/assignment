namespace RoboticArm;

public interface ITaskHandler
{
    Task<ArmTask> GetNextTaskAsync(CancellationToken cancellationToken);
    Task HandleTaskAsync(ArmTask task, CancellationToken cancellationToken);
}