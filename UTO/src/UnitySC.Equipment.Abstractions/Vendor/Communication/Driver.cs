using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    public abstract class Driver : IEquipmentFacade, IDisposable, INotifyPropertyChanged
    {

        protected readonly ILogger Logger;

        protected Driver(string category, byte port)
        {
            var name = category + "Driver" + port;
            Logger = Agileo.Common.Logging.Logger.GetLogger(name);
        }

        public abstract bool IsCommunicationEnabled { get; }

        public void RegisterAlarmist(string baseAlarmSource, int baseALID)
        {
            throw new NotImplementedException();
        }

        public void SendCommunicationLogEvent(int communicatorID, bool isOut, string message, DateTime dateTime)
        {
            string direction = isOut ? "Send Data ..." : "Receive Data ...";
            if (!message.Contains("Failed to establish communication"))
            {
                Logger.Info(direction, message);
            }
        }

        public void SendEquipmentEvent(int eventID, EventArgs args)
        { }

        public virtual void SendEquipmentAlarm(byte deviceNumber, bool isGetErrorStatus, string alarmKey, params object[] substitutionParam)
        {
            //Do nothing
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public event EventHandler<CommandExecutionEventArgs> CommandDone;
        protected void OnCommandDone(CommandExecutionEventArgs args)
        {
            CommandDone?.Invoke(this, args);
        }


        public event EventHandler<CommandExecutionEventArgs> CommandFailed;
        protected void OnCommandFailed(CommandExecutionEventArgs args)
        {
            CommandFailed?.Invoke(this, args);
        }


        public event EventHandler<ErrorOccurredEventArgs> ErrorOccured;
        protected void OnErrorOccured(ErrorOccurredEventArgs args)
        {
            ErrorOccured?.Invoke(this, args);
        }


        public event EventHandler CommunicationEstablished;
        protected virtual void OnCommunicationEstablished(EventArgs eventArgs)
        {
            Logger.Info("Communication established");
            CommunicationEstablished?.Invoke(this, eventArgs);
        }

        public event EventHandler CommunicationClosed;
        protected virtual void OnCommunicationClosed(EventArgs eventArgs)
        {
            CommunicationClosed?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Call Connect instead.
        /// </summary>
        /// <returns></returns>
        public abstract bool EnableCommunications();

        public void Connect()
        {
            EnableCommunications();
        }

        public abstract void Disconnect();

        internal void SignalCommandDone(CommandExecutionEventArgs args)
        {
            OnCommandDone(args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Abort()
        {
            //Do nothing
        }

        public virtual void AckError()
        {
            //Do nothing
        }
    }
}
