using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule
{
    public abstract partial class ProcessModule : UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.CommunicatingDevice, IProcessModule
    {
        public static new readonly DeviceType Type;

        static ProcessModule()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule.ProcessModule.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule.ProcessModule");
            }
        }

        public ProcessModule(string name)
            : this(name, Type)
        {
        }

        protected ProcessModule(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "PrepareForTransfer":
                    {
                        System.Byte slot = (System.Byte)execution.Context.GetArgument("slot");

                        UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.TransferType transfertype = (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.TransferType)execution.Context.GetArgument("transferType");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPrepareForTransfer(slot, transfertype);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePrepareForTransfer(slot, transfertype, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PrepareForTransfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "PrepareForProcess":
                    {
                        System.Byte slot = (System.Byte)execution.Context.GetArgument("slot");

                        System.Boolean automaticstart = (System.Boolean)execution.Context.GetArgument("automaticStart");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPrepareForProcess(slot, automaticstart);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePrepareForProcess(slot, automaticstart, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PrepareForProcess interrupted.");
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

        public void PrepareForTransfer(byte slot,UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.TransferType transferType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            arguments.Add(new Argument("transferType", transferType));
            CommandExecution execution = new CommandExecution(this, "PrepareForTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PrepareForTransferAsync(byte slot,UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.TransferType transferType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            arguments.Add(new Argument("transferType", transferType));
            CommandExecution execution = new CommandExecution(this, "PrepareForTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void PrepareForProcess(byte slot,bool automaticStart)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            arguments.Add(new Argument("automaticStart", automaticStart));
            CommandExecution execution = new CommandExecution(this, "PrepareForProcess", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PrepareForProcessAsync(byte slot,bool automaticStart)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            arguments.Add(new Argument("automaticStart", automaticStart));
            CommandExecution execution = new CommandExecution(this, "PrepareForProcess", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool IsReadyForTransfer
        {
            get { return (bool)GetStatusValue("IsReadyForTransfer"); }
            protected set { SetStatusValue("IsReadyForTransfer", value); }
        }

        public bool IsReadyToAcceptTransfer
        {
            get { return (bool)GetStatusValue("IsReadyToAcceptTransfer"); }
            protected set { SetStatusValue("IsReadyToAcceptTransfer", value); }
        }
    }
}
