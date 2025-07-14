using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data.TC_PMStates
{
    public class StateData
    {
        // Values
        public ErrorID Error;
        public bool Suspended;
        public bool ProcessPreparing;
        public bool Processing;
        public bool Initialized;
        public bool Communicating;
        public bool Online1_Offline0;
    }

    public abstract class BaseTCPMState
    {
        private StateData _stateData;
        private TC_PMState _state = TC_PMState.Unknown;
        protected StateData StateData { get => _stateData; set => _stateData = value; }

        public TC_PMState State { get => _state; set => _state = value; }

        public BaseTCPMState()
        {
            _stateData = new StateData();
            StateData.Communicating = false;
            StateData.Error = ErrorID.Undefined;
            StateData.Initialized = false;
            StateData.Online1_Offline0 = false;
            StateData.Processing = false;
            StateData.ProcessPreparing = false;
            StateData.Suspended = false;
        }

        public BaseTCPMState(StateData stateData)
        {
            _stateData = stateData;
        }
        public BaseTCPMState ChangeState_SetNotInitialized()
        {
            _stateData.Initialized = false;
            return CheckNextState();
        }
        public BaseTCPMState ChangeState_InitializationFinished()
        {
            _stateData.Initialized = true;
            _stateData.Error = ErrorID.Undefined;
            StateData.Processing = false;
            StateData.ProcessPreparing = false;
            StateData.Suspended = false;
            return CheckNextState();
        }


        public BaseTCPMState ChangeState_TCCommunicating(bool connected)
        {
            _stateData.Communicating = connected;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_SetPMOffline()
        {
            _stateData.Online1_Offline0 = false;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_SetPMOnline()
        {
            _stateData.Online1_Offline0 = true;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_PrepareProcess()
        {
            _stateData.Processing = false;
            _stateData.ProcessPreparing = true;
            return CheckNextState();
        }
        public BaseTCPMState ChangeState_Processing()
        {
            _stateData.ProcessPreparing = false;
            _stateData.Processing = true;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_ProcessFinisihed()
        {
            _stateData.ProcessPreparing = false;
            _stateData.Processing = false;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_PauseProcess()
        {
            _stateData.Processing = false;
            _stateData.Suspended = true;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_ResumeProcess()
        {
            _stateData.Processing = true;
            _stateData.Suspended = false;
            return CheckNextState();
        }

        public BaseTCPMState ChangeState_OnError(ErrorID error)
        {
            _stateData.ProcessPreparing = false;
            _stateData.Processing = false;
            _stateData.Suspended = false;

            _stateData.Error = error;
            return CheckNextState();
        }
        public BaseTCPMState ChangeState_OnErrorCleared()
        {
            _stateData.Error = ErrorID.Undefined;
            return CheckNextState();
        }

        public BaseTCPMState CheckNextState()
        {
            BaseTCPMState previousState = this;
            BaseTCPMState nextState;
            bool found = false;
            do
            {
                nextState = previousState.NextState();
                found = (nextState.State == previousState.State);
                if (!found)
                    previousState = nextState;

            } while (!found);
            return nextState;
        }


        public abstract BaseTCPMState NextState();

    }



}
