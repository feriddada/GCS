namespace GCS.AI_CHAT.Models.Analysis;

public class IMUHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public double GyroNoise { get; set; }

    public double AccelerometerNoise { get; set; }

    public bool SensorDrift { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}