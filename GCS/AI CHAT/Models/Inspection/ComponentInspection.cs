namespace GCS.AI_CHAT.Models.Inspection;

public class ComponentInspection
{
    public string Name { get; set; } = "";

    public DataState State { get; set; }

    public bool CanAnalyze =>
        State == DataState.Available;

    public string Reason { get; set; } = "";
}