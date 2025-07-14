using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [ServiceContract]
    public interface ILaserPiano450ServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void CrystalTemperatureChangedCallback(double value);
    }
}
