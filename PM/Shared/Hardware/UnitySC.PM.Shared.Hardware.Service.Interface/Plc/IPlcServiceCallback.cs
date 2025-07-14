using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Plc
{
    [ServiceContract]
    public interface IPlcServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(string state);

        [OperationContract(IsOneWay = true)]
        void IdChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void CustomChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void AmsNetIdChangedCallback(string value);
    }
}
