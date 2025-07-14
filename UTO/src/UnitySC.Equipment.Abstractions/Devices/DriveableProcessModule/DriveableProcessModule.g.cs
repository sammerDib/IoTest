using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule
{
    public abstract partial class DriveableProcessModule : UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule, IDriveableProcessModule
    {
        public static new readonly DeviceType Type;

        static DriveableProcessModule()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule");
            }
        }

        public DriveableProcessModule(string name)
            : this(name, Type)
        {
        }

        protected DriveableProcessModule(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "PrepareTransfer":
                    {
                        UnitySC.Shared.TC.Shared.Data.TransferType transfertype = (UnitySC.Shared.TC.Shared.Data.TransferType)execution.Context.GetArgument("transferType");

                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        UnitySC.Equipment.Abstractions.Enums.MaterialType materialtype = (UnitySC.Equipment.Abstractions.Enums.MaterialType)execution.Context.GetArgument("materialType");

                        Agileo.SemiDefinitions.SampleDimension dimension = (Agileo.SemiDefinitions.SampleDimension)execution.Context.GetArgument("dimension");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPrepareTransfer(transfertype, arm, materialtype, dimension);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePrepareTransfer(transfertype, arm, materialtype, dimension, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PrepareTransfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "PostTransfer":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPostTransfer();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePostTransfer(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PostTransfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SelectRecipe":
                    {
                        UnitySC.Equipment.Abstractions.Material.Wafer wafer = (UnitySC.Equipment.Abstractions.Material.Wafer)execution.Context.GetArgument("wafer");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSelectRecipe(wafer);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSelectRecipe(wafer, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SelectRecipe interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "StartRecipe":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalStartRecipe();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateStartRecipe(execution.Tempomat);
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
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalAbortRecipe();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateAbortRecipe(execution.Tempomat);
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

                case "ResetSmokeDetectorAlarm":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalResetSmokeDetectorAlarm();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateResetSmokeDetectorAlarm(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command ResetSmokeDetectorAlarm interrupted.");
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

        public void PrepareTransfer(UnitySC.Shared.TC.Shared.Data.TransferType transferType,Agileo.SemiDefinitions.RobotArm arm,UnitySC.Equipment.Abstractions.Enums.MaterialType materialType,Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("transferType", transferType));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("materialType", materialType));
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "PrepareTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PrepareTransferAsync(UnitySC.Shared.TC.Shared.Data.TransferType transferType,Agileo.SemiDefinitions.RobotArm arm,UnitySC.Equipment.Abstractions.Enums.MaterialType materialType,Agileo.SemiDefinitions.SampleDimension dimension)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("transferType", transferType));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("materialType", materialType));
            arguments.Add(new Argument("dimension", dimension));
            CommandExecution execution = new CommandExecution(this, "PrepareTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void PostTransfer()
        {
            CommandExecution execution = new CommandExecution(this, "PostTransfer");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PostTransferAsync()
        {
            CommandExecution execution = new CommandExecution(this, "PostTransfer");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SelectRecipe(UnitySC.Equipment.Abstractions.Material.Wafer wafer)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("wafer", wafer));
            CommandExecution execution = new CommandExecution(this, "SelectRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SelectRecipeAsync(UnitySC.Equipment.Abstractions.Material.Wafer wafer)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("wafer", wafer));
            CommandExecution execution = new CommandExecution(this, "SelectRecipe", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void StartRecipe()
        {
            CommandExecution execution = new CommandExecution(this, "StartRecipe");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task StartRecipeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "StartRecipe");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void AbortRecipe()
        {
            CommandExecution execution = new CommandExecution(this, "AbortRecipe");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task AbortRecipeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "AbortRecipe");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void ResetSmokeDetectorAlarm()
        {
            CommandExecution execution = new CommandExecution(this, "ResetSmokeDetectorAlarm");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ResetSmokeDetectorAlarmAsync()
        {
            CommandExecution execution = new CommandExecution(this, "ResetSmokeDetectorAlarm");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.Shared.Data.Enum.ActorType ActorType
        {
            get { return (UnitySC.Shared.Data.Enum.ActorType)GetStatusValue("ActorType"); }
            protected set { SetStatusValue("ActorType", value); }
        }

        public UnitySC.Equipment.Abstractions.Enums.ProcessModuleState ProcessModuleState
        {
            get { return (UnitySC.Equipment.Abstractions.Enums.ProcessModuleState)GetStatusValue("ProcessModuleState"); }
            protected set { SetStatusValue("ProcessModuleState", value); }
        }

        public UnitySC.Equipment.Abstractions.Enums.ProcessModuleState PreviousProcessModuleState
        {
            get { return (UnitySC.Equipment.Abstractions.Enums.ProcessModuleState)GetStatusValue("PreviousProcessModuleState"); }
            protected set { SetStatusValue("PreviousProcessModuleState", value); }
        }

        public string SelectedRecipe
        {
            get { return (string)GetStatusValue("SelectedRecipe"); }
            protected set { SetStatusValue("SelectedRecipe", value); }
        }

        public UnitsNet.Ratio RecipeProgress
        {
            get { return (UnitsNet.Ratio)GetStatusValue("RecipeProgress"); }
            protected set
            {
                UnitsNet.Units.RatioUnit? unit = DeviceType.AllStatuses().Get("RecipeProgress").Unit as UnitsNet.Units.RatioUnit?;
                UnitsNet.Ratio newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("RecipeProgress", newValue);
            }
        }

        public UnitySC.Shared.Data.Enum.EnumPMTransferState TransferState
        {
            get { return (UnitySC.Shared.Data.Enum.EnumPMTransferState)GetStatusValue("TransferState"); }
            protected set { SetStatusValue("TransferState", value); }
        }

        public bool TransferValidationState
        {
            get { return (bool)GetStatusValue("TransferValidationState"); }
            protected set { SetStatusValue("TransferValidationState", value); }
        }
    }
}
