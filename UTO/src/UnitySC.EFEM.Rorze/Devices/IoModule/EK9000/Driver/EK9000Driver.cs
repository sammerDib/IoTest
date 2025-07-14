using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Communication.Modbus;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Driver.EventArgs;
using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Driver
{
    public class EK9000Driver : IDisposable
    {
        private const string Id = nameof(EK9000Driver);

        private ILogger Logger { get; } = Agileo.Common.Logging.Logger.GetLogger(nameof(EK9000Driver));

        private readonly ModbusConfiguration _configuration;

        private int _currentNbRetry;
        private readonly int _maxNbRetry;

        #region Constructor

        public EK9000Driver(ModbusConfiguration configuration)
        {
            _configuration = configuration;
            _maxNbRetry = _configuration.ConnectionRetryNumber;
        }

        #endregion Constructor

        #region Communication

        public void EnableCommunications()
        {
            IsCommunicationStarted = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!RegisterMdb())
                    {
                        Reconnect();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        public void Disconnect()
        {
            IsCommunicationStarted = false;
            Dispose();
        }

        /// <summary>
        /// Indicates if communication is already established (<see langword="true"/>) or not (<see langword="false"/>).
        /// </summary>
        public bool IsCommunicationEnabled => DataDrivers.Instance.Driver(Id)?.IsCommunicationEnabled ?? false;

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

        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        private void OnConnectionStateChanged(bool connected)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs(connected));
        }

        /// <summary>
        /// Notifies that communication with the device is started.
        /// (i.e. driver is attempting to establish the communication. Depending on configuration and actual hardware state this may or may not succeed).
        /// </summary>
        public event EventHandler CommunicationStarted;

        /// <summary>
        /// Sends the <see cref="CommunicationStarted"/> event.
        /// </summary>
        private void OnCommunicationStarted()
        {
            CommunicationStarted?.Invoke(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Notifies that communication with the device is stopped.
        /// (i.e. communication will not be established until <see cref="EnableCommunications"/> is called.)
        /// </summary>
        public event EventHandler CommunicationStopped;

        /// <summary>
        /// Sends the <see cref="CommunicationStopped"/> event.
        /// </summary>
        private void OnCommunicationStopped()
        {
            CommunicationStopped?.Invoke(this, System.EventArgs.Empty);
        }

        #endregion Communication

        #region Tags

        public void SetOutputSignal<T>(string outputName, T value)
        {
            MessageDataBus.Instance
                .GetTagByName<T>("Outputs", outputName)
                .Value = value;
        }

        public event EventHandler<DrivenTagValueChangedEventArgs> TagValueChanged;

        private void OnTagValueChanged(object sender, DrivenTagValueChangedEventArgs e)
        {
            TagValueChanged?.Invoke(this, new DrivenTagValueChangedEventArgs(e));
        }

        #endregion Tags

        #region Event Handlers

        private void CommunicationClosed(object sender, System.EventArgs e)
        {
            OnConnectionStateChanged(false);
        }

        private void CommunicationEstablished(object sender, System.EventArgs e)
        {
            _currentNbRetry = 0;
            OnConnectionStateChanged(true);
        }

        private void Driver_CommunicationClosed(object sender, System.EventArgs e)
        {
            OnConnectionStateChanged(false);
            if (IsCommunicationStarted)
            {
                // Only when communication is started, so we don't try to reconnect if communication is closed due to a call to StopCommunication
                Reconnect();
            }
        }

        #endregion Event Handlers

        #region Private

        private bool RegisterMdb()
        {
            Logger.Info("Start register MDB");

            _isDisposed = false;
            var errors = new List<ErrorDescription>();

            var thread = new Thread(() =>
            {
                var drivers = new Collection<GenericDriverParameters>
                {
                    new ModbusDriverConfig(
                        Id,
                        "Agileo.Common.Communication",
                        "Agileo.Common.Communication.Modbus.ModbusDriver",
                        _configuration.IpAddress,
                        _configuration.TcpPort,
                        _configuration.PollingPeriodInterval,
                        _configuration.IsSimulated,
                        _configuration.MaxSpaceBetweenWordsInRange
                    )
                };

                errors = MessageDataBus.Instance.RegisterDrivers(drivers);
                if (errors.Count != 0)
                {
                    // Errors already logged by MDB and below in this method
                    return;
                }

                errors = MessageDataBus.Instance.AppendFromFile(_configuration.TagsConfigurationPath);
                if (errors.Count != 0)
                {
                    // Errors already logged by MDB and below in this method
                    return;
                }

                foreach (var eg in MessageDataBus.Instance.Groups.Values.OfType<ExternalGroup>())
                {
                    eg.CommunicationClosed      += CommunicationClosed;
                    eg.CommunicationEstablished += CommunicationEstablished;
                    eg.OnTagValueChanged        += OnTagValueChanged;
                }
            });
            thread.Start();

            if (!thread.Join(_configuration.ConnectionTimeout))
            {
                thread.Abort();
                Logger.Error("Failed to register MDB due to connection timeout");
                Dispose();
                return false;
            }

            if (errors.Count != 0)
            {
                Logger.Error(
                    new TraceParam(string.Join(Environment.NewLine, errors.Select(e => e.ToString()))),
                    "Failed to register MDB");
                Dispose();
                return false;
            }

            Logger.Info("Success to register MDB");
            var driver = DataDrivers.Instance.Driver(Id);
            driver.CommunicationClosed += Driver_CommunicationClosed;

            //ForceValueChanged is called to ensure that all statuses are updated
            MessageDataBus.Instance.ForceValueChanged();
            return true;
        }

        private void Reconnect()
        {
            if (_currentNbRetry++ < _maxNbRetry || _maxNbRetry == -1)
            {
                IsCommunicationStarted = true;
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(_configuration.ConnectionRetryDelay);
                    Logger.Debug("Retry connection");
                    Dispose(); // MDB needs to be properly disposed and recreated for reconnection to work
                    EnableCommunications();
                });
            }
            else
            {
                IsCommunicationStarted = false;
                _currentNbRetry = 0;
            }
        }

        #endregion Private

        #region IDisposable

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            // Unregister to events
            foreach (var eg in MessageDataBus.Instance.Groups.Values.OfType<ExternalGroup>())
            {
                eg.CommunicationClosed      -= CommunicationClosed;
                eg.CommunicationEstablished -= CommunicationEstablished;
                eg.OnTagValueChanged        -= OnTagValueChanged;
            }

            var driver = DataDrivers.Instance.Driver(Id);
            if (driver != null)
            {
                driver.CommunicationClosed -= Driver_CommunicationClosed;
            }

            // Communication is closed, ensure we reflect that state
            OnConnectionStateChanged(false);

            MessageDataBus.Dispose();
        }

        #endregion IDisposable
    }
}
