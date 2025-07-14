using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer
{
    [ServiceContract]
    public interface ISpectroServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(DeviceState state);

        [OperationContract(IsOneWay = true)]
        void RawMeasuresCallback(SpectroSignal spectroSignal);
    }
}
