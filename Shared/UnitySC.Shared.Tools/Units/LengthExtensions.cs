namespace UnitySC.Shared.Tools.Units
{
    public static class LengthExtensions
    {
        public static Length Nanometers(this double value)
        {
            return new Length(value, LengthUnit.Nanometer);
        }

        public static Length Nanometers(this int value)
        {
            return new Length(value, LengthUnit.Nanometer);
        }

        public static Length Micrometers(this double value)
        {
            return new Length(value, LengthUnit.Micrometer);
        }

        public static Length Micrometers(this int value)
        {
            return new Length(value, LengthUnit.Micrometer);
        }

        public static Length Millimeters(this double value)
        {
            return new Length(value, LengthUnit.Millimeter);
        }

        public static Length Millimeters(this int value)
        {
            return new Length(value, LengthUnit.Millimeter);
        }

        public static Length Centimeters(this double value)
        {
            return new Length(value, LengthUnit.Centimeter);
        }

        public static Length Centimeters(this int value)
        {
            return new Length(value, LengthUnit.Centimeter);
        }

        public static Length Meters(this double value)
        {
            return new Length(value, LengthUnit.Meter);
        }

        public static Length Meters(this int value)
        {
            return new Length(value, LengthUnit.Meter);
        }
    }
}
