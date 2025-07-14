using System.ServiceModel;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract]
    public interface ILiseHFServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void LaserPowerStatusChangedCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void InterlockStatusChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void LaserTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void CrystalTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void AttenuationPositionChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void FastAttenuationPositionChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void ShutterIrisPositionChangedCallback(string value);
    }
}
