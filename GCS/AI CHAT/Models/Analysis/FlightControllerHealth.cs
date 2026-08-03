namespace GCS.AI_CHAT.Models.Analysis;

public class FlightControllerHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public double CpuLoad { get; set; }

    public double LoopRate { get; set; }

    public bool InternalError { get; set; }

    public bool Failsafe { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}