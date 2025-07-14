using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [DataContract]
    [XmlInclude(typeof(XYPosition))]
    [XmlInclude(typeof(XYZTopZBottomPosition))]
    [XmlInclude(typeof(XYZPosition))]
    [XmlInclude(typeof(AnaPosition))]    
    public class SubstrateSlotConfig
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Length Diameter
        {
            get;
            set;
        }

        [DataMember]
        public bool IsPresenceSensorAvailable { get; set; }

        [DataMember]
        [XmlElement("PositionPark")]       
        public PositionBase PositionPark { get; set; }

        [DataMember]        
        [XmlElement("PositionManualLoad")]
        public PositionBase PositionManualLoad { get; set; }

        [DataMember]
        [XmlElement("PositionChuckCenter")]       
        public PositionBase PositionChuckCenter { get; set; } = new XYPosition { Referential = new MotorReferential(), X = 0d, Y = 0d };
    }

    [Serializable]
    [DataContract]
    public class SubstSlotWithPositionConfig : SubstrateSlotConfig
    {
        [DataMember]
        [XmlElement("PositionSensor")]
        public PositionBase PositionSensor { get; set; }              
    }
}
