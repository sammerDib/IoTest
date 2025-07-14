using System.ComponentModel;

namespace UnitySC.PM.LIGHTSPEED.Data.Enum
{
    /// <summary>
    /// Define when the auto exposure time trigger is set for a foop
    /// </summary>
    public enum AutoExposureTimeTrigger
    {
        [Description("On the first wafer of a lot")]
        OnFirstWaferOfLot = 0,

        [Description("On all wafer")]
        OnAllWafer = 1,

        [Description("Never")]
        Never = 2
    }
}
