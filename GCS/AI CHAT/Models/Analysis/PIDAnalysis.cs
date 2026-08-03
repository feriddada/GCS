namespace GCS.AI_CHAT.Models.Analysis;

public class PIDAnalysis
{
    public bool OscillationDetected { get; set; }

    public bool OvershootDetected { get; set; }

    public bool SlowResponse { get; set; }

    public string Stability { get; set; } = "";

    public string Recommendation { get; set; } = "";
}