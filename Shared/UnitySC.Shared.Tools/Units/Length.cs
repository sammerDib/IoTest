using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools.Units
{
    [DataContract]
    [Serializable]
    public class Length : IEquatable<Length>, IComparable<Length>, IFormattable
    {
        private static readonly MultikeyDictionary<LengthUnit, LengthUnit, decimal> s_conversionsDictionary;

        private static readonly Dictionary<LengthUnit, string> s_unitSymbolsDictionnary;

        static Length()
        {
            s_conversionsDictionary = new MultikeyDictionary<LengthUnit, LengthUnit, decimal>
            {
                { LengthUnit.Nanometer, LengthUnit.Micrometer, 1e-3m },
                { LengthUnit.Nanometer, LengthUnit.Millimeter, 1e-6m },
                { LengthUnit.Nanometer, LengthUnit.Centimeter, 1e-7m },
                { LengthUnit.Nanometer, LengthUnit.Meter, 1e-9m },
                { LengthUnit.Micrometer, LengthUnit.Millimeter, 1e-3m },
                { LengthUnit.Micrometer, LengthUnit.Centimeter, 1e-4m },
                { LengthUnit.Micrometer, LengthUnit.Meter, 1e-6m },
                { LengthUnit.Millimeter, LengthUnit.Centimeter, 1e-1m },
                { LengthUnit.Millimeter, LengthUnit.Meter, 1e-3m },
                { LengthUnit.Centimeter, LengthUnit.Meter, 1e-2m },
            };

            s_unitSymbolsDictionnary = new Dictionary<LengthUnit, string>
            {
                { LengthUnit.Nanometer, "nm" },
                { LengthUnit.Micrometer, "µm" },
                { LengthUnit.Millimeter, "mm" },
                { LengthUnit.Centimeter, "cm" },
                { LengthUnit.Meter, "m" },
            };
        }

        // For the serialisation
        private Length()
        {
        }

        public Length(double value, LengthUnit unit)
        {
            if (unit == LengthUnit.Undefined)
                throw new ArgumentException("The quantity can not be created with an undefined unit.", nameof(unit));
            if (double.IsNaN(value)) throw new ArgumentException("NaN is not a valid number.");

            Value = value;
            _unit = unit;
        }

        private LengthUnit? _unit = null;

        public static string GetUnitSymbol(LengthUnit unit)
        {
            return s_unitSymbolsDictionnary.TryGetValue(unit, out string unitSymbol) ? unitSymbol : string.Empty;
        }

        public static bool TryGetLengthUnit(string unit, out LengthUnit lengthUnit)
        {
            var values = s_unitSymbolsDictionnary.Where(x => x.Value == unit.ToLower());
            lengthUnit = values.Any() ? values.Select(x => x.Key).First() : LengthUnit.Undefined;
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
        public LengthUnit Unit
        {
            get
            {
                return _unit ?? LengthUnit.Undefined;
            }
            private set
            {
                _unit = value;
            }
        }

        [XmlAttribute("Unit")]
        public LengthUnit UnitReadOnly
        {
            get { return Unit; }
            set
            {
                if (!(_unit is null)) throw new Exception("UnitReadOnly can not be set");
                Unit = value;
            }
        }

        public double Nanometers => GetValueAs(LengthUnit.Nanometer);
        public double Micrometers => GetValueAs(LengthUnit.Micrometer);
        public double Millimeters => GetValueAs(LengthUnit.Millimeter);

        public double Centimeters => GetValueAs(LengthUnit.Centimeter);

        public double Meters => GetValueAs(LengthUnit.Meter);

        public string UnitSymbol => s_unitSymbolsDictionnary.TryGetValue(Unit, out string unitSymbol) ? unitSymbol : string.Empty;

        #endregion Properties

        #region Conversions

        /// <summary>
        ///     Converts this Length to another Length with the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>A Length with the specified unit.</returns>
        public Length ToUnit(LengthUnit unit)
        {
            double convertedValue = GetValueAs(unit);
            return new Length(convertedValue, unit);
        }

        /// <summary>
        ///     Converts this Length to the most representative unit.
        ///     The most representaive unit is the biggest unit where the length has a value greater than 1,
        /// or the smallest unit if no such exist.
        /// </summary>
        /// <example>
        ///     0.05mm will be 50µm
        ///     0.35mm will be 350µm
        ///     50000mm will be 50m
        ///     55mm will be 5.5cm
        ///     0.0005µm will be 0.5nm
        /// </example>
        /// <returns>A Length with the most representative unit.</returns>
        public Length ToMostRepresentativeUnit()
        {
            LengthUnit[] unitsDecreasing = { LengthUnit.Meter, LengthUnit.Centimeter, LengthUnit.Millimeter, LengthUnit.Micrometer, LengthUnit.Nanometer };
            foreach (var unit in unitsDecreasing)
            {
                double convertedValue = GetValueAs(unit);
                if (Math.Abs(convertedValue) >= 1)
                    return new Length(convertedValue, unit);
            }

            // Return smallest unit
            return ToUnit(LengthUnit.Nanometer);
        }

        /// <summary>
        /// Convert this Length to the specified LengthUnit.
        /// </summary>
        /// <returns>Value as double.</returns>
        public double GetValueAs(LengthUnit unitTo)
        {
            if (Unit == unitTo)
                return Value;

            if (s_conversionsDictionary.TryGetValue(Unit, unitTo, out decimal mulCoef))
            {
                return UnitTools.MultiplyDoubleDecimal(Value, mulCoef);
            }

            // We try to find the reverse conversion
            if (s_conversionsDictionary.TryGetValue(unitTo, Unit, out decimal divCoef))
            {
                return UnitTools.DivideDoubleDecimal(Value, divCoef);
            }

            throw new NotImplementedException($"Can not convert {Unit} to {unitTo}.");
        }

        #endregion Conversions

        #region Arithmetic Operators

        public static Length operator -(Length right)
        {
            return new Length(-right.Value, right.Unit);
        }

        public static Length operator +(Length left, Length right)
        {
            return new Length(left.Value + right.GetValueAs(left.Unit), left.Unit);
        }

        public static Length operator -(Length left, Length right)
        {
            return new Length(left.Value - right.GetValueAs(left.Unit), left.Unit);
        }

        public static Length operator *(double left, Length right)
        {
            return new Length(left * right.Value, right.Unit);
        }

        public static Length operator *(Length left, double right)
        {
            return new Length(left.Value * right, left.Unit);
        }

        public static Length operator /(Length left, double right)
        {
            return new Length(left.Value / right, left.Unit);
        }

        public static Length Min(Length a, Length b)
        {
            if (a is null && b is null) throw new ArgumentNullException("Min length computation impossible. Both arguments are null.");
            if (a is null) return b;
            if (b is null) return a;

            return a <= b ? a : b;
        }

        public static Length Max(Length a, Length b)
        {
            if (a is null && b is null) throw new ArgumentNullException("Min length computation impossible. Both arguments are null.");
            if (a is null) return b;
            if (b is null) return a;

            return a >= b ? a : b;
        }

        #endregion Arithmetic Operators

        #region Equality / IComparable

        public static bool operator <=(Length left, Length right)
        {
            return left.Value <= right.GetValueAs(left.Unit);
        }

        public static bool operator >=(Length left, Length right)
        {
            return left.Value >= right.GetValueAs(left.Unit);
        }

        public static bool operator <(Length left, Length right)
        {
            return left.Value < right.GetValueAs(left.Unit);
        }

        public static bool operator >(Length left, Length right)
        {
            return left.Value > right.GetValueAs(left.Unit);
        }

        public static bool operator ==(Length left, Length right)
        {
            if ((left is null) && (right is null))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Length left, Length right)
        {
            return !(left == right);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (!(obj is Length objLength)) throw new ArgumentException("Expected type Length.", nameof(obj));

            return CompareTo(objLength);
        }

        public int CompareTo(Length other)
        {
            return Value.CompareTo(other.GetValueAs(Unit));
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Length objLength))
                return false;

            return Equals(objLength);
        }

        public bool Equals(Length other)
        {
            return Value.Equals(other.GetValueAs(Unit));
        }

        public bool Near(Length other, Length tolerance)
        {
            if (tolerance.Value < 0)
                throw new ArgumentOutOfRangeException("tolerance", "Tolerance must be greater than or equal to 0.");

            double thisValue = (double)Value;
            double otherValueInThisUnits = other.GetValueAs(Unit);
            return Math.Abs(thisValue - otherValueInThisUnits) <= tolerance.GetValueAs(Unit);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current Length.</returns>
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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is null)
                return $"{Value} {UnitSymbol}";
            string formattedValue = Value.ToString(format);
            return $"{formattedValue} {UnitSymbol}";
        }

        public string ToString(string format)
        {
            if (format is null)
                return $"{Value} {UnitSymbol}";
            string formattedValue = Value.ToString(format);
            return $"{formattedValue} {UnitSymbol}";
        }

        #endregion ToString
    }
}
