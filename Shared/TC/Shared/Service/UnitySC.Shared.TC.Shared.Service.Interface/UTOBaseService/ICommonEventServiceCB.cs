using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface ICommonEventServiceCB
    {
        [OperationContract(IsOneWay = true)]
        void FireEvent(CommonEvent ecid);
    }
}
