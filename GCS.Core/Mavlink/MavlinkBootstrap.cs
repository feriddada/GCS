using MavLinkSharp;

namespace GCS.Core.Mavlink;

public static class MavlinkBootstrap
{
    public static void Init()
    {
        MavLink.Initialize("common.xml");

        MavLink.IncludeMessages(new uint[]
        {
            0,   // HEARTBEAT
            1,   // SYS_STATUS
            11,  // SET_MODE
            30,  // ATTITUDE
            33,  // GLOBAL_POSITION_INT
            
            // Mission protocol
            39,
            40,
            41,  // MISSION_SET_CURRENT
            43,  // MISSION_REQUEST_LIST
            44,  // MISSION_COUNT
            45,  // MISSION_CLEAR_ALL
            47,  // MISSION_ACK
            51,  // MISSION_REQUEST_INT
            73,  // MISSION_ITEM_INT
            
            65,  // RC_CHANNELS
            74,  // VFR_HUD
            76,  // COMMAND_LONG
            77,  // COMMAND_ACK
            253  // STATUSTEXT
        });
    }
}