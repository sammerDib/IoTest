using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000
{
    public partial class EK9000 : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IEK9000
    {
        public static new readonly DeviceType Type;

        static EK9000()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.EK9000.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.EK9000");
            }
        }

        public EK9000(string name)
            : this(name, Type)
        {
        }

        protected EK9000(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "SetDigitalOutput":
                    {
                        UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.DigitalOutputs output = (UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.DigitalOutputs)execution.Context.GetArgument("output");

                        System.Boolean value = (System.Boolean)execution.Context.GetArgument("value");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetDigitalOutput(output, value);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetDigitalOutput(output, value, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetDigitalOutput interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetAnalogOutput":
                    {
                        UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.AnalogOutputs output = (UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.AnalogOutputs)execution.Context.GetArgument("output");

                        System.Double value = (System.Double)execution.Context.GetArgument("value");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetAnalogOutput(output, value);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetAnalogOutput(output, value, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetAnalogOutput interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetFfuSpeed":
                    {
                        UnitsNet.RotationalSpeed setpoint = (UnitsNet.RotationalSpeed)execution.Context.GetArgument("setPoint");

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

                case "SetDateAndTime":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetDateAndTime();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetDateAndTime(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetDateAndTime interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetLightColor":
                    {
                        Agileo.SemiDefinitions.LightColors color = (Agileo.SemiDefinitions.LightColors)execution.Context.GetArgument("color");

                        Agileo.GUI.Services.LightTower.LightState mode = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("mode");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetLightColor(color, mode);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetLightColor(color, mode, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetLightColor interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetBuzzerState":
                    {
                        Agileo.SemiDefinitions.BuzzerState state = (Agileo.SemiDefinitions.BuzzerState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetBuzzerState(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetBuzzerState(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetBuzzerState interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetReaderPosition":
                    {
                        Agileo.SemiDefinitions.SampleDimension dimension = (Agileo.SemiDefinitions.SampleDimension)execution.Context.GetArgument("dimension");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetReaderPosition(dimension);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetReaderPosition(dimension, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetReaderPosition interrupted.");
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

        public void SetDigitalOutput(UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.DigitalOutputs output,bool value)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("output", output));
            arguments.Add(new Argument("value", value));
            CommandExecution execution = new CommandExecution(this, "SetDigitalOutput", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetDigitalOutputAsync(UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.DigitalOutputs output,bool value)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("output", output));
            arguments.Add(new Argument("value", value));
            CommandExecution execution = new CommandExecution(this, "SetDigitalOutput", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetAnalogOutput(UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.AnalogOutputs output,double value)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("output", output));
            arguments.Add(new Argument("value", value));
            CommandExecution execution = new CommandExecution(this, "SetAnalogOutput", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetAnalogOutputAsync(UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums.AnalogOutputs output,double value)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("output", output));
            arguments.Add(new Argument("value", value));
            CommandExecution execution = new CommandExecution(this, "SetAnalogOutput", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
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

        public void SetDateAndTime()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetDateAndTimeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetLightColor(Agileo.SemiDefinitions.LightColors color,Agileo.GUI.Services.LightTower.LightState mode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("color", color));
            arguments.Add(new Argument("mode", mode));
            CommandExecution execution = new CommandExecution(this, "SetLightColor", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetLightColorAsync(Agileo.SemiDefinitions.LightColors color,Agileo.GUI.Services.LightTower.LightState mode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("color", color));
            arguments.Add(new Argument("mode", mode));
            CommandExecution execution = new CommandExecution(this, "SetLightColor", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetBuzzerState(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "SetBuzzerState", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetBuzzerStateAsync(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "SetBuzzerState", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetReaderPosition(Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "SetReaderPosition", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetReaderPositionAsync(Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "SetReaderPosition", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool I_EMO_Status
        {
            get { return (bool)GetStatusValue("I_EMO_Status"); }
            protected set { SetStatusValue("I_EMO_Status", value); }
        }

        public bool I_FFU_Alarm
        {
            get { return (bool)GetStatusValue("I_FFU_Alarm"); }
            protected set { SetStatusValue("I_FFU_Alarm", value); }
        }

        public bool I_VacuumPressureSensor
        {
            get { return (bool)GetStatusValue("I_VacuumPressureSensor"); }
            protected set { SetStatusValue("I_VacuumPressureSensor", value); }
        }

        public bool I_CDA_PressureSensor
        {
            get { return (bool)GetStatusValue("I_CDA_PressureSensor"); }
            protected set { SetStatusValue("I_CDA_PressureSensor", value); }
        }

        public bool I_ServiceLightLed
        {
            get { return (bool)GetStatusValue("I_ServiceLightLed"); }
            protected set { SetStatusValue("I_ServiceLightLed", value); }
        }

        public bool I_AirFlowPressureSensorIonizer
        {
            get { return (bool)GetStatusValue("I_AirFlowPressureSensorIonizer"); }
            protected set { SetStatusValue("I_AirFlowPressureSensorIonizer", value); }
        }

        public bool I_Ionizer1Status
        {
            get { return (bool)GetStatusValue("I_Ionizer1Status"); }
            protected set { SetStatusValue("I_Ionizer1Status", value); }
        }

        public bool I_RV201Interlock
        {
            get { return (bool)GetStatusValue("I_RV201Interlock"); }
            protected set { SetStatusValue("I_RV201Interlock", value); }
        }

        public bool I_MaintenanceSwitch
        {
            get { return (bool)GetStatusValue("I_MaintenanceSwitch"); }
            protected set { SetStatusValue("I_MaintenanceSwitch", value); }
        }

        public bool I_RobotDriverPower
        {
            get { return (bool)GetStatusValue("I_RobotDriverPower"); }
            protected set { SetStatusValue("I_RobotDriverPower", value); }
        }

        public bool I_EFEM_DoorStatus
        {
            get { return (bool)GetStatusValue("I_EFEM_DoorStatus"); }
            protected set { SetStatusValue("I_EFEM_DoorStatus", value); }
        }

        public bool I_TPMode
        {
            get { return (bool)GetStatusValue("I_TPMode"); }
            protected set { SetStatusValue("I_TPMode", value); }
        }

        public bool I_OCRTableAlarm
        {
            get { return (bool)GetStatusValue("I_OCRTableAlarm"); }
            protected set { SetStatusValue("I_OCRTableAlarm", value); }
        }

        public bool I_OCRTablePositionReach
        {
            get { return (bool)GetStatusValue("I_OCRTablePositionReach"); }
            protected set { SetStatusValue("I_OCRTablePositionReach", value); }
        }

        public bool I_OCRWaferReaderLimitSensor1
        {
            get { return (bool)GetStatusValue("I_OCRWaferReaderLimitSensor1"); }
            protected set { SetStatusValue("I_OCRWaferReaderLimitSensor1", value); }
        }

        public bool I_OCRWaferReaderLimitSensor2
        {
            get { return (bool)GetStatusValue("I_OCRWaferReaderLimitSensor2"); }
            protected set { SetStatusValue("I_OCRWaferReaderLimitSensor2", value); }
        }

        public UnitsNet.Pressure I_DifferentialAirPressureSensor
        {
            get { return (UnitsNet.Pressure)GetStatusValue("I_DifferentialAirPressureSensor"); }
            protected set
            {
                UnitsNet.Units.PressureUnit? unit = DeviceType.AllStatuses().Get("I_DifferentialAirPressureSensor").Unit as UnitsNet.Units.PressureUnit?;
                UnitsNet.Pressure newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("I_DifferentialAirPressureSensor", newValue);
            }
        }

        public bool I_PM1_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM1_DoorOpened"); }
            protected set { SetStatusValue("I_PM1_DoorOpened", value); }
        }

        public bool I_PM1_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM1_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM1_ReadyToLoadUnload", value); }
        }

        public bool I_PM2_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM2_DoorOpened"); }
            protected set { SetStatusValue("I_PM2_DoorOpened", value); }
        }

        public bool I_PM2_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM2_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM2_ReadyToLoadUnload", value); }
        }

        public bool I_PM3_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM3_DoorOpened"); }
            protected set { SetStatusValue("I_PM3_DoorOpened", value); }
        }

        public bool I_PM3_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM3_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM3_ReadyToLoadUnload", value); }
        }

        public bool O_SignalTower_LightningRed
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningRed"); }
            protected set { SetStatusValue("O_SignalTower_LightningRed", value); }
        }

        public bool O_SignalTower_LightningYellow
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningYellow"); }
            protected set { SetStatusValue("O_SignalTower_LightningYellow", value); }
        }

        public bool O_SignalTower_LightningGreen
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningGreen"); }
            protected set { SetStatusValue("O_SignalTower_LightningGreen", value); }
        }

        public bool O_SignalTower_LightningBlue
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningBlue"); }
            protected set { SetStatusValue("O_SignalTower_LightningBlue", value); }
        }

        public bool O_SignalTower_BlinkingRed
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingRed"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingRed", value); }
        }

        public bool O_SignalTower_BlinkingYellow
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingYellow"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingYellow", value); }
        }

        public bool O_SignalTower_BlinkingGreen
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingGreen"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingGreen", value); }
        }

        public bool O_SignalTower_BlinkingBlue
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingBlue"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingBlue", value); }
        }

        public bool O_SignalTower_Buzzer1
        {
            get { return (bool)GetStatusValue("O_SignalTower_Buzzer1"); }
            protected set { SetStatusValue("O_SignalTower_Buzzer1", value); }
        }

        public bool O_SignalTower_Buzzer2
        {
            get { return (bool)GetStatusValue("O_SignalTower_Buzzer2"); }
            protected set { SetStatusValue("O_SignalTower_Buzzer2", value); }
        }

        public bool O_OCRWaferReaderValve1
        {
            get { return (bool)GetStatusValue("O_OCRWaferReaderValve1"); }
            protected set { SetStatusValue("O_OCRWaferReaderValve1", value); }
        }

        public bool O_OCRWaferReaderValve2
        {
            get { return (bool)GetStatusValue("O_OCRWaferReaderValve2"); }
            protected set { SetStatusValue("O_OCRWaferReaderValve2", value); }
        }

        public bool O_OCRTableDrive
        {
            get { return (bool)GetStatusValue("O_OCRTableDrive"); }
            protected set { SetStatusValue("O_OCRTableDrive", value); }
        }

        public bool O_OCRTableReset
        {
            get { return (bool)GetStatusValue("O_OCRTableReset"); }
            protected set { SetStatusValue("O_OCRTableReset", value); }
        }

        public bool O_OCRTableInitialization
        {
            get { return (bool)GetStatusValue("O_OCRTableInitialization"); }
            protected set { SetStatusValue("O_OCRTableInitialization", value); }
        }

        public UnitsNet.RotationalSpeed O_FFU_Speed
        {
            get { return (UnitsNet.RotationalSpeed)GetStatusValue("O_FFU_Speed"); }
            protected set
            {
                UnitsNet.Units.RotationalSpeedUnit? unit = DeviceType.AllStatuses().Get("O_FFU_Speed").Unit as UnitsNet.Units.RotationalSpeedUnit?;
                UnitsNet.RotationalSpeed newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("O_FFU_Speed", newValue);
            }
        }

        public bool O_RobotArmNotExtended_PM1
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM1"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM1", value); }
        }

        public bool O_RobotArmNotExtended_PM2
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM2"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM2", value); }
        }

        public bool O_RobotArmNotExtended_PM3
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM3"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM3", value); }
        }

        public bool Alarm
        {
            get { return (bool)GetStatusValue("Alarm"); }
            protected set { SetStatusValue("Alarm", value); }
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
    }
}
