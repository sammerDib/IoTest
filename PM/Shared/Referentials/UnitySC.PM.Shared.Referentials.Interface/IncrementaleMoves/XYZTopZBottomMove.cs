using System.Runtime.Serialization;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    /// <summary>
    /// Comprise move orders for up to four axes.
    ///
    /// NOTE: For axes that do not need to move, use double.NaN as movment value.
    /// </summary>
    ///

    [DataContract]
    public class XYZTopZBottomMove : IncrementalMoveBase
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double ZTop { get; set; }

        [DataMember]
        public double ZBottom { get; set; }

        public XYZTopZBottomMove(double x, double y, double zTop, double zBottom) :
            base(new MotorReferential())

        {
            X = x;
            Y = y;
            ZTop = zTop;
            ZBottom = zBottom;
        }

        public bool Equals(XYZTopZBottomMove other)
        {
            return (
                (Referential == other.Referential) &&
                (X == other.X || (X.IsNaN() && other.X.IsNaN())) &&
                (Y == other.Y || (Y.IsNaN() && other.Y.IsNaN())) &&
                (ZTop == other.ZTop || (ZTop.IsNaN() && other.ZTop.IsNaN())) &&
                (ZBottom == other.ZBottom || (ZBottom.IsNaN() && other.ZBottom.IsNaN()))
            );
        }

        public override bool Equals(object other)
        {
            if (other is XYZTopZBottomMove otherOfGoodType)
            {
                return Equals(otherOfGoodType);
            }
            return false;
        }

        public static bool operator ==(XYZTopZBottomMove left, XYZTopZBottomMove right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(XYZTopZBottomMove left, XYZTopZBottomMove right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left is null || right is null)
            {
                return true;
            }

            return !left.Equals(right);
        }

        public override int GetHashCode() => (Referential, X, Y, ZTop, ZBottom).GetHashCode();

        public override object Clone()
        {
            var clone = new XYZTopZBottomMove(X, Y, ZTop, ZBottom);
            return clone;
        }
    }
}
