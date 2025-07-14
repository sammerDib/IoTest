using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x
{
    public partial class MapperRR75x : UnitySC.EFEM.Rorze.Devices.Robot.RR75x.RR75x, IMapperRR75x
    {
        public static new readonly DeviceType Type;

        static MapperRR75x()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.MapperRR75x.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.MapperRR75x");
            }
        }

        public MapperRR75x(string name)
            : this(name, Type)
        {
        }

        protected MapperRR75x(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "MapLocation":
                    {
                        Agileo.EquipmentModeling.IMaterialLocationContainer location = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("location");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalMapLocation(location);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateMapLocation(location, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command MapLocation interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "MapTransferLocation":
                    {
                        Agileo.SemiDefinitions.TransferLocation location = (Agileo.SemiDefinitions.TransferLocation)execution.Context.GetArgument("location");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalMapTransferLocation(location);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateMapTransferLocation(location, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command MapTransferLocation interrupted.");
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

        public void MapLocation(Agileo.EquipmentModeling.IMaterialLocationContainer location)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            CommandExecution execution = new CommandExecution(this, "MapLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task MapLocationAsync(Agileo.EquipmentModeling.IMaterialLocationContainer location)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            CommandExecution execution = new CommandExecution(this, "MapLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void MapTransferLocation(Agileo.SemiDefinitions.TransferLocation location)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            CommandExecution execution = new CommandExecution(this, "MapTransferLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task MapTransferLocationAsync(Agileo.SemiDefinitions.TransferLocation location)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            CommandExecution execution = new CommandExecution(this, "MapTransferLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool RobotPositionReverted
        {
            get { return (bool)GetStatusValue("RobotPositionReverted"); }
            protected set { SetStatusValue("RobotPositionReverted", value); }
        }
    }
}
