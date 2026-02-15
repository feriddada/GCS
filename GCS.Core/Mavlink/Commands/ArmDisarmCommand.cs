

namespace GCS.Core.Mavlink.Commands;

public static class ArmDisarmCommand
{
    private const ushort MAV_CMD_COMPONENT_ARM_DISARM = 400;
    private const byte CRC_EXTRA_COMMAND_LONG = 152;

    public static byte[] Create(
        byte targetSystem,
        byte targetComponent,
        bool arm,
        ref byte sequence)
    {
        Span<byte> buffer = stackalloc byte[45]; // MAVLink v2 COMMAND_LONG

        // --- MAVLink v2 header ---
        buffer[0] = 0xFD;       // magic
        buffer[1] = 33;         // payload length
        buffer[2] = 0;          // incompat flags
        buffer[3] = 0;          // compat flags
        buffer[4] = sequence++; // seq
        buffer[5] = 255;        // sysid (GCS)
        buffer[6] = 190;        // compid (MISSIONPLANNER)

        // message id = 76 (COMMAND_LONG)
        buffer[7] = 76;
        buffer[8] = 0;
        buffer[9] = 0;

        // --- payload ---
        WriteFloat(buffer, 10, arm ? 1f : 0f); // param1
        WriteFloat(buffer, 14, 0f);            // param2
        WriteFloat(buffer, 18, 0f);            // param3
        WriteFloat(buffer, 22, 0f);            // param4
        WriteFloat(buffer, 26, 0f);            // param5
        WriteFloat(buffer, 30, 0f);            // param6
        WriteFloat(buffer, 34, 0f);            // param7

        buffer[38] = (byte)(MAV_CMD_COMPONENT_ARM_DISARM & 0xFF);
        buffer[39] = (byte)(MAV_CMD_COMPONENT_ARM_DISARM >> 8);
        buffer[40] = targetSystem;
        buffer[41] = targetComponent;
        buffer[42] = 0; // confirmation




        return buffer.ToArray();
    }

    private static void WriteFloat(Span<byte> buffer, int offset, float value)
    {
        BitConverter.TryWriteBytes(buffer.Slice(offset, 4), value);
    }
}
