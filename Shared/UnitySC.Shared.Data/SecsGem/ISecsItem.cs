using System;
using System.Runtime.Serialization;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
    public interface ISecsItem
    {
        #region Public Properties

        byte BinaryValue { get; set; }
        bool BooleanValue { get; set; }
        float Float4Value { get; set; }
        double Float8Value { get; set; }
        SecsFormat Format { get; set; }
        sbyte Int1Value { get; set; }

        short Int2Value { get; set; }

        int Int4Value { get; set; }

        long Int8Value { get; set; }

        ISecsItemList ItemList { get; }

        string StringValue { get; set; }

        byte Uint1Value { get; set; }

        ushort Uint2Value { get; set; }

        uint Uint4Value { get; set; }

        ulong Uint8Value { get; set; }

        #endregion Public Properties
    }

    [DataContract]
    public class SecsItem : ISecsItem
    {
        #region Private Fields

        private byte m_BinaryValue;
        private bool m_BooleanValue;
        private float m_Float4Value;
        private double m_Float8Value;
        private SecsFormat m_Format;
        private sbyte m_Int1Value;
        private short m_Int2Value;
        private int m_Int4Value;
        private long m_Int8Value;
        private SecsItemList m_ItemList;
        private string m_StringValue;
        private byte m_Uint1Value;
        private ushort m_Uint2Value;
        private uint m_Uint4Value;
        private ulong m_Uint8Value;

        #endregion Private Fields

        #region Public Constructors
        public SecsItem() { }

        public SecsItem(SecsFormat format)
        {
            Format = format;
        }

        public SecsItem(SecsFormat format, object value) : this(format)
        {
            switch (format)
            {
                case SecsFormat.Undefined:
                    break;
                case SecsFormat.List:
                    if (value is ISecsItemList list)
                    {
                        ItemList = new SecsItemList();
                        foreach (var item in list)
                            ((ISecsItemList)ItemList).Add(item);
                    }
                    break;

                case SecsFormat.Binary:
                    BinaryValue = (byte)value;
                    break;

                case SecsFormat.Boolean:
                    BooleanValue = (bool)value;
                    break;

                case SecsFormat.Ascii:
                    StringValue = value.ToString();
                    break;

                case SecsFormat.Jis8:
                    StringValue = value.ToString();
                    break;

                case SecsFormat.Character:
                    StringValue += value.ToString();
                    break;

                case SecsFormat.Int8:
                    Int8Value = (long)value;
                    break;

                case SecsFormat.Int1:
                    Int1Value = (sbyte)value;
                    break;

                case SecsFormat.Int2:
                    Int2Value = (short)value;
                    break;

                case SecsFormat.Int4:
                    Int4Value = (int)value;
                    break;

                case SecsFormat.Float8:
                    Float8Value = (double)value;
                    break;

                case SecsFormat.Float4:
                    Float4Value = (float)value;
                    break;

                case SecsFormat.UInt8:
                    Uint8Value = (ulong)value;
                    break;

                case SecsFormat.UInt1:
                    Uint1Value = (byte)value;
                    break;

                case SecsFormat.UInt2:
                    Uint2Value = (ushort)value;
                    break;

                case SecsFormat.UInt4:
                    Uint4Value = (uint)value;
                    break;

                default: throw new NotImplementedException();
            }
        }

        public SecsItem(ISecsItem value)
        {
            Format = value?.Format ?? SecsFormat.Undefined;

            switch (Format)
            {
                case SecsFormat.Undefined:
                    break;
                case SecsFormat.List:
                    var intf = value.ItemList;
                    if (intf != null)
                    {
                        ItemList = new SecsItemList();
                        foreach (var item in intf)
                            ((ISecsItemList)ItemList).Add(item);
                    }

                    break;
                case SecsFormat.Binary:
                    BinaryValue = value.BinaryValue;
                    break;
                case SecsFormat.Boolean:
                    BooleanValue = value.BooleanValue;
                    break;
                case SecsFormat.Ascii:
                    StringValue = value.StringValue;
                    break;
                case SecsFormat.Jis8:
                    StringValue = value.StringValue;
                    break;
                case SecsFormat.Character:
                    StringValue += value.StringValue;
                    break;
                case SecsFormat.Int8:
                    Int8Value = value.Int8Value;
                    break;
                case SecsFormat.Int1:
                    Int1Value = value.Int1Value;
                    break;
                case SecsFormat.Int2:
                    Int2Value = value.Int2Value;
                    break;
                case SecsFormat.Int4:
                    Int4Value = value.Int4Value;
                    break;
                case SecsFormat.Float8:
                    Float8Value = value.Float8Value;
                    break;
                case SecsFormat.Float4:
                    Float4Value = value.Float4Value;
                    break;
                case SecsFormat.UInt8:
                    Uint8Value = value.Uint8Value;
                    break;
                case SecsFormat.UInt1:
                    Uint1Value = value.Uint1Value;
                    break;
                case SecsFormat.UInt2:
                    Uint2Value = value.Uint2Value;
                    break;
                case SecsFormat.UInt4:
                    Uint4Value = value.Uint4Value;
                    break;
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{new string(' ', identation)}SecsItem({Format}): ");
            switch (Format)
            {
                case SecsFormat.Undefined:
                    sb.AppendLine("void");
                    break;
                case SecsFormat.List:
                    sb.AppendLine("");
                    sb.AppendLine(ItemList.ToIdentString(identation + Constants.StringIdentation));
                    break;
                case SecsFormat.Binary:
                    sb.AppendLine(BinaryValue.ToString());
                    break;
                case SecsFormat.Boolean:
                    sb.AppendLine(BooleanValue.ToString());
                    break;
                case SecsFormat.Ascii:
                    sb.AppendLine($"\"{StringValue}\"");
                    break;
                case SecsFormat.Jis8:
                    sb.AppendLine($"\"{StringValue}\"");
                    break;
                case SecsFormat.Character:
                    sb.AppendLine($"\"{StringValue}\"");
                    break;
                case SecsFormat.Int8:
                    sb.AppendLine(Int8Value.ToString());
                    break;
                case SecsFormat.Int1:
                    sb.AppendLine(Int1Value.ToString());
                    break;
                case SecsFormat.Int2:
                    sb.AppendLine(Int2Value.ToString());
                    break;
                case SecsFormat.Int4:
                    sb.AppendLine(Int4Value.ToString());
                    break;
                case SecsFormat.Float8:
                    sb.AppendLine(Float8Value.ToString());
                    break;
                case SecsFormat.Float4:
                    sb.AppendLine(Float4Value.ToString());
                    break;
                case SecsFormat.UInt8:
                    sb.AppendLine(Uint8Value.ToString());
                    break;
                case SecsFormat.UInt1:
                    sb.AppendLine(Uint1Value.ToString());
                    break;
                case SecsFormat.UInt2:
                    sb.AppendLine(Uint2Value.ToString());
                    break;
                case SecsFormat.UInt4:
                    sb.AppendLine(Uint4Value.ToString());
                    break;
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        [DataMember]
        public byte BinaryValue
        {
            get
            {
                return m_BinaryValue;
            }
            set
            {
                m_BinaryValue = value;
            }
        }

        [DataMember]
        public bool BooleanValue
        {
            get
            {
                return m_BooleanValue;
            }
            set
            {
                m_BooleanValue = value;
            }
        }

        [DataMember]
        public float Float4Value
        {
            get
            {
                return m_Float4Value;
            }
            set
            {
                m_Float4Value = value;
            }
        }

        [DataMember]
        public double Float8Value
        {
            get
            {
                return m_Float8Value;
            }
            set
            {
                m_Float8Value = value;
            }
        }

        [DataMember]
        public SecsFormat Format
        {
            get
            {
                return m_Format;
            }
            set
            {
                m_Format = value;
            }
        }

        [DataMember]
        public sbyte Int1Value
        {
            get
            {
                return m_Int1Value;
            }
            set
            {
                m_Int1Value = value;
            }
        }

        [DataMember]
        public short Int2Value
        {
            get
            {
                return m_Int2Value;
            }
            set
            {
                m_Int2Value = value;
            }
        }

        [DataMember]
        public int Int4Value
        {
            get
            {
                return m_Int4Value;
            }
            set
            {
                m_Int4Value = value;
            }
        }

        [DataMember]
        public long Int8Value
        {
            get
            {
                return m_Int8Value;
            }
            set
            {
                m_Int8Value = value;
            }
        }

        [DataMember]
        public SecsItemList ItemList
        {
            get
            {
                return m_ItemList;
            }
            set
            {
                m_ItemList = value;
            }
        }

        [DataMember]
        public string StringValue
        {
            get
            {
                return m_StringValue;
            }
            set
            {
                m_StringValue = value;
            }
        }

        [DataMember]
        public byte Uint1Value
        {
            get
            {
                return m_Uint1Value;
            }
            set
            {
                m_Uint1Value = value;
            }
        }

        [DataMember]
        public ushort Uint2Value
        {
            get
            {
                return m_Uint2Value;
            }
            set
            {
                m_Uint2Value = value;
            }
        }

        [DataMember]
        public uint Uint4Value
        {
            get
            {
                return m_Uint4Value;
            }
            set
            {
                m_Uint4Value = value;
            }
        }

        [DataMember]
        public ulong Uint8Value
        {
            get
            {
                return m_Uint8Value;
            }
            set
            {
                m_Uint8Value = value;
            }
        }

        ISecsItemList ISecsItem.ItemList => ItemList;

        #endregion Public Properties
    }
}
