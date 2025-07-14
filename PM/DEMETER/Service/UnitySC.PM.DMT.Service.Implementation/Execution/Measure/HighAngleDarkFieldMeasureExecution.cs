using System;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    internal class HighAngleDarkFieldMeasureExecution : MeasureExecutionWithAutoExposure
    {
        private readonly USPImageMil _maskImage;
        private readonly MilGraphicsContext _milGC;
        private readonly USPImageMil _screenimage;
        private readonly IDMTScreenService _screenService;

        public HighAngleDarkFieldMeasureExecution(HighAngleDarkFieldMeasure measure, ILogger logger, RecipeExecution rexec,
            bool productionMode, int toolId, int chamberId, Guid recipeKey, int recipeVersion, int productId,
            PMConfiguration pmConfiguration, IDMTScreenService screenService, DMTCameraManager cameraManager,
            DbRegisterAcquisitionServiceProxy dbRegisterResultServiceProxy, HighAngleDarkFieldMeasureConfiguration measureConfiguration) :
            base(measure, logger, rexec, toolId, chamberId, recipeKey, recipeVersion, productId, pmConfiguration,
                cameraManager, dbRegisterResultServiceProxy, measureConfiguration)
        {
            _screenService = screenService;
            DmtResultType = Measure.Side == Side.Front ? DMTResult.HighAngleDarkField_Front : DMTResult.HighAngleDarkField_Back;
            if (productionMode && Recipe.AreAcquisitionsSavedInDatabase)
            {
                AddPreRegisterFlowForResult(DmtResultType, $"High angle dark-field ({Measure.Side})");
            }
        }

        public new HighAngleDarkFieldMeasure Measure => (HighAngleDarkFieldMeasure)base.Measure;
        
        protected new HighAngleDarkFieldMeasureConfiguration MeasureConfiguration => (HighAngleDarkFieldMeasureConfiguration)base.MeasureConfiguration;

        public override int ComputeNumberOfAcquisitionSteps()
        {
            int steps = base.ComputeNumberOfAcquisitionSteps();
            steps += AcquireOneImageFlow.MaximumNumberOfSteps;
            return steps;
        }

        public override int ComputeNumberOfComputationSteps()
        {
            return 0;
        }

        protected override void SetScreenForAutoExposure()
        {
            _screenService.DisplayHighAngleDarkFielMaskdOnSide(Side, Colors.White);
        }

        public DMTSingleAcquisitionFlowTask GetDMTAcquisitionFlowTask()
        {
            var highAngleDarkFieldMask = CalibrationManager.GetHighAngleDarkFieldMaskForSide(Measure.Side) ?? throw new Exception("Unable to retrieve high angle dark-field mask.");
            var acquisitionInput = new AcquireOneImageInput(Measure, highAngleDarkFieldMask);
            var acquisitionFlow = new AcquireOneImageFlow(acquisitionInput, ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>(), HardwareManager);
            var savePath = GetFullPathName(Measure.MeasureName.ToLower(), Side);
            var saveImageInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock, DmtResultType, Measure.MeasureName.ToLower(), savePath);
            if (PreRegisterFlowByResultType.TryGetValue(DmtResultType, out var preRegisterFlow))
            {
                saveImageInput.InternalDbResItemId = preRegisterFlow.Result.OutPreRegister.InternalDBResItemId;
            }
            var saveImageFlow = FlowsAreSimulated ? new SaveImageFlowDummy(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                                  : new SaveImageFlow(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService);
            var key = new Key { MeasureName = Measure.MeasureName, Side = Side };

            switch (Measure.AutoExposureTimeTrigger)
            {
                case AutoExposureTimeTrigger.Never:
                    CalibratedExposureTime = Measure.ExposureTimeMs;
                    acquisitionInput.ExposureTimeMs = Measure.ExposureTimeMs;
                    return new DMTSingleAcquisitionFlowTask(acquisitionFlow, saveImageFlow);

                case AutoExposureTimeTrigger.OnFirstWaferOfLot:
                    if (!s_cache.TryGetValue(key, out double ExposureTimeMs))
                    {
                        var firstWaferAutoExposureInput = new AutoExposureInput(Measure, highAngleDarkFieldMask);
                        var firstWaferAutoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(firstWaferAutoExposureInput, HardwareManager, CameraManager)
                                                                           : new AutoExposureFlow(firstWaferAutoExposureInput, HardwareManager, CameraManager);
                        return new DMTSingleAcquisitionFlowTask(firstWaferAutoExposureFlow, acquisitionFlow, saveImageFlow);
                    }

                    acquisitionInput.ExposureTimeMs = ExposureTimeMs;
                    return new DMTSingleAcquisitionFlowTask(acquisitionFlow, saveImageFlow);

                case AutoExposureTimeTrigger.OnAllWafer:
                    var autoExposureInput = new AutoExposureInput(Measure, highAngleDarkFieldMask);
                    var autoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(autoExposureInput, HardwareManager, CameraManager)
                                                             : new AutoExposureFlow(autoExposureInput, HardwareManager, CameraManager);
                    return new DMTSingleAcquisitionFlowTask(autoExposureFlow, acquisitionFlow, saveImageFlow);

                default:
                    throw new ArgumentException(
                        $"Invalid AutoExposureTimeTrigger enum value {Measure.AutoExposureTimeTrigger.ToString()}");
            }
        }
    }
}
