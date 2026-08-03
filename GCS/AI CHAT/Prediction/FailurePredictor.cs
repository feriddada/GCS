using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Prediction;

public class FailurePredictor
{
    public FailurePrediction Predict(
        AIAnalysisResult analysis)
    {
        FailurePrediction prediction =
            new();

        //---------------------------------
        // Battery
        //---------------------------------

        if (analysis.Health.Battery != null)
        {
            prediction.BatteryRisk =
                100 - analysis.Health.Battery.HealthScore;
        }

        //---------------------------------
        // Motor
        //---------------------------------

        if (analysis.Health.Motor != null)
        {
            prediction.MotorRisk =
                100 - analysis.Health.Motor.HealthScore;
        }

        //---------------------------------
        // ESC
        //---------------------------------

        if (analysis.Health.ESC != null)
        {
            prediction.ESCRisk =
                100 - analysis.Health.ESC.HealthScore;
        }

        //---------------------------------
        // Crash Risk
        //---------------------------------

        prediction.CrashRisk =
            (prediction.BatteryRisk +
             prediction.MotorRisk +
             prediction.ESCRisk) / 3;

        //---------------------------------
        // Comment
        //---------------------------------

        if (prediction.CrashRisk >= 70)
        {
            prediction.Comment =
                "High crash probability.";

            prediction.Recommendation =
                "Land immediately and inspect the aircraft.";
        }
        else if (prediction.CrashRisk >= 40)
        {
            prediction.Comment =
                "Medium crash probability.";

            prediction.Recommendation =
                "Monitor aircraft carefully.";
        }
        else
        {
            prediction.Comment =
                "Aircraft risk is low.";

            prediction.Recommendation =
                "Aircraft is ready for flight.";
        }

        return prediction;
    }
}