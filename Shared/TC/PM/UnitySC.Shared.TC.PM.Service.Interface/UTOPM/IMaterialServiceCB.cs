using System.ServiceModel;

namespace UnitySC.Shared.TC.PM.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface IMaterialServiceCB
    {
        [OperationContract(IsOneWay = true)]
        void PMReadyToTransfer();               // PM en position chargement
    }
}
