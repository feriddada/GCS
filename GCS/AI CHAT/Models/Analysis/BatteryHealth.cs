
using GCS.AI_CHAT.Models.AI;
using GCS.AI_CHAT.Models;
public class BatteryHealth
{
    public int HealthScore { get; set; }

    public string Status { get; set; } = "";

    public double Voltage { get; set; }

    public double Current { get; set; }

    public int Remaining { get; set; }

    public bool VoltageSag { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
    public List<Evidence> Evidence { get; set; }
    = new();
}