using System;
using System.Collections.Generic;
using System.Reflection;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver
{
    public class Dio2Driver : GenericRC5xxDriver
    {
        #region Fields

        private readonly IMacroCommandSubscriber _dio2SignalDataSubscriber;

        #endregion Fields

        #region Constructors

        public Dio2Driver(ILogger logger, ConnectionMode connectionMode, double aliveBitPeriod)
            : base(logger, connectionMode, 2, aliveBitPeriod)
        {
            _dio2SignalDataSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
        }

        #endregion Constructors

        #region Commands to Hardware

        public override void GetStatuses()
        {
            // Create commands
            var statCmd = StatusAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.IO, Port, Sender, this);
            var gdioCmd = GetDio2SignalCommand.NewOrder(Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetStatusesCompleted);
            macroCmd.AddMacroCommand(statCmd);
            macroCmd.AddMacroCommand(gdioCmd);

            // Send the command.
            CommandsSubscriber.AddMacro(macroCmd);
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
                base.CommandEndedCallback(evtId, evtResults);
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                // Command completion

                // Received events
                case EFEMEvents.ExpansionIOSignalReceived:
                    if (evtResults is not StatusEventArgs<SignalData> ioStatusEvtArgs
                        || ioStatusEvtArgs.Status is not Dio2SignalData dio2SignalData)
                    {
                        Logger.Warning(
                            $"{Name}. {MethodBase.GetCurrentMethod()?.Name} - {evtId} unexpected event arg type received: {evtResults}");
                        return;
                    }

                    OnDio2SignalDataReceived(
                        new StatusEventArgs<Dio2SignalData>(ioStatusEvtArgs.SourceName, dio2SignalData));
                    return;

                // Not managed by this driver
                default:
                    base.CommandEndedCallback(evtId, evtResults);
                    break;
            }
        }

        /// <summary>
        /// Enable Dio2 listeners
        /// </summary>
        protected override void EnableListeners()
        {
            base.EnableListeners();

            var ioSignalEvent = GetDio2SignalCommand.NewEvent(Port, Sender, this);
            _dio2SignalDataSubscriber.AddMacro(ioSignalEvent);
        }

        /// <summary>
        /// Flush Listeners
        /// </summary>
        protected override void DisableListeners()
        {
            base.DisableListeners();

            DiscardOpenTransactions(_dio2SignalDataSubscriber);
        }

        /// <inheritdoc />
        protected override void AddSpecificInitializationCommands(BaseMacroCommand genericInitializationMacroCommand)
        {
            var enableEvents = RC530EventCommand.NewOrder(
                Port,
                RC530EventTargetParameter.AllEvents,
                EventEnableParameter.Enable,
                Sender,
                this);
            genericInitializationMacroCommand.AddMacroCommand(enableEvents);

            // Create an empty signal data to have a signal resetting all output to 0.
            var outputSignal = new Dio2SignalData();
            var setOutputSignal = ChangeOutputSignalCommand.NewOrder(
                Port,
                new List<SignalData> { outputSignal },
                mustSendEquipmentEvent: false,
                Sender,
                this);

            genericInitializationMacroCommand.AddMacroCommand(setOutputSignal);
        }

        #endregion Overrides

        #region Events

        /// <summary>
        /// Occurs when GDIO status received from DIO2.
        /// </summary>
        public event EventHandler<StatusEventArgs<Dio2SignalData>> Dio2SignalDataReceived;

        /// <summary>
        /// Sends the <see cref="Dio2SignalDataReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{Dio2SignalData}"/> to be attached with the event.</param>
        protected virtual void OnDio2SignalDataReceived(StatusEventArgs<Dio2SignalData> args)
        {
            try
            {
                Dio2SignalDataReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion Events

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_dio2SignalDataSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
