namespace GCS.AI_CHAT.Models;

using GCS.AI_CHAT.Models.AI;
public class Evidence
{
    // Battery
    // Motor
    // ESC
    // EKF
    // IMU
    public string Source { get; set; } = "";

    // Human readable
    public string Description { get; set; } = "";

    // Measured value
    public double Value { get; set; }

    // %, A, °C, m/s...
    public string Unit { get; set; } = "";

    // 0-100
    public int Weight { get; set; }

    // Info / Warning / Critical
    public string Severity { get; set; } = "";

    // Flight time (log üçün)
    public double Time { get; set; }

    // Sonradan hansı log mesajından gəldiyini saxlayacağıq
    public string Message { get; set; } = "";
}