using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Shutter
{
    [ServiceContract(CallbackContract = typeof(IShutterServiceCallback))]
    public interface IShutterService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<VoidResult> OpenShutterCommand();

        [OperationContract]
        Response<VoidResult> CloseShutterCommand();
    }
}
