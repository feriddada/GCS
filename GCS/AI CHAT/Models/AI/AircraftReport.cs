namespace GCS.AI_CHAT.Models.AI
{
    public class AircraftReport
    {
        public string BatteryStatus { get; set; } = "--";

        public string GPSStatus { get; set; } = "--";

        public string FlightMode { get; set; } = "--";

        public string OverallHealth { get; set; } = "--";

        public string CrashRisk { get; set; } = "--";

        public string Recommendation { get; set; } = "--";
    }
}