using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor
{
    [ServiceContract]
    public interface IDistanceSensorServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(DeviceState state);

        [OperationContract(IsOneWay = true)]
        void DistanceChangedCallback(double distance);

        [OperationContract(IsOneWay = true)]
        void IdChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void CustomChangedCallback(string value);
    }
}
