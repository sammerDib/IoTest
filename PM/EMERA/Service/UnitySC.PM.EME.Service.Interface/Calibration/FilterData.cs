using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract]
    public class FilterData : ICalibrationData
    {
        [DataMember] public List<Filter> Filters { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        public static FilterData ReadFrom(TextReader reader)
        {
            var deserializer = new XmlSerializer(typeof(FilterData));
            return (FilterData)deserializer.Deserialize(reader);
        }
    }
}
