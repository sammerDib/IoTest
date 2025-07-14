using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Tolerances
{
    [DataContract]
    public class LengthTolerance : IEquatable<LengthTolerance>
    {
        public LengthTolerance()
        {
        }

        public LengthTolerance(double value, LengthToleranceUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        [DataMember]
        [XmlAttribute("Value")]
        public double Value { get; set; }

        [DataMember]
        [XmlAttribute("Unit")]
        public LengthToleranceUnit Unit { get; set; }

        public bool IsInTolerance(Length valueToTest, Length targetValue)
        {
            switch (Unit)
            {
                case LengthToleranceUnit.Percentage:
                    if (Math.Abs(valueToTest.Micrometers - targetValue.Micrometers) * 100 / targetValue.Micrometers <= Value)
                        return true;
                    break;

                case LengthToleranceUnit.Micrometer:
                    if (Math.Abs(valueToTest.Micrometers - targetValue.Micrometers) <= Value)
                        return true;
                    break;

                case LengthToleranceUnit.Nanometer:
                    if (Math.Abs(valueToTest.Nanometers - targetValue.Nanometers) <= Value)
                        return true;
                    break;

                case LengthToleranceUnit.Millimeter:
                    if (Math.Abs(valueToTest.Millimeters - targetValue.Millimeters) <= Value)
                        return true;
                    break;

                default:
                    break;
            }

            return false;
        }

        public Length GetAbsoluteTolerance(Length targetValue)
        {
            return Unit == LengthToleranceUnit.Percentage ?
                new Length(Math.Abs(Value * targetValue.Micrometers * 0.01), LengthUnit.Micrometer).ToUnit(targetValue.Unit) :
                new Length(Value, Unit.ToLengthUnit());
        }

        public bool Equals(LengthTolerance other)
        {
            if (other == null) { return false; }
            return Value == other.Value && Unit == other.Unit;
        }

        public override int GetHashCode() => (Value, Unit).GetHashCode();

        public override bool Equals(object obj) => Equals(obj as LengthTolerance);

        public static bool operator ==(LengthTolerance left, LengthTolerance right)
        {
            if ((left is null) && (right is null))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(LengthTolerance left, LengthTolerance right)
        {
            return !(left == right);
        }
    }
}
