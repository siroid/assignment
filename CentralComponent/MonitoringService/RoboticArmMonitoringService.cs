namespace MonitoringService;

public class RoboticArmMonitoringService : IMonitoringService
{
    private readonly ILogger<RoboticArmMonitoringService> _logger;

    public RoboticArmMonitoringService(ILogger<RoboticArmMonitoringService> logger)
    {
        _logger = logger;
    }

    public async Task<ArmStatus[]> CheckStatusAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);

        var random = new Random();
        var statuses = Enumerable.Range(1, 5)
            .Select(i => new ArmStatus
            {
                ArmId = $"Arm-{i}",
                Status = random.Next(0, 10) > 2 ? "OK" : "Error"
            })
            .ToArray();

        foreach (var status in statuses)
        {
            _logger.LogInformation($"Robotic arm {status.ArmId} status checked: {status.Status}");
        }

        return statuses;
    }
}