using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower
{
    public abstract partial class LightTower : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, ILightTower
    {
        public static new readonly DeviceType Type;

        static LightTower()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower");
            }
        }

        public LightTower(string name)
            : this(name, Type)
        {
        }

        protected LightTower(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "DefineGreenLightMode":
                    {
                        Agileo.GUI.Services.LightTower.LightState state = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineGreenLightMode(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineGreenLightMode(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineGreenLightMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DefineOrangeLightMode":
                    {
                        Agileo.GUI.Services.LightTower.LightState state = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineOrangeLightMode(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineOrangeLightMode(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineOrangeLightMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DefineBlueLightMode":
                    {
                        Agileo.GUI.Services.LightTower.LightState state = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineBlueLightMode(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineBlueLightMode(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineBlueLightMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DefineRedLightMode":
                    {
                        Agileo.GUI.Services.LightTower.LightState state = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineRedLightMode(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineRedLightMode(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineRedLightMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DefineBuzzerMode":
                    {
                        Agileo.SemiDefinitions.BuzzerState state = (Agileo.SemiDefinitions.BuzzerState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineBuzzerMode(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineBuzzerMode(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineBuzzerMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DefineState":
                    {
                        UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState state = (UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDefineState(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDefineState(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DefineState interrupted.");
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

        public void DefineGreenLightMode(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineGreenLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineGreenLightModeAsync(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineGreenLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DefineOrangeLightMode(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineOrangeLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineOrangeLightModeAsync(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineOrangeLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DefineBlueLightMode(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineBlueLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineBlueLightModeAsync(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineBlueLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DefineRedLightMode(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineRedLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineRedLightModeAsync(Agileo.GUI.Services.LightTower.LightState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineRedLightMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DefineBuzzerMode(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineBuzzerMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineBuzzerModeAsync(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineBuzzerMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DefineState(UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineState", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DefineStateAsync(UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "DefineState", arguments);
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

        public Agileo.GUI.Services.LightTower.LightState GreenLight
        {
            get { return (Agileo.GUI.Services.LightTower.LightState)GetStatusValue("GreenLight"); }
            protected set { SetStatusValue("GreenLight", value); }
        }

        public Agileo.GUI.Services.LightTower.LightState OrangeLight
        {
            get { return (Agileo.GUI.Services.LightTower.LightState)GetStatusValue("OrangeLight"); }
            protected set { SetStatusValue("OrangeLight", value); }
        }

        public Agileo.GUI.Services.LightTower.LightState BlueLight
        {
            get { return (Agileo.GUI.Services.LightTower.LightState)GetStatusValue("BlueLight"); }
            protected set { SetStatusValue("BlueLight", value); }
        }

        public Agileo.GUI.Services.LightTower.LightState RedLight
        {
            get { return (Agileo.GUI.Services.LightTower.LightState)GetStatusValue("RedLight"); }
            protected set { SetStatusValue("RedLight", value); }
        }

        public Agileo.SemiDefinitions.BuzzerState BuzzerState
        {
            get { return (Agileo.SemiDefinitions.BuzzerState)GetStatusValue("BuzzerState"); }
            protected set { SetStatusValue("BuzzerState", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState SignalTowerState
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.LightTower.Enums.LightTowerState)GetStatusValue("SignalTowerState"); }
            protected set { SetStatusValue("SignalTowerState", value); }
        }
    }
}
