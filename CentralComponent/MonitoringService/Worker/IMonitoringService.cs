namespace MonitoringService
{
    public interface IMonitoringService
    {
        Task<ArmStatus[]> CheckStatusAsync(CancellationToken cancellationToken);
    }
}
