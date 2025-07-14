using System;

namespace UnitySC.Shared.Data.Enum
{
    public enum IoType
    {
        Digital,
        Analogic
    }

    public enum AttributeType
    {
        DigitalIO,
        AnalogicIO,
        Status,
        Message,
        Other,
        StructDataType
    }

    /// <summary>
    /// Overload on the boolean I/O value so extensive information can be retrieved
    /// </summary>
    public enum IoValue
    {
        Error = -1,
        False = 0,
        True = 1,
        Unknown = 2,
    };

    /// <summary>
    /// Defines the interface of the IO Manager
    /// </summary>
    public interface IoStatusInterface
    {
        /// <summary>
        /// Gets wafer presence state
        /// </summary>
        IoValue IsWaferPresent { get; }

        /// <summary>
        /// Gets the FFU fan status
        /// </summary>
        IoValue IsFFUFanOK { get; }

        /// <summary>
        /// Gets the FFU filter clogging status
        /// </summary>
        ///
        IoValue IsInterlockOK { get; }

        /// <summary>
        /// Gets the maintenance status
        /// </summary>
        IoValue IsInMaintenance { get; }
    }

    /// <summary>
    /// Lists the possible handling types
    /// </summary>
    public enum HandlingType { htStandard = 0, htStandardSlit, htRorzeFlipper };

    /// <summary>
    /// Lists the different wafer positioner statuses
    /// </summary>
    public enum WaferPositionerStatus { wpOK, wpError, wpUnknown };

    /// <summary>
    /// Lists the possible sensor configurations
    /// </summary>
    [Flags]
    public enum SensorID { None = 1, Frontside = 2, Backside = 4 }

    public enum OnOffUnk { Unknown = 0, On, Off }

    public enum ChuckPosition { UnknownPosition = 0, ProcessPosition, LoadingUnloadingPosition }

    public enum SlitDoorPosition { UnknownPosition = 0, OpenPosition, ClosePosition, ErrorPosition }
    public enum SlitDoorValidationState { BadConfiguration = 0, InterfaceNotImplemented, Operational_DoorAvailable, Operational_DoorDisabled }

    public enum WaferSize { Unknown = 0, _50mm, _150mm, _200mm, _300mm }


}
