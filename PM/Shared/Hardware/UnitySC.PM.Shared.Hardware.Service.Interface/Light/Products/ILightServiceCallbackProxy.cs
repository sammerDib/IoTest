namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    public interface ILightServiceCallbackProxy
    {
        void LightIntensityChanged(string lightID, double intensity);
    }
}
