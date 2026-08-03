namespace GCS.AI_CHAT.Models.Analysis;

public class EKFHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public bool VarianceDetected { get; set; }

    public bool InnovationError { get; set; }

    public bool FailsafeTriggered { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}