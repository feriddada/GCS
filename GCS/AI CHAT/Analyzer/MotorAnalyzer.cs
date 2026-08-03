using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.AI;
namespace GCS.AI_CHAT.Analyzer;

public class MotorAnalyzer
{
    public MotorHealth Analyze(FlightData data)
    {
        MotorHealth health = new();

        if (data.MotorCurrents.Count == 0)
            return health;

        health.AverageCurrent =
            data.MotorCurrents.Average();

        health.MaxCurrent =
            data.MotorCurrents.Max();

        health.CurrentDifference =
            health.MaxCurrent -
            health.AverageCurrent;

        health.ImbalanceDetected =
            health.CurrentDifference > 3;

        int score = 100;

        if (health.ImbalanceDetected)
            score -= 25;

        if (health.CurrentDifference > 6)
            score -= 25;

        health.HealthScore = Math.Max(score, 0);

        if (score >= 90)
            health.Status = "Excellent";
        else if (score >= 75)
            health.Status = "Good";
        else if (score >= 50)
            health.Status = "Warning";
        else
            health.Status = "Critical";

        health.Warning =
            score < 75;

        health.Critical =
            score < 50;

        if (health.ImbalanceDetected)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "Motor",
                    Description = "Motor current imbalance",
                    Value = health.CurrentDifference,
                    Unit = "A",
                    Weight = 95,
                    Severity = "Warning"
                });
        }

        return health;
    }
}