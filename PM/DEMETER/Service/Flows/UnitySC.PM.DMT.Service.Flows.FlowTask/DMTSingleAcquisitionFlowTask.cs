using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Flow.Implementation;

using UnitySCSharedAlgosOpenCVWrapper;

using AcquireOneImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageResult>;

namespace UnitySC.PM.DMT.Service.Flows.FlowTask
{
    public class DMTSingleAcquisitionFlowTask
    {
        public IFlowTask FirstTask;

        public AcquireOneImageFlowTask LastAcquisitionTask;

        public SaveImageFlowTask SaveImageTask;

        private readonly AutoExposureFlow _aeFlow;

        private readonly AcquireOneImageFlow _aiFlow;

        private readonly SaveImageFlow _siFlow;

        private CancellationTokenSource _cancellationTokenSource;

        public bool HasAutoExposure => FirstTask is AutoExposureFlowTask;

        public DMTSingleAcquisitionFlowTask(AutoExposureFlow aeFlow, AcquireOneImageFlow aiFlow, SaveImageFlow siFlow)
        {
            _aeFlow = aeFlow;
            _aiFlow = aiFlow;
            _siFlow = siFlow;
        }

        public DMTSingleAcquisitionFlowTask(AcquireOneImageFlow aiFlow, SaveImageFlow siFlow)
        {
            _aiFlow = aiFlow;
            _siFlow = siFlow;
        }

        public DMTSingleAcquisitionFlowTask(AutoExposureFlow aeFlow, AcquireOneImageFlow aiFlow)
        {
            _aeFlow = aeFlow;
            _aiFlow = aiFlow;
        }

        public DMTSingleAcquisitionFlowTask(AcquireOneImageFlow aiFlow)
        {
            _aiFlow = aiFlow;
        }

        public Task<AcquireOneImageResult> Start(CancellationTokenSource cancellationTokenSource,
            AutoExposureFlow.StatusChangedEventHandler aeFlowStatusChangedEventHandler,
            AcquireOneImageFlow.StatusChangedEventHandler aiFlowStatusChangedEventHandler,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> onResultGeneratedEventHandler)
        {
            _cancellationTokenSource = cancellationTokenSource;
            if (!(_aeFlow is null))
            {
                _aeFlow.CancellationToken = _cancellationTokenSource.Token;
            }

            _aiFlow.CancellationToken = _cancellationTokenSource.Token;
            if (!(_siFlow is null))
            {
                _siFlow.CancellationToken = _cancellationTokenSource.Token;
            }
            if (FirstTask is null)
            {
                CreateAcquisitionTasks(aeFlowStatusChangedEventHandler, aiFlowStatusChangedEventHandler, siStatusChangedHandler, onResultGeneratedEventHandler);
            }
            FirstTask.Start();
            return (Task<AcquireOneImageResult>)LastAcquisitionTask;
        }

        public DMTSingleAcquisitionFlowTask ContinueWith(DMTSingleAcquisitionFlowTask nextSingleAcquisition,
            AutoExposureFlow.StatusChangedEventHandler aeFlowStatusChangedEventHandler,
            AcquireOneImageFlow.StatusChangedEventHandler aiFlowStatusChangedEventHandler,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> onResultGeneratedEventHandler)
        {
            nextSingleAcquisition._cancellationTokenSource = _cancellationTokenSource;
            nextSingleAcquisition.CreateAcquisitionTasks(aeFlowStatusChangedEventHandler, aiFlowStatusChangedEventHandler, siStatusChangedHandler, onResultGeneratedEventHandler, LastAcquisitionTask);
            return nextSingleAcquisition;
        }

        public DMTDeflectometryAcquisitionFlowTask ContinueWith(
            DMTDeflectometryAcquisitionFlowTask nextAcquisitionFlow,
            AutoExposureFlow.StatusChangedEventHandler aeStatusChangedHandler, 
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler)
        {
            nextAcquisitionFlow.CreateContinueWithTaskChain(_cancellationTokenSource, LastAcquisitionTask, aeStatusChangedHandler, apiStatusChangedHandler);
            return nextAcquisitionFlow;
        }

        public DMTCorrectorFlowTask ContinueWith(DMTCorrectorFlowTask nextCorrectorFlow,
            IFlowTask otherComputationFlowTask)
        {
            nextCorrectorFlow.CreateAndChainComputationContinuationTasks(_cancellationTokenSource, LastAcquisitionTask, otherComputationFlowTask);
            return nextCorrectorFlow;
        }

        private void CreateAcquisitionTasks(
            AutoExposureFlow.StatusChangedEventHandler aeFlowStatusChangedEventHandler,
            AcquireOneImageFlow.StatusChangedEventHandler aiFlowStatusChangedEventHandler,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> onResultGeneratedEventHandler, AcquireOneImageFlowTask previousAcquireImageTask = null)
        {
            _aiFlow.StatusChanged += aiFlowStatusChangedEventHandler;
            bool exposureIsManual = _aeFlow is null;
            if (!(_aeFlow is null))
            {
                _aeFlow.StatusChanged += aeFlowStatusChangedEventHandler;
                var aeFlowTask = new AutoExposureFlowTask(_aeFlow, _cancellationTokenSource);
                var firstTask = previousAcquireImageTask is null ? aeFlowTask : previousAcquireImageTask.CheckSuccessAndContinueWith(aeFlowTask);
                FirstTask = firstTask;
                LastAcquisitionTask = ((AutoExposureFlowTask)firstTask).ContinueWithAcquireOneImageFlow(_aiFlow);
                LastAcquisitionTask.ContinueWith(CreateUnregisterAcquireImageFlowStatusChangedHandlerTask(aiFlowStatusChangedEventHandler));
                if (!(_siFlow is null))
                {
                    _siFlow.StatusChanged += siStatusChangedHandler;
                    var siFlowTask = new SaveImageFlowTask(_siFlow, _cancellationTokenSource);
                    SaveImageTask = (SaveImageFlowTask)LastAcquisitionTask.CheckSuccessAndContinueWith(siFlowTask, GetBeforeSaveImageStartAction(exposureIsManual));
                    SaveImageTask.ContinueWith(previousSiTask =>
                    {
                        _siFlow.StatusChanged -= siStatusChangedHandler;
                    });
                    SaveImageTask.ContinueWithResultGeneratedHandlerInvocationIfNeeded(onResultGeneratedEventHandler);
                }

            }
            else
            {
                var aiFlowTask = new AcquireOneImageFlowTask(_aiFlow, _cancellationTokenSource);
                var firstTask = previousAcquireImageTask is null ? aiFlowTask : previousAcquireImageTask.CheckSuccessAndContinueWith(aiFlowTask);
                FirstTask = firstTask;
                LastAcquisitionTask = firstTask;
                LastAcquisitionTask.ContinueWith(
                    CreateUnregisterAcquireImageFlowStatusChangedHandlerTask(aiFlowStatusChangedEventHandler));
                if (!(_siFlow is null))
                {
                    _siFlow.StatusChanged += siStatusChangedHandler;
                    var siFlowTask = new SaveImageFlowTask(_siFlow, _cancellationTokenSource);
                    SaveImageTask = (SaveImageFlowTask)LastAcquisitionTask.CheckSuccessAndContinueWith(siFlowTask,GetBeforeSaveImageStartAction(exposureIsManual));
                    SaveImageTask.ContinueWithResultGeneratedHandlerInvocationIfNeeded(onResultGeneratedEventHandler);
                }

            }
        }

        private AcquireOneImageFlowTask.BeforeFlow2Action GetBeforeSaveImageStartAction(bool exposureIsManual)
        {
            return aiTask =>
            {
                _siFlow.Input.ExposureTimeMs = aiTask.Result.ExposureTimeMs;
                _siFlow.Input.ImageMilToSave = aiTask.Result.AcquiredImage;
                if (exposureIsManual)
                {
                    var imageData = ImageUtils.CreateImageDataFromUSPImageMil(_siFlow.Input.ImageMilToSave);
                    _siFlow.Input.UniformityCorrectionTargetSaturationLevel = (int)ImageOperators.ComputeGreyLevelSaturation(imageData, _siFlow.Input.UniformityCorrectionAcceptableRatioOfSaturatedPixels);
                }
            };
        }

        private Action<Task> CreateUnregisterAcquireImageFlowStatusChangedHandlerTask(AcquireOneImageFlow.StatusChangedEventHandler aiFlowStatusChangedEventHandler)
        {
            return lastAcquisitionTask =>
            {
                _aiFlow.StatusChanged -= aiFlowStatusChangedEventHandler;
            };
        }
    }
}
