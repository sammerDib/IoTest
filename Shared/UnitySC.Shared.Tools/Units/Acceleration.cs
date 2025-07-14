using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Units
{
    [DataContract]
    public class Acceleration : IEquatable<Acceleration>, IComparable<Acceleration>, IFormattable
    {
        [DataMember]
        public double MillimetersPerSecondSquared;

        // Required by XML.Serialize
        private Acceleration()
        {
            MillimetersPerSecondSquared = double.NaN;
        }

        public Acceleration(double millimetersPerSecondSquared)
        {
            MillimetersPerSecondSquared = millimetersPerSecondSquared;
        }

        public int CompareTo(Acceleration other)
        {
            return MillimetersPerSecondSquared.CompareTo(other.MillimetersPerSecondSquared);
        }

        public bool Equals(Acceleration other)
        {
            return MillimetersPerSecondSquared.Equals(other.MillimetersPerSecondSquared);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Acceleration objLength))
                return false;

            return Equals(objLength);
        }

        public override int GetHashCode()
        {
            return MillimetersPerSecondSquared.GetHashCode();
        }

        public string ToString(string format = null, IFormatProvider formatProvider = null)
        {
            string formattedValue = MillimetersPerSecondSquared.ToString(format, formatProvider);
            return $"{formattedValue} mm/s²";
        }
    }
}
