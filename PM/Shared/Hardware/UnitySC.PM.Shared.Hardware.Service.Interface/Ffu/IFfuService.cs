using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ffu
{
    [ServiceContract(CallbackContract = typeof(IFfuServiceCallback))]
    public interface IFfuService
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
        Response<VoidResult> SetSpeed(ushort speedPercent);

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<VoidResult> CustomCommand(string custom);

        [OperationContract]
        Response<Dictionary<string, ushort>> GetDefaultFfuValues();
    }
}
