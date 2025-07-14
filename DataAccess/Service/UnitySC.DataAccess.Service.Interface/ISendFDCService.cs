using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(ISendFdcServiceCallback))]
    public interface ISendFdcService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> RequestAllFDCsUpdate();

    }
}
