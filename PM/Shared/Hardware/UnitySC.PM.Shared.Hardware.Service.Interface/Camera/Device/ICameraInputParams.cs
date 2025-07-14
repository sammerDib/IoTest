namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public interface ICameraInputParams
    {
        double Gain { get; set; }

        double ExposureTimeMs { get; set; }

        double FrameRate { get; set; }

        string ColorMode { get; set; }
    }
}