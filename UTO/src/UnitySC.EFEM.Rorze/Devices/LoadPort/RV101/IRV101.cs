using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101
{
    [Device]
    public interface IRV101 : ILoadPort
    {
        #region Statuses

        #region State

        [Status]
        OperationMode OperationMode { get; }

        [Status]
        OriginReturnCompletion OriginReturnCompletion { get; }

        [Status]
        CommandProcessing CommandProcessing { get; }

        [Status]
        OperationStatus OperationStatus { get; }

        [Status]
        bool IsNormalSpeed { get; }

        [Status]
        string MotionSpeedPercentage { get; }

        [Status]
        string ErrorControllerCode { get; }

        [Status]
        ErrorControllerId ErrorControllerName { get; }

        [Status]
        string ErrorCode { get; }

        [Status]
        ErrorCode ErrorDescription { get; }

        #endregion State

        #region Inputs

        [Status(Category = "Inputs", Documentation = "Signal not connected")]
        bool I_EmergencyStop { get; }

        [Status(Category = "Inputs", Documentation = "Signal not connected")]
        bool I_TemporarilyStop { get; }

        [Status(Category = "Inputs", Documentation = "Signal not connected")]
        bool I_VacuumSourcePressure { get; }

        [Status(Category = "Inputs", Documentation = "Signal not connected")]
        bool I_AirSupplySourcePressure { get; }

        [Status(Category = "Inputs")]
        bool I_ProtrusionDetection { get; }

        [Status(Category = "Inputs")]
        bool I_Cover { get; }

        [Status(Category = "Inputs")]
        bool I_DrivePower { get; }

        [Status(Category = "Inputs")]
        bool I_MappingSensor { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_ShutterOpen { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_ShutterClose { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceLeft { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceRight { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceMiddle { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadA { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadB { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadC { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadD { get; }

        [Status(Category = "Inputs")]
        bool I_200mmPresenceLeft { get; }

        [Status(Category = "Inputs")]
        bool I_200mmPresenceRight { get; }

        [Status(Category = "Inputs")]
        bool I_150mmPresenceLeft { get; }

        [Status(Category = "Inputs")]
        bool I_150mmPresenceRight { get; }

        [Status(Category = "Inputs")]
        bool I_AccessSwitch1 { get; }

        [Status(Category = "Inputs")]
        bool I_AccessSwitch2 { get; }

        #endregion Inputs

        #region Outputs

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_PreparationCompleted { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_TemporarilyStop { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_SignificantError { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_LightError { get; }

        [Status(Category = "Outputs")]
        bool O_ClampMovingDirection { get; }

        [Status(Category = "Outputs")]
        bool O_ClampMovingStart { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_ShutterOpen { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_ShutterClose { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_ShutterMotionDisabled { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_ShutterOpen2 { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CoverLock { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CarrierPresenceSensorOn { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_PreparationCompleted2 { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CarrierProperlyPlaced { get; }

        [Status(Category = "Outputs")]
        bool O_AccessSwitch1 { get; }

        [Status(Category = "Outputs")]
        bool O_AccessSwitch2 { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_LOAD_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_UNLOAD_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_PRESENCE_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_PLACEMENT_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_LATCH_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_ERROR_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_BUSY_LED { get; }

        #endregion Outputs

        #region Carrier Presence

        [Status]
        CarrierType CarrierDetectionMode { get; }

        [Status]
        CarrierType CarrierType { get; }

        [Status]
        uint CarrierTypeIndex { get; }

        #endregion Carrier Presence

        #region VersionedDevice

        [Status]
        string Version { get; }

        #endregion

        #endregion Statuses
    }
}
