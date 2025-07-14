using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Driver.Statuses;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

using ErrorCode = UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode;
using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem
{
    public partial class Dio1MediumSizeEfem
        : IConfigurableDevice<GenericRC5xxConfiguration>, ILightTowerIos, IProcessModuleIos
    {
        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver.Dio1SignalDataReceived += Driver_Dio1SignalDataReceived;
                    }
                    else
                    {
                        SetUpSimulatedMode();
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Properties

        public new Dio1MediumSizeEfemDriver Driver
        {
            get => base.Driver as Dio1MediumSizeEfemDriver;
            set => base.Driver = value;
        }

        #endregion Properties

        #region Configuration

        public GenericRC5xxConfiguration CreateDefaultConfiguration()
        {
            return new GenericRC5xxConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(IoModule)}/{nameof(RC530)}/{nameof(Dio1MediumSizeEfem)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= ConfigurationExtension.LoadDeviceConfiguration(
                this,
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region Event Handlers

        private void Driver_Dio1SignalDataReceived(
            object sender,
            StatusEventArgs<Dio1MediumSizeEfemSignalData> e)
        {
            // Inputs
            I_MaintenanceSwitch = e.Status.MaintenanceSwitch;
            I_PressureSensor_VAC = e.Status.PressureSensor_VAC;
            I_Led_PushButton = e.Status.Led_PushButton;
            I_PressureSensor_ION_AIR = e.Status.PressureSensor_ION_AIR;
            I_Ionizer1Alarm = e.Status.Ionizer1Alarm;
            I_LightCurtain = e.Status.LightCurtain;
            I_PM1_DoorOpened = e.Status.PM1_DoorOpened;
            I_PM1_ReadyToLoadUnload = e.Status.PM1_ReadyToLoadUnload;
            I_PM2_DoorOpened = e.Status.PM2_DoorOpened;
            I_PM2_ReadyToLoadUnload = e.Status.PM2_ReadyToLoadUnload;
            I_PM3_DoorOpened = e.Status.PM3_DoorOpened;
            I_PM3_ReadyToLoadUnload = e.Status.PM3_ReadyToLoadUnload;

            // Outputs
            O_RobotArmNotExtended_PM1 = e.Status.RobotArmNotExtended_PM1
                                        ?? throw new InvalidOperationException(
                                            "Given status could not be null when received from equipment.");
            O_RobotArmNotExtended_PM2 = e.Status.RobotArmNotExtended_PM2
                                        ?? throw new InvalidOperationException(
                                            "Given status could not be null when received from equipment.");
            O_RobotArmNotExtended_PM3 = e.Status.RobotArmNotExtended_PM3
                                        ?? throw new InvalidOperationException(
                                            "Given status could not be null when received from equipment.");
            O_SignalTower_LightningRed = e.Status.SignalTower_LightningRed
                                         ?? throw new InvalidOperationException(
                                             "Given status could not be null when received from equipment.");
            O_SignalTower_LightningYellow = e.Status.SignalTower_LightningYellow
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
            O_SignalTower_LightningGreen = e.Status.SignalTower_LightningGreen
                                           ?? throw new InvalidOperationException(
                                               "Given status could not be null when received from equipment.");
            O_SignalTower_LightningBlue = e.Status.SignalTower_LightningBlue
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
            O_SignalTower_BlinkingRed = e.Status.SignalTower_BlinkingRed
                                        ?? throw new InvalidOperationException(
                                            "Given status could not be null when received from equipment.");
            O_SignalTower_BlinkingYellow = e.Status.SignalTower_BlinkingYellow
                                           ?? throw new InvalidOperationException(
                                               "Given status could not be null when received from equipment.");
            O_SignalTower_BlinkingGreen = e.Status.SignalTower_BlinkingGreen
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
            O_SignalTower_BlinkingBlue = e.Status.SignalTower_BlinkingBlue
                                         ?? throw new InvalidOperationException(
                                             "Given status could not be null when received from equipment.");
            O_SignalTower_Buzzer1 = e.Status.SignalTower_Buzzer1
                                    ?? throw new InvalidOperationException(
                                        "Given status could not be null when received from equipment.");
            O_SignalTower_Buzzer2 = e.Status.SignalTower_Buzzer2
                                    ?? throw new InvalidOperationException(
                                        "Given status could not be null when received from equipment.");
            O_SignalTower_PowerSupply = e.Status.SignalTowerPowerSupply
                                        ?? throw new InvalidOperationException(
                                            "Given status could not be null when received from equipment.");
        }

        #endregion Event Handlers

        #region Commands

        protected virtual void InternalSetLightColor(LightColors color, LightState mode)
        {
            try
            {
                // Update only the needed bits (outputs are null by default and a mask is used to only set non-null outputs)
                var outputSignal = new Dio1MediumSizeEfemSignalData()
                {
                    SignalTowerPowerSupply = true
                };
                switch (color)
                {
                    case LightColors.Red:
                        outputSignal.SignalTower_LightningRed = mode == LightState.On;
                        outputSignal.SignalTower_BlinkingRed = mode == LightState.Flashing
                                                               || mode == LightState.FlashingFast
                                                               || mode == LightState.FlashingSlow;
                        break;
                    case LightColors.Blue:
                        outputSignal.SignalTower_LightningBlue = mode == LightState.On;
                        outputSignal.SignalTower_BlinkingBlue = mode == LightState.Flashing
                                                                || mode == LightState.FlashingFast
                                                                || mode == LightState.FlashingSlow;
                        break;
                    case LightColors.Yellow:
                    case LightColors.Orange:
                        outputSignal.SignalTower_LightningYellow = mode == LightState.On;
                        outputSignal.SignalTower_BlinkingYellow = mode == LightState.Flashing
                            || mode == LightState.FlashingFast
                            || mode == LightState.FlashingSlow;
                        break;
                    case LightColors.Green:
                        outputSignal.SignalTower_LightningGreen = mode == LightState.On;
                        outputSignal.SignalTower_BlinkingGreen = mode == LightState.Flashing
                                                                 || mode == LightState.FlashingFast
                                                                 || mode == LightState.FlashingSlow;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, null);
                }

                InternalSetOutputSignal(outputSignal);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected virtual void InternalSetBuzzerState(BuzzerState state)
        {
            try
            {
                // Update only the needed bits (outputs are null by default and a mask is used to only set non-null outputs)
                var outputSignal = new Dio1MediumSizeEfemSignalData
                {
                    SignalTowerPowerSupply = true,
                    SignalTower_Buzzer1 = state == BuzzerState.Slow,
                    SignalTower_Buzzer2 = state == BuzzerState.Fast
                };
                InternalSetOutputSignal(outputSignal);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Overrides

        protected override GenericRC5xxDriver CreateDriver()
        {
            return new Dio1MediumSizeEfemDriver(
                Logger,
                Configuration.CommunicationConfig.ConnectionMode,
                Configuration.CommunicationConfig.AliveBitPeriod);
        }

        protected override void UpdateErrorDescription(int partOfEfemInError, int errorCode)
        {
            ErrorDescription = (ErrorCode)errorCode;
        }

        protected override void SetOrClearAlarmByKey(int statusErrorCode)
        {
            if ((ErrorCode)statusErrorCode != RC530.Driver.Enums.ErrorCode.NoError)
            {
                //New alarm detected
                SetAlarmById((statusErrorCode + 1000).ToString());
            }
            else
            {
                //Clear the previously set alarm
                ClearAlarmById(((int)ErrorDescription + 1000).ToString());
            }
        }

        #endregion Overrides

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (Driver != null)
            {
                Driver.Dio1SignalDataReceived -= Driver_Dio1SignalDataReceived;
            }

            if (SimulationData != null)
            {
                DisposeSimulatedMode();
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
