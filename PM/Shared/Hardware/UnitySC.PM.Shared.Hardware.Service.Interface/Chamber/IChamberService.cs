using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    [ServiceContract(CallbackContract = typeof(IChamberServiceCallback))]
    public interface IChamberService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<List<string>> GetWebcamUrls();
        [OperationContract]
        Task<Response<VoidResult>> ResetProcess();

        [OperationContract]
        Response<bool> SetChamberLightState(bool value);
    }
}

