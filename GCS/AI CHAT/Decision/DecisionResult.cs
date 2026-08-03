namespace GCS.AI_CHAT.Decision;

public class DecisionResult
{
    // ===== Legacy AI =====

    public string Problem { get; set; } = "";

    public string MostLikelyCause { get; set; } = "";

    public string Recommendation { get; set; } = "";

    public int Confidence { get; set; }

    // ===== New AI =====

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public DecisionSeverity Severity { get; set; }

    public List<string> Recommendations { get; set; }
        = new();

    public List<string> RelatedParameters { get; set; }
        = new();
}