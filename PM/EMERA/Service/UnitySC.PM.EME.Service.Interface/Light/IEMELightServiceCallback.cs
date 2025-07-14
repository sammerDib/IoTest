using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Light;

namespace UnitySC.PM.EME.Service.Interface.Light
{
    [ServiceContract]
    public interface IEMELightServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateLightSourceCallback(LightSourceMessage lightSource);
    }
}
