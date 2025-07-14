using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice
{
    public partial class CommunicatingDevice : UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.GenericDevice, ICommunicatingDevice
    {
        public static new readonly DeviceType Type;

        static CommunicatingDevice()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.CommunicatingDevice.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.CommunicatingDevice");
            }
        }

        public CommunicatingDevice(string name)
            : this(name, Type)
        {
        }

        protected CommunicatingDevice(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "Connect":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalConnect();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateConnect(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Connect interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Disconnect":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDisconnect();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDisconnect(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Disconnect interrupted.");
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

        public void Connect()
        {
            CommandExecution execution = new CommandExecution(this, "Connect");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ConnectAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Connect");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Disconnect()
        {
            CommandExecution execution = new CommandExecution(this, "Disconnect");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DisconnectAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Disconnect");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool IsConnected
        {
            get { return (bool)GetStatusValue("IsConnected"); }
            protected set { SetStatusValue("IsConnected", value); }
        }

        public bool IsSlaveDevice
        {
            get { return (bool)GetStatusValue("IsSlaveDevice"); }
            protected set { SetStatusValue("IsSlaveDevice", value); }
        }
    }
}
