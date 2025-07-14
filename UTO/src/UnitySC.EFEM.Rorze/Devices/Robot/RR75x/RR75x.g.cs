using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x
{
    public partial class RR75x : UnitySC.Equipment.Abstractions.Devices.Robot.Robot, IRR75x
    {
        public static new readonly DeviceType Type;

        static RR75x()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.Robot.RR75x.RR75x.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.Robot.RR75x.RR75x");
            }
        }

        public RR75x(string name)
            : this(name, Type)
        {
        }

        protected RR75x(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GetStatuses":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetStatuses();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetStatuses(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetStatuses interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public void GetStatuses()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetStatusesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationMode OperationMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationMode)GetStatusValue("OperationMode"); }
            protected set { SetStatusValue("OperationMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OriginReturnCompletion OriginReturnCompletion
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OriginReturnCompletion)GetStatusValue("OriginReturnCompletion"); }
            protected set { SetStatusValue("OriginReturnCompletion", value); }
        }

        public UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing CommandProcessing
        {
            get { return (UnitySC.EFEM.Rorze.Drivers.Enums.CommandProcessing)GetStatusValue("CommandProcessing"); }
            protected set { SetStatusValue("CommandProcessing", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationStatus OperationStatus
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationStatus)GetStatusValue("OperationStatus"); }
            protected set { SetStatusValue("OperationStatus", value); }
        }

        public bool IsNormalSpeed
        {
            get { return (bool)GetStatusValue("IsNormalSpeed"); }
            protected set { SetStatusValue("IsNormalSpeed", value); }
        }

        public string MotionSpeedPercentage
        {
            get { return (string)GetStatusValue("MotionSpeedPercentage"); }
            protected set { SetStatusValue("MotionSpeedPercentage", value); }
        }

        public string ErrorControllerCode
        {
            get { return (string)GetStatusValue("ErrorControllerCode"); }
            protected set { SetStatusValue("ErrorControllerCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.ErrorControllerId ErrorControllerName
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.ErrorControllerId)GetStatusValue("ErrorControllerName"); }
            protected set { SetStatusValue("ErrorControllerName", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public bool I_EmergencyStop_SignalNotConnected
        {
            get { return (bool)GetStatusValue("I_EmergencyStop_SignalNotConnected"); }
            protected set { SetStatusValue("I_EmergencyStop_SignalNotConnected", value); }
        }

        public bool I_Pause_SignalNotConnected
        {
            get { return (bool)GetStatusValue("I_Pause_SignalNotConnected"); }
            protected set { SetStatusValue("I_Pause_SignalNotConnected", value); }
        }

        public bool I_VacuumSourcePressure_SignalNotConnected
        {
            get { return (bool)GetStatusValue("I_VacuumSourcePressure_SignalNotConnected"); }
            protected set { SetStatusValue("I_VacuumSourcePressure_SignalNotConnected", value); }
        }

        public bool I_AirSourcePressure_SignalNotConnected
        {
            get { return (bool)GetStatusValue("I_AirSourcePressure_SignalNotConnected"); }
            protected set { SetStatusValue("I_AirSourcePressure_SignalNotConnected", value); }
        }

        public bool I_ExhaustFan
        {
            get { return (bool)GetStatusValue("I_ExhaustFan"); }
            protected set { SetStatusValue("I_ExhaustFan", value); }
        }

        public bool I_ExhaustFan_ForUpperArm
        {
            get { return (bool)GetStatusValue("I_ExhaustFan_ForUpperArm"); }
            protected set { SetStatusValue("I_ExhaustFan_ForUpperArm", value); }
        }

        public bool I_ExhaustFan_ForLowerArm
        {
            get { return (bool)GetStatusValue("I_ExhaustFan_ForLowerArm"); }
            protected set { SetStatusValue("I_ExhaustFan_ForLowerArm", value); }
        }

        public bool I_UpperArm_Finger1_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger1_WaferPresence1"); }
            protected set { SetStatusValue("I_UpperArm_Finger1_WaferPresence1", value); }
        }

        public bool I_UpperArm_Finger1_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger1_WaferPresence2"); }
            protected set { SetStatusValue("I_UpperArm_Finger1_WaferPresence2", value); }
        }

        public bool I_UpperArm_Finger2_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger2_WaferPresence1"); }
            protected set { SetStatusValue("I_UpperArm_Finger2_WaferPresence1", value); }
        }

        public bool I_UpperArm_Finger2_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger2_WaferPresence2"); }
            protected set { SetStatusValue("I_UpperArm_Finger2_WaferPresence2", value); }
        }

        public bool I_UpperArm_Finger3_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger3_WaferPresence1"); }
            protected set { SetStatusValue("I_UpperArm_Finger3_WaferPresence1", value); }
        }

        public bool I_UpperArm_Finger3_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger3_WaferPresence2"); }
            protected set { SetStatusValue("I_UpperArm_Finger3_WaferPresence2", value); }
        }

        public bool I_UpperArm_Finger4_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger4_WaferPresence1"); }
            protected set { SetStatusValue("I_UpperArm_Finger4_WaferPresence1", value); }
        }

        public bool I_UpperArm_Finger4_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger4_WaferPresence2"); }
            protected set { SetStatusValue("I_UpperArm_Finger4_WaferPresence2", value); }
        }

        public bool I_UpperArm_Finger5_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger5_WaferPresence1"); }
            protected set { SetStatusValue("I_UpperArm_Finger5_WaferPresence1", value); }
        }

        public bool I_UpperArm_Finger5_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_UpperArm_Finger5_WaferPresence2"); }
            protected set { SetStatusValue("I_UpperArm_Finger5_WaferPresence2", value); }
        }

        public bool I_LowerArm_WaferPresence1
        {
            get { return (bool)GetStatusValue("I_LowerArm_WaferPresence1"); }
            protected set { SetStatusValue("I_LowerArm_WaferPresence1", value); }
        }

        public bool I_LowerArm_WaferPresence2
        {
            get { return (bool)GetStatusValue("I_LowerArm_WaferPresence2"); }
            protected set { SetStatusValue("I_LowerArm_WaferPresence2", value); }
        }

        public bool I_EmergencyStop_TeachingPendant
        {
            get { return (bool)GetStatusValue("I_EmergencyStop_TeachingPendant"); }
            protected set { SetStatusValue("I_EmergencyStop_TeachingPendant", value); }
        }

        public bool I_DeadManSwitch
        {
            get { return (bool)GetStatusValue("I_DeadManSwitch"); }
            protected set { SetStatusValue("I_DeadManSwitch", value); }
        }

        public bool I_ModeKey
        {
            get { return (bool)GetStatusValue("I_ModeKey"); }
            protected set { SetStatusValue("I_ModeKey", value); }
        }

        public bool I_InterlockInput00
        {
            get { return (bool)GetStatusValue("I_InterlockInput00"); }
            protected set { SetStatusValue("I_InterlockInput00", value); }
        }

        public bool I_InterlockInput01
        {
            get { return (bool)GetStatusValue("I_InterlockInput01"); }
            protected set { SetStatusValue("I_InterlockInput01", value); }
        }

        public bool I_InterlockInput02
        {
            get { return (bool)GetStatusValue("I_InterlockInput02"); }
            protected set { SetStatusValue("I_InterlockInput02", value); }
        }

        public bool I_InterlockInput03
        {
            get { return (bool)GetStatusValue("I_InterlockInput03"); }
            protected set { SetStatusValue("I_InterlockInput03", value); }
        }

        public bool I_Sensor1ForTeaching
        {
            get { return (bool)GetStatusValue("I_Sensor1ForTeaching"); }
            protected set { SetStatusValue("I_Sensor1ForTeaching", value); }
        }

        public bool I_Sensor2ForTeaching
        {
            get { return (bool)GetStatusValue("I_Sensor2ForTeaching"); }
            protected set { SetStatusValue("I_Sensor2ForTeaching", value); }
        }

        public bool I_ExternalInput1
        {
            get { return (bool)GetStatusValue("I_ExternalInput1"); }
            protected set { SetStatusValue("I_ExternalInput1", value); }
        }

        public bool I_ExternalInput2
        {
            get { return (bool)GetStatusValue("I_ExternalInput2"); }
            protected set { SetStatusValue("I_ExternalInput2", value); }
        }

        public bool I_ExternalInput3
        {
            get { return (bool)GetStatusValue("I_ExternalInput3"); }
            protected set { SetStatusValue("I_ExternalInput3", value); }
        }

        public bool I_ExternalInput4
        {
            get { return (bool)GetStatusValue("I_ExternalInput4"); }
            protected set { SetStatusValue("I_ExternalInput4", value); }
        }

        public bool I_ExternalInput5
        {
            get { return (bool)GetStatusValue("I_ExternalInput5"); }
            protected set { SetStatusValue("I_ExternalInput5", value); }
        }

        public bool I_ExternalInput6
        {
            get { return (bool)GetStatusValue("I_ExternalInput6"); }
            protected set { SetStatusValue("I_ExternalInput6", value); }
        }

        public bool I_ExternalInput7
        {
            get { return (bool)GetStatusValue("I_ExternalInput7"); }
            protected set { SetStatusValue("I_ExternalInput7", value); }
        }

        public bool I_ExternalInput8
        {
            get { return (bool)GetStatusValue("I_ExternalInput8"); }
            protected set { SetStatusValue("I_ExternalInput8", value); }
        }

        public bool I_ExternalInput9
        {
            get { return (bool)GetStatusValue("I_ExternalInput9"); }
            protected set { SetStatusValue("I_ExternalInput9", value); }
        }

        public bool I_ExternalInput10
        {
            get { return (bool)GetStatusValue("I_ExternalInput10"); }
            protected set { SetStatusValue("I_ExternalInput10", value); }
        }

        public bool I_ExternalInput11
        {
            get { return (bool)GetStatusValue("I_ExternalInput11"); }
            protected set { SetStatusValue("I_ExternalInput11", value); }
        }

        public bool I_ExternalInput12
        {
            get { return (bool)GetStatusValue("I_ExternalInput12"); }
            protected set { SetStatusValue("I_ExternalInput12", value); }
        }

        public bool I_ExternalInput13
        {
            get { return (bool)GetStatusValue("I_ExternalInput13"); }
            protected set { SetStatusValue("I_ExternalInput13", value); }
        }

        public bool I_ExternalInput14
        {
            get { return (bool)GetStatusValue("I_ExternalInput14"); }
            protected set { SetStatusValue("I_ExternalInput14", value); }
        }

        public bool I_ExternalInput15
        {
            get { return (bool)GetStatusValue("I_ExternalInput15"); }
            protected set { SetStatusValue("I_ExternalInput15", value); }
        }

        public bool I_ExternalInput16
        {
            get { return (bool)GetStatusValue("I_ExternalInput16"); }
            protected set { SetStatusValue("I_ExternalInput16", value); }
        }

        public bool I_ExternalInput17
        {
            get { return (bool)GetStatusValue("I_ExternalInput17"); }
            protected set { SetStatusValue("I_ExternalInput17", value); }
        }

        public bool I_ExternalInput18
        {
            get { return (bool)GetStatusValue("I_ExternalInput18"); }
            protected set { SetStatusValue("I_ExternalInput18", value); }
        }

        public bool I_Sensor1ForTeaching_Ext
        {
            get { return (bool)GetStatusValue("I_Sensor1ForTeaching_Ext"); }
            protected set { SetStatusValue("I_Sensor1ForTeaching_Ext", value); }
        }

        public bool I_Sensor2ForTeaching_Ext
        {
            get { return (bool)GetStatusValue("I_Sensor2ForTeaching_Ext"); }
            protected set { SetStatusValue("I_Sensor2ForTeaching_Ext", value); }
        }

        public bool O_PreparationComplete_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_PreparationComplete_SignalNotConnected"); }
            protected set { SetStatusValue("O_PreparationComplete_SignalNotConnected", value); }
        }

        public bool O_Pause_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_Pause_SignalNotConnected"); }
            protected set { SetStatusValue("O_Pause_SignalNotConnected", value); }
        }

        public bool O_FatalError_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_FatalError_SignalNotConnected"); }
            protected set { SetStatusValue("O_FatalError_SignalNotConnected", value); }
        }

        public bool O_LightError_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_LightError_SignalNotConnected"); }
            protected set { SetStatusValue("O_LightError_SignalNotConnected", value); }
        }

        public bool O_ZAxisBrakeOFF_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_ZAxisBrakeOFF_SignalNotConnected"); }
            protected set { SetStatusValue("O_ZAxisBrakeOFF_SignalNotConnected", value); }
        }

        public bool O_BatteryVoltageTooLow_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_BatteryVoltageTooLow_SignalNotConnected"); }
            protected set { SetStatusValue("O_BatteryVoltageTooLow_SignalNotConnected", value); }
        }

        public bool O_DrivePower_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_DrivePower_SignalNotConnected"); }
            protected set { SetStatusValue("O_DrivePower_SignalNotConnected", value); }
        }

        public bool O_TorqueLimitation_SignalNotConnected
        {
            get { return (bool)GetStatusValue("O_TorqueLimitation_SignalNotConnected"); }
            protected set { SetStatusValue("O_TorqueLimitation_SignalNotConnected", value); }
        }

        public bool O_UpperArm_Finger1_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger1_SolenoidValveOn"); }
            protected set { SetStatusValue("O_UpperArm_Finger1_SolenoidValveOn", value); }
        }

        public bool O_UpperArm_Finger1_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger1_SolenoidValveOff"); }
            protected set { SetStatusValue("O_UpperArm_Finger1_SolenoidValveOff", value); }
        }

        public bool O_UpperArm_Finger2_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger2_SolenoidValveOn"); }
            protected set { SetStatusValue("O_UpperArm_Finger2_SolenoidValveOn", value); }
        }

        public bool O_UpperArm_Finger2_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger2_SolenoidValveOff"); }
            protected set { SetStatusValue("O_UpperArm_Finger2_SolenoidValveOff", value); }
        }

        public bool O_UpperArm_Finger3_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger3_SolenoidValveOn"); }
            protected set { SetStatusValue("O_UpperArm_Finger3_SolenoidValveOn", value); }
        }

        public bool O_UpperArm_Finger3_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger3_SolenoidValveOff"); }
            protected set { SetStatusValue("O_UpperArm_Finger3_SolenoidValveOff", value); }
        }

        public bool O_UpperArm_Finger4_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger4_SolenoidValveOn"); }
            protected set { SetStatusValue("O_UpperArm_Finger4_SolenoidValveOn", value); }
        }

        public bool O_UpperArm_Finger4_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger4_SolenoidValveOff"); }
            protected set { SetStatusValue("O_UpperArm_Finger4_SolenoidValveOff", value); }
        }

        public bool O_UpperArm_Finger5_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger5_SolenoidValveOn"); }
            protected set { SetStatusValue("O_UpperArm_Finger5_SolenoidValveOn", value); }
        }

        public bool O_UpperArm_Finger5_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_UpperArm_Finger5_SolenoidValveOff"); }
            protected set { SetStatusValue("O_UpperArm_Finger5_SolenoidValveOff", value); }
        }

        public bool O_LowerArm_SolenoidValveOn
        {
            get { return (bool)GetStatusValue("O_LowerArm_SolenoidValveOn"); }
            protected set { SetStatusValue("O_LowerArm_SolenoidValveOn", value); }
        }

        public bool O_LowerArm_SolenoidValveOff
        {
            get { return (bool)GetStatusValue("O_LowerArm_SolenoidValveOff"); }
            protected set { SetStatusValue("O_LowerArm_SolenoidValveOff", value); }
        }

        public bool O_XAxis_ExcitationOnOff_LogicSignal
        {
            get { return (bool)GetStatusValue("O_XAxis_ExcitationOnOff_LogicSignal"); }
            protected set { SetStatusValue("O_XAxis_ExcitationOnOff_LogicSignal", value); }
        }

        public bool O_ZAxis_ExcitationOnOff_LogicSignal
        {
            get { return (bool)GetStatusValue("O_ZAxis_ExcitationOnOff_LogicSignal"); }
            protected set { SetStatusValue("O_ZAxis_ExcitationOnOff_LogicSignal", value); }
        }

        public bool O_RotationAxisExcitationOnOff_LogicSignal
        {
            get { return (bool)GetStatusValue("O_RotationAxisExcitationOnOff_LogicSignal"); }
            protected set { SetStatusValue("O_RotationAxisExcitationOnOff_LogicSignal", value); }
        }

        public bool O_UpperArmExcitationOnOff_LogicSignal
        {
            get { return (bool)GetStatusValue("O_UpperArmExcitationOnOff_LogicSignal"); }
            protected set { SetStatusValue("O_UpperArmExcitationOnOff_LogicSignal", value); }
        }

        public bool O_LowerArmExcitationOnOff_LogicSignal
        {
            get { return (bool)GetStatusValue("O_LowerArmExcitationOnOff_LogicSignal"); }
            protected set { SetStatusValue("O_LowerArmExcitationOnOff_LogicSignal", value); }
        }

        public bool O_UpperArmOrigin_LogicSignal
        {
            get { return (bool)GetStatusValue("O_UpperArmOrigin_LogicSignal"); }
            protected set { SetStatusValue("O_UpperArmOrigin_LogicSignal", value); }
        }

        public bool O_LowerArmOrigin_LogicSignal
        {
            get { return (bool)GetStatusValue("O_LowerArmOrigin_LogicSignal"); }
            protected set { SetStatusValue("O_LowerArmOrigin_LogicSignal", value); }
        }

        public bool O_ExternalOutput1
        {
            get { return (bool)GetStatusValue("O_ExternalOutput1"); }
            protected set { SetStatusValue("O_ExternalOutput1", value); }
        }

        public bool O_ExternalOutput2
        {
            get { return (bool)GetStatusValue("O_ExternalOutput2"); }
            protected set { SetStatusValue("O_ExternalOutput2", value); }
        }

        public bool O_ExternalOutput3
        {
            get { return (bool)GetStatusValue("O_ExternalOutput3"); }
            protected set { SetStatusValue("O_ExternalOutput3", value); }
        }

        public bool O_ExternalOutput4
        {
            get { return (bool)GetStatusValue("O_ExternalOutput4"); }
            protected set { SetStatusValue("O_ExternalOutput4", value); }
        }

        public bool O_ExternalOutput5
        {
            get { return (bool)GetStatusValue("O_ExternalOutput5"); }
            protected set { SetStatusValue("O_ExternalOutput5", value); }
        }

        public bool O_ExternalOutput6
        {
            get { return (bool)GetStatusValue("O_ExternalOutput6"); }
            protected set { SetStatusValue("O_ExternalOutput6", value); }
        }

        public bool O_ExternalOutput7
        {
            get { return (bool)GetStatusValue("O_ExternalOutput7"); }
            protected set { SetStatusValue("O_ExternalOutput7", value); }
        }

        public bool O_ExternalOutput8
        {
            get { return (bool)GetStatusValue("O_ExternalOutput8"); }
            protected set { SetStatusValue("O_ExternalOutput8", value); }
        }

        public bool O_ExternalOutput9
        {
            get { return (bool)GetStatusValue("O_ExternalOutput9"); }
            protected set { SetStatusValue("O_ExternalOutput9", value); }
        }

        public bool O_ExternalOutput10
        {
            get { return (bool)GetStatusValue("O_ExternalOutput10"); }
            protected set { SetStatusValue("O_ExternalOutput10", value); }
        }

        public bool O_ExternalOutput11
        {
            get { return (bool)GetStatusValue("O_ExternalOutput11"); }
            protected set { SetStatusValue("O_ExternalOutput11", value); }
        }

        public bool O_ExternalOutput12
        {
            get { return (bool)GetStatusValue("O_ExternalOutput12"); }
            protected set { SetStatusValue("O_ExternalOutput12", value); }
        }

        public bool O_ExternalOutput13
        {
            get { return (bool)GetStatusValue("O_ExternalOutput13"); }
            protected set { SetStatusValue("O_ExternalOutput13", value); }
        }

        public bool O_ExternalOutput14
        {
            get { return (bool)GetStatusValue("O_ExternalOutput14"); }
            protected set { SetStatusValue("O_ExternalOutput14", value); }
        }

        public bool O_ExternalOutput15
        {
            get { return (bool)GetStatusValue("O_ExternalOutput15"); }
            protected set { SetStatusValue("O_ExternalOutput15", value); }
        }

        public bool O_ExternalOutput16
        {
            get { return (bool)GetStatusValue("O_ExternalOutput16"); }
            protected set { SetStatusValue("O_ExternalOutput16", value); }
        }

        public bool O_ExternalOutput17
        {
            get { return (bool)GetStatusValue("O_ExternalOutput17"); }
            protected set { SetStatusValue("O_ExternalOutput17", value); }
        }

        public bool O_ExternalOutput18
        {
            get { return (bool)GetStatusValue("O_ExternalOutput18"); }
            protected set { SetStatusValue("O_ExternalOutput18", value); }
        }

        public string Version
        {
            get { return (string)GetStatusValue("Version"); }
            protected set { SetStatusValue("Version", value); }
        }
    }
}
