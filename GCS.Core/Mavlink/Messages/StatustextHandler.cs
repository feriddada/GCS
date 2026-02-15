using GCS.Core.Domain;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Diagnostics;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles STATUSTEXT (msg 253) - Autopilot messages.
/// </summary>
public sealed class StatustextHandler : IMavlinkMessageHandler
{
    public uint MessageId => 253;

    private readonly Action<AutopilotMessage> _onMessage;

    public StatustextHandler(Action<AutopilotMessage> onMessage)
    {
        _onMessage = onMessage;
    }

    public void Handle(Frame frame)
    {
        try
        {
            byte severityByte = Convert.ToByte(frame.Fields["severity"]);
            string text = ExtractText(frame.Fields["text"]);

            if (string.IsNullOrWhiteSpace(text)) return;

            var severity = severityByte switch
            {
                0 or 1 or 2 => AutopilotMessageSeverity.Critical,
                3 => AutopilotMessageSeverity.Error,
                4 => AutopilotMessageSeverity.Warning,
                _ => AutopilotMessageSeverity.Info
            };

            // Log to debug output so we can see messages even if UI fails
            Debug.WriteLine($"[STATUSTEXT] [{severity}] {text}");

            _onMessage(new AutopilotMessage(severity, text, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[StatustextHandler] Error: {ex.Message}");
        }
    }

    private static string ExtractText(object? field)
    {
        if (field == null) return string.Empty;

        return field switch
        {
            string s => s.TrimEnd('\0'),
            char[] c => new string(c).TrimEnd('\0'),
            byte[] b => System.Text.Encoding.ASCII.GetString(b).TrimEnd('\0'),
            _ => field.ToString()?.TrimEnd('\0') ?? string.Empty
        };
    }
}