using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner
{
    public abstract partial class Aligner : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IAligner
    {
        public static new readonly DeviceType Type;

        static Aligner()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner");
            }
        }

        public Aligner(string name)
            : this(name, Type)
        {
        }

        protected Aligner(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "Align":
                    {
                        UnitsNet.Angle target = (UnitsNet.Angle)execution.Context.GetArgument("target");
                        UnitsNet.Units.AngleUnit? unittarget = execution.Context.Command.Parameters.FirstOrDefault(p => p.Name == "target").Unit as UnitsNet.Units.AngleUnit?;
                        if (unittarget != null && unittarget != target.Unit)
                        {
                            target = target.ToUnit(unittarget.Value);
                        }

                        UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType aligntype = (UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType)execution.Context.GetArgument("alignType");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalAlign(target, aligntype);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateAlign(target, aligntype, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Align interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Centering":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalCentering();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateCentering(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Centering interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "PrepareTransfer":
                    {
                        UnitySC.Equipment.Abstractions.Enums.EffectorType effector = (UnitySC.Equipment.Abstractions.Enums.EffectorType)execution.Context.GetArgument("effector");

                        Agileo.SemiDefinitions.SampleDimension dimension = (Agileo.SemiDefinitions.SampleDimension)execution.Context.GetArgument("dimension");

                        UnitySC.Equipment.Abstractions.Enums.MaterialType materialtype = (UnitySC.Equipment.Abstractions.Enums.MaterialType)execution.Context.GetArgument("materialType");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPrepareTransfer(effector, dimension, materialtype);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePrepareTransfer(effector, dimension, materialtype, execution.Tempomat);
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

                case "Clamp":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalClamp();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateClamp(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Clamp interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Unclamp":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalUnclamp();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateUnclamp(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Unclamp interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "MoveZAxis":
                    {
                        System.Boolean isbottom = (System.Boolean)execution.Context.GetArgument("isBottom");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalMoveZAxis(isbottom);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateMoveZAxis(isbottom, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command MoveZAxis interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetDateAndTime":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetDateAndTime();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetDateAndTime(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetDateAndTime interrupted.");
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

        public void Align(UnitsNet.Angle target,UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType alignType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("target", target));
            arguments.Add(new Argument("alignType", alignType));
            CommandExecution execution = new CommandExecution(this, "Align", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task AlignAsync(UnitsNet.Angle target,UnitySC.Equipment.Abstractions.Devices.Aligner.Enums.AlignType alignType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("target", target));
            arguments.Add(new Argument("alignType", alignType));
            CommandExecution execution = new CommandExecution(this, "Align", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Centering()
        {
            CommandExecution execution = new CommandExecution(this, "Centering");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task CenteringAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Centering");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void PrepareTransfer(UnitySC.Equipment.Abstractions.Enums.EffectorType effector,Agileo.SemiDefinitions.SampleDimension dimension,UnitySC.Equipment.Abstractions.Enums.MaterialType materialType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("effector", effector));
            arguments.Add(new Argument("dimension", dimension));
            arguments.Add(new Argument("materialType", materialType));
            CommandExecution execution = new CommandExecution(this, "PrepareTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PrepareTransferAsync(UnitySC.Equipment.Abstractions.Enums.EffectorType effector,Agileo.SemiDefinitions.SampleDimension dimension,UnitySC.Equipment.Abstractions.Enums.MaterialType materialType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("effector", effector));
            arguments.Add(new Argument("dimension", dimension));
            arguments.Add(new Argument("materialType", materialType));
            CommandExecution execution = new CommandExecution(this, "PrepareTransfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Clamp()
        {
            CommandExecution execution = new CommandExecution(this, "Clamp");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ClampAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Clamp");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Unclamp()
        {
            CommandExecution execution = new CommandExecution(this, "Unclamp");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task UnclampAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Unclamp");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void MoveZAxis(bool isBottom)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("isBottom", isBottom));
            CommandExecution execution = new CommandExecution(this, "MoveZAxis", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task MoveZAxisAsync(bool isBottom)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("isBottom", isBottom));
            CommandExecution execution = new CommandExecution(this, "MoveZAxis", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetDateAndTime()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetDateAndTimeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public Agileo.SemiDefinitions.SampleDimension WaferDimension
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("WaferDimension"); }
            protected set { SetStatusValue("WaferDimension", value); }
        }

        public string SimplifiedWaferId
        {
            get { return (string)GetStatusValue("SimplifiedWaferId"); }
            protected set { SetStatusValue("SimplifiedWaferId", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus WaferStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus)GetStatusValue("WaferStatus"); }
            protected set { SetStatusValue("WaferStatus", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence WaferPresence
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence)GetStatusValue("WaferPresence"); }
            protected set { SetStatusValue("WaferPresence", value); }
        }

        public bool SubstrateDetectionError
        {
            get { return (bool)GetStatusValue("SubstrateDetectionError"); }
            protected set { SetStatusValue("SubstrateDetectionError", value); }
        }

        public bool IsClamped
        {
            get { return (bool)GetStatusValue("IsClamped"); }
            protected set { SetStatusValue("IsClamped", value); }
        }
    }
}
