

namespace GCS.Core.Mavlink.Commands;

public static class MissionCountCommand
{
    public static byte[] Create(
        byte targetSystem,
        byte targetComponent,
        ushort count,
        ref byte seq)
    {
        Span<byte> buffer = stackalloc byte[19];

        buffer[0] = 0xFD;
        buffer[1] = 4;
        buffer[2] = 0;
        buffer[3] = 0;
        buffer[4] = seq++;
        buffer[5] = 255;
        buffer[6] = 190;

        buffer[7] = 44; // MISSION_COUNT
        buffer[8] = 0;
        buffer[9] = 0;

        buffer[10] = (byte)(count & 0xFF);
        buffer[11] = (byte)(count >> 8);
        buffer[12] = targetSystem;
        buffer[13] = targetComponent;


        return buffer.ToArray();
    }
}
