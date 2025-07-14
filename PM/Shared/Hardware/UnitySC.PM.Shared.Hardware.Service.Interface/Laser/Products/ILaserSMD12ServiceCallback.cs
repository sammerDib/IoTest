using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [ServiceContract]
    public interface ILaserSMD12ServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PsuTemperatureChangedCallback(double value);
    }
}
