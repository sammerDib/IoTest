using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data.FDC
{
    public enum FDCValueType
    {
        TypeDouble,
        TypeInt,
        TypeString
    }

    public enum FDCSendFrequency
    {
        Year,
        Month,
        Week,
        Day,
        Hour,
        Hours12,
        Minutes30,
        Minutes20,
        Minutes15,
        Minutes10,
        Minutes5,
        Minute,
        Seconds30,
        Seconds10,
        Once,
        CustomDelay,
        Never
    }

    [DataContract]
    [Serializable]
    public class FDCItemConfig
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FDCValueType ValueType { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public FDCSendFrequency SendFrequency { get; set; }

        [DataMember]
        public bool CanBeReset { get; set; }

        [DataMember]
        public bool CanInitValue { get; set; }

        [DataMember]
        public double InitValue { get; set; }

        [XmlIgnore]
        [DataMember]
        public TimeSpan CustomDelay { get; set; }

        // Property for XML serialization as a formatted string
        [XmlElement("CustomDelay")]
        public string CustomDelayString
        {
            get => CustomDelay.ToString("d'.'hh':'mm':'ss");
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CustomDelay = TimeSpan.ParseExact(value, "d'.'hh':'mm':'ss", null);
                }
                else
                {
                    CustomDelay = new TimeSpan(0);
                }
            }
        }


    }
}
