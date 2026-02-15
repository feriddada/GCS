using GCS.Core.Domain;
using System;

namespace GCS.Core.Mavlink.Commands;

public static class MissionItemIntCommand
{
    public static byte[] Create(
        MissionItem item,
        byte targetSystem,
        byte targetComponent,
        ref byte seq)
    {
        Span<byte> buffer = stackalloc byte[37];

        buffer[0] = 0xFD;
        buffer[1] = 28;
        buffer[2] = 0;
        buffer[3] = 0;
        buffer[4] = seq++;
        buffer[5] = 255;
        buffer[6] = 190;

        buffer[7] = 73; // MISSION_ITEM_INT
        buffer[8] = 0;
        buffer[9] = 0;

        BitConverter.TryWriteBytes(buffer.Slice(10, 2), (ushort)item.Sequence);
        buffer[12] = 3; // MAV_FRAME_GLOBAL_RELATIVE_ALT_INT

        BitConverter.TryWriteBytes(buffer.Slice(13, 4), (int)(item.LatitudeDeg * 1e7));
        BitConverter.TryWriteBytes(buffer.Slice(17, 4), (int)(item.LongitudeDeg * 1e7));
        BitConverter.TryWriteBytes(buffer.Slice(21, 4), item.AltitudeMeters);

        BitConverter.TryWriteBytes(buffer.Slice(25, 2), item.Command);

        buffer[27] = (byte)(item.AutoContinue ? 1 : 0);
        buffer[28] = targetSystem;
        buffer[29] = targetComponent;


        return buffer.ToArray();
    }
}
