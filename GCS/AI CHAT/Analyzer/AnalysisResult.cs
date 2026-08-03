namespace GCS.AI_CHAT.Analyzer;

public class AnalysisResult
{
    public bool HasProblem { get; set; }

    public string Problem { get; set; } = "";

    public string Severity { get; set; } = "";

    public string Recommendation { get; set; } = "";

    public double Confidence { get; set; }
}