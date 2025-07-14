using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums
{
    public enum LoadPortIndicators
    {
        /// <summary>
        ///     Load indicator.
        /// </summary>
        [Description("Load indicator.")] Load,

        /// <summary>
        ///     Unload indicator.
        /// </summary>
        [Description("Unload indicator.")] Unload,

        /// <summary>
        ///     Access SW indicator.
        /// </summary>
        [Description("Access SW indicator.")] Access,

        /// <summary>
        ///     Green indicator.
        /// </summary>
        [Description("Green indicator.")] Green,

        /// <summary>
        ///     Red indicator.
        /// </summary>
        [Description("Red indicator.")] Red
    }
}
