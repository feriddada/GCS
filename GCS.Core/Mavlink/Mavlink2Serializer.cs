using MavLinkSharp;
using System;
using System.Buffers.Binary;

namespace GCS.Core.Mavlink;

/// <summary>
/// Builds outgoing MAVLink v2 packets using MavLinkSharp's own Metadata
/// (field ordering, payload length, CrcExtra) and Crc utilities.
/// 
/// No hardcoded seeds or manual CRC â€” everything is driven by the same
/// dialect XML that the parser uses, so TX and RX are guaranteed in sync.
/// </summary>
public static class Mavlink2Serializer
{
    private static byte _seq;

    /// <summary>
    /// Generic serializer: looks up the message definition from Metadata,
    /// writes fields in wire order, computes the correct CRC, returns the
    /// complete v2 packet ready to hand to ITransport.SendAsync().
    /// </summary>
    /// <param name="messageId">MAVLink message ID (e.g. 76 = COMMAND_LONG).</param>
    /// <param name="sysId">Sender system ID (GCS).</param>
    /// <param name="compId">Sender component ID (GCS).</param>
    /// <param name="fieldValues">Field name â†’ value, keyed exactly as the XML defines them.</param>
    public static ReadOnlyMemory<byte> Build(
        uint messageId,
        byte sysId,
        byte compId,
        Dictionary<string, object> fieldValues)
    {
        var msg = Metadata.Messages[messageId];   // throws if not loaded â€” fast fail

        // 1) serialise payload in OrderedFields wire order (same logic the parser uses)
        byte[] payload = new byte[msg.PayloadLength];
        var span = payload.AsSpan();

        foreach (var field in msg.OrderedFields)
        {
            if (fieldValues.TryGetValue(field.Name, out var value))
                WriteField(ref span, field, value);
            else
                span = span.Slice(field.Length);   // leave zeroed
        }

        // 2) build the raw packet using the same layout as CreatePacketRaw in the lib's own tests
        unchecked { _seq++; }

        int totalLen = Protocol.V2.HeaderLength + payload.Length + Protocol.V2.ChecksumLength;
        byte[] packet = new byte[totalLen];
        int i = 0;

        packet[i++] = Protocol.V2.StartMarker;          // 0xFD
        packet[i++] = (byte)payload.Length;
        packet[i++] = 0;                                 // incompat flags
        packet[i++] = 0;                                 // compat flags
        packet[i++] = _seq;
        packet[i++] = sysId;
        packet[i++] = compId;
        packet[i++] = (byte)(messageId & 0xFF);  // msg id [0]
        packet[i++] = (byte)((messageId >> 8) & 0xFF); // msg id [1]
        packet[i++] = (byte)((messageId >> 16) & 0xFF); // msg id [2]

        Array.Copy(payload, 0, packet, Protocol.V2.HeaderLength, payload.Length);

        // 3) CRC over everything after STX (header bytes 1..9 + payload), then accumulate CrcExtra
        //    â€” identical to what TryParseV2 validates against
        var crcSpan = packet.AsSpan(1, Protocol.V2.HeaderLength - 1 + payload.Length);
        ushort crc = Crc.Calculate(crcSpan);
        crc = Crc.Accumulate(msg.CrcExtra, crc);

        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(totalLen - 2), crc);

        return packet;
    }

    // â”€â”€ convenience wrappers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>COMMAND_LONG (76)</summary>
    public static ReadOnlyMemory<byte> CommandLong(
        byte targetSys, byte targetComp,
        byte senderSys, byte senderComp,
        ushort command, byte confirmation = 0,
        float p1 = 0, float p2 = 0, float p3 = 0, float p4 = 0,
        float p5 = 0, float p6 = 0, float p7 = 0)
    {
        return Build(76, senderSys, senderComp, new()
        {
            ["target_system"] = targetSys,
            ["target_component"] = targetComp,
            ["command"] = command,
            ["confirmation"] = confirmation,
            ["param1"] = p1,
            ["param2"] = p2,
            ["param3"] = p3,
            ["param4"] = p4,
            ["param5"] = p5,
            ["param6"] = p6,
            ["param7"] = p7,
        });
    }

    /// <summary>SET_MODE (11)</summary>
    public static ReadOnlyMemory<byte> SetMode(
        byte targetSys,
        byte senderSys, byte senderComp,
        byte baseMode, uint customMode)
    {
        return Build(11, senderSys, senderComp, new()
        {
            ["target_system"] = targetSys,
            ["base_mode"] = baseMode,
            ["custom_mode"] = customMode,
        });
    }

    // â”€â”€ field writer (mirrors Field.GetValue / the test helper WriteValue) â”€â”€

    private static void WriteField(ref Span<byte> span, Field field, object value)
    {
        if (field.DataType.IsArray)
        {
            // array fields: write element-by-element
            var arr = (Array)value;
            var elemType = field.ElementType;
            for (int idx = 0; idx < field.ArrayLength; idx++)
            {
                object elem = idx < arr.Length ? arr.GetValue(idx)! : 0;
                WritePrimitive(ref span, elemType, elem);
            }
        }
        else
        {
            WritePrimitive(ref span, field.DataType, value);
        }
    }

    private static void WritePrimitive(ref Span<byte> span, Type type, object value)
    {
        if (type == typeof(char)) { span[0] = (byte)(char)value; span = span.Slice(1); }
        else if (type == typeof(sbyte)) { span[0] = (byte)(sbyte)Convert.ToSByte(value); span = span.Slice(1); }
        else if (type == typeof(byte)) { span[0] = Convert.ToByte(value); span = span.Slice(1); }
        else if (type == typeof(short)) { BinaryPrimitives.WriteInt16LittleEndian(span, Convert.ToInt16(value)); span = span.Slice(2); }
        else if (type == typeof(ushort)) { BinaryPrimitives.WriteUInt16LittleEndian(span, Convert.ToUInt16(value)); span = span.Slice(2); }
        else if (type == typeof(int)) { BinaryPrimitives.WriteInt32LittleEndian(span, Convert.ToInt32(value)); span = span.Slice(4); }
        else if (type == typeof(uint)) { BinaryPrimitives.WriteUInt32LittleEndian(span, Convert.ToUInt32(value)); span = span.Slice(4); }
        else if (type == typeof(float)) { BinaryPrimitives.WriteInt32LittleEndian(span, BitConverter.SingleToInt32Bits(Convert.ToSingle(value))); span = span.Slice(4); }
        else if (type == typeof(long)) { BinaryPrimitives.WriteInt64LittleEndian(span, Convert.ToInt64(value)); span = span.Slice(8); }
        else if (type == typeof(ulong)) { BinaryPrimitives.WriteUInt64LittleEndian(span, Convert.ToUInt64(value)); span = span.Slice(8); }
        else if (type == typeof(double)) { BinaryPrimitives.WriteInt64LittleEndian(span, BitConverter.DoubleToInt64Bits(Convert.ToDouble(value))); span = span.Slice(8); }
        else throw new InvalidOperationException($"Unsupported MAVLink field type: {type}");
    }
}