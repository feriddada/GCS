using GCS.AI_CHAT.KnowledgeBase;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;
using GCS.AI_CHAT.Probability;

namespace GCS.AI_CHAT.Reasoning;

public class HypothesisEngine
{
    private readonly ProbabilityEngine probability =
        new();

    private readonly KnowledgeEngine knowledge =
        new();

    public List<Hypothesis> Analyze(
        List<Evidence> evidences)
    {
        List<Hypothesis> results =
            new();

        //---------------------------------
        // Match Knowledge Rules
        //---------------------------------

        var matchedRules =
            knowledge.Match(evidences);

        //---------------------------------
        // Create Hypothesis
        //---------------------------------

        foreach (var rule in matchedRules)
        {
            Hypothesis hypothesis =
                new()
                {
                    Name = rule.Problem,

                    Reason =
                        rule.Recommendation,

                    Evidence =
                        evidences
                        .Where(x =>
                            rule.RequiredEvidence.Any(r =>
                                x.Description.Contains(r,
                                StringComparison.OrdinalIgnoreCase)))
                        .ToList()
                };

            hypothesis.Confidence =
                probability.Calculate(hypothesis);

            results.Add(hypothesis);
        }

        return results;
    }
}