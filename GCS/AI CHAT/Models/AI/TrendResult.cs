namespace GCS.AI_CHAT.Models.AI;

public class TrendResult
{
    public bool Rising { get; set; }

    public bool Falling { get; set; }

    public bool Stable { get; set; }

    public double Change { get; set; }

    public string Comment { get; set; } = "";
}