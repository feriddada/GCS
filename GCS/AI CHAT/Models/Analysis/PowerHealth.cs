namespace GCS.AI_CHAT.Models.Analysis;

public class PowerHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public double Voltage { get; set; }

    public double Current { get; set; }

    public bool BrownoutRisk { get; set; }

    public bool VoltageDrop { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}