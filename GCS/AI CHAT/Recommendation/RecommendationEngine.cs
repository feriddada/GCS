using GCS.AI_CHAT.KnowledgeBase;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Recommendation;

public class RecommendationEngine
{
    private readonly KnowledgeDatabase database =
        new();

    public List<string> Generate(
        List<Hypothesis> hypotheses)
    {
        List<string> recommendations =
            new();

        foreach (var hypothesis in hypotheses)
        {
            var rule =
                database.Rules.FirstOrDefault(x =>
                    x.Problem == hypothesis.Name);

            if (rule == null)
                continue;

            //---------------------------------
            // Recommendation
            //---------------------------------

            recommendations.Add(
                rule.Recommendation);

            //---------------------------------
            // Suggested Parameters
            //---------------------------------

            foreach (var parameter in rule.Parameters)
            {
                recommendations.Add(
                    $"Suggested Parameter: {parameter}");
            }
        }

        return recommendations
            .Distinct()
            .ToList();
    }
}