using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [ServiceContract(CallbackContract = typeof(IAlgoServiceCallback))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(PatternRecognitionDataWithContext))]
    public interface IAlgoService
    {
        [OperationContract]
        Response<AFLiseSettings> GetAFLiseSettings(AFLiseInput input, ObjectiveConfig objectiveConfig = null);

        [OperationContract]
        Response<VoidResult> StartAFLise(AFLiseInput input);

        [OperationContract]
        Response<VoidResult> CancelAFLise();

        [OperationContract]
        Response<VoidResult> StartAutoFocus(AutofocusInput input);

        [OperationContract]
        Response<VoidResult> CancelAutoFocus();

        [OperationContract]
        Response<VoidResult> StartAFCamera(AFCameraInput input);

        [OperationContract]
        Response<VoidResult> CancelAFCamera();

        [OperationContract]
        Response<BareWaferAlignmentSettings> GetBWASettings(BareWaferAlignmentInput input);

        [OperationContract]
        Response<VoidResult> StartBWA(BareWaferAlignmentInput input);

        [OperationContract]
        Response<VoidResult> CancelBWA();

        [OperationContract]
        Response<VoidResult> StartBWAImage(BareWaferAlignmentImageInput input);

        [OperationContract]
        Response<VoidResult> CancelBWAImage();

        [OperationContract]
        Response<VoidResult> StartAutoLight(AutolightInput input);

        [OperationContract]
        Response<VoidResult> CheckWaferPresence(CheckWaferPresenceInput input);

        [OperationContract]
        Response<VoidResult> CancelAutoLight();

        [OperationContract]
        Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input);

        [OperationContract]
        Response<VoidResult> StartPatternRec(PatternRecInput input);

        [OperationContract]
        Response<VoidResult> CancelPatternRec();

        [OperationContract]
        Response<VoidResult> SubscribeToAlgoChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToAlgoChanges();

        [OperationContract]
        Response<VoidResult> StartAutoAlign(AutoAlignInput input);

        [OperationContract]
        Response<VoidResult> CancelAutoAlign();

        [OperationContract]
        Response<VoidResult> StartAlignmentMarks(AlignmentMarksInput input);

        [OperationContract]
        Response<VoidResult> CancelAlignmentMarks();

        [OperationContract]
        Response<CheckPatternRecSettings> GetCheckPatternRecSettings();

        [OperationContract]
        Response<VoidResult> StartCheckPatternRec(CheckPatternRecInput checkPatternRecInput);

        [OperationContract]
        Response<VoidResult> CancelCheckPatternRec();

        [OperationContract]
        Response<VoidResult> StartDieAndStreetSizes(DieAndStreetSizesInput input);

        [OperationContract]
        Response<VoidResult> CancelDieAndStreetSizes();

        [OperationContract]
        Response<VoidResult> StartWaferMap(WaferMapInput input);

        [OperationContract]
        Response<VoidResult> CancelWaferMap();
    }
}
