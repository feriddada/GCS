namespace GCS.AI_CHAT.Models.Analysis;

public class GPSAnalysis
{
    public int Satellites { get; set; }

    public double HDOP { get; set; }

    public bool GPSGlitch { get; set; }

    public string Accuracy { get; set; } = "";

    public string Recommendation { get; set; } = "";
}