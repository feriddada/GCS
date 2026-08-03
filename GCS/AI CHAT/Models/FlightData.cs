namespace GCS.AI_CHAT.Models;

public class FlightData
{
    //---------------------------------
    // Flight Controller
    //---------------------------------

    public bool FlightControllerConnected { get; set; }

    public string FlightControllerName { get; set; } = "";

    public string Firmware { get; set; } = "";

    //---------------------------------
    // Battery
    //---------------------------------

    public double BatteryVoltage { get; set; }

    public double BatteryCurrent { get; set; }

    public int BatteryRemaining { get; set; }

    //---------------------------------
    // GPS
    //---------------------------------

    public int Satellites { get; set; }

    public double HDOP { get; set; }

    public bool GPSGlitch { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    //---------------------------------
    // Aircraft Attitude
    //---------------------------------

    public double Roll { get; set; }

    public double Pitch { get; set; }

    public double Yaw { get; set; }

    //---------------------------------
    // IMU
    //---------------------------------

    public double AccX { get; set; }

    public double AccY { get; set; }

    public double AccZ { get; set; }

    public double GyroX { get; set; }

    public double GyroY { get; set; }

    public double GyroZ { get; set; }

    //---------------------------------
    // Vibration
    //---------------------------------

    public double VibeX { get; set; }

    public double VibeY { get; set; }

    public double VibeZ { get; set; }

    //---------------------------------
    // ESC
    //---------------------------------

    public List<double> ESCTemperatures { get; set; }
        = new();

    public List<double> ESCVoltages { get; set; }
        = new();

    public List<double> ESCCurrents { get; set; }
        = new();

    //---------------------------------
    // Motor
    //---------------------------------

    public List<double> MotorCurrents { get; set; }
        = new();

    public List<double> MotorRPM { get; set; }
        = new();

    //---------------------------------
    // EKF
    //---------------------------------

    public double EKFVariance { get; set; }

    public bool EKFHealthy { get; set; }

    //---------------------------------
    // Sensors
    //---------------------------------

    public double GyroNoise { get; set; }

    public double AccelerometerNoise { get; set; }

    //---------------------------------
    // Power
    //---------------------------------

    public double PowerVoltage { get; set; }

    public double PowerCurrent { get; set; }

    //---------------------------------
    // Flight
    //---------------------------------

    public bool Armed { get; set; }

    public string FlightMode { get; set; } = "";

    public double FlightTime { get; set; }

    public double Altitude { get; set; }

    public double RelativeAltitude { get; set; }

    public double AirSpeed { get; set; }

    public double GroundSpeed { get; set; }

    public double ClimbRate { get; set; }

    //---------------------------------
    // Telemetry
    //---------------------------------

    public bool TelemetryConnected { get; set; }

    public int RSSI { get; set; }

    //---------------------------------
    // Flight Controller Health
    //---------------------------------

    public double CpuLoad { get; set; }

    public double LoopRate { get; set; }

    public bool Failsafe { get; set; }

    //---------------------------------
    // Environment
    //---------------------------------

    public double WindSpeed { get; set; }

    public double WindDirection { get; set; }
}