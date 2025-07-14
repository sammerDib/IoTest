using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface.Chuck
{
    [ServiceContract(CallbackContract = typeof(IDMTChuckServiceCallback))]
    [ServiceKnownType(typeof(ANAChuckConfig))]
    [ServiceKnownType(typeof(DMTChuckConfig))]
    [ServiceKnownType(typeof(EMEChuckConfig))]
    public interface IDMTChuckService : IUSPChuckService
    {
        [OperationContract]
        Response<RfidTag> GetTag();
    }
}
