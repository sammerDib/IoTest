using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Algo
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class AlgoSupervisor : IAlgoSupervisor
    {
        public event VignettingChangedHandler VignettingChangedEvent;
        public event AutoFocusCameraChangedHandler AutoFocusCameraChangedEvent;
        public event AxisOrthogonalityChangedHandler AxisOrthogonalityChangedEvent;
        public event PixelSizeComputationChangedHandler PixelSizeComputationChangedEvent;
        public event ImagePreprocessingChangedHandler ImagePreprocessingChangedEvent;
        public event PatternRecChangedHandler PatternRecChangedEvent;
        public event MultiSizeChuckChangedHandler MultiSizeChuckChangedEvent;
        public event FilterCalibrationChangedHandler FilterCalibrationChangedEvent;
        public event DistortionChangedHandler DistortionChangedEvent;
        public event CameraExposureChangedHandler CameraExposureChangedEvent;
        public event GetZFocusChangedHandler GetZFocusChangedEvent;
        public event DistanceSensorCalibrationChangedHandler DistanceSensorCalibrationChangedEvent;

        private readonly DuplexServiceInvoker<IAlgoService> _algoService;

        public AlgoSupervisor()
        {
            var instanceContext = new InstanceContext(this);
            var logger = ClassLocator.Default.GetInstance<SerilogLogger<IAlgoService>>();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            var customAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _algoService = new DuplexServiceInvoker<IAlgoService>(instanceContext, "EMERAAlgoService", logger, messenger, x => x.SubscribeToAlgoChanges(), customAddress);
        }

        public Response<VoidResult> SubscribeToAlgoChanges()
        {
            return _algoService.TryInvokeAndGetMessages(s => s.SubscribeToAlgoChanges());
        }

        public Response<VoidResult> UnsubscribeToAlgoChanges()
        {
            return _algoService.TryInvokeAndGetMessages(s => s.UnsubscribeToAlgoChanges());
        }

        public Response<VoidResult> StartAutoFocusCamera(AutoFocusCameraInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAutoFocusCamera(input));
        }

        public void AutoFocusCameraChanged(AutoFocusCameraResult autoFocusResult)
        {
            AutoFocusCameraChangedEvent?.Invoke(autoFocusResult);
        }

        public Response<VoidResult> CancelAutoFocusCamera()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAutoFocusCamera());
        }

        public Response<VoidResult> StartVignetting(VignettingInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartVignetting(input));
        }

        public void VignettingChanged(VignettingResult vignettingResult)
        {
            VignettingChangedEvent?.Invoke(vignettingResult);
        }

        public Response<VoidResult> CancelVignetting()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelVignetting());
        }

        public Response<VoidResult> StartAxisOrthogonality(AxisOrthogonalityInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAxisOrthogonality(input));
        }

        public void AxisOrthogonalityChanged(AxisOrthogonalityResult result)
        {
            AxisOrthogonalityChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelAxisOrthogonality()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAxisOrthogonality());
        }

        public Response<VoidResult> StartPixelSizeComputation(PixelSizeComputationInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartPixelSizeComputation(input));
        }

        public void PixelSizeComputationChanged(PixelSizeComputationResult result)
        {
            PixelSizeComputationChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelPixelSizeComputation()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelPixelSizeComputation());
        }

        public Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartImagePreprocessing(input));
        }

        public void ImagePreprocessingChanged(ImagePreprocessingResult imagePreprocessingResult)
        {
            ImagePreprocessingChangedEvent?.Invoke(imagePreprocessingResult);
        }

        public Response<VoidResult> StartPatternRec(PatternRecInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartPatternRec(input));
        }

        public void PatternRecChanged(PatternRecResult prResult)
        {
            PatternRecChangedEvent?.Invoke(prResult);
        }

        public Response<VoidResult> CancelPatternRec()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelPatternRec());
        }

        public Response<double> GetFlowImageScale()
        {
            return _algoService.TryInvokeAndGetMessages(s => s.GetFlowImageScale());
        }

        public Response<VoidResult> StartMultiSizeChuck(MultiSizeChuckInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartMultiSizeChuck(input));
        }

        public void MultiSizeChuckChanged(MultiSizeChuckResult result)
        {
            MultiSizeChuckChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelMultiSizeChuck()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelMultiSizeChuck());
        }

        public Response<VoidResult> StartDistortion(DistortionInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartDistortion(input));
        }

        public void DistortionChanged(DistortionResult result)
        {
            DistortionChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelDistortion()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelDistortion());
        }

        public Response<VoidResult> StartFilterCalibration(FilterCalibrationInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartFilterCalibration(input));
        }

        public void FilterCalibrationChanged(FilterCalibrationResult result)
        {
            FilterCalibrationChangedEvent.Invoke(result);
        }

        public Response<VoidResult> CancelFilterCalibration()
            => _algoService.TryInvokeAndGetMessages(x => x.CancelFilterCalibration());

        public Response<VoidResult> StartAutoExposure(AutoExposureInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAutoExposure(input));
        }

        public void AutoExposureChanged(AutoExposureResult result)
        {
            CameraExposureChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelAutoExposure()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAutoExposure());
        }

        public Response<VoidResult> StartGetZFocus(GetZFocusInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartGetZFocus(input));
        }

        public void GetZFocusChanged(GetZFocusResult result)
        {
            GetZFocusChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelGetZFocus()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelGetZFocus());
        }

        public Response<VoidResult> StartDistanceSensorCalibration(DistanceSensorCalibrationInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartDistanceSensorCalibration(input));
        }

        public void DistanceSensorCalibrationChanged(DistanceSensorCalibrationResult result)
        {
            DistanceSensorCalibrationChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelDistanceSensorCalibration()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelDistanceSensorCalibration());
        }
    }
}
