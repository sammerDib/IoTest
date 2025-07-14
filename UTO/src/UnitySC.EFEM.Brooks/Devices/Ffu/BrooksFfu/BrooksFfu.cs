using System;

using Agileo.EquipmentModeling;

using BAI.General;
using BAI.Systems.Devices.FFU;
using BAI.Systems.Modules.EFEM;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.EFEM.Brooks.Devices.Ffu.BrooksFfu.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Resources;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Brooks.Devices.Ffu.BrooksFfu
{
    public partial class BrooksFfu : IConfigurableDevice<BrooksFfuConfiguration>
    {
        #region Fields

        private FfuRemoteProxy _ffuRemoteProxy;
        private EfemProxy _efemProxy;

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

                        var ffu = _efemProxy.GetDevice(Configuration.BrooksFfuName);
                        if (ffu is not FfuRemoteProxy ffuRemoteProxy)
                        {
                            throw new InvalidOperationException(
                                Resources.Messages.FfuNotPresentInEfemConfig);
                        }

                        _ffuRemoteProxy = ffuRemoteProxy;
                        _ffuRemoteProxy.FfuPressureChanged += FfuRemoteProxy_FfuPressureChanged;
                        _ffuRemoteProxy.FfuPwmChanged += FfuRemoteProxy_FfuPwmChanged;
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
                if (!_ffuRemoteProxy.Connected)
                {
                    _ffuRemoteProxy.Connect();
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
                if (_ffuRemoteProxy.Connected)
                {
                    _ffuRemoteProxy.Disconnect();
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

        #region IConfigurableDevice

        public new BrooksFfuConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksFfuConfiguration>(base.Configuration);

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        public new BrooksFfuConfiguration CreateDefaultConfiguration()
        {
            return new BrooksFfuConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Ffu)}/{nameof(BrooksFfu)}/Resources";

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _ffuRemoteProxy.Initialize();

                //Check device ready
                if (!_ffuRemoteProxy.IsOperable())
                {
                    throw new InvalidOperationException(Resources.Messages.FfuNotOperable);
                }

                if (_ffuRemoteProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Resources.Messages.FfuInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IFfu Commands

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

        protected override void InternalSetFfuSpeed(double setPoint, FfuSpeedUnit unit)
        {
            try
            {
                if (unit == FfuSpeedUnit.Rpm)
                {
                    throw new InvalidOperationException("RPM unit is not supported by this FFU");
                }

                var mode = unit == FfuSpeedUnit.Pwm
                    ? FfuCtrlMode.Pwm
                    : FfuCtrlMode.Pressure;
                _ffuRemoteProxy.ChangeControl(
                    mode,
                    new NumberWithUnit(
                        setPoint,
                        mode == FfuCtrlMode.Pwm
                            ? Constants.PwmUnit
                            : Constants.PressureUnit));
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Event handler

        private void FfuRemoteProxy_FfuPwmChanged(
            string source,
            string sensor,
            NumberWithUnit after)
        {
            FanSpeed = RotationalSpeed.From(after.Number, RotationalSpeedUnit.RevolutionPerMinute);
        }

        private void FfuRemoteProxy_FfuPressureChanged(
            string source,
            string sensor,
            NumberWithUnit after)
        {
            DifferentialPressure = Pressure.From(after.Number, PressureUnit.InchOfWaterColumn);
        }

        #endregion

        #region public

        public override string IsFfuSpeedValid(double setPoint, FfuSpeedUnit unit)
        {
            if (unit is not FfuSpeedUnit.Pressure and not FfuSpeedUnit.Pwm)
            {
                return Messages.InvalidFfuSpeedUnitPwm;
            }

            return String.Empty;
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
                if (_ffuRemoteProxy != null)
                {
                    _ffuRemoteProxy.FfuPressureChanged -= FfuRemoteProxy_FfuPressureChanged;
                    _ffuRemoteProxy.FfuPwmChanged -= FfuRemoteProxy_FfuPwmChanged;
                    _ffuRemoteProxy.Dispose();
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
