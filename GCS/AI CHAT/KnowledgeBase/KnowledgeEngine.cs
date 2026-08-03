using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.KnowledgeBase;

public class KnowledgeEngine
{
    private readonly KnowledgeDatabase database =
        new();

    public List<KnowledgeRule> Match(
        List<Evidence> evidences)
    {
        List<KnowledgeRule> matched =
            new();

        foreach (var rule in database.Rules)
        {
            bool ok = true;

            foreach (var required in rule.RequiredEvidence)
            {
                if (!evidences.Any(x =>
                    x.Description.Contains(required,
                    StringComparison.OrdinalIgnoreCase)))
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                matched.Add(rule);
        }

        return matched;
    }
}