using System.ServiceModel;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [ServiceContract(CallbackContract = typeof(IAlgoServiceCallback))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(XYPosition))]
    public interface IAlgoService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToAlgoChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToAlgoChanges();

        [OperationContract]
        Response<VoidResult> StartAutoFocusCamera(AutoFocusCameraInput input);

        [OperationContract]
        Response<VoidResult> CancelAutoFocusCamera();

        [OperationContract]
        Response<VoidResult> StartVignetting(VignettingInput input);

        [OperationContract]
        Response<VoidResult> CancelVignetting();

        [OperationContract]
        Response<VoidResult> StartAxisOrthogonality(AxisOrthogonalityInput input);

        [OperationContract]
        Response<VoidResult> CancelAxisOrthogonality();

        [OperationContract]
        Response<VoidResult> StartPixelSizeComputation(PixelSizeComputationInput input);

        [OperationContract]
        Response<VoidResult> CancelPixelSizeComputation();

        [OperationContract]
        Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input);

        [OperationContract]
        Response<VoidResult> StartPatternRec(PatternRecInput input);

        [OperationContract]
        Response<VoidResult> CancelPatternRec();

        [OperationContract]
        Response<double> GetFlowImageScale();

        [OperationContract]
        Response<VoidResult> StartMultiSizeChuck(MultiSizeChuckInput input);

        [OperationContract]
        Response<VoidResult> CancelMultiSizeChuck();

        [OperationContract]
        Response<VoidResult> StartDistortion(DistortionInput input);

        [OperationContract]
        Response<VoidResult> CancelDistortion();

        [OperationContract]
        Response<VoidResult> StartFilterCalibration(FilterCalibrationInput input);

        [OperationContract]
        Response<VoidResult> CancelFilterCalibration();

        [OperationContract]
        Response<VoidResult> StartAutoExposure(AutoExposureInput input);

        [OperationContract]
        Response<VoidResult> CancelAutoExposure();

        [OperationContract]
        Response<VoidResult> StartGetZFocus(GetZFocusInput input);

        [OperationContract]
        Response<VoidResult> CancelGetZFocus();        

        [OperationContract]
        Response<VoidResult> StartDistanceSensorCalibration(DistanceSensorCalibrationInput input);

        [OperationContract]
        Response<VoidResult> CancelDistanceSensorCalibration();
    }
}
