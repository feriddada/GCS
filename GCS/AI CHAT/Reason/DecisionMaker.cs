using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.Reasoning;

public class DecisionMaker
{
    public Experience Decide(List<Experience> list)
    {
        return list
            .GroupBy(x => x.Cause)
            .OrderByDescending(x => x.Count())
            .First()
            .First();
    }
}