using System;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.TC.Shared.Data
{
    public class CommunicationOperations : ICommunicationOperations
    {
        private ICommunicationOperationsCB _communicationOprationsCB;
        private UnitySC.Shared.Data.Enum.ECommunicationState _communicationState;
        private EnableState _communicationControlSwitchState;
        private bool _interruptWaitingDelay = false;
        private ILogger _logger;
        private String _currentStateLogged;
        private String _communicationName;

        public CommunicationOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<ICommunicationOperations>>();
        }

        public void Init(String communicationName, ICommunicationOperationsCB cb)
        {
            _communicationOprationsCB = cb;
            _communicationName = communicationName;
        }

        #region ITCCommunicationControl

        public UnitySC.Shared.Data.Enum.ECommunicationState State
        {
            get => _communicationState;
            private set
            {
                if (_communicationState != value)
                {
                    _communicationState = value;
                    Transition_OnCommunicationSwitchStateChange();
                }
            }
        }

        public bool IsCommunicating
        {
            get { return ((_communicationState & UnitySC.Shared.Data.Enum.ECommunicationState.Communicating) == UnitySC.Shared.Data.Enum.ECommunicationState.Communicating); }
        }

        public EnableState SwitchState
        {
            get => _communicationControlSwitchState;
            set
            {
                if (_communicationControlSwitchState != value)
                {
                    // Communication Enabled/Disabled
                    _communicationControlSwitchState = value;
                    if (_communicationControlSwitchState == EnableState.Enabled)
                        LogStatus("Request communication starting");
                    else
                        LogStatus("Request communication stopped");
                    Transition_OnCommunicationSwitchStateChange();
                }
            }
        }

        #endregion ITCCommunicationControl

        private void LogStatus(String status)
        {
            if (_currentStateLogged != status)
            {
                _currentStateLogged = status;
                _logger.Information("[" + _communicationName + "] " + _currentStateLogged);
            }
        }

        #region ITCCommunicationControlCallback

        public void AttemptCommunicationFailedOrCommunicationLost()
        {
            if (IsCommunicating)//here test what was previous state of communication before this new communication state reception
            {
                LogStatus("Communication lost.");
                _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested;
                _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingPending;
            }
            _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating;
            _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.Communicating;
            Transition_OnCommunicationStateChanged();
        }

        public void AttemptCommunicationSucceed()
        {
            if (!IsCommunicating)//here test what was previous state of communication before this new communication state reception
            {
                LogStatus("Communication succeed");
                _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested;
                _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingPending;
            }
            _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating;
            _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.Communicating;

            Transition_OnCommunicationStateChanged();
        }

        #endregion ITCCommunicationControlCallback

        #region Private transitions on states changed

        private void Transition_OnCommunicationSwitchStateChange()
        {
            _interruptWaitingDelay = true;
            switch (_communicationControlSwitchState)
            {
                case EnableState.Enabled:
                    _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.Enabled;
                    _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating;
                    _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested;
                    _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.Communicating;
                    _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingPending;
                    break;

                case EnableState.Disabled:
                default:
                    _communicationState = 0; // Reset all - Communication disabled
                    break;
            }
            Transition_OnCommunicationStateChanged();
        }

        private void Transition_OnCommunicationStateChanged()
        {
            if ((_communicationState & UnitySC.Shared.Data.Enum.ECommunicationState.Enabled) == UnitySC.Shared.Data.Enum.ECommunicationState.Enabled)
            {
                // Communication enabled
                if (!IsCommunicating)
                {
                    if (((_communicationState & UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating) == UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating) &&
                        ((_communicationState & UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested) == UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested))
                    {
                        _communicationOprationsCB.CommunicationInterrupted();
                        // Deconnection detected
                        _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested;
                        _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingPending; // To be sure the both are not set in the same time
                        // Loop to check communicating again
                        StartLoopCheckingConnection(false);
                    }
                }
                else
                {
                    _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.NotCommunicating; // To be sure the both are not set in the same time
                    if ((_communicationState & UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested) == UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested)
                    {
                        _communicationOprationsCB.CommunicationEstablished();
                        _communicationState &= ~UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingRequested;
                        _communicationState |= UnitySC.Shared.Data.Enum.ECommunicationState.CommunicationCheckingPending; // To be sure the both are not set in the same time
                                                                                                                         // Loop to check still communicating
                        StartLoopCheckingConnection(true);
                    }
                }
            }
            else
            {
                // Communication disabled
                _communicationOprationsCB.CommunicationInterrupted();
            }
        }

        private void StartLoopCheckingConnection(bool shouldBeConnected)
        {
            Task.Run(() =>
            {
                bool stopConnectionChecking = false;
                while (!stopConnectionChecking)
                {
                    if (IsCommunicating == shouldBeConnected)
                    {
                        DoWaitingADelay();
                        _communicationOprationsCB.CommunicationCheck();
                    }
                    else
                        stopConnectionChecking = true;
                }
            });
        }

        public void DoWaitingADelay()
        {
            DateTime _startTime = DateTime.Now;
            bool _stop = false;
            _interruptWaitingDelay = false;
            do
            {
                Thread.Sleep(250);
                if (_interruptWaitingDelay)
                    break;
                _stop = DateTime.Now.Subtract(_startTime).TotalSeconds > 1;
            } while (!_stop);
        }

        #endregion Private transitions on states changed
    }
}
