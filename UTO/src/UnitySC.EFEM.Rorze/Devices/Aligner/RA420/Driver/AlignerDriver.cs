using System;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver
{
    /// <summary>
    /// Class responsible to communicate with the RORZE Aligner model RA420.
    /// </summary>
    public class AlignerDriver : DriverBase
    {
        #region Fields

        private readonly IMacroCommandSubscriber _commandsSubscriber;
        private readonly IMacroCommandSubscriber _statusReceivedSubscriber;
        private readonly IMacroCommandSubscriber _gpioReceivedSubscriber;
        private readonly IMacroCommandSubscriber _substratePresenceReceivedSubscriber;
        private readonly IMacroCommandSubscriber _gposReceivedSubscriber;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignerDriver" /> class.
        /// </summary>
        /// <param name="logger">The logger to use to trace any information</param>
        /// <param name="connectionMode">Indicates whether the driver will be client or server.</param>
        /// <param name="port">Port's number of the device, in case there are more than one.</param>
        /// <param name="aliveBitPeriod">Contains alive bit request period</param>
        public AlignerDriver(
            ILogger logger,
            ConnectionMode connectionMode,
            byte port = 1,
            double aliveBitPeriod = 1000)
            : base(logger, nameof(Equipment.Abstractions.Devices.Aligner.Aligner), connectionMode, port, RorzeConstants.DeviceTypeAbb.Aligner, aliveBitPeriod)
        {
            _commandsSubscriber = AddReplySubscriber(SubscriberType.SenderAndListener);
            _statusReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForEverything);
            _gpioReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _substratePresenceReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _gposReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
        }

        #endregion Constructors

        #region Commands to Hardware

        /// <summary>
        /// Enable aligner events and reset error.
        /// </summary>
        public void InitializeCommunication()
        {
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitializeCommunicationCompleted);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Gets the aligner statuses.
        /// </summary>
        public void GetStatuses()
        {
            // Create commands
            var statCmd = StatusAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);
            var gpioCmd = GpioCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);
            var gwidCmd = GetSubstratePresenceCommand.NewOrder(Port, Sender, this);
            var gsizCmd = GetSubstrateSizeCommand.NewOrder(Port, Sender, this);
            var gposCmd = GposCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetStatusesCompleted);
            macroCmd.AddMacroCommand(statCmd);
            macroCmd.AddMacroCommand(gpioCmd);
            macroCmd.AddMacroCommand(gwidCmd);
            macroCmd.AddMacroCommand(gsizCmd);
            macroCmd.AddMacroCommand(gposCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCmd);
        }

        /// <summary>
        /// Aligns a substrate with the specified angle.
        /// </summary>
        /// <param name="alignAngle">Angle at which the substrate should be aligned.</param>
        public void Align(double alignAngle, AlignmentMode mode = AlignmentMode.SubstrateAlignment)
        {
            double convertedAngle;

            // Rorze specification defines 0.00 <= angle < 360.00
            if (alignAngle < 0 || alignAngle >= 360)
            {
                convertedAngle = CoerceAngle(alignAngle);
                Logger.Debug(FormattableString.Invariant(
                    $"Command {nameof(Align)} - Specified angle converted from {alignAngle} to {convertedAngle} degrees."));
            }
            else
            {
                convertedAngle = alignAngle;
            }

            // Create the command
            var alignCmd = AlignCommand.NewOrder(
                Port,
                Sender,
                this,
                mode,
                AlignmentValue.FromAngle(convertedAngle),
                AlignPostOperationActions.ShiftAfterAlignment);

            // Create the macro-command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.AlignCommandCompleted);
            macroCommand.AddMacroCommand(alignCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void SetDateAndTime()
        {
            // Create the command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                Sender,
                this,
                true);

            // Send the command
            _commandsSubscriber.AddMacro(setTimeCmd);
        }

        public void ChuckSubstrate(ChuckSubstrateZAxisMovement zAxisMovement)
        {
            // Create the command
            var chuckCmd = ChuckSubstrateCommand.NewOrder(Port, Sender, this, zAxisMovement);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.ChuckSubstrateCompleted);
            macroCommand.AddMacroCommand(chuckCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void CancelSubstrateChuck(ZAxisMovement zAxisMovement)
        {
            // Create the command
            var cancelChuckCmd = CancelSubstrateChuckCommand.NewOrder(Port, Sender, this, zAxisMovement);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.CancelSubstrateChuckCompleted);
            macroCommand.AddMacroCommand(cancelChuckCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void ResetError(ResetErrorParameter resetErrorType)
        {
            // Create the command
            var resetErrorCmd = ResetErrorCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Aligner, Port, resetErrorType,
                false, Sender, this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.ResetErrorCompleted);
            macroCommand.AddMacroCommand(resetErrorCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void SetSize(uint size)
        {
            // Create the command
            var setSubstrateSizeCommand = SetSubstrateSizeCommand.NewOrder(
                Port,
                (byte)size,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.SetSubstrateSizeCompleted);
            macroCommand.AddMacroCommand(setSubstrateSizeCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void GoHome()
        {
            var homeCmd = HomeCommand.NewOrder(
                Port,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.HomeCompleted);
            macroCommand.AddMacroCommand(homeCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Suspends EFEM operation axes.
        /// If a wafer is being transferred, the operation is stopped with vacuum chucking the wafer on the arm and retracting the arm.
        /// </summary>
        /// <remarks>
        /// This api will not imply a behavior similar to an EMO switch.
        /// Sends HOLD then ABORT messages
        /// </remarks>
        public override void EmergencyStop()
        {
            ClearCommandsQueue();

            var pauseCommand = PauseCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                Sender,
                this);

            var stopCommand = StopCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(
                this,
                (int)EFEMEvents.StopMotionCommandCompleted);
            macroCommand.AddMacroCommand(pauseCommand);
            macroCommand.AddMacroCommand(stopCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>To be used at software's start-up, after an abort command, ...</remarks>
        /// <remarks>Send INIT Rorze message</remarks>
        public void QuickInit()
        {
            // Create each individual command
            var resetError = ResetErrorCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                ResetErrorParameter.ResetAndStop,
                false,
                Sender,
                this);

            var chuckCommand = ChuckSubstrateCommand.NewOrder(
                Port,
                Sender,
                this,
                ChuckSubstrateZAxisMovement.MoveZAxisToVeryBottom,
                false);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(
                this,
                (int)EFEMEvents.InitCompleted);

            macroCommand.AddMacroCommand(resetError);
            macroCommand.AddMacroCommand(chuckCommand);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>To be used at software's start-up, after an abort command, ...</remarks>
        /// <remarks>Send INIT Rorze message</remarks>
        public override void Initialization()
        {
            // Create each individual command
            var resetError = ResetErrorCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                ResetErrorParameter.ResetAndStop,
                false,
                Sender,
                this);
            var initCmd = InitializeStatusCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                Sender,
                this);
            var originSearchCmd = OriginSearchCommand.NewAlignerOrder(
                Port,
                AlignerOriginSearchParameter.AlignInChuckedState,
                Sender,
                this);
            var chuckCommand = ChuckSubstrateCommand.NewOrder(
                Port,
                Sender,
                this,
                ChuckSubstrateZAxisMovement.MoveZAxisToVeryBottom,
                false);
            var resetError2 = ResetErrorCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                ResetErrorParameter.ResetAndStop,
                false,
                Sender,
                this);
            var setSubstrateSizeCommand = SetSubstrateSizeCommand.NewOrder(
                Port,
                1,
                Sender,
                this);
            var homeCmd = HomeCommand.NewOrder(
                Port,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitCompleted);

            macroCommand.AddMacroCommand(resetError);
            macroCommand.AddMacroCommand(initCmd);
            macroCommand.AddMacroCommand(originSearchCmd);
            macroCommand.AddMacroCommand(chuckCommand);
            macroCommand.AddMacroCommand(resetError2);
            macroCommand.AddMacroCommand(setSubstrateSizeCommand);
            macroCommand.AddMacroCommand(homeCmd);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        #endregion Commands to Hardware

        #region Overrides

        /// <summary>
        /// Called when a command is completed by the hardware.
        /// </summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">Contains the results of the command. To be cast in the appropriate type if command results are expected.</param>
        protected override void CommandEndedCallback(int evtId, EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(EFEMEvents), evtId))
            {
                Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                // Command completion
                case EFEMEvents.InitCompleted:
                    OnCommandDone(new CommandEventArgs(AlignerCommands.Initialization));
                    break;

                case EFEMEvents.InitializeCommunicationCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(InitializeCommunication)));
                    break;

                case EFEMEvents.GetStatusesCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetStatuses)));
                    break;

                case EFEMEvents.ChuckSubstrateCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ChuckSubstrateCommand)));
                    break;

                case EFEMEvents.CancelSubstrateChuckCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(CancelSubstrateChuckCommand)));
                    break;

                case EFEMEvents.AlignCommandCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(AlignCommand)));
                    break;

                case EFEMEvents.GetSubstratePresenceCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetSubstratePresenceCommand)));
                    break;

                case EFEMEvents.GetSubstrateSizeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetSubstrateSizeCommand)));
                    break;

                case EFEMEvents.ResetErrorCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ResetErrorCommand)));
                    break;

                case EFEMEvents.SetDateTimeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetDateAndTimeCommand)));
                    break;

                case EFEMEvents.SetSubstrateSizeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetSubstrateSizeCommand)));
                    break;

                case EFEMEvents.HomeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(HomeCommand)));
                    break;

                case EFEMEvents.StopMotionCommandCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(StopCommand)));
                    break;

                case EFEMEvents.GetVersionCompleted:
                    break;

                // Received events
                case EFEMEvents.StatusReceived:
                    OnStatusReceived(evtResults as StatusEventArgs<AlignerStatus>);
                    break;

                case EFEMEvents.GpioEventReceived:
                    OnGpioReceived(evtResults as StatusEventArgs<AlignerGpioStatus>);
                    break;

                case EFEMEvents.GposEventReceived:
                    OnGposReceived(evtResults as StatusEventArgs<AlignerGposStatus>);
                    break;

                case EFEMEvents.SubstratePresenceReceived:
                    OnSubstratePresenceReceived(evtResults as StatusEventArgs<AlignerSubstratePresenceStatus>);
                    break;

                case EFEMEvents.SubstrateSizeReceived:
                    OnSubstrateSizeReceived(evtResults as StatusEventArgs<AlignerSubstrateSizeStatus>);
                    break;

                // Not managed by this driver
                default:
                    Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                    break;
            }
        }

        /// <summary>
        /// Enable Aligner listeners
        /// </summary>
        protected override void EnableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabling", Name));

            var statusCmd = StatusAcquisitionCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);
            _statusReceivedSubscriber.AddMacro(statusCmd);

            var gpioCmd = GpioCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);
            _gpioReceivedSubscriber.AddMacro(gpioCmd);

            var substratePresenceCmd = GetSubstratePresenceCommand.NewEvent(Port, Sender, this);
            _substratePresenceReceivedSubscriber.AddMacro(substratePresenceCmd);

            var gposCmd = GposCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);
            _gposReceivedSubscriber.AddMacro(gposCmd);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabled", Name));
        }

        /// <summary>
        /// Flush Listeners
        /// </summary>
        protected override void DisableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabling", Name));

            DiscardOpenTransactions(_statusReceivedSubscriber);
            DiscardOpenTransactions(_gpioReceivedSubscriber);
            DiscardOpenTransactions(_substratePresenceReceivedSubscriber);
            DiscardOpenTransactions(_gposReceivedSubscriber);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabled", Name));
        }

        /// <summary>
        /// Flush the queue holding commands to be sent to the device.
        /// </summary>
        /// <remarks>In case a command is in progress when this method is called, the command's completion will NOT be notified.</remarks>
        public override void ClearCommandsQueue()
        {
            Logger.Info($"Command {nameof(ClearCommandsQueue)} sent.");
            base.ClearCommandsQueue();
            DiscardOpenTransactions(_commandsSubscriber);
            OnCommandInterrupted();
        }

        protected override void AliveBitRequest()
        {
            // Create commands
            var acquisitionCommand = VersionAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Aligner, Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetVersionCompleted);
            macroCmd.AddMacroCommand(acquisitionCommand);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCmd);
        }
        #endregion Overrides

        #region Events

        /// <summary>
        /// Occurs when status received from Aligner.
        /// </summary>
        public event EventHandler<StatusEventArgs<AlignerStatus>> StatusReceived;

        /// <summary>
        /// Sends the <see cref="StatusReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{AlignerStatus}"/> to be attached with the event.</param>
        protected virtual void OnStatusReceived(StatusEventArgs<AlignerStatus> args)
        {
            try
            {
                StatusReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(StatusReceived)} event sending.");
            }
        }

        /// <summary>
        /// Occurs when GPIO status received from Aligner.
        /// </summary>
        public event EventHandler<StatusEventArgs<AlignerGpioStatus>> GpioReceived;

        /// <summary>
        /// Sends the <see cref="GpioReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{AlignerGpioStatus}"/> to be attached with the event.</param>
        protected virtual void OnGpioReceived(StatusEventArgs<AlignerGpioStatus> args)
        {
            try
            {
                GpioReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(GpioReceived)} event sending.");
            }
        }

        /// <summary>
        /// Occurs when GPOS status received from Aligner.
        /// </summary>
        public event EventHandler<StatusEventArgs<AlignerGposStatus>> GposReceived;

        /// <summary>
        /// Sends the <see cref="GposReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{AlignerGposStatus}"/> to be attached with the event.</param>
        protected virtual void OnGposReceived(StatusEventArgs<AlignerGposStatus> args)
        {
            try
            {
                GposReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(GposReceived)} event sending.");
            }
        }

        /// <summary>
        /// Occurs when substrate presence received from Aligner.
        /// </summary>
        public event EventHandler<StatusEventArgs<AlignerSubstratePresenceStatus>> SubstratePresenceReceived;

        /// <summary>
        /// Sends the <see cref="SubstratePresenceReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{AlignerSubstratePresenceStatus}"/> to be attached with the event.</param>
        protected virtual void OnSubstratePresenceReceived(StatusEventArgs<AlignerSubstratePresenceStatus> args)
        {
            try
            {
                SubstratePresenceReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(SubstratePresenceReceived)} event sending.");
            }
        }

        /// <summary>
        /// Occurs when substrate size received from Aligner.
        /// </summary>
        public event EventHandler<StatusEventArgs<AlignerSubstrateSizeStatus>> SubstrateSizeReceived;

        /// <summary>
        /// Sends the <see cref="SubstrateSizeReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{AlignerSubstrateSizeStatus}"/> to be attached with the event.</param>
        protected virtual void OnSubstrateSizeReceived(StatusEventArgs<AlignerSubstrateSizeStatus> args)
        {
            try
            {
                SubstrateSizeReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(SubstrateSizeReceived)} event sending.");
            }
        }

        #endregion Events

        #region Helpers

        private BaseMacroCommand BuildInitMacroCommand(int eventToFacade)
        {
            // Create each individual command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                Sender,
                this,
                false);
            var disableEventCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                EventTargetParameter.AllEvents,
                EventEnableParameter.Disable,
                Sender,
                this);
            var enableStatusCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                EventTargetParameter.StatusEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableGpioCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                EventTargetParameter.PioEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableGposCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                EventTargetParameter.StoppingPositionEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableSubstratePresenceCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Aligner,
                Port,
                EventTargetParameter.SubstrateIdEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, eventToFacade);
            macroCommand.AddMacroCommand(setTimeCmd);
            macroCommand.AddMacroCommand(disableEventCmd);
            macroCommand.AddMacroCommand(enableStatusCmd);
            macroCommand.AddMacroCommand(enableGpioCmd);
            macroCommand.AddMacroCommand(enableGposCmd);
            macroCommand.AddMacroCommand(enableSubstratePresenceCmd);

            return macroCommand;
        }

        /// <summary>
        /// Converts the specified angle measure in degrees to one between 0 and 360 degrees.
        /// </summary>
        /// <param name="alignAngle">The align angle.</param>
        /// <returns>Value between 0 and 360 degrees</returns>
        private static double CoerceAngle(double alignAngle)
        {
            var normalizedAngle = alignAngle % 360;

            if (normalizedAngle < 0)
                normalizedAngle += 360;

            return normalizedAngle;
        }

        #endregion Helpers

        #region IDiposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_commandsSubscriber);
                RemoveReplySubscriber(_statusReceivedSubscriber);
                RemoveReplySubscriber(_gpioReceivedSubscriber);
                RemoveReplySubscriber(_substratePresenceReceivedSubscriber);
                RemoveReplySubscriber(_gposReceivedSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDiposable
    }
}
