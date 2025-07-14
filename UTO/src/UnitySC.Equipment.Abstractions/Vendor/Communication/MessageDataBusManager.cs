using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;

using Agileo.Common.Communication;
using Agileo.Common.Tracing;
using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    public class MessageDataBusManager
    {
        #region Constants
        private const string _tracerName = "MessageDataBusManager";
        #endregion Constants

        #region Fields
        private Timer _retryTimer;
        private readonly string _tagsConfigurationFilePath;
        #endregion Fields

        #region Properties
        public bool IsConfigured { get; private set; }
        public int RetryPeriod { get; }
        public Collection<GenericDriverParameters> DriversParameters { get; private set; } = new Collection<GenericDriverParameters>();
        #endregion Properties

        #region Constructor

        public MessageDataBusManager(string tagsConfigurationFilePath, int retryPeriod)
        {
            _tagsConfigurationFilePath = tagsConfigurationFilePath;
            RetryPeriod = retryPeriod;
        }
        #endregion Constructor

        #region Public Methods

        public void AddDriverParameters(GenericDriverParameters driverParameters)
        {
            DriversParameters.Add(driverParameters);
        }

        public void RegisterDrivers()
        {
            List<ErrorDescription> errors = MessageDataBus.Instance.RegisterDrivers(DriversParameters);
            if (errors.Count > 0)
            {
                throw new Exception($"MessageDataBus can't register drivers.\r\n{string.Join("\r\n", errors.Select(x => x.ErrorText).ToArray())}");
            }
        }

        public void Append()
        {
            List<ErrorDescription> mdbErrors = MessageDataBus.Instance.AppendFromFile(_tagsConfigurationFilePath);

            if (mdbErrors.Count != 0)
            {
                TraceManager.Instance().Trace(_tracerName, TraceLevelType.Warning,
                    string.Join(";", mdbErrors.Select(x => x.ErrorText).ToArray()));
                Retry();
            }
            else
            {
                OnConnect();
            }
        }

        public void Dispose()
        {
            IsConfigured = false;
            _retryTimer.Stop();
            _retryTimer.Elapsed -= RetryTimer_Elapsed;
            _retryTimer = null;
        }
        #endregion Public Methods

        #region Timer Elapsed
        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _retryTimer.Stop();
            _retryTimer.Elapsed -= RetryTimer_Elapsed;
            _retryTimer = null;
            Action action = Append;
            action.BeginInvoke(null, null);
        }
        #endregion Timer Elapsed

        #region Event

        public event EventHandler<EventArgs> Connected;

        private void OnConnect()
        {
            foreach (GenericDriverParameters driverParameter in MessageDataBus.Instance
                .DriverParameterCollection)
            {
                DataDrivers.Instance.Driver(driverParameter.Name).CommunicationClosed +=
                    MessageDataBusManager_CommunicationClosed;
            }
            IsConfigured = true;
            TraceManager.Instance()
                .Trace(_tracerName, TraceLevelType.Info, "MessageDataBus successfully configured.");
            Connected?.Invoke(null, new EventArgs());
        }

        private void MessageDataBusManager_CommunicationClosed(object sender, EventArgs e)
        {
            IsConfigured = false;
            foreach (GenericDriverParameters driverParameter in MessageDataBus.Instance
                .DriverParameterCollection)
            {
                DataDrivers.Instance.Driver(driverParameter.Name).CommunicationClosed -=
                    MessageDataBusManager_CommunicationClosed;
            }

            OnDisconnect();
            MessageDataBus.Instance.UnregisterDrivers();
            MessageDataBus.Dispose();
            RegisterDrivers();
            Retry();
        }

        public event EventHandler<EventArgs> Disconnected;
        private void OnDisconnect()
        {
            TraceManager.Instance().Trace(_tracerName, TraceLevelType.Info, "MessageDataBus disconnected. Retrying to connect.");
            Disconnected?.Invoke(null, new EventArgs());
        }
        #endregion Event

        #region Private Methods
        private void Retry()
        {
            _retryTimer = new Timer(RetryPeriod);
            _retryTimer.Elapsed += RetryTimer_Elapsed;
            _retryTimer.Start();
        }
        #endregion Private Methods
    }
}
