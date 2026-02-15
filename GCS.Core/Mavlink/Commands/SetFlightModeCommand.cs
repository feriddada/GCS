using System.Buffers.Binary;
using MavLinkSharp;

namespace GCS.Core.Mavlink.Commands;

public static class SetFlightModeCommand
{
    private const byte CRC_EXTRA = 152; // COMMAND_LONG

    public static byte[] Create(
        byte targetSystem,
        byte targetComponent,
        uint customMode,
        ref byte seq)
    {
        Span<byte> buffer = stackalloc byte[45];

        // MAVLink v2 header
        buffer[0] = 0xFD;
        buffer[1] = 33; // payload length
        buffer[2] = 0;
        buffer[3] = 0;
        buffer[4] = seq++;
        buffer[5] = 255; // GCS
        buffer[6] = 190;

        buffer[7] = 76; // COMMAND_LONG
        buffer[8] = 0;
        buffer[9] = 0;

        // payload
        BinaryPrimitives.WriteSingleLittleEndian(buffer[10..14], customMode);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[14..18], 0);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[18..22], 0);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[22..26], 0);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[26..30], 0);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[30..34], 0);
        BinaryPrimitives.WriteSingleLittleEndian(buffer[34..38], 0);

        BinaryPrimitives.WriteUInt16LittleEndian(buffer[38..40], 176);
        buffer[40] = targetSystem;
        buffer[41] = targetComponent;
        buffer[42] = 0;

        // CRC
        var crcSpan = buffer.Slice(1, 42);
        ushort crc = Crc.Calculate(crcSpan);
        crc = Crc.Accumulate(CRC_EXTRA, crc);

        buffer[43] = (byte)(crc & 0xFF);
        buffer[44] = (byte)(crc >> 8);

        return buffer.ToArray();
    }
}
