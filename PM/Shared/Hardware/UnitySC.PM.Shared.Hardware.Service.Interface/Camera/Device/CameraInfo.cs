using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Camera
{
    [DataContract]
    public class CameraInfo
    {
        [DataMember]
        public string Model { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// Valeur mini pour l'exposure time, en seconds
        /// </summary>
        [DataMember]
        public double MinExposureTimeMs { get; set; }

        /// <summary>
        /// Valeur maxi pour l'exposure time, en seconds
        /// </summary>
        [DataMember]
        public double MaxExposureTimeMs { get; set; }

        [DataMember]
        public double MinGain { get; set; }

        [DataMember]
        public double MaxGain { get; set; }

        [DataMember]
        public List<string> ColorModes { get; set; }

        [DataMember]
        public double MinFrameRate { get; set; }

        [DataMember]
        public double MaxFrameRate { get; set; }

        [DataMember]
        public string DeadPixelsFile { get; set; }

        public string ExposureTimeRange { get { return $"{MinExposureTimeMs:F3} - {MaxExposureTimeMs:F3}"; } }

        public string GainRange { get { return $"{MinGain:F2} - {MaxGain:F2}"; } }

        public string FrameRateRange { get { return $"{MinFrameRate:F3} - {MaxFrameRate:F3}"; } }

    }
}
