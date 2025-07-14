namespace UnitySC.Shared.Tools.Units
{
    public static class AngleExtensions
    {
        public static Angle Degrees(this int degrees)
        {
            return new Angle(degrees, AngleUnit.Degree);
        }
        public static Angle Degrees(this double degrees)
        {
            return new Angle(degrees, AngleUnit.Degree);
        }

        public static Angle Radians(this int radians)
        {
            return new Angle(radians, AngleUnit.Radian);
        }

        public static Angle Radians(this double radians)
        {
            return new Angle(radians, AngleUnit.Radian);
        }
    }
}
