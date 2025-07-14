using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx
{
    public abstract partial class GenericRC5xx : IExtendedConfigurableDevice
    {
        #region Properties

        protected GenericRC5xxDriver Driver { get; set; }

        protected DriverWrapper DriverWrapper { get; set; }

        #endregion Properties

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
                    LoadConfiguration();
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver = CreateDriver();
                        Driver.Setup(
                            Configuration.CommunicationConfig.IpAddress,
                            Configuration.CommunicationConfig.TcpPort,
                            Configuration.CommunicationConfig.AnswerTimeout,
                            Configuration.CommunicationConfig.ConfirmationTimeout,
                            Configuration.CommunicationConfig.InitTimeout,
                            maxNbRetry: Configuration.CommunicationConfig.MaxNbRetry,
                            connectionRetryDelay: Configuration.CommunicationConfig
                                .ConnectionRetryDelay);
                        Driver.StatusReceived += Driver_StatusReceived;
                        Driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        Driver.CommunicationClosed += Driver_CommunicationClosed;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        DriverWrapper = new DriverWrapper(Driver, Logger);
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            if (State == OperatingModes.Idle && !mustForceInit)
            {
                Logger.Info("No need to initialize the device because State is already Idle");
                return;
            }

            base.InternalInitialize(mustForceInit);
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    GenericRC5xxCommand.Initialization);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericDevice Commands

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            Driver.EnableCommunications();
        }

        protected override void InternalStopCommunication()
        {
            Driver.Disconnect();
        }

        #endregion ICommunicatingDevice Commands

        #region IGenericRC5xx Commands

        protected virtual void InternalSetOutputSignal(SignalData signalData)
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetOutputSignal(new List<SignalData> { signalData }); },
                    GenericRC5xxCommand.SetOutputSignal);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected virtual void InternalSetDateAndTime()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetDateAndTime(); },
                    GenericRC5xxCommand.SetDateAndTime);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected virtual void InternalGetStatuses()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GetStatuses(); },
                    GenericRC5xxCommand.GetStatuses);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericRC5xx Commands

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public GenericRC5xxConfiguration Configuration
            => ConfigManager.Current.Cast<GenericRC5xxConfiguration>();

        /// <inheritdoc />
        public abstract string RelativeConfigurationDir { get; }

        /// <inheritdoc />
        public abstract void LoadConfiguration(string deviceConfigRootPath = "");

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        #endregion IConfigurableDevice

        #region Event Handlers

        private void Driver_StatusReceived(object sender, StatusEventArgs<IoStatus> args)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.StatusReceived)} event received. "
                + $"Source={args.SourceName}, Status={args.Status}.");
            OperationMode = args.Status.OperationMode;
            CommandProcessing = args.Status.CommandProcessing;
            IoModuleInError = args.Status.IoModuleInError.ToString("X2");
            SetOrClearAlarmByKey(args.Status.ErrorCode);
            ErrorCode = args.Status.ErrorCode.ToString("X2");
            UpdateErrorDescription(args.Status.IoModuleInError, args.Status.ErrorCode);
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = IsCommunicating = true;
            Task.Factory.StartNew(
                () =>
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.Initialization(); },
                        GenericRC5xxCommand.Initialization);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        GenericRC5xxCommand.GetStatuses);
                    SetState(OperatingModes.Idle);
                });
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        #endregion Event Handlers

        #region Abstract

        protected abstract GenericRC5xxDriver CreateDriver();

        protected abstract void UpdateErrorDescription(int partOfEfemInError, int errorCode);

        protected abstract void SetOrClearAlarmByKey(int statusErrorCode);

        #endregion Abstract

        #region Other Methods

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            DriverWrapper?.InterruptTask();
        }

        #endregion Other Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Driver != null)
                {
                    Driver.StatusReceived -= Driver_StatusReceived;
                    Driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                    Driver.CommunicationClosed -= Driver_CommunicationClosed;
                    Driver.CommunicationStarted -= Driver_CommunicationStarted;
                    Driver.CommunicationStopped -= Driver_CommunicationStopped;
                    Driver = null;
                }
            }
        }

        #endregion IDisposable
    }
}
