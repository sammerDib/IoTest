using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data.Geometry
{
    public struct PointUnits
    {
        public Length X;
        public Length Y;

        public PointUnits(Length x, Length y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(PointUnits point1, PointUnits point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(PointUnits point1, PointUnits point2) => !(point1 == point2);

        public override bool Equals(object obj)
        {
            if (obj is PointUnits other)
            {
                return other == this;
            }
            return false;
        }

        public override int GetHashCode() => (X.Millimeters, Y.Millimeters).GetHashCode();
    }
}
