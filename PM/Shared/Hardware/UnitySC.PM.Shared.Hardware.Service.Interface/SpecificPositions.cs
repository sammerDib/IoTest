using System.ComponentModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public enum SpecificPositions
    {
        [Description("Park")]
        PositionPark,
        [Description("Manual load")]
        PositionManualLoad,
        [Description("Chuck center")]
        PositionChuckCenter,
        [Description("Home")]
        PositionHome,
        [Description("Wafer center")]
        PositionWaferCenter
    }
}
