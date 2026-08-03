using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.Mapper;

public class FlightDataMapper
{
    public FlightData Map(UAVState state)
    {
        FlightData data = new();

        //---------------------------------
        // Battery
        //---------------------------------

        data.BatteryVoltage = state.BatteryVoltage;

        //---------------------------------
        // GPS
        //---------------------------------

        data.Satellites = state.GpsSatellites;

        //---------------------------------
        // Aircraft
        //---------------------------------

        data.Roll = state.Roll;
        data.Pitch = state.Pitch;

        //---------------------------------
        // Flight
        //---------------------------------

        data.Altitude = state.Altitude;

        return data;
    }
}