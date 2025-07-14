using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data.FDC
{
    [DataContract]
    [KnownType(typeof(byte))]
    [KnownType(typeof(char))]
    [KnownType(typeof(short))]
    [KnownType(typeof(ushort))]
    [KnownType(typeof(int))]
    [KnownType(typeof(uint))]
    [KnownType(typeof(long))]
    [KnownType(typeof(ulong))]
    [KnownType(typeof(decimal))]
    [KnownType(typeof(float))]
    [KnownType(typeof(double))]
    [KnownType(typeof(string))]
    [KnownType(typeof(DateTime))]
    [KnownType(typeof(TimeSpan))]
    [XmlInclude(typeof(DateTime))]
    [XmlInclude(typeof(TimeSpan))]

    public class FDCValue
    {
        [DataMember]
        public object Value { get; set; }
    }

    [DataContract]
    public class FDCData
    {
        public static FDCData MakeNew<T>(string name, T value, string unit = null, DateTime? date = null)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception("FDC Name cannot be null or empty");

            if (!value.GetType().IsValueType && value.GetType().GetCustomAttribute(typeof(DataContractAttribute), true) == null)
                throw new Exception("FDC Value must be a value type or must have the DataContract attribute");

            var fdcdate = date ?? DateTime.Now;
            var fdcunit = unit ?? string.Empty;
            var fdcval = new FDCValue() { Value = value };
            return new FDCData(name, fdcval, fdcunit, fdcdate);
        }

        // Constructor should be privat so you need to call Makenew to vreat some FDCData
        private FDCData(string name, FDCValue value, string unit, DateTime date)
        {
            Name = name;
            ValueFDC = value;
            Unit = unit;
            Date = date;
        }
        private FDCData()
        {
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FDCValue ValueFDC { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}
