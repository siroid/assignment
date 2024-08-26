using Confluent.Kafka;
using DataContracts;

namespace RoboticArm;

public class TelemetryService : ITelemetryService, IDisposable
{
    private readonly ILogger<TelemetryService> _logger;
    private readonly IAppState _appState;
    private readonly IProducer<Null, string> _producer;
    private readonly string _telemetryTopic;
    private readonly string _emergencyTopic;
    private readonly int _armId;

    public TelemetryService(ILogger<TelemetryService> logger, IAppState appState)
    {
        _logger = logger;
        _appState = appState;
        var config = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER")
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();

        _armId = int.Parse(Environment.GetEnvironmentVariable("ARM_ID"));
        _telemetryTopic = Environment.GetEnvironmentVariable("KAFKA_TELEMETRY_TOPIC");
        _emergencyTopic = Environment.GetEnvironmentVariable("KAFKA_EMERGENCY_TOPIC");
    }

    public async Task SendTelemetryDataAsync(CancellationToken cancellationToken)
    {
        if (!_appState.IsInitialized)
        {
            _logger.LogInformation("Arm is waiting for initializion");
            return;
        }

        // Generate telemetry data with possible warnings or errors
        var telemetryData = GenerateTelemetryData();

        // Serialize telemetry data to JSON
        var telemetryJson = System.Text.Json.JsonSerializer.Serialize(telemetryData);

        try
        {
            DeliveryResult<Null, string> result = null;
            if (telemetryData.Status == "Error")
            {
                result = await _producer.ProduceAsync(_emergencyTopic, new Message<Null, string> { Value = telemetryJson }, cancellationToken);
                _logger.LogError($"Error detected in telemetry data: {telemetryJson}");
            }
            else
            {
                result = await _producer.ProduceAsync(_telemetryTopic, new Message<Null, string> { Value = telemetryJson }, cancellationToken);
                _logger.LogWarning($"Regular telemetry data: {telemetryJson}");
            }

            // Log telemetry data sent
            _logger.LogInformation($"Telemetry data sent to Kafka: {telemetryJson}, Offset: {result.Offset}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was canceled.");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Failed to deliver message to Kafka: {e.Error.Reason}");
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10)); // Ensure all messages are sent before disposing
        _producer.Dispose();
    }

    private TelemetryData GenerateTelemetryData()
    {
        var random = new Random();
        var telemetryData = new TelemetryData
        {
            ArmId = _armId,
            Timestamp = DateTime.Now,
            Position = new Position
            {
                X = random.NextDouble() * 100,
                Y = random.NextDouble() * 100,
                Z = random.NextDouble() * 100
            },
            Temperature = random.Next(20, 100), // Temperature in degrees Celsius
            Status = "In Operation",
            PowerConsumption = random.NextDouble() * 10 // Power consumption in kW
        };

        // Simulate warnings and errors
        if (telemetryData.Temperature > 70 && telemetryData.Temperature <= 80)
        {
            telemetryData.Status = "Warning";
            telemetryData.Message = "Temperature approaching critical threshold.";
        }
        else if (telemetryData.Temperature > 80)
        {
            telemetryData.Status = "Error";
            telemetryData.Message = "Critical temperature threshold exceeded!";
        }
        else if (telemetryData.Position.X > 95 || telemetryData.Position.Y > 95 || telemetryData.Position.Z > 95)
        {
            telemetryData.Status = "Warning";
            telemetryData.Message = "Positioning error detected: Out of bounds.";
        }
        else if (telemetryData.PowerConsumption > 8)
        {
            telemetryData.Status = "Warning";
            telemetryData.Message = "High power consumption detected.";
        }
        else if (random.Next(0, 100) < 5) // 5% chance of random error
        {
            telemetryData.Status = "Error";
            telemetryData.Message = "Unexpected operational error.";
        }

        return telemetryData;
    }
}
