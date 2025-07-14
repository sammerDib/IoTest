using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    public interface ILightServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void LightIntensityChangedCallback(string lightID, double intensity);
    }
}
