using Confluent.Kafka;

namespace RoboticArm
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public TelemetryService(ILogger<TelemetryService> logger, IConfiguration configuration)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:TelemetryTopic"];
        }

        public async Task SendTelemetryDataAsync(CancellationToken cancellationToken)
        {
            // Generate telemetry data with possible warnings or errors
            var telemetryData = GenerateTelemetryData();

            // Serialize telemetry data to JSON
            var telemetryJson = System.Text.Json.JsonSerializer.Serialize(telemetryData);

            try
            {
                // Send telemetry data to Kafka
                var result = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = telemetryJson }, cancellationToken);

                // Log telemetry data sent
                _logger.LogInformation($"Telemetry data sent to Kafka: {telemetryJson}, Offset: {result.Offset}");

                // Log warning or error based on telemetry data
                if (telemetryData.Status == "Error")
                {
                    _logger.LogError($"Error detected in telemetry data: {telemetryJson}");
                }
                else if (telemetryData.Status == "Warning")
                {
                    _logger.LogWarning($"Warning detected in telemetry data: {telemetryJson}");
                }
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Failed to deliver message to Kafka: {e.Error.Reason}");
            }
        }

        private TelemetryData GenerateTelemetryData()
        {
            var random = new Random();
            var telemetryData = new TelemetryData
            {
                Timestamp = DateTime.Now,
                Position = new Position
                {
                    X = random.NextDouble() * 100,
                    Y = random.NextDouble() * 100,
                    Z = random.NextDouble() * 100
                },
                Temperature = random.Next(20, 90), // Temperature in degrees Celsius
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
}
