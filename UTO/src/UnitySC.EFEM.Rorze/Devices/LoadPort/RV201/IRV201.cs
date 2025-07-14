using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201
{
    [Device]
    public interface IRV201 : ILoadPort
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

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_EmergencyStop { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_TemporarilyStop { get; }

        [Status(Category = "Inputs")]
        bool I_ExhaustFan1 { get; }

        [Status(Category = "Inputs")]
        bool I_ExhaustFan2 { get; }

        [Status(Category = "Inputs")]
        bool I_Protrusion { get; }

        [Status(Category = "Inputs")]
        bool I_Protrusion2 { get; }

        [Status(Category = "Inputs")]
        bool I_FOUPDoorLeftClose { get; }

        [Status(Category = "Inputs")]
        bool I_FOUPDoorLeftOpen { get; }

        [Status(Category = "Inputs")]
        bool I_FOUPDoorRightClose { get; }

        [Status(Category = "Inputs")]
        bool I_FOUPDoorRightOpen { get; }

        [Status(Category = "Inputs")]
        bool I_MappingSensorContaining { get; }

        [Status(Category = "Inputs")]
        bool I_MappingSensorPreparation { get; }

        [Status(Category = "Inputs")]
        bool I_UpperPressureLimit { get; }

        [Status(Category = "Inputs")]
        bool I_LowerPressureLimit { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierClampOpen { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierClampClose { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceLeft { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceRight { get; }

        [Status(Category = "Inputs")]
        bool I_PresenceMiddle { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_InfoPadA { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_InfoPadB { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_InfoPadC { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_InfoPadD { get; }

        [Status(Category = "Inputs")]
        bool I_Presence { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_FOSBIdentificationSensor { get; }

        [Status(Category = "Inputs")]
        bool I_ObstacleDetectingSensor { get; }

        [Status(Category = "Inputs")]
        bool I_DoorDetection { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_OpenCarrierDetectingSensor { get; }

        [Status(Category = "Inputs")]
        bool I_StageRotationBackward { get; }

        [Status(Category = "Inputs")]
        bool I_StageRotationForward { get; }

        [Status(Category = "Inputs")]
        bool I_BcrLifting { get; }

        [Status(Category = "Inputs")]
        bool I_BcrLowering { get; }

        [Status(Category = "Inputs")]
        bool I_CoverLock { get; }

        [Status(Category = "Inputs")]
        bool I_CoverUnlock { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierRetainerLowering { get; }

        [Status(Category = "Inputs")]
        bool I_CarrierRetainerLifting { get; }

        [Status(Category = "Inputs")]
        bool I_External_SW1_ACCESS { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_External_SW2_TEST { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_External_SW3_UNLOAD { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_PFA_L { get; }

        [Status(Category = "Inputs", Documentation = "Option")]
        bool I_PFA_R { get; }

        [Status(Category = "Inputs")]
        bool I_Dsc300mm { get; }

        [Status(Category = "Inputs")]
        bool I_Dsc200mm { get; }

        [Status(Category = "Inputs")]
        bool I_Dsc150mm { get; }

        [Status(Category = "Inputs")]
        bool I_CstCommon { get; }

        [Status(Category = "Inputs")]
        bool I_Cst200mm { get; }

        [Status(Category = "Inputs")]
        bool I_Cst150mm { get; }

        [Status(Category = "Inputs")]
        bool I_Adapter { get; }

        [Status(Category = "Inputs")]
        bool I_CoverClosed { get; }

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
        bool O_Protrusion2Enabled { get; }

        [Status(Category = "Outputs")]
        bool O_AdapterClamp { get; }

        [Status(Category = "Outputs")]
        bool O_AdapterPower { get; }

        [Status(Category = "Outputs")]
        bool O_ObstacleDetectionCancel { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampClose { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierClampOpen { get; }

        [Status(Category = "Outputs")]
        bool O_FOUPDoorLockOpen { get; }

        [Status(Category = "Outputs")]
        bool O_FOUPDoorLockClose { get; }

        [Status(Category = "Outputs")]
        bool O_MappingSensorPreparation { get; }

        [Status(Category = "Outputs")]
        bool O_MappingSensorContaining { get; }

        [Status(Category = "Outputs")]
        bool O_ChuckingOn { get; }

        [Status(Category = "Outputs")]
        bool O_ChuckingOff { get; }

        [Status(Category = "Outputs")]
        bool O_CoverLock { get; }

        [Status(Category = "Outputs")]
        bool O_CoverUnlock { get; }

        [Status(Category = "Outputs", Documentation = "External Output")]
        bool O_DoorOpen_Ext { get; }

        [Status(Category = "Outputs", Documentation = "External Output")]
        bool O_CarrierClamp_Ext { get; }

        [Status(Category = "Outputs", Documentation = "External Output")]
        bool O_CarrierPresenceOn_Ext { get; }

        [Status(Category = "Outputs", Documentation = "External Output")]
        bool O_PreparationCompleted_Ext { get; }

        [Status(Category = "Outputs", Documentation = "External Output")]
        bool O_CarrierProperlyPlaced_Ext { get; }

        [Status(Category = "Outputs")]
        bool O_StageRotationBackward { get; }

        [Status(Category = "Outputs")]
        bool O_StageRotationForward { get; }

        [Status(Category = "Outputs")]
        bool O_BcrLifting { get; }

        [Status(Category = "Outputs")]
        bool O_BcrLowering { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierRetainerLowering { get; }

        [Status(Category = "Outputs")]
        bool O_CarrierRetainerLifting { get; }

        [Status(Category = "Outputs")]
        bool O_SW1_LED { get; }

        [Status(Category = "Outputs", Documentation = "Option")]
        bool O_SW3_LED { get; }

        [Status(Category = "Outputs")]
        bool O_LOAD_LED { get; }

        [Status(Category = "Outputs")]
        bool O_UNLOAD_LED { get; }

        [Status(Category = "Outputs")]
        bool O_PRESENCE_LED { get; }

        [Status(Category = "Outputs")]
        bool O_PLACEMENT_LED { get; }

        [Status(Category = "Outputs")]
        bool O_MANUAL_LED { get; }

        [Status(Category = "Outputs")]
        bool O_ERROR_LED { get; }

        [Status(Category = "Outputs")]
        bool O_CLAMP_LED { get; }

        [Status(Category = "Outputs")]
        bool O_DOCK_LED { get; }

        [Status(Category = "Outputs")]
        bool O_BUSY_LED { get; }

        [Status(Category = "Outputs")]
        bool O_AUTO_LED { get; }

        [Status(Category = "Outputs")]
        bool O_RESERVED_LED { get; }

        [Status(Category = "Outputs")]
        bool O_CLOSE_LED { get; }

        [Status(Category = "Outputs")]
        bool O_LOCK_LED { get; }

        #endregion Outputs

        #region Carrier Presence

        [Status]
        CarrierType CarrierDetectionMode { get; }

        [Status]
        CarrierType CarrierType { get; }

        #endregion Carrier Presence

        #region VersionedDevice

        [Status]
        string Version { get; }

        #endregion

        #endregion Statuses
    }
}
