namespace GCS.AI_CHAT.Models;

public class LiveAIItem
{
    public DateTime Time { get; set; }
        = DateTime.Now;

    public string Component { get; set; } = "";

    public string Title { get; set; } = "";

    public string Message { get; set; } = "";

    public int Confidence { get; set; }

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}