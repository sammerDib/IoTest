using System.ServiceModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Mppc
{
    [ServiceContract(CallbackContract = typeof(IMppcServiceCallback))]
    public interface IMppcService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> Connect(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> SetOutputVoltage(MppcCollector collector, double voltage);

        [OperationContract]
        Response<VoidResult> TempCorrectionFactorSetting(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> SwitchTempCompensationMode(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> RefVoltageTempSetting(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> PowerFctSetting(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> OutputVoltageOn(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> OutputVoltageOff(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> PowerReset(MppcCollector collector);

        [OperationContract]
        Response<VoidResult> CustomCommand(MppcCollector collector, string custom);

        [OperationContract]
        Response<VoidResult> ManageRelays(MppcCollector collector, bool relayActivated);
    }
}
