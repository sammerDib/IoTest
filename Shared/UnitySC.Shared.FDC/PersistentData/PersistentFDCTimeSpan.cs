using System;

using System.Xml.Serialization;
using System.Globalization;
using System.Xml;

namespace UnitySC.Shared.FDC.PersistentData
{
    [Serializable]
    public class PersistentFDCTimeSpan : IPersistentFDCData
    {
        // Default constructor requested for Serialization
        public PersistentFDCTimeSpan() { }

        public PersistentFDCTimeSpan(string name) : this(name, TimeSpan.Zero) { }

        public PersistentFDCTimeSpan(string name, TimeSpan ts)
        {
            FDCName = name;
            Timespan = ts;
        }
        public string FDCName { get; set; }

       // Warning : Timespan is not serializable in TextFormat Ok in BinaryFormat
       [XmlIgnore]
        public TimeSpan Timespan { get; set; }

        // ISO 8601 duration string property - only for serialization purpose - use Timespan property to collect duration
        [XmlElement("Duration")]
        public string XmlDuration
        {
            get
            {
                return XmlConvert.ToString(Timespan);
            }
            set
            {
                Timespan = string.IsNullOrEmpty(value) ?  TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
