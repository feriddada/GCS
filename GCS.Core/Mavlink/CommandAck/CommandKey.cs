namespace GCS.Core.Mavlink.CommandAck;

internal readonly record struct CommandKey(
    ushort CommandId,
    byte SystemId,
    byte ComponentId
);
