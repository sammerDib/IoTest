using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    [ServiceContract(CallbackContract = typeof(ILightServiceCallback))]
    public interface ILightService
    {
        [OperationContract]
        Response<double> GetLightIntensity(string lightID);

        [OperationContract]
        Response<VoidResult> SetLightIntensity(string lightID, double intensity);

        [OperationContract]
        Response<VoidResult> SubscribeToLightChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToLightChanges();
    }
}
