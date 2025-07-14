namespace UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer
{
    public interface ISpectroServiceCallbackProxy
    {
        void RawMeasuresCallback(SpectroSignal spectroSignal);
    }
}
