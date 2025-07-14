using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Chamber
{
    [ServiceContract(CallbackContract = typeof(IEMEChamberServiceCallback))]
    public interface IEMEChamberService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<EMEChamberConfig> GetChamberConfiguration();

        [OperationContract]
        Response<VoidResult> OpenSlitDoor();

        [OperationContract]
        Response<VoidResult> CloseSlitDoor();
    }
}
