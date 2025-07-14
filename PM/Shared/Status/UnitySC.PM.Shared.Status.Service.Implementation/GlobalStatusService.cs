using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Status.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GlobalStatusService : DuplexServiceBase<IGlobalStatusServiceCallback>, IGlobalStatusService, IGlobalStatusServer
    {
        private readonly object _lock = new object();
        private const int MaxNbMessagesHistory = 100;
        private List<Message> _messagesHistory = new List<Message>(MaxNbMessagesHistory);
        private IGlobalStatusServiceCallback _channelUsedToReserveHardware;
        private CancellationTokenSource _wcfReservecancellationToken;
        private bool _localHardwareReservation = false;
        private PMGlobalStates _currentServerState;
        private PMControlMode _currentControlMode; 

        public event GlobalStatusChangedEventHandler GlobalStatusChanged;

        public event ToolModeChangedEventHandler ToolModeChanged;

        public GlobalStatusService(ILogger<GlobalStatusService> logger) : base(logger, ExceptionType.HardwareException)
        {
            _wcfReservecancellationToken = new CancellationTokenSource();
        }

        #region IGlobalStatusService

        Response<VoidResult> IGlobalStatusService.SubscribeToGlobalStatusChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to global status change");
                    Subscribe();
                    // Return history message
                    messageContainer.AddRange(_messagesHistory.ToList());
                }
            });
        }

        Response<VoidResult> IGlobalStatusService.UnsubscribeToGlobalStatusChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to global status change");
                    Unsubscribe();
                }
            });
        }

        Response<PMGlobalStates> IGlobalStatusService.GetServerState()
        {
            return InvokeDataResponse(() =>
            {
                lock (_lock)
                {
                    return _currentServerState;
                }
            });
        }

        void IGlobalStatusServer.SetToolModeStatus(ToolMode toolMode)
        {
            ToolModeChanged?.Invoke(toolMode);
            InvokeCallback(globalStatusServiceCallback => globalStatusServiceCallback.ToolModeChangedCallback(toolMode));
        }

        Response<PMConfiguration> IGlobalStatusService.GetConfiguration()
        {
            return InvokeDataResponse(() =>
            {
                _logger.Debug("Get PP Configuration");
                return ClassLocator.Default.GetInstance<PMConfiguration>();
            });
        }

        Response<bool> IGlobalStatusService.ReserveHardware()
        {
            return InvokeDataResponse(messageContainer =>
            {
                _logger.Information("Reserve Hardware");
                lock (_lock)
                {
                    bool result = false;
                    if (_currentServerState == PMGlobalStates.Free)
                    {
                        _currentServerState = PMGlobalStates.Busy;
                        result = true;
                        SetWcfReservation();
                        NotifyChanges();
                    }
                    else if (_currentServerState == PMGlobalStates.Busy || _currentServerState == PMGlobalStates.Error)
                    {
                        // Check if the hardware is rerserved by the server
                        if (!_localHardwareReservation)
                        {
                            // Check if the hardware is reserve by a client
                            if (_channelUsedToReserveHardware == null || ((ICommunicationObject)_channelUsedToReserveHardware).State != CommunicationState.Opened)
                            {
                                result = true;
                                SetWcfReservation();
                            }
                        }
                    }
                    else if (_currentServerState == PMGlobalStates.ErrorHandling)
                    {
                        result = true;
                        SetWcfReservation();
                    }
                    if (!result)
                        messageContainer.Add(new Message(MessageLevel.Warning, "Hardware can't be reserve"));

                    return result;
                }
            });
        }

        private void SetWcfReservation()
        {
            _channelUsedToReserveHardware = OperationContext.Current.GetCallbackChannel<IGlobalStatusServiceCallback>();
            CheckWcfReservation();
        }

        private void ReleaseWcfReservation()
        {
            if (_channelUsedToReserveHardware != null)
            {
                _wcfReservecancellationToken.Cancel();
                _channelUsedToReserveHardware = null;
            }
        }

        private void CheckWcfReservation()
        {
            var token = _wcfReservecancellationToken.Token;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (_channelUsedToReserveHardware == null || ((ICommunicationObject)_channelUsedToReserveHardware).State != CommunicationState.Opened)
                    {
                        _channelUsedToReserveHardware = null;
                        if (_currentServerState == PMGlobalStates.Busy)
                        {
                            _currentServerState = PMGlobalStates.Free;
                            NotifyChanges();
                        }
                        break;
                    }
                    await Task.Delay(1000);
                }
            });
        }

        Response<bool> IGlobalStatusService.ReleaseHardware()
        {
            return InvokeDataResponse(messageContainer =>
            {
                _logger.Information("Release Hardware");
                lock (_lock)
                {
                    if (_channelUsedToReserveHardware == OperationContext.Current.GetCallbackChannel<IGlobalStatusServiceCallback>())
                    {
                        _channelUsedToReserveHardware = null;
                        if (_currentServerState == PMGlobalStates.Busy)
                        {
                            _currentServerState = PMGlobalStates.Free;
                            ReleaseWcfReservation();
                            NotifyChanges();
                        }

                        return true;
                    }
                    return false;
                }
            });
        }

        Response<bool> IGlobalStatusService.ResetHardware()
        {
            return InvokeDataResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Reset Hardware");

                    Task.Factory.StartNew(() =>
                    {
                        ClassLocator.Default.GetInstance<IHardwareManager>().Reset();
                    });
                    return true;
                }
            });
        }

        Response<VoidResult> IGlobalStatusService.ClearAllMessages()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _messagesHistory.Clear();
            });
        }

        Response<VoidResult> IGlobalStatusService.ClearMessage(Message messageToClear)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                var index=_messagesHistory.IndexOf(messageToClear);
                if (index != -1)
                    _messagesHistory.RemoveAt(index);
            });
        }

        #endregion IGlobalStatusService

        #region IGlobalStatusServer

        void IGlobalStatusServer.SetGlobalState(PMGlobalStates globalState)
        {
            lock (_lock)
            {
                SetState(globalState);
            }
        }

        bool IGlobalStatusServer.ReleaseLocalHardware()
        {
            lock (_lock)
            {
                _logger.Information("Release local Hardware");
                if (_localHardwareReservation)
                {
                    _localHardwareReservation = false; //TODO CRO : on inverse ce qui avait avant car etrange
                    if (_currentServerState == PMGlobalStates.Busy)
                    {
                        _currentServerState = PMGlobalStates.Free;
                        NotifyChanges();
                    }

                    return true;
                }
                return false;
            }
        }

        void IGlobalStatusServer.SetGlobalStatus(GlobalStatus globalStatus)
        {
            lock (_lock)
            {
                if (globalStatus.ControlModeSwitch != PMControlModeSwitch.NoSwitch)
                    _currentControlMode = (globalStatus.ControlModeSwitch == PMControlModeSwitch.SwitchToEngineering) ? PMControlMode.Engineering : PMControlMode.Production;
                
                if (globalStatus.CurrentState != null && globalStatus.CurrentState != _currentServerState)
                {
                    _currentServerState = (PMGlobalStates)globalStatus.CurrentState;
                    _logger.Information($"Set global status State:{globalStatus.CurrentState} Messages {globalStatus.Messages?.Count()}");
                }

                if (globalStatus.Messages != null)
                    AddMessageToHistory(globalStatus.Messages);

                NotifyChanges(globalStatus.Messages);
            }
        }

        bool IGlobalStatusServer.ReserveLocalHardware()
        {
            lock (_lock)
            {
                _logger.Information("Reserve local Hardware");
                bool result = false;
                if (_currentServerState == PMGlobalStates.Free)
                {
                    _currentServerState = PMGlobalStates.Busy;
                    _localHardwareReservation = true;
                    result = true;
                    NotifyChanges();
                }
                else if (_currentServerState == PMGlobalStates.Busy)
                {
                    // Check if the hardware is rerserved by the server
                    if (!_localHardwareReservation)
                    {
                        // Check if the hardware is reserve by a client
                        if (_channelUsedToReserveHardware == null || ((ICommunicationObject)_channelUsedToReserveHardware).State != CommunicationState.Opened)
                        {
                            _localHardwareReservation = true;
                            result = true;
                        }
                    }
                }
                return result;
            }
        }

 
        void IGlobalStatusServer.AddMessage(Message message)
        {
            var messages = new List<Message>() { message };
            AddMessageToHistory(messages);
            NotifyChanges(messages);
        }

        #endregion IGlobalStatusServer

        private void AddMessageToHistory(List<Message> messages)
        {
            foreach (var newMessage in messages.Where(x => x.Level >= MessageLevel.Warning))
            {
                if (messages.Count >= MaxNbMessagesHistory)
                {
                    _messagesHistory.RemoveAt(0);
                }
                _messagesHistory.Add(newMessage);
            }
        }

        private void NotifyChanges(List<Message> messages = null)
        {
            GlobalStatus globalStatus;
            if (messages != null)
                globalStatus = new GlobalStatus(_currentServerState, messages);
            else
                globalStatus = new GlobalStatus(_currentServerState);
            // Notify globalStatus changed to all client
            InvokeCallback(globalStatusServiceCallback => globalStatusServiceCallback.GlobalStatusChangedCallback(globalStatus));
            // Notify globalStatus changed to all internal subscribers of event
            if (GlobalStatusChanged != null)
                GlobalStatusChanged.Invoke(globalStatus);      
        }

        public void SetState(PMGlobalStates newGlobalState)
        {
            _logger.Information($"Set global State:{newGlobalState}");
            if (_currentServerState != newGlobalState)
            {
                if (_currentServerState == PMGlobalStates.Busy)
                {
                    _localHardwareReservation = false;
                    ReleaseWcfReservation();
                }

                MessageLevel level = newGlobalState == PMGlobalStates.Error ? MessageLevel.Error : MessageLevel.Information;
                var message = new Message(level, string.Format("State of Process Module change from {0} to {1}", _currentServerState, newGlobalState));
            }
            _currentServerState = newGlobalState;
            NotifyChanges();
        }

        public PMGlobalStates GetGlobalState()
        {
            return _currentServerState;
        }

        public void SetControlMode(PMControlMode controlMode)
        {
            _currentControlMode = controlMode;
        }

        public PMControlMode GetControlMode()
        {
            return _currentControlMode;
        }

  
    }
}
