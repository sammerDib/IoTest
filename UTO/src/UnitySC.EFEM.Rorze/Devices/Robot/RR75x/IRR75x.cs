using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

using OperationMode = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x
{
    [Device]
    public interface IRR75x : IRobot
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

        [Status(Category = "Inputs")]
        bool I_EmergencyStop_SignalNotConnected { get; }

        [Status(Category = "Inputs")]
        bool I_Pause_SignalNotConnected { get; }

        [Status(Category = "Inputs")]
        bool I_VacuumSourcePressure_SignalNotConnected { get; }

        [Status(Category = "Inputs")]
        bool I_AirSourcePressure_SignalNotConnected { get; }

        [Status(Category = "Inputs")]
        bool I_ExhaustFan { get; }

        [Status(Category = "Inputs")]
        bool I_ExhaustFan_ForUpperArm { get; }

        [Status(Category = "Inputs")]
        bool I_ExhaustFan_ForLowerArm { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger1_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger1_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger2_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger2_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger3_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger3_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger4_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger4_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger5_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_UpperArm_Finger5_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_LowerArm_WaferPresence1 { get; }

        [Status(Category = "Inputs")]
        bool I_LowerArm_WaferPresence2 { get; }

        [Status(Category = "Inputs")]
        bool I_EmergencyStop_TeachingPendant { get; }

        [Status(Category = "Inputs")]
        bool I_DeadManSwitch { get; }

        [Status(Category = "Inputs")]
        bool I_ModeKey { get; }

        [Status(Category = "Inputs")]
        bool I_InterlockInput00 { get; }

        [Status(Category = "Inputs")]
        bool I_InterlockInput01 { get; }

        [Status(Category = "Inputs")]
        bool I_InterlockInput02 { get; }

        [Status(Category = "Inputs")]
        bool I_InterlockInput03 { get; }

        [Status(Category = "Inputs")]
        bool I_Sensor1ForTeaching { get; }

        [Status(Category = "Inputs")]
        bool I_Sensor2ForTeaching { get; }

        #endregion Inputs

        #region External Inputs

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput1 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput2 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput3 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput4 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput5 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput6 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput7 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput8 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput9 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput10 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput11 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput12 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput13 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput14 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput15 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput16 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput17 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_ExternalInput18 { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_Sensor1ForTeaching_Ext { get; }

        [Status(Category = "Inputs", Documentation = "External input")]
        bool I_Sensor2ForTeaching_Ext { get; }

        #endregion External Inputs

        #region Outputs

        [Status(Category = "Outputs")]
        bool O_PreparationComplete_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_Pause_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_FatalError_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_LightError_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_ZAxisBrakeOFF_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_BatteryVoltageTooLow_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_DrivePower_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_TorqueLimitation_SignalNotConnected { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger1_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger1_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger2_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger2_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger3_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger3_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger4_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger4_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger5_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArm_Finger5_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_LowerArm_SolenoidValveOn { get; }

        [Status(Category = "Outputs")]
        bool O_LowerArm_SolenoidValveOff { get; }

        [Status(Category = "Outputs")]
        bool O_XAxis_ExcitationOnOff_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_ZAxis_ExcitationOnOff_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_RotationAxisExcitationOnOff_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArmExcitationOnOff_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_LowerArmExcitationOnOff_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_UpperArmOrigin_LogicSignal { get; }

        [Status(Category = "Outputs")]
        bool O_LowerArmOrigin_LogicSignal { get; }

        #endregion Outputs

        #region External Outputs

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput1 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput2 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput3 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput4 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput5 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput6 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput7 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput8 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput9 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput10 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput11 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput12 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput13 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput14 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput15 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput16 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput17 { get; }

        [Status(Category = "Outputs", Documentation = "External outputs")]
        bool O_ExternalOutput18 { get; }

        #endregion External Outputs

        #region VersionedDevice

        [Status]
        string Version { get; }

        #endregion

        #endregion Statuses

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void GetStatuses();

        #endregion Commands
    }
}
