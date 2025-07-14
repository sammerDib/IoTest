using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor
{
    [ServiceContract(CallbackContract = typeof(IDistanceSensorServiceCallback))]
    public interface IDistanceSensorService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<VoidResult> CustomCommand(string custom);
    }
}
