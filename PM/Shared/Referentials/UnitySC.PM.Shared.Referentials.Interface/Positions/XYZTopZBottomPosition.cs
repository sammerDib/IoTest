using System;

using System.Runtime.Serialization;
using System.Text;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class XYZTopZBottomPosition : XYPosition, IEquatable<XYZTopZBottomPosition>
    {
        [DataMember]
        public double ZTop { get; set; }

        [DataMember]
        public double ZBottom { get; set; }

        public XYZTopZBottomPosition() : base(null)
        {
        }

        public XYZTopZBottomPosition(ReferentialBase referential) : base(referential)
        {
        }

        public XYZTopZBottomPosition(ReferentialBase referential, double x, double y, double zTop, double zBottom) : base(referential, x, y)
        {
            ZTop = zTop;
            ZBottom = zBottom;
        }

        public override int GetHashCode() => (base.GetHashCode(), ZTop, ZBottom).GetHashCode();

        public override bool Equals(object other)
        {
            if (other is XYZTopZBottomPosition otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public bool Equals(XYZTopZBottomPosition other)
        {
            if (other is null)
                return false;

            return (Referential == other.Referential) &&
                (X == other.X || (X.IsNaN() && other.X.IsNaN())) &&
                (Y == other.Y || (Y.IsNaN() && other.Y.IsNaN())) &&
                (ZTop == other.ZTop || (ZTop.IsNaN() && other.ZTop.IsNaN())) &&
                (ZBottom == other.ZBottom || (ZBottom.IsNaN() && other.ZBottom.IsNaN()));
        }

        public static bool operator ==(XYZTopZBottomPosition lAxesPosition, XYZTopZBottomPosition rAxesPosition)
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

        public static bool operator !=(XYZTopZBottomPosition lAxesPosition, XYZTopZBottomPosition rAxesPosition)
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
            return new XYZTopZBottomPosition(Referential, X, Y, ZTop, ZBottom);
        }

        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                .AppendLine($"\tZTop = {ZTop}")
                .AppendLine($"\tZBottom = {ZBottom}")
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
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is XYZTopZBottomPosition other)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return base.Near(otherPosition, tolerance) &&
                        ZTop.Near(other.ZTop, tolerance.Millimeters) &&
                        ZBottom.Near(other.ZBottom, tolerance.Millimeters);
            }
            return false;
        }
    }
}
