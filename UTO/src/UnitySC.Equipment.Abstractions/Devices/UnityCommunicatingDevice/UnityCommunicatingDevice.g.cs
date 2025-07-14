using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice
{
    public abstract partial class UnityCommunicatingDevice : UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.GenericDevice, IUnityCommunicatingDevice
    {
        public static new readonly DeviceType Type;

        static UnityCommunicatingDevice()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice");
            }
        }

        public UnityCommunicatingDevice(string name)
            : this(name, Type)
        {
        }

        protected UnityCommunicatingDevice(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "StartCommunication":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStartCommunication();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStartCommunication(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command StartCommunication interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "StopCommunication":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStopCommunication();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStopCommunication(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command StopCommunication interrupted.");
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

        public void StartCommunication()
        {
            CommandExecution execution = new CommandExecution(this, "StartCommunication");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StartCommunicationAsync()
        {
            CommandExecution execution = new CommandExecution(this, "StartCommunication");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void StopCommunication()
        {
            CommandExecution execution = new CommandExecution(this, "StopCommunication");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StopCommunicationAsync()
        {
            CommandExecution execution = new CommandExecution(this, "StopCommunication");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool IsCommunicating
        {
            get { return (bool)GetStatusValue("IsCommunicating"); }
            protected set { SetStatusValue("IsCommunicating", value); }
        }

        public bool IsCommunicationStarted
        {
            get { return (bool)GetStatusValue("IsCommunicationStarted"); }
            protected set { SetStatusValue("IsCommunicationStarted", value); }
        }
    }
}
