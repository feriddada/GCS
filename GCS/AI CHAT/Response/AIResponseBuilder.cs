using System.Text;
using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.Response;

public class AIResponseBuilder
{
    public string Build(AIAnalysisResult result)
    {
        StringBuilder text =
            new();

        //---------------------------------
        // HEADER
        //---------------------------------

        text.AppendLine("========== AI AIRCRAFT ANALYSIS ==========");
        text.AppendLine();

        //---------------------------------
        // INSPECTION
        //---------------------------------

        text.AppendLine("SYSTEM INSPECTION");
        text.AppendLine("--------------------------------");

        foreach (var component in result.Inspection.Components)
        {
            string state = component.CanAnalyze
                ? "OK"
                : "UNAVAILABLE";

            text.AppendLine(
                $"{component.Name} : {state}");

            if (!component.CanAnalyze &&
                !string.IsNullOrWhiteSpace(component.Reason))
            {
                text.AppendLine(
                    $"   Reason : {component.Reason}");
            }
        }

        text.AppendLine();

        //---------------------------------
        // HEALTH
        //---------------------------------

        text.AppendLine("AIRCRAFT HEALTH");
        text.AppendLine("--------------------------------");

        text.AppendLine(
            $"Overall Health : {result.OverallHealthScore}%");

        text.AppendLine(
            $"Safe To Fly : {(result.SafeToFly ? "YES" : "NO")}");

        text.AppendLine();

        //---------------------------------
        // BATTERY
        //---------------------------------

        if (result.Health.Battery != null)
        {
            text.AppendLine("Battery");

            text.AppendLine(
                $"Status : {result.Health.Battery.Status}");

            text.AppendLine(
                $"Health : {result.Health.Battery.HealthScore}%");

            text.AppendLine();
        }

        //---------------------------------
        // MOTOR
        //---------------------------------

        if (result.Health.Motor != null)
        {
            text.AppendLine("Motor");

            text.AppendLine(
                $"Status : {result.Health.Motor.Status}");

            text.AppendLine(
                $"Health : {result.Health.Motor.HealthScore}%");

            text.AppendLine();
        }

        //---------------------------------
        // ESC
        //---------------------------------

        if (result.Health.ESC != null)
        {
            text.AppendLine("ESC");

            text.AppendLine(
                $"Status : {result.Health.ESC.Status}");

            text.AppendLine(
                $"Health : {result.Health.ESC.HealthScore}%");

            text.AppendLine();
        }

        //---------------------------------
        // DETECTED PROBLEMS
        //---------------------------------

        text.AppendLine("DETECTED PROBLEMS");
        text.AppendLine("--------------------------------");

        if (result.Hypotheses.Count == 0)
        {
            text.AppendLine(
                "No problems detected.");
        }
        else
        {
            foreach (var hypothesis in result.Hypotheses)
            {
                text.AppendLine();

                text.AppendLine(
                    $"Problem : {hypothesis.Name}");

                text.AppendLine(
                    $"Confidence : {hypothesis.Confidence}%");

                text.AppendLine(
                    $"Reason : {hypothesis.Reason}");
            }
        }

        text.AppendLine();

        //---------------------------------
        // PREDICTION
        //---------------------------------

        text.AppendLine("AI PREDICTION");
        text.AppendLine("--------------------------------");

        text.AppendLine(
            $"Crash Risk : {result.Prediction.CrashRisk}%");

        text.AppendLine(
            $"Recommendation : {result.Prediction.Recommendation}");

        text.AppendLine();

        //---------------------------------
        // RECOMMENDATIONS
        //---------------------------------

        text.AppendLine("RECOMMENDATIONS");
        text.AppendLine("--------------------------------");

        if (result.Recommendations.Count == 0)
        {
            text.AppendLine(
                "No recommendation.");
        }
        else
        {
            foreach (var recommendation in result.Recommendations)
            {
                text.AppendLine(
                    $"• {recommendation}");
            }
        }

        return text.ToString();
    }
}