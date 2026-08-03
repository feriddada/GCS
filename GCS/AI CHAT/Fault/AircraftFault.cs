namespace GCS.AI_CHAT.Fault;

public class AircraftFault
{
    public AircraftFaultType Type { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int Confidence { get; set; }

    public AircraftFaultSeverity Severity { get; set; }
}