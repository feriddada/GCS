using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.History;

public class TelemetryHistory
{
    private readonly int maxSamples;

    private readonly List<FlightData> history =
        new();

    public TelemetryHistory(int maxSamples = 100)
    {
        this.maxSamples = maxSamples;
    }

    public void Add(FlightData data)
    {
        history.Add(data);

        if (history.Count > maxSamples)
            history.RemoveAt(0);
    }

    public IReadOnlyList<FlightData> Samples =>
        history;

    public FlightData? Latest =>
        history.LastOrDefault();

    public bool HasEnoughData =>
        history.Count >= 5;
}