using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Units
{
    [DataContract]
    public class Speed : IEquatable<Speed>, IComparable<Speed>, IFormattable
    {
        [DataMember]
        public double MillimetersPerSecond;

        // Required by XML.Serialize
        private Speed()
        {
            MillimetersPerSecond = double.NaN;
        }

        public Speed(double millimetersPerSecond)
        {
            MillimetersPerSecond = millimetersPerSecond;
        }

        public static bool operator ==(Speed left, Speed right)
        {
            if ((left is null) && (right is null))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Speed left, Speed right)
        {
            return !(left == right);
        }

        public int CompareTo(Speed other)
        {
            return MillimetersPerSecond.CompareTo(other.MillimetersPerSecond);
        }

        public bool Equals(Speed other)
        {
            return MillimetersPerSecond.Equals(other.MillimetersPerSecond);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Speed objLength))
                return false;

            return Equals(objLength);
        }

        public override int GetHashCode()
        {
            return MillimetersPerSecond.GetHashCode();
        }

        public string ToString(string format = null, IFormatProvider formatProvider = null)
        {
            string formattedValue = MillimetersPerSecond.ToString(format, formatProvider);
            return $"{formattedValue} mm/s";
        }
    }
}
