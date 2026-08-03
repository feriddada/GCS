namespace GCS.AI_CHAT.Models;

public class UAVState
{
    //---------------------------------
    // Battery
    //---------------------------------

    public double BatteryVoltage { get; set; }

    public double BatteryCurrent { get; set; }

    public int BatteryRemaining { get; set; }

    //---------------------------------
    // GPS
    //---------------------------------

    public int GpsSatellites { get; set; }

    public double HDOP { get; set; }

    //---------------------------------
    // Attitude
    //---------------------------------

    public double Roll { get; set; }

    public double Pitch { get; set; }

    public double Yaw { get; set; }

    //---------------------------------
    // Flight
    //---------------------------------

    public double Altitude { get; set; }

    public double AirSpeed { get; set; }

    public double GroundSpeed { get; set; }

    //---------------------------------
    // IMU
    //---------------------------------

    public double VibeX { get; set; }

    public double VibeY { get; set; }

    public double VibeZ { get; set; }

    //---------------------------------
    // Motor
    //---------------------------------

    public List<double> MotorCurrents { get; set; }
        = new();

    //---------------------------------
    // ESC
    //---------------------------------

    public List<double> ESCTemperatures { get; set; }
        = new();
}