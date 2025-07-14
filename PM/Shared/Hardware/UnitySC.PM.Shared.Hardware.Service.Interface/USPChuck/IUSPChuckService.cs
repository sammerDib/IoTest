using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [ServiceContract(CallbackContract = typeof(IUSPChuckServiceCallback))]
    [ServiceKnownType(typeof(EMEChuckConfig))]
    [ServiceKnownType(typeof(ANAChuckConfig))]
    [ServiceKnownType(typeof(DMTChuckConfig))]
    public interface IUSPChuckService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> RefreshAllValues();

        [OperationContract]
        Response<ChuckState> GetCurrentState();       
    }
}
