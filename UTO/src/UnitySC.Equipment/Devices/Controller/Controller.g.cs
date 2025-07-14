using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Devices.Controller
{
    public partial class Controller : UnitySC.Equipment.Abstractions.Devices.Controller.Controller, IController
    {
        public static new readonly DeviceType Type;

        static Controller()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Devices.Controller.Controller.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Devices.Controller.Controller");
            }
        }

        public Controller(string name)
            : this(name, Type)
        {
        }

        protected Controller(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "LoadProcessModule":
                    {
                        Agileo.EquipmentModeling.IMaterialLocationContainer loadport = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("loadPort");

                        System.Byte sourceslot = (System.Byte)execution.Context.GetArgument("sourceSlot");

                        Agileo.SemiDefinitions.RobotArm robotarm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("robotArm");

                        UnitsNet.Angle alignangle = (UnitsNet.Angle)execution.Context.GetArgument("alignAngle");
                        UnitsNet.Units.AngleUnit? unitalignAngle = execution.Context.Command.Parameters.FirstOrDefault(p => p.Name == "alignAngle").Unit as UnitsNet.Units.AngleUnit?;
                        if (unitalignAngle != null && unitalignAngle != alignangle.Unit)
                        {
                            alignangle = alignangle.ToUnit(unitalignAngle.Value);
                        }

                        UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType aligntype = (UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType)execution.Context.GetArgument("alignType");

                        UnitySC.Equipment.Abstractions.Enums.EffectorType effectortype = (UnitySC.Equipment.Abstractions.Enums.EffectorType)execution.Context.GetArgument("effectorType");

                        Agileo.EquipmentModeling.IMaterialLocationContainer processmodule = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("processModule");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalLoadProcessModule(loadport, sourceslot, robotarm, alignangle, aligntype, effectortype, processmodule);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateLoadProcessModule(loadport, sourceslot, robotarm, alignangle, aligntype, effectortype, processmodule, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command LoadProcessModule interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "UnloadProcessModule":
                    {
                        Agileo.EquipmentModeling.IMaterialLocationContainer processmodule = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("processModule");

                        Agileo.SemiDefinitions.RobotArm robotarm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("robotArm");

                        UnitySC.Equipment.Abstractions.Enums.EffectorType effectortype = (UnitySC.Equipment.Abstractions.Enums.EffectorType)execution.Context.GetArgument("effectorType");

                        Agileo.EquipmentModeling.IMaterialLocationContainer loadport = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("loadPort");

                        System.Byte destinationslot = (System.Byte)execution.Context.GetArgument("destinationSlot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalUnloadProcessModule(processmodule, robotarm, effectortype, loadport, destinationslot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateUnloadProcessModule(processmodule, robotarm, effectortype, loadport, destinationslot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command UnloadProcessModule interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Clean":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalClean();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateClean(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Clean interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "CreateJob":
                    {
                        UnitySC.Equipment.Devices.Controller.JobDefinition.Job job = (UnitySC.Equipment.Devices.Controller.JobDefinition.Job)execution.Context.GetArgument("job");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalCreateJob(job);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateCreateJob(job, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command CreateJob interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "StartJobExecution":
                    {
                        UnitySC.Equipment.Devices.Controller.JobDefinition.Job job = (UnitySC.Equipment.Devices.Controller.JobDefinition.Job)execution.Context.GetArgument("job");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStartJobExecution(job);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStartJobExecution(job, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command StartJobExecution interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "RequestManualMode":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRequestManualMode();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRequestManualMode(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command RequestManualMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "RequestEngineeringMode":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRequestEngineeringMode();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRequestEngineeringMode(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command RequestEngineeringMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Pause":
                    {
                        System.String jobname = (System.String)execution.Context.GetArgument("jobName");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPause(jobname);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePause(jobname, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Pause interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Resume":
                    {
                        System.String jobname = (System.String)execution.Context.GetArgument("jobName");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalResume(jobname);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateResume(jobname, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Resume interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Stop":
                    {
                        System.String jobname = (System.String)execution.Context.GetArgument("jobName");

                        UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum.StopConfig stopconfig = (UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum.StopConfig)execution.Context.GetArgument("stopConfig");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStop(jobname, stopconfig);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStop(jobname, stopconfig, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Stop interrupted.");
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

        public void LoadProcessModule(Agileo.EquipmentModeling.IMaterialLocationContainer loadPort,byte sourceSlot,Agileo.SemiDefinitions.RobotArm robotArm,UnitsNet.Angle alignAngle,UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType alignType,UnitySC.Equipment.Abstractions.Enums.EffectorType effectorType,Agileo.EquipmentModeling.IMaterialLocationContainer processModule)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("loadPort", loadPort));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            arguments.Add(new Argument("robotArm", robotArm));
            arguments.Add(new Argument("alignAngle", alignAngle));
            arguments.Add(new Argument("alignType", alignType));
            arguments.Add(new Argument("effectorType", effectorType));
            arguments.Add(new Argument("processModule", processModule));
            CommandExecution execution = new CommandExecution(this, "LoadProcessModule", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task LoadProcessModuleAsync(Agileo.EquipmentModeling.IMaterialLocationContainer loadPort,byte sourceSlot,Agileo.SemiDefinitions.RobotArm robotArm,UnitsNet.Angle alignAngle,UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType alignType,UnitySC.Equipment.Abstractions.Enums.EffectorType effectorType,Agileo.EquipmentModeling.IMaterialLocationContainer processModule)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("loadPort", loadPort));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            arguments.Add(new Argument("robotArm", robotArm));
            arguments.Add(new Argument("alignAngle", alignAngle));
            arguments.Add(new Argument("alignType", alignType));
            arguments.Add(new Argument("effectorType", effectorType));
            arguments.Add(new Argument("processModule", processModule));
            CommandExecution execution = new CommandExecution(this, "LoadProcessModule", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void UnloadProcessModule(Agileo.EquipmentModeling.IMaterialLocationContainer processModule,Agileo.SemiDefinitions.RobotArm robotArm,UnitySC.Equipment.Abstractions.Enums.EffectorType effectorType,Agileo.EquipmentModeling.IMaterialLocationContainer loadPort,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("processModule", processModule));
            arguments.Add(new Argument("robotArm", robotArm));
            arguments.Add(new Argument("effectorType", effectorType));
            arguments.Add(new Argument("loadPort", loadPort));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "UnloadProcessModule", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task UnloadProcessModuleAsync(Agileo.EquipmentModeling.IMaterialLocationContainer processModule,Agileo.SemiDefinitions.RobotArm robotArm,UnitySC.Equipment.Abstractions.Enums.EffectorType effectorType,Agileo.EquipmentModeling.IMaterialLocationContainer loadPort,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("processModule", processModule));
            arguments.Add(new Argument("robotArm", robotArm));
            arguments.Add(new Argument("effectorType", effectorType));
            arguments.Add(new Argument("loadPort", loadPort));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "UnloadProcessModule", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Clean()
        {
            CommandExecution execution = new CommandExecution(this, "Clean");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task CleanAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Clean");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void CreateJob(UnitySC.Equipment.Devices.Controller.JobDefinition.Job job)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("job", job));
            CommandExecution execution = new CommandExecution(this, "CreateJob", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task CreateJobAsync(UnitySC.Equipment.Devices.Controller.JobDefinition.Job job)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("job", job));
            CommandExecution execution = new CommandExecution(this, "CreateJob", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void StartJobExecution(UnitySC.Equipment.Devices.Controller.JobDefinition.Job job)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("job", job));
            CommandExecution execution = new CommandExecution(this, "StartJobExecution", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StartJobExecutionAsync(UnitySC.Equipment.Devices.Controller.JobDefinition.Job job)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("job", job));
            CommandExecution execution = new CommandExecution(this, "StartJobExecution", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void RequestManualMode()
        {
            CommandExecution execution = new CommandExecution(this, "RequestManualMode");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task RequestManualModeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "RequestManualMode");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void RequestEngineeringMode()
        {
            CommandExecution execution = new CommandExecution(this, "RequestEngineeringMode");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task RequestEngineeringModeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "RequestEngineeringMode");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Pause(string jobName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            CommandExecution execution = new CommandExecution(this, "Pause", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PauseAsync(string jobName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            CommandExecution execution = new CommandExecution(this, "Pause", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Resume(string jobName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            CommandExecution execution = new CommandExecution(this, "Resume", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ResumeAsync(string jobName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            CommandExecution execution = new CommandExecution(this, "Resume", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Stop(string jobName,UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum.StopConfig stopConfig)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            arguments.Add(new Argument("stopConfig", stopConfig));
            CommandExecution execution = new CommandExecution(this, "Stop", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StopAsync(string jobName,UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum.StopConfig stopConfig)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobName", jobName));
            arguments.Add(new Argument("stopConfig", stopConfig));
            CommandExecution execution = new CommandExecution(this, "Stop", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public string CurrentActivityStep
        {
            get { return (string)GetStatusValue("CurrentActivityStep"); }
            protected set { SetStatusValue("CurrentActivityStep", value); }
        }

        public double SubstrateThroughput
        {
            get { return (double)GetStatusValue("SubstrateThroughput"); }
            protected set { SetStatusValue("SubstrateThroughput", value); }
        }
    }
}
