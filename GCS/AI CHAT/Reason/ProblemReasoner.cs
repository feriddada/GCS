using GCS.AI_CHAT.Memory;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Probability;
using GCS.AI_CHAT.Decision;
namespace GCS.AI_CHAT.Reasoning;

public class ProblemReasoner
{
    private AIMemory memory =
        new AIMemory();

    private ProbabilityEngine probability =
        new ProbabilityEngine();

    private DecisionMaker decision =
        new DecisionMaker();

    public DecisionResult Analyze(string problem)
    {
        var experiences =
            memory.Search(problem);

        DecisionResult result =
            new DecisionResult();

        result.Problem = problem;

        if (experiences.Count == 0)
        {
            result.MostLikelyCause = "Unknown";
            result.Confidence = 0;
            result.Recommendation = "No previous data found";

            return result;
        }

        // Ən yaxşı səbəbi DecisionMaker seçir
        Experience best =
            decision.Decide(experiences);

        result.MostLikelyCause =
            best.Cause;

        result.Recommendation =
            best.Solution;

        result.Confidence =
            probability.CalculateConfidence(
                experiences,
                best.Cause);

        return result;
    }
}