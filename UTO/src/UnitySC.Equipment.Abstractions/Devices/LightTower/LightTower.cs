using System;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower
{
    public partial class LightTower : IConfigurableDevice<LightTowerConfiguration>
    {
        #region Private Methods

        private void InstanceInitialization()
        {
            GreenLight = LightState.Off;
            OrangeLight = LightState.Off;
            BlueLight = LightState.Off;
            RedLight = LightState.Off;
            BuzzerState = BuzzerState.Off;
        }

        #endregion Private Methods

        #region Override

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Override

        #region Commands

        protected void InternalDefineGreenLightMode(LightState state)
        {
            try
            {
                SetLightColor(LightColors.Green, state);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected void InternalDefineOrangeLightMode(LightState state)
        {
            try
            {
                SetLightColor(LightColors.Orange, state);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected void InternalDefineBlueLightMode(LightState state)
        {
            try
            {
                SetLightColor(LightColors.Blue, state);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected void InternalDefineRedLightMode(LightState state)
        {
            try
            {
                SetLightColor(LightColors.Red, state);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected void InternalDefineBuzzerMode(BuzzerState state)
        {
            try
            {
                SetBuzzerState(state);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected abstract void InternalSetDateAndTime();

        protected virtual void InternalDefineState(Enums.LightTowerState state)
        {
            if (!Configuration.LightTowerStatuses.Any())
            {
                //If no configuration is available it means that we are using EFEM Ctrl and we do not pilot the light tower
                return;
            }

            // Get each light state in configuration
            var stateConfig =
                Configuration.LightTowerStatuses.Find(s => s.LightTowerState == state);

            if (stateConfig == null)
            {
                throw new ArgumentNullException(
                    $"No light configuration exists for SignalTower state '{state}'.");
            }

            InternalDefineGreenLightMode(UpdateLightState(stateConfig.Green, GreenLight));
            InternalDefineOrangeLightMode(UpdateLightState(stateConfig.Orange, OrangeLight));
            InternalDefineBlueLightMode(UpdateLightState(stateConfig.Blue, BlueLight));
            InternalDefineRedLightMode(UpdateLightState(stateConfig.Red, RedLight));
            InternalDefineBuzzerMode(stateConfig.BuzzerState);
            SignalTowerState = state;
        }

        #endregion Commands

        #region Other Methods

        protected abstract void SetLightColor(LightColors color, LightState mode);

        protected abstract void SetBuzzerState(BuzzerState state);

        #endregion Other Methods

        #region Helpers

        protected Agileo.SemiDefinitions.LightState ToSemiLightStates(LightState state)
        {
            switch (state)
            {
                case LightState.Off:
                    return Agileo.SemiDefinitions.LightState.Off;
                case LightState.On:
                    return Agileo.SemiDefinitions.LightState.On;
                case LightState.Flashing:
                    return Agileo.SemiDefinitions.LightState.Flashing;
                case LightState.FlashingSlow:
                    return Agileo.SemiDefinitions.LightState.FlashingSlow;
                case LightState.FlashingFast:
                    return Agileo.SemiDefinitions.LightState.FlashingFast;
                default:
                    return Agileo.SemiDefinitions.LightState.Undetermined;
            }
        }

        protected LightState UpdateLightState(
            Agileo.SemiDefinitions.LightState newLightState,
            LightState currentLightState)
        {
            switch (newLightState)
            {
                case Agileo.SemiDefinitions.LightState.Undetermined:
                    return currentLightState;
                case Agileo.SemiDefinitions.LightState.Off:
                    return LightState.Off;
                case Agileo.SemiDefinitions.LightState.On:
                    return LightState.On;
                case Agileo.SemiDefinitions.LightState.Flashing:
                    return LightState.Flashing;
                case Agileo.SemiDefinitions.LightState.FlashingSlow:
                    return LightState.FlashingSlow;
                case Agileo.SemiDefinitions.LightState.FlashingFast:
                    return LightState.FlashingFast;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(newLightState),
                        newLightState,
                        null);
            }
        }

        #endregion Helpers

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public LightTowerConfiguration Configuration
            => ConfigManager.Current.Cast<LightTowerConfiguration>();

        public virtual string RelativeConfigurationDir
            => $"./Devices/{nameof(LightTower)}/Resources";

        public virtual void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        public LightTowerConfiguration CreateDefaultConfiguration()
        {
            return new LightTowerConfiguration();
        }

        #endregion
    }
}
