using System;
using System.Xml.Serialization;

namespace MergeContext.Context
{
    public enum ExclusionType { Pad, Finger }

    public class PadFinger : AdcTools.Serializable
    {
        [XmlAttribute]
        public ExclusionType ExclusionType;

        /// <summary>
        /// Angle en degré du pad / finger
        /// </summary>
        [XmlAttribute]
        public double AngleDegree { get; set; }
        [XmlIgnore]
        public double Angle
        {
            get { return AngleDegree / 180 * Math.PI; }
            set { AngleDegree = value / Math.PI * 180; }
        }

        /// <summary>
        /// Taille en µm du pad / finger
        /// </summary>
        [XmlAttribute]
        public double Size { get; set; }

    }
}
