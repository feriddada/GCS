using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Probability;

public class ProbabilityEngine
{
    public int Calculate(Hypothesis hypothesis)
    {
        if (hypothesis.Evidence.Count == 0)
            return 0;

        int totalWeight =
            hypothesis.Evidence.Sum(x => x.Weight);

        int confidence =
            totalWeight /
            hypothesis.Evidence.Count;

        if (confidence > 100)
            confidence = 100;

        return confidence;
    }
    public int CalculateConfidence(
    List<Experience> experiences,
    string cause)
    {
        if (experiences.Count == 0)
            return 0;

        int sameCause =
            experiences.Count(x =>
                x.Cause == cause);

        return
            (int)((double)sameCause /
            experiences.Count * 100);
    }
}
