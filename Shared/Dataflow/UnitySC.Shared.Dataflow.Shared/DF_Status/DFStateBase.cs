using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Dataflow.Shared.DF_Status
{
    public class DFStateData
    {
        // Values
        public ErrorID Error;
        public bool Initializing;
        public bool Maintenance_InError;
        public bool Executing;
    }

    public abstract class DFStateBase
    {
        private DFStateData _stateData;
        private TC_DataflowStatus _state = TC_DataflowStatus.None;
        protected DFStateData StateData { get => _stateData; set => _stateData = value; }

        public TC_DataflowStatus State { get => _state; set => _state = value; }

        public DFStateBase()
        {
            _stateData = new DFStateData();
            StateData.Error = ErrorID.Undefined;
            StateData.Initializing = false;
            StateData.Maintenance_InError = false;
            StateData.Executing = false;
        }

        public DFStateBase(DFStateData stateData)
        {
            _stateData = stateData;
        }
        public DFStateBase ChangeState_Maintenance_InError(ErrorID errorID)
        {
            _stateData.Maintenance_InError = true;
            StateData.Initializing = false;
            StateData.Executing = false;
            StateData.Error = errorID;
            return CheckNextState();
        }
        public DFStateBase ChangeState_Initializing()
        {
            StateData.Initializing = true;
            StateData.Error = ErrorID.Undefined;
            _stateData.Maintenance_InError = false;
            return CheckNextState();
        }


        public DFStateBase ChangeState_Idle()
        {
            StateData.Executing = false;
            StateData.Initializing = false;
            return CheckNextState();
        }

        public DFStateBase ChangeState_Executing()
        {
            StateData.Executing = true;
            return CheckNextState();
        }


        public DFStateBase CheckNextState()
        {
            DFStateBase previousState = this;
            DFStateBase nextState;
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


        public abstract DFStateBase NextState();

    }



}
