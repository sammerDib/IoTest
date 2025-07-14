using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract]
    public class WaferReferentialCalibrationData : ICalibrationData
    {
        [DataMember]
        public List<WaferConfiguration> WaferConfigurations { get; set; }
        [DataMember] 
        public DateTime CreationDate { get; set; }

        public static WaferReferentialCalibrationData ReadFrom(TextReader reader)
        {
            var deserializer = new XmlSerializer(typeof(WaferReferentialCalibrationData));
            return (WaferReferentialCalibrationData)deserializer.Deserialize(reader);
        }
    }
}
