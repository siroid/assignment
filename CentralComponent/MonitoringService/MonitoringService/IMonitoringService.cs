namespace MonitoringService
{
    public interface IMonitoringService
    {
        Task ProcessEventsAsync(CancellationToken cancellationToken);
    }
}
