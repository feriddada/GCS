namespace GCS.AI_CHAT.Models.AI;

public class Cause
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public int Probability { get; set; }

    public List<string> Symptoms { get; set; }
        = new();

    public List<string> Recommendations { get; set; }
        = new();
}