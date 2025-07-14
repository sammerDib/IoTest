using System.ServiceModel;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public delegate void VignettingChangedHandler(VignettingResult vignettingResult);

    public delegate void AutoFocusCameraChangedHandler(AutoFocusCameraResult autoFocusCameraResult);

    public delegate void AxisOrthogonalityChangedHandler(AxisOrthogonalityResult axisOrthogonalityResult);

    public delegate void PixelSizeComputationChangedHandler(PixelSizeComputationResult pixelSizeComputationResult);

    public delegate void ImagePreprocessingChangedHandler(ImagePreprocessingResult imagePreprocessingResult);

    public delegate void PatternRecChangedHandler(PatternRecResult patternRecResult);

    public delegate void MultiSizeChuckChangedHandler(MultiSizeChuckResult multiSizeChuckResult);

    public delegate void DistortionChangedHandler(DistortionResult distortionResult);

    public delegate void FilterCalibrationChangedHandler(FilterCalibrationResult result);

    public delegate void GetZFocusChangedHandler(GetZFocusResult result);

    public delegate void CameraExposureChangedHandler(AutoExposureResult result);

    public delegate void DistanceSensorCalibrationChangedHandler(DistanceSensorCalibrationResult result);

    [ServiceContract]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(MotorReferential))]
    public interface IAlgoServiceCallback
    {
        event VignettingChangedHandler VignettingChangedEvent;
        event AutoFocusCameraChangedHandler AutoFocusCameraChangedEvent;
        event AxisOrthogonalityChangedHandler AxisOrthogonalityChangedEvent;
        event PixelSizeComputationChangedHandler PixelSizeComputationChangedEvent;
        event ImagePreprocessingChangedHandler ImagePreprocessingChangedEvent;
        event PatternRecChangedHandler PatternRecChangedEvent;
        event MultiSizeChuckChangedHandler MultiSizeChuckChangedEvent;
        event DistortionChangedHandler DistortionChangedEvent;
        event FilterCalibrationChangedHandler FilterCalibrationChangedEvent;
        event CameraExposureChangedHandler CameraExposureChangedEvent;
        event GetZFocusChangedHandler GetZFocusChangedEvent;
        event DistanceSensorCalibrationChangedHandler DistanceSensorCalibrationChangedEvent;

        [OperationContract(IsOneWay = true)]
        void AutoFocusCameraChanged(AutoFocusCameraResult autoFocusResult);

        [OperationContract(IsOneWay = true)]
        void VignettingChanged(VignettingResult vignettingResult);

        [OperationContract(IsOneWay = true)]
        void AxisOrthogonalityChanged(AxisOrthogonalityResult result);

        [OperationContract(IsOneWay = true)]
        void PixelSizeComputationChanged(PixelSizeComputationResult result);

        [OperationContract(IsOneWay = true)]
        void ImagePreprocessingChanged(ImagePreprocessingResult imagePreprocessingResult);

        [OperationContract(IsOneWay = true)]
        void PatternRecChanged(PatternRecResult prResult);

        [OperationContract(IsOneWay = true)]
        void MultiSizeChuckChanged(MultiSizeChuckResult result);

        [OperationContract(IsOneWay = true)]
        void FilterCalibrationChanged(FilterCalibrationResult result);

        [OperationContract(IsOneWay = true)]
        void DistortionChanged(DistortionResult result);

        [OperationContract(IsOneWay = true)]
        void AutoExposureChanged(AutoExposureResult result);

        [OperationContract(IsOneWay = true)]
        void GetZFocusChanged(GetZFocusResult result);

        [OperationContract(IsOneWay = true)]
        void DistanceSensorCalibrationChanged(DistanceSensorCalibrationResult result);
    }
}
