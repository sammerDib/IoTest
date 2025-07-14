using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasurePoint
    {
        public MeasurePoint()
        {
        }

        public MeasurePoint(int id, PointPosition position, PatternRecognitionDataWithContext patternRec = null, bool isDiePosition = false)
        {
            Id = id;
            Position = position;
            PatternRec = patternRec;
            IsDiePosition = isDiePosition;
        }

        public MeasurePoint(int id, double x, double y, bool isDiePosition)
            : this(id, new PointPosition(x, y), null, isDiePosition)
        {
        }

        public MeasurePoint(int id, XYZTopZBottomPosition position, bool isDiePosition)
            : this(id, new PointPosition(position), null, isDiePosition)
        {
        }

        [DataMember]
        public PatternRecognitionDataWithContext PatternRec { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public PointPosition Position { get; set; }

        [DataMember]
        public bool IsDiePosition { get; set; }

        [DataMember]
        public bool IsSubMeasurePoint { get; set; } = false;
    }

    [DataContract]
    public class PointPosition : IEquatable<PointPosition>
    {
        public PointPosition()
        {
        }

        public PointPosition(double x, double y, double zTop = double.NaN, double zBottom = double.NaN)
        {
            X = x;
            Y = y;
            ZTop = zTop;
            ZBottom = zBottom;
        }

        public PointPosition(XYZTopZBottomPosition position)
            : this(position.X, position.Y, position.ZTop, position.ZBottom)
        {
        }

        [DataMember]
        [XmlAttribute("X")]
        public double X { get; set; }

        [DataMember]
        [XmlAttribute("Y")]
        public double Y { get; set; }

        [DataMember]
        [XmlAttribute("ZTop")]
        public double ZTop { get; set; }

        [DataMember]
        [XmlAttribute("ZBottom")]
        public double ZBottom { get; set; }

        public XYPosition ToXYPosition(ReferentialBase referential)
        {
            return new XYPosition(
                referential,
                X,
                Y
            );
        }

        public XYZTopZBottomPosition ToXYZTopZBottomPosition(ReferentialBase referential)
        {
            return new XYZTopZBottomPosition(
                referential,
                X,
                Y,
                ZTop,
                ZBottom
            );
        }

        public bool Equals(PointPosition other)
        {
            if (other == null) return false;

            return ToXYZTopZBottomPosition(null).Equals(other.ToXYZTopZBottomPosition(null));
        }
    }
}
