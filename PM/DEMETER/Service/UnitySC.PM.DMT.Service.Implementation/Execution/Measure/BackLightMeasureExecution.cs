using System;

using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    internal class BackLightMeasureExecution : MeasureExecutionBase
    {
        private readonly CameraBase _camera;

        public BackLightMeasureExecution(BackLightMeasure measure, ILogger logger, RecipeExecution rexec,
            bool productionMode, int toolId, int chamberId, Guid recipeKey, int recipeVersion, int productId,
            PMConfiguration pmConfiguration, DMTCameraManager cameraManager, DbRegisterAcquisitionServiceProxy dbRegisterAcqService, BackLightMeasureConfiguration measureConfiguration) :
            base(measure, logger, rexec, toolId, chamberId, recipeKey, recipeVersion, productId, pmConfiguration, cameraManager,
                dbRegisterAcqService, measureConfiguration)
        {
            _camera = HardwareManager.CamerasBySide[Side.Front];
            DmtResultType = Measure.Side == Side.Front ? DMTResult.TopoBackLight_Front : DMTResult.TopoBackLight_Back;
            if (productionMode && Recipe.AreAcquisitionsSavedInDatabase)
            {
                AddPreRegisterFlowForResult(DmtResultType, $"Backlight ({Measure.Side})");
            }
        }

        public new BackLightMeasure Measure => (BackLightMeasure)base.Measure;
        
        protected new BackLightMeasureConfiguration MeasureConfiguration => (BackLightMeasureConfiguration)base.MeasureConfiguration;

        public override int ComputeNumberOfAcquisitionSteps()
        {
            return AcquireOneImageFlow.MaximumNumberOfSteps;
        }

        public override int ComputeNumberOfComputationSteps()
        {
            return 0;
        }

        public DMTSingleAcquisitionFlowTask GetDMTAcquisitionFlowTask()
        {
            var acquisitionInput = new AcquireOneImageInput(Measure);
            var saveImageInput = new SaveImageInput(Rexec.Recipe, Rexec.RemoteProductionInfo, AdaWriter, AdaWriterLock, DmtResultType, Measure.MeasureName.ToLower(),
                GetFullPathName(Measure.MeasureName.ToLower(), Measure.Side));
            if (PreRegisterFlowByResultType.TryGetValue(DmtResultType, out var preRegisterFlow))
            {
                saveImageInput.InternalDbResItemId = preRegisterFlow.Result.OutPreRegister.InternalDBResItemId;
            }
            return new DMTSingleAcquisitionFlowTask(
                new AcquireOneImageFlow(acquisitionInput, CameraManager, HardwareManager),
                FlowsAreSimulated ? new SaveImageFlowDummy(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService)
                                  : new SaveImageFlow(saveImageInput, AlgorithmManager, CalibrationManager, DbRegisterAcqService));
        }
    }
}
