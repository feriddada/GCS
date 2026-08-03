namespace GCS.AI_CHAT.Models.AI;

public class AIDashboard
{
    public int OverallHealth { get; set; }

    public bool SafeToFly { get; set; }

    public List<AIStatus> Components { get; set; }
        = new();

    public List<AIMessage> Messages { get; set; }
        = new();

    public List<string> Recommendations { get; set; }
        = new();
}