using System.Windows;

namespace UnitySC.PM.EME.Service.Core.Flows
{
    internal struct CameraSettings
    {
        public Size ImageResolution { get; set; }
        public double FrameRate { get; set; }
        public double ExposureTimeMs { get; set; }
    }
}
