using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Prediction;

public class TrendAnalyzer
{
    public TrendResult Analyze(
        List<TrendPoint> values)
    {
        TrendResult result = new();

        if (values.Count < 2)
        {
            result.Stable = true;
            result.Comment =
                "Not enough data.";

            return result;
        }

        double first =
            values.First().Value;

        double last =
            values.Last().Value;

        result.Change =
            last - first;

        if (result.Change > 0.5)
        {
            result.Rising = true;
            result.Comment =
                "Increasing trend detected.";
        }
        else if (result.Change < -0.5)
        {
            result.Falling = true;
            result.Comment =
                "Decreasing trend detected.";
        }
        else
        {
            result.Stable = true;
            result.Comment =
                "Stable trend.";
        }

        return result;
    }
}