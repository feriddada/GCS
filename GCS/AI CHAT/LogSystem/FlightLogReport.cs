using GCS.AI_CHAT.Analyzer;

namespace GCS.AI_CHAT.LogSystem;

public class FlightLogReport
{
    public List<AnalysisResult> Results { get; set; }
        = new();

    public int TotalProblems { get; set; }

    public bool SafeToFly { get; set; }

    public string Summary { get; set; } = "";
}