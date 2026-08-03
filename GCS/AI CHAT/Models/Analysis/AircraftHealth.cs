using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.AI;
using GCS.AI_CHAT.Models;
public class AircraftHealth
{
    public BatteryHealth Battery { get; set; } = new();

    public MotorHealth Motor { get; set; } = new();

    public ESCHealth ESC { get; set; } = new();

    public VibrationHealth Vibration { get; set; } = new();

    public EKFHealth EKF { get; set; } = new();

    public IMUHealth IMU { get; set; } = new();

    public PowerHealth Power { get; set; } = new();

    public FlightControllerHealth FlightController { get; set; } = new();

    public int OverallHealthScore { get; set; }
    public List<Evidence> AllEvidence { get; set; }
    = new();
}