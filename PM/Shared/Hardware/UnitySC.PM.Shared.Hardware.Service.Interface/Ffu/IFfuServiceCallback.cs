using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ffu
{
    [ServiceContract]
    public interface IFfuServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(DeviceState state);

        [OperationContract(IsOneWay = true)]
        void StatusChangedCallback(string status);

        [OperationContract(IsOneWay = true)]
        void CurrentSpeedChangedCallback(ushort value);

        [OperationContract(IsOneWay = true)]
        void TemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void WarningChangedCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void AlarmChangedCallback(bool value);
        
        [OperationContract(IsOneWay = true)]
        void CustomChangedCallback(string value);
    }
}
