using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Algo;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract]
    public class DistortionCalibrationData : ICalibrationData
    {
        [DataMember]
        public DistortionData DistortionData { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        public static DistortionCalibrationData ReadFrom(TextReader reader)
        {
            var deserializer = new XmlSerializer(typeof(DistortionCalibrationData));
            return (DistortionCalibrationData)deserializer.Deserialize(reader);
        }
    }
}
