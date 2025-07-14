using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    public abstract partial class Robot : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, IRobot
    {
        public static new readonly DeviceType Type;

        static Robot()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.Robot.Robot.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.Robot.Robot");
            }
        }

        public Robot(string name)
            : this(name, Type)
        {
        }

        protected Robot(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GoToHome":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGoToHome();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGoToHome(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GoToHome interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "GoToLocation":
                    {
                        Agileo.EquipmentModeling.IMaterialLocationContainer destinationdevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("destinationDevice");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGoToLocation(destinationdevice);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGoToLocation(destinationdevice, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GoToLocation interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "GoToTransferLocation":
                    {
                        Agileo.SemiDefinitions.TransferLocation location = (Agileo.SemiDefinitions.TransferLocation)execution.Context.GetArgument("location");

                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        System.Byte slot = (System.Byte)execution.Context.GetArgument("slot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGoToTransferLocation(location, arm, slot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGoToTransferLocation(location, arm, slot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GoToTransferLocation interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "GoToSpecifiedLocation":
                    {
                        Agileo.EquipmentModeling.IMaterialLocationContainer destinationdevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("destinationDevice");

                        System.Byte destinationslot = (System.Byte)execution.Context.GetArgument("destinationSlot");

                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        System.Boolean ispickupposition = (System.Boolean)execution.Context.GetArgument("isPickUpPosition");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGoToSpecifiedLocation(destinationdevice, destinationslot, arm, ispickupposition);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGoToSpecifiedLocation(destinationdevice, destinationslot, arm, ispickupposition, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GoToSpecifiedLocation interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Pick":
                    {
                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        Agileo.EquipmentModeling.IMaterialLocationContainer sourcedevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("sourceDevice");

                        System.Byte sourceslot = (System.Byte)execution.Context.GetArgument("sourceSlot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPick(arm, sourcedevice, sourceslot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePick(arm, sourcedevice, sourceslot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Pick interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Place":
                    {
                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        Agileo.EquipmentModeling.IMaterialLocationContainer destinationdevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("destinationDevice");

                        System.Byte destinationslot = (System.Byte)execution.Context.GetArgument("destinationSlot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPlace(arm, destinationdevice, destinationslot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePlace(arm, destinationdevice, destinationslot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Place interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Transfer":
                    {
                        Agileo.SemiDefinitions.RobotArm pickarm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("pickArm");

                        Agileo.EquipmentModeling.IMaterialLocationContainer sourcedevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("sourceDevice");

                        System.Byte sourceslot = (System.Byte)execution.Context.GetArgument("sourceSlot");

                        Agileo.SemiDefinitions.RobotArm placearm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("placeArm");

                        Agileo.EquipmentModeling.IMaterialLocationContainer destinationdevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("destinationDevice");

                        System.Byte destinationslot = (System.Byte)execution.Context.GetArgument("destinationSlot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalTransfer(pickarm, sourcedevice, sourceslot, placearm, destinationdevice, destinationslot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateTransfer(pickarm, sourcedevice, sourceslot, placearm, destinationdevice, destinationslot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Transfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Swap":
                    {
                        Agileo.SemiDefinitions.RobotArm pickarm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("pickArm");

                        Agileo.EquipmentModeling.IMaterialLocationContainer sourcedevice = (Agileo.EquipmentModeling.IMaterialLocationContainer)execution.Context.GetArgument("sourceDevice");

                        System.Byte sourceslot = (System.Byte)execution.Context.GetArgument("sourceSlot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSwap(pickarm, sourcedevice, sourceslot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSwap(pickarm, sourcedevice, sourceslot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Swap interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "ExtendArm":
                    {
                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        Agileo.SemiDefinitions.TransferLocation location = (Agileo.SemiDefinitions.TransferLocation)execution.Context.GetArgument("location");

                        System.Byte slot = (System.Byte)execution.Context.GetArgument("slot");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalExtendArm(arm, location, slot);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateExtendArm(arm, location, slot, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command ExtendArm interrupted.");
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
                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalClamp(arm);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateClamp(arm, execution.Tempomat);
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
                        Agileo.SemiDefinitions.RobotArm arm = (Agileo.SemiDefinitions.RobotArm)execution.Context.GetArgument("arm");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalUnclamp(arm);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateUnclamp(arm, execution.Tempomat);
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

                case "SetMotionSpeed":
                    {
                        UnitsNet.Ratio percentage = (UnitsNet.Ratio)execution.Context.GetArgument("percentage");
                        UnitsNet.Units.RatioUnit? unitpercentage = execution.Context.Command.Parameters.FirstOrDefault(p => p.Name == "percentage").Unit as UnitsNet.Units.RatioUnit?;
                        if (unitpercentage != null && unitpercentage != percentage.Unit)
                        {
                            percentage = percentage.ToUnit(unitpercentage.Value);
                        }

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetMotionSpeed(percentage);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetMotionSpeed(percentage, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetMotionSpeed interrupted.");
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

        public void GoToHome()
        {
            CommandExecution execution = new CommandExecution(this, "GoToHome");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GoToHomeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GoToHome");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void GoToLocation(Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            CommandExecution execution = new CommandExecution(this, "GoToLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GoToLocationAsync(Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            CommandExecution execution = new CommandExecution(this, "GoToLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void GoToTransferLocation(Agileo.SemiDefinitions.TransferLocation location,Agileo.SemiDefinitions.RobotArm arm,byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "GoToTransferLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GoToTransferLocationAsync(Agileo.SemiDefinitions.TransferLocation location,Agileo.SemiDefinitions.RobotArm arm,byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("location", location));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "GoToTransferLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void GoToSpecifiedLocation(Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot,Agileo.SemiDefinitions.RobotArm arm,bool isPickUpPosition)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("isPickUpPosition", isPickUpPosition));
            CommandExecution execution = new CommandExecution(this, "GoToSpecifiedLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GoToSpecifiedLocationAsync(Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot,Agileo.SemiDefinitions.RobotArm arm,bool isPickUpPosition)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("isPickUpPosition", isPickUpPosition));
            CommandExecution execution = new CommandExecution(this, "GoToSpecifiedLocation", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Pick(Agileo.SemiDefinitions.RobotArm arm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            CommandExecution execution = new CommandExecution(this, "Pick", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PickAsync(Agileo.SemiDefinitions.RobotArm arm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            CommandExecution execution = new CommandExecution(this, "Pick", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Place(Agileo.SemiDefinitions.RobotArm arm,Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "Place", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PlaceAsync(Agileo.SemiDefinitions.RobotArm arm,Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "Place", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Transfer(Agileo.SemiDefinitions.RobotArm pickArm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot,Agileo.SemiDefinitions.RobotArm placeArm,Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("pickArm", pickArm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            arguments.Add(new Argument("placeArm", placeArm));
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "Transfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task TransferAsync(Agileo.SemiDefinitions.RobotArm pickArm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot,Agileo.SemiDefinitions.RobotArm placeArm,Agileo.EquipmentModeling.IMaterialLocationContainer destinationDevice,byte destinationSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("pickArm", pickArm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            arguments.Add(new Argument("placeArm", placeArm));
            arguments.Add(new Argument("destinationDevice", destinationDevice));
            arguments.Add(new Argument("destinationSlot", destinationSlot));
            CommandExecution execution = new CommandExecution(this, "Transfer", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Swap(Agileo.SemiDefinitions.RobotArm pickArm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("pickArm", pickArm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            CommandExecution execution = new CommandExecution(this, "Swap", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SwapAsync(Agileo.SemiDefinitions.RobotArm pickArm,Agileo.EquipmentModeling.IMaterialLocationContainer sourceDevice,byte sourceSlot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("pickArm", pickArm));
            arguments.Add(new Argument("sourceDevice", sourceDevice));
            arguments.Add(new Argument("sourceSlot", sourceSlot));
            CommandExecution execution = new CommandExecution(this, "Swap", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void ExtendArm(Agileo.SemiDefinitions.RobotArm arm,Agileo.SemiDefinitions.TransferLocation location,byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("location", location));
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "ExtendArm", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ExtendArmAsync(Agileo.SemiDefinitions.RobotArm arm,Agileo.SemiDefinitions.TransferLocation location,byte slot)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            arguments.Add(new Argument("location", location));
            arguments.Add(new Argument("slot", slot));
            CommandExecution execution = new CommandExecution(this, "ExtendArm", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Clamp(Agileo.SemiDefinitions.RobotArm arm)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            CommandExecution execution = new CommandExecution(this, "Clamp", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ClampAsync(Agileo.SemiDefinitions.RobotArm arm)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            CommandExecution execution = new CommandExecution(this, "Clamp", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Unclamp(Agileo.SemiDefinitions.RobotArm arm)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            CommandExecution execution = new CommandExecution(this, "Unclamp", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task UnclampAsync(Agileo.SemiDefinitions.RobotArm arm)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("arm", arm));
            CommandExecution execution = new CommandExecution(this, "Unclamp", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetMotionSpeed(UnitsNet.Ratio percentage)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("percentage", percentage));
            CommandExecution execution = new CommandExecution(this, "SetMotionSpeed", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetMotionSpeedAsync(UnitsNet.Ratio percentage)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("percentage", percentage));
            CommandExecution execution = new CommandExecution(this, "SetMotionSpeed", arguments);
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

        public Agileo.SemiDefinitions.SampleDimension UpperArmWaferDimension
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("UpperArmWaferDimension"); }
            protected set { SetStatusValue("UpperArmWaferDimension", value); }
        }

        public Agileo.SemiDefinitions.SampleDimension LowerArmWaferDimension
        {
            get { return (Agileo.SemiDefinitions.SampleDimension)GetStatusValue("LowerArmWaferDimension"); }
            protected set { SetStatusValue("LowerArmWaferDimension", value); }
        }

        public string UpperArmSimplifiedWaferId
        {
            get { return (string)GetStatusValue("UpperArmSimplifiedWaferId"); }
            protected set { SetStatusValue("UpperArmSimplifiedWaferId", value); }
        }

        public string LowerArmSimplifiedWaferId
        {
            get { return (string)GetStatusValue("LowerArmSimplifiedWaferId"); }
            protected set { SetStatusValue("LowerArmSimplifiedWaferId", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus UpperArmWaferStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus)GetStatusValue("UpperArmWaferStatus"); }
            protected set { SetStatusValue("UpperArmWaferStatus", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus LowerArmWaferStatus
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferStatus)GetStatusValue("LowerArmWaferStatus"); }
            protected set { SetStatusValue("LowerArmWaferStatus", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence UpperArmWaferPresence
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence)GetStatusValue("UpperArmWaferPresence"); }
            protected set { SetStatusValue("UpperArmWaferPresence", value); }
        }

        public UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence LowerArmWaferPresence
        {
            get { return (UnitySC.Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence)GetStatusValue("LowerArmWaferPresence"); }
            protected set { SetStatusValue("LowerArmWaferPresence", value); }
        }

        public bool UpperArmSubstrateDetectionError
        {
            get { return (bool)GetStatusValue("UpperArmSubstrateDetectionError"); }
            protected set { SetStatusValue("UpperArmSubstrateDetectionError", value); }
        }

        public bool LowerArmSubstrateDetectionError
        {
            get { return (bool)GetStatusValue("LowerArmSubstrateDetectionError"); }
            protected set { SetStatusValue("LowerArmSubstrateDetectionError", value); }
        }

        public bool UpperArmClamped
        {
            get { return (bool)GetStatusValue("UpperArmClamped"); }
            protected set { SetStatusValue("UpperArmClamped", value); }
        }

        public bool LowerArmClamped
        {
            get { return (bool)GetStatusValue("LowerArmClamped"); }
            protected set { SetStatusValue("LowerArmClamped", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Robot.Enums.ArmState UpperArmState
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Robot.Enums.ArmState)GetStatusValue("UpperArmState"); }
            protected set { SetStatusValue("UpperArmState", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.Robot.Enums.ArmState LowerArmState
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.Robot.Enums.ArmState)GetStatusValue("LowerArmState"); }
            protected set { SetStatusValue("LowerArmState", value); }
        }

        public Agileo.SemiDefinitions.TransferLocation Position
        {
            get { return (Agileo.SemiDefinitions.TransferLocation)GetStatusValue("Position"); }
            protected set { SetStatusValue("Position", value); }
        }

        public bool HasBeenInitialized
        {
            get { return (bool)GetStatusValue("HasBeenInitialized"); }
            protected set { SetStatusValue("HasBeenInitialized", value); }
        }

        public UnitsNet.Ratio Speed
        {
            get { return (UnitsNet.Ratio)GetStatusValue("Speed"); }
            protected set
            {
                UnitsNet.Units.RatioUnit? unit = DeviceType.AllStatuses().Get("Speed").Unit as UnitsNet.Units.RatioUnit?;
                UnitsNet.Ratio newValue = value;
                if (unit != null && unit.Value != value.Unit)
                {
                    newValue = value.ToUnit(unit.Value);
                }

                SetStatusValue("Speed", newValue);
            }
        }
    }
}
