using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Chuck
{
    [ServiceContract(CallbackContract = typeof(IEMEChuckServiceCallback))]
    [ServiceKnownType(typeof(ANAChuckConfig))]
    [ServiceKnownType(typeof(DMTChuckConfig))]
    [ServiceKnownType(typeof(EMEChuckConfig))]
    public interface IEMEChuckService : IUSPChuckService
    {
        [OperationContract]
        Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer);

        [OperationContract]
        Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer);
    }
}
