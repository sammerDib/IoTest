using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.ReaderPositioner
{
    public abstract partial class ReaderPositioner : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IReaderPositioner
    {
        public static new readonly DeviceType Type;

        static ReaderPositioner()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.ReaderPositioner.ReaderPositioner.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.ReaderPositioner.ReaderPositioner");
            }
        }

        public ReaderPositioner(string name)
            : this(name, Type)
        {
        }

        protected ReaderPositioner(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "SetPosition":
                    {
                        Agileo.SemiDefinitions.SampleDimension dimension = (Agileo.SemiDefinitions.SampleDimension)execution.Context.GetArgument("dimension");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetPosition(dimension);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetPosition(dimension, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetPosition interrupted.");
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

        public void SetPosition(Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "SetPosition", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetPositionAsync(Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "SetPosition", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public Agileo.SemiDefinitions.SampleDimension CurrentPosition
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("CurrentPosition"); }
            protected set { SetStatusValue("CurrentPosition", value); }
        }
    }
}
