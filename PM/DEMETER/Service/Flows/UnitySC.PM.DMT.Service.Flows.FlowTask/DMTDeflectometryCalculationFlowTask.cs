using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

using AcquirePhaseImagesForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionConfiguration>;
using AdjustCurvatureDynamicsForRawCurvatureMapFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapConfiguration>;
using ComputeLowAngleDarkFieldImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeLowAngleDarkFieldImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeLowAngleDarkFieldImageResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeLowAngleDarkFieldImageConfiguration>;
using ComputeNanoTopoFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeNanoTopoInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeNanoTopoResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeNanoTopoConfiguration>;
using ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputePhaseMapAndMaskForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputePhaseMapAndMaskForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputePhaseMapAndMaskForPeriodAndDirectionConfiguration>;
using ComputeUnwrappedPhasesForDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionConfiguration>;
using SaveMaskFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.SaveMaskInput,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveMaskResult,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageConfiguration>;

namespace UnitySC.PM.DMT.Service.Flows.FlowTask
{
    public class DMTDeflectometryCalculationFlowTask
    {
        private Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                AdjustCurvatureDynamicsForRawCurvatureMapFlow>
            _adjustCurvatureMapByComputeRawCurvatureMapFlows;

        private Dictionary<int,
                Dictionary<FringesDisplacement, ComputeRawCurvatureMapForPeriodAndDirectionFlow>>
            _computeCurvatureMapFlowsByPeriodAndDirection;

        private ComputeLowAngleDarkFieldImageFlow _computeLowAngleDarkFieldImageFlow;

        private SaveImageFlow _darkSaveImageFlow;

        private ComputeNanoTopoFlow _computeNanoTopoFlow;

        private SaveImageFlow _nanoTopoSaveImageFlow;

        private SaveMaskFlow _saveMaskFlow;

        private readonly int _largestPeriod;
        private readonly List<int> _periods;

        private readonly Dictionary<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlow> _phaseMapFlowsByAcquisitionTask;

        private List<ComputeUnwrappedPhaseMapForDirectionFlow> _computeUnwrappedPhaseMapFlowForNanoTopo;

        private readonly Dictionary<int,
                Dictionary<FringesDisplacement, ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask>>
            _phaseMapTasksByPeriodAndDirection;

        private Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
            _saveImageFlowByAdjustCurvatureMapFlow;

        private Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
            _saveImageFlowByComputeUnwrappedPhaseMapFlow;

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlow.StatusChangedEventHandler _cpmStatusChangedHandler;

        private ComputeRawCurvatureMapForPeriodAndDirectionFlow.StatusChangedEventHandler _crcmStatusChangedHandler;

        private AdjustCurvatureDynamicsForRawCurvatureMapFlow.StatusChangedEventHandler _acmStatusChangedHandler;

        private SaveImageFlow.StatusChangedEventHandler _siStatusChangedHandler;

        private ComputeLowAngleDarkFieldImageFlow.StatusChangedEventHandler _cladfStatusChangedHandler;

        private ComputeNanoTopoFlow.StatusChangedEventHandler _nanoTopoStatusChangedHandler;

        public readonly List<SaveImageFlowTask> SaveImageTasks;

        private readonly CancellationTokenSource _cancellationTokenSource;

        public IFlowTask LastComputationTask;

        public SaveMaskFlowTask SaveMaskFlowTask;

        public readonly Side Side;

        public DMTDeflectometryCalculationFlowTask(
            DMTDeflectometryAcquisitionFlowTask previousAcquisitionFlowTask,
            List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> computePhaseMapFlows,
            SaveMaskFlow saveMaskFlow = null)
        {
            _cancellationTokenSource = previousAcquisitionFlowTask.CancellationTokenSource;
            _saveMaskFlow = saveMaskFlow;
            _phaseMapFlowsByAcquisitionTask =
                InitializePhaseMapFlowByAcquisitionTaskDictionaryFromComputePhaseMapFlowList(
                 previousAcquisitionFlowTask, computePhaseMapFlows);
            _phaseMapTasksByPeriodAndDirection =
                new Dictionary<int,
                    Dictionary<FringesDisplacement, ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask>>(
                 _phaseMapFlowsByAcquisitionTask.Values.Count);

            foreach (int period in _phaseMapFlowsByAcquisitionTask.Values.Select(cpmFlow => cpmFlow.Input.Period))
            {
                _phaseMapTasksByPeriodAndDirection[period] =
                    new Dictionary<FringesDisplacement, ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask>
                    {
                        { FringesDisplacement.X, null },
                        { FringesDisplacement.Y, null }
                    };
            }

            _periods = _phaseMapFlowsByAcquisitionTask.Values.First().Input.Fringe.Periods;
            _largestPeriod = _periods.Max();

            Side = _phaseMapFlowsByAcquisitionTask.Values.First().Input.Side;

            SaveImageTasks = new List<SaveImageFlowTask>(0);
        }

        public IFlowTask CreateAndChainComputationContinuationTasks(
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler, IFlowTask previousComputationFlowTask = null)
        {
            InitializeFlowCancellationTokens();
            EnsureSaveImageTaskListCapacity();
            LastComputationTask = _phaseMapFlowsByAcquisitionTask
                                  .OrderBy(taskFlowkvPair =>
                                               taskFlowkvPair.Value.Input.FringesDisplacementDirection ==
                                               FringesDisplacement.X
                                                   ? 0
                                                   : 10)
                                  .ThenBy(taskFlowKVPair => taskFlowKVPair.Value.Input.Period)
                                  .Aggregate(previousComputationFlowTask, (previousTask, currentTaskFlowKvPair) =>
                                                 CreateContinuationTaskFromPreviousTaskAndTaskFlowPair(previousTask,
                                                  currentTaskFlowKvPair, resultGeneratedHandler));

            ChainSaveMaskFlowToLastSaveImageTaskIfNeeded();

            return LastComputationTask;
        }

        public DMTDeflectometryCalculationFlowTask AddComputePhaseMapHandler(
            ComputePhaseMapAndMaskForPeriodAndDirectionFlow.StatusChangedEventHandler cpmStatusChangedHandler)
        {
            _cpmStatusChangedHandler = cpmStatusChangedHandler;

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddSaveImageHandler(SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler)
        {
            _siStatusChangedHandler = siStatusChangedHandler;

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddCurvatureMapFlows(
            Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow, AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                adjustCurvatureMapByComputeRawCurvatureMapFlows,
            Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                saveImageFlowsByAdjustCurvatureMapFlows)
        {
            _saveImageFlowByAdjustCurvatureMapFlow = saveImageFlowsByAdjustCurvatureMapFlows;
            _adjustCurvatureMapByComputeRawCurvatureMapFlows = adjustCurvatureMapByComputeRawCurvatureMapFlows;
            _computeCurvatureMapFlowsByPeriodAndDirection = _adjustCurvatureMapByComputeRawCurvatureMapFlows.Keys
                .GroupBy(ccmFlow => ccmFlow.Input.Period)
                .ToDictionary(periodGroup => periodGroup.Key, periodGroup =>
                {
                    return periodGroup.GroupBy(ccmFlow => ccmFlow.Input.FringesDisplacementDirection)
                        .ToDictionary(directionGroup => directionGroup.Key,
                            directionGroup => directionGroup.First());
                });
            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddCurvatureMapsHandlers(
            ComputeRawCurvatureMapForPeriodAndDirectionFlow.StatusChangedEventHandler crcmStatusChangedHandler,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow.StatusChangedEventHandler acdStatusChangedHandler)
        {
            _crcmStatusChangedHandler = crcmStatusChangedHandler;
            _acmStatusChangedHandler = acdStatusChangedHandler;

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddLowAngleDarkFieldFlows(ComputeLowAngleDarkFieldImageFlow computeLowAngleDarkFieldImageFlow,
            SaveImageFlow darkSaveImageFlow)
        {
            _computeLowAngleDarkFieldImageFlow = computeLowAngleDarkFieldImageFlow;
            if (!(darkSaveImageFlow is null))
            {
                _darkSaveImageFlow = darkSaveImageFlow;
            }

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddComputeLowAngleDarkFieldHandlers(
            ComputeLowAngleDarkFieldImageFlow.StatusChangedEventHandler cladfStatusChangedHandler)
        {
            _cladfStatusChangedHandler = cladfStatusChangedHandler;

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddUnwrappedPhaseMapsFlows(Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
            saveImageFlowByComputeUnwrappedPhaseMapFlow)
        {

            _saveImageFlowByComputeUnwrappedPhaseMapFlow = saveImageFlowByComputeUnwrappedPhaseMapFlow;
            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddNanoTopoFlows(
            List<ComputeUnwrappedPhaseMapForDirectionFlow> computeUnwrappedPhaseMapFlowForNanoTopo,
            ComputeNanoTopoFlow computeNanoTopoFlow, SaveImageFlow nanoTopoSaveImageFlow)
        {
            _computeUnwrappedPhaseMapFlowForNanoTopo = computeUnwrappedPhaseMapFlowForNanoTopo;
            _computeNanoTopoFlow = computeNanoTopoFlow;
            if (!(nanoTopoSaveImageFlow is null))
            {
                _nanoTopoSaveImageFlow = nanoTopoSaveImageFlow;
            }
            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddNanoTopoHandlers(ComputeNanoTopoFlow.StatusChangedEventHandler nanoTopoStatusChangedHandler)
        {
            _nanoTopoStatusChangedHandler = nanoTopoStatusChangedHandler;

            return this;
        }

        public DMTDeflectometryCalculationFlowTask AddSaveMaskFlow(SaveMaskFlow saveMaskFlow)
        {
            _saveMaskFlow = saveMaskFlow;
            return this;
        }

        public void SetCorrectorResultForPhaseMapFlows(DMTCorrectorFlowTask correctorFlowTask)
        {
            correctorFlowTask.ComputationTask.CheckSuccessAndContinueWith(correctorTask =>
            {
                foreach (var phaseMapFlow in _phaseMapFlowsByAcquisitionTask.Values)
                {
                    phaseMapFlow.Input.CorrectorResult = correctorTask.Result;
                }
            });
        }

        private void EnsureSaveImageTaskListCapacity()
        {
            int saveImageTasksCount = 0;
            if (!_saveImageFlowByComputeUnwrappedPhaseMapFlow.IsNullOrEmpty())
            {
                saveImageTasksCount += _saveImageFlowByComputeUnwrappedPhaseMapFlow.Count;
            }

            if (!_saveImageFlowByAdjustCurvatureMapFlow.IsNullOrEmpty())
            {
                saveImageTasksCount += _saveImageFlowByAdjustCurvatureMapFlow.Count;
            }

            if (!(_nanoTopoSaveImageFlow is null))
            {
                saveImageTasksCount += 1;
            }

            if (!(_darkSaveImageFlow is null))
            {
                saveImageTasksCount += 1;
            }
            SaveImageTasks.Capacity = saveImageTasksCount;
        }

        private void ChainSaveMaskFlowToLastSaveImageTaskIfNeeded()
        {
            if (!(_saveMaskFlow is null))
            {
                int lowestPeriod = _phaseMapTasksByPeriodAndDirection.Keys.Min();
                var firstComputePhaseMapTask = _phaseMapTasksByPeriodAndDirection[lowestPeriod][FringesDisplacement.X];
                switch (LastComputationTask)
                {
                    case ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask cpmFlowTask:
                        SaveMaskFlowTask = cpmFlowTask.CheckSuccessAndContinueWith(_saveMaskFlow, cpmTask =>
                        {
                            _saveMaskFlow.Input.MaskToSave = firstComputePhaseMapTask.Result.PsdResult.Mask;
                        });
                        break;
                    case ComputeNanoTopoFlowTask cntFlowTask:
                        SaveMaskFlowTask = cntFlowTask.CheckSuccessAndContinueWith(_saveMaskFlow, cntTask =>
                        {
                            _saveMaskFlow.Input.MaskToSave = firstComputePhaseMapTask.Result.PsdResult.Mask;
                        });
                        break;
                    case AdjustCurvatureDynamicsForRawCurvatureMapFlowTask _:
                    case ComputeLowAngleDarkFieldImageFlowTask _:
                    case ComputeUnwrappedPhasesForDirectionFlowTask _:
                        var lastSaveImageFlow = SaveImageTasks.Last();
                        SaveMaskFlowTask = lastSaveImageFlow.CheckSuccessAndContinueWith(_saveMaskFlow, ccmTask =>
                        {
                            _saveMaskFlow.Input.MaskToSave = firstComputePhaseMapTask.Result.PsdResult.Mask;
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LastComputationTask));
                }
            }
        }

        private void InitializeFlowCancellationTokens()
        {
            InitializeSaveMaskFlowCancellationToken();

            InitializeComputePhaseMapFlowsCancellationToken();

            InitializeCurvatureMapFlowsCancellationToken();

            InitializeLowAngleDarkFieldFlowsCancellationToken();

            InitializeUnwrappedPhasesForDirectionFlowsCancellationToken();

            InitializeNanoTopoFlowCancellationToken();
        }

        private void InitializeLowAngleDarkFieldFlowsCancellationToken()
        {
            if (!(_computeLowAngleDarkFieldImageFlow is null))
            {
                _computeLowAngleDarkFieldImageFlow.CancellationToken = _cancellationTokenSource.Token;
            }

            if (!(_darkSaveImageFlow is null))
            {
                _darkSaveImageFlow.CancellationToken = _cancellationTokenSource.Token;
            }
        }

        private void InitializeCurvatureMapFlowsCancellationToken()
        {
            if (!_adjustCurvatureMapByComputeRawCurvatureMapFlows.IsNullOrEmpty())
            {
                foreach (var (crcmFlow, acmFlow) in
                         _adjustCurvatureMapByComputeRawCurvatureMapFlows.Select(kvPair => (kvPair.Key, kvPair.Value)))
                {
                    crcmFlow.CancellationToken = _cancellationTokenSource.Token;
                    acmFlow.CancellationToken = _cancellationTokenSource.Token;
                }
            }

            if (!_saveImageFlowByAdjustCurvatureMapFlow.IsNullOrEmpty())
            {
                foreach (var siFlow in _saveImageFlowByAdjustCurvatureMapFlow.Values.Where(flow => !(flow is null)))
                {
                    siFlow.CancellationToken = _cancellationTokenSource.Token;
                }
            }
        }

        private void InitializeComputePhaseMapFlowsCancellationToken()
        {
            foreach (var cpmFlow in _phaseMapFlowsByAcquisitionTask.Values)
            {
                cpmFlow.CancellationToken = _cancellationTokenSource.Token;
            }
        }

        private void InitializeSaveMaskFlowCancellationToken()
        {
            if (!(_saveMaskFlow is null))
            {
                _saveMaskFlow.CancellationToken = _cancellationTokenSource.Token;
            }
        }

        private void InitializeUnwrappedPhasesForDirectionFlowsCancellationToken()
        {
            if (_saveImageFlowByComputeUnwrappedPhaseMapFlow == null || _saveImageFlowByComputeUnwrappedPhaseMapFlow.Count == 0)
                return;
            
            var token = _cancellationTokenSource.Token;

            foreach (var kvPair in _saveImageFlowByComputeUnwrappedPhaseMapFlow)
            {
                if (kvPair.Key != null)
                {
                    var cupmFlow = kvPair.Key;
                    cupmFlow.CancellationToken = token;
                }
                if (kvPair.Value != null)
                {
                    var siFlow = kvPair.Value;
                    siFlow.CancellationToken = token;
                }               
            }
        }

        private void InitializeNanoTopoFlowCancellationToken()
        {
            if (!_computeUnwrappedPhaseMapFlowForNanoTopo.IsNullOrEmpty())
            {
                foreach (var unwrappedPhasemapFlow in _computeUnwrappedPhaseMapFlowForNanoTopo)
                {
                    unwrappedPhasemapFlow.CancellationToken = _cancellationTokenSource.Token;
                }
            }

            if (!(_computeNanoTopoFlow is null))
            {
                _computeNanoTopoFlow.CancellationToken = _cancellationTokenSource.Token;
            }
        }

        private IFlowTask CreateContinuationTaskFromPreviousTaskAndTaskFlowPair(
            IFlowTask previousTask,
            KeyValuePair<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
                ComputePhaseMapAndMaskForPeriodAndDirectionFlow> currentTaskFlowKvPair,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler)
        {
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask;
            int period = currentTaskFlowKvPair.Value.Input.Period;
            var direction = currentTaskFlowKvPair.Value.Input.FringesDisplacementDirection;
            if (previousTask is null)
            {
                computePhaseMapTask = CreateFirstComputePhaseMapTask(currentTaskFlowKvPair, _cpmStatusChangedHandler);
                LastComputationTask = computePhaseMapTask;
                _phaseMapTasksByPeriodAndDirection[period][direction] = computePhaseMapTask;
                if (!AreCurvatureMapOrUnwrappedPhasesNeeded(period))
                {
                    return LastComputationTask;
                }

                var curvatureMapFlow = GetCurvatureMapFlow(period, direction);
                if (!(curvatureMapFlow is null))
                {
                    CreateCurvatureMapComputationAndSavingTask(_crcmStatusChangedHandler, _acmStatusChangedHandler,
                                                               _siStatusChangedHandler, resultGeneratedHandler,
                                                               curvatureMapFlow, computePhaseMapTask);
                }

                if (!AreUnwrappedPhasesMapNeeded(period))
                {
                    return LastComputationTask;
                }

                CreateUnwrappedPhaseMapComputationAndSavingTasks(resultGeneratedHandler, direction,
                                                                 computePhaseMapTask);
            }
            else
            {
                var previousTasks = new[] { previousTask, currentTaskFlowKvPair.Key };
                computePhaseMapTask =
                    CreateComputePhaseMapContinuationTask(previousTasks, currentTaskFlowKvPair,
                                                          _cpmStatusChangedHandler);

                LastComputationTask = computePhaseMapTask;
                _phaseMapTasksByPeriodAndDirection[period][direction] =
                    computePhaseMapTask;

                if (!AreCurvatureMapDarkImageOrUnwrappedPhaseNeeded(period, direction))
                {
                    return LastComputationTask;
                }

                var curvatureMapFlow = GetCurvatureMapFlow(period, direction);
                if (!(curvatureMapFlow is null))
                {
                    CreateCurvatureMapComputationAndSavingTask(_crcmStatusChangedHandler, _acmStatusChangedHandler,
                                                               _siStatusChangedHandler, resultGeneratedHandler,
                                                               curvatureMapFlow, computePhaseMapTask);
                }

                if (IsDarkImageNeeded(period, direction))
                {
                    CreateDarkImageComputationAndSavingTasks(_siStatusChangedHandler, _cladfStatusChangedHandler,
                                                             resultGeneratedHandler);
                }

                if (AreUnwrappedPhasesMapNeeded(period))
                {
                    CreateUnwrappedPhaseMapComputationAndSavingTasks(resultGeneratedHandler, direction,
                                                                     computePhaseMapTask);
                }

                if (IsNanoTopoNeeded(period, direction))
                {
                    CreateNanoTopoComputationAndSavingTasks(_siStatusChangedHandler, _nanoTopoStatusChangedHandler, resultGeneratedHandler, computePhaseMapTask);
                }
            }

            return LastComputationTask;
        }

        private bool IsDarkImageNeeded(int period, FringesDisplacement direction)
        {
            return !(_computeLowAngleDarkFieldImageFlow is null)
                   && period == _computeLowAngleDarkFieldImageFlow.Input.Period
                   && direction == FringesDisplacement.Y;
        }

        private bool AreCurvatureMapDarkImageOrUnwrappedPhaseNeeded(int period, FringesDisplacement direction)
        {
            return !_computeCurvatureMapFlowsByPeriodAndDirection.IsNullOrEmpty()
                   || period == _largestPeriod
                   || (!(_computeLowAngleDarkFieldImageFlow is null) && period == _computeLowAngleDarkFieldImageFlow.Input.Period &&
                       direction == FringesDisplacement.Y);
        }

        private bool AreUnwrappedPhasesMapNeeded(int period)
        {
            return period == _largestPeriod && !_saveImageFlowByComputeUnwrappedPhaseMapFlow.IsNullOrEmpty();
        }

        private bool IsNanoTopoNeeded(int period, FringesDisplacement direction)
        {
            return period == _largestPeriod && !(_computeNanoTopoFlow is null) && direction == FringesDisplacement.Y;
        }

        private bool AreCurvatureMapOrUnwrappedPhasesNeeded(int period)
        {
            return !_computeCurvatureMapFlowsByPeriodAndDirection.IsNullOrEmpty() || period == _largestPeriod;
        }

        private void CreateDarkImageComputationAndSavingTasks(
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            ComputeLowAngleDarkFieldImageFlow.StatusChangedEventHandler cdiStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler)
        {
            int darkPeriod = _computeLowAngleDarkFieldImageFlow.Input.Period;
            var taskList = new List<IFlowTask>
                           {
                               LastComputationTask,
                               _phaseMapTasksByPeriodAndDirection[darkPeriod][FringesDisplacement.X],
                               _phaseMapTasksByPeriodAndDirection[darkPeriod][FringesDisplacement.Y]
                           };
            var computeDarkImageTask =
                CreateComputeLowAngleDarkFieldImageContinuationTask(taskList, cdiStatusChangedHandler);
            LastComputationTask = computeDarkImageTask;
            if (!(_darkSaveImageFlow is null))
            {
                SaveImageTasks.Add(CreateLowAngleDarkFieldImageSaveImageContinuationTask(computeDarkImageTask,
                                                                            siStatusChangedHandler,
                                                                            resultGeneratedHandler));
            }
        }

        private void CreateUnwrappedPhaseMapComputationAndSavingTasks(
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler, FringesDisplacement direction,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask)
        {
            var cupmFlow =
                _saveImageFlowByComputeUnwrappedPhaseMapFlow.Keys.First(flow => flow.Input
                                                                                .FringesDisplacementDirection ==
                                                                            direction);

            var cupmTask = CreateUnwrappedPhaseMapContinuationTask(cupmFlow, computePhaseMapTask, direction);
            LastComputationTask = cupmTask;

            if (!(_saveImageFlowByComputeUnwrappedPhaseMapFlow[cupmFlow] is null))
            {
                SaveImageTasks
                    .Add(CreateUnwrappedPhaseMapSaveImageContinuationTask(resultGeneratedHandler,
                                                                          cupmFlow,
                                                                          cupmTask));
            }
        }


        private void CreateNanoTopoComputationAndSavingTasks(
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            ComputeNanoTopoFlow.StatusChangedEventHandler cntStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask)
        {
            _computeNanoTopoFlow.StatusChanged += cntStatusChangedHandler;
            var cupmFlowX = _computeUnwrappedPhaseMapFlowForNanoTopo.First(cupm => cupm.Input.FringesDisplacementDirection == FringesDisplacement.X);
            var cupmFlowY = _computeUnwrappedPhaseMapFlowForNanoTopo.First(cupm => cupm.Input.FringesDisplacementDirection == FringesDisplacement.Y);

            var cupmTaskX = CreateUnwrappedPhaseMapContinuationTaskForNanoTopo(cupmFlowX, FringesDisplacement.X, computePhaseMapTask);
            LastComputationTask = cupmTaskX;
            var cupmTaskY = CreateUnwrappedPhaseMapContinuationTaskForNanoTopo(cupmFlowY, FringesDisplacement.Y, computePhaseMapTask);

            var tasksToWait = new IFlowTask[]
            {
                cupmTaskX,
                cupmTaskY
            };

            var computeNanoTopoTask = ComputeNanoTopoFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(tasksToWait, _computeNanoTopoFlow, _cancellationTokenSource, cupmTasks =>
            {
                _computeNanoTopoFlow.Input.UnwrappedX = ((ComputeUnwrappedPhasesForDirectionFlowTask)cupmTasks[0]).Result.UnwrappedPhaseMap;
                _computeNanoTopoFlow.Input.UnwrappedY = ((ComputeUnwrappedPhasesForDirectionFlowTask)cupmTasks[1]).Result.UnwrappedPhaseMap;
                _computeNanoTopoFlow.Input.Mask = computePhaseMapTask.Result.PsdResult.Mask;
            });

            computeNanoTopoTask.ContinueWith(previousNanoTopoTask => { _computeNanoTopoFlow.StatusChanged -= cntStatusChangedHandler; });

            LastComputationTask = computeNanoTopoTask;
            if (!(_nanoTopoSaveImageFlow is null))
            {
                SaveImageTasks.Add(CreateNanoTopoSaveImageContinuationTask(computeNanoTopoTask, siStatusChangedHandler, resultGeneratedHandler));
            }
        }

        private void CreateCurvatureMapComputationAndSavingTask(
            ComputeRawCurvatureMapForPeriodAndDirectionFlow.StatusChangedEventHandler crcmStatusChangedHandler,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow.StatusChangedEventHandler acdStatusChangedHandler,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler,
            ComputeRawCurvatureMapForPeriodAndDirectionFlow curvatureMapFlow,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask)
        {
            var adjustCurvatureMapFlow =
                _adjustCurvatureMapByComputeRawCurvatureMapFlows
                    [curvatureMapFlow];
            var curvatureMapTask = CreateCurvatureMapContinuationTask(computePhaseMapTask,
                                                                      curvatureMapFlow, adjustCurvatureMapFlow,
                                                                      crcmStatusChangedHandler, acdStatusChangedHandler);
            curvatureMapTask.ContinueWith(previousCCMTask =>
            {
                curvatureMapFlow.StatusChanged -= crcmStatusChangedHandler;
            });
            if (!(_saveImageFlowByAdjustCurvatureMapFlow[adjustCurvatureMapFlow] is null))
            {
                SaveImageTasks.Add(CreateCurvatureMapSaveImageContinuationTask(curvatureMapTask, adjustCurvatureMapFlow,
                    siStatusChangedHandler, resultGeneratedHandler));
            }

            LastComputationTask = curvatureMapTask;
        }

        private SaveImageFlowTask CreateUnwrappedPhaseMapSaveImageContinuationTask(
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler,
            ComputeUnwrappedPhaseMapForDirectionFlow cupmFlow,
            ComputeUnwrappedPhasesForDirectionFlowTask cupmTask)
        {
            var siFlow = _saveImageFlowByComputeUnwrappedPhaseMapFlow[cupmFlow];
            var siFlowTask = new SaveImageFlowTask(siFlow, _cancellationTokenSource);
            var task = (SaveImageFlowTask)cupmTask.CheckSuccessAndContinueWith(siFlowTask, previousComputeUnwrappedPhaseMapFlowTask =>
            {
                siFlow.Input.ImageDataToSave = previousComputeUnwrappedPhaseMapFlowTask.Result.UnwrappedPhaseMap;
            });
            task.ContinueWithResultGeneratedHandlerInvocationIfNeeded(resultGeneratedHandler);
            return task;
        }

        private ComputeUnwrappedPhasesForDirectionFlowTask CreateUnwrappedPhaseMapContinuationTask(
            ComputeUnwrappedPhaseMapForDirectionFlow cupmFlow,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask, FringesDisplacement direction)
        {
            var tasksToWait = new List<IFlowTask>();
            if (LastComputationTask != computePhaseMapTask)
            {
                tasksToWait.Add(LastComputationTask);
            }

            tasksToWait.AddRange(_periods.Select(computedPeriod =>
                                                     _phaseMapTasksByPeriodAndDirection[computedPeriod][direction]));

            return ComputeUnwrappedPhasesForDirectionFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(tasksToWait.ToArray(),
                                                cupmFlow, _cancellationTokenSource, prevTasks =>
                                                {
                                                    foreach (var previousCPMTaskResult in prevTasks
                                                                 .OfType<ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask>())
                                                    {
                                                        cupmFlow.Input.PhaseMaps
                                                            .Add(previousCPMTaskResult.Result.Period,
                                                                previousCPMTaskResult.Result.PsdResult);
                                                    }
                                                });
        }

        private ComputeUnwrappedPhasesForDirectionFlowTask CreateUnwrappedPhaseMapContinuationTaskForNanoTopo(
            ComputeUnwrappedPhaseMapForDirectionFlow cupmFlow, FringesDisplacement direction,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask)
        {
            var tasksToWait = new List<IFlowTask>();
            if (LastComputationTask != computePhaseMapTask)
            {
                tasksToWait.Add(LastComputationTask);
            }
            tasksToWait.AddRange(_periods.Select(computedPeriod =>
                                                     _phaseMapTasksByPeriodAndDirection[computedPeriod][direction]));


            return ComputeUnwrappedPhasesForDirectionFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(tasksToWait.ToArray(),
                                                cupmFlow, _cancellationTokenSource, prevTasks =>
                                                {
                                                    foreach (var previousCPMTaskResult in prevTasks
                                                                 .OfType<ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask>())
                                                    {
                                                        cupmFlow.Input.PhaseMaps
                                                            .Add(previousCPMTaskResult.Result.Period,
                                                                previousCPMTaskResult.Result.PsdResult);
                                                    }
                                                });
        }

        private ComputeRawCurvatureMapForPeriodAndDirectionFlow GetCurvatureMapFlow(
            int period, FringesDisplacement direction)
        {
            if (_computeCurvatureMapFlowsByPeriodAndDirection.IsNullOrEmpty() ||
                !_computeCurvatureMapFlowsByPeriodAndDirection.TryGetValue(period, out var flowsByDirection))
                return null;
            if (flowsByDirection.TryGetValue(direction, out var computeRawCurvatureMapFlow))
            {
                return computeRawCurvatureMapFlow;
            }

            throw new
                Exception($"Both X and Y curvature map computation flows should exist for {period}px period, {direction} curvature map computation flow not found.");
        }

        private SaveImageFlowTask CreateLowAngleDarkFieldImageSaveImageContinuationTask(
            ComputeLowAngleDarkFieldImageFlowTask computeDarkImageTask,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler)
        {
            _darkSaveImageFlow.StatusChanged += siStatusChangedHandler;
            var siFlowTask = new SaveImageFlowTask(_darkSaveImageFlow, _cancellationTokenSource);
            var task = (SaveImageFlowTask)computeDarkImageTask.CheckSuccessAndContinueWith(siFlowTask, cdiTask =>
            {
                _darkSaveImageFlow.Input.ImageDataToSave = cdiTask.Result.DarkImage;
            });
            task.ContinueWithResultGeneratedHandlerInvocationIfNeeded(resultGeneratedHandler);
            task.ContinueWith(saveImageFlowTask =>
            {
                _darkSaveImageFlow.StatusChanged -= siStatusChangedHandler;
            });
            return task;
        }

        private SaveImageFlowTask CreateNanoTopoSaveImageContinuationTask(
            ComputeNanoTopoFlowTask computeNanoTopoTask,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler)
        {
            _nanoTopoSaveImageFlow.StatusChanged += siStatusChangedHandler;
            var siFlowTask = new SaveImageFlowTask(_nanoTopoSaveImageFlow, _cancellationTokenSource);
            var task = (SaveImageFlowTask)computeNanoTopoTask.CheckSuccessAndContinueWith(_nanoTopoSaveImageFlow, cntTask =>
            {
                _nanoTopoSaveImageFlow.Input.ImageDataToSave = cntTask.Result.NanoTopoImage;
            });
            task.ContinueWithResultGeneratedHandlerInvocationIfNeeded(resultGeneratedHandler);
            task.ContinueWith(saveImageFlowTask =>
            {
                _nanoTopoSaveImageFlow.StatusChanged -= siStatusChangedHandler;
            });
            return task;
        }

        private ComputeLowAngleDarkFieldImageFlowTask CreateComputeLowAngleDarkFieldImageContinuationTask(
            List<IFlowTask> taskList,
            ComputeLowAngleDarkFieldImageFlow.StatusChangedEventHandler cdiStatusChangedHandler)
        {
            _computeLowAngleDarkFieldImageFlow.StatusChanged += cdiStatusChangedHandler;
            var computeDarkImageTask = ComputeLowAngleDarkFieldImageFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(
                taskList.ToArray(), _computeLowAngleDarkFieldImageFlow, _cancellationTokenSource, cpmTasks =>
                {
                    _computeLowAngleDarkFieldImageFlow.Input.XResult =
                        ((ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask)cpmTasks[1]).Result.PsdResult;
                    _computeLowAngleDarkFieldImageFlow.Input.YResult =
                        ((ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask)cpmTasks[2]).Result.PsdResult;
                });
            computeDarkImageTask.ContinueWith(previousComputeDarkImageTask =>
            {
                _computeLowAngleDarkFieldImageFlow.StatusChanged -= cdiStatusChangedHandler;
            });

            return computeDarkImageTask;
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask CreateComputePhaseMapContinuationTask(
            IFlowTask[] previousTasks,
            KeyValuePair<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
                ComputePhaseMapAndMaskForPeriodAndDirectionFlow> currentTaskFlowKvPair,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlow.StatusChangedEventHandler cpmStatusChangedHandler)
        {
            var cpmFlow = currentTaskFlowKvPair.Value;
            cpmFlow.StatusChanged += cpmStatusChangedHandler;
            var task =
                ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(previousTasks, cpmFlow, _cancellationTokenSource, taskArray =>
                {
                    var previousAcquisitionTask = (AcquirePhaseImagesForPeriodAndDirectionFlowTask)taskArray[1];
                    cpmFlow.Input.PhaseImages = previousAcquisitionTask.Result.TemporaryResults;
                });
            task.ContinueWith(previousComputePhaseMapTask =>
            {
                cpmFlow.StatusChanged -= cpmStatusChangedHandler;
            });
            return task;
        }

        private SaveImageFlowTask CreateCurvatureMapSaveImageContinuationTask(
            AdjustCurvatureDynamicsForRawCurvatureMapFlowTask curvatureMapTask,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow adjustCurvatureMapFlow,
            SaveImageFlow.StatusChangedEventHandler siStatusChangedHandler,
            Action<DMTResultGeneratedEventArgs> resultGeneratedHandler)
        {
            var siFlow = _saveImageFlowByAdjustCurvatureMapFlow[adjustCurvatureMapFlow];
            siFlow.StatusChanged += siStatusChangedHandler;
            var siFlowTask = new SaveImageFlowTask(siFlow, _cancellationTokenSource);
            var task = (SaveImageFlowTask)curvatureMapTask.CheckSuccessAndContinueWith(siFlowTask, previousAdjustCurvatureMapFlowTask =>
            {
                siFlow.Input.ImageDataToSave = previousAdjustCurvatureMapFlowTask.Result.CurvatureMap;
            });
            task.ContinueWithResultGeneratedHandlerInvocationIfNeeded(resultGeneratedHandler);
            task.ContinueWith(saveImageFlowTask =>
            {
                siFlow.StatusChanged -= siStatusChangedHandler;
            });
            return task;
        }

        private AdjustCurvatureDynamicsForRawCurvatureMapFlowTask CreateCurvatureMapContinuationTask(
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask computePhaseMapTask,
            ComputeRawCurvatureMapForPeriodAndDirectionFlow rawCurvatureMapFlow,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow adjustCurvatureMapFlow,
            ComputeRawCurvatureMapForPeriodAndDirectionFlow.StatusChangedEventHandler crcmStatusChangedHandler,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow.StatusChangedEventHandler acdStatusChangedHandler)
        {
            rawCurvatureMapFlow.StatusChanged += crcmStatusChangedHandler;
            adjustCurvatureMapFlow.StatusChanged += acdStatusChangedHandler;
            var computeRawCurvatureMapTask = computePhaseMapTask.CheckSuccessAndContinueWith(rawCurvatureMapFlow, previousComputePhaseMapTask =>
            {
                rawCurvatureMapFlow.Input.PhaseMapAndMask = previousComputePhaseMapTask.Result.PsdResult;
            });
            computeRawCurvatureMapTask.ContinueWith(previousComputeRawCurvatureMapTask =>
            {
                rawCurvatureMapFlow.StatusChanged -= crcmStatusChangedHandler;
            });
            var adjustCurvatureDynamicsTask = computeRawCurvatureMapTask.CheckSuccessAndContinueWith(adjustCurvatureMapFlow, previousRawCurvatureMapFlowTask =>
            {
                adjustCurvatureMapFlow.Input.RawCurvatureMap =
                    previousRawCurvatureMapFlowTask.Result.RawCurvatureMap;
                adjustCurvatureMapFlow.Input.Mask =
                    previousRawCurvatureMapFlowTask.Result.Mask;
            });
            adjustCurvatureDynamicsTask.ContinueWith(previousAdjustCurvatureDynamicsTask =>
            {
                adjustCurvatureMapFlow.StatusChanged -= acdStatusChangedHandler;
            });
            return adjustCurvatureDynamicsTask;
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask CreateFirstComputePhaseMapTask(
            KeyValuePair<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
                ComputePhaseMapAndMaskForPeriodAndDirectionFlow> cpmFlowByAcquisitionTaskKvPair,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlow.StatusChangedEventHandler cpmStatusChangedHandler)
        {
            var cpmFlow = cpmFlowByAcquisitionTaskKvPair.Value;
            cpmFlow.StatusChanged += cpmStatusChangedHandler;
            var computePhaseMapTask = cpmFlowByAcquisitionTaskKvPair.Key.CheckSuccessAndContinueWith(cpmFlow, task =>
            {
                cpmFlow.Input.PhaseImages = task.Result.TemporaryResults;
            });
            computePhaseMapTask.ContinueWith(previousComputePhaseMapTask =>
            {
                cpmFlow.StatusChanged -= cpmStatusChangedHandler;
            });

            return computePhaseMapTask;
        }

        private static
            Dictionary<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
                ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
            InitializePhaseMapFlowByAcquisitionTaskDictionaryFromComputePhaseMapFlowList(
                DMTDeflectometryAcquisitionFlowTask previousAcquisitionFlowTask,
                IEnumerable<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> computePhaseMapFlows)
        {
            return computePhaseMapFlows.Select(flow =>
                                       {
                                           int period = flow.Input.Period;
                                           var direction = flow.Input.FringesDisplacementDirection;
                                           return
                                               new KeyValuePair<AcquirePhaseImagesForPeriodAndDirectionFlowTask,
                                                   ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(
                                                previousAcquisitionFlowTask.AcquisitionTasksByPeriodAndDirection
                                                    [period][direction], flow);
                                       })
                                       .ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);
        }
    }
}
