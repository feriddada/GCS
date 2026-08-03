using GCS.AI_CHAT.Models;
using GCS.Core.Domain;
using GCS.Core.State;

namespace GCS.AI_CHAT.Mapper;

public class VehicleStateMapper
{
    public FlightData Map(
        VehicleState state)
    {
        FlightData data =
            new();

        //---------------------------------
        // Connection
        //---------------------------------

        data.FlightControllerConnected =
            state.Connection?.IsConnected ?? false;

        //---------------------------------
        // Battery
        //---------------------------------

        if (state.Battery != null)
        {
            data.BatteryVoltage =
                state.Battery.VoltageVolts;

            data.BatteryCurrent =
                state.Battery.CurrentAmps;

            data.BatteryRemaining =
                state.Battery.RemainingPercent;
        }

        //---------------------------------
        // GPS
        //---------------------------------

        if (state.Gps != null)
        {
            data.Satellites =
                state.Gps.SatellitesVisible;

            data.HDOP =
                state.Gps.HdopMeters;
        }

        //---------------------------------
        // Position
        //---------------------------------

        if (state.Position != null)
        {
            data.Latitude =
    state.Position.LatitudeDeg;

            data.Longitude =
                state.Position.LongitudeDeg;

            data.Altitude =
                state.Position.AltitudeRelMeters;
        }

        //---------------------------------
        // Attitude
        //---------------------------------

        if (state.Attitude != null)
        {
            data.Roll =
     state.Attitude.RollRad * 180.0 / Math.PI;

            data.Pitch =
                state.Attitude.PitchRad * 180.0 / Math.PI;

            data.Yaw =
                state.Attitude.YawRad * 180.0 / Math.PI;
        }

        //---------------------------------
        // Flight
        //---------------------------------

        data.Armed =
            state.IsArmed;

        data.FlightMode =
       state.FlightMode?.ToString() ?? "";

        return data;
    }
}