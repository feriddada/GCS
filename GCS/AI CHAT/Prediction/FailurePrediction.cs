namespace GCS.AI_CHAT.Models.AI;

public class FailurePrediction
{
    //---------------------------------
    // Risks
    //---------------------------------

    public int CrashRisk { get; set; }

    public int BatteryRisk { get; set; }

    public int MotorRisk { get; set; }

    public int ESCRisk { get; set; }

    //---------------------------------
    // AI Result
    //---------------------------------

    public string Comment { get; set; } = "";

    public string Recommendation { get; set; } = "";
}