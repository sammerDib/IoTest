using System;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Resources;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.EFEM.Rorze.Devices.Ffu
{
    public partial class Ffu : IConfigurableDevice<FfuConfiguration>
    {
        #region Properties

        private IFfuIos IoModule { get; set; }

        private const string _lowSpeedThresholdAlarmKey = "FFU_011";
        private const string _lowPressureThresholdAlarmKey = "FFU_012";

        #endregion

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
                    var ioModule = this.GetTopDeviceContainer()
                        .AllDevices()
                        .FirstOrDefault(d => d is IFfuIos);
                    if (ioModule is not IFfuIos ffuIoModule)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IFfuIos)} is not found in equipment model tree.");
                    }

                    IoModule = ffuIoModule;
                    if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    IoModule.StatusValueChanged += IoModule_StatusValueChanged;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region Configuration

        public override string RelativeConfigurationDir => $"./Devices/{nameof(Ffu)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            IoModule.StartCommunication();
        }

        protected override void InternalStopCommunication()
        {
            IoModule.StopCommunication();
        }

        #endregion ICommunicatingDevice Commands

        #region IFfu Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            base.InternalInitialize(mustForceInit);
            try
            {
                IoModule.SetFfuSpeed(
                    RotationalSpeed.FromRevolutionsPerMinute(Configuration.FfuSetPoint));
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
                if (unit != FfuSpeedUnit.Rpm)
                {
                    throw new InvalidOperationException("Units other than RPM are not supported by this FFU");
                }

                var ffuSpeed = RotationalSpeed.From(
                    setPoint,
                    RotationalSpeedUnit.RevolutionPerMinute);
                IoModule.SetFfuSpeed(ffuSpeed);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                IoModule.SetDateAndTime();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }



        #endregion IFfu Commands

        #region public

        public override string IsFfuSpeedValid(double setPoint, FfuSpeedUnit unit)
        {
            if (unit != FfuSpeedUnit.Rpm)
            {
                return Messages.InvalidFfuSpeedUnitRpm;
            }

            if (setPoint is not 0 and (< 300 or > 1650))
            {
                return Messages.RotationalSpeedNotInRange;
            }

            return string.Empty;
        }

        #endregion

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IFfuIos.Alarm):
                    HasAlarm = IoModule.Alarm;
                    break;
                case nameof(IFfuIos.FanSpeed):
                    FanSpeed = IoModule.FanSpeed;
                    if (FanSpeed.RevolutionsPerMinute < Configuration.LowSpeedThreshold)
                    {
                        SetAlarm(_lowSpeedThresholdAlarmKey);
                    }

                    break;
                case nameof(IFfuIos.MeanPressure):
                    DifferentialPressure = IoModule.MeanPressure;
                    if (DifferentialPressure.Value < Configuration.LowPressureThreshold)
                    {
                        SetAlarm(_lowPressureThresholdAlarmKey);
                    }

                    break;
                case nameof(IFfuIos.IsCommunicating):
                    IsCommunicating = IoModule.IsCommunicating;
                    break;
                case nameof(IFfuIos.IsCommunicationStarted):
                    IsCommunicationStarted = IoModule.IsCommunicationStarted;
                    break;
            }

            if (State is OperatingModes.Maintenance or OperatingModes.Idle)
            {
                SetState(
                    !IsCommunicating || IoModule.State == OperatingModes.Maintenance
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
        }

        #endregion Event Handlers

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IoModule != null)
            {
                IoModule.StatusValueChanged -= IoModule_StatusValueChanged;
                IoModule = null;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
