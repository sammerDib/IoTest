using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Camera
{
    [DataContract]
    public class CameraInputParams : ICameraInputParams
    {
        [DataMember]
        public double Gain { get; set; }

        [DataMember]
        public double ExposureTimeMs { get; set; }

        [DataMember]
        public double FrameRate { get; set; }

        [DataMember]
        public string ColorMode { get; set; }
    }
}
