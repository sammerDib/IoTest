using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu
{
    public abstract partial class Ffu : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IFfu
    {
        public static new readonly DeviceType Type;

        static Ffu()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.Ffu.Ffu.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.Ffu.Ffu");
            }
        }

        public Ffu(string name)
            : this(name, Type)
        {
        }

        protected Ffu(string name, DeviceType type)
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
                        System.Double setpoint = (System.Double)execution.Context.GetArgument("setPoint");

                        UnitySC.Equipment.Abstractions.Devices.Ffu.Enum.FfuSpeedUnit unit = (UnitySC.Equipment.Abstractions.Devices.Ffu.Enum.FfuSpeedUnit)execution.Context.GetArgument("unit");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetFfuSpeed(setpoint, unit);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetFfuSpeed(setpoint, unit, execution.Tempomat);
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

                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public void SetFfuSpeed(double setPoint,UnitySC.Equipment.Abstractions.Devices.Ffu.Enum.FfuSpeedUnit unit)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("setPoint", setPoint));
            arguments.Add(new Argument("unit", unit));
            CommandExecution execution = new CommandExecution(this, "SetFfuSpeed", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetFfuSpeedAsync(double setPoint,UnitySC.Equipment.Abstractions.Devices.Ffu.Enum.FfuSpeedUnit unit)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("setPoint", setPoint));
            arguments.Add(new Argument("unit", unit));
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

        public UnitsNet.Pressure DifferentialPressure
        {
            get { return (UnitsNet.Pressure)GetStatusValue("DifferentialPressure"); }
            protected set
            {
                UnitsNet.Units.PressureUnit? unit = DeviceType.AllStatuses().Get("DifferentialPressure").Unit as UnitsNet.Units.PressureUnit?;
                UnitsNet.Pressure newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("DifferentialPressure", newValue);
            }
        }

        public bool HasAlarm
        {
            get { return (bool)GetStatusValue("HasAlarm"); }
            protected set { SetStatusValue("HasAlarm", value); }
        }
    }
}
