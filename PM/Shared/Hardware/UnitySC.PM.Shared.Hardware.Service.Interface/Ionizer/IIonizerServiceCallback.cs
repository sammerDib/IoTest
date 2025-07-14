using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer
{
    [ServiceContract]
    public interface IIonizerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(DeviceState state);

        [OperationContract(IsOneWay = true)]
        void AlarmChangedCallback(bool value);
    }
}
