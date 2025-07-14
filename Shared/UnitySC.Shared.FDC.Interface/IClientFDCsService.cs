using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.FDC.Interface
{
    [ServiceContract]
    public interface IClientFDCsService
    {
        [OperationContract]
        Response<VoidResult> ClientStarted(string name);

        [OperationContract]
        Response<VoidResult> ClientIsRunning(string name);

        [OperationContract]
        Response<VoidResult> ClientStopped(string name);

        [OperationContract]
        Response<VoidResult> ApplicationModeLocalChanged(bool isInLocalMode);
    }
}
