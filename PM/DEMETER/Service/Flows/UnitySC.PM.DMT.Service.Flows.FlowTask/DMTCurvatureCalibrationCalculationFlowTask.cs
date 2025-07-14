using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Flows.Calibration;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

using AcquirePhaseImagesForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionConfiguration>;
using AdjustCurvatureDynamicsForRawCurvatureMapFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AdjustCurvatureDynamicsForRawCurvatureMapConfiguration>;
using ComputeDarkImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
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
using ComputeRawCurvatureMapForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeRawCurvatureMapForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeRawCurvatureMapForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeRawCurvatureMapForPeriodAndDirectionConfiguration>;
using ComputeUnwrappedPhasesForDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.ComputeUnwrappedPhaseMapForDirectionConfiguration>;
using CurvatureDynamicsCalibrationFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.CurvatureDynamicsCalibrationInput,
    UnitySC.PM.DMT.Service.Interface.Flow.CurvatureDynamicsCalibrationResult,
    UnitySC.PM.DMT.Service.Interface.Flow.CurvatureDynamicsCalibrationConfiguration>;
using SaveImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageResult,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageConfiguration>;

namespace UnitySC.PM.DMT.Service.Flows.FlowTask
{
    public class DMTCurvatureCalibrationCalculationFlowTask
    {
        public CurvatureDynamicsCalibrationFlowTask LastComputationTask;

        private readonly DMTDeflectometryAcquisitionFlowTask _acquisitionFlowTask;

        private readonly Dictionary<FringesDisplacement, ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
            _computePhaseMapByDirection;

        private readonly Dictionary<FringesDisplacement, ComputeRawCurvatureMapForPeriodAndDirectionFlow>
            _computeRawCurvatureMapByDirection;

        private readonly CurvatureDynamicsCalibrationFlow _curvatureDynamicsCalibrationFlow;

        internal CancellationTokenSource CancellationTokenSource;

        private readonly int _period;

        public DMTCurvatureCalibrationCalculationFlowTask(
            DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask,
            IEnumerable<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> computePhaseMapFlows,
            IEnumerable<ComputeRawCurvatureMapForPeriodAndDirectionFlow> computeRawCurvatureMapFlows,
            CurvatureDynamicsCalibrationFlow curvatureDynamicsCalibrationFlow)
        {
            _acquisitionFlowTask = acquisitionFlowTask;
            CancellationTokenSource = acquisitionFlowTask.CancellationTokenSource;
            _period = _acquisitionFlowTask.AcquisitionTasksByPeriodAndDirection.Keys.First();
            _computePhaseMapByDirection = computePhaseMapFlows
                                          .Select(cpmFlow =>
                                                      new KeyValuePair<FringesDisplacement,
                                                          ComputePhaseMapAndMaskForPeriodAndDirectionFlow>(cpmFlow.Input.FringesDisplacementDirection,
                                                       cpmFlow))
                                          .ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);
            _computeRawCurvatureMapByDirection = computeRawCurvatureMapFlows
                                                 .Select(crcmFlow =>
                                                             new KeyValuePair<FringesDisplacement,
                                                                 ComputeRawCurvatureMapForPeriodAndDirectionFlow>(crcmFlow.Input.FringesDisplacementDirection,
                                                              crcmFlow))
                                                 .ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);
            _curvatureDynamicsCalibrationFlow = curvatureDynamicsCalibrationFlow;
        }

        public void CreateAndChainComputationContinuationTasks()
        {
            var lastXAcquisitionTask =
                _acquisitionFlowTask.AcquisitionTasksByPeriodAndDirection[_period][FringesDisplacement.X];
            var lastYAcquisitionTask =
                _acquisitionFlowTask.AcquisitionTasksByPeriodAndDirection[_period][FringesDisplacement.Y];
            var cpmXFlow = _computePhaseMapByDirection[FringesDisplacement.X];
            var cpmYFlow = _computePhaseMapByDirection[FringesDisplacement.Y];
            var cpmXFlowTask = CrateComputePhaseMapFlowTask(lastXAcquisitionTask, cpmXFlow);
            var crcmXFlow = _computeRawCurvatureMapByDirection[FringesDisplacement.X];
            var crcmYFlow = _computeRawCurvatureMapByDirection[FringesDisplacement.Y];
            var crcmXFlowTask = CreateComputeRawCurvatureMapTask(cpmXFlowTask, crcmXFlow);
            var cpmYFlowTaskPreviousTaskArray = new IFlowTask[] { lastYAcquisitionTask, crcmXFlowTask };
            var cpmYFlowTask = CreateComputePhaseMapTaskFromAcquisitionAndPreviousCurvatureMap(cpmYFlowTaskPreviousTaskArray, cpmYFlow);
            var crcmYFlowTask = CreateComputeRawCurvatureMapTask(cpmYFlowTask, crcmYFlow);
            var cdccPreviousFlowTasks = new[]
                                        {
                                            crcmXFlowTask,
                                            crcmYFlowTask
                                        };
            LastComputationTask = CreateCurvatureCalibrationCoefficientComputationTask(cdccPreviousFlowTasks);
        }

        private CurvatureDynamicsCalibrationFlowTask CreateCurvatureCalibrationCoefficientComputationTask(ComputeRawCurvatureMapForPeriodAndDirectionFlowTask[] cdccPreviousFlowTasks)
        {
            return CurvatureDynamicsCalibrationFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(cdccPreviousFlowTasks,
                _curvatureDynamicsCalibrationFlow, _acquisitionFlowTask.CancellationTokenSource, previousCRCMFlowTasks =>
                {
                    var previousFlowTasks = previousCRCMFlowTasks
                        .Cast<ComputeRawCurvatureMapForPeriodAndDirectionFlowTask>().ToArray();
                    _curvatureDynamicsCalibrationFlow.Input.XRawCurvatureMap =
                        previousFlowTasks[0].Result.RawCurvatureMap;
                    _curvatureDynamicsCalibrationFlow.Input.YRawCurvatureMap =
                        previousFlowTasks[1].Result.RawCurvatureMap;
                    _curvatureDynamicsCalibrationFlow.Input.CurvatureMapMask = previousFlowTasks[0].Result.Mask;
                }); 
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask
            CreateComputePhaseMapTaskFromAcquisitionAndPreviousCurvatureMap(
                IFlowTask[] cpmFlowTaskPreviousTaskArray, ComputePhaseMapAndMaskForPeriodAndDirectionFlow cpmYlow)
        {
            return ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask.CheckPreviousFlowTasksSuccessAndContinueWhenAll(cpmFlowTaskPreviousTaskArray, cpmYlow, _acquisitionFlowTask.CancellationTokenSource, previousTaskArray =>
            {
                cpmYlow.Input.PhaseImages = ((AcquirePhaseImagesForPeriodAndDirectionFlowTask)previousTaskArray[0]).Result.TemporaryResults;
            });
        }

        private ComputeRawCurvatureMapForPeriodAndDirectionFlowTask CreateComputeRawCurvatureMapTask(
            ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask cpmFlowTask,
            ComputeRawCurvatureMapForPeriodAndDirectionFlow crcmFlow)
        {
            return cpmFlowTask.CheckSuccessAndContinueWith(crcmFlow, previousCPMFlowTask =>
            {
                crcmFlow.Input.PhaseMapAndMask = previousCPMFlowTask.Result.PsdResult;
            });
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlowTask CrateComputePhaseMapFlowTask(
            AcquirePhaseImagesForPeriodAndDirectionFlowTask lastAcquisitionTask,
            ComputePhaseMapAndMaskForPeriodAndDirectionFlow cpmFlow)
        {
            return lastAcquisitionTask.CheckSuccessAndContinueWith(cpmFlow, previousAPIFlowTask =>
            {
                cpmFlow.Input.PhaseImages = previousAPIFlowTask.Result.TemporaryResults;
            });
        }
    }
}
