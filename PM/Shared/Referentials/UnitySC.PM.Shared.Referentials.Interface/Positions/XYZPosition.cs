using System;
using System.Runtime.Serialization;
using System.Text;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class XYZPosition : XYPosition, IEquatable<XYZPosition>
    {
        [DataMember]
        public double Z { get; set; }

        public XYZPosition() : base(null)
        {
        }

        public XYZPosition(ReferentialBase referential) : base(referential)
        {
        }

        public XYZPosition(ReferentialBase referential, double x, double y, double z) : base(referential, x, y)
        {
            Z = z;
        }

        public override int GetHashCode() => (base.GetHashCode(), Z).GetHashCode();

        public override bool Equals(object other)
        {
            if (other is XYZPosition otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public bool Equals(XYZPosition other)
        {
            if (other is null)
                return false;

            return (Referential == other.Referential) &&
                (X == other.X || (X.IsNaN() && other.X.IsNaN())) &&
                (Y == other.Y || (Y.IsNaN() && other.Y.IsNaN())) &&
                (Z == other.Z || (Z.IsNaN() && other.Z.IsNaN()));
        }

        public static bool operator ==(XYZPosition lAxesPosition, XYZPosition rAxesPosition)
        {
            if (lAxesPosition is null && rAxesPosition is null)
            {
                return true;
            }

            if (lAxesPosition is null || rAxesPosition is null)
            {
                return false;
            }

            return lAxesPosition.Equals(rAxesPosition);
        }

        public static bool operator !=(XYZPosition lAxesPosition, XYZPosition rAxesPosition)
        {
            if (lAxesPosition is null && rAxesPosition is null)
            {
                return false;
            }

            if (lAxesPosition is null || rAxesPosition is null)
            {
                return true;
            }

            return !lAxesPosition.Equals(rAxesPosition);
        }

        public override object Clone()
        {
            return new XYZPosition(Referential, X, Y, Z);
        }

        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                .AppendLine($"\tZ = {Z}")
                .ToString();
        }

        public XYPosition ToXYPosition()
        {
            return new XYPosition(
                Referential,
                X,
                Y
            );
        }
        public XPosition ToXPosition()
        {
            return new XPosition(
                Referential,
                X                
            );
        }
        public YPosition ToYPosition()
        {
            return new YPosition(
                Referential,
                Y
            );
        }
        public ZPosition ToZPosition()
        {
            return new ZPosition(
                Referential,
                Z
            );
        }
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is XYZPosition other)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return base.Near(otherPosition, tolerance) && Z.Near(other.Z, tolerance.Millimeters);
            }
            return false;
        }
    }
}
