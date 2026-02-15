namespace GCS.Core.Domain;

public enum MissionTransferState
{
    Idle,
    Uploading,
    Downloading,
    Completed,
    Failed
}

public record MissionState(
    MissionTransferState State,
    int Progress,
    int Total,
    string? ErrorMessage
);
