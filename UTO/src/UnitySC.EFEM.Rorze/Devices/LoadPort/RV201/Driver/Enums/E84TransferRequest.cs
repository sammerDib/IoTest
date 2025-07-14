namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    /// <summary>
    /// Represents if Load port is ready to Load or unload using OHT.
    /// </summary>
    public enum E84TransferRequest
    {
        /// <summary>
        /// Load sequence requested
        /// </summary>
        LOAD,

        /// <summary>
        /// Unload sequence requested
        /// </summary>
        UNLOAD,

        /// <summary>
        /// Stop sequence requested
        /// </summary>
        STOP
    }
}
