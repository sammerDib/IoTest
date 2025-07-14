using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer
{
    [ServiceContract(CallbackContract = typeof(IIonizerServiceCallback))]
    public interface IIonizerService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> OpenAirValve();
    }
}
