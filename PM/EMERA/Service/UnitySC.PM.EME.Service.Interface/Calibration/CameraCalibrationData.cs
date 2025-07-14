using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract]
    public class CameraCalibrationData : ICalibrationData
    {
        [DataMember]
        public Length PixelSize { get; set; } = 1.Micrometers();

        [DataMember]
        public DateTime CreationDate { get; set; }

        public static CameraCalibrationData ReadFrom(TextReader reader)
        {
            var deserializer = new XmlSerializer(typeof(CameraCalibrationData));
            return (CameraCalibrationData)deserializer.Deserialize(reader);
        }
    }
}
