using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx
{
    public abstract partial class GenericRC5xx : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IGenericRC5xx
    {
        public static new readonly DeviceType Type;

        static GenericRC5xx()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.GenericRC5xx.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.GenericRC5xx");
            }
        }

        public GenericRC5xx(string name)
            : this(name, Type)
        {
        }

        protected GenericRC5xx(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "SetOutputSignal":
                    {
                        UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status.SignalData signaldata = (UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status.SignalData)execution.Context.GetArgument("signalData");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetOutputSignal(signaldata);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetOutputSignal(signaldata, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetOutputSignal interrupted.");
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

        public void SetOutputSignal(UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status.SignalData signalData)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("signalData", signalData));
            CommandExecution execution = new CommandExecution(this, "SetOutputSignal", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetOutputSignalAsync(UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status.SignalData signalData)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("signalData", signalData));
            CommandExecution execution = new CommandExecution(this, "SetOutputSignal", arguments);
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

        public UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums.OperationMode OperationMode
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums.OperationMode)GetStatusValue("OperationMode"); }
            protected set { SetStatusValue("OperationMode", value); }
        }

        public UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums.CommandProcessing CommandProcessing
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums.CommandProcessing)GetStatusValue("CommandProcessing"); }
            protected set { SetStatusValue("CommandProcessing", value); }
        }

        public string IoModuleInError
        {
            get { return (string)GetStatusValue("IoModuleInError"); }
            protected set { SetStatusValue("IoModuleInError", value); }
        }

        public string ErrorCode
        {
            get { return (string)GetStatusValue("ErrorCode"); }
            protected set { SetStatusValue("ErrorCode", value); }
        }
    }
}
