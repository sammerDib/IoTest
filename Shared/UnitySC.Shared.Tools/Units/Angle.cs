using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools.Units
{
    public enum ModuloType
    {
        Positive, //[0 360[ modulo 2Pi

        PositiveAndNegative  //]-180 180] modulo Pi
    }

    [DataContract]
    [Serializable]
    public class Angle : IEquatable<Angle>, IComparable<Angle>, IFormattable
    {
        private static readonly MultikeyDictionary<AngleUnit, AngleUnit, decimal> ConversionsDictionary;

        private static readonly Dictionary<AngleUnit, string> UnitSymbolsDictionnary;

        static Angle()
        {
            ConversionsDictionary = new MultikeyDictionary<AngleUnit, AngleUnit, decimal>
            {
                { AngleUnit.Degree, AngleUnit.Radian, (decimal)(Math.PI/180) }
            };

            UnitSymbolsDictionnary = new Dictionary<AngleUnit, string>
            {
                { AngleUnit.Degree, "°" },
                { AngleUnit.Radian, "rad" },
            };
        }

        // For the serialisation
        private Angle()
        {
        }

        public Angle(double value, AngleUnit unit)
        {
            if (unit == AngleUnit.Undefined)
                throw new ArgumentException("The quantity can not be created with an undefined unit.", nameof(unit));
            if (double.IsNaN(value)) throw new ArgumentException("NaN is not a valid number.");

            Value = value;
            _unit = unit;
        }

        private AngleUnit? _unit;

        public static string GetUnitSymbol(AngleUnit unit)
        {
            return UnitSymbolsDictionnary.TryGetValue(unit, out string unitSymbol) ? unitSymbol : string.Empty;
        }

        public static bool TryGetAngleUnit(string unit, out AngleUnit angleUnit)
        {
            var values = UnitSymbolsDictionnary.Where(x => x.Value == unit.ToLower());
            angleUnit = values.Any() ? values.Select(x => x.Key).First() : AngleUnit.Undefined;
            return values.Any();
        }

        #region Properties

        [DataMember]
        [XmlIgnore]
        public double Value { get; private set; } = double.NaN;

        [XmlAttribute("Value")]
        public double ValueReadOnly
        {
            get { return Value; }
            set
            {
                if (!double.IsNaN(Value)) throw new Exception("ValueReadOnly can not be set");
                Value = value;
            }
        }

        [DataMember]
        [XmlIgnore]
        public AngleUnit Unit
        {
            get
            {
                return _unit ?? AngleUnit.Undefined;
            }
            private set
            {
                _unit = value;
            }
        }

        [XmlAttribute("Unit")]
        public AngleUnit UnitReadOnly
        {
            get { return Unit; }
            set
            {
                if (!(_unit is null)) throw new Exception("UnitReadOnly can not be set");
                Unit = value;
            }
        }

        public double Degrees => GetValueAs(AngleUnit.Degree);
        public double Radians => GetValueAs(AngleUnit.Radian);

        public string UnitSymbol => UnitSymbolsDictionnary.TryGetValue(Unit, out string unitSymbol) ? unitSymbol : string.Empty;

        #endregion Properties

        #region Conversions

        /// <summary>
        ///     Converts this Angle to another Angle with the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>A Angle with the specified unit.</returns>
        public Angle ToUnit(AngleUnit unit)
        {
            double convertedValue = GetValueAs(unit);
            return new Angle(convertedValue, unit);
        }

        private double GetValueAs(AngleUnit unitTo)
        {
            if (Unit == unitTo)
                return Value;


            if (ConversionsDictionary.TryGetValue(Unit, unitTo, out decimal mulCoef))
            {
                return UnitTools.MultiplyDoubleDecimal(Value, mulCoef);
            }

            // We try to find the reverse conversion
            if (ConversionsDictionary.TryGetValue(unitTo, Unit, out decimal divCoef))
            {
                return UnitTools.DivideDoubleDecimal(Value, divCoef);
            }

            throw new NotImplementedException($"Can not convert {Unit} to {unitTo}.");
        }

        #endregion Conversions

        #region Arithmetic Operators

        public static Angle operator -(Angle right)
        {
            return new Angle(-right.Value, right.Unit);
        }

        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Value + right.GetValueAs(left.Unit), left.Unit);
        }

        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Value - right.GetValueAs(left.Unit), left.Unit);
        }

        public static Angle operator *(double left, Angle right)
        {
            return new Angle(left * right.Value, right.Unit);
        }

        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Value * right, left.Unit);
        }

        public static Angle operator /(Angle left, double right)
        {
            return new Angle(left.Value / right, left.Unit);
        }

        #endregion Arithmetic Operators

        #region Equality / IComparable

        public static bool operator <=(Angle left, Angle right)
        {
            return left.Value <= right.GetValueAs(left.Unit);
        }

        public static bool operator >=(Angle left, Angle right)
        {
            return left.Value >= right.GetValueAs(left.Unit);
        }

        public static bool operator <(Angle left, Angle right)
        {
            return left.Value < right.GetValueAs(left.Unit);
        }

        public static bool operator >(Angle left, Angle right)
        {
            return left.Value > right.GetValueAs(left.Unit);
        }

        public static bool operator ==(Angle left, Angle right)
        {
            if ((left is null) && (right is null))
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Angle left, Angle right)
        {
            return !(left == right);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (!(obj is Angle objLength)) throw new ArgumentException("Expected type Angle.", nameof(obj));

            return CompareTo(objLength);
        }

        public int CompareTo(Angle other)
        {
            return Value.CompareTo(other.GetValueAs(Unit));
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Angle objLength))
                return false;

            return Equals(objLength);
        }

        public bool Equals(Angle other)
        {
            return Value.Equals(other.GetValueAs(Unit));
        }

        public bool Near(Angle other, double tolerance)
        {
            if (tolerance < 0)
                throw new ArgumentOutOfRangeException("tolerance", "Tolerance must be greater than or equal to 0.");

            double thisValue = (double)Value;
            double otherValueInThisUnits = other.GetValueAs(Unit);
            return Math.Abs(thisValue - otherValueInThisUnits) <= tolerance;
        }

        public Angle Modulo(ModuloType moduloType)
        {
            var angleInDegree = ToUnit(AngleUnit.Degree);
            double newValue = angleInDegree.Value - 360 * Math.Floor(angleInDegree.Value / 360);
            if ((moduloType == ModuloType.PositiveAndNegative) && (newValue > 180))
                newValue -= 360;
            return newValue.Degrees().ToUnit(Unit);
        }

        public Angle Abs()
        {
            return new Angle(Math.Abs(Value), Unit);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current Angle.</returns>
        public override int GetHashCode()
        {
            return new { Value, Unit }.GetHashCode();
        }

        #endregion Equality / IComparable

        #region ToString

        public override string ToString()
        {
            return $"{Value} {UnitSymbol}";
        }

        public string ToString(string format)
        {
            if (format is null)
                return ToString();
            string formattedValue = Value.ToString(format);
            return $"{formattedValue} {UnitSymbol}";
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is null)
                return $"{Value} {UnitSymbol}";
            string formattedValue = Value.ToString(format);
            return $"{formattedValue} {UnitSymbol}";
        }

        #endregion ToString
    }
}
