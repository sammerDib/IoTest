using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Plc
{
    [ServiceContract(CallbackContract = typeof(IPlcServiceCallback))]
    public interface IPlcService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<VoidResult> SmokeDetectorResetAlarm();

        [OperationContract]
        Response<VoidResult> Restart();

        [OperationContract]
        Response<VoidResult> Reboot();

        [OperationContract]
        Response<VoidResult> CustomCommand(string custom);
    }
}
