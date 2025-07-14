using System;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Flows.AxisOrthogonality;
using UnitySC.PM.EME.Service.Core.Flows.DistanceSensorCalibration;
using UnitySC.PM.EME.Service.Core.Flows.Distortion;
using UnitySC.PM.EME.Service.Core.Flows.FilterCalibration;
using UnitySC.PM.EME.Service.Core.Flows.ImageProcessing;
using UnitySC.PM.EME.Service.Core.Flows.MultiSizeChuck;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Flows.PixelSize;
using UnitySC.PM.EME.Service.Core.Flows.Vignetting;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AlgoService : DuplexServiceBase<IAlgoServiceCallback>, IAlgoService, IFlowEvent
    {
        private readonly IEmeraCamera _camera;
        private readonly FlowsConfiguration _flowsConfiguration;

        private AutoFocusCameraFlow _autoFocusCameraFlow;
        private IFlowTask _afCameraFlowTask;
        private VignettingFlow _vignettingFlow;
        private IFlowTask _vignettingFlowTask;
        private AxisOrthogonalityFlow _axisOrthogonalityFlow;
        private IFlowTask _axisOrthogonalityFlowTask;
        private PixelSizeComputationFlow _pixelSizeComputationFlow;
        private IFlowTask _pixelSizeComputationFlowTask;
        private DistortionFlow _distortionFlow;
        private IFlowTask _distortionFlowTask;
        private ImagePreprocessingFlow _imagePreprocessingFlow;
        private IFlowTask _imagePreprocessingFlowTask;
        private PatternRecFlow _patternRecFlow;
        private IFlowTask _patternRecFlowTask;
        private MultiSizeChuckFlow _multiSizeChuckFlow;
        private IFlowTask _multiSizeChuckFlowTask;
        private FilterCalibrationFlow _filterCalibrationFlow;
        private IFlowTask _filterCalibrationFlowTask;
        private AutoExposureFlow _autoExposureFlow;
        private IFlowTask _autoExposureFlowTask;
        private GetZFocusFlow _getZFocusFlow;
        private IFlowTask _getZFocusFlowTask;
        private DistanceSensorCalibrationFlow _distanceSensorCalibrationFlow;
        private IFlowTask _distanceSensorCalibrationFlowTask;

        private event EventHandler<FlowEventArgs> RaiseFlowEvent;

        private bool FlowsAreSimulated =>
            ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>().FlowsAreSimulated;

        public AlgoService(ILogger logger, IEmeraCamera camera) : base(logger, ExceptionType.AlgoException)
        {
            _flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            _camera = camera;
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
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Subscribed to algo service change");
                Subscribe();
            });
        }

        public Response<VoidResult> UnsubscribeToAlgoChanges()
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("UnSubscribed to algo service change");
                Unsubscribe();
            });
        }

        public Response<VoidResult> StartAutoFocusCamera(AutoFocusCameraInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information($"Start AutoFocus Camera:  Range: {input.RangeType}");
                if (FlowsAreSimulated)
                    _autoFocusCameraFlow = new AutoFocusCameraFlowDummy(input, _camera);
                else
                    _autoFocusCameraFlow = new AutoFocusCameraFlow(input, _camera);
                _afCameraFlowTask = new FlowTask<AutoFocusCameraInput, AutoFocusCameraResult, AutoFocusCameraConfiguration>(_autoFocusCameraFlow);
                _autoFocusCameraFlow.StatusChanged += AutoFocusCameraFlowStatusChanged;
                Task.Run(() => _afCameraFlowTask.Start());
            });
        }

        private void AutoFocusCameraFlowStatusChanged(FlowStatus status, AutoFocusCameraResult statusData)
        {
            InvokeCallback(callback => callback.AutoFocusCameraChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _autoFocusCameraFlow.StatusChanged -= AutoFocusCameraFlowStatusChanged;
        }

        public Response<VoidResult> CancelAutoFocusCamera()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_autoFocusCameraFlow != null)
                    _afCameraFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartVignetting(VignettingInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                if (FlowsAreSimulated)
                    _vignettingFlow = new VignettingFlowDummy(input, _camera);
                else
                    _vignettingFlow = new VignettingFlow(input, _camera);
                _vignettingFlowTask = new FlowTask<VignettingInput, VignettingResult, VignettingConfiguration>(_vignettingFlow);
                _vignettingFlow.StatusChanged += VignettingFlow_StatusChanged;

                Task.Run(() => _vignettingFlowTask.Start());
            });
        }

        private void VignettingFlow_StatusChanged(FlowStatus status, VignettingResult statusData)
        {
            InvokeCallback(callback => callback.VignettingChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _vignettingFlow.StatusChanged -= VignettingFlow_StatusChanged;
        }

        public Response<VoidResult> CancelVignetting()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_vignettingFlow != null)
                    _vignettingFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartAxisOrthogonality(AxisOrthogonalityInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Axis Orthogonality");
                if (FlowsAreSimulated)
                    _axisOrthogonalityFlow = new AxisOrthogonalityFlowDummy(input, _camera);
                else
                    _axisOrthogonalityFlow = new AxisOrthogonalityFlow(input, _camera);
                _axisOrthogonalityFlowTask = new FlowTask<AxisOrthogonalityInput, AxisOrthogonalityResult, AxisOrthogonalityConfiguration>(_axisOrthogonalityFlow);
                _axisOrthogonalityFlow.StatusChanged += AxisOrthogonalityFlow_StatusChanged;
                Task.Run(() => _axisOrthogonalityFlowTask.Start());
            });
        }

        private void AxisOrthogonalityFlow_StatusChanged(FlowStatus status, AxisOrthogonalityResult statusData)
        {
            InvokeCallback(callback => callback.AxisOrthogonalityChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _axisOrthogonalityFlow.StatusChanged -= AxisOrthogonalityFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAxisOrthogonality()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_axisOrthogonalityFlow != null)
                    _axisOrthogonalityFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartPixelSizeComputation(PixelSizeComputationInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Axis Pixel Size Computation");
                if (FlowsAreSimulated)
                    _pixelSizeComputationFlow = new PixelSizeComputationFlowDummy(input, _camera);
                else
                    _pixelSizeComputationFlow = new PixelSizeComputationFlow(input, _camera);
                _pixelSizeComputationFlowTask = new FlowTask<PixelSizeComputationInput, PixelSizeComputationResult, PixelSizeComputationConfiguration>(_pixelSizeComputationFlow);
                _pixelSizeComputationFlow.StatusChanged += PixelSizeComputationFlow_StatusChanged;
                Task.Run(() => _pixelSizeComputationFlowTask.Start());
            });
        }

        private void PixelSizeComputationFlow_StatusChanged(FlowStatus status, PixelSizeComputationResult statusData)
        {
            InvokeCallback(callback => callback.PixelSizeComputationChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _pixelSizeComputationFlow.StatusChanged -= PixelSizeComputationFlow_StatusChanged;
        }

        public Response<VoidResult> CancelPixelSizeComputation()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_pixelSizeComputationFlow != null)
                    _pixelSizeComputationFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartDistortion(DistortionInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Distortion");
                if (FlowsAreSimulated)
                    _distortionFlow = new DistortionFlowDummy(input, _camera);
                else
                    _distortionFlow = new DistortionFlow(input, _camera);
                _distortionFlowTask = new FlowTask<DistortionInput, DistortionResult, DistortionConfiguration>(_distortionFlow);
                _distortionFlow.StatusChanged += DistortionFlow_StatusChanged;
                Task.Run(() => _distortionFlowTask.Start());
            });
        }

        private void DistortionFlow_StatusChanged(FlowStatus status, DistortionResult statusData)
        {
            InvokeCallback(callback => callback.DistortionChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _distortionFlow.StatusChanged -= DistortionFlow_StatusChanged;
        }

        public Response<VoidResult> CancelDistortion()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_distortionFlow != null)
                    _distortionFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Image Processing");
                if (FlowsAreSimulated)
                    _imagePreprocessingFlow = new ImagePreprocessingFlow(input);
                else
                    _imagePreprocessingFlow = new ImagePreprocessingFlow(input);
                _imagePreprocessingFlowTask = new FlowTask<ImagePreprocessingInput, ImagePreprocessingResult, AxesMovementConfiguration>(_imagePreprocessingFlow);
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

        public Response<VoidResult> StartPatternRec(PatternRecInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Pattern Rec");
                if (FlowsAreSimulated)
                    _patternRecFlow = new PatternRecFlowDummy(input, _camera);
                else
                    _patternRecFlow = new PatternRecFlow(input, _camera);
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
            return InvokeVoidResponse(_ =>
            {
                _patternRecFlowTask.Cancel();
            });
        }

        public Response<double> GetFlowImageScale()
        {
            return InvokeDataResponse(_ =>
            {
                return _flowsConfiguration.ImageScale;
            });
        }

        public Response<VoidResult> StartMultiSizeChuck(MultiSizeChuckInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Multi Size Chuck");
                if (FlowsAreSimulated)
                    _multiSizeChuckFlow = new MultiSizeChuckFlowDummy(input, _camera);
                else
                    _multiSizeChuckFlow = new MultiSizeChuckFlow(input, _camera);
                _multiSizeChuckFlowTask = new FlowTask<MultiSizeChuckInput, MultiSizeChuckResult, MultiSizeChuckConfiguration>(_multiSizeChuckFlow);
                _multiSizeChuckFlow.StatusChanged += MultiSizeChuckFlow_StatusChanged;
                Task.Run(() => _multiSizeChuckFlowTask.Start());
            });
        }

        private void MultiSizeChuckFlow_StatusChanged(FlowStatus status, MultiSizeChuckResult statusData)
        {
            InvokeCallback(callback => callback.MultiSizeChuckChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _multiSizeChuckFlow.StatusChanged -= MultiSizeChuckFlow_StatusChanged;
        }

        public Response<VoidResult> CancelMultiSizeChuck()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_multiSizeChuckFlow != null)
                    _multiSizeChuckFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartFilterCalibration(FilterCalibrationInput input)
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Start Filter Calibration");
                if (FlowsAreSimulated)
                    _filterCalibrationFlow = new FilterCalibrationFlowDummy(input, _flowsConfiguration, _camera);
                else
                    _filterCalibrationFlow = new FilterCalibrationFlow(input, _flowsConfiguration, _camera);
                _filterCalibrationFlowTask = new FlowTask<FilterCalibrationInput, FilterCalibrationResult, FilterCalibrationConfiguration>(_filterCalibrationFlow);
                _filterCalibrationFlow.StatusChanged += FilterCalibrationFlow_StatusChanged;
                Task.Run(() => _filterCalibrationFlowTask.Start());
            });
        }

        private void FilterCalibrationFlow_StatusChanged(FlowStatus status, FilterCalibrationResult statusData)
        {
            InvokeCallback(callback => callback.FilterCalibrationChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _filterCalibrationFlow.StatusChanged -= FilterCalibrationFlow_StatusChanged;
        }

        public Response<VoidResult> CancelFilterCalibration()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_filterCalibrationFlowTask != null)
                    _filterCalibrationFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartAutoExposure(AutoExposureInput input)
        {
            return InvokeVoidResponse(_ => Task.Run(() =>
            {
                if (FlowsAreSimulated)
                    _autoExposureFlow = new AutoExposureFlowDummy(input);
                else
                    _autoExposureFlow = new AutoExposureFlow(input, _camera);
                _autoExposureFlowTask = new FlowTask<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>(_autoExposureFlow);
                _autoExposureFlow.StatusChanged += AutoExposureFlow_StatusChanged;
                Task.Run(() => _autoExposureFlowTask.Start());

            }));
        }

        private void AutoExposureFlow_StatusChanged(FlowStatus status, AutoExposureResult statusData)
        {
            InvokeCallback(callback => callback.AutoExposureChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _autoExposureFlow.StatusChanged -= AutoExposureFlow_StatusChanged;
        }

        public Response<VoidResult> CancelAutoExposure()
        {
            return InvokeVoidResponse(_ => { if (_autoExposureFlow != null) _autoExposureFlowTask.Cancel(); });
        }

        public Response<VoidResult> StartGetZFocus(GetZFocusInput input)
        {
            return InvokeVoidResponse(_ => Task.Run(() =>
            {
                if (FlowsAreSimulated)
                    _getZFocusFlow = new GetZFocusFlowDummy(input);
                else
                    _getZFocusFlow = new GetZFocusFlow(input);
                _getZFocusFlowTask =
                    new FlowTask<GetZFocusInput, GetZFocusResult, GetZFocusConfiguration>(_getZFocusFlow);
                _getZFocusFlow.StatusChanged += GetZFocusFlow_StatusChanged;
                Task.Run(() => _getZFocusFlowTask.Start());
            }));
        }

        private void GetZFocusFlow_StatusChanged(FlowStatus status, GetZFocusResult result)
        {
            InvokeCallback(callback => callback.GetZFocusChanged(result));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _getZFocusFlow.StatusChanged -= GetZFocusFlow_StatusChanged;
        }

        public Response<VoidResult> CancelGetZFocus()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_getZFocusFlow != null) _getZFocusFlowTask.Cancel();
            });
        }

        public Response<VoidResult> StartDistanceSensorCalibration(DistanceSensorCalibrationInput input)
        {
            return InvokeVoidResponse(_ => Task.Run(() =>
            {
                if (FlowsAreSimulated)
                    _distanceSensorCalibrationFlow = new DistanceSensorCalibrationFlowDummy(input, _camera) ;
                else
                    _distanceSensorCalibrationFlow = new DistanceSensorCalibrationFlow(input, _camera);
                _distanceSensorCalibrationFlowTask =
                    new FlowTask<DistanceSensorCalibrationInput, DistanceSensorCalibrationResult, DistanceSensorCalibrationConfiguration>(_distanceSensorCalibrationFlow);
                _distanceSensorCalibrationFlow.StatusChanged += DistanceSensorCalibrationFlow_StatusChanged;
                Task.Run(() => _distanceSensorCalibrationFlowTask.Start());
            }));
        }

        private void DistanceSensorCalibrationFlow_StatusChanged(FlowStatus status, DistanceSensorCalibrationResult result)
        {
            InvokeCallback(callback => callback.DistanceSensorCalibrationChanged(result));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _distanceSensorCalibrationFlow.StatusChanged -= DistanceSensorCalibrationFlow_StatusChanged;
        }

        public Response<VoidResult> CancelDistanceSensorCalibration()
        {
            return InvokeVoidResponse(_ =>
            {
                if (_distanceSensorCalibrationFlow != null) _distanceSensorCalibrationFlowTask.Cancel();
            });
        }
    }
}
