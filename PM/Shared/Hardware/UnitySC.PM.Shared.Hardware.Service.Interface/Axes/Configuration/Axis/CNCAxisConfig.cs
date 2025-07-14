using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class CNCAxisConfig : MotorizedAxisConfig
    {
        [DataMember]
        public virtual int NumberLenses { get; set; }

        [DataMember]
        [XmlArray("NameLenses")]
        [XmlArrayItem("NameLens")]
        public virtual List<string> NameLenses { get; set; }
    }
}
