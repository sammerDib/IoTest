using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem
{
    public partial class Dio1MediumSizeEfem : UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.GenericRC5xx, IDio1MediumSizeEfem
    {
        public static new readonly DeviceType Type;

        static Dio1MediumSizeEfem()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Dio1MediumSizeEfem.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Dio1MediumSizeEfem");
            }
        }

        public Dio1MediumSizeEfem(string name)
            : this(name, Type)
        {
        }

        protected Dio1MediumSizeEfem(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "SetLightColor":
                    {
                        Agileo.SemiDefinitions.LightColors color = (Agileo.SemiDefinitions.LightColors)execution.Context.GetArgument("color");

                        Agileo.GUI.Services.LightTower.LightState mode = (Agileo.GUI.Services.LightTower.LightState)execution.Context.GetArgument("mode");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetLightColor(color, mode);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetLightColor(color, mode, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetLightColor interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetBuzzerState":
                    {
                        Agileo.SemiDefinitions.BuzzerState state = (Agileo.SemiDefinitions.BuzzerState)execution.Context.GetArgument("state");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetBuzzerState(state);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetBuzzerState(state, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetBuzzerState interrupted.");
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

        public void SetLightColor(Agileo.SemiDefinitions.LightColors color,Agileo.GUI.Services.LightTower.LightState mode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("color", color));
            arguments.Add(new Argument("mode", mode));
            CommandExecution execution = new CommandExecution(this, "SetLightColor", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetLightColorAsync(Agileo.SemiDefinitions.LightColors color,Agileo.GUI.Services.LightTower.LightState mode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("color", color));
            arguments.Add(new Argument("mode", mode));
            CommandExecution execution = new CommandExecution(this, "SetLightColor", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetBuzzerState(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "SetBuzzerState", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetBuzzerStateAsync(Agileo.SemiDefinitions.BuzzerState state)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("state", state));
            CommandExecution execution = new CommandExecution(this, "SetBuzzerState", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode ErrorDescription
        {
            get { return (UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode)GetStatusValue("ErrorDescription"); }
            protected set { SetStatusValue("ErrorDescription", value); }
        }

        public bool I_MaintenanceSwitch
        {
            get { return (bool)GetStatusValue("I_MaintenanceSwitch"); }
            protected set { SetStatusValue("I_MaintenanceSwitch", value); }
        }

        public bool I_PressureSensor_VAC
        {
            get { return (bool)GetStatusValue("I_PressureSensor_VAC"); }
            protected set { SetStatusValue("I_PressureSensor_VAC", value); }
        }

        public bool I_Led_PushButton
        {
            get { return (bool)GetStatusValue("I_Led_PushButton"); }
            protected set { SetStatusValue("I_Led_PushButton", value); }
        }

        public bool I_PressureSensor_ION_AIR
        {
            get { return (bool)GetStatusValue("I_PressureSensor_ION_AIR"); }
            protected set { SetStatusValue("I_PressureSensor_ION_AIR", value); }
        }

        public bool I_Ionizer1Alarm
        {
            get { return (bool)GetStatusValue("I_Ionizer1Alarm"); }
            protected set { SetStatusValue("I_Ionizer1Alarm", value); }
        }

        public bool I_LightCurtain
        {
            get { return (bool)GetStatusValue("I_LightCurtain"); }
            protected set { SetStatusValue("I_LightCurtain", value); }
        }

        public bool I_PM1_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM1_DoorOpened"); }
            protected set { SetStatusValue("I_PM1_DoorOpened", value); }
        }

        public bool I_PM1_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM1_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM1_ReadyToLoadUnload", value); }
        }

        public bool I_PM2_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM2_DoorOpened"); }
            protected set { SetStatusValue("I_PM2_DoorOpened", value); }
        }

        public bool I_PM2_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM2_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM2_ReadyToLoadUnload", value); }
        }

        public bool I_PM3_DoorOpened
        {
            get { return (bool)GetStatusValue("I_PM3_DoorOpened"); }
            protected set { SetStatusValue("I_PM3_DoorOpened", value); }
        }

        public bool I_PM3_ReadyToLoadUnload
        {
            get { return (bool)GetStatusValue("I_PM3_ReadyToLoadUnload"); }
            protected set { SetStatusValue("I_PM3_ReadyToLoadUnload", value); }
        }

        public bool O_RobotArmNotExtended_PM1
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM1"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM1", value); }
        }

        public bool O_RobotArmNotExtended_PM2
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM2"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM2", value); }
        }

        public bool O_RobotArmNotExtended_PM3
        {
            get { return (bool)GetStatusValue("O_RobotArmNotExtended_PM3"); }
            protected set { SetStatusValue("O_RobotArmNotExtended_PM3", value); }
        }

        public bool O_SignalTower_LightningRed
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningRed"); }
            protected set { SetStatusValue("O_SignalTower_LightningRed", value); }
        }

        public bool O_SignalTower_LightningYellow
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningYellow"); }
            protected set { SetStatusValue("O_SignalTower_LightningYellow", value); }
        }

        public bool O_SignalTower_LightningGreen
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningGreen"); }
            protected set { SetStatusValue("O_SignalTower_LightningGreen", value); }
        }

        public bool O_SignalTower_LightningBlue
        {
            get { return (bool)GetStatusValue("O_SignalTower_LightningBlue"); }
            protected set { SetStatusValue("O_SignalTower_LightningBlue", value); }
        }

        public bool O_SignalTower_BlinkingRed
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingRed"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingRed", value); }
        }

        public bool O_SignalTower_BlinkingYellow
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingYellow"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingYellow", value); }
        }

        public bool O_SignalTower_BlinkingGreen
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingGreen"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingGreen", value); }
        }

        public bool O_SignalTower_BlinkingBlue
        {
            get { return (bool)GetStatusValue("O_SignalTower_BlinkingBlue"); }
            protected set { SetStatusValue("O_SignalTower_BlinkingBlue", value); }
        }

        public bool O_SignalTower_Buzzer1
        {
            get { return (bool)GetStatusValue("O_SignalTower_Buzzer1"); }
            protected set { SetStatusValue("O_SignalTower_Buzzer1", value); }
        }

        public bool O_SignalTower_Buzzer2
        {
            get { return (bool)GetStatusValue("O_SignalTower_Buzzer2"); }
            protected set { SetStatusValue("O_SignalTower_Buzzer2", value); }
        }

        public bool O_SignalTower_PowerSupply
        {
            get { return (bool)GetStatusValue("O_SignalTower_PowerSupply"); }
            protected set { SetStatusValue("O_SignalTower_PowerSupply", value); }
        }
    }
}
