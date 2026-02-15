using MavLinkSharp;
using System;
using System.Collections.Generic;

namespace GCS.Core.Mavlink.Dispatch;

public sealed class MavlinkDispatcher
{
    private readonly Dictionary<uint, IMavlinkMessageHandler> _handlers;

    public MavlinkDispatcher(IEnumerable<IMavlinkMessageHandler> handlers)
    {
        _handlers = new Dictionary<uint, IMavlinkMessageHandler>();

        foreach (var handler in handlers)
        {
            if (_handlers.ContainsKey(handler.MessageId))
                throw new InvalidOperationException(
                    $"Duplicate MAVLink handler for message {handler.MessageId}");

            _handlers[handler.MessageId] = handler;
        }
    }

    public void Dispatch(Frame frame)
    {
        if (_handlers.TryGetValue(frame.MessageId, out var handler))
        {
            handler.Handle(frame);
        }
        // else: message silently ignored
    }
}
