using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Shutter
{
    [ServiceContract]
    public interface IShutterServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(string state);

        [OperationContract(IsOneWay = true)]
        void ShutterIrisPositionChangedCallback(string shutterIrisPosition);
    }
}