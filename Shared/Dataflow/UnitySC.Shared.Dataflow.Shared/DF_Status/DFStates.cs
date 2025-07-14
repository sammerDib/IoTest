using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Dataflow.Shared.DF_Status
{
    public class DFNoneState : DFStateBase
    {
        public DFNoneState()
               : base()
        {
            State = TC_DataflowStatus.None;
        }
        public DFNoneState(DFStateData stateData)
            : base(stateData)
        {
            State = TC_DataflowStatus.None;
        }

        public override DFStateBase NextState()
        {
            return new DFMaintenance(StateData);
        }
    }

    public class DFMaintenance : DFStateBase
    {

        public DFMaintenance()
            : base()
        {

        }
        public DFMaintenance(DFStateData stateData)
            : base(stateData)
        {
            State = TC_DataflowStatus.Maintenance;
        }

        public override DFStateBase NextState()
        {
            if (StateData.Initializing)
                return new DFInitializingState(StateData);
            else
                return this;
        }
    }

    public class DFInitializingState : DFStateBase
    {
        public DFInitializingState()
               : base()
        {
            State = TC_DataflowStatus.Initializing;
            StateData.Error = ErrorID.Undefined;
            StateData.Maintenance_InError = false;
            StateData.Executing = false;
        }
        public DFInitializingState(DFStateData stateData)
            : base(stateData)
        {
            State = TC_DataflowStatus.Initializing;
            StateData.Error = ErrorID.Undefined;
            StateData.Maintenance_InError = false;
            StateData.Executing = false;
        }

        public override DFStateBase NextState()
        {
            if ((StateData.Error != ErrorID.Undefined) || StateData.Maintenance_InError)
                return new DFMaintenance(StateData);
            else
            if (!StateData.Initializing)
                return new DFIdleState(StateData);
            else
                return this;
        }
    }
    public class DFIdleState : DFStateBase
    {
        public DFIdleState(DFStateData stateData)
            : base(stateData)
        {
            State = TC_DataflowStatus.Idle;
        }

        public override DFStateBase NextState()
        {
            if ((StateData.Error != ErrorID.Undefined) || StateData.Maintenance_InError)
                return new DFMaintenance(StateData);
            else
            if (StateData.Executing)
                return new DFIdleState(StateData);
            else
                return this;
        }
    }
    public class DFExecutingState : DFStateBase
    {
        public DFExecutingState(DFStateData stateData)
            : base(stateData)
        {
            State = TC_DataflowStatus.Executing;
        }

        public override DFStateBase NextState()
        {
            if ((StateData.Error != ErrorID.Undefined) || StateData.Maintenance_InError)
                return new DFMaintenance(StateData);
            else
            if (!StateData.Executing)
                return new DFIdleState(StateData);
            else
                return this;
        }
    }
}
