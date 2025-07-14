using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.SmifLoadPort
{
    public abstract partial class SmifLoadPort : UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort, ISmifLoadPort
    {
        public static new readonly DeviceType Type;

        static SmifLoadPort()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.SmifLoadPort.SmifLoadPort.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.SmifLoadPort.SmifLoadPort");
            }
        }

        public SmifLoadPort(string name)
            : this(name, Type)
        {
        }

        protected SmifLoadPort(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GoToSlot":
                    {
                        System.Byte slot = (System.Byte)execution.Context.GetArgument("slot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGoToSlot(slot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGoToSlot(slot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GoToSlot interrupted.");
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

        public void GoToSlot(byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "GoToSlot", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GoToSlotAsync(byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "GoToSlot", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public byte CurrentSlot
        {
            get { return (byte)GetStatusValue("CurrentSlot"); }
            protected set { SetStatusValue("CurrentSlot", value); }
        }
    }
}
