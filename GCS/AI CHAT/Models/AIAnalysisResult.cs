using GCS.AI_CHAT.Models.AI;
using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.Inspection;
namespace GCS.AI_CHAT.Models;

public class AIAnalysisResult
{
    //---------------------------------
    // Aircraft Health
    //---------------------------------

    public AircraftHealth Health { get; set; }
        = new();

    //---------------------------------
    // AI Reasoning
    //---------------------------------

    public List<Hypothesis> Hypotheses { get; set; }
        = new();

    //---------------------------------
    // Recommendations
    //---------------------------------

    public List<string> Recommendations { get; set; }
        = new();

    //---------------------------------
    // Failure Prediction
    //---------------------------------

    public FailurePrediction Prediction { get; set; }
        = new();

    //---------------------------------
    // Overall Status
    //---------------------------------
    public InspectionReport Inspection { get; set; }
    = new();

    public bool SafeToFly { get; set; }

    public int OverallHealthScore { get; set; }
}