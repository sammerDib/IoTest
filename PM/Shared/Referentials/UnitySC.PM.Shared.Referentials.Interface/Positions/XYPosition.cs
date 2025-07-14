using System;

using System.Runtime.Serialization;
using System.Text;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class XYPosition : PositionBase, IEquatable<XYPosition>
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        // Required by XmlSerializer
        public XYPosition() : base(null)
        {
        }

        public XYPosition(ReferentialBase referential) : base(referential)
        {
        }

        public XYPosition(ReferentialBase referential, double x, double y) : base(referential)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode() => (base.GetHashCode(), X, Y).GetHashCode();

        public override bool Equals(object other)
        {
            if (other is XYPosition otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public bool Equals(XYPosition other)
        {
            if (other is null)
                return false;

            return (
                (Referential == other.Referential) &&
                (X == other.X || (X.IsNaN() && other.X.IsNaN())) &&
                (Y == other.Y || (Y.IsNaN() && other.Y.IsNaN()))

            );
        }

        public static bool operator ==(XYPosition lPosition, XYPosition rPosition)
        {
            if (lPosition is null && rPosition is null)
            {
                return true;
            }

            if (lPosition is null || rPosition is null)
            {
                return false;
            }

            return lPosition.Equals(rPosition);
        }

        public static bool operator !=(XYPosition lPosition, XYPosition rPosition)
        {
            if (lPosition is null && rPosition is null)
            {
                return false;
            }

            if (lPosition is null || rPosition is null)
            {
                return true;
            }

            return !lPosition.Equals(rPosition);
        }

        public override object Clone()
        {
            return new XYPosition(Referential, X, Y);
        }

        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                .AppendLine($"\tX = {X}")
                .AppendLine($"\tY = {Y}")
                .ToString();
        }
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is XYPosition other)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return X.Near(other.X, tolerance.Millimeters) && Y.Near(other.Y, tolerance.Millimeters);
            }
            return false;
        }
    }
}
