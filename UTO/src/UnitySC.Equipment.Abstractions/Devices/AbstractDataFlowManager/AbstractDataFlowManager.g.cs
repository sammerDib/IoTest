using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager
{
    public abstract partial class AbstractDataFlowManager : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IAbstractDataFlowManager
    {
        public static new readonly DeviceType Type;

        static AbstractDataFlowManager()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.AbstractDataFlowManager.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.AbstractDataFlowManager");
            }
        }

        public AbstractDataFlowManager(string name)
            : this(name, Type)
        {
        }

        protected AbstractDataFlowManager(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "StartRecipe":
                    {
                        UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.MaterialRecipe materialrecipe = (UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.MaterialRecipe)execution.Context.GetArgument("materialRecipe");

                        System.String processjobid = (System.String)execution.Context.GetArgument("processJobId");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStartRecipe(materialrecipe, processjobid);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStartRecipe(materialrecipe, processjobid, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command StartRecipe interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "AbortRecipe":
                    {
                        System.String jobid = (System.String)execution.Context.GetArgument("jobId");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalAbortRecipe(jobid);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateAbortRecipe(jobid, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command AbortRecipe interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "StartJobOnMaterial":
                    {
                        UnitySC.DataAccess.Dto.DataflowRecipeInfo recipe = (UnitySC.DataAccess.Dto.DataflowRecipeInfo)execution.Context.GetArgument("recipe");

                        UnitySC.Equipment.Abstractions.Material.Wafer wafer = (UnitySC.Equipment.Abstractions.Material.Wafer)execution.Context.GetArgument("wafer");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStartJobOnMaterial(recipe, wafer);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStartJobOnMaterial(recipe, wafer, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command StartJobOnMaterial interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "GetAvailableRecipes":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetAvailableRecipes();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetAvailableRecipes(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetAvailableRecipes interrupted.");
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

        public void StartRecipe(UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.MaterialRecipe materialRecipe,string processJobId)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("materialRecipe", materialRecipe));
            arguments.Add(new Argument("processJobId", processJobId));
            CommandExecution execution = new CommandExecution(this, "StartRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StartRecipeAsync(UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.MaterialRecipe materialRecipe,string processJobId)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("materialRecipe", materialRecipe));
            arguments.Add(new Argument("processJobId", processJobId));
            CommandExecution execution = new CommandExecution(this, "StartRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void AbortRecipe(string jobId)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobId", jobId));
            CommandExecution execution = new CommandExecution(this, "AbortRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task AbortRecipeAsync(string jobId)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("jobId", jobId));
            CommandExecution execution = new CommandExecution(this, "AbortRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void StartJobOnMaterial(UnitySC.DataAccess.Dto.DataflowRecipeInfo recipe,UnitySC.Equipment.Abstractions.Material.Wafer wafer)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("recipe", recipe));
            arguments.Add(new Argument("wafer", wafer));
            CommandExecution execution = new CommandExecution(this, "StartJobOnMaterial", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StartJobOnMaterialAsync(UnitySC.DataAccess.Dto.DataflowRecipeInfo recipe,UnitySC.Equipment.Abstractions.Material.Wafer wafer)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("recipe", recipe));
            arguments.Add(new Argument("wafer", wafer));
            CommandExecution execution = new CommandExecution(this, "StartJobOnMaterial", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void GetAvailableRecipes()
        {
            CommandExecution execution = new CommandExecution(this, "GetAvailableRecipes");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetAvailableRecipesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GetAvailableRecipes");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public bool IsStopCancelAllJobsRequested
        {
            get { return (bool)GetStatusValue("IsStopCancelAllJobsRequested"); }
            protected set { SetStatusValue("IsStopCancelAllJobsRequested", value); }
        }

        public UnitySC.Shared.Data.Enum.TC_DataflowStatus DataflowState
        {
            get { return (UnitySC.Shared.Data.Enum.TC_DataflowStatus)GetStatusValue("DataflowState"); }
            protected set { SetStatusValue("DataflowState", value); }
        }
    }
}
