using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools.Tolerances
{
    [DataContract]
    public class Tolerance : IEquatable<Tolerance>
    {
        public Tolerance()
        {
        }

        public Tolerance(double value, ToleranceUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        [DataMember]
        [XmlAttribute("Value")]
        public double Value { get; set; }

        [DataMember]
        [XmlAttribute("Unit")]
        public ToleranceUnit Unit { get; set; }

        public bool IsInTolerance(double valueToTest, double targetValue)
        {
            switch (Unit)
            {
                case ToleranceUnit.Percentage:
                    if (Math.Abs(valueToTest - targetValue) * 100 / targetValue <= Value)
                        return true;
                    break;

                case ToleranceUnit.AbsoluteValue:
                    if (Math.Abs(valueToTest - targetValue) <= Value)
                        return true;
                    break;

                default:
                    break;
            }

            return false;
        }

        public double GetAbsoluteTolerance(double targetValue)
        {
            switch (Unit)
            {
                case ToleranceUnit.Percentage:
                    return Math.Abs(Value * targetValue / 100);

                case ToleranceUnit.AbsoluteValue:
                    return Math.Abs(Value);

                default: throw new ArgumentOutOfRangeException();
            }
        }

        public bool Equals(Tolerance other)
        {
            if (other == null) { return false; }
            return Value == other.Value && Unit == other.Unit;
        }

        public override int GetHashCode() => (Value, Unit).GetHashCode();

        public override bool Equals(object obj) => Equals(obj as Tolerance);

        public static bool operator ==(Tolerance left, Tolerance right)
        {
            if ((left is null) && (right is null))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Tolerance left, Tolerance right)
        {
            return !(left == right);
        }
    }
}
