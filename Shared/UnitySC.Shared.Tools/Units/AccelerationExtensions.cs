namespace UnitySC.Shared.Tools.Units
{
    public static class AccelerationExtensions
    {
        public static Acceleration MillimetersPerSecondSquared(this double value)
        {
            return new Acceleration(value);
        }

        public static Acceleration MillimetersPerSecondSquared(this int value)
        {
            return new Acceleration(value);
        }
    }
}
