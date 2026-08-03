using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Live;

public class AIMessageGenerator
{
    public List<AIMessage> Generate(
        AIAnalysisResult result)
    {
        List<AIMessage> messages =
            new();

        foreach (var hypothesis in result.Hypotheses)
        {
            messages.Add(
                new AIMessage
                {
                    Title = hypothesis.Name,

                    Message = hypothesis.Reason,

                    Confidence =
                        hypothesis.Confidence,

                    Priority =
                        hypothesis.Confidence,

                    RequiresAttention =
                        hypothesis.Confidence > 70
                });
        }

        return messages;
    }
}