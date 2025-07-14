using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.Systems.Common;
using BAI.Systems.Devices.LightTower;
using BAI.Systems.Modules.EFEM;

using UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower.Configuration;
using UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower.Resources;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower
{
    public partial class BrooksLightTower : IConfigurableDevice<BrooksLightTowerConfiguration>
    {
        #region Fields

        private LightTowerLocalProxy _lightTowerLocalProxy;
        private EfemProxy _efemProxy;

        #endregion

        #region IConfigurableDevice

        public new BrooksLightTowerConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksLightTowerConfiguration>(base.Configuration);

        public new BrooksLightTowerConfiguration CreateDefaultConfiguration()
        {
            return new BrooksLightTowerConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(LightTower)}/{nameof(BrooksLightTower)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration<BrooksLightTowerConfiguration>(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            BAI.CTC.ClientInit.ClientLibLoader.InitializeLoader();
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
                        _efemProxy = Helpers.Helper.GetEfemProxy(this);

                        var lightTower = _efemProxy.GetDevice(Configuration.BrooksLightTowerName);
                        if (lightTower is not LightTowerLocalProxy lightTowerLocalProxyProxy)
                        {
                            throw new InvalidOperationException(
                                Messages.LightTowerNotPresentInEfemConfig);
                        }

                        _lightTowerLocalProxy = lightTowerLocalProxyProxy;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                if (!_lightTowerLocalProxy.Connected)
                {
                    _lightTowerLocalProxy.Connect();
                }

                IsCommunicationStarted = true;
                IsCommunicating = true;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStopCommunication()
        {
            try
            {
                if (_lightTowerLocalProxy.Connected)
                {
                    _lightTowerLocalProxy.Disconnect();
                }

                IsCommunicationStarted = false;
                IsCommunicating = false;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _lightTowerLocalProxy.Initialize();

                //Status update
                UpdateIoStates();

                //Check device ready
                if (!_lightTowerLocalProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.LightTowerNotOperable);
                }

                if (_lightTowerLocalProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.LightTowerInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region ILightTower Commands

        protected override void InternalSetDateAndTime()
        {
            try
            {
                _efemProxy.SetControllerLocalTime(DateTime.Now);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void SetLightColor(LightColors color, LightState mode)
        {
            try
            {
                var signal = ConvertLightColorsToLightTowerComponent(color);
                var state = ConvertLightStateToUiIndicatorState(mode);
                _lightTowerLocalProxy.SetLightTowerComponentState(signal, state);
                UpdateIoStates();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void SetBuzzerState(BuzzerState state)
        {
            try
            {
                var buzzerState = ConvertBuzzerStateToUiIndicatorState(state);
                _lightTowerLocalProxy.SetLightTowerComponentState(
                    LightTowerComponent.Buzzer,
                    buzzerState);
                UpdateIoStates();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Private

        private void UpdateIoStates()
        {
            var states = _lightTowerLocalProxy.GetLightTowerState();
            foreach (var state in states)
            {
                switch (state.Signal)
                {
                    case LightTowerComponent.Blue:
                        BlueLight = ConvertUiIndicatorStateToLightState(state.State);
                        break;
                    case LightTowerComponent.Green:
                        GreenLight = ConvertUiIndicatorStateToLightState(state.State);
                        break;
                    case LightTowerComponent.Amber:
                        OrangeLight = ConvertUiIndicatorStateToLightState(state.State);
                        break;
                    case LightTowerComponent.Red:
                        RedLight = ConvertUiIndicatorStateToLightState(state.State);
                        break;
                    case LightTowerComponent.White:
                        break;
                    case LightTowerComponent.Buzzer:
                    case LightTowerComponent.Buzzer2:
                        BuzzerState = ConvertUiIndicatorStateToBuzzerState(state.State);
                        break;
                }
            }
        }

        private BuzzerState ConvertUiIndicatorStateToBuzzerState(UiIndicatorState state)
        {
            switch (state)
            {
                case UiIndicatorState.Off:
                    return BuzzerState.Off;
                case UiIndicatorState.On:
                    return BuzzerState.On;
                case UiIndicatorState.Pulsating:
                    return BuzzerState.Slow;
                case UiIndicatorState.Strobe:
                    return BuzzerState.Fast;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private UiIndicatorState ConvertBuzzerStateToUiIndicatorState(BuzzerState state)
        {
            switch (state)
            {
                case BuzzerState.Undetermined:
                case BuzzerState.Off:
                    return UiIndicatorState.Off;
                case BuzzerState.On:
                    return UiIndicatorState.On;
                case BuzzerState.Slow:
                    return UiIndicatorState.Pulsating;
                case BuzzerState.Fast:
                    return UiIndicatorState.Strobe;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private LightState ConvertUiIndicatorStateToLightState(UiIndicatorState mode)
        {
            switch (mode)
            {
                case UiIndicatorState.Off:
                    return LightState.Off;
                case UiIndicatorState.On:
                    return LightState.On;
                case UiIndicatorState.Pulsating:
                    return LightState.FlashingSlow;
                case UiIndicatorState.Strobe:
                    return LightState.FlashingFast;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private UiIndicatorState ConvertLightStateToUiIndicatorState(LightState mode)
        {
            switch (mode)
            {
                case LightState.Off:
                    return UiIndicatorState.Off;
                case LightState.On:
                    return UiIndicatorState.On;
                case LightState.Flashing:
                case LightState.FlashingSlow:
                    return UiIndicatorState.Pulsating;
                case LightState.FlashingFast:
                    return UiIndicatorState.Strobe;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private LightTowerComponent ConvertLightColorsToLightTowerComponent(LightColors color)
        {
            switch (color)
            {
                case LightColors.Red:
                    return LightTowerComponent.Red;
                case LightColors.Blue:
                    return LightTowerComponent.Blue;
                case LightColors.Orange:
                case LightColors.Yellow:
                    return LightTowerComponent.Amber;
                case LightColors.Green:
                    return LightTowerComponent.Green;
                case LightColors.White:
                    return LightTowerComponent.White;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_lightTowerLocalProxy != null)
                {
                    _lightTowerLocalProxy.Dispose();
                }

                if (_efemProxy != null)
                {
                    _efemProxy.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
