using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0
{
    public partial class Dio0 : UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.GenericRC5xx, IDio0
    {
        public static new readonly DeviceType Type;

        static Dio0()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Dio0.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Dio0");
            }
        }

        public Dio0(string name)
            : this(name, Type)
        {
        }

        protected Dio0(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "SetFfuSpeed":
                    {
                        UnitsNet.RotationalSpeed setpoint = (UnitsNet.RotationalSpeed)execution.Context.GetArgument("setPoint");
                        UnitsNet.Units.RotationalSpeedUnit? unitsetPoint = execution.Context.Command.Parameters.FirstOrDefault(p => p.Name == "setPoint").Unit as UnitsNet.Units.RotationalSpeedUnit?;
                        if (unitsetPoint != null && unitsetPoint != setpoint.Unit)
                        {
                            setpoint = setpoint.ToUnit(unitsetPoint.Value);
                        }

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetFfuSpeed(setpoint);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetFfuSpeed(setpoint, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetFfuSpeed interrupted.");
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

        public void SetFfuSpeed(UnitsNet.RotationalSpeed setPoint)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("setPoint", setPoint));
            CommandExecution execution = new CommandExecution(this, "SetFfuSpeed", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetFfuSpeedAsync(UnitsNet.RotationalSpeed setPoint)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("setPoint", setPoint));
            CommandExecution execution = new CommandExecution(this, "SetFfuSpeed", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums.IoModuleInError IoModuleInErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums.IoModuleInError)GetStatusValue("IoModuleInErrorDescription"); }
            protected set { SetStatusValue("IoModuleInErrorDescription", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public UnitsNet.Pressure MeanPressure
        {
            get { return (UnitsNet.Pressure)GetStatusValue("MeanPressure"); }
            protected set
            {
                UnitsNet.Units.PressureUnit? unit = DeviceType.AllStatuses().Get("MeanPressure").Unit as UnitsNet.Units.PressureUnit?;
                UnitsNet.Pressure newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("MeanPressure", newValue);
            }
        }

        public bool I_DvrAlarm
        {
            get { return (bool)GetStatusValue("I_DvrAlarm"); }
            protected set { SetStatusValue("I_DvrAlarm", value); }
        }

        public UnitsNet.RotationalSpeed FanSpeed
        {
            get { return (UnitsNet.RotationalSpeed)GetStatusValue("FanSpeed"); }
            protected set
            {
                UnitsNet.Units.RotationalSpeedUnit? unit = DeviceType.AllStatuses().Get("FanSpeed").Unit as UnitsNet.Units.RotationalSpeedUnit?;
                UnitsNet.RotationalSpeed newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("FanSpeed", newValue);
            }
        }

        public bool I_FANDetection1
        {
            get { return (bool)GetStatusValue("I_FANDetection1"); }
            protected set { SetStatusValue("I_FANDetection1", value); }
        }

        public bool I_FANDetection2
        {
            get { return (bool)GetStatusValue("I_FANDetection2"); }
            protected set { SetStatusValue("I_FANDetection2", value); }
        }

        public bool I_FAN1Rotating
        {
            get { return (bool)GetStatusValue("I_FAN1Rotating"); }
            protected set { SetStatusValue("I_FAN1Rotating", value); }
        }

        public bool I_FAN2Rotating
        {
            get { return (bool)GetStatusValue("I_FAN2Rotating"); }
            protected set { SetStatusValue("I_FAN2Rotating", value); }
        }

        public bool I_FAN3Rotating
        {
            get { return (bool)GetStatusValue("I_FAN3Rotating"); }
            protected set { SetStatusValue("I_FAN3Rotating", value); }
        }

        public bool I_FAN4Rotating
        {
            get { return (bool)GetStatusValue("I_FAN4Rotating"); }
            protected set { SetStatusValue("I_FAN4Rotating", value); }
        }

        public bool I_FAN5Rotating
        {
            get { return (bool)GetStatusValue("I_FAN5Rotating"); }
            protected set { SetStatusValue("I_FAN5Rotating", value); }
        }

        public bool I_FAN6Rotating
        {
            get { return (bool)GetStatusValue("I_FAN6Rotating"); }
            protected set { SetStatusValue("I_FAN6Rotating", value); }
        }

        public bool I_FAN7Rotating
        {
            get { return (bool)GetStatusValue("I_FAN7Rotating"); }
            protected set { SetStatusValue("I_FAN7Rotating", value); }
        }

        public bool I_FAN8Rotating
        {
            get { return (bool)GetStatusValue("I_FAN8Rotating"); }
            protected set { SetStatusValue("I_FAN8Rotating", value); }
        }

        public bool I_FAN9Rotating
        {
            get { return (bool)GetStatusValue("I_FAN9Rotating"); }
            protected set { SetStatusValue("I_FAN9Rotating", value); }
        }

        public bool I_FAN10Rotating
        {
            get { return (bool)GetStatusValue("I_FAN10Rotating"); }
            protected set { SetStatusValue("I_FAN10Rotating", value); }
        }

        public bool I_FAN11Rotating
        {
            get { return (bool)GetStatusValue("I_FAN11Rotating"); }
            protected set { SetStatusValue("I_FAN11Rotating", value); }
        }

        public bool I_FAN12Rotating
        {
            get { return (bool)GetStatusValue("I_FAN12Rotating"); }
            protected set { SetStatusValue("I_FAN12Rotating", value); }
        }

        public bool I_FAN13Rotating
        {
            get { return (bool)GetStatusValue("I_FAN13Rotating"); }
            protected set { SetStatusValue("I_FAN13Rotating", value); }
        }

        public bool I_FAN14Rotating
        {
            get { return (bool)GetStatusValue("I_FAN14Rotating"); }
            protected set { SetStatusValue("I_FAN14Rotating", value); }
        }

        public bool I_FAN15Rotating
        {
            get { return (bool)GetStatusValue("I_FAN15Rotating"); }
            protected set { SetStatusValue("I_FAN15Rotating", value); }
        }

        public bool I_FAN16Rotating
        {
            get { return (bool)GetStatusValue("I_FAN16Rotating"); }
            protected set { SetStatusValue("I_FAN16Rotating", value); }
        }

        public bool I_FAN17Rotating
        {
            get { return (bool)GetStatusValue("I_FAN17Rotating"); }
            protected set { SetStatusValue("I_FAN17Rotating", value); }
        }

        public bool I_FAN18Rotating
        {
            get { return (bool)GetStatusValue("I_FAN18Rotating"); }
            protected set { SetStatusValue("I_FAN18Rotating", value); }
        }

        public bool I_FAN19Rotating
        {
            get { return (bool)GetStatusValue("I_FAN19Rotating"); }
            protected set { SetStatusValue("I_FAN19Rotating", value); }
        }

        public bool I_FAN20Rotating
        {
            get { return (bool)GetStatusValue("I_FAN20Rotating"); }
            protected set { SetStatusValue("I_FAN20Rotating", value); }
        }

        public bool I_FAN1AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN1AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN1AlarmOccurred", value); }
        }

        public bool I_FAN2AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN2AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN2AlarmOccurred", value); }
        }

        public bool I_FAN3AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN3AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN3AlarmOccurred", value); }
        }

        public bool I_FAN4AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN4AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN4AlarmOccurred", value); }
        }

        public bool I_FAN5AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN5AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN5AlarmOccurred", value); }
        }

        public bool I_FAN6AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN6AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN6AlarmOccurred", value); }
        }

        public bool I_FAN7AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN7AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN7AlarmOccurred", value); }
        }

        public bool I_FAN8AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN8AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN8AlarmOccurred", value); }
        }

        public bool I_FAN9AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN9AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN9AlarmOccurred", value); }
        }

        public bool I_FAN10AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN10AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN10AlarmOccurred", value); }
        }

        public bool I_FAN11AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN11AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN11AlarmOccurred", value); }
        }

        public bool I_FAN12AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN12AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN12AlarmOccurred", value); }
        }

        public bool I_FAN13AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN13AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN13AlarmOccurred", value); }
        }

        public bool I_FAN14AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN14AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN14AlarmOccurred", value); }
        }

        public bool I_FAN15AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN15AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN15AlarmOccurred", value); }
        }

        public bool I_FAN16AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN16AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN16AlarmOccurred", value); }
        }

        public bool I_FAN17AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN17AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN17AlarmOccurred", value); }
        }

        public bool I_FAN18AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN18AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN18AlarmOccurred", value); }
        }

        public bool I_FAN19AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN19AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN19AlarmOccurred", value); }
        }

        public bool I_FAN20AlarmOccurred
        {
            get { return (bool)GetStatusValue("I_FAN20AlarmOccurred"); }
            protected set { SetStatusValue("I_FAN20AlarmOccurred", value); }
        }

        public bool I_Sensor1_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor1_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor1_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor1_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor1_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor1_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor2_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor2_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor2_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor2_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor2_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor2_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor3_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor3_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor3_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor3_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor3_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor3_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor4_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor4_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor4_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor4_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor4_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor4_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor5_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor5_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor5_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor5_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor5_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor5_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor6_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor6_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor6_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor6_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor6_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor6_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor7_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor7_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor7_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor7_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor7_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor7_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor8_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor8_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor8_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor8_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor8_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor8_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor9_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor9_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor9_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor9_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor9_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor9_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor10_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor10_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor10_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor10_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor10_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor10_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor11_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor11_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor11_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor11_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor11_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor11_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_Sensor12_WithinUpperLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor12_WithinUpperLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor12_WithinUpperLimitThresholdValue", value); }
        }

        public bool I_Sensor12_WithinLowerLimitThresholdValue
        {
            get { return (bool)GetStatusValue("I_Sensor12_WithinLowerLimitThresholdValue"); }
            protected set { SetStatusValue("I_Sensor12_WithinLowerLimitThresholdValue", value); }
        }

        public bool I_ControllerDirectInput_IN0
        {
            get { return (bool)GetStatusValue("I_ControllerDirectInput_IN0"); }
            protected set { SetStatusValue("I_ControllerDirectInput_IN0", value); }
        }

        public bool I_ControllerDirectInput_IN1
        {
            get { return (bool)GetStatusValue("I_ControllerDirectInput_IN1"); }
            protected set { SetStatusValue("I_ControllerDirectInput_IN1", value); }
        }

        public bool I_ControllerDirectInput_IN2
        {
            get { return (bool)GetStatusValue("I_ControllerDirectInput_IN2"); }
            protected set { SetStatusValue("I_ControllerDirectInput_IN2", value); }
        }

        public bool I_ControllerDirectInput_IN3
        {
            get { return (bool)GetStatusValue("I_ControllerDirectInput_IN3"); }
            protected set { SetStatusValue("I_ControllerDirectInput_IN3", value); }
        }

        public bool O_SystemIsReady
        {
            get { return (bool)GetStatusValue("O_SystemIsReady"); }
            protected set { SetStatusValue("O_SystemIsReady", value); }
        }

        public bool O_BatchAlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_BatchAlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_BatchAlarmClear_1ShotOutput", value); }
        }

        public bool O_Fan_OperationStop_AllUsingFans_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_Fan_OperationStop_AllUsingFans_1ShotOutput"); }
            protected set { SetStatusValue("O_Fan_OperationStop_AllUsingFans_1ShotOutput", value); }
        }

        public bool O_Fan_OperationStart_AllUsingFans_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_Fan_OperationStart_AllUsingFans_1ShotOutput"); }
            protected set { SetStatusValue("O_Fan_OperationStart_AllUsingFans_1ShotOutput", value); }
        }

        public bool O_FAN1_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN1_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN1_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN2_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN2_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN2_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN3_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN3_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN3_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN4_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN4_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN4_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN5_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN5_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN5_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN6_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN6_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN6_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN7_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN7_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN7_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN8_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN8_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN8_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN9_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN9_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN9_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN10_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN10_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN10_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN11_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN11_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN11_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN12_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN12_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN12_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN13_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN13_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN13_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN14_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN14_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN14_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN15_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN15_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN15_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN16_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN16_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN16_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN17_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN17_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN17_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN18_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN18_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN18_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN19_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN19_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN19_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN20_OperationStart_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN20_OperationStart_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN20_OperationStart_1ShotOutput", value); }
        }

        public bool O_FAN1_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN1_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN1_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN2_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN2_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN2_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN3_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN3_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN3_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN4_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN4_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN4_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN5_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN5_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN5_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN6_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN6_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN6_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN7_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN7_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN7_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN8_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN8_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN8_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN9_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN9_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN9_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN10_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN10_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN10_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN11_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN11_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN11_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN12_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN12_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN12_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN13_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN13_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN13_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN14_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN14_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN14_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN15_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN15_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN15_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN16_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN16_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN16_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN17_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN17_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN17_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN18_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN18_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN18_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN19_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN19_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN19_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN20_AlarmClear_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN20_AlarmClear_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN20_AlarmClear_1ShotOutput", value); }
        }

        public bool O_FAN1_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN1_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN1_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN2_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN2_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN2_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN3_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN3_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN3_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN4_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN4_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN4_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN5_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN5_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN5_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN6_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN6_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN6_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN7_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN7_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN7_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN8_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN8_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN8_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN9_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN9_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN9_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN10_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN10_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN10_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN11_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN11_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN11_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN12_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN12_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN12_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN13_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN13_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN13_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN14_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN14_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN14_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN15_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN15_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN15_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN16_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN16_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN16_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN17_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN17_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN17_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN18_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN18_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN18_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN19_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN19_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN19_OperationStop_1ShotOutput", value); }
        }

        public bool O_FAN20_OperationStop_1ShotOutput
        {
            get { return (bool)GetStatusValue("O_FAN20_OperationStop_1ShotOutput"); }
            protected set { SetStatusValue("O_FAN20_OperationStop_1ShotOutput", value); }
        }

        public bool I_Lp1_Valid
        {
            get { return (bool)GetStatusValue("I_Lp1_Valid"); }
            protected set { SetStatusValue("I_Lp1_Valid", value); }
        }

        public bool I_Lp1_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp1_Cs_0"); }
            protected set { SetStatusValue("I_Lp1_Cs_0", value); }
        }

        public bool I_Lp1_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp1_Cs_1"); }
            protected set { SetStatusValue("I_Lp1_Cs_1", value); }
        }

        public bool I_Lp1_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp1_Tr_Req"); }
            protected set { SetStatusValue("I_Lp1_Tr_Req", value); }
        }

        public bool I_Lp1_Busy
        {
            get { return (bool)GetStatusValue("I_Lp1_Busy"); }
            protected set { SetStatusValue("I_Lp1_Busy", value); }
        }

        public bool I_Lp1_Compt
        {
            get { return (bool)GetStatusValue("I_Lp1_Compt"); }
            protected set { SetStatusValue("I_Lp1_Compt", value); }
        }

        public bool I_Lp1_Cont
        {
            get { return (bool)GetStatusValue("I_Lp1_Cont"); }
            protected set { SetStatusValue("I_Lp1_Cont", value); }
        }

        public bool I_Lp2_Valid
        {
            get { return (bool)GetStatusValue("I_Lp2_Valid"); }
            protected set { SetStatusValue("I_Lp2_Valid", value); }
        }

        public bool I_Lp2_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp2_Cs_0"); }
            protected set { SetStatusValue("I_Lp2_Cs_0", value); }
        }

        public bool I_Lp2_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp2_Cs_1"); }
            protected set { SetStatusValue("I_Lp2_Cs_1", value); }
        }

        public bool I_Lp2_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp2_Tr_Req"); }
            protected set { SetStatusValue("I_Lp2_Tr_Req", value); }
        }

        public bool I_Lp2_Busy
        {
            get { return (bool)GetStatusValue("I_Lp2_Busy"); }
            protected set { SetStatusValue("I_Lp2_Busy", value); }
        }

        public bool I_Lp2_Compt
        {
            get { return (bool)GetStatusValue("I_Lp2_Compt"); }
            protected set { SetStatusValue("I_Lp2_Compt", value); }
        }

        public bool I_Lp2_Cont
        {
            get { return (bool)GetStatusValue("I_Lp2_Cont"); }
            protected set { SetStatusValue("I_Lp2_Cont", value); }
        }

        public bool I_Lp3_Valid
        {
            get { return (bool)GetStatusValue("I_Lp3_Valid"); }
            protected set { SetStatusValue("I_Lp3_Valid", value); }
        }

        public bool I_Lp3_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp3_Cs_0"); }
            protected set { SetStatusValue("I_Lp3_Cs_0", value); }
        }

        public bool I_Lp3_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp3_Cs_1"); }
            protected set { SetStatusValue("I_Lp3_Cs_1", value); }
        }

        public bool I_Lp3_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp3_Tr_Req"); }
            protected set { SetStatusValue("I_Lp3_Tr_Req", value); }
        }

        public bool I_Lp3_Busy
        {
            get { return (bool)GetStatusValue("I_Lp3_Busy"); }
            protected set { SetStatusValue("I_Lp3_Busy", value); }
        }

        public bool I_Lp3_Compt
        {
            get { return (bool)GetStatusValue("I_Lp3_Compt"); }
            protected set { SetStatusValue("I_Lp3_Compt", value); }
        }

        public bool I_Lp3_Cont
        {
            get { return (bool)GetStatusValue("I_Lp3_Cont"); }
            protected set { SetStatusValue("I_Lp3_Cont", value); }
        }

        public bool I_Lp4_Valid
        {
            get { return (bool)GetStatusValue("I_Lp4_Valid"); }
            protected set { SetStatusValue("I_Lp4_Valid", value); }
        }

        public bool I_Lp4_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp4_Cs_0"); }
            protected set { SetStatusValue("I_Lp4_Cs_0", value); }
        }

        public bool I_Lp4_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp4_Cs_1"); }
            protected set { SetStatusValue("I_Lp4_Cs_1", value); }
        }

        public bool I_Lp4_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp4_Tr_Req"); }
            protected set { SetStatusValue("I_Lp4_Tr_Req", value); }
        }

        public bool I_Lp4_Busy
        {
            get { return (bool)GetStatusValue("I_Lp4_Busy"); }
            protected set { SetStatusValue("I_Lp4_Busy", value); }
        }

        public bool I_Lp4_Compt
        {
            get { return (bool)GetStatusValue("I_Lp4_Compt"); }
            protected set { SetStatusValue("I_Lp4_Compt", value); }
        }

        public bool I_Lp4_Cont
        {
            get { return (bool)GetStatusValue("I_Lp4_Cont"); }
            protected set { SetStatusValue("I_Lp4_Cont", value); }
        }

        public bool I_Lp5_Valid
        {
            get { return (bool)GetStatusValue("I_Lp5_Valid"); }
            protected set { SetStatusValue("I_Lp5_Valid", value); }
        }

        public bool I_Lp5_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp5_Cs_0"); }
            protected set { SetStatusValue("I_Lp5_Cs_0", value); }
        }

        public bool I_Lp5_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp5_Cs_1"); }
            protected set { SetStatusValue("I_Lp5_Cs_1", value); }
        }

        public bool I_Lp5_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp5_Tr_Req"); }
            protected set { SetStatusValue("I_Lp5_Tr_Req", value); }
        }

        public bool I_Lp5_Busy
        {
            get { return (bool)GetStatusValue("I_Lp5_Busy"); }
            protected set { SetStatusValue("I_Lp5_Busy", value); }
        }

        public bool I_Lp5_Compt
        {
            get { return (bool)GetStatusValue("I_Lp5_Compt"); }
            protected set { SetStatusValue("I_Lp5_Compt", value); }
        }

        public bool I_Lp5_Cont
        {
            get { return (bool)GetStatusValue("I_Lp5_Cont"); }
            protected set { SetStatusValue("I_Lp5_Cont", value); }
        }

        public bool I_Lp6_Valid
        {
            get { return (bool)GetStatusValue("I_Lp6_Valid"); }
            protected set { SetStatusValue("I_Lp6_Valid", value); }
        }

        public bool I_Lp6_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp6_Cs_0"); }
            protected set { SetStatusValue("I_Lp6_Cs_0", value); }
        }

        public bool I_Lp6_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp6_Cs_1"); }
            protected set { SetStatusValue("I_Lp6_Cs_1", value); }
        }

        public bool I_Lp6_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp6_Tr_Req"); }
            protected set { SetStatusValue("I_Lp6_Tr_Req", value); }
        }

        public bool I_Lp6_Busy
        {
            get { return (bool)GetStatusValue("I_Lp6_Busy"); }
            protected set { SetStatusValue("I_Lp6_Busy", value); }
        }

        public bool I_Lp6_Compt
        {
            get { return (bool)GetStatusValue("I_Lp6_Compt"); }
            protected set { SetStatusValue("I_Lp6_Compt", value); }
        }

        public bool I_Lp6_Cont
        {
            get { return (bool)GetStatusValue("I_Lp6_Cont"); }
            protected set { SetStatusValue("I_Lp6_Cont", value); }
        }

        public bool I_Lp7_Valid
        {
            get { return (bool)GetStatusValue("I_Lp7_Valid"); }
            protected set { SetStatusValue("I_Lp7_Valid", value); }
        }

        public bool I_Lp7_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp7_Cs_0"); }
            protected set { SetStatusValue("I_Lp7_Cs_0", value); }
        }

        public bool I_Lp7_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp7_Cs_1"); }
            protected set { SetStatusValue("I_Lp7_Cs_1", value); }
        }

        public bool I_Lp7_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp7_Tr_Req"); }
            protected set { SetStatusValue("I_Lp7_Tr_Req", value); }
        }

        public bool I_Lp7_Busy
        {
            get { return (bool)GetStatusValue("I_Lp7_Busy"); }
            protected set { SetStatusValue("I_Lp7_Busy", value); }
        }

        public bool I_Lp7_Compt
        {
            get { return (bool)GetStatusValue("I_Lp7_Compt"); }
            protected set { SetStatusValue("I_Lp7_Compt", value); }
        }

        public bool I_Lp7_Cont
        {
            get { return (bool)GetStatusValue("I_Lp7_Cont"); }
            protected set { SetStatusValue("I_Lp7_Cont", value); }
        }

        public bool I_Lp8_Valid
        {
            get { return (bool)GetStatusValue("I_Lp8_Valid"); }
            protected set { SetStatusValue("I_Lp8_Valid", value); }
        }

        public bool I_Lp8_Cs_0
        {
            get { return (bool)GetStatusValue("I_Lp8_Cs_0"); }
            protected set { SetStatusValue("I_Lp8_Cs_0", value); }
        }

        public bool I_Lp8_Cs_1
        {
            get { return (bool)GetStatusValue("I_Lp8_Cs_1"); }
            protected set { SetStatusValue("I_Lp8_Cs_1", value); }
        }

        public bool I_Lp8_Tr_Req
        {
            get { return (bool)GetStatusValue("I_Lp8_Tr_Req"); }
            protected set { SetStatusValue("I_Lp8_Tr_Req", value); }
        }

        public bool I_Lp8_Busy
        {
            get { return (bool)GetStatusValue("I_Lp8_Busy"); }
            protected set { SetStatusValue("I_Lp8_Busy", value); }
        }

        public bool I_Lp8_Compt
        {
            get { return (bool)GetStatusValue("I_Lp8_Compt"); }
            protected set { SetStatusValue("I_Lp8_Compt", value); }
        }

        public bool I_Lp8_Cont
        {
            get { return (bool)GetStatusValue("I_Lp8_Cont"); }
            protected set { SetStatusValue("I_Lp8_Cont", value); }
        }

        public bool O_Lp1_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp1_L_Req"); }
            protected set { SetStatusValue("O_Lp1_L_Req", value); }
        }

        public bool O_Lp1_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp1_U_Req"); }
            protected set { SetStatusValue("O_Lp1_U_Req", value); }
        }

        public bool O_Lp1_Ready
        {
            get { return (bool)GetStatusValue("O_Lp1_Ready"); }
            protected set { SetStatusValue("O_Lp1_Ready", value); }
        }

        public bool O_Lp1_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp1_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp1_Ho_Avbl", value); }
        }

        public bool O_Lp1_Es
        {
            get { return (bool)GetStatusValue("O_Lp1_Es"); }
            protected set { SetStatusValue("O_Lp1_Es", value); }
        }

        public bool O_Lp2_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp2_L_Req"); }
            protected set { SetStatusValue("O_Lp2_L_Req", value); }
        }

        public bool O_Lp2_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp2_U_Req"); }
            protected set { SetStatusValue("O_Lp2_U_Req", value); }
        }

        public bool O_Lp2_Ready
        {
            get { return (bool)GetStatusValue("O_Lp2_Ready"); }
            protected set { SetStatusValue("O_Lp2_Ready", value); }
        }

        public bool O_Lp2_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp2_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp2_Ho_Avbl", value); }
        }

        public bool O_Lp2_Es
        {
            get { return (bool)GetStatusValue("O_Lp2_Es"); }
            protected set { SetStatusValue("O_Lp2_Es", value); }
        }

        public bool O_Lp3_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp3_L_Req"); }
            protected set { SetStatusValue("O_Lp3_L_Req", value); }
        }

        public bool O_Lp3_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp3_U_Req"); }
            protected set { SetStatusValue("O_Lp3_U_Req", value); }
        }

        public bool O_Lp3_Ready
        {
            get { return (bool)GetStatusValue("O_Lp3_Ready"); }
            protected set { SetStatusValue("O_Lp3_Ready", value); }
        }

        public bool O_Lp3_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp3_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp3_Ho_Avbl", value); }
        }

        public bool O_Lp3_Es
        {
            get { return (bool)GetStatusValue("O_Lp3_Es"); }
            protected set { SetStatusValue("O_Lp3_Es", value); }
        }

        public bool O_Lp4_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp4_L_Req"); }
            protected set { SetStatusValue("O_Lp4_L_Req", value); }
        }

        public bool O_Lp4_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp4_U_Req"); }
            protected set { SetStatusValue("O_Lp4_U_Req", value); }
        }

        public bool O_Lp4_Ready
        {
            get { return (bool)GetStatusValue("O_Lp4_Ready"); }
            protected set { SetStatusValue("O_Lp4_Ready", value); }
        }

        public bool O_Lp4_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp4_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp4_Ho_Avbl", value); }
        }

        public bool O_Lp4_Es
        {
            get { return (bool)GetStatusValue("O_Lp4_Es"); }
            protected set { SetStatusValue("O_Lp4_Es", value); }
        }

        public bool O_Lp5_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp5_L_Req"); }
            protected set { SetStatusValue("O_Lp5_L_Req", value); }
        }

        public bool O_Lp5_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp5_U_Req"); }
            protected set { SetStatusValue("O_Lp5_U_Req", value); }
        }

        public bool O_Lp5_Ready
        {
            get { return (bool)GetStatusValue("O_Lp5_Ready"); }
            protected set { SetStatusValue("O_Lp5_Ready", value); }
        }

        public bool O_Lp5_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp5_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp5_Ho_Avbl", value); }
        }

        public bool O_Lp5_Es
        {
            get { return (bool)GetStatusValue("O_Lp5_Es"); }
            protected set { SetStatusValue("O_Lp5_Es", value); }
        }

        public bool O_Lp6_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp6_L_Req"); }
            protected set { SetStatusValue("O_Lp6_L_Req", value); }
        }

        public bool O_Lp6_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp6_U_Req"); }
            protected set { SetStatusValue("O_Lp6_U_Req", value); }
        }

        public bool O_Lp6_Ready
        {
            get { return (bool)GetStatusValue("O_Lp6_Ready"); }
            protected set { SetStatusValue("O_Lp6_Ready", value); }
        }

        public bool O_Lp6_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp6_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp6_Ho_Avbl", value); }
        }

        public bool O_Lp6_Es
        {
            get { return (bool)GetStatusValue("O_Lp6_Es"); }
            protected set { SetStatusValue("O_Lp6_Es", value); }
        }

        public bool O_Lp7_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp7_L_Req"); }
            protected set { SetStatusValue("O_Lp7_L_Req", value); }
        }

        public bool O_Lp7_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp7_U_Req"); }
            protected set { SetStatusValue("O_Lp7_U_Req", value); }
        }

        public bool O_Lp7_Ready
        {
            get { return (bool)GetStatusValue("O_Lp7_Ready"); }
            protected set { SetStatusValue("O_Lp7_Ready", value); }
        }

        public bool O_Lp7_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp7_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp7_Ho_Avbl", value); }
        }

        public bool O_Lp7_Es
        {
            get { return (bool)GetStatusValue("O_Lp7_Es"); }
            protected set { SetStatusValue("O_Lp7_Es", value); }
        }

        public bool O_Lp8_L_Req
        {
            get { return (bool)GetStatusValue("O_Lp8_L_Req"); }
            protected set { SetStatusValue("O_Lp8_L_Req", value); }
        }

        public bool O_Lp8_U_Req
        {
            get { return (bool)GetStatusValue("O_Lp8_U_Req"); }
            protected set { SetStatusValue("O_Lp8_U_Req", value); }
        }

        public bool O_Lp8_Ready
        {
            get { return (bool)GetStatusValue("O_Lp8_Ready"); }
            protected set { SetStatusValue("O_Lp8_Ready", value); }
        }

        public bool O_Lp8_Ho_Avbl
        {
            get { return (bool)GetStatusValue("O_Lp8_Ho_Avbl"); }
            protected set { SetStatusValue("O_Lp8_Ho_Avbl", value); }
        }

        public bool O_Lp8_Es
        {
            get { return (bool)GetStatusValue("O_Lp8_Es"); }
            protected set { SetStatusValue("O_Lp8_Es", value); }
        }

        public bool PlacementSensorALoadPort1
        {
            get { return (bool)GetStatusValue("PlacementSensorALoadPort1"); }
            protected set { SetStatusValue("PlacementSensorALoadPort1", value); }
        }

        public bool PlacementSensorBLoadPort1
        {
            get { return (bool)GetStatusValue("PlacementSensorBLoadPort1"); }
            protected set { SetStatusValue("PlacementSensorBLoadPort1", value); }
        }

        public bool PlacementSensorCLoadPort1
        {
            get { return (bool)GetStatusValue("PlacementSensorCLoadPort1"); }
            protected set { SetStatusValue("PlacementSensorCLoadPort1", value); }
        }

        public bool PlacementSensorDLoadPort1
        {
            get { return (bool)GetStatusValue("PlacementSensorDLoadPort1"); }
            protected set { SetStatusValue("PlacementSensorDLoadPort1", value); }
        }

        public bool WaferProtrudeSensor1LoadPort1
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor1LoadPort1"); }
            protected set { SetStatusValue("WaferProtrudeSensor1LoadPort1", value); }
        }

        public bool WaferProtrudeSensor2LoadPort1
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor2LoadPort1"); }
            protected set { SetStatusValue("WaferProtrudeSensor2LoadPort1", value); }
        }

        public bool WaferProtrudeSensor3LoadPort1
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor3LoadPort1"); }
            protected set { SetStatusValue("WaferProtrudeSensor3LoadPort1", value); }
        }

        public bool PlacementSensorALoadPort2
        {
            get { return (bool)GetStatusValue("PlacementSensorALoadPort2"); }
            protected set { SetStatusValue("PlacementSensorALoadPort2", value); }
        }

        public bool PlacementSensorBLoadPort2
        {
            get { return (bool)GetStatusValue("PlacementSensorBLoadPort2"); }
            protected set { SetStatusValue("PlacementSensorBLoadPort2", value); }
        }

        public bool PlacementSensorCLoadPort2
        {
            get { return (bool)GetStatusValue("PlacementSensorCLoadPort2"); }
            protected set { SetStatusValue("PlacementSensorCLoadPort2", value); }
        }

        public bool PlacementSensorDLoadPort2
        {
            get { return (bool)GetStatusValue("PlacementSensorDLoadPort2"); }
            protected set { SetStatusValue("PlacementSensorDLoadPort2", value); }
        }

        public bool WaferProtrudeSensor1LoadPort2
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor1LoadPort2"); }
            protected set { SetStatusValue("WaferProtrudeSensor1LoadPort2", value); }
        }

        public bool WaferProtrudeSensor2LoadPort2
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor2LoadPort2"); }
            protected set { SetStatusValue("WaferProtrudeSensor2LoadPort2", value); }
        }

        public bool WaferProtrudeSensor3LoadPort2
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor3LoadPort2"); }
            protected set { SetStatusValue("WaferProtrudeSensor3LoadPort2", value); }
        }

        public bool PlacementSensorALoadPort3
        {
            get { return (bool)GetStatusValue("PlacementSensorALoadPort3"); }
            protected set { SetStatusValue("PlacementSensorALoadPort3", value); }
        }

        public bool PlacementSensorBLoadPort3
        {
            get { return (bool)GetStatusValue("PlacementSensorBLoadPort3"); }
            protected set { SetStatusValue("PlacementSensorBLoadPort3", value); }
        }

        public bool PlacementSensorCLoadPort3
        {
            get { return (bool)GetStatusValue("PlacementSensorCLoadPort3"); }
            protected set { SetStatusValue("PlacementSensorCLoadPort3", value); }
        }

        public bool PlacementSensorDLoadPort3
        {
            get { return (bool)GetStatusValue("PlacementSensorDLoadPort3"); }
            protected set { SetStatusValue("PlacementSensorDLoadPort3", value); }
        }

        public bool WaferProtrudeSensor1LoadPort3
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor1LoadPort3"); }
            protected set { SetStatusValue("WaferProtrudeSensor1LoadPort3", value); }
        }

        public bool WaferProtrudeSensor2LoadPort3
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor2LoadPort3"); }
            protected set { SetStatusValue("WaferProtrudeSensor2LoadPort3", value); }
        }

        public bool WaferProtrudeSensor3LoadPort3
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor3LoadPort3"); }
            protected set { SetStatusValue("WaferProtrudeSensor3LoadPort3", value); }
        }

        public bool PlacementSensorALoadPort4
        {
            get { return (bool)GetStatusValue("PlacementSensorALoadPort4"); }
            protected set { SetStatusValue("PlacementSensorALoadPort4", value); }
        }

        public bool PlacementSensorBLoadPort4
        {
            get { return (bool)GetStatusValue("PlacementSensorBLoadPort4"); }
            protected set { SetStatusValue("PlacementSensorBLoadPort4", value); }
        }

        public bool PlacementSensorCLoadPort4
        {
            get { return (bool)GetStatusValue("PlacementSensorCLoadPort4"); }
            protected set { SetStatusValue("PlacementSensorCLoadPort4", value); }
        }

        public bool PlacementSensorDLoadPort4
        {
            get { return (bool)GetStatusValue("PlacementSensorDLoadPort4"); }
            protected set { SetStatusValue("PlacementSensorDLoadPort4", value); }
        }

        public bool WaferProtrudeSensor1LoadPort4
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor1LoadPort4"); }
            protected set { SetStatusValue("WaferProtrudeSensor1LoadPort4", value); }
        }

        public bool WaferProtrudeSensor2LoadPort4
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor2LoadPort4"); }
            protected set { SetStatusValue("WaferProtrudeSensor2LoadPort4", value); }
        }

        public bool WaferProtrudeSensor3LoadPort4
        {
            get { return (bool)GetStatusValue("WaferProtrudeSensor3LoadPort4"); }
            protected set { SetStatusValue("WaferProtrudeSensor3LoadPort4", value); }
        }

        public bool Alarm
        {
            get { return (bool)GetStatusValue("Alarm"); }
            protected set { SetStatusValue("Alarm", value); }
        }
    }
}
