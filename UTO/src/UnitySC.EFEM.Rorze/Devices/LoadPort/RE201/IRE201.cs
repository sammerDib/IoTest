using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.SmifLoadPort;

using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IRE201 : ISmifLoadPort
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

        #endregion

        #region Inputs

        [Status(Category = "Inputs")]
        bool I_SubstrateDetection { get; }

        [Status(Category = "Inputs")]
        bool I_MotionProhibited { get; }

        [Status(Category = "Inputs")]
        bool I_ClampRightClose { get; }

        [Status(Category = "Inputs")]
        bool I_ClampLeftClose { get; }

        [Status(Category = "Inputs")]
        bool I_ClampRightOpen { get; }

        [Status(Category = "Inputs")]
        bool I_ClampLeftOpen { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierPresenceMiddle { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierPresenceLeft { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierPresenceRight { get; }

        [Status(Category = "Inputs")]
        bool I_AccessSwitch { get; }

        [Status(Category = "Inputs")]
        bool I_ProtrusionDetection { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadA { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadB { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadC { get; }

        [Status(Category = "Inputs")]
        bool I_InfoPadD { get; }

        [Status(Category = "Inputs")]
        bool I_PositionForReadingId { get; }

        #endregion

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
        bool O_LaserStop { get; }

        [Status(Category = "Outputs")]
        bool O_InterlockCancel { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampCloseRight { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampOpenRight { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampCloseLeft { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampOpenLeft { get; }

        [Status(Category = "Outputs")]
        bool O_GreenIndicator { get; }

        [Status(Category = "Outputs")]
        bool O_RedIndicator { get; }

        [Status(Category = "Outputs")]
        bool O_LoadIndicator { get; }

        [Status(Category = "Outputs")]
        bool O_UnloadIndicator { get; }

        [Status(Category = "Outputs")]
        bool O_AccessSwitchIndicator { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CarrierOpen { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CarrierClamp { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_PodPresenceSensorOn { get; }

        [Status(Category = "Outputs", Documentation = "Signal not connected")]
        bool O_CarrierProperPlaced { get; }

        #endregion

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

        #endregion
    }
}
