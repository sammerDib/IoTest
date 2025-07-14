using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

using AcquireOneImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageResult>;
using AcquirePhaseImagesForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionConfiguration>;

namespace UnitySC.PM.DMT.Service.Flows.FlowTask
{
    public class DMTDeflectometryAcquisitionFlowTask
    {
        public IFlowTask FirstTask;

        public AcquirePhaseImagesForPeriodAndDirectionFlowTask LastAcquisitionTask;

        internal Task LastTemporaryResultStorageTask;

        public Dictionary<int, Dictionary<FringesDisplacement, List<ServiceImage>>>
            TemporaryResultsByPeriodAndDirection;

        private readonly List<AcquirePhaseImagesForPeriodAndDirectionFlow>
            _acquirePhaseImagesForPeriodAndDirectionFlows;

        private readonly AutoExposureFlow _aeFlow;

        internal CancellationTokenSource CancellationTokenSource;

        internal Dictionary<int, Dictionary<FringesDisplacement, AcquirePhaseImagesForPeriodAndDirectionFlowTask>>
            AcquisitionTasksByPeriodAndDirection;

        public DMTDeflectometryAcquisitionFlowTask(
            AutoExposureFlow aeFlow, List<AcquirePhaseImagesForPeriodAndDirectionFlow> acquirePhaseImagesFlows)
        {
            _aeFlow = aeFlow;
            _acquirePhaseImagesForPeriodAndDirectionFlows = acquirePhaseImagesFlows;
            InitializeTemporaryResultsAndTaskDictionaries();
        }

        public DMTDeflectometryAcquisitionFlowTask(
            List<AcquirePhaseImagesForPeriodAndDirectionFlow> acquirePhaseImagesFlows)
        {
            _acquirePhaseImagesForPeriodAndDirectionFlows = acquirePhaseImagesFlows;
            InitializeTemporaryResultsAndTaskDictionaries();
        }

        public void Start(
            CancellationTokenSource cancellationTokenSource,
            AutoExposureFlow.StatusChangedEventHandler aeStatusChangedHandler,
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler)
        {
            CancellationTokenSource = cancellationTokenSource;
            CreateAcquisitionFlowTasks(null, cancellationTokenSource, aeStatusChangedHandler, apiStatusChangedHandler);
        }

        private void CreateAcquisitionFlowTasks(
            IFlowTask previousAcquisitionTask, CancellationTokenSource cancellationTokenSource,
            AutoExposureFlow.StatusChangedEventHandler aeStatusChangedHandler,
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler)
        {
            if (!(cancellationTokenSource is null))
            {
                CancellationTokenSource = cancellationTokenSource;
            }
            if (!(previousAcquisitionTask is null) &&
                !(previousAcquisitionTask is AcquireOneImageFlowTask) &&
                !(previousAcquisitionTask is AcquirePhaseImagesForPeriodAndDirectionFlowTask))
            {
                throw new Exception("previousAcquisitionTask parameter is not of the right type");
            }
            
            if (!(_aeFlow is null))
            {
                _aeFlow.CancellationToken = CancellationTokenSource.Token;
                CreateFirstAutoExposureTask(previousAcquisitionTask, cancellationTokenSource, aeStatusChangedHandler);
            }

            foreach (var apiFlow in _acquirePhaseImagesForPeriodAndDirectionFlows)
            {
                apiFlow.CancellationToken = CancellationTokenSource.Token;
            }

            _acquirePhaseImagesForPeriodAndDirectionFlows.Aggregate(previousAcquisitionTask,
                                                                    (previousTask, currentFlow) =>
                                                                        CreateContinuationTaskFromPreviousTaskAndCurrentAcquirePhaseImageFlow(cancellationTokenSource,
                                                                         apiStatusChangedHandler, currentFlow,
                                                                         previousTask));
            LastTemporaryResultStorageTask = CreateStoreTemporaryResultsContinuationTask();
        }

        private void CreateFirstAutoExposureTask(
            IFlowTask previousAcquisitionTask, CancellationTokenSource cancellationTokenSource,
            AutoExposureFlow.StatusChangedEventHandler
                aeStatusChangedHandler)
        {
            _aeFlow.StatusChanged += aeStatusChangedHandler;
            if (previousAcquisitionTask is null)
            {
                FirstTask = new AutoExposureFlowTask(_aeFlow, cancellationTokenSource);
                FirstTask.Start();
            }
            else
            {
                AutoExposureFlowTask firstTask;
                var aeFlowTask = new AutoExposureFlowTask(_aeFlow, CancellationTokenSource);
                switch (previousAcquisitionTask)
                {
                    case AcquireOneImageFlowTask acquireOneImageFlowTask:
                        firstTask = (AutoExposureFlowTask)acquireOneImageFlowTask.CheckSuccessAndContinueWith(aeFlowTask);
                        break;
                    case AcquirePhaseImagesForPeriodAndDirectionFlowTask acquirePhaseImagesForPeriodAndDirectionFlowTask:
                        firstTask = (AutoExposureFlowTask)acquirePhaseImagesForPeriodAndDirectionFlowTask.CheckSuccessAndContinueWith(aeFlowTask);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Cannot support Flow task {previousAcquisitionTask.GetType()}");
                }
                FirstTask = firstTask;
            }

            ((AutoExposureFlowTask) FirstTask).ContinueWith(previousAEFlowTask =>
            {
                _aeFlow.StatusChanged -= aeStatusChangedHandler;
            });
        }

        private AcquirePhaseImagesForPeriodAndDirectionFlowTask CreateContinuationTaskFromPreviousTaskAndCurrentAcquirePhaseImageFlow(
            CancellationTokenSource cancellationTokenSource,
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler,
            AcquirePhaseImagesForPeriodAndDirectionFlow currentFlow, IFlowTask previousTask)
        {
            currentFlow.StatusChanged += apiStatusChangedHandler;
            int period = currentFlow.Input.Period;
            var direction = currentFlow.Input.FringesDisplacementDirection;
            if (previousTask is null)
            {
                if (_aeFlow is null)
                {
                    var task = new AcquirePhaseImagesForPeriodAndDirectionFlowTask(currentFlow, cancellationTokenSource);

                    LastAcquisitionTask = task;
                    FirstTask = task;
                    task.Start();
                }
                else
                {
                    LastAcquisitionTask = CreateContinuationFromAutoExposure(currentFlow);
                }
            }
            else
            {
                if (!(_aeFlow is null) && LastAcquisitionTask is null)
                {
                    LastAcquisitionTask = CreateContinuationFromAutoExposure(currentFlow);
                }
                else
                {
                    LastAcquisitionTask = CreateAcquisitionTaskContinuation(currentFlow, previousTask);
                }
            }

            LastAcquisitionTask.ContinueWith(previousAcquirePhaseImageFlowTask =>
            {
                currentFlow.StatusChanged -= apiStatusChangedHandler;
            });
            AcquisitionTasksByPeriodAndDirection[period][direction] = LastAcquisitionTask;
            return LastAcquisitionTask;
        }

        internal void CreateContinueWithTaskChain(
            CancellationTokenSource cancellationTokenSource,
            AcquireOneImageFlowTask previousSingleAcquisitionTask,
            AutoExposureFlow.StatusChangedEventHandler
                aeStatusChangedHandler,
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler)
        {
            CancellationTokenSource = cancellationTokenSource;
            CreateAcquisitionFlowTasks(previousSingleAcquisitionTask, cancellationTokenSource, aeStatusChangedHandler,
                                       apiStatusChangedHandler);
        }

        public DMTDeflectometryAcquisitionFlowTask ContinueWith(
            DMTDeflectometryAcquisitionFlowTask nextAcquisitionFlow,
            AutoExposureFlow.StatusChangedEventHandler aeStatusChangedHandler,
            AcquirePhaseImagesForPeriodAndDirectionFlow.StatusChangedEventHandler apiStatusChangedHandler)
        {
            nextAcquisitionFlow.CancellationTokenSource = CancellationTokenSource;
            nextAcquisitionFlow.CreateAcquisitionFlowTasks(LastAcquisitionTask, null, aeStatusChangedHandler,
                                                           apiStatusChangedHandler);
            return nextAcquisitionFlow;
        }

        private double GetDefaultExposureTimeMsIfFailure(AcquirePhaseImagesForPeriodAndDirectionFlow currentFlow)
        {
            return _aeFlow.Configuration.DefaultAutoExposureSetting
                           .First(setting => setting.WaferSide == currentFlow.Input.AcquisitionSide &&
                                             setting.Measure == Interface.Measure.MeasureType.DeflectometryMeasure)
                           .DefaultExposureTimeMsIfFailure;
        }

        private AcquirePhaseImagesForPeriodAndDirectionFlowTask CreateContinuationFromAutoExposure(
            AcquirePhaseImagesForPeriodAndDirectionFlow acquirePhaseFlow)
        {
            
            return ((AutoExposureFlowTask)FirstTask).ContinueWithAcquirePhaseImagesForPeriodAndDirectionFlow(acquirePhaseFlow);
        }

        private AcquirePhaseImagesForPeriodAndDirectionFlowTask CreateAcquisitionTaskContinuation(
            AcquirePhaseImagesForPeriodAndDirectionFlow acquirePhaseFlow,
            IFlowTask previousTask)
        {
            if (previousTask is AcquireOneImageFlowTask previousAcquireOneImageTask)
            {
                // We're continuing from a single acquisition flow task
                CreateContinuationTaskFromPreviousAcquireOneImageFlow(acquirePhaseFlow, previousAcquireOneImageTask);
                return LastAcquisitionTask;
            }

            if (!(LastAcquisitionTask is null))
            {
                // We're continuing from a previous AcquirePhaseImageFlow from this instance of
                // DMTDeflectometryAcquisitionFlowTask
                return CreateContinuationTaskFromAcquirePhaseImageFlow(acquirePhaseFlow);
            }

            // We're continuing from another instance of DMTDeflectometryAcquisitionFlowTask
            LastAcquisitionTask =
                ((AcquirePhaseImagesForPeriodAndDirectionFlowTask)previousTask).CheckSuccessAndContinueWith(acquirePhaseFlow);
            return LastAcquisitionTask;
        }

        private AcquirePhaseImagesForPeriodAndDirectionFlowTask CreateContinuationTaskFromAcquirePhaseImageFlow(
            AcquirePhaseImagesForPeriodAndDirectionFlow acquirePhaseFlow)
        {
            return LastAcquisitionTask.CheckSuccessAndContinueWith(acquirePhaseFlow, task =>
                {
                    SetExposureTimeMsFromAutoExposureFlowOrThrow(acquirePhaseFlow);
                    int period = task.Result.Period;
                    var direction = task.Result.FringesDisplacementDirection;
                    TemporaryResultsByPeriodAndDirection[period][direction] = task.Result.TemporaryResults;
                });
        }

        private void SetExposureTimeMsFromAutoExposureFlowOrThrow(AcquirePhaseImagesForPeriodAndDirectionFlow acquirePhaseFlow)
        {
            if (!(_aeFlow is null))
            {
                if (_aeFlow.Result.Status.State == FlowState.Success)
                {
                    acquirePhaseFlow.Input.ExposureTimeMs = _aeFlow.Result.ExposureTimeMs;
                }
                else if (_aeFlow.Configuration.IgnoreAutoExposureFailure)
                {
                    acquirePhaseFlow.Input.ExposureTimeMs = GetDefaultExposureTimeMsIfFailure(acquirePhaseFlow);
                }
                else
                {
                    throw new TaskCanceledException();
                }
            }
        }

        private void CreateContinuationTaskFromPreviousAcquireOneImageFlow(
            AcquirePhaseImagesForPeriodAndDirectionFlow acquirePhaseFlow,
            AcquireOneImageFlowTask previousAcquireOneImageTask)
        {
            LastAcquisitionTask = previousAcquireOneImageTask.CheckSuccessAndContinueWith(acquirePhaseFlow, task =>
            {
                SetExposureTimeMsFromAutoExposureFlowOrThrow(acquirePhaseFlow);
            });
        }

        private Task CreateStoreTemporaryResultsContinuationTask()
        {

            return LastAcquisitionTask.ContinueWith(previousAcquisitionTask =>
            {
                if (previousAcquisitionTask.Status == TaskStatus.RanToCompletion &&
                    previousAcquisitionTask.Result.Status.State == FlowState.Success)
                {
                    int period = previousAcquisitionTask.Result.Period;
                    var direction = previousAcquisitionTask.Result.FringesDisplacementDirection;
                    TemporaryResultsByPeriodAndDirection[period][direction] =
                        previousAcquisitionTask.Result.TemporaryResults;
                }
            });
        }

        private void InitializeTemporaryResultsAndTaskDictionaries()
        {
            var periods = _acquirePhaseImagesForPeriodAndDirectionFlows.SelectMany(flow => flow.Input.Fringe.Periods)
                                                                       .Distinct().ToList();
            TemporaryResultsByPeriodAndDirection =
                new Dictionary<int, Dictionary<FringesDisplacement, List<ServiceImage>>>(periods.Count);
            AcquisitionTasksByPeriodAndDirection =
                new Dictionary<int,
                    Dictionary<FringesDisplacement,
                        AcquirePhaseImagesForPeriodAndDirectionFlowTask>>(periods.Count);
            foreach (int period in periods)
            {
                TemporaryResultsByPeriodAndDirection[period] = new Dictionary<FringesDisplacement, List<ServiceImage>>
                                                               {
                                                                   { FringesDisplacement.X, null },
                                                                   { FringesDisplacement.Y, null },
                                                               };
                AcquisitionTasksByPeriodAndDirection[period] =
                    new Dictionary<FringesDisplacement, AcquirePhaseImagesForPeriodAndDirectionFlowTask>
                    {
                        { FringesDisplacement.X, null },
                        { FringesDisplacement.Y, null },
                    };
            }
        }
    }
}
