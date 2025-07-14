using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Status.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IGlobalStatusServiceCallback))]
    public interface IGlobalStatusService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToGlobalStatusChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToGlobalStatusChanges();

        [OperationContract]
        Response<PMGlobalStates> GetServerState();

        [OperationContract]
        Response<PMConfiguration> GetConfiguration();

        [OperationContract]
        Response<bool> ReserveHardware();

        [OperationContract]
        Response<bool> ReleaseHardware();

        [OperationContract]
        Response<bool> ResetHardware();

        [OperationContract]
        Response<VoidResult> ClearAllMessages();

        [OperationContract]
        Response<VoidResult> ClearMessage(Message messageToClear);
    }
}
