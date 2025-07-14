using System.ServiceModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter
{
    [ServiceContract(CallbackContract = typeof(IOpticalPowermeterServiceCallback))]
    public interface IOpticalPowermeterService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> Connect(PowerIlluminationFlow flow);

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent(PowerIlluminationFlow flow);

        [OperationContract]
        Response<VoidResult> CustomCommand(PowerIlluminationFlow flow, string custom);
    }
}
