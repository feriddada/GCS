using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.Models.AI;

public class Hypothesis
{
    // Məsələn:
    // Propeller Imbalance
    // ESC Failure
    // Battery Failure
    public string Name { get; set; } = "";

    // AI niyə belə düşündü
    public string Reason { get; set; } = "";

    // 0-100
    public int Confidence { get; set; }

    // Bu nəticəyə hansı sübutlarla gəldi
    public List<Evidence> Evidence { get; set; }
        = new();
}