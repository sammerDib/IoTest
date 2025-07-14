using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data.PMProcessingState
{

    public class ProcessingStateData
    {
        // Values
        public bool Processing;
        public bool Initialized;
        public ErrorID Error;
    }

    public abstract class BasePMProcessingState
    {
        private ProcessingStateData _processingStateData;
        private PMProcessingStates _state = PMProcessingStates.NotReady;
        public ProcessingStateData ProcessingStateData { get => _processingStateData; set => _processingStateData = value; }

        public PMProcessingStates State
        {
            get => _state;
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                }
            }
        }

        public BasePMProcessingState()
        {
            _processingStateData = new ProcessingStateData();
        }

        public BasePMProcessingState(ProcessingStateData stateData)
        {
            _processingStateData = stateData;
        }

        public BasePMProcessingState ChangeState_SetNotInitialized()
        {
            _processingStateData.Initialized = false;
            return NextState();
        }
        public BasePMProcessingState ChangeState_InitializationStarted()
        {
            _processingStateData.Initialized = false;
            return NextState();
        }

        public BasePMProcessingState ChangeState_InitializationFinished()
        {
            _processingStateData.Initialized = true;
            _processingStateData.Processing = false;
            _processingStateData.Error = ErrorID.Undefined;
            return NextState();
        }

        public BasePMProcessingState ChangeState_Processing()
        {
            _processingStateData.Processing = true;
            return NextState();
        }

        public BasePMProcessingState ChangeState_ProcessFinisihed()
        {
            _processingStateData.Processing = false;
            return NextState();
        }

        public BasePMProcessingState ChangeState_OnError(ErrorID error)
        {
            _processingStateData.Error = error;
            return NextState();
        }
        public BasePMProcessingState ChangeState_OnErrorCleared()
        {
            _processingStateData.Error = ErrorID.Undefined;
            return NextState();
        }
        public abstract BasePMProcessingState NextState();
    }
}
