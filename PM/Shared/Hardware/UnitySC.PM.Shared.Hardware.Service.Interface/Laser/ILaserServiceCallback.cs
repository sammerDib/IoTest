using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [ServiceContract]
    public interface ILaserServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(DeviceState state);

        [OperationContract(IsOneWay = true)]
        void InterlockStatusChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void LaserTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void PowerChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void CustomChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void LaserPowerStatusChangedCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void CrystalTemperatureChangedCallback(double value);
    }
}
