using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.Shared.UI.Proxy
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class GlobalStatusSupervisor : ObservableObject, IGlobalStatusService, IGlobalStatusServiceCallback
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private ActorType _actorType;
        private bool _addActorInMessage;

        public delegate void OnNewMessageEvent(Message message);

        public event OnNewMessageEvent OnNewMessage;

        public delegate void OnStateChangedEvent(PMGlobalStates state);

        public delegate void OnStateToolModeChangedEvent(ToolMode toolMode);

        public event OnStateChangedEvent OnStateChanged;

        public event OnStateToolModeChangedEvent OnStateToolModeChanged;

        private PMGlobalStates? _currentState;

        public PMGlobalStates? CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;

                    if (OnStateChanged != null)
                        OnStateChanged.Invoke(_currentState.Value);
                }
            }
        }

        public PMConfiguration Configuration { get; private set; }

        protected void RaiseErrorEvent(Message message)
        {
            OnNewMessage?.Invoke(message);
        }

        private DuplexServiceInvoker<IGlobalStatusService> _globalStatusService;

        /// <summary>
        /// Constructor
        /// </summary>
        public GlobalStatusSupervisor(ActorType actorType, bool addActorInMessage, ILogger<GlobalStatusSupervisor> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _actorType = actorType;
            _addActorInMessage = addActorInMessage;
        }

        public void Init()
        {
            var instanceContext = new InstanceContext(this);
            _globalStatusService = new DuplexServiceInvoker<IGlobalStatusService>(instanceContext,
                _actorType + "GlobalStatus", ClassLocator.Default.GetInstance<SerilogLogger<IGlobalStatusService>>(),
                _messenger, s => s.SubscribeToGlobalStatusChanges(),
                ClientConfiguration.GetServiceAddress(_actorType));

            // Subscribe to changes
            CurrentState = _globalStatusService.Invoke(s => s.GetServerState());
            GetConfiguration();
        }

        public void SendUIMessage(Message message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                _messenger.Send(message);
            });
        }

        public void GlobalStatusChangedCallback(GlobalStatus globalStatus)
        {
            CurrentState = globalStatus.CurrentState;
            if (globalStatus.Messages != null)
            {
                foreach (var message in globalStatus.Messages)
                {
                    if (_addActorInMessage)
                        message.Source = string.IsNullOrEmpty(message.Source) ? _actorType.ToString() : string.Format("({0}) {1}", _actorType, message.Source);

                    RaiseErrorEvent(message);

                    // System.Windows.Application.Current is null for the unit tests
                    if (System.Windows.Application.Current == null)
                        Dispatcher.CurrentDispatcher.Invoke(() => _messenger.Send(message));
                    else
                        System.Windows.Application.Current.Dispatcher.Invoke(() => _messenger.Send(message));
                }
            }
        }

        public Response<VoidResult> SubscribeToGlobalStatusChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _globalStatusService.TryInvokeAndGetMessages(s => s.SubscribeToGlobalStatusChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Global status subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnsubscribeToGlobalStatusChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _globalStatusService.TryInvokeAndGetMessages(s => s.UnsubscribeToGlobalStatusChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Global status unsubscribe error");
            }

            return resp;
        }

        public async Task<bool> WaitHardwareInitializationDone(int waitingTime = 60)
        {
            int nbTrys = 0;
            int maxNbTrys = waitingTime * 10;
            while (nbTrys < maxNbTrys)
            {
                var serverState = GetServerState()?.Result;
                if (serverState == PMGlobalStates.Busy || serverState == PMGlobalStates.Free)
                {
                    Console.WriteLine("Server is ready");
                    break;
                }

                await Task.Delay(100);
                nbTrys++;
            }

            if (nbTrys == maxNbTrys)
                return false;

            return true;
        }

        public async Task<bool> AsyncWaitServerIsReady()
        {
            if ((CurrentState != null) && (CurrentState == PMGlobalStates.Free))
                return true;

            do
            {
                await Task.Delay(1000);
            }
            while ((CurrentState == null) || (CurrentState != PMGlobalStates.Free));
            //Console.WriteLine("WaitServerIsReady Ready");
            if (CurrentState == PMGlobalStates.Error || CurrentState == PMGlobalStates.ErrorHandling)
            {
                return false;
            }

            return true;
        }

        public bool IsChannelOpened()
        {
            if (_globalStatusService is null)
                return false;
            return _globalStatusService.IsChannelOpened();
        }

        public Response<PMGlobalStates> GetServerState()
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.GetServerState());
        }

        public Response<PMConfiguration> GetConfiguration()
        {
            var res = _globalStatusService.TryInvokeAndGetMessages(s => s.GetConfiguration());
            Configuration = res.Result;
            return res;
        }

        public Response<bool> ReserveHardware()
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.ReserveHardware());
        }

        public Response<bool> ReleaseHardware()
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.ReleaseHardware());
        }

        public Response<bool> ResetHardware()
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.ResetHardware());
        }
        public Response<VoidResult> ClearAllMessages()
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.ClearAllMessages());
        }

        public Response<VoidResult> ClearMessage(Message messageToClear)
        {
            return _globalStatusService.TryInvokeAndGetMessages(s => s.ClearMessage(messageToClear));
        }

        public void ToolModeChangedCallback(ToolMode toolMode)
        {
            if (OnStateToolModeChanged !=null)
                OnStateToolModeChanged(toolMode);
        }

        private AutoRelayCommand _resetCommand;

        public AutoRelayCommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new AutoRelayCommand(
              () =>
              {
                  ResetHardware();
              },
              () => { return true; }));
            }
        }
    }
}
