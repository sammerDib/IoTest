using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    public class FakeAlgoSupervisor : IAlgoSupervisor
    {
        public List<Filter> ExpectedFilterCalibrationResult { get; set; }
        public AutoExposureResult ExpectedAutoExposure { get; set; }

#pragma warning disable CS0067 //The event 'xxx' is never used
        public event VignettingChangedHandler VignettingChangedEvent;

        public event AutoFocusCameraChangedHandler AutoFocusCameraChangedEvent;

        public event AxisOrthogonalityChangedHandler AxisOrthogonalityChangedEvent;

        public event ImagePreprocessingChangedHandler ImagePreprocessingChangedEvent;

        public event PatternRecChangedHandler PatternRecChangedEvent;

        public event MultiSizeChuckChangedHandler MultiSizeChuckChangedEvent;

        public event PixelSizeComputationChangedHandler PixelSizeComputationChangedEvent;

        public event FilterCalibrationChangedHandler FilterCalibrationChangedEvent;

        public event CameraExposureChangedHandler CameraExposureChangedEvent;

        public event DistortionChangedHandler DistortionChangedEvent;

        public event GetZFocusChangedHandler GetZFocusChangedEvent;
        public event DistanceSensorCalibrationChangedHandler DistanceSensorCalibrationChangedEvent;
#pragma warning restore CS0067 //The event 'xxx' is never used

        public Response<VoidResult> SubscribeToAlgoChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnsubscribeToAlgoChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartAutoFocusCamera(AutoFocusCameraInput input)
        {
            var fakeResult = new AutoFocusCameraResult()
            {
                SensorDistance = 1.2,                
                Status = new FlowStatus(FlowState.Success)
            };
            AutoFocusCameraChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public void AutoFocusCameraChanged(AutoFocusCameraResult autoFocusResult)
        {
            AutoFocusCameraChangedEvent?.Invoke(autoFocusResult);
        }

        public Response<VoidResult> CancelAutoFocusCamera()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartAxisOrthogonality(AxisOrthogonalityInput input)
        {
            throw new NotImplementedException();
        }

        public void AxisOrthogonalityChanged(AxisOrthogonalityResult result)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelAxisOrthogonality()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartDistortion(DistortionInput input)
        {
            throw new NotImplementedException();
        }

        public void DistortionChanged(DistortionResult result)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelDistortion()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartMultiSizeChuck(MultiSizeChuckInput input)
        {
            throw new NotImplementedException();
        }

        public void MultiSizeChuckChanged(MultiSizeChuckResult result)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelMultiSizeChuck()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartPatternRec(PatternRecInput input)
        {
            throw new NotImplementedException();
        }

        public void PatternRecChanged(PatternRecResult prResult)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelPatternRec()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartPixelSizeComputation(PixelSizeComputationInput input)
        {
            var fakeResult = new PixelSizeComputationResult()
            {
                PixelSize = 42.Millimeters(),
                Status = new FlowStatus(FlowState.Success)
            };
            PixelSizeComputationChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public void PixelSizeComputationChanged(PixelSizeComputationResult result)
        {
            PixelSizeComputationChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelPixelSizeComputation()
        {
            var fakeResult = new PixelSizeComputationResult()
            {
                PixelSize = null,
                Status = new FlowStatus(FlowState.Canceled)
            };
            PixelSizeComputationChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public Response<VoidResult> StartVignetting(VignettingInput input)
        {
            throw new NotImplementedException();
        }

        public void VignettingChanged(VignettingResult vignettingResult)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelVignetting()
        {
            throw new NotImplementedException();
        }

        public Response<double> GetFlowImageScale()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input)
        {
            throw new NotImplementedException();
        }

        public void ImagePreprocessingChanged(ImagePreprocessingResult imagePreprocessingResult)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartFilterCalibration()
        {
            var fakeResult = new FilterCalibrationResult()
            {
                Filters = ExpectedFilterCalibrationResult
            };
            FilterCalibrationChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public void FilterCalibrationChanged(FilterCalibrationResult result)
        {
            FilterCalibrationChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> StartFilterCalibration(FilterCalibrationInput input)
        {
            var fakeResult = new FilterCalibrationResult()
            {
                Filters = ExpectedFilterCalibrationResult,
                Status = new FlowStatus(FlowState.Success)
            };
            FilterCalibrationChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public Response<VoidResult> StartAutoExposure(AutoExposureInput input)
        {
            AutoExposureChanged(ExpectedAutoExposure);
            return new Response<VoidResult>();
        }

        public void AutoExposureChanged(AutoExposureResult result)
        {
            CameraExposureChangedEvent?.Invoke(result);
        }

        public Response<VoidResult> CancelFilterCalibration()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelAutoExposure()
        {
            var cancelResult = new AutoExposureResult()
            {
                Brightness = 0,
                ExposureTime = 0,
                Status = new FlowStatus(FlowState.Canceled)
            };
            AutoExposureChanged(cancelResult);
            return new Response<VoidResult>();
        }

        public Response<VoidResult> StartGetZFocus(GetZFocusInput input)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> CancelGetZFocus()
        {
            throw new NotImplementedException();
        }

        public void GetZFocusChanged(GetZFocusResult result)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartDistanceSensorCalibration(DistanceSensorCalibrationInput input)
        {
            var fakeResult = new DistanceSensorCalibrationResult()
            {
                OffsetX = 42.Millimeters(),
                OffsetY = 42.Millimeters(),
                Status = new FlowStatus(FlowState.Success)
            };
            DistanceSensorCalibrationChanged(fakeResult);
            return new Response<VoidResult>();
        }

        public Response<VoidResult> CancelDistanceSensorCalibration()
        {
            throw new NotImplementedException();
        }

        public void DistanceSensorCalibrationChanged(DistanceSensorCalibrationResult result)
        {
            DistanceSensorCalibrationChangedEvent?.Invoke(result);
        }
    }
}
