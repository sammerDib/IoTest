using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.AdvancedFlow;
using UnitySC.PM.ANA.Service.Core.AlignmentMarks;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.Autolight;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AlgoService : DuplexServiceBase<IAlgoServiceCallback>, IAlgoService, IFlowEvent
    {
        private AFLiseFlow _afLiseFlow;
        private IFlowTask _afLiseflowTask;
        private AFV2CameraFlow _afV2CameraFlow;
        private IFlowTask _afV2CameraFlowTask;
        private AutofocusFlow _autoFocusFlow;
        private IFlowTask _autoFocusFlowTask;
        private BareWaferAlignmentFlow _bwaFlow;
        private IFlowTask _bwaFlowTask;
        private BareWaferAlignmentImageFlow _bwaImageFlow;
        private IFlowTask _bwaImageFlowTask;
        private ImagePreprocessingFlow _imagePreprocessingFlow;
        private IFlowTask _imagePreprocessingFlowTask;
        private PatternRecFlow _patternRecFlow;
        private IFlowTask _patternRecFlowTask;
        private AutolightFlow _autolightFlow;
        private IFlowTask _autolightFlowTask;
        private AutoAlignFlow _autoAlignFlow;
        private IFlowTask _autoAlignFlowTask;
        private AlignmentMarksFlow _alignmentMarksFlow;
        private IFlowTask _alignmentMarksFlowTask;
        private CheckPatternRecFlow _checkPatternRecFlow;
        private IFlowTask _checkPatternRecFlowTask;
        private DieAndStreetSizesFlow _dieAndStreetSizesFlow;
        private IFlowTask _dieAndStreetSizesTask;
        private WaferMapFlow _waferMapFlow;
        private IFlowTask _waferMapTask;
        private CheckWaferPresenceFlow _checkWaferPresenceFlow;
        private IFlowTask _checkWaferPresenceTask;

        private event EventHandler<FlowEventArgs> RaiseFlowEvent;

        private ContextManager _contextManager;

        internal bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;

        public AlgoService(ILogger logger) : base(logger, ExceptionType.AlgoException)
        {
            _contextManager = ClassLocator.Default.GetInstance<ContextManager>();
        }

        public void AddFlowEventListener(EventHandler<FlowEventArgs> l)
        {
            RaiseFlowEvent += l;
        }

        public void RemoveFlowEventListener(EventHandler<FlowEventArgs> l)
        {
            RaiseFlowEvent -= l;
        }

        public void OnRaiseFlowEvent(FlowEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var raiseEvent = RaiseFlowEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        public Response<VoidResult> SubscribeToAlgoChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Subscribed to algo service change");
                Subscribe();
            });
        }

        public Response<VoidResult> UnsubscribeToAlgoChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("UnSubscribed to algo service change");
                Unsubscribe();
            });
        }

        #region AF Lise

        public Response<VoidResult> StartAFLise(AFLiseInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start AF Lise");
                if (FlowsAreSimulated)
                    _afLiseFlow = new AFLiseFlowDummy(input);
                else
                    _afLiseFlow = new AFLiseFlow(input);
                _afLiseflowTask = new FlowTask<AFLiseInput, AFLiseResult, AFLiseConfiguration>(_afLiseFlow);
                _afLiseFlow.StatusChanged += AfFLiseFlow_StatusChanged;
                Task.Run(() => _afLiseflowTask.Start());
            });
        }

        public Response<AFLiseSettings> GetAFLiseSettings(AFLiseInput input, ObjectiveConfig objectiveConfig = null)
        {
            return InvokeDataResponse(messageContainer =>
            {
                var aFLiseSettings = new AFLiseSettings();
                var currentObjective = objectiveConfig ?? ClassLocator.Default.GetInstance<AnaHardwareManager>().GetObjectiveInUseByProbe(input.ProbeID);
                var objectiveCalibration = ClassLocator.Default.GetInstance<CalibrationManager>().GetObjectiveCalibration(currentObjective.DeviceID);
                var liseCalib = objectiveCalibration?.AutoFocus?.Lise;
                var flowConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();
                var afLiseConfiguration = flowConfiguration?.Flows?.OfType<AFLiseConfiguration>().FirstOrDefault();

                if (liseCalib is null)
                {
                    return aFLiseSettings;
                }
                aFLiseSettings.ZMin = objectiveCalibration.AutoFocus.ZFocusPosition;
                aFLiseSettings.ZMax = objectiveCalibration.AutoFocus.ZFocusPosition;
                if (afLiseConfiguration != null)
                {
                    aFLiseSettings.ZMin -= (afLiseConfiguration.AutoFocusScanRange / 2.0);
                    aFLiseSettings.ZMax += (afLiseConfiguration.AutoFocusScanRange / 2.0);
                }

                if (!(objectiveCalibration?.OpticalReferenceElevationFromStandardWafer is null))
                {
                    aFLiseSettings.ZMin -= objectiveCalibration.OpticalReferenceElevationFromStandardWafer;
                    aFLiseSettings.ZMax -= objectiveCalibration.OpticalReferenceElevationFromStandardWafer;
                }

                aFLiseSettings.CalibratedGainMin = liseCalib.MinGain;
                aFLiseSettings.CalibratedGainMax = liseCalib.MaxGain;
                aFLiseSettings.CalibratedZ = liseCalib.ZStartPosition;

                return aFLiseSettings;
            });
        }

        private void AfFLiseFlow_StatusChanged(FlowStatus status, AFLiseResult statusData)
        {
            InvokeCallback(callback => callback.AFLiseChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _afLiseFlow.StatusChanged -= AfFLiseFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAFLise()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _afLiseflowTask.Cancel();
            });
        }

        #endregion AF Lise

        #region AF IMage

        public Response<VoidResult> StartAFCamera(AFCameraInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information($"Start AF V2 Image:  Range: {input.RangeType} - Use current z position: {input.UseCurrentZPosition}");
                if (FlowsAreSimulated)
                    _afV2CameraFlow = new AFV2CameraFlowDummy(input);
                else
                    _afV2CameraFlow = new AFV2CameraFlow(input);
                _afV2CameraFlowTask = new FlowTask<AFCameraInput, AFCameraResult, AFCameraConfiguration>(_afV2CameraFlow);
                _afV2CameraFlow.StatusChanged += AfV2CameraFlow_StatusChanged;
                Task.Run(() => _afV2CameraFlowTask.Start());
            });
        }

        private void AfV2CameraFlow_StatusChanged(FlowStatus status, AFCameraResult statusData)
        {
            InvokeCallback(callback => callback.AFCameraChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _afV2CameraFlow.StatusChanged -= AfV2CameraFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAFCamera()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_afV2CameraFlow != null)
                    _afV2CameraFlowTask.Cancel();
            });
        }

        #endregion AF IMage

        #region Auto Focus

        public Response<VoidResult> StartAutoFocus(AutofocusInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Auto Focus");

                if (FlowsAreSimulated)
                    _autoFocusFlow = new AutofocusFlowDummy(input);
                else
                    _autoFocusFlow = new AutofocusFlow(input);
                _autoFocusFlowTask = new FlowTask<AutofocusInput, AutofocusResult, AutofocusConfiguration>(_autoFocusFlow);
                _autoFocusFlow.StatusChanged += AutoFocusFlow_StatusChanged;
                Task.Run(() => _autoFocusFlowTask.Start());
            });
        }

        private void AutoFocusFlow_StatusChanged(FlowStatus status, AutofocusResult statusData)
        {
            InvokeCallback(callback => callback.AutoFocusChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _autoFocusFlow.StatusChanged -= AutoFocusFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAutoFocus()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _autoFocusFlowTask?.Cancel();
            });
        }

        #endregion Auto Focus

        #region BWA

        public Response<VoidResult> StartBWA(BareWaferAlignmentInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Bwa");
                if (FlowsAreSimulated)
                    _bwaFlow = new BareWaferAlignmentFlowDummy(input);
                else
                    _bwaFlow = new BareWaferAlignmentFlow(input);
                _bwaFlowTask = new FlowTask<BareWaferAlignmentInput, BareWaferAlignmentChangeInfo, BareWaferAlignmentConfiguration>(_bwaFlow);
                _bwaFlow.StatusChanged += BwaFlow_StatusChanged;
                Task.Run(() => _bwaFlowTask.Start());
            });
        }

        public Response<BareWaferAlignmentSettings> GetBWASettings(BareWaferAlignmentInput input)
        {
            return InvokeDataResponse(messageContainer =>
            {
                var bWASettings = new BareWaferAlignmentSettings();

                var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(input.Wafer);
                double unitconvertcoef = 1.0 / 1000.0; // micrometer to milimeter
                foreach (var imagePosition in imagePositions)
                {
                    switch (imagePosition.EdgePosition)
                    {
                        case EdgePosition.TOP:
                            bWASettings.ImagePositionTop = new XYPosition(new StageReferential(), imagePosition.Centroid.X * unitconvertcoef, imagePosition.Centroid.Y * unitconvertcoef);
                            break;

                        case EdgePosition.RIGHT:
                            bWASettings.ImagePositionRight = new XYPosition(new StageReferential(), imagePosition.Centroid.X * unitconvertcoef, imagePosition.Centroid.Y * unitconvertcoef);
                            break;

                        case EdgePosition.BOTTOM:
                            bWASettings.ImagePositionBottom = new XYPosition(new StageReferential(), imagePosition.Centroid.X * unitconvertcoef, imagePosition.Centroid.Y * unitconvertcoef);
                            break;

                        case EdgePosition.LEFT:
                            bWASettings.ImagePositionLeft = new XYPosition(new StageReferential(), imagePosition.Centroid.X * unitconvertcoef, imagePosition.Centroid.Y * unitconvertcoef);
                            break;
                    }
                }

                return bWASettings;
            });
        }

        private void BwaFlow_StatusChanged(FlowStatus status, BareWaferAlignmentChangeInfo statusData)
        {
            InvokeCallback(callback => callback.BwaChanged(statusData));
            OnRaiseFlowEvent(new FlowEventArgs(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _bwaFlow.StatusChanged -= BwaFlow_StatusChanged;
        }

        public Response<VoidResult> CancelBWA()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_bwaFlowTask != null)
                    _bwaFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartBWAImage(BareWaferAlignmentImageInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Bwa Image");
                if (FlowsAreSimulated)
                    _bwaImageFlow = new BareWaferAlignmentImageFlowDummy(input);
                else
                    _bwaImageFlow = new BareWaferAlignmentImageFlow(input);
                _bwaImageFlowTask = new FlowTask<BareWaferAlignmentImageInput, BareWaferAlignmentImage, BareWaferAlignmentImageConfiguration>(_bwaImageFlow);
                _bwaImageFlow.StatusChanged += BwaImageFlow_StatusChanged;
                Task.Run(() => _bwaImageFlowTask.Start());
            });
        }

        public Response<VoidResult> CancelBWAImage()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _bwaImageFlowTask.Cancel();
            });
        }

        private void BwaImageFlow_StatusChanged(FlowStatus status, BareWaferAlignmentImage statusData)
        {
            InvokeCallback(callback => callback.BwaImageChanged(statusData));
            OnRaiseFlowEvent(new FlowEventArgs(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _bwaImageFlow.StatusChanged -= BwaImageFlow_StatusChanged;
        }

        #endregion BWA

        #region Auto Light

        public Response<VoidResult> StartAutoLight(AutolightInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Autolight");
                AnaHardwareManager hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
                var camera = hardwareManager.Cameras.FirstOrDefault(x => x.Value.Name == "Camera Up").Value;
                if (FlowsAreSimulated)
                    _autolightFlow = new AutolightFlowDummy(input);
                else
                    _autolightFlow = new AutolightFlow(input);
                _autolightFlowTask = new FlowTask<AutolightInput, AutolightResult, AutolightConfiguration>(_autolightFlow);
                _autolightFlow.StatusChanged += _autolightFlow_StatusChanged;
                Task.Run(() => _autolightFlowTask.Start());
            });
        }

        private void _autolightFlow_StatusChanged(FlowStatus status, AutolightResult statusData)
        {
            InvokeCallback(callback => callback.AutoLightChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _autolightFlow.StatusChanged -= _autolightFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAutoLight()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _autolightFlowTask.Cancel();
            });
        }

        #endregion Auto Light

        #region Advenced Flow

        private void _checkWaferPresence_StatusChanged(FlowStatus status, CheckWaferPresenceResult statusData)
        {
            InvokeCallback(callback => callback.CheckWaferPresenceChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _checkWaferPresenceFlow.StatusChanged -= _checkWaferPresence_StatusChanged;
        }

        public Response<VoidResult> CheckWaferPresence(CheckWaferPresenceInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start CheckWaferPresence");
                var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

                if (FlowsAreSimulated)
                    _checkWaferPresenceFlow = new CheckWaferPresenceFlow(input);
                else
                    _checkWaferPresenceFlow = new CheckWaferPresenceFlow(input);
                _checkWaferPresenceTask = new FlowTask<CheckWaferPresenceInput, CheckWaferPresenceResult, AxesMovementConfiguration>(_checkWaferPresenceFlow);
                _checkWaferPresenceFlow.StatusChanged += _checkWaferPresence_StatusChanged;
                Task.Run(() => _checkWaferPresenceTask.Start());
            });
        }

        #endregion Advenced Flow

        #region Image processing

        public Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Image Processing");
                if (FlowsAreSimulated)
                    _imagePreprocessingFlow = new ImagePreprocessingFlow(input);
                else
                    _imagePreprocessingFlow = new ImagePreprocessingFlow(input);
                _imagePreprocessingFlowTask = new FlowTask<ImagePreprocessingInput, ImagePreprocessingResult, ImagePreprocessingConfiguration>(_imagePreprocessingFlow);
                _imagePreprocessingFlow.StatusChanged += ImagePreprocessingFlow_StatusChanged;
                Task.Run(() => _imagePreprocessingFlowTask.Start());
            });
        }

        private void ImagePreprocessingFlow_StatusChanged(FlowStatus status, ImagePreprocessingResult statusData)
        {
            InvokeCallback(callback => callback.ImagePreprocessingChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _imagePreprocessingFlow.StatusChanged -= ImagePreprocessingFlow_StatusChanged;
        }

        #endregion Image processing

        #region Pattern Rec

        public Response<VoidResult> StartPatternRec(PatternRecInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Pattern Rec");
                if (FlowsAreSimulated)
                    _patternRecFlow = new PatternRecFlowDummy(input);
                else
                    _patternRecFlow = new PatternRecFlow(input);
                _patternRecFlowTask = new FlowTask<PatternRecInput, PatternRecResult, PatternRecConfiguration>(_patternRecFlow);
                _patternRecFlow.StatusChanged += PatternRecFlow_StatusChanged;
                Task.Run(() => _patternRecFlowTask.Start());
            });
        }

        private void PatternRecFlow_StatusChanged(FlowStatus status, PatternRecResult statusData)
        {
            InvokeCallback(callback => callback.PatternRecChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _patternRecFlow.StatusChanged -= PatternRecFlow_StatusChanged;
        }

        public Response<VoidResult> CancelPatternRec()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _patternRecFlowTask.Cancel();
            });
        }

        #endregion Pattern Rec

        #region Auto Align

        public Response<VoidResult> StartAutoAlign(AutoAlignInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Auto Align");
                if (FlowsAreSimulated)
                    _autoAlignFlow = new AutoAlignFlowDummy(input);
                else
                    _autoAlignFlow = new AutoAlignFlow(input);

                _autoAlignFlowTask = new FlowTask<AutoAlignInput, AutoAlignResult>(_autoAlignFlow);
                _autoAlignFlow.StatusChanged += AutoAlignFlow_StatusChanged;
                Task.Run(() => _autoAlignFlowTask.Start());
            });
        }

        private void AutoAlignFlow_StatusChanged(FlowStatus status, AutoAlignResult statusData)
        {
            InvokeCallback(callback => callback.AutoAlignChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _autoAlignFlow.StatusChanged -= AutoAlignFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAutoAlign()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _autoAlignFlowTask.Cancel();
            });
        }

        #endregion Auto Align

        #region AlignmentMarks

        public Response<VoidResult> StartAlignmentMarks(AlignmentMarksInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Alignment Marks");
                if (FlowsAreSimulated)
                    _alignmentMarksFlow = new AlignmentMarksFlowDummy(input);
                else
                    _alignmentMarksFlow = new AlignmentMarksFlow(input);

                _alignmentMarksFlowTask = new FlowTask<AlignmentMarksInput, AlignmentMarksResult, AlignmentMarksConfiguration>(_alignmentMarksFlow);
                _alignmentMarksFlow.StatusChanged += _alignmentMarksFlow_StatusChanged; ;
                Task.Run(() => _alignmentMarksFlowTask.Start());
            });
        }

        private void _alignmentMarksFlow_StatusChanged(FlowStatus status, AlignmentMarksResult statusData)
        {
            InvokeCallback(callback => callback.AlignmentMarksChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _alignmentMarksFlow.StatusChanged -= _alignmentMarksFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAlignmentMarks()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_alignmentMarksFlowTask != null)
                    _alignmentMarksFlowTask.Cancel();
            });
        }

        #endregion AlignmentMarks

        #region Check Pattern Rec

        public Response<VoidResult> StartCheckPatternRec(CheckPatternRecInput checkPatternRecInput)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Check Pattern Rec");

                _checkPatternRecFlow = new CheckPatternRecFlow(checkPatternRecInput);

                _checkPatternRecFlowTask = new FlowTask<CheckPatternRecInput, CheckPatternRecResult, CheckPatternRecConfiguration>(_checkPatternRecFlow);
                _checkPatternRecFlow.StatusChanged += CheckPatternRecFlow_StatusChanged;
                Task.Run(() => _checkPatternRecFlowTask.Start());
            });
        }

        public Response<CheckPatternRecSettings> GetCheckPatternRecSettings()
        {
            return InvokeDataResponse(messageContainer =>
            {
                AnaHardwareManager hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

                var checkPatternRecSettings = new CheckPatternRecSettings() { ShiftRatio = (double)1/(double)3, NbChecks = 4 };

                return checkPatternRecSettings;
            });
        }

        private void CheckPatternRecFlow_StatusChanged(FlowStatus status, CheckPatternRecResult statusData)
        {
            InvokeCallback(callback => callback.CheckPatternRecChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _checkPatternRecFlow.StatusChanged -= CheckPatternRecFlow_StatusChanged;
        }

        public Response<VoidResult> CancelCheckPatternRec()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _checkPatternRecFlowTask.Cancel();
            });
        }

        #endregion Check Pattern Rec

        #region Die and Street Sizes

        public Response<VoidResult> StartDieAndStreetSizes(DieAndStreetSizesInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Die Size And Pitch");

                if (FlowsAreSimulated)
                    _dieAndStreetSizesFlow = new DieAndStreetSizesFlowDummy(input);
                else
                    _dieAndStreetSizesFlow = new DieAndStreetSizesFlow(input);

                _dieAndStreetSizesTask = new FlowTask<DieAndStreetSizesInput, DieAndStreetSizesResult, DieAndStreetSizesConfiguration>(_dieAndStreetSizesFlow);
                _dieAndStreetSizesFlow.StatusChanged += DieSizeAndPitchFlow_StatusChanged;
                Task.Run(() => _dieAndStreetSizesTask.Start());
            });
        }

        private void DieSizeAndPitchFlow_StatusChanged(FlowStatus status, DieAndStreetSizesResult statusData)
        {
            InvokeCallback(callback => callback.DieAndStreetSizesChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _dieAndStreetSizesFlow.StatusChanged -= DieSizeAndPitchFlow_StatusChanged;
        }

        public Response<VoidResult> CancelDieAndStreetSizes()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _dieAndStreetSizesTask?.Cancel();
            });
        }

        #endregion Die and Street Sizes

        #region Wafer Map

        public Response<VoidResult> StartWaferMap(WaferMapInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Start Wafer Map");

                if (FlowsAreSimulated)
                    _waferMapFlow = new WaferMapFlowDummy(input);
                else
                    _waferMapFlow = new WaferMapFlow(input);

                _waferMapTask = new FlowTask<WaferMapInput, WaferMapResult, WaferMapConfiguration>(_waferMapFlow);
                _waferMapFlow.StatusChanged += _waferMapFlow_StatusChanged;
                Task.Run(() => _waferMapTask.Start());
            });
        }

        private void _waferMapFlow_StatusChanged(FlowStatus status, WaferMapResult statusData)
        {
            InvokeCallback(callback => callback.WaferMapChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _waferMapFlow.StatusChanged -= _waferMapFlow_StatusChanged;
        }

        public Response<VoidResult> CancelWaferMap()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _waferMapTask?.Cancel();
            });
        }

        #endregion Wafer Map
    }
}
