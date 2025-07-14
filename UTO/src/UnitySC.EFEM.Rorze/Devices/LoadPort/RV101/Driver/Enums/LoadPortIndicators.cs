using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    /// <summary>
    /// List all Rorze Load port indicators
    /// </summary>
    /// <remarks>All load port indicators are not valid for each LP type !</remarks>
    public enum LoadPortIndicators
    {
        /// <summary>
        /// Load indicator. Valid for FOUP and OC.
        /// </summary>
        [Description("Load indicator. Valid for FOUP and OC.")]
        Load,

        /// <summary>
        /// Unload indicator. Valid for FOUP and OC.
        /// </summary>
        [Description("Unload indicator. Valid for FOUP and OC.")]
        Unload,

        /// <summary>
        /// Busy indicator. Valid for OC.
        /// </summary>
        [Description("Busy indicator. Valid for OC.")]
        Busy,

        /// <summary>
        /// Load SW indicator. Valid for SMIF.
        /// </summary>
        [Description("Load SW indicator. Valid for SMIF.")]
        Latch,

        /// <summary>
        /// Error indicator. Valid for FOUP.
        /// </summary>
        [Description("Error indicator. Valid for FOUP.")]
        Error
    }
}
