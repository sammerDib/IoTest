using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Agileo.Common.Communication;
using Agileo.Common.Communication.TCPIP;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common.Enums;

using Timer = System.Timers.Timer;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    /// <summary>
    /// Base class responsible to centralize behaviors common to drivers of any EFEM part.
    /// </summary>
    public abstract class DriverBase : IExtendedDeviceDriver, IDisposable, IEquipmentFacade
    {
        /// <summary>Provide the driver Name to be used.</summary>
        public string Name
            => string.Format(CultureInfo.InvariantCulture, "{0}{1}", Category, Port);

        public ConnectionMode ConnectionMode { get; }

        private readonly TCPPostman _postman;
        private readonly string _connectionId;

        private int _currentNbRetry;
        private int _maxNbRetry;
        private int _connectionRetryDelay;

        private readonly IMacroCommandSubscriber _aliveBitCommandsSubscriber;
        private readonly Timer _aliveBitTimer;

        public IMacroCommandSender Sender => _postman;

        public ILogger Logger { get; }

        #region Constructors

        /// <summary>
        /// Protected constructor to centralize some instantiation code for inherited classes.
        /// </summary>
        /// <param name="logger">The logger to use to trace any information</param>
        /// <param name="category">The kind of device (Aligner, Robot...).</param>
        /// <param name="connectionMode">
        /// Indicates which connection mode the driver must have (client or server).
        /// </param>
        /// <param name="port">Port's number of the device.</param>
        /// <param name="id">Contains the string ID of the connection</param>
        /// <param name="aliveBitPeriod">Contains alive bit request period</param>
        protected DriverBase(
            ILogger logger,
            string category,
            ConnectionMode connectionMode,
            byte port = 1,
            string id = null,
            double aliveBitPeriod = 10000)
        {
            Logger = logger;
            Category = category;
            Port = port;
            ConnectionMode = connectionMode;

            _connectionId = id + port;
            _postman = new TCPPostman(this, connectionMode == ConnectionMode.Client, AcceptConnect);
            _aliveBitCommandsSubscriber = AddReplySubscriber(SubscriberType.SenderAndListener);
            _aliveBitCommandsSubscriber.TraceActivation = false;

            if (connectionMode == ConnectionMode.Client)
            {
                _aliveBitTimer = new Timer();
                _aliveBitTimer.AutoReset = true;
                _aliveBitTimer.Interval = aliveBitPeriod;
                _aliveBitTimer.Elapsed += AliveTimer_Elapsed;
                _aliveBitTimer.Start();
            }

            Logger.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} connected to the Communication object",
                    Name));
        }

        public void Setup(
            System.Net.IPAddress ipAddress,
            uint tcpPort = 12000,
            uint answerTimeout = 30,
            uint confirmationTimeout = 10,
            uint initTimeout = 300,
            string commandPostfix = "\r",
            int maxNbRetry = -1,
            int connectionRetryDelay = 5000,
            List<string> endReplyIndicators = null)
        {
            if (_isSetupCalledOnce)
            {
                return;
            }

            _isSetupCalledOnce = true;
            _maxNbRetry = maxNbRetry;
            _connectionRetryDelay = connectionRetryDelay;

            if (endReplyIndicators != null)
            {
                foreach (var endReplyIndicator in endReplyIndicators)
                {
                    _postman.EndReplyIndicator = endReplyIndicator;
                }
            }
            else
            {
                _postman.EndReplyIndicatorBytes = Encoding.ASCII.GetBytes(commandPostfix);
            }

            _postman.NumberOfCharactersAfterEndReplyIndicator = 0;
            _postman.StopDecodingOnFirstEndReplyIndicator = true;
            _postman.PortIndex = (int)tcpPort;
            _postman.RemoteIPAddress = ipAddress;
            _postman.MaxReplyLength = 100;
            _postman.CommandPostfix = commandPostfix;

            _postman.CommunicationClosed += Postman_CommunicationClosed;
            _postman.CommunicationIsEstablished += Postman_CommunicationIsEstablished;

            // For client, define a connection timeout
            // For server, keep the default infinite timeout
            if (ConnectionMode == ConnectionMode.Client)
            {
                _postman.ConnectionTimeout = 1000;
            }

            // as postman is a base class defined in utilities we have to pass proper tracer
            // instead of instantiating them on the place where they are used
            _postman.SetTracer(Logger.Name);

            var build = new StringBuilder();
            build.AppendLine(
                string.Format(CultureInfo.InvariantCulture, "IP Address: {0}", ipAddress));
            build.AppendLine(string.Format(CultureInfo.InvariantCulture, "TCP Port: {0}", tcpPort));
            build.AppendLine("Time Outs:");
            build.AppendLine(
                string.Format(CultureInfo.InvariantCulture, "Init    Time Outs: {0}", initTimeout));
            build.AppendLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Answer  Time Outs: {0}",
                    answerTimeout));
            build.AppendLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Confirm Time Outs: {0}",
                    confirmationTimeout));
            Logger.Info("Communication - Setup Done: " + build);
        }

        #endregion Constructors

        /// <summary>Activate Listeners when the Communication is established</summary>
        protected abstract void EnableListeners();

        /// <summary>Deactivate Listeners when the Communication is closed</summary>
        protected abstract void DisableListeners();

        #region ICommunication

        /// <summary>Notifies that communication with the device is closed.</summary>
        public event EventHandler CommunicationClosed;

        /// <summary>Notifies that communication with the device is open.</summary>
        public event EventHandler CommunicationEstablished;

        /// <summary>
        /// Notifies that communication with the device is stopped. (i.e. communication will not be established
        /// until <see cref="EnableCommunications" /> is called.)
        /// </summary>
        public event EventHandler CommunicationStopped;

        /// <summary>
        /// Notifies that communication with the device is started. (i.e. driver is attempting to establish the
        /// communication. Depending on configuration and actual hardware state this may or may not succeed).
        /// </summary>
        public event EventHandler CommunicationStarted;

        /// <summary>
        /// Notifies that a message has been exchanged between the driver and the physical device.
        /// </summary>
        public event EventHandler<MessageExchangedEventArgs> MessageExchanged;

        /// <summary>
        /// Indicates if communication is already established (<see langword="true" />) or not (
        /// <see langword="false" />).
        /// </summary>
        public bool IsCommunicationEnabled => _postman?.IsConnected ?? false;

        private bool _isCommunicationStarted;

        /// <summary>
        /// Indicates if the driver is attempting to connect (client) or listening for connection (server).
        /// </summary>
        public bool IsCommunicationStarted
        {
            get => _isCommunicationStarted;
            private set
            {
                if (_isCommunicationStarted == value)
                {
                    return;
                }

                _isCommunicationStarted = value;
                if (_isCommunicationStarted)
                {
                    OnCommunicationStarted();
                }
                else
                {
                    OnCommunicationStopped();
                }
            }
        }

        /// <summary>Flush the queue holding commands to be sent to the device.</summary>
        /// <remarks>
        /// In case a command is in progress when this method is called, the command's completion will NOT be
        /// notified.
        /// </remarks>
        public virtual void ClearCommandsQueue()
        {
            //Do nothing in base class
        }

        /// <summary>Close communication with the device</summary>
        public void Disconnect()
        {
            IsCommunicationStarted = false;

            _postman.Disconnect();
            if (_postman.IsConnected)
            {
                Logger.Error("Communication - Failed to disconnect TCP communication.");
            }
        }

        /// <summary>Establish communication with the device</summary>
        /// <returns>
        /// <see langword="true" /> in case of success. <see langword="false" /> otherwise.
        /// </returns>
        public bool EnableCommunications()
        {
            if (_postman == null)
            {
                return false;
            }

            IsCommunicationStarted = true;

            if (_postman.IsConnected)
            {
                return true;
            }

            EnableListeners(); //Should be done before connecting TCP, so we can receive the first "ready" message from Rorze
            _postman.Connect();
            return false; //Ready should be awaited first
        }

        #region Raising Events

        /// <summary>Sends the <see cref="CommunicationClosed" /> event.</summary>
        protected virtual void OnCommunicationClosed()
        {
            if (CommunicationClosed != null)
            {
                CommunicationClosed(this, System.EventArgs.Empty);
            }
        }

        /// <summary>Sends the <see cref="CommunicationEstablished" /> event.</summary>
        protected virtual void OnCommunicationEstablished()
        {
            _currentNbRetry = 0;
            if (CommunicationEstablished != null)
            {
                CommunicationEstablished(this, System.EventArgs.Empty);
            }
        }

        /// <summary>Sends the <see cref="CommunicationStopped" /> event.</summary>
        protected virtual void OnCommunicationStopped()
        {
            CommunicationStopped?.Invoke(this, System.EventArgs.Empty);
        }

        /// <summary>Sends the <see cref="CommunicationStarted" /> event.</summary>
        protected virtual void OnCommunicationStarted()
        {
            CommunicationStarted?.Invoke(this, System.EventArgs.Empty);
        }

        /// <summary>Sends the <see cref="MessageExchanged" /> event.</summary>
        /// <param name="args">
        /// The <see cref="MessageExchangedEventArgs" /> to be attached with the event.
        /// </param>
        protected virtual void OnMessageExchanged(MessageExchangedEventArgs args)
        {
            try
            {
                var messageDirectionRepresentation = args.IsOut
                    ? "[SENT]"
                    : "[RECV]";
                var text =
                    $"{messageDirectionRepresentation} {args.Message.Replace("\r\n", "").Replace("\r", "")}";
                TraceManager.Instance()
                    .Trace("Com - " + $"{Category}{Port}", TraceLevelType.Info, text);

                if (MessageExchanged != null)
                {
                    MessageExchanged(this, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Raising Events

        #endregion ICommunication

        #region IDeviceDriver

        /// <summary>Notifies that a command ended.</summary>
        public event EventHandler<CommandEventArgs> CommandDone;

        /// <summary>Notifies when an error occurred while driving the device.</summary>
        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>The kind of device (Aligner, Robot...).</summary>
        /// <remarks>Should correspond to one field of <see cref="DrivenDevices" />.</remarks>
        public string Category { get; }

        /// <summary>
        /// Port's number of the device. Used to differentiate device(s) of same type in the tool, in case
        /// there are more than one.
        /// </summary>
        /// <remarks>By default, in case there is only one device, should be "1".</remarks>
        public byte Port { get; }

        /// <summary>
        /// Software equivalent to an EMO switch. Immediately stops any movement on the device. Leaves the
        /// device in an unknown state.
        /// </summary>
        public abstract void EmergencyStop();

        /// <summary>Get the list of all possible errors that could occur in this driver.</summary>
        /// <returns>A collection of <see cref="Error" /> applicable to the device.</returns>
        public ReadOnlyCollection<Error> GetPotentialErrors()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>To be used at software's start-up, after an abort command, ...</remarks>
        public abstract void Initialization();

        #region Raising Events

        /// <summary>Sends the <see cref="CommandDone" /> event.</summary>
        /// <param name="args">The <see cref="CommandEventArgs" /> to be attached with the event.</param>
        protected virtual void OnCommandDone(CommandEventArgs args)
        {
            try
            {
                var msg = new StringBuilder().AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Command {1} Done [{2}]",
                        Name,
                        args.Name,
                        args.Status));
                if (args.Error != null)
                {
                    msg.Append(args.Error);
                }

                Logger.Info(msg.ToString());

                if (CommandDone != null)
                {
                    CommandDone(this, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>Sends the <see cref="ErrorOccurred" /> event.</summary>
        /// <param name="args">
        /// The <see cref="ErrorOccurredEventArgs" /> to be attached with the event.
        /// </param>
        protected virtual void OnErrorOccurred(ErrorOccurredEventArgs args)
        {
            try
            {
                Logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Error Occurred on Command {1}: {2}",
                        Name,
                        args.CommandInError,
                        args.ToString()));
                if (ErrorOccurred != null)
                {
                    ErrorOccurred(this, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Raising Events

        #endregion IDeviceDriver

        #region IEquipmentFacade

        void IEquipmentFacade.RegisterAlarmist(string baseAlarmSource, int baseALID)
        {
            //Required by interface implementation.
        }

        void IEquipmentFacade.SendCommunicationLogEvent(
            int communicatorID,
            bool isOut,
            string message,
            DateTime dateTime)
        {
            //Informs the Owner component that a communication Log came on Communication pipe.
            OnMessageExchanged(new MessageExchangedEventArgs(isOut, message, dateTime));
        }

        void IEquipmentFacade.SendEquipmentEvent(int evtId, System.EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(CommandEvents), evtId))
            {
                CommandEndedCallback(evtId, evtResults);
                return;
            }

            switch ((CommandEvents)evtId)
            {
                case CommandEvents.CmdCompleteWithError:

                    if (evtResults is ErrorOccurredEventArgs eventArgs)
                    {
                        if (eventArgs.CommandInError == "TRANSREQ")
                        {
                            OnErrorOccurred(eventArgs);
                        }
                        else
                        {
                            OnCommandDone(
                                new CommandEventArgs(
                                    eventArgs.CommandInError,
                                    CommandStatusCode.Error,
                                    eventArgs.Error));
                        }
                    }
                    else
                    {
                        OnCommandDone(new CommandEventArgs(string.Empty, CommandStatusCode.Error));
                    }

                    // Purge the command in progress
                    ClearCommandsQueue();
                    break;

                case CommandEvents.CmdNotRecognized:
                    if (evtResults is ErrorOccurredEventArgs notRecognized)
                    {
                        OnCommandDone(
                            new CommandEventArgs(
                                notRecognized.CommandInError,
                                CommandStatusCode.CommandDoesNotExist,
                                notRecognized.Error));
                    }
                    else
                    {
                        OnCommandDone(
                            new CommandEventArgs(
                                string.Empty,
                                CommandStatusCode.CommandDoesNotExist));
                    }

                    // Purge the command in progress
                    ClearCommandsQueue();
                    break;

                default:
                    // Let overrides treat the event
                    CommandEndedCallback(evtId, evtResults);
                    break;
            }
        }

        void IEquipmentFacade.SendEquipmentAlarm(
            byte deviceNumber,
            bool isGetErrorStatus,
            string alarmKey,
            params object[] substitutionParam)
        {
            // Informs the Owner component that a communication Error came (Agileo internal layers Communication - communication protocol)

            // Trace the error
            var parameters = string.Format(
                CultureInfo.InvariantCulture,
                "[Parameters]\n"
                + $"deviceNumber={deviceNumber}\n"
                + $"isGetErrorStatus={isGetErrorStatus}\n"
                + $"alarmKey={alarmKey}\n"
                + $"substitutionParam={substitutionParam.Select(obj => obj.ToString())}");
            Logger.Debug(
                new TraceParam(parameters),
                $"{nameof(IEquipmentFacade.SendEquipmentAlarm)} called.");

            OnErrorOccurred(
                new ErrorOccurredEventArgs(
                    new Error
                    {
                        Code = "A_COMMUNICATION_ERROR",
                        Description = "TCP communication error.",
                        Cause = "Internal Communication Error",
                        Handling = "Contact the Service section of Agileo Automation.",
                        Sources = new List<string>()
                    }));
        }

        /// <summary>Called when a command is completed by the hardware.</summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">
        /// Contains the results of the command. To be cast in the appropriate type if command results are
        /// expected.
        /// </param>
        protected abstract void CommandEndedCallback(int evtId, System.EventArgs evtResults);

        #endregion IEquipmentFacade

        #region Postman Handling

        private void Postman_CommunicationIsEstablished(object sender, System.EventArgs e)
        {
            try
            {
                // Security in case of component in phase of clean-up
                if (_postman == null)
                {
                    return;
                }

                OnCommunicationEstablished();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void Postman_CommunicationClosed(object sender, System.EventArgs e)
        {
            try
            {
                // Security in case of component in phase of clean-up
                if (_postman == null)
                {
                    return;
                }

                ClearCommandsQueue();
                DisableListeners();

                if (ConnectionMode == ConnectionMode.Client && IsCommunicationStarted)
                {
                    // Only for client (server is always listening
                    // and when communication is started (so we don't try to reconnect if communication is closed due to a call to StopCommunication
                    Reconnect();
                }

                // Notify drivers about loss of communication
                OnCommunicationClosed();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>Clear the subscriber of macro command</summary>
        /// <param name="subscriberToClean">subscriber to remove</param>
        protected void DiscardOpenTransactions(IMacroCommandSubscriber subscriberToClean)
        {
            Logger.Debug(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} - DiscardOpenTransactions on subscriber of type '{1}'.",
                    Name,
                    subscriberToClean.Type));
            _postman.DiscardOpenTransactions(subscriberToClean);
        }

        protected IMacroCommandSubscriber AddReplySubscriber(SubscriberType type)
        {
            return _postman.AddReplySubscriber(type);
        }

        protected void RemoveReplySubscriber(IMacroCommandSubscriber subscriber)
        {
            _postman.RemoveReplySubscriber(subscriber);
        }

        private void AliveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsCommunicationEnabled)
            {
                return;
            }

            AliveBitRequest();
        }
        #endregion Postman Handling

        #region Alive bit

        protected abstract void AliveBitRequest();

        #endregion

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls
        private bool _isSetupCalledOnce;

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Used to clean managed resources to allow finalization and/or to clean unmanaged resources during
        /// finalization.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs the actual cleanup actions on managed/unmanaged resources.</summary>
        /// <param name="disposing">When <see Langword="true" />, managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                if (_aliveBitTimer == null)
                {
                    return;
                }

                _aliveBitTimer.Elapsed -= AliveTimer_Elapsed;
                _aliveBitTimer.Stop();
                _aliveBitTimer.Dispose();
            }
        }

        #endregion IDisposable Support

        /// <inheritdoc />
        public event EventHandler CommandInterrupted;

        protected void OnCommandInterrupted()
        {
            if (CommandInterrupted != null)
            {
                CommandInterrupted(this, System.EventArgs.Empty);
            }
        }

        private void Reconnect()
        {
            if (_currentNbRetry++ < _maxNbRetry || _maxNbRetry == -1)
            {
                IsCommunicationStarted = true;
                Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(_connectionRetryDelay);
                        Logger.Debug("Retry connection");
                        EnableCommunications();
                    });
            }
            else
            {
                IsCommunicationStarted = false;
                _currentNbRetry = 0;
            }
        }

        private bool AcceptConnect(byte[] receivedBytes)
        {
            var text = Encoding.ASCII.GetString(receivedBytes).Trim();
            var splitStrings = text.Split('.');
            var isAccepted = !_postman.IsConnected
                             && splitStrings.Length == 2
                             && splitStrings[0]
                                 .Substring(1)
                                 .Equals(_connectionId) // Removes first char ('e' for event)
                             && splitStrings[1].StartsWith("CNCT");

            if (isAccepted)
            {
                (this as IEquipmentFacade).SendCommunicationLogEvent(0, false, text, DateTime.Now);
            }

            return isAccepted;
        }
    }
}
