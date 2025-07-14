using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.CompositeCommands;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.LoginCommands;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver
{
    public class CognexSubstrateIdReaderDriver : DriverBase
    {
        #region Fields

        private readonly IMacroCommandSubscriber _commandsSubscriber;
        private readonly IMacroCommandSubscriber _loginListenerSubscriber;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CognexSubstrateIdReaderDriver"/> class.
        /// </summary>
        /// <param name="logger">The logger to use to trace any information</param>
        /// <param name="connectionMode"></param>
        /// <param name="port">Port's number of the device.</param>
        /// <param name="id"></param>
        /// <param name="aliveBitPeriod">Contains alive bit request period</param>
        public CognexSubstrateIdReaderDriver(
            ILogger logger,
            ConnectionMode connectionMode = ConnectionMode.Server,
            byte port = 1,
            string id = null,
            double aliveBitPeriod = 1000)
            : base(logger, nameof(Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader), connectionMode, port, id, aliveBitPeriod)
        {
            _commandsSubscriber      = AddReplySubscriber(SubscriberType.SenderAndListener);
            _loginListenerSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
        }

        #endregion Constructor

        #region Commands to Hardware

        public void LoadJob(string jobName)
        {
            // Create command
            var goOffline = SwitchOnlineCommand.NewOrder(false, Sender, this);
            var loadJob   = LoadJobCommand.NewOrder(jobName, Sender, this);
            var goOnline  = SwitchOnlineCommand.NewOrder(true, Sender, this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, (int)CommandEvents.LoadJobCompleted);
            macroCommand.AddMacroCommand(goOffline);
            macroCommand.AddMacroCommand(loadJob);
            macroCommand.AddMacroCommand(goOnline);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void Read()
        {
            // Create command
            var readSubstrateId = ReadCommand.NewOrder(Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(readSubstrateId);
        }

        public void GetFileList()
        {
            // Create command
            var getFileListCmd = GetFileListCommand.NewOrder(Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(getFileListCmd);
        }

        public void GetImage(string imagePath)
        {
            // Create command
            var getImageCmd = GetImageCommand.NewOrder(imagePath, Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(getImageCmd);
        }

        private void TreatLoginRequest()
        {
            // Create command
            var commandUserEntering     = new UserEnteringCommand(Sender, this);
            var commandPasswordEntering = new PasswordEnteringCommand(Sender, this);

            // Create the Macro Command
            MacroLogin loginMacro = new MacroLogin(this);
            loginMacro.AddMacroCommand(commandUserEntering);
            loginMacro.AddMacroCommand(commandPasswordEntering);

            // Send the command
            _commandsSubscriber.AddMacro(loginMacro);
        }

        public override void EmergencyStop()
        {
            ClearCommandsQueue();
        }

        public override void Initialization()
        {
            OnCommandDone(new CommandEventArgs(nameof(Initialization)));
        }

        #endregion Commands to Hardware

        #region Overrides

        protected override void CommandEndedCallback(int evtId, System.EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(CommandEvents), evtId))
            {
                Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                return;
            }

            switch ((CommandEvents)evtId)
            {
                case CommandEvents.CognexLoginRequestReceived:
                    TreatLoginRequest();
                    break;

                case CommandEvents.CognexLoginCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(LoginRequestCommand)));
                    break;

                case CommandEvents.ReadSubstrateIdCompleted:
                    if (evtResults is SubstrateIdReceivedEventArgs substrateIdReceivedEventArgs)
                    {
                        OnCommandDone(new CommandEventArgs(nameof(ReadCommand)));
                        OnSubstrateIdReceived(substrateIdReceivedEventArgs);
                    }

                    break;

                case CommandEvents.GetFileListCompleted:
                    if (evtResults is FileNamesReceivedEventArgs fileNamesListReceivedEventArgs)
                    {
                        OnCommandDone(new CommandEventArgs(nameof(GetFileListCommand)));
                        OnFileNamesListReceived(fileNamesListReceivedEventArgs);
                    }

                    break;

                case CommandEvents.LoadJobCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(LoadJobCommand)));
                    break;

                case CommandEvents.GetImageCompleted:
                    if (evtResults is ImageReceivedEventArgs imageReceivedEventArgs)
                    {
                        OnCommandDone(new CommandEventArgs(nameof(GetImageCommand)));
                        SaveImage(imageReceivedEventArgs.FilePath, imageReceivedEventArgs.BitMapDataList);
                    }

                    break;

                // Not managed by this driver
                default:
                    Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                    break;
            }
        }

        protected override void EnableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabling", Name));

            // Create macro to listen for login request from Cognex
            var commandLoginRequest = new LoginRequestCommand(Sender, this);
            var macroLoginRequest   = new MacroLoginRequest(this);
            macroLoginRequest.AddMacroCommand(commandLoginRequest);
            _loginListenerSubscriber.AddMacro(macroLoginRequest);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabled", Name));
        }

        protected override void DisableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabling", Name));

            DiscardOpenTransactions(_loginListenerSubscriber);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabled", Name));
        }

        public override void ClearCommandsQueue()
        {
            base.ClearCommandsQueue();
            Logger.Info($"Command {nameof(ClearCommandsQueue)} sent.");
            DiscardOpenTransactions(_commandsSubscriber);
            OnCommandInterrupted();
        }

        protected override void AliveBitRequest()
        {
            //Do nothing
        }
        #endregion Overrides

        #region Events

        public event EventHandler<SubstrateIdReceivedEventArgs> SubstrateIdReceived;

        protected virtual void OnSubstrateIdReceived(SubstrateIdReceivedEventArgs args)
        {
            try
            {
                SubstrateIdReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(SubstrateIdReceived)} event sending.");
            }
        }

        public event EventHandler<FileNamesReceivedEventArgs> FileNamesListReceived;

        protected virtual void OnFileNamesListReceived(FileNamesReceivedEventArgs args)
        {
            try
            {
                FileNamesListReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Exception occurred during {nameof(FileNamesListReceived)} event sending.");
            }
        }

        #endregion Events

        #region Private methods

        private void SaveImage(string filePath, List<string> bitMapDataList)
        {
            var bitMapDataLines = new List<string>();

            foreach (var dataLine in bitMapDataList)
            {
                bitMapDataLines.Add(dataLine.Replace(CognexConstants.CognexCommandPostfix, String.Empty));
            }

            var bitmapDataLength = int.Parse(bitMapDataLines[0]) / 2;
            int indexEnd         = bitMapDataLines.Count - 2;
            byte[] bitmapData    = new byte[bitmapDataLength];

            int j = 0;
            for (int index = 1; index <= indexEnd; index++)
            {
                var line = bitMapDataLines[index];
                for (int i = 0; i < line.Length; i += 2)
                {
                    bitmapData[j++] = Convert.ToByte(line.Substring(i, 2), 16);
                }
            }

            File.WriteAllBytes(filePath, bitmapData);
        }

        #endregion Private methods

        #region IDisposable

        /// <summary>
        /// Performs the actual cleanup actions on managed/unmanaged resources.
        /// </summary>
        /// <param name="disposing">When <see Langword="true"/>, managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_commandsSubscriber);
                RemoveReplySubscriber(_loginListenerSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
