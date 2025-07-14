using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    internal class DeflectometryMeasureExecution : MeasureExecutionWithAutoExposure
    {
        private readonly CameraBase _camera;
        private readonly Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> _fringeImageDict;
        private readonly ScreenBase _screen;
        private readonly CalibrationManager _calibrationManager;
        private DMTDeflectometryAcquisitionFlowTask _acquisitionFlowTask;

        public DeflectometryMeasureExecution(DeflectometryMeasure measure, ILogger logger, RecipeExecution rexec,
            bool productionMode, int toolId, int chamberId, Guid recipeKey, int recipeVersion, int productId,
            PMConfiguration pmConfiguration, DMTCameraManager cameraManager, CalibrationManager calibrationManager,
            DbRegisterAcquisitionServiceProxy dbRegisterAcqService, DeflectometryMeasureConfiguration measureConfiguration) :
            base(measure, logger, rexec, toolId, chamberId, recipeKey, recipeVersion, productId, pmConfiguration,
                 cameraManager, dbRegisterAcqService, measureConfiguration)
        {
            _camera = HardwareManager.CamerasBySide[Side];
            _screen = HardwareManager.ScreensBySide[Side];
            _fringeImageDict = FringeManager.GetFringeImageDict(Side, Measure.Fringe);
            _calibrationManager = calibrationManager;
            if (productionMode && Recipe.AreAcquisitionsSavedInDatabase)
            {
                if (Measure.Outputs.HasFlag(DeflectometryOutput.Curvature))
                {
                    var cxDmtResultType = Measure.Side == Side.Front ? DMTResult.CurvatureX_Front : DMTResult.CurvatureX_Back;
                    var cyDmtResultType = Measure.Side == Side.Front ? DMTResult.CurvatureY_Front : DMTResult.CurvatureY_Back;
                    AddPreRegisterFlowForResult(cxDmtResultType, $"CX ({Measure.Side})", "CX");
                    AddPreRegisterFlowForResult(cyDmtResultType, $"CY ({Measure.Side})", "CY");
                }

                if (Measure.Outputs.HasFlag(DeflectometryOutput.LowAngleDarkField))
                {
                    var darkDmtResultType = Measure.Side == Side.Front ? DMTResult.LowAngleDarkField_Front : DMTResult.LowAngleDarkField_Back;
                    AddPreRegisterFlowForResult(darkDmtResultType, $"Dark-field image ({Measure.Side})", "dark");
                }
            }
        }

        public new DeflectometryMeasure Measure => (DeflectometryMeasure)base.Measure;
        
        protected new DeflectometryMeasureConfiguration MeasureConfiguration => (DeflectometryMeasureConfiguration)base.MeasureConfiguration;

        public override int ComputeNumberOfAcquisitionSteps()
        {
            int steps = base.ComputeNumberOfAcquisitionSteps();

            // Flows are executed for each direction, hence the multiplication by 2
            steps += 2 * (Measure.Fringe.Periods.Count * Measure.Fringe.NbImagesPerDirection *
                     AcquirePhaseImagesForPeriodAndDirectionFlow.NumberOfStepsPerImage +
                     AcquirePhaseImagesForPeriodAndDirectionFlow.NumberOfImageIndependentSteps);

            return steps;
        }

        public override int ComputeNumberOfComputationSteps()
        {
            // Flows are executed for each direction, hence the multiplication by 2
            int steps = Measure.Fringe.Periods.Count * 2 * ComputePhaseMapAndMaskForPeriodAndDirectionFlow.MaximumNumberOfSteps;
            if (Measure.Outputs.HasFlag(DeflectometryOutput.Curvature))
            {
                steps += 2 * (ComputeRawCurvatureMapForPeriodAndDirectionFlow.MaximumNumberOfSteps +
                         AdjustCurvatureDynamicsForRawCurvatureMapFlow.MaximumNumberOfSteps);
            }

            if (Measure.Outputs.HasFlag(DeflectometryOutput.LowAngleDarkField))
            {
                steps += 2 * ComputeLowAngleDarkFieldImageFlow.MaximumNumberOfSteps;
            }

            if (Measure.Outputs.HasFlag(DeflectometryOutput.UnwrappedPhase))
            {
                steps += 2 * ComputeUnwrappedPhaseMapForDirectionFlow.MaximumNumberOfSteps;
            }

            if (Measure.Outputs.HasFlag(DeflectometryOutput.GlobalTopo))
            {
                steps += 5; //TODO Adjust
            }

            if (Measure.Outputs.HasFlag(DeflectometryOutput.NanoTopo))
            {
                steps += 5; //TODO Adjust
            }

            return steps;
        }

        public DMTDeflectometryAcquisitionFlowTask GetAcquisitionTask()
        {
            var acquisitionFlowList = Measure.Fringe.Periods.SelectMany(CreateXAndYAcquirePhaseImagesFlowsArrayForPeriod)
                                             .OrderBy(flow => flow.Input.FringesDisplacementDirection == FringesDisplacement.X ? 0 : 10)
                                             .ThenBy(flow => flow.Input.Period)
                                             .ToList();

            int largestPeriod = Measure.Fringe.Periods.Max();
            var autoExposureFlow = CreateAutoExposureFlowForAcquisition(largestPeriod);
            var key = new Key
            {
                MeasureName = Measure.MeasureName,
                Side = Side
            };
            switch (Measure.AutoExposureTimeTrigger)
            {
                case AutoExposureTimeTrigger.Never:
                    CalibratedExposureTime = Measure.ExposureTimeMs;
                    acquisitionFlowList.ForEach(flow => flow.Input.ExposureTimeMs = Measure.ExposureTimeMs);
                    _acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(acquisitionFlowList);
                    break;

                case AutoExposureTimeTrigger.OnFirstWaferOfLot:
                    if (!s_cache.TryGetValue(key, out double ExposureTimeMs))
                    {
                        _acquisitionFlowTask =
                            new DMTDeflectometryAcquisitionFlowTask(autoExposureFlow, acquisitionFlowList);
                        break;
                    }

                    acquisitionFlowList.ForEach(flow => flow.Input.ExposureTimeMs = ExposureTimeMs);
                    _acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(acquisitionFlowList);
                    break;

                case AutoExposureTimeTrigger.OnAllWafer:

                    _acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(autoExposureFlow, acquisitionFlowList);
                    break;

                default:
                    throw new
                        ArgumentException($"Invalid AutoExposureTimeTrigger enum value {Measure.AutoExposureTimeTrigger.ToString()}");
            }

            return _acquisitionFlowTask;
        }

        private IEnumerable<AcquirePhaseImagesForPeriodAndDirectionFlow>
            CreateXAndYAcquirePhaseImagesFlowsArrayForPeriod(int period)
        {
            var xPhaseAcquisitionFlow = CreateAcquirePhaseImageFlowForPeriodAndDirection(period, FringesDisplacement.X);
            var yPhaseAcquisitionFlow = CreateAcquirePhaseImageFlowForPeriodAndDirection(period, FringesDisplacement.Y);

            return new[] { xPhaseAcquisitionFlow, yPhaseAcquisitionFlow };
        }

        private AcquirePhaseImagesForPeriodAndDirectionFlow CreateAcquirePhaseImageFlowForPeriodAndDirection(
            int period, FringesDisplacement direction)
        {
            var phaseAcquisitionInput = new AcquirePhaseImagesForPeriodAndDirectionInput(Measure, period,
             direction);
            var phaseAcquisitionFlow = FlowsAreSimulated ? new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(phaseAcquisitionInput, HardwareManager, CameraManager, FringeManager)
                                                         : new AcquirePhaseImagesForPeriodAndDirectionFlow(phaseAcquisitionInput, HardwareManager, CameraManager, FringeManager);
            return phaseAcquisitionFlow;
        }

        private AutoExposureFlow CreateAutoExposureFlowForAcquisition(int largestPeriod)
        {
            // TODO : To check for mutli-period acquisitions
            var fringeImage =
                FringeManager.GetFringeImageDict(Measure.Side, Measure.Fringe)[FringesDisplacement.X][largestPeriod][0];
            var autoExposureInput = new AutoExposureInput(Measure, fringeImage);
            if ((Measure.Fringe.FringeType == Interface.Fringe.FringeType.Multi && Measure.Fringe.Periods.Max() > _screen.Height / 2) ||
                Measure.Outputs.HasFlag(DeflectometryOutput.GlobalTopo) ||
                Measure.Fringe.Period > _screen.Height / 2)
            {
                autoExposureInput.DisplayImageType = AcquisitionScreenDisplayImage.Color;
                autoExposureInput.Color = Color.FromRgb((byte)MeasureConfiguration.FringesMaxValue,
                                            (byte)MeasureConfiguration.FringesMaxValue,
                                            (byte)MeasureConfiguration.FringesMaxValue);
            }
            var autoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(autoExposureInput, HardwareManager, CameraManager)
                                                     : new AutoExposureFlow(autoExposureInput, HardwareManager, CameraManager);
            return autoExposureFlow;
        }

        public DMTDeflectometryCalculationFlowTask GetCalculationTask()
        {
            var computePhaseMapFlows =
                Measure.Fringe.Periods.SelectMany(CreateComputeXandYPhaseMapFlowsForPeriod).ToList();
            
            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(_acquisitionFlowTask, computePhaseMapFlows);

            int smallestPeriod = Measure.Fringe.Periods.Min();
            var sideOutputs = Recipe.Measures.Where(m => m.Side == Measure.Side && m.IsEnabled)
                .SelectMany(m => m.GetOutputTypes())
                .Distinct()
                .ToList();
            if (sideOutputs.Any())
            {
                AddSaveMaskFlowToComputationTask(sideOutputs, computationFlowTask);
            }
            
            if (Measure.Outputs.HasFlag(DeflectometryOutput.Curvature))
            {
                AddCurvatureMapFlowsToComputationTask(smallestPeriod, computationFlowTask);
            }

            if (Measure.Outputs.HasFlag(DeflectometryOutput.LowAngleDarkField))
            {
                AddLowAngleDarkFieldFlowsToComputationTask(smallestPeriod, computationFlowTask);
            }

            // TODO : UnwrappedPhase and Nanotopo both launch a ComputeUnwrapped, which is a long computation. Waiting for specs : cf task #8983
            if (Measure.Outputs.HasFlag(DeflectometryOutput.UnwrappedPhase))
            {
                AddUnwrappedPhaseFlowsToComputationTask(computationFlowTask);
            }
            
            if (Measure.Outputs.HasFlag(DeflectometryOutput.NanoTopo))
            {
                AddNanoTopoFlowsToComputationTask(computationFlowTask);
            }

            return computationFlowTask;
        }

        private void AddSaveMaskFlowToComputationTask(List<ResultType> sideOutputs, DMTDeflectometryCalculationFlowTask computationFlowTask)
        {
            var saveMaskInput = new SaveMaskInput
            {
                MaskSide = Measure.Side,
                AdaWriterLock = AdaWriterLock,
                RecipeResults = sideOutputs,
                AdaWriterForSide = AdaWriter,
                SaveFullPath = GetFullPathName("mask", Measure.Side),
            };
            var maskSaveFlow = FlowsAreSimulated ? new SaveMaskFlowDummy(saveMaskInput, CalibrationManager) : new SaveMaskFlow(saveMaskInput, CalibrationManager);
            computationFlowTask.AddSaveMaskFlow(maskSaveFlow);
        }

        private void AddNanoTopoFlowsToComputationTask(DMTDeflectometryCalculationFlowTask computationFlowTask)
        {
            var computeUnwrappedPhaseMapXFlow = CreateComputeUnwrappedPhaseMapFlowForDirection(FringesDisplacement.X);
            computeUnwrappedPhaseMapXFlow.Input.IsNeededForSlopeMaps = false;
            computeUnwrappedPhaseMapXFlow.Input.IsNeededForTopography = true;
            var computeUnwrappedPhaseMapYFlow = CreateComputeUnwrappedPhaseMapFlowForDirection(FringesDisplacement.Y);
            computeUnwrappedPhaseMapYFlow.Input.IsNeededForSlopeMaps = false;
            computeUnwrappedPhaseMapYFlow.Input.IsNeededForTopography = true;

            var computeUnwrappedPhaseMapFlowForNanoTopo = new List<ComputeUnwrappedPhaseMapForDirectionFlow>
            {
                computeUnwrappedPhaseMapXFlow,
                computeUnwrappedPhaseMapYFlow,
            };

            var nanoTopoFlow = CreateComputeNanoTopoFlow();
            var nanoTopoSaveImageFlow = CreateNanoTopoSaveImageFlow();
            computationFlowTask.AddNanoTopoFlows(computeUnwrappedPhaseMapFlowForNanoTopo, nanoTopoFlow, nanoTopoSaveImageFlow);
        }

        private void AddUnwrappedPhaseFlowsToComputationTask(DMTDeflectometryCalculationFlowTask computationFlowTask)
        {
            var computeUnwrappedPhaseMapXFlow = CreateComputeUnwrappedPhaseMapFlowForDirection(FringesDisplacement.X);
            computeUnwrappedPhaseMapXFlow.Input.IsNeededForSlopeMaps = true;
            var computeUnwrappedPhaseMapYFlow = CreateComputeUnwrappedPhaseMapFlowForDirection(FringesDisplacement.Y);
            computeUnwrappedPhaseMapYFlow.Input.IsNeededForSlopeMaps = true;
            var saveImageUnwrappedPhaseMapXFlow = CreateUnwrappedPhaseMapSaveImageFlowForDirection(FringesDisplacement.X);
            var saveImageUnwrappedPhaseMapYFlow = CreateUnwrappedPhaseMapSaveImageFlowForDirection(FringesDisplacement.Y);

            var saveImageFlowByComputeUnwrappedPhaseMapFlow = new Dictionary<ComputeUnwrappedPhaseMapForDirectionFlow, SaveImageFlow>
            {
                {computeUnwrappedPhaseMapXFlow, saveImageUnwrappedPhaseMapXFlow },
                {computeUnwrappedPhaseMapYFlow, saveImageUnwrappedPhaseMapYFlow },
            };
            computationFlowTask.AddUnwrappedPhaseMapsFlows(saveImageFlowByComputeUnwrappedPhaseMapFlow);
        }

        private void AddLowAngleDarkFieldFlowsToComputationTask(int smallestPeriod,
            DMTDeflectometryCalculationFlowTask computationFlowTask)
        {
            var lowAngleDarkFieldImageFlow = CreateComputeLowAngleDarkFieldImageFlowForPeriod(smallestPeriod);
            var lowAngleDarkFieldSaveImageFlow = CreateLowAngleDarkFieldSaveImageFlow();
            computationFlowTask.AddLowAngleDarkFieldFlows(lowAngleDarkFieldImageFlow, lowAngleDarkFieldSaveImageFlow);
        }

        private void AddCurvatureMapFlowsToComputationTask(int smallestPeriod,
            DMTDeflectometryCalculationFlowTask computationFlowTask)
        {
            var computeRawCurvatureMapXFlow = CreateComputeRawCurvatureMapFlowForPeriodAndDirection(smallestPeriod, FringesDisplacement.X);
            var computeRawCurvatureMapYFlow = CreateComputeRawCurvatureMapFlowForPeriodAndDirection(smallestPeriod, FringesDisplacement.Y);
            var adjustXCurvatureMapFlow = CreateAdjustCurvatureDynamicsFlowForDirection(FringesDisplacement.X);
            var adjustYCurvatureMapFlow = CreateAdjustCurvatureDynamicsFlowForDirection(FringesDisplacement.Y);

            var xCurvatureMapSaveImageFlow = CreateCurvatureMapSaveImageFlowForDirection(FringesDisplacement.X);
            var yCurvatureMapSaveImageFlow = CreateCurvatureMapSaveImageFlowForDirection(FringesDisplacement.Y);

            var adjustCurvatureMapFlowByComputeRawCurvatureMapFlow = new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow, AdjustCurvatureDynamicsForRawCurvatureMapFlow>
            {
                { computeRawCurvatureMapXFlow, adjustXCurvatureMapFlow },
                { computeRawCurvatureMapYFlow, adjustYCurvatureMapFlow }
            };
            var curvatureSaveImageFlowsByAdjustCurvatureMapFlow = new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
            {
                { adjustXCurvatureMapFlow, xCurvatureMapSaveImageFlow },
                { adjustYCurvatureMapFlow, yCurvatureMapSaveImageFlow }
            };
            computationFlowTask.AddCurvatureMapFlows(adjustCurvatureMapFlowByComputeRawCurvatureMapFlow, curvatureSaveImageFlowsByAdjustCurvatureMapFlow);
        }

        private SaveImageFlow CreateUnwrappedPhaseMapSaveImageFlowForDirection(FringesDisplacement direction)
        {
            DMTResult dmtresultType;
            switch (Side)
            {
                case Side.Front:
                    dmtresultType = direction == FringesDisplacement.X ? DMTResult.UnwrappedPhaseX_Front : DMTResult.UnwrappedPhaseY_Front;
                    break;

                case Side.Back:
                    dmtresultType = direction == FringesDisplacement.X ? DMTResult.UnwrappedPhaseX_Back : DMTResult.UnwrappedPhaseY_Back;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }

            var imageName = $"Slope{direction}";
            var saveFullPath = GetFullPathName(imageName, Measure.Side);
            var saveImageUnwrappedPhaseMapXInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock, dmtresultType, imageName, saveFullPath, keep32BitsDepth: true);
            var saveImageUnwrappedPhaseMapXFlow = FlowsAreSimulated ? new SaveImageFlowDummy(saveImageUnwrappedPhaseMapXInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                                                    : new SaveImageFlow(saveImageUnwrappedPhaseMapXInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService);
            return saveImageUnwrappedPhaseMapXFlow;
        }

        private ComputeUnwrappedPhaseMapForDirectionFlow CreateComputeUnwrappedPhaseMapFlowForDirection(FringesDisplacement direction)
        {
            var computeUnwrappedPhaseMapInput = new ComputeUnwrappedPhaseMapForDirectionInput(Measure, direction);
            return FlowsAreSimulated ? new ComputeUnwrappedPhaseMapForDirectionFlowDummy(computeUnwrappedPhaseMapInput)
                                     : new ComputeUnwrappedPhaseMapForDirectionFlow(computeUnwrappedPhaseMapInput);
        }

        private ComputeNanoTopoFlow CreateComputeNanoTopoFlow()
        {
            var computeNanoTopoFlowInput = new ComputeNanoTopoInput()
            {
                Side = Measure.Side,
                ScreenPixelSize = _screen.PixelPitchHorizontal,
                Periods = Measure.Fringe.Periods
            };
            return FlowsAreSimulated
                ? new ComputeNanoTopoFlowDummy(computeNanoTopoFlowInput, CalibrationManager)
                : new ComputeNanoTopoFlow(computeNanoTopoFlowInput, CalibrationManager);
        }

        private SaveImageFlow CreateNanoTopoSaveImageFlow()
        {
            // TODO : return null since nanotopo algorithm is not fully implem
            return null;
        }

        private IEnumerable<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> CreateComputeXandYPhaseMapFlowsForPeriod(
            int period)
        {
            var perspectiveCalibration = _calibrationManager.GetPerspectiveCalibrationForSide(Measure.Side).Clone();
            var computePhaseMapXInput =
                new ComputePhaseMapAndMaskForPeriodAndDirectionInput(Measure, period, FringesDisplacement.X)
                {
                    WaferDiameter = Recipe.Step.Product.WaferCategory.DimentionalCharacteristic.Diameter,
                    PerspectiveCalibration = perspectiveCalibration,
                    UseEnhancedMask = Measure.UseEnhancedMask                    
                };
            var computePhaseMapXFlow = FlowsAreSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(computePhaseMapXInput)
                                                         : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(computePhaseMapXInput);

            var computePhaseMapYInput =
                new ComputePhaseMapAndMaskForPeriodAndDirectionInput(Measure, period, FringesDisplacement.Y)
                {
                    WaferDiameter = Recipe.Step.Product.WaferCategory.DimentionalCharacteristic.Diameter,
                    PerspectiveCalibration = perspectiveCalibration,
                    UseEnhancedMask = Measure.UseEnhancedMask
                };
            var computePhaseMapYFlow = FlowsAreSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(computePhaseMapYInput)
                                                         : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(computePhaseMapYInput);
            return new[] { computePhaseMapXFlow, computePhaseMapYFlow };
        }

        private ComputeLowAngleDarkFieldImageFlow CreateComputeLowAngleDarkFieldImageFlowForPeriod(int smallestPeriod)
        {
            ComputeLowAngleDarkFieldImageFlow lowAngleDarkFieldImageFlow;
            var computeDarkImageInput = new ComputeLowAngleDarkFieldImageInput(Measure, smallestPeriod);
            lowAngleDarkFieldImageFlow = FlowsAreSimulated ? new ComputeLowAngleDarkFieldImageFlowDummy(computeDarkImageInput)
                                              : new ComputeLowAngleDarkFieldImageFlow(computeDarkImageInput);
            return lowAngleDarkFieldImageFlow;
        }

        private SaveImageFlow CreateCurvatureMapSaveImageFlowForDirection(FringesDisplacement direction)
        {
            DMTResult dmtResType;
            switch (direction)
            {
                case FringesDisplacement.X:
                    dmtResType = (Side == Side.Front) ? DMTResult.CurvatureX_Front : DMTResult.CurvatureX_Back;
                    break;

                case FringesDisplacement.Y:
                    dmtResType = (Side == Side.Front) ? DMTResult.CurvatureY_Front : DMTResult.CurvatureY_Back;
                    break;

                default:
                    throw new ArgumentException($"Unknown fringe displacement type given {direction}");
            }

            var imageName = $"C{Enum.GetName(typeof(FringesDisplacement), direction)}";
            var saveFullPath = GetFullPathName(imageName, Side);
            var curvatureMapSaveImageInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock, dmtResType, imageName, saveFullPath);
            if (PreRegisterFlowByResultType.TryGetValue(dmtResType, out var preRegisterFlow))
            {
                curvatureMapSaveImageInput.InternalDbResItemId = preRegisterFlow.Result.OutPreRegister.InternalDBResItemId;
            }
            var curvatureMapSaveImageFlow =
                FlowsAreSimulated ? new SaveImageFlowDummy(curvatureMapSaveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                  : new SaveImageFlow(curvatureMapSaveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService);
            return curvatureMapSaveImageFlow;
        }

        private SaveImageFlow CreateLowAngleDarkFieldSaveImageFlow()
        {
            var dmtResType = Side == Side.Front ? DMTResult.LowAngleDarkField_Front : DMTResult.LowAngleDarkField_Back;
            var savePath = GetFullPathName("dark", Side);
            var darkSaveImageInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock, dmtResType, "dark", savePath);
            if (PreRegisterFlowByResultType.TryGetValue(dmtResType, out var preRegisterFlow))
            {
                darkSaveImageInput.InternalDbResItemId = preRegisterFlow.Result.OutPreRegister.InternalDBResItemId;
            }
            return FlowsAreSimulated ? new SaveImageFlowDummy(darkSaveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                     : new SaveImageFlow(darkSaveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService);
        }

        private AdjustCurvatureDynamicsForRawCurvatureMapFlow CreateAdjustCurvatureDynamicsFlowForDirection(FringesDisplacement direction)
        {
            var adjustXCurvatureMapInput = new AdjustCurvatureDynamicsForRawCurvatureMapInput(Measure, direction,
                (float)(_calibrationManager.GetCurvatureDynamicsCalibrationBySide(Side)?.DynamicsCoefficient ?? 0f));
            var adjustXCurvatureMapFlow = FlowsAreSimulated ? new AdjustCurvatureDynamicsForRawCurvatureMapFlowDummy(adjustXCurvatureMapInput)
                                                            : new AdjustCurvatureDynamicsForRawCurvatureMapFlow(adjustXCurvatureMapInput);
            return adjustXCurvatureMapFlow;
        }

        private ComputeRawCurvatureMapForPeriodAndDirectionFlow CreateComputeRawCurvatureMapFlowForPeriodAndDirection(
            int smallestPeriod, FringesDisplacement direction)
        {
            var crcmXInput = new ComputeRawCurvatureMapForPeriodAndDirectionInput(Measure, smallestPeriod, direction);
            var computeRawCurvatureMapXFlow = FlowsAreSimulated ? new ComputeRawCurvatureMapForPeriodAndDirectionFlowDummy(crcmXInput)
                                                                : new ComputeRawCurvatureMapForPeriodAndDirectionFlow(crcmXInput);

            return computeRawCurvatureMapXFlow;
        }

        protected void DisplayFringe(FringesDisplacement direction, int period, int imageNumber)
        {
            _screen.DisplayImage(_fringeImageDict[direction][period][imageNumber]);
        }

        protected override void SetScreenForAutoExposure()
        {
            // If we have a multi period measure or a period >  _screen.Height/2
            // Assuming NanoTopo and GlobalTopo are performed with multi-smallestPeriod
            if ((Measure.Fringe.FringeType == Interface.Fringe.FringeType.Multi && Measure.Fringe.Periods.Max() > _screen.Height / 2) ||
                Measure.Outputs.HasFlag(DeflectometryOutput.GlobalTopo) ||
                Measure.Fringe.Period > _screen.Height / 2)
                _screen.ClearAsync(
                              Color.FromRgb((byte)MeasureConfiguration.FringesMaxValue,
                                            (byte)MeasureConfiguration.FringesMaxValue,
                                            (byte)MeasureConfiguration.FringesMaxValue));
            else
            {
                int smallestPeriod = Measure.Fringe.Periods.Min();
                DisplayFringe(FringesDisplacement.X, smallestPeriod, 0);
            }
        }

        protected override void SetFixedExposureTime(double expo)
        {
            ActualExposureTimeMs = CameraManager.SetExposureTime(_camera, expo);
            Logger.Information($"calibrated={CalibratedExposureTime:0.000}ms actual={ActualExposureTimeMs:0.000}ms");
        }
    }
}
