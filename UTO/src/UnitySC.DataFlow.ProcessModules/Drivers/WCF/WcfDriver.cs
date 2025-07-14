using System;
using System.ServiceModel;
using System.Threading.Tasks;

using Agileo.Common.Logging;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataFlow.ProcessModules.Drivers.WCF
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public abstract class WcfDriver<T>
    {
        #region Fields

        protected readonly DuplexServiceInvoker<T> _serviceInvoker;

        protected readonly ILogger _logger;

        #endregion Fields

        #region Properties

        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            protected set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    if (value)
                    {
                        Task.Run(() => OnCommunicationEstablished());
                    }
                    else
                    {
                        Task.Run(() => OnCommunicationClosed());
                    }
                }
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler CommunicationEstablished;

        public event EventHandler CommunicationClosed;

        #endregion Events

        #region Constructor

        protected WcfDriver(WcfConfiguration configuration, ILogger logger)
        {
            _logger = logger;

            var instanceContext = new InstanceContext(this);

            _serviceInvoker = new DuplexServiceInvoker<T>(
                instanceContext,
                configuration.WcfServiceUriSegment,
                new NullLogger<T>(),
                customAddress: new ServiceAddress()
                {
                    Host = configuration.WcfHostIpAddressAsString,
                    Port = (int)configuration.WcfHostPort
                });
        }

        #endregion Constructor

        #region Abstract Methods

        public abstract bool Connect();

        public abstract void Disconnect();

        #endregion Abstract Methods

        #region Protected Methods

        protected void OnCommunicationEstablished()
        {
            CommunicationEstablished?.Invoke(this, EventArgs.Empty);
        }

        protected void OnCommunicationClosed()
        {
            CommunicationClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion Protected Methods
    }
}
