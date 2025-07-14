namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public delegate void IntensityChangedEventHandler(object sender, double intensity);

    public interface ILight : IDevice
    {
        double GetIntensity();

        void SetIntensity(double intensity);
    }
}
