namespace GCS.AI_CHAT.Models.Analysis;

using GCS.AI_CHAT.Models.AI;
public class ESCHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public double MaxTemperature { get; set; }

    public bool OverheatDetected { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
    public List<Evidence> Evidence { get; set; }
    = new();
}