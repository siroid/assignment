namespace RoboticArm
{
    public interface ITelemetryService
    {
        Task SendTelemetryDataAsync(CancellationToken cancellationToken);
    }
}
