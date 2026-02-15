

namespace GCS.Core.Mavlink.Commands;

public static class MissionRequestIntCommand
{
    public static byte[] Create(
        ushort seqRequested,
        byte targetSystem,
        byte targetComponent,
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

        buffer[7] = 51; // MISSION_REQUEST_INT
        buffer[8] = 0;
        buffer[9] = 0;

        buffer[10] = (byte)(seqRequested & 0xFF);
        buffer[11] = (byte)(seqRequested >> 8);
        buffer[12] = targetSystem;
        buffer[13] = targetComponent;



        return buffer.ToArray();
    }
}
