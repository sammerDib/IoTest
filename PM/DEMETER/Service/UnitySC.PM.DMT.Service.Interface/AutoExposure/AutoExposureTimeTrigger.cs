using System.ComponentModel;

namespace UnitySC.PM.DMT.Service.Interface.AutoExposure
{
    /// <summary>
    /// Define when the auto exposure time trigger is set for a foop
    /// </summary>
    public enum AutoExposureTimeTrigger
    {
        [Description("On the first wafer of a lot")]
        OnFirstWaferOfLot,

        [Description("On all wafer")]
        OnAllWafer,

        [Description("Never")]
        Never
    }
}
