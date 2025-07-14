using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data.TC_PMStates
{
    public class TC_InitializingState : BaseTCPMState
    {
        public TC_InitializingState()
               : base()
        {
            State = TC_PMState.Initializing;
        }
        public TC_InitializingState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Initializing;
        }

        public override BaseTCPMState NextState()
        {
            if (StateData.Initialized)
                return new TC_OfflineState(StateData);
            else
                return this;
        }
    }

    public class TC_OfflineState : BaseTCPMState
    {

        public TC_OfflineState()
            : base()
        {

        }
        public TC_OfflineState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Offline;
        }

        public override BaseTCPMState NextState()
        {
            if (!StateData.Initialized || (StateData.Error != ErrorID.Undefined))
                return new ErrorState(StateData);
            else
            if (StateData.Communicating && StateData.Online1_Offline0)
                return new IdleState(StateData);
            else
                return this;
        }
    }

    public class IdleState : BaseTCPMState
    {
        public IdleState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Idle;
        }

        public override BaseTCPMState NextState()
        {
            if (!StateData.Initialized || (StateData.Error != ErrorID.Undefined))
                return new ErrorState(StateData);
            else
            {
                if (StateData.Processing)
                    return new ActiveState(StateData);
                else
                if (!StateData.Communicating || !StateData.Online1_Offline0)
                    return new TC_OfflineState(StateData);
                else
                    return this;
            }
        }
    }
    public class PendingToActive : BaseTCPMState
    {
        public PendingToActive(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Pending_To_Active;
        }

        public override BaseTCPMState NextState()
        {
            if (!StateData.Initialized || (StateData.Error != ErrorID.Undefined))
                return new ErrorState(StateData);
            else
            {
                if (StateData.Processing)
                    return new ActiveState(StateData);
                else
                   if (!StateData.ProcessPreparing)
                    return new IdleState(StateData);
                else
                    return this;
            }
        }
    }

    public class ActiveState : BaseTCPMState
    {
        public ActiveState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Active;
        }

        public override BaseTCPMState NextState()
        {
            if (!StateData.Initialized || (StateData.Error != ErrorID.Undefined))
                return new ErrorState(StateData);
            else
            {
                if (StateData.Suspended)
                    return new SuspendedState(StateData);
                else
                if (!StateData.Processing)
                    return new IdleState(StateData);
                else
                    return this;
            }
        }
    }


    public class ErrorState : BaseTCPMState
    {
        public ErrorState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Error;
        }

        public override BaseTCPMState NextState()
        {
            if (StateData.Error == ErrorID.Undefined)
            {
                if (!StateData.Initialized)
                    return new TC_InitializingState(StateData);
                else
                    return new IdleState(StateData);
            }
            else
                return this;
        }
    }


    public class SuspendedState : BaseTCPMState
    {
        public SuspendedState(StateData stateData)
            : base(stateData)
        {
            State = TC_PMState.Suspended;
        }

        public override BaseTCPMState NextState()
        {
            if (!StateData.Initialized || (StateData.Error != ErrorID.Undefined))
                return new ErrorState(StateData);
            else
            {
                if (!StateData.Suspended)
                    return new ActiveState(StateData);
                else
                    return this;
            }
        }
    }
}
