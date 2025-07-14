using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice
{
    public abstract partial class GenericDevice : Device, IGenericDevice
    {
        public static readonly DeviceType Type;

        static GenericDevice()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.GenericDevice.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.GenericDevice");
            }
        }

        public GenericDevice(string name)
            : this(name, Type)
        {
        }

        protected GenericDevice(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "Initialize":
                    {
                        System.Boolean mustforceinit = (System.Boolean)execution.Context.GetArgument("mustForceInit");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalInitialize(mustforceinit);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateInitialize(mustforceinit, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Initialize interrupted.");
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

        public void Initialize(bool mustForceInit)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("mustForceInit", mustForceInit));
            CommandExecution execution = new CommandExecution(this, "Initialize", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task InitializeAsync(bool mustForceInit)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("mustForceInit", mustForceInit));
            CommandExecution execution = new CommandExecution(this, "Initialize", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.OperatingModes State
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.OperatingModes)GetStatusValue("State"); }
            protected set { SetStatusValue("State", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.OperatingModes PreviousState
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.OperatingModes)GetStatusValue("PreviousState"); }
            protected set { SetStatusValue("PreviousState", value); }
        }
    }
}
