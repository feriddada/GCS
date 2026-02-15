namespace GCS.Core.Validation;

public sealed record ValidationError(
    string Code,
    string Message
);
