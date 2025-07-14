using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data.PMProcessingState
{
    public class PMP_NotReady : BasePMProcessingState
    {

        public PMP_NotReady()
        {
        }
        public PMP_NotReady(ProcessingStateData stateData)
            : base(stateData)
        {
            State = PMProcessingStates.NotReady;
        }


        public override BasePMProcessingState NextState()
        {
            if (ProcessingStateData.Initialized)
            {
                if ((ProcessingStateData.Error != ErrorID.Undefined))
                    return new PMP_NotReady(ProcessingStateData);
                else
                    return new PMP_Idle(ProcessingStateData);
            }
            else
                return this;

        }
    }

    public class PMP_Idle : BasePMProcessingState
    {
        public PMP_Idle(ProcessingStateData stateData)
            : base(stateData)
        {
            ProcessingStateData = stateData;
            State = PMProcessingStates.Idle;
        }


        public override BasePMProcessingState NextState()
        {
            if ((!ProcessingStateData.Initialized) || (ProcessingStateData.Error != ErrorID.Undefined))
                return new PMP_NotReady(ProcessingStateData);
            else if (ProcessingStateData.Processing)
                return new PMP_Processing(ProcessingStateData);
            else
                return this;
        }
    }

    public class PMP_Processing : BasePMProcessingState
    {
        public PMP_Processing(ProcessingStateData stateData)
            : base(stateData)
        {
            ProcessingStateData = stateData;
            State = PMProcessingStates.Processing;
        }


        public override BasePMProcessingState NextState()
        {
            if (ProcessingStateData.Error != ErrorID.Undefined)
                return new PMP_NotReady(ProcessingStateData);
            else
            if (!ProcessingStateData.Processing)
                return new PMP_Idle(ProcessingStateData);
            else
                return this;
        }
    }
}
