using System;

using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2
{
    public partial class Dio2 : IConfigurableDevice<GenericRC5xxConfiguration>, IProcessModuleIos
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
                        Driver.Dio2SignalDataReceived += Driver_Dio2SignalDataReceived;
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

        #endregion Setup

        #region Properties

        public new Dio2Driver Driver
        {
            get => base.Driver as Dio2Driver;
            set => base.Driver = value;
        }

        #endregion Properties

        #region Configuration

        public GenericRC5xxConfiguration CreateDefaultConfiguration()
        {
            return new GenericRC5xxConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(IoModule)}/{nameof(RC530)}/{nameof(Dio2)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region Event Handlers

        private void Driver_Dio2SignalDataReceived(object sender, StatusEventArgs<Dio2SignalData> e)
        {
            // Inputs
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
        }

        #endregion Event Handlers

        #region Overrides

        protected override GenericRC5xxDriver CreateDriver()
        {
            return new Dio2Driver(Logger, Configuration.CommunicationConfig.ConnectionMode, Configuration.CommunicationConfig.AliveBitPeriod);
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
                Driver.Dio2SignalDataReceived -= Driver_Dio2SignalDataReceived;
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
