using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.AI;
namespace GCS.AI_CHAT.Analyzer;

public class ESCAnalyzer
{
    public ESCHealth Analyze(FlightData data)
    {
        ESCHealth health = new();

        //---------------------------------
        // No Data
        //---------------------------------

        if (data.ESCTemperatures.Count == 0)
        {
            health.Status = "No Data";
            health.HealthScore = 0;

            health.Evidence.Add(
                new Evidence
                {
                    Source = "ESC",
                    Description = "ESC telemetry is not available",
                    Value = 0,
                    Unit = "",
                    Weight = 100
                });

            return health;
        }

        //---------------------------------
        // Raw Values
        //---------------------------------

        health.MaxTemperature =
            data.ESCTemperatures.Max();

        //---------------------------------
        // Health Score
        //---------------------------------

        int score = 100;

        if (health.MaxTemperature > 70)
            score -= 20;

        if (health.MaxTemperature > 85)
            score -= 30;

        health.HealthScore =
            Math.Max(score, 0);

        //---------------------------------
        // Status
        //---------------------------------

        if (score >= 90)
            health.Status = "Excellent";

        else if (score >= 75)
            health.Status = "Good";

        else if (score >= 50)
            health.Status = "Warning";

        else
            health.Status = "Critical";

        //---------------------------------
        // Flags
        //---------------------------------

        health.OverheatDetected =
            health.MaxTemperature > 70;

        health.Warning =
            score < 80;

        health.Critical =
            score < 50;

        //---------------------------------
        // Evidence
        //---------------------------------

        if (health.OverheatDetected)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "ESC",
                    Description = "ESC overheating detected",
                    Value = health.MaxTemperature,
                    Unit = "°C",
                    Weight = 90,
                    Severity = "Warning",

                });
        }

        if (health.Critical)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "ESC",
                    Description = "ESC health is critical",
                    Value = health.HealthScore,
                    Unit = "%",
                    Weight = 100,
                    Severity = "Warning",
                });
        }

        return health;
    }
}