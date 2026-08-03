using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.AI;
namespace GCS.AI_CHAT.Analyzer;

public class BatteryAnalyzer
{
    public BatteryHealth Analyze(FlightData data)
    {
        BatteryHealth health = new();

        //---------------------------------
        // Telemetry Check
        //---------------------------------

        System.Diagnostics.Debug.WriteLine(
            $"BatteryAnalyzer -> Voltage={data.BatteryVoltage}, Remaining={data.BatteryRemaining}");

        bool telemetryAvailable =
            data.BatteryVoltage > 5.0 &&
            data.BatteryRemaining >= 0;

        if (!telemetryAvailable)
        {
            health.Status = "No Telemetry";

            health.HealthScore = 100;

            health.Warning = false;

            health.Critical = false;

            return health;
        }
        //---------------------------------
        // Raw Values
        //---------------------------------

        health.Voltage = data.BatteryVoltage;
        health.Current = data.BatteryCurrent;
        health.Remaining = data.BatteryRemaining;

        //---------------------------------
        // Voltage Sag
        //---------------------------------
        health.VoltageSag =
            telemetryAvailable &&
            data.BatteryVoltage < 13.5;

        //---------------------------------
        // Health Score
        //---------------------------------

        int score = 100;
        if (telemetryAvailable &&
            data.BatteryRemaining < 30)

            if (telemetryAvailable &&
    data.BatteryVoltage < 14)

                if (health.VoltageSag)
            score -= 15;

        if (score < 0)
            score = 0;

        health.HealthScore = score;

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
        // Warning
        //---------------------------------

        health.Warning =
            score < 75;

        //---------------------------------
        // Critical
        //---------------------------------

        health.Critical =
            score < 50;

        //---------------------------------
        // Evidence
        //---------------------------------

        if (health.VoltageSag)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "Battery",
                    Description = "Voltage sag detected",
                    Value = health.Voltage,
                    Unit = "V",
                    Weight = 85,
                    Severity = "Warning"
                });
        }

        if (health.Remaining < 30)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "Battery",
                    Description = "Battery capacity is low",
                    Value = health.Remaining,
                    Unit = "%",
                    Weight = 90,
                    Severity = "Warning"
                });
        }

        if (health.Critical)
        {
            health.Evidence.Add(
                new Evidence
                {
                    Source = "Battery",
                    Description = "Battery health is critical",
                    Value = health.HealthScore,
                    Unit = "%",
                    Weight = 100,
                    Severity = "Critical"
                });
        }

        return health;
    }
}