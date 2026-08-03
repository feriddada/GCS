namespace GCS.AI_CHAT.Models;

public class CauseProbability
{
    public string Cause { get; set; } = "";

    public int Count { get; set; }

    public int Confidence { get; set; }

    public string Solution { get; set; } = "";
}