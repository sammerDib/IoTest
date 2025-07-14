using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver
{
    /// <summary>Defines interactions with all RC500 series IO cards.</summary>
    public abstract class GenericRC5xxDriver : DriverBase
    {
        #region Fields

        protected readonly IMacroCommandSubscriber CommandsSubscriber;
        protected readonly IMacroCommandSubscriber ConnectedEventSubscriber;
        protected readonly IMacroCommandSubscriber StatusReceivedSubscriber;

        #endregion Fields

        #region Constructors

        protected GenericRC5xxDriver(ILogger logger, ConnectionMode connectionMode, byte port, double aliveBitPeriod)
            : base(logger, "IO", connectionMode, port, RorzeConstants.DeviceTypeAbb.IO, aliveBitPeriod)
        {
            CommandsSubscriber = AddReplySubscriber(SubscriberType.SenderAndListener);
            ConnectedEventSubscriber =
                AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            StatusReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForEverything);
        }

        #endregion Constructors

        #region Commands to Hardware

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>To be used at software's start-up, after an abort command, ...</remarks>
        public override void Initialization()
        {
            // Create commands
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.IO,
                Port,
                Sender,
                this,
                false);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.InitCompleted);
            macroCommand.AddMacroCommand(setTimeCmd);

            // Add specific parts of the initialization before sending
            AddSpecificInitializationCommands(macroCommand);

            // Send the command
            CommandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Called to initialize parts of the HW that could not be done generically. Method implementation only
        /// aims to add specific commands to the given macro command (not sending).
        /// </summary>
        protected abstract void AddSpecificInitializationCommands(
            BaseMacroCommand genericInitializationMacroCommand);

        public abstract void GetStatuses();

        public void SetOutputSignal(List<SignalData> signalDataList)
        {
            // Create commands
            var setOutputSignalCommand = ChangeOutputSignalCommand.NewOrder(
                Port,
                signalDataList,
                true,
                Sender,
                this);

            // Send the command
            CommandsSubscriber.AddMacro(setOutputSignalCommand);
        }

        public void SetDateAndTime()
        {
            // Create commands
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.IO,
                Port,
                Sender,
                this,
                true);

            // Send the command
            CommandsSubscriber.AddMacro(setTimeCmd);
        }

        public override void EmergencyStop()
        {
            ClearCommandsQueue();
        }

        #endregion Commands to Hardware

        #region Overrides

        /// <summary>Called when a command is completed by the hardware.</summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">
        /// Contains the results of the command. To be cast in the appropriate type if command results are
        /// expected.
        /// </param>
        protected override void CommandEndedCallback(int evtId, EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(EFEMEvents), evtId))
            {
                Logger.Warning(
                    $"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                // Command completion
                case EFEMEvents.InitCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(Initialization)));
                    break;

                case EFEMEvents.ChangeOutputSignalCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ChangeOutputSignalCommand)));
                    break;

                case EFEMEvents.GetStatusesCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetStatuses)));
                    break;

                case EFEMEvents.GetVersionCompleted:
                    break;

                // Received events
                case EFEMEvents.ConnectedReceived:
                    OnCommunicationEstablished();
                    break;

                case EFEMEvents.StatusReceived:
                    OnStatusReceived(evtResults as StatusEventArgs<IoStatus>);
                    break;

                // Not managed by this driver
                default:
                    Logger.Warning(
                        $"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                    break;
            }
        }

        /// <summary>Enable Aligner listeners</summary>
        protected override void EnableListeners()
        {
            Logger.Debug($"{Name} Listeners are Enabling");

            var connectedEvt = new ConnectionCommand(
                RorzeConstants.DeviceTypeAbb.IO,
                Port,
                Sender,
                this);
            ConnectedEventSubscriber.AddMacro(connectedEvt);

            var statusCmd = StatusAcquisitionCommand.NewEvent(
                RorzeConstants.DeviceTypeAbb.IO,
                Port,
                Sender,
                this);
            StatusReceivedSubscriber.AddMacro(statusCmd);

            Logger.Debug($"{Name} Listeners are Enabled");
        }

        /// <summary>Flush Listeners</summary>
        protected override void DisableListeners()
        {
            Logger.Debug($"{Name} Listeners are Disabling");

            DiscardOpenTransactions(ConnectedEventSubscriber);
            DiscardOpenTransactions(StatusReceivedSubscriber);

            Logger.Debug($"{Name} Listeners are Disabled");
        }

        /// <summary>Flush the queue holding commands to be sent to the device.</summary>
        /// <remarks>
        /// In case a command is in progress when this method is called, the command's completion will NOT be
        /// notified.
        /// </remarks>
        public override void ClearCommandsQueue()
        {
            Logger.Info($"Command {nameof(ClearCommandsQueue)} sent.");
            base.ClearCommandsQueue();
            DiscardOpenTransactions(CommandsSubscriber);
            OnCommandInterrupted();
        }

        protected override void AliveBitRequest()
        {
            // Create commands
            var acquisitionCommand = VersionAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.IO, Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetVersionCompleted);
            macroCmd.AddMacroCommand(acquisitionCommand);

            // Send the command.
            CommandsSubscriber.AddMacro(macroCmd);
        }
        #endregion Overrides

        #region Events

        /// <summary>Occurs when status received from RC5xx card.</summary>
        public event EventHandler<StatusEventArgs<IoStatus>> StatusReceived;

        /// <summary>Sends the <see cref="StatusReceived" /> event.</summary>
        /// <param name="args">
        /// The <see cref="StatusEventArgs{IOStatus}" /> to be attached with the event.
        /// </param>
        protected virtual void OnStatusReceived(StatusEventArgs<IoStatus> args)
        {
            try
            {
                StatusReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    $"Exception occurred during {nameof(StatusReceived)} event sending.");
            }
        }

        #endregion Events

        #region IDisposable

        /// <summary>Performs the actual cleanup actions on managed/unmanaged resources.</summary>
        /// <param name="disposing">When <see Langword="true" />, managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(CommandsSubscriber);
                RemoveReplySubscriber(ConnectedEventSubscriber);
                RemoveReplySubscriber(StatusReceivedSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
