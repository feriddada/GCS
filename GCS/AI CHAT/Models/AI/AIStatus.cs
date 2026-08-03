namespace GCS.AI_CHAT.Models.AI;

public class AIStatus
{
    public string Component { get; set; } = "";

    public string Status { get; set; } = "";

    public int HealthScore { get; set; }

    public string AIComment { get; set; } = "";

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}