namespace GCS.AI_CHAT.Models.Analysis;

using GCS.AI_CHAT.Models.AI;
public class MotorHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    // Motor Analysis
    public double AverageCurrent { get; set; }

    public double MaxCurrent { get; set; }

    public double CurrentDifference { get; set; }

    public bool ImbalanceDetected { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
    public List<Evidence> Evidence { get; set; }
    = new();
}