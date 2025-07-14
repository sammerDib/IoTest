using System.ServiceModel;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IAcquisitionServiceCallback))]
    public interface IAcquisitionService
    {
        [OperationContract]
        Response<VoidResult> Open();

        [OperationContract]
        Response<VoidResult> Close();

        Response<VoidResult> UnSubscribeToChanges();

        Response<VoidResult> SubscribeToChanges();
    }
}
