using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [ServiceContract]
    [ServiceKnownType(typeof(BareWaferAlignmentResult))]
    [ServiceKnownType(typeof(BareWaferAlignmentImage))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(MotorReferential))]
    public interface IAlgoServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void AFLiseChanged(AFLiseResult afResult);

        [OperationContract(IsOneWay = true)]
        void AFCameraChanged(AFCameraResult afResult);

        [OperationContract(IsOneWay = true)]
        void AutoFocusChanged(AutofocusResult afResult);

        [OperationContract(IsOneWay = true)]
        void BwaChanged(BareWaferAlignmentChangeInfo bwaResult);

        [OperationContract(IsOneWay = true)]
        void BwaImageChanged(BareWaferAlignmentImage bwaResult);

        [OperationContract(IsOneWay = true)]
        void AutoLightChanged(AutolightResult alResult);

        [OperationContract(IsOneWay = true)]
        void CheckWaferPresenceChanged(CheckWaferPresenceResult alResult);

        [OperationContract(IsOneWay = true)]
        void ImagePreprocessingChanged(ImagePreprocessingResult prResult);

        [OperationContract(IsOneWay = true)]
        void PatternRecChanged(PatternRecResult prResult);

        [OperationContract(IsOneWay = true)]
        void AutoAlignChanged(AutoAlignResult alResult);

        [OperationContract(IsOneWay = true)]
        void AlignmentMarksChanged(AlignmentMarksResult amResult);

        [OperationContract(IsOneWay = true)]
        void CheckPatternRecChanged(CheckPatternRecResult checkPatternRecResult);

        [OperationContract(IsOneWay = true)]
        void DieAndStreetSizesChanged(DieAndStreetSizesResult dsapResult);

        [OperationContract(IsOneWay = true)]
        void WaferMapChanged(WaferMapResult dsapResult);
    }
}
