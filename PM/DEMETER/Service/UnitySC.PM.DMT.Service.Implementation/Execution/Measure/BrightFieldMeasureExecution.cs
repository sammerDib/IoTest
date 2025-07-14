using System;

using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Corrector;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    internal class BrightFieldMeasureExecution : MeasureExecutionWithAutoExposure
    {
        private DMTSingleAcquisitionFlowTask _acqisitionFlowTask;
        public bool IsMeasureUsedForCorrector = false;
        private AutoExposureConfiguration _autoExposureConfiguration;

        public BrightFieldMeasureExecution(BrightFieldMeasure measure, ILogger logger, RecipeExecution rexec,
            bool productionMode, int toolId, int chamberId, Guid recipeKey, int recipeVersion, int productId,
            PMConfiguration pmConfiguration, DMTCameraManager cameraManager, DbRegisterAcquisitionServiceProxy dbRegisterAcqService, BrightFieldMeasureConfiguration measureConfiguration, AutoExposureConfiguration autoExposureConfiguration) :
            base(measure, logger, rexec, toolId, chamberId, recipeKey, recipeVersion, productId, pmConfiguration,
                 cameraManager, dbRegisterAcqService, measureConfiguration)
        {
            DmtResultType = Measure.Side == Side.Front ? DMTResult.BrightField_Front : DMTResult.BrightField_Back;
            if (productionMode && Recipe.AreAcquisitionsSavedInDatabase)
            {
                AddPreRegisterFlowForResult(DmtResultType, $"Bright-field ({Measure.Side})");
            }
            _autoExposureConfiguration = autoExposureConfiguration;
        }

        public new BrightFieldMeasure Measure => (BrightFieldMeasure)base.Measure;
        
        protected new BrightFieldMeasureConfiguration MeasureConfiguration => (BrightFieldMeasureConfiguration)base.MeasureConfiguration;

        public override int ComputeNumberOfAcquisitionSteps()
        {
            int steps = base.ComputeNumberOfAcquisitionSteps();
            steps += AcquireOneImageFlow.MaximumNumberOfSteps;

            return steps;
        }

        public override int ComputeNumberOfComputationSteps()
        {
            return CorrectorFlow.MaximumNumberOfSteps;
        }

        protected override void SetScreenForAutoExposure()
        {
        }

        public DMTSingleAcquisitionFlowTask GetDMTAcquisitionFlowTask()
        {
            var acquisitionInput = new AcquireOneImageInput(Measure);
            var acquisitionFlow = new AcquireOneImageFlow(acquisitionInput, CameraManager, HardwareManager);
            var saveFullPath = GetFullPathName(Measure.MeasureName.ToLower(), Measure.Side);
            bool keep32BitsDepth = false;
            var defaultAutoExposureConfiguration =
                _autoExposureConfiguration.GetDefaultAutoExposureSettingForMeasure(Measure);
            var saveImageInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock,
                DmtResultType, Measure.MeasureName.ToLower(), saveFullPath, keep32BitsDepth,
                Measure.ApplyUniformityCorrection, defaultAutoExposureConfiguration.TargetSaturation,
                (float)defaultAutoExposureConfiguration.RatioSaturated);

            if (PreRegisterFlowByResultType.TryGetValue(DmtResultType, out var preRegisterFlow))
            {
                saveImageInput.InternalDbResItemId = preRegisterFlow.Result.OutPreRegister.InternalDBResItemId;
            }
            var saveImageFlow =
                FlowsAreSimulated ? new SaveImageFlowDummy(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                  : new SaveImageFlow(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService);
            var key = new Key { MeasureName = Measure.MeasureName, Side = Side };

            switch (Measure.AutoExposureTimeTrigger)
            {
                case AutoExposureTimeTrigger.Never:
                    CalibratedExposureTime = Measure.ExposureTimeMs;
                    acquisitionInput.ExposureTimeMs = Measure.ExposureTimeMs;
                    if (IsMeasureUsedForCorrector)
                    {
                        _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(acquisitionFlow);
                    }
                    else
                    {
                        _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(acquisitionFlow, saveImageFlow);
                    }

                    break;

                case AutoExposureTimeTrigger.OnFirstWaferOfLot:
                    if (!s_cache.TryGetValue(key, out double exposureTimeMs))
                    {
                        var firstWaferAutoExposureInput = new AutoExposureInput(Measure, null);
                        var firstWaferAutoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(firstWaferAutoExposureInput, HardwareManager, CameraManager)
                                                                           : new AutoExposureFlow(firstWaferAutoExposureInput, HardwareManager, CameraManager);
                        if (IsMeasureUsedForCorrector)
                        {
                            _acqisitionFlowTask =
                                new DMTSingleAcquisitionFlowTask(firstWaferAutoExposureFlow, acquisitionFlow);
                        }
                        else
                        {
                            _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(firstWaferAutoExposureFlow,
                                acquisitionFlow, saveImageFlow);
                        }
                    }
                    else
                    {
                        acquisitionInput.ExposureTimeMs = exposureTimeMs;
                        if (IsMeasureUsedForCorrector)
                        {
                            _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(acquisitionFlow);
                        }
                        else
                        {
                            _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(acquisitionFlow, saveImageFlow);
                        }
                    }

                    break;

                case AutoExposureTimeTrigger.OnAllWafer:
                    var autoExposureInput = new AutoExposureInput(Measure, null);

                    var autoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(autoExposureInput, HardwareManager, CameraManager)
                                                             : new AutoExposureFlow(autoExposureInput, HardwareManager, CameraManager);
                    if (IsMeasureUsedForCorrector)
                    {
                        _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(autoExposureFlow, acquisitionFlow);
                    }
                    else
                    {
                        _acqisitionFlowTask = new DMTSingleAcquisitionFlowTask(autoExposureFlow, acquisitionFlow, saveImageFlow);
                    }

                    break;

                default:
                    throw new ArgumentException(
                        $"Invalid AutoExposureTimeTrigger enum value {Measure.AutoExposureTimeTrigger.ToString()}");
            }

            return _acqisitionFlowTask;
        }

        public DMTCorrectorFlowTask GetDMTCalculationFlowTask()
        {
            var calibration = CalibrationManager.GetPerspectiveCalibrationForSide(Measure.Side);
            if (!(calibration is null))
            {
                var correctorInput = new CorrectorInput(Rexec.Recipe.Step.Product.WaferCategory.DimentionalCharacteristic,
                    calibration.Clone(), Measure.Side);
                var correctorFlow = FlowsAreSimulated ? new CorrectorFlowDummy(correctorInput) : new CorrectorFlow(correctorInput);
                return new DMTCorrectorFlowTask(correctorFlow);
            }

            return null;
        }
    }
}
