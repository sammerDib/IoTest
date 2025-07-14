using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status
{
    #region Enums

    [Flags]
    public enum RobotGeneralInputs: Int64
    {
        EmergencyStop_SignalNotConnected        = (Int64) 1 <<  0,
        Pause_SignalNotConnected                = (Int64) 1 <<  1,
        VacuumSourcePressure_SignalNotConnected = (Int64) 1 <<  2,
        AirSourcePressure_SignalNotConnected    = (Int64) 1 <<  3,

        ExhaustFan                              = (Int64) 1 <<  5,
        ExhaustFan_ForUpperArm                  = (Int64) 1 <<  6,
        ExhaustFan_ForLowerArm                  = (Int64) 1 <<  7,
        UpperArm_Finger1_WaferPresence1         = (Int64) 1 <<  8,
        UpperArm_Finger1_WaferPresence2         = (Int64) 1 <<  9,
        UpperArm_Finger2_WaferPresence1         = (Int64) 1 << 10,
        UpperArm_Finger2_WaferPresence2         = (Int64) 1 << 11,
        UpperArm_Finger3_WaferPresence1         = (Int64) 1 << 12,
        UpperArm_Finger3_WaferPresence2         = (Int64) 1 << 13,
        UpperArm_Finger4_WaferPresence1         = (Int64) 1 << 14,
        UpperArm_Finger4_WaferPresence2         = (Int64) 1 << 15,
        UpperArm_Finger5_WaferPresence1         = (Int64) 1 << 16,
        UpperArm_Finger5_WaferPresence2         = (Int64) 1 << 17,
        LowerArm_WaferPresence1                 = (Int64) 1 << 18,
        LowerArm_WaferPresence2                 = (Int64) 1 << 19,

        EmergencyStop_TeachingPendant           = (Int64) 1 << 21,
        DeadManSwitch                           = (Int64) 1 << 22,
        ModeKey                                 = (Int64) 1 << 23,
        InterlockInput00                        = (Int64) 1 << 24,
        InterlockInput01                        = (Int64) 1 << 25,
        InterlockInput02                        = (Int64) 1 << 26,
        InterlockInput03                        = (Int64) 1 << 27,
        Sensor1ForTeaching                      = (Int64) 1 << 28,
        Sensor2ForTeaching                      = (Int64) 1 << 29,

        // External Inputs
        ExternalInput1         = (Int64) 1 << 32,
        ExternalInput2         = (Int64) 1 << 33,
        ExternalInput3         = (Int64) 1 << 34,
        ExternalInput4         = (Int64) 1 << 35,
        ExternalInput5         = (Int64) 1 << 36,
        ExternalInput6         = (Int64) 1 << 37,
        ExternalInput7         = (Int64) 1 << 38,
        ExternalInput8         = (Int64) 1 << 39,
        ExternalInput9         = (Int64) 1 << 40,
        ExternalInput10        = (Int64) 1 << 41,
        ExternalInput11        = (Int64) 1 << 42,
        ExternalInput12        = (Int64) 1 << 43,
        ExternalInput13        = (Int64) 1 << 44,
        ExternalInput14        = (Int64) 1 << 45,
        ExternalInput15        = (Int64) 1 << 46,
        ExternalInput16        = (Int64) 1 << 47,
        ExternalInput17        = (Int64) 1 << 48,
        ExternalInput18        = (Int64) 1 << 49,

        Sensor1ForTeaching_Ext = (Int64) 1 << 56,
        Sensor2ForTeaching_Ext = (Int64) 1 << 57
    }

    [Flags]
    public enum RobotGeneralOutputs : Int64
    {
        PreparationComplete_SignalNotConnected  = (Int64) 1 <<  0,
        Pause_SignalNotConnected                = (Int64) 1 <<  1,
        FatalError_SignalNotConnected           = (Int64) 1 <<  2,
        LightError_SignalNotConnected           = (Int64) 1 <<  3,
        ZAxisBrakeOFF_SignalNotConnected        = (Int64) 1 <<  4,
        BatteryVoltageTooLow_SignalNotConnected = (Int64) 1 <<  5,
        DrivePower_SignalNotConnected           = (Int64) 1 <<  6,
        TorqueLimitation_SignalNotConnected     = (Int64) 1 <<  7,
        UpperArm_Finger1_SolenoidValveOn        = (Int64) 1 <<  8,
        UpperArm_Finger1_SolenoidValveOff       = (Int64) 1 <<  9,
        UpperArm_Finger2_SolenoidValveOn        = (Int64) 1 << 10,
        UpperArm_Finger2_SolenoidValveOff       = (Int64) 1 << 11,
        UpperArm_Finger3_SolenoidValveOn        = (Int64) 1 << 12,
        UpperArm_Finger3_SolenoidValveOff       = (Int64) 1 << 13,
        UpperArm_Finger4_SolenoidValveOn        = (Int64) 1 << 14,
        UpperArm_Finger4_SolenoidValveOff       = (Int64) 1 << 15,
        UpperArm_Finger5_SolenoidValveOn        = (Int64) 1 << 16,
        UpperArm_Finger5_SolenoidValveOff       = (Int64) 1 << 17,
        LowerArm_SolenoidValveOn                = (Int64) 1 << 18,
        LowerArm_SolenoidValveOff               = (Int64) 1 << 19,

        XAxis_ExcitationOnOff_LogicSignal       = (Int64) 1 << 24,
        ZAxis_ExcitationOnOff_LogicSignal       = (Int64) 1 << 25,
        RotationAxisExcitationOnOff_LogicSignal = (Int64) 1 << 26,
        UpperArmExcitationOnOff_LogicSignal     = (Int64) 1 << 27,
        LowerArmExcitationOnOff_LogicSignal     = (Int64) 1 << 28,

        UpperArmOrigin_LogicSignal              = (Int64) 1 << 30,
        LowerArmOrigin_LogicSignal              = (Int64) 1 << 31,

        // External outputs
        ExternalOutput1  = (Int64) 1 << 32,
        ExternalOutput2  = (Int64) 1 << 33,
        ExternalOutput3  = (Int64) 1 << 34,
        ExternalOutput4  = (Int64) 1 << 35,
        ExternalOutput5  = (Int64) 1 << 36,
        ExternalOutput6  = (Int64) 1 << 37,
        ExternalOutput7  = (Int64) 1 << 38,
        ExternalOutput8  = (Int64) 1 << 39,
        ExternalOutput9  = (Int64) 1 << 40,
        ExternalOutput10 = (Int64) 1 << 41,
        ExternalOutput11 = (Int64) 1 << 42,
        ExternalOutput12 = (Int64) 1 << 43,
        ExternalOutput13 = (Int64) 1 << 44,
        ExternalOutput14 = (Int64) 1 << 45,
        ExternalOutput15 = (Int64) 1 << 46,
        ExternalOutput16 = (Int64) 1 << 47,
        ExternalOutput17 = (Int64) 1 << 48,
        ExternalOutput18 = (Int64) 1 << 49
    }

    #endregion Enums

    public class RobotGpioStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotGpioStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="RobotGpioStatus"/> instance</param>
        /// </summary>
        public RobotGpioStatus(RobotGpioStatus other)
        {
            Set(other);
        }

        public RobotGpioStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            var pi = (RobotGeneralInputs)Int64.Parse(statuses[0], NumberStyles.AllowHexSpecifier);

            I_EmergencyStop_SignalNotConnected        = (pi & RobotGeneralInputs.EmergencyStop_SignalNotConnected) != 0;
            I_Pause_SignalNotConnected                = (pi & RobotGeneralInputs.Pause_SignalNotConnected) != 0;
            I_VacuumSourcePressure_SignalNotConnected = (pi & RobotGeneralInputs.VacuumSourcePressure_SignalNotConnected) != 0;
            I_AirSourcePressure_SignalNotConnected    = (pi & RobotGeneralInputs.AirSourcePressure_SignalNotConnected) != 0;
            I_ExhaustFan                              = (pi & RobotGeneralInputs.ExhaustFan) != 0;
            I_ExhaustFan_ForUpperArm                  = (pi & RobotGeneralInputs.ExhaustFan_ForUpperArm) != 0;
            I_ExhaustFan_ForLowerArm                  = (pi & RobotGeneralInputs.ExhaustFan_ForLowerArm) != 0;
            I_UpperArm_Finger1_WaferPresence1         = (pi & RobotGeneralInputs.UpperArm_Finger1_WaferPresence1) != 0;
            I_UpperArm_Finger1_WaferPresence2         = (pi & RobotGeneralInputs.UpperArm_Finger1_WaferPresence2) != 0;
            I_UpperArm_Finger2_WaferPresence1         = (pi & RobotGeneralInputs.UpperArm_Finger2_WaferPresence1) != 0;
            I_UpperArm_Finger2_WaferPresence2         = (pi & RobotGeneralInputs.UpperArm_Finger2_WaferPresence2) != 0;
            I_UpperArm_Finger3_WaferPresence1         = (pi & RobotGeneralInputs.UpperArm_Finger3_WaferPresence1) != 0;
            I_UpperArm_Finger3_WaferPresence2         = (pi & RobotGeneralInputs.UpperArm_Finger3_WaferPresence2) != 0;
            I_UpperArm_Finger4_WaferPresence1         = (pi & RobotGeneralInputs.UpperArm_Finger4_WaferPresence1) != 0;
            I_UpperArm_Finger4_WaferPresence2         = (pi & RobotGeneralInputs.UpperArm_Finger4_WaferPresence2) != 0;
            I_UpperArm_Finger5_WaferPresence1         = (pi & RobotGeneralInputs.UpperArm_Finger5_WaferPresence1) != 0;
            I_UpperArm_Finger5_WaferPresence2         = (pi & RobotGeneralInputs.UpperArm_Finger5_WaferPresence2) != 0;
            I_LowerArm_WaferPresence1                 = (pi & RobotGeneralInputs.LowerArm_WaferPresence1) != 0;
            I_LowerArm_WaferPresence2                 = (pi & RobotGeneralInputs.LowerArm_WaferPresence2) != 0;
            I_EmergencyStop_TeachingPendant           = (pi & RobotGeneralInputs.EmergencyStop_TeachingPendant) != 0;
            I_DeadManSwitch                           = (pi & RobotGeneralInputs.DeadManSwitch) != 0;
            I_ModeKey                                 = (pi & RobotGeneralInputs.ModeKey) != 0;
            I_InterlockInput00                        = (pi & RobotGeneralInputs.InterlockInput00) != 0;
            I_InterlockInput01                        = (pi & RobotGeneralInputs.InterlockInput01) != 0;
            I_InterlockInput02                        = (pi & RobotGeneralInputs.InterlockInput02) != 0;
            I_InterlockInput03                        = (pi & RobotGeneralInputs.InterlockInput03) != 0;
            I_Sensor1ForTeaching                      = (pi & RobotGeneralInputs.Sensor1ForTeaching) != 0;
            I_Sensor2ForTeaching                      = (pi & RobotGeneralInputs.Sensor2ForTeaching) != 0;

            I_ExternalInput1         = (pi & RobotGeneralInputs.ExternalInput1) != 0;
            I_ExternalInput2         = (pi & RobotGeneralInputs.ExternalInput2) != 0;
            I_ExternalInput3         = (pi & RobotGeneralInputs.ExternalInput3) != 0;
            I_ExternalInput4         = (pi & RobotGeneralInputs.ExternalInput4) != 0;
            I_ExternalInput5         = (pi & RobotGeneralInputs.ExternalInput5) != 0;
            I_ExternalInput6         = (pi & RobotGeneralInputs.ExternalInput6) != 0;
            I_ExternalInput7         = (pi & RobotGeneralInputs.ExternalInput7) != 0;
            I_ExternalInput8         = (pi & RobotGeneralInputs.ExternalInput8) != 0;
            I_ExternalInput9         = (pi & RobotGeneralInputs.ExternalInput9) != 0;
            I_ExternalInput10        = (pi & RobotGeneralInputs.ExternalInput10) != 0;
            I_ExternalInput11        = (pi & RobotGeneralInputs.ExternalInput11) != 0;
            I_ExternalInput12        = (pi & RobotGeneralInputs.ExternalInput12) != 0;
            I_ExternalInput13        = (pi & RobotGeneralInputs.ExternalInput13) != 0;
            I_ExternalInput14        = (pi & RobotGeneralInputs.ExternalInput14) != 0;
            I_ExternalInput15        = (pi & RobotGeneralInputs.ExternalInput15) != 0;
            I_ExternalInput16        = (pi & RobotGeneralInputs.ExternalInput16) != 0;
            I_ExternalInput17        = (pi & RobotGeneralInputs.ExternalInput17) != 0;
            I_ExternalInput18        = (pi & RobotGeneralInputs.ExternalInput18) != 0;
            I_Sensor1ForTeaching_Ext = (pi & RobotGeneralInputs.Sensor1ForTeaching_Ext) != 0;
            I_Sensor2ForTeaching_Ext = (pi & RobotGeneralInputs.Sensor2ForTeaching_Ext) != 0;

            var po = (RobotGeneralOutputs) Int64.Parse(statuses[1], NumberStyles.AllowHexSpecifier);

            O_PreparationComplete_SignalNotConnected  = (po & RobotGeneralOutputs.PreparationComplete_SignalNotConnected) != 0;
            O_Pause_SignalNotConnected                = (po & RobotGeneralOutputs.Pause_SignalNotConnected) != 0;
            O_FatalError_SignalNotConnected           = (po & RobotGeneralOutputs.FatalError_SignalNotConnected) != 0;
            O_LightError_SignalNotConnected           = (po & RobotGeneralOutputs.LightError_SignalNotConnected) != 0;
            O_ZAxisBrakeOFF_SignalNotConnected        = (po & RobotGeneralOutputs.ZAxisBrakeOFF_SignalNotConnected) != 0;
            O_BatteryVoltageTooLow_SignalNotConnected = (po & RobotGeneralOutputs.BatteryVoltageTooLow_SignalNotConnected) != 0;
            O_DrivePower_SignalNotConnected           = (po & RobotGeneralOutputs.DrivePower_SignalNotConnected) != 0;
            O_TorqueLimitation_SignalNotConnected     = (po & RobotGeneralOutputs.TorqueLimitation_SignalNotConnected) != 0;
            O_UpperArm_Finger1_SolenoidValveOn        = (po & RobotGeneralOutputs.UpperArm_Finger1_SolenoidValveOn) != 0;
            O_UpperArm_Finger1_SolenoidValveOff       = (po & RobotGeneralOutputs.UpperArm_Finger1_SolenoidValveOff) != 0;
            O_UpperArm_Finger2_SolenoidValveOn        = (po & RobotGeneralOutputs.UpperArm_Finger2_SolenoidValveOn) != 0;
            O_UpperArm_Finger2_SolenoidValveOff       = (po & RobotGeneralOutputs.UpperArm_Finger2_SolenoidValveOff) != 0;
            O_UpperArm_Finger3_SolenoidValveOn        = (po & RobotGeneralOutputs.UpperArm_Finger3_SolenoidValveOn) != 0;
            O_UpperArm_Finger3_SolenoidValveOff       = (po & RobotGeneralOutputs.UpperArm_Finger3_SolenoidValveOff) != 0;
            O_UpperArm_Finger4_SolenoidValveOn        = (po & RobotGeneralOutputs.UpperArm_Finger4_SolenoidValveOn) != 0;
            O_UpperArm_Finger4_SolenoidValveOff       = (po & RobotGeneralOutputs.UpperArm_Finger4_SolenoidValveOff) != 0;
            O_UpperArm_Finger5_SolenoidValveOn        = (po & RobotGeneralOutputs.UpperArm_Finger5_SolenoidValveOn) != 0;
            O_UpperArm_Finger5_SolenoidValveOff       = (po & RobotGeneralOutputs.UpperArm_Finger5_SolenoidValveOff) != 0;
            O_LowerArm_SolenoidValveOn                = (po & RobotGeneralOutputs.LowerArm_SolenoidValveOn) != 0;
            O_LowerArm_SolenoidValveOff               = (po & RobotGeneralOutputs.LowerArm_SolenoidValveOff) != 0;
            O_XAxis_ExcitationOnOff_LogicSignal       = (po & RobotGeneralOutputs.XAxis_ExcitationOnOff_LogicSignal) != 0;
            O_ZAxis_ExcitationOnOff_LogicSignal       = (po & RobotGeneralOutputs.ZAxis_ExcitationOnOff_LogicSignal) != 0;
            O_RotationAxisExcitationOnOff_LogicSignal = (po & RobotGeneralOutputs.RotationAxisExcitationOnOff_LogicSignal) != 0;
            O_UpperArmExcitationOnOff_LogicSignal     = (po & RobotGeneralOutputs.UpperArmExcitationOnOff_LogicSignal) != 0;
            O_LowerArmExcitationOnOff_LogicSignal     = (po & RobotGeneralOutputs.LowerArmExcitationOnOff_LogicSignal) != 0;
            O_UpperArmOrigin_LogicSignal              = (po & RobotGeneralOutputs.UpperArmOrigin_LogicSignal) != 0;
            O_LowerArmOrigin_LogicSignal              = (po & RobotGeneralOutputs.LowerArmOrigin_LogicSignal) != 0;

            O_ExternalOutput1  = (po & RobotGeneralOutputs.ExternalOutput1) != 0;
            O_ExternalOutput2  = (po & RobotGeneralOutputs.ExternalOutput2) != 0;
            O_ExternalOutput3  = (po & RobotGeneralOutputs.ExternalOutput3) != 0;
            O_ExternalOutput4  = (po & RobotGeneralOutputs.ExternalOutput4) != 0;
            O_ExternalOutput5  = (po & RobotGeneralOutputs.ExternalOutput5) != 0;
            O_ExternalOutput6  = (po & RobotGeneralOutputs.ExternalOutput6) != 0;
            O_ExternalOutput7  = (po & RobotGeneralOutputs.ExternalOutput7) != 0;
            O_ExternalOutput8  = (po & RobotGeneralOutputs.ExternalOutput8) != 0;
            O_ExternalOutput9  = (po & RobotGeneralOutputs.ExternalOutput9) != 0;
            O_ExternalOutput10 = (po & RobotGeneralOutputs.ExternalOutput10) != 0;
            O_ExternalOutput11 = (po & RobotGeneralOutputs.ExternalOutput11) != 0;
            O_ExternalOutput12 = (po & RobotGeneralOutputs.ExternalOutput12) != 0;
            O_ExternalOutput13 = (po & RobotGeneralOutputs.ExternalOutput13) != 0;
            O_ExternalOutput14 = (po & RobotGeneralOutputs.ExternalOutput14) != 0;
            O_ExternalOutput15 = (po & RobotGeneralOutputs.ExternalOutput15) != 0;
            O_ExternalOutput16 = (po & RobotGeneralOutputs.ExternalOutput16) != 0;
            O_ExternalOutput17 = (po & RobotGeneralOutputs.ExternalOutput17) != 0;
            O_ExternalOutput18 = (po & RobotGeneralOutputs.ExternalOutput18) != 0;
        }

        #endregion Constructors

        #region Properties

        public bool I_EmergencyStop_SignalNotConnected        { get; internal set; }
        public bool I_Pause_SignalNotConnected                { get; internal set; }
        public bool I_VacuumSourcePressure_SignalNotConnected { get; internal set; }
        public bool I_AirSourcePressure_SignalNotConnected    { get; internal set; }
        public bool I_ExhaustFan                              { get; internal set; }
        public bool I_ExhaustFan_ForUpperArm                  { get; internal set; }
        public bool I_ExhaustFan_ForLowerArm                  { get; internal set; }
        public bool I_UpperArm_Finger1_WaferPresence1         { get; internal set; }
        public bool I_UpperArm_Finger1_WaferPresence2         { get; internal set; }
        public bool I_UpperArm_Finger2_WaferPresence1         { get; internal set; }
        public bool I_UpperArm_Finger2_WaferPresence2         { get; internal set; }
        public bool I_UpperArm_Finger3_WaferPresence1         { get; internal set; }
        public bool I_UpperArm_Finger3_WaferPresence2         { get; internal set; }
        public bool I_UpperArm_Finger4_WaferPresence1         { get; internal set; }
        public bool I_UpperArm_Finger4_WaferPresence2         { get; internal set; }
        public bool I_UpperArm_Finger5_WaferPresence1         { get; internal set; }
        public bool I_UpperArm_Finger5_WaferPresence2         { get; internal set; }
        public bool I_LowerArm_WaferPresence1                 { get; internal set; }
        public bool I_LowerArm_WaferPresence2                 { get; internal set; }
        public bool I_EmergencyStop_TeachingPendant           { get; internal set; }
        public bool I_DeadManSwitch                           { get; internal set; }
        public bool I_ModeKey                                 { get; internal set; }
        public bool I_InterlockInput00                        { get; internal set; }
        public bool I_InterlockInput01                        { get; internal set; }
        public bool I_InterlockInput02                        { get; internal set; }
        public bool I_InterlockInput03                        { get; internal set; }
        public bool I_Sensor1ForTeaching                      { get; internal set; }
        public bool I_Sensor2ForTeaching                      { get; internal set; }

        public bool I_ExternalInput1         { get; internal set; }
        public bool I_ExternalInput2         { get; internal set; }
        public bool I_ExternalInput3         { get; internal set; }
        public bool I_ExternalInput4         { get; internal set; }
        public bool I_ExternalInput5         { get; internal set; }
        public bool I_ExternalInput6         { get; internal set; }
        public bool I_ExternalInput7         { get; internal set; }
        public bool I_ExternalInput8         { get; internal set; }
        public bool I_ExternalInput9         { get; internal set; }
        public bool I_ExternalInput10        { get; internal set; }
        public bool I_ExternalInput11        { get; internal set; }
        public bool I_ExternalInput12        { get; internal set; }
        public bool I_ExternalInput13        { get; internal set; }
        public bool I_ExternalInput14        { get; internal set; }
        public bool I_ExternalInput15        { get; internal set; }
        public bool I_ExternalInput16        { get; internal set; }
        public bool I_ExternalInput17        { get; internal set; }
        public bool I_ExternalInput18        { get; internal set; }
        public bool I_Sensor1ForTeaching_Ext { get; internal set; }
        public bool I_Sensor2ForTeaching_Ext { get; internal set; }

        public bool O_PreparationComplete_SignalNotConnected  { get; internal set; }
        public bool O_Pause_SignalNotConnected                { get; internal set; }
        public bool O_FatalError_SignalNotConnected           { get; internal set; }
        public bool O_LightError_SignalNotConnected           { get; internal set; }
        public bool O_ZAxisBrakeOFF_SignalNotConnected        { get; internal set; }
        public bool O_BatteryVoltageTooLow_SignalNotConnected { get; internal set; }
        public bool O_DrivePower_SignalNotConnected           { get; internal set; }
        public bool O_TorqueLimitation_SignalNotConnected     { get; internal set; }
        public bool O_UpperArm_Finger1_SolenoidValveOn        { get; internal set; }
        public bool O_UpperArm_Finger1_SolenoidValveOff       { get; internal set; }
        public bool O_UpperArm_Finger2_SolenoidValveOn        { get; internal set; }
        public bool O_UpperArm_Finger2_SolenoidValveOff       { get; internal set; }
        public bool O_UpperArm_Finger3_SolenoidValveOn        { get; internal set; }
        public bool O_UpperArm_Finger3_SolenoidValveOff       { get; internal set; }
        public bool O_UpperArm_Finger4_SolenoidValveOn        { get; internal set; }
        public bool O_UpperArm_Finger4_SolenoidValveOff       { get; internal set; }
        public bool O_UpperArm_Finger5_SolenoidValveOn        { get; internal set; }
        public bool O_UpperArm_Finger5_SolenoidValveOff       { get; internal set; }
        public bool O_LowerArm_SolenoidValveOn                { get; internal set; }
        public bool O_LowerArm_SolenoidValveOff               { get; internal set; }
        public bool O_XAxis_ExcitationOnOff_LogicSignal       { get; internal set; }
        public bool O_ZAxis_ExcitationOnOff_LogicSignal       { get; internal set; }
        public bool O_RotationAxisExcitationOnOff_LogicSignal { get; internal set; }
        public bool O_UpperArmExcitationOnOff_LogicSignal     { get; internal set; }
        public bool O_LowerArmExcitationOnOff_LogicSignal     { get; internal set; }
        public bool O_UpperArmOrigin_LogicSignal              { get; internal set; }
        public bool O_LowerArmOrigin_LogicSignal              { get; internal set; }

        public bool O_ExternalOutput1  { get; internal set; }
        public bool O_ExternalOutput2  { get; internal set; }
        public bool O_ExternalOutput3  { get; internal set; }
        public bool O_ExternalOutput4  { get; internal set; }
        public bool O_ExternalOutput5  { get; internal set; }
        public bool O_ExternalOutput6  { get; internal set; }
        public bool O_ExternalOutput7  { get; internal set; }
        public bool O_ExternalOutput8  { get; internal set; }
        public bool O_ExternalOutput9  { get; internal set; }
        public bool O_ExternalOutput10 { get; internal set; }
        public bool O_ExternalOutput11 { get; internal set; }
        public bool O_ExternalOutput12 { get; internal set; }
        public bool O_ExternalOutput13 { get; internal set; }
        public bool O_ExternalOutput14 { get; internal set; }
        public bool O_ExternalOutput15 { get; internal set; }
        public bool O_ExternalOutput16 { get; internal set; }
        public bool O_ExternalOutput17 { get; internal set; }
        public bool O_ExternalOutput18 { get; internal set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(RobotGpioStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_EmergencyStop_SignalNotConnected        = false;
                    I_Pause_SignalNotConnected                = false;
                    I_VacuumSourcePressure_SignalNotConnected = false;
                    I_AirSourcePressure_SignalNotConnected    = false;
                    I_ExhaustFan                              = false;
                    I_ExhaustFan_ForUpperArm                  = false;
                    I_ExhaustFan_ForLowerArm                  = false;
                    I_UpperArm_Finger1_WaferPresence1         = false;
                    I_UpperArm_Finger1_WaferPresence2         = false;
                    I_UpperArm_Finger2_WaferPresence1         = false;
                    I_UpperArm_Finger2_WaferPresence2         = false;
                    I_UpperArm_Finger3_WaferPresence1         = false;
                    I_UpperArm_Finger3_WaferPresence2         = false;
                    I_UpperArm_Finger4_WaferPresence1         = false;
                    I_UpperArm_Finger4_WaferPresence2         = false;
                    I_UpperArm_Finger5_WaferPresence1         = false;
                    I_UpperArm_Finger5_WaferPresence2         = false;
                    I_LowerArm_WaferPresence1                 = false;
                    I_LowerArm_WaferPresence2                 = false;
                    I_EmergencyStop_TeachingPendant           = false;
                    I_DeadManSwitch                           = false;
                    I_ModeKey                                 = false;
                    I_InterlockInput00                        = false;
                    I_InterlockInput01                        = false;
                    I_InterlockInput02                        = false;
                    I_InterlockInput03                        = false;
                    I_Sensor1ForTeaching                      = false;
                    I_Sensor2ForTeaching                      = false;

                    I_ExternalInput1         = false;
                    I_ExternalInput2         = false;
                    I_ExternalInput3         = false;
                    I_ExternalInput4         = false;
                    I_ExternalInput5         = false;
                    I_ExternalInput6         = false;
                    I_ExternalInput7         = false;
                    I_ExternalInput8         = false;
                    I_ExternalInput9         = false;
                    I_ExternalInput10        = false;
                    I_ExternalInput11        = false;
                    I_ExternalInput12        = false;
                    I_ExternalInput13        = false;
                    I_ExternalInput14        = false;
                    I_ExternalInput15        = false;
                    I_ExternalInput16        = false;
                    I_ExternalInput17        = false;
                    I_ExternalInput18        = false;
                    I_Sensor1ForTeaching_Ext = false;
                    I_Sensor2ForTeaching_Ext = false;

                    O_PreparationComplete_SignalNotConnected  = false;
                    O_Pause_SignalNotConnected                = false;
                    O_FatalError_SignalNotConnected           = false;
                    O_LightError_SignalNotConnected           = false;
                    O_ZAxisBrakeOFF_SignalNotConnected        = false;
                    O_BatteryVoltageTooLow_SignalNotConnected = false;
                    O_DrivePower_SignalNotConnected           = false;
                    O_TorqueLimitation_SignalNotConnected     = false;
                    O_UpperArm_Finger1_SolenoidValveOn        = false;
                    O_UpperArm_Finger1_SolenoidValveOff       = false;
                    O_UpperArm_Finger2_SolenoidValveOn        = false;
                    O_UpperArm_Finger2_SolenoidValveOff       = false;
                    O_UpperArm_Finger3_SolenoidValveOn        = false;
                    O_UpperArm_Finger3_SolenoidValveOff       = false;
                    O_UpperArm_Finger4_SolenoidValveOn        = false;
                    O_UpperArm_Finger4_SolenoidValveOff       = false;
                    O_UpperArm_Finger5_SolenoidValveOn        = false;
                    O_UpperArm_Finger5_SolenoidValveOff       = false;
                    O_LowerArm_SolenoidValveOn                = false;
                    O_LowerArm_SolenoidValveOff               = false;
                    O_XAxis_ExcitationOnOff_LogicSignal       = false;
                    O_ZAxis_ExcitationOnOff_LogicSignal       = false;
                    O_RotationAxisExcitationOnOff_LogicSignal = false;
                    O_UpperArmExcitationOnOff_LogicSignal     = false;
                    O_LowerArmExcitationOnOff_LogicSignal     = false;
                    O_UpperArmOrigin_LogicSignal              = false;
                    O_LowerArmOrigin_LogicSignal              = false;

                    O_ExternalOutput1  = false;
                    O_ExternalOutput2  = false;
                    O_ExternalOutput3  = false;
                    O_ExternalOutput4  = false;
                    O_ExternalOutput5  = false;
                    O_ExternalOutput6  = false;
                    O_ExternalOutput7  = false;
                    O_ExternalOutput8  = false;
                    O_ExternalOutput9  = false;
                    O_ExternalOutput10 = false;
                    O_ExternalOutput11 = false;
                    O_ExternalOutput12 = false;
                    O_ExternalOutput13 = false;
                    O_ExternalOutput14 = false;
                    O_ExternalOutput15 = false;
                    O_ExternalOutput16 = false;
                    O_ExternalOutput17 = false;
                    O_ExternalOutput18 = false;
                }
                else
                {
                    I_EmergencyStop_SignalNotConnected        = other.I_EmergencyStop_SignalNotConnected;
                    I_Pause_SignalNotConnected                = other.I_Pause_SignalNotConnected;
                    I_VacuumSourcePressure_SignalNotConnected = other.I_VacuumSourcePressure_SignalNotConnected;
                    I_AirSourcePressure_SignalNotConnected    = other.I_AirSourcePressure_SignalNotConnected;
                    I_ExhaustFan                              = other.I_ExhaustFan;
                    I_ExhaustFan_ForUpperArm                  = other.I_ExhaustFan_ForUpperArm;
                    I_ExhaustFan_ForLowerArm                  = other.I_ExhaustFan_ForLowerArm;
                    I_UpperArm_Finger1_WaferPresence1         = other.I_UpperArm_Finger1_WaferPresence1;
                    I_UpperArm_Finger1_WaferPresence2         = other.I_UpperArm_Finger1_WaferPresence2;
                    I_UpperArm_Finger2_WaferPresence1         = other.I_UpperArm_Finger2_WaferPresence1;
                    I_UpperArm_Finger2_WaferPresence2         = other.I_UpperArm_Finger2_WaferPresence2;
                    I_UpperArm_Finger3_WaferPresence1         = other.I_UpperArm_Finger3_WaferPresence1;
                    I_UpperArm_Finger3_WaferPresence2         = other.I_UpperArm_Finger3_WaferPresence2;
                    I_UpperArm_Finger4_WaferPresence1         = other.I_UpperArm_Finger4_WaferPresence1;
                    I_UpperArm_Finger4_WaferPresence2         = other.I_UpperArm_Finger4_WaferPresence2;
                    I_UpperArm_Finger5_WaferPresence1         = other.I_UpperArm_Finger5_WaferPresence1;
                    I_UpperArm_Finger5_WaferPresence2         = other.I_UpperArm_Finger5_WaferPresence2;
                    I_LowerArm_WaferPresence1                 = other.I_LowerArm_WaferPresence1;
                    I_LowerArm_WaferPresence2                 = other.I_LowerArm_WaferPresence2;
                    I_EmergencyStop_TeachingPendant           = other.I_EmergencyStop_TeachingPendant;
                    I_DeadManSwitch                           = other.I_DeadManSwitch;
                    I_ModeKey                                 = other.I_ModeKey;
                    I_InterlockInput00                        = other.I_InterlockInput00;
                    I_InterlockInput01                        = other.I_InterlockInput01;
                    I_InterlockInput02                        = other.I_InterlockInput02;
                    I_InterlockInput03                        = other.I_InterlockInput03;
                    I_Sensor1ForTeaching                      = other.I_Sensor1ForTeaching;
                    I_Sensor2ForTeaching                      = other.I_Sensor2ForTeaching;

                    I_ExternalInput1         = other.I_ExternalInput1;
                    I_ExternalInput2         = other.I_ExternalInput2;
                    I_ExternalInput3         = other.I_ExternalInput3;
                    I_ExternalInput4         = other.I_ExternalInput4;
                    I_ExternalInput5         = other.I_ExternalInput5;
                    I_ExternalInput6         = other.I_ExternalInput6;
                    I_ExternalInput7         = other.I_ExternalInput7;
                    I_ExternalInput8         = other.I_ExternalInput8;
                    I_ExternalInput9         = other.I_ExternalInput9;
                    I_ExternalInput10        = other.I_ExternalInput10;
                    I_ExternalInput11        = other.I_ExternalInput11;
                    I_ExternalInput12        = other.I_ExternalInput12;
                    I_ExternalInput13        = other.I_ExternalInput13;
                    I_ExternalInput14        = other.I_ExternalInput14;
                    I_ExternalInput15        = other.I_ExternalInput15;
                    I_ExternalInput16        = other.I_ExternalInput16;
                    I_ExternalInput17        = other.I_ExternalInput17;
                    I_ExternalInput18        = other.I_ExternalInput18;
                    I_Sensor1ForTeaching_Ext = other.I_Sensor1ForTeaching_Ext;
                    I_Sensor2ForTeaching_Ext = other.I_Sensor2ForTeaching_Ext;

                    O_PreparationComplete_SignalNotConnected  = other.O_PreparationComplete_SignalNotConnected;
                    O_Pause_SignalNotConnected                = other.O_Pause_SignalNotConnected;
                    O_FatalError_SignalNotConnected           = other.O_FatalError_SignalNotConnected;
                    O_LightError_SignalNotConnected           = other.O_LightError_SignalNotConnected;
                    O_ZAxisBrakeOFF_SignalNotConnected        = other.O_ZAxisBrakeOFF_SignalNotConnected;
                    O_BatteryVoltageTooLow_SignalNotConnected = other.O_BatteryVoltageTooLow_SignalNotConnected;
                    O_DrivePower_SignalNotConnected           = other.O_DrivePower_SignalNotConnected;
                    O_TorqueLimitation_SignalNotConnected     = other.O_TorqueLimitation_SignalNotConnected;
                    O_UpperArm_Finger1_SolenoidValveOn        = other.O_UpperArm_Finger1_SolenoidValveOn;
                    O_UpperArm_Finger1_SolenoidValveOff       = other.O_UpperArm_Finger1_SolenoidValveOff;
                    O_UpperArm_Finger2_SolenoidValveOn        = other.O_UpperArm_Finger2_SolenoidValveOn;
                    O_UpperArm_Finger2_SolenoidValveOff       = other.O_UpperArm_Finger2_SolenoidValveOff;
                    O_UpperArm_Finger3_SolenoidValveOn        = other.O_UpperArm_Finger3_SolenoidValveOn;
                    O_UpperArm_Finger3_SolenoidValveOff       = other.O_UpperArm_Finger3_SolenoidValveOff;
                    O_UpperArm_Finger4_SolenoidValveOn        = other.O_UpperArm_Finger4_SolenoidValveOn;
                    O_UpperArm_Finger4_SolenoidValveOff       = other.O_UpperArm_Finger4_SolenoidValveOff;
                    O_UpperArm_Finger5_SolenoidValveOn        = other.O_UpperArm_Finger5_SolenoidValveOn;
                    O_UpperArm_Finger5_SolenoidValveOff       = other.O_UpperArm_Finger5_SolenoidValveOff;
                    O_LowerArm_SolenoidValveOn                = other.O_LowerArm_SolenoidValveOn;
                    O_LowerArm_SolenoidValveOff               = other.O_LowerArm_SolenoidValveOff;
                    O_XAxis_ExcitationOnOff_LogicSignal       = other.O_XAxis_ExcitationOnOff_LogicSignal;
                    O_ZAxis_ExcitationOnOff_LogicSignal       = other.O_ZAxis_ExcitationOnOff_LogicSignal;
                    O_RotationAxisExcitationOnOff_LogicSignal = other.O_RotationAxisExcitationOnOff_LogicSignal;
                    O_UpperArmExcitationOnOff_LogicSignal     = other.O_UpperArmExcitationOnOff_LogicSignal;
                    O_LowerArmExcitationOnOff_LogicSignal     = other.O_LowerArmExcitationOnOff_LogicSignal;
                    O_UpperArmOrigin_LogicSignal              = other.O_UpperArmOrigin_LogicSignal;
                    O_LowerArmOrigin_LogicSignal              = other.O_LowerArmOrigin_LogicSignal;

                    O_ExternalOutput1  = other.O_ExternalOutput1;
                    O_ExternalOutput2  = other.O_ExternalOutput2;
                    O_ExternalOutput3  = other.O_ExternalOutput3;
                    O_ExternalOutput4  = other.O_ExternalOutput4;
                    O_ExternalOutput5  = other.O_ExternalOutput5;
                    O_ExternalOutput6  = other.O_ExternalOutput6;
                    O_ExternalOutput7  = other.O_ExternalOutput7;
                    O_ExternalOutput8  = other.O_ExternalOutput8;
                    O_ExternalOutput9  = other.O_ExternalOutput9;
                    O_ExternalOutput10 = other.O_ExternalOutput10;
                    O_ExternalOutput11 = other.O_ExternalOutput11;
                    O_ExternalOutput12 = other.O_ExternalOutput12;
                    O_ExternalOutput13 = other.O_ExternalOutput13;
                    O_ExternalOutput14 = other.O_ExternalOutput14;
                    O_ExternalOutput15 = other.O_ExternalOutput15;
                    O_ExternalOutput16 = other.O_ExternalOutput16;
                    O_ExternalOutput17 = other.O_ExternalOutput17;
                    O_ExternalOutput18 = other.O_ExternalOutput18;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new RobotGpioStatus(this);
        }

        #endregion Status Override
    }
}
