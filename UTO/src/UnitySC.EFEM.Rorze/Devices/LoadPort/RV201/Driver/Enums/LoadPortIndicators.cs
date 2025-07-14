using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
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
        /// Manual mode indicator. Valid for FOUP.
        /// </summary>
        [Description("Manual mode indicator. Valid for FOUP.")]
        Manual,

        /// <summary>
        /// Access SW indicator. Valid for FOUP and OC.
        /// </summary>
        [Description("Access SW indicator. Valid for FOUP and OC.")]
        Access,

        /// <summary>
        /// Busy indicator. Valid for OC.
        /// </summary>
        [Description("Busy indicator. Valid for OC.")]
        Busy,

        /// <summary>
        /// Load SW indicator. Valid for SMIF.
        /// </summary>
        [Description("Load SW indicator. Valid for SMIF.")]
        Load_SW,

        /// <summary>
        /// Unload SW indicator. Valid for SMIF.
        /// </summary>
        [Description("Unload SW indicator. Valid for SMIF.")]
        Unload_SW,

        /// <summary>
        /// Auto mode indicator. Valid for FOUP.
        /// </summary>
        [Description("Auto mode indicator. Valid for FOUP.")]
        Auto,

        /// <summary>
        /// Error indicator. Valid for FOUP.
        /// </summary>
        [Description("Error indicator. Valid for FOUP.")]
        Error
    }
}
