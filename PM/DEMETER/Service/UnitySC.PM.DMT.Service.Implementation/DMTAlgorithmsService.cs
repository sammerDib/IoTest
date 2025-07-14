using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTAlgorithmsService : DuplexServiceBase<IDMTAlgorithmsServiceCallback>, IDMTAlgorithmsService
    {
        private readonly AlgorithmManager _algorithmManager;

        private readonly IDMTInternalCameraMethods _cameraManager;

        private CancellationTokenSource _flowCancellationTokenSource;

        private readonly FringeManager _fringeManager;

        private readonly DMTHardwareManager _hardwareManager;
        
        private readonly CalibrationManager _calibrationManager;

        private AutoExposureFlow _autoExposureFlow;

        private IFlowTask _autoExposureFlowTask;
        internal bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;

        public DMTAlgorithmsService(ILogger<DMTAlgorithmsService> logger, AlgorithmManager algorithmManager,
            DMTHardwareManager hardwareManager, DMTCameraManager cameraManager, FringeManager fringeManager, CalibrationManager calibrationManager) : base(
            logger,
            ExceptionType.AlgoException)
        {
            _algorithmManager = algorithmManager;
            _hardwareManager = hardwareManager;
            _cameraManager = cameraManager;
            _fringeManager = fringeManager;
            _calibrationManager = calibrationManager;
            _flowCancellationTokenSource = new CancellationTokenSource();
        }

        Response<VoidResult> IDMTAlgorithmsService.Subscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Subscribed to AlgorithmsService");
                Subscribe();
            });
        }

        Response<VoidResult> IDMTAlgorithmsService.Unsubscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Unsubscribed to AlgorithmsService");
                Unsubscribe();
            });
        }

        Response<VoidResult> IDMTAlgorithmsService.StartAutoExposureOnMeasure(MeasureBase measure)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                USPImageMil screenImage = null;
                switch (measure)
                {
                    case DeflectometryMeasure dfMeasure:
                        screenImage = _fringeManager.GetFringeImages(measure.Side, dfMeasure.Fringe)[0];
                        break;

                    case HighAngleDarkFieldMeasure highAngleDarkfieldMeasure:
                        screenImage = _calibrationManager.GetHighAngleDarkFieldMaskForSide(measure.Side);
                        break;
                }
                var autoExposureInput = new AutoExposureInput(measure, screenImage);

                StartAutoExposureFlow(autoExposureInput);
            });
        }

        public Response<VoidResult> StartAutoExposure(Side side, MeasureType measureType, bool ignorePerspectiveCalibration = false)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                var flowConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
                var setting = flowConfiguration.Flows.OfType<AutoExposureConfiguration>().First()
                    .DefaultAutoExposureSetting.First(defaultSetting =>
                        defaultSetting.Measure == measureType && defaultSetting.WaferSide == side);
                var camera = _hardwareManager.CamerasBySide[side];
                var roi = new ROI()
                {
                    RoiType = RoiType.Rectangular,
                    Rect = new System.Windows.Rect(0, 0, camera.Width, camera.Height)
                };
                var autoExposureInput = new AutoExposureInput(side, measureType, roi,
                    AcquisitionScreenDisplayImage.Color, Colors.White, null, null, setting.TargetSaturation);
                autoExposureInput.IgnorePerspectiveCalibration = ignorePerspectiveCalibration;

                StartAutoExposureFlow(autoExposureInput);
            });
        }

        public Response<VoidResult> CancelAutoExposure()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _autoExposureFlowTask.Cancel();
            });
        }

        public override void Shutdown()
        {
            if (_autoExposureFlow != null)
            {
                _autoExposureFlow.StatusChanged -= AutoExposureFlowProgressHandler;
            }
            _flowCancellationTokenSource.Dispose();

            base.Shutdown();
        }

        private void AutoExposureFlowProgressHandler(FlowStatus status, AutoExposureResult statusData)
        {
            var recipeStatus = new AutoExposureStatus();
            switch (status.State)
            {
                case FlowState.InProgress:
                    recipeStatus.State = DMTRecipeState.Executing;
                    break;

                case FlowState.Error:
                    recipeStatus.State = DMTRecipeState.Failed;
                    break;

                case FlowState.Canceled:
                    recipeStatus.State = DMTRecipeState.Aborted;
                    break;

                case FlowState.Success:
                    recipeStatus.State = DMTRecipeState.ExecutionComplete;
                    break;
            }

            if (statusData != null)
            {
                recipeStatus.ExposureTimeMs = statusData.ExposureTimeMs;
                recipeStatus.CurrentStep = statusData.CurrentStep;
                recipeStatus.TotalSteps = statusData.TotalSteps;
                recipeStatus.Message = status.Message;
            }

            InvokeCallback(client => client.ReportProgress(recipeStatus));
            if (status.State == FlowState.Error || status.State == FlowState.Partial ||
                status.State == FlowState.Canceled || status.State == FlowState.Success)
            {
                _autoExposureFlow.StatusChanged -= AutoExposureFlowProgressHandler;
            }
        }

        private void StartAutoExposureFlow(AutoExposureInput input)
        {
            _autoExposureFlow = FlowsAreSimulated
                ? new AutoExposureFlowDummy(input, _hardwareManager, _cameraManager)
                : new AutoExposureFlow(input, _hardwareManager, _cameraManager);
            _flowCancellationTokenSource = new CancellationTokenSource();
            _autoExposureFlow.CancellationToken = _flowCancellationTokenSource.Token;

            _autoExposureFlow.StatusChanged += AutoExposureFlowProgressHandler;
            _autoExposureFlowTask =
                new FlowTask<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>(_autoExposureFlow);
            _autoExposureFlowTask.Start();
        }
    }
}
