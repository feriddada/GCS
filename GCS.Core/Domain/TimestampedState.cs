namespace GCS.Core.Domain;

public abstract record TimestampedState(
    DateTime TimestampUtc
);
