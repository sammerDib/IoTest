namespace UnitySC.Shared.Tools.Units
{
    public static class SpeedExtension
    {
        public static Speed MillimetersPerSecond(this double value)
        {
            return new Speed(value);
        }

        public static Speed MillimetersPerSecond(this int value)
        {
            return new Speed(value);
        }
    }
}
