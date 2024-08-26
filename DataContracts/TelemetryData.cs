namespace DataContracts;

public class TelemetryData
{
    public int ArmId { get; set;} // Robotic Arm ID
    public DateTime Timestamp { get; set; }
    public Position Position { get; set; }
    public double Temperature { get; set; } // Temperature in Celsius
    public string Status { get; set; } // Operational status
    public double PowerConsumption { get; set; } // Power consumption in kW
    public string Message { get; set; } // Message for warnings or errors

    public override string ToString()
    {
        return $"Timestamp: {Timestamp}, Position: ({Position.X}, {Position.Y}, {Position.Z}), " +
               $"Temperature: {Temperature}°C, Status: {Status}, Power Consumption: {PowerConsumption} kW, Message: {Message}";
    }
}