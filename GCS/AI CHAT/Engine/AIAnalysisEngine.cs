using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Prediction;
using GCS.AI_CHAT.Reasoning;
using GCS.AI_CHAT.Recommendation;
using GCS.AI_CHAT.Inspector;
namespace GCS.AI_CHAT.Engine;

public class AIAnalysisEngine
{
    //---------------------------------
    // Engines
    //---------------------------------

    private readonly AircraftHealthEngine healthEngine =
        new();

    private readonly HypothesisEngine hypothesisEngine =
        new();

    private readonly RecommendationEngine recommendationEngine =
        new();

    private readonly FailurePredictor failurePredictor =
        new();

    //---------------------------------
    // Main Analysis
    //---------------------------------
    private readonly SystemInspector inspector =
        new();
    public AIAnalysisResult Analyze(FlightData data)
    {
        AIAnalysisResult result =
            new();
        //---------------------------------
        // System Inspection
        //---------------------------------

        result.Inspection =
            inspector.Inspect(data);

        //---------------------------------
        // Aircraft Health
        //---------------------------------

        result.Health =
            healthEngine.Analyze(
                data,
                result.Inspection);

        CalculateOverallHealth(result);

        //---------------------------------
        // Safe To Fly
        //---------------------------------

        result.SafeToFly =
            result.OverallHealthScore >= 70;

        //---------------------------------
        // AI Reasoning
        //---------------------------------

        result.Hypotheses =
            hypothesisEngine.Analyze(
                result.Health.AllEvidence);

        //---------------------------------
        // Recommendations
        //---------------------------------

        result.Recommendations =
            recommendationEngine.Generate(
                result.Hypotheses);

        //---------------------------------
        // Failure Prediction
        //---------------------------------

        result.Prediction =
            failurePredictor.Predict(result);

        return result;
    }

    //---------------------------------
    // Overall Health
    //---------------------------------

    private void CalculateOverallHealth(
        AIAnalysisResult result)
    {
        List<int> scores =
            new();

        if (result.Health.Battery != null)
            scores.Add(result.Health.Battery.HealthScore);

        if (result.Health.Motor != null)
            scores.Add(result.Health.Motor.HealthScore);

        if (result.Health.ESC != null)
            scores.Add(result.Health.ESC.HealthScore);

        if (scores.Count == 0)
        {
            result.OverallHealthScore = 0;
            return;
        }

        result.OverallHealthScore =
            (int)scores.Average();
    }
}