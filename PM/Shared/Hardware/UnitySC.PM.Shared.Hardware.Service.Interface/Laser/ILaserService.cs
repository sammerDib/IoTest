using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [ServiceContract(CallbackContract = typeof(ILaserServiceCallback))]
    public interface ILaserService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> PowerOn();

        [OperationContract]
        Response<VoidResult> PowerOff();

        [OperationContract]
        Response<VoidResult> SetPower(int power);

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<VoidResult> CustomCommand(string custom);
    }
}
