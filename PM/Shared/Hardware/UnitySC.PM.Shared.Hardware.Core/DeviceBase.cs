using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Core
{
    public abstract class DeviceBase : IDevice
    {
        protected IGlobalStatusServer GlobalStatusServer;
        protected ILogger Logger;

        public event Service.Interface.StateChangedEventHandler OnStatusChanged;

        public DeviceBase(IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            if (logger == null)
                throw new Exception("Invalid Logger (NULL)!");

            GlobalStatusServer = globalStatusServer;
            this.Logger = logger;
            State = new DeviceState(DeviceStatus.Unknown);
        }

        public string Name { get; set; }

        private DeviceState _state = new DeviceState(DeviceStatus.Unknown);

        public DeviceState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == null)
                    return;

                if (value.Status != _state.Status)
                {
                    MessageLevel level = value.Status == DeviceStatus.Error ? MessageLevel.Error : MessageLevel.Information;
                    var message = new Message(level, $"Status of {MessagSource} change from {_state.Status} to {value.Status}", value.StatusMessage, MessagSource);
                    GlobalStatusServer?.AddMessage(message);
                    LogMessageInHardwareLogger(message);
                    // We notify that the device status changed
                    if (OnStatusChanged != null)
                    {
                        OnStatusChanged.Invoke(this, new StatusChangedEventArgs(_state.Status, value.Status));
                    }
                    else if (value.Status == DeviceStatus.Error)
                    {
                        GlobalStatusServer?.SetGlobalState(PMGlobalStates.Error);
                    }
                }
                _state = value;
            }
        }

        public string DeviceID { get; set; }

        public abstract DeviceFamily Family { get; }

        public void AddMessage(Message message)
        {
            message.Source = MessagSource;
            GlobalStatusServer?.AddMessage(message);
            LogMessageInHardwareLogger(message);
            if (message.Level == MessageLevel.Fatal)
            {
                State = new DeviceState(DeviceStatus.Error, message.UserContent);
            }
        }

        private string MessagSource => string.Format("{0}-{1}", Family.ToString(), Name);

        private void LogMessageInHardwareLogger(Message message)
        {
            string text = string.Format("Message-{0}: {1} {2}", message.Level, message.UserContent, message.AdvancedContent);
            switch (message.Level)
            {
                case MessageLevel.None:
                    Logger.Debug(text);
                    break;

                case MessageLevel.Information:
                case MessageLevel.Question:
                case MessageLevel.Success:
                    Logger.Information(text);
                    break;

                case MessageLevel.Warning:
                    Logger.Warning(text);
                    break;

                case MessageLevel.Error:
                case MessageLevel.Fatal:
                    Logger.Error(text);
                    break;
            }
        }
    }
}
