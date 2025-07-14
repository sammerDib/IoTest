using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Camera.IDSCamera
{
    public enum Mode
    {
        Unknown,
        Bright,
        Normal,
    }

    [DataContract]
    public struct IDSCameraMode
    {
        [DataMember]
        public Mode Mode { get; set; }

        /// <summary>
        /// Exposure time in milliseconds (ms).
        /// </summary>
        [DataMember]
        public double ExposureTimeMs { get; set; }

        /// <summary>
        /// Framerate in frames per second (fps).
        /// </summary>
        [DataMember]
        public double Framerate { get; set; }
    }
}
