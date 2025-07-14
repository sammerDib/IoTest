using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Flows.Corrector;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using AcquireOneImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageResult>;
using CorrectorFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorInput,
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorResult,
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorConfiguration>;

namespace UnitySC.PM.DMT.Service.Flows.FlowTask
{
    public class DMTCorrectorFlowTask
    {
        public CorrectorFlowTask ComputationTask;

        private AcquireOneImageFlowTask _previousAcquisitionFlowTask;

        private readonly CorrectorFlow _correctorFlow;
        
        private CancellationTokenSource _cancellationSource;

        public DMTCorrectorFlowTask(CorrectorFlow correctorFlow)
        {
            _correctorFlow = correctorFlow;
        }

        public CorrectorFlowTask CreateAndChainComputationContinuationTasks(
            CancellationTokenSource cancellationTokenSource, AcquireOneImageFlowTask previousAcquisitionFlowTask,
            IFlowTask otherComputationTask = null)
        {
            _cancellationSource = cancellationTokenSource;
            _previousAcquisitionFlowTask = previousAcquisitionFlowTask;
            if (otherComputationTask is null)
            {
                ComputationTask = _previousAcquisitionFlowTask.CheckSuccessAndContinueWith(_correctorFlow, lastAcquisitionTask =>
                {
                    _correctorFlow.Input.AcquiredImage = lastAcquisitionTask.Result.AcquiredImage.Clone();
                });
            }
            else
            {
                var previousTasks = new []
                {
                    otherComputationTask,
                    _previousAcquisitionFlowTask
                };
                ComputationTask = CorrectorFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(previousTasks, _correctorFlow, _cancellationSource, antecedents =>
                {
                    _correctorFlow.Input.AcquiredImage = _previousAcquisitionFlowTask.Result.AcquiredImage.Clone();
                });
            }

            return ComputationTask;
        }
    }
}
