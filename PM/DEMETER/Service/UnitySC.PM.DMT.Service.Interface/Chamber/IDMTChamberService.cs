using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface.Chamber
{
    [ServiceContract(CallbackContract = typeof(IDMTChamberServiceCallback))]
    public interface IDMTChamberService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<DMTChamberConfig> GetChamberConfiguration();

        [OperationContract]
        Response<VoidResult> OpenSlitDoor();

        [OperationContract]
        Response<VoidResult> CloseSlitDoor();

        [OperationContract]
        Response<VoidResult> OpenCdaPneumaticValve();

        [OperationContract]
        Response<VoidResult> CloseCdaPneumaticValve();
    }
}
