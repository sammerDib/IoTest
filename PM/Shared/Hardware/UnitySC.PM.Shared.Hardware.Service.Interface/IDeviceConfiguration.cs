namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public interface IDeviceIdentification
    {

        /// <summary>
        /// Device name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Device ID
        /// </summary>
        string DeviceID { get; set; }
    }


    public interface IDeviceConfiguration : IDeviceIdentification
    {
        /// <summary>
        /// True if the device is enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// True if the device is simulated
        /// </summary>
        bool IsSimulated { get; set; }

        /// <summary>
        /// Device log level
        /// </summary>
        DeviceLogLevel LogLevel { get; set; }

    }

    public enum DeviceLogLevel
    {
        None = 0,
        Verbose,
        Debug,
        Information,
        Warning,
        Error
    }
}
