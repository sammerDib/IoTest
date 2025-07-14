using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MergeContext.Context
{
    public class ChamberSettings : AdcTools.Serializable
    {
        public List<ExclusionArea> Exclusions { get; set; } = new List<ExclusionArea>();
        public List<PadFinger> PadFingers { get; set; } = new List<PadFinger>();
    }

    /// <summary>
    /// Zone d'exclusion
    /// L'angle 0° correspond au 0 trigonométrique et on troune dans le sens trigonométrique.
    /// </summary>
    public class ExclusionArea
    {
        /// <summary>
        /// Angle en degré du début de la zone d'exclusion
        /// </summary>
        [XmlAttribute]
        public double StartExclusionAngleDegree { get; set; }
        [XmlIgnore]
        public double StartExclusionAngle
        {
            get { return StartExclusionAngleDegree / 180 * Math.PI; }
            set { StartExclusionAngleDegree = value / Math.PI * 180; }
        }

        /// <summary>
        /// Angle en degré de la fin de la zone d'exclusion
        /// </summary>
        [XmlAttribute]
        public double EndExclusionAngleDegree { get; set; }
        [XmlIgnore]
        public double EndExclusionAngle
        {
            get { return EndExclusionAngleDegree / 180 * Math.PI; }
            set { EndExclusionAngleDegree = value / Math.PI * 180; }
        }

    }
}
