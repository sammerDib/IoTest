using System.ServiceModel;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IDMTAlgorithmsServiceCallback))]
    public interface IDMTAlgorithmsService
    {
        [OperationContract]
        Response<VoidResult> Subscribe();

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<VoidResult> StartAutoExposureOnMeasure(MeasureBase measure);

        [OperationContract]
        Response<VoidResult> StartAutoExposure(Side side, MeasureType measureType, bool ignorePerspectiveCalibration);

        [OperationContract]
        Response<VoidResult> CancelAutoExposure();
    }
}
