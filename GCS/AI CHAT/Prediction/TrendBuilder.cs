using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Prediction;

public class TrendBuilder
{
    public List<TrendPoint> BatteryVoltage(
        IReadOnlyList<FlightData> history)
    {
        List<TrendPoint> points =
            new();

        foreach (var item in history)
        {
            points.Add(
                new TrendPoint
                {
                    Value =
                        item.BatteryVoltage
                });
        }

        return points;
    }
}