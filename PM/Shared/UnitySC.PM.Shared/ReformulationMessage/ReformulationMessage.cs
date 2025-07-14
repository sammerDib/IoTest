using System;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.ReformulationMessage
{
    public class ReformulationMessage
    {
        [XmlElement("Content")]
        public string Content { get; set; }

        [XmlElement("UserContent")]
        public string UserContent { get; set; }

        [XmlElement("Source")]
        public string Source { get; set; }

        [XmlElement("Level")]
        public string LevelString
        {
            get
            {
                return Level.ToString();
            }
            set
            {
                Level = (MessageLevel)Enum.Parse(typeof(MessageLevel), value);
            }
        }

        [XmlIgnore]
        public MessageLevel Level
        {
            get; set;
        }
    }
}