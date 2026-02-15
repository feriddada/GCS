
namespace GCS.Core.Mavlink.Commands;

public static class MissionClearAllCommand
{
    public static byte[] Create(
        byte targetSystem,
        byte targetComponent,
        ref byte seq)
    {
        Span<byte> buffer = stackalloc byte[17];

        buffer[0] = 0xFD;
        buffer[1] = 2;
        buffer[2] = 0;
        buffer[3] = 0;
        buffer[4] = seq++;
        buffer[5] = 255;
        buffer[6] = 190;

        buffer[7] = 45; // MISSION_CLEAR_ALL
        buffer[8] = 0;
        buffer[9] = 0;

        buffer[10] = targetSystem;
        buffer[11] = targetComponent;



        return buffer.ToArray();
    }
}
