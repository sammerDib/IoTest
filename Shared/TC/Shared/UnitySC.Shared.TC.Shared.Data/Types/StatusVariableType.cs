using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract]
    public class StatusVariableValue
    {
        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        private object _value;

        [XmlIgnore]
        [IgnoreDataMemberAttribute]
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        [XmlAttribute]
        [DataMember(Name = "Value")]
        public virtual string ValueAsString
        {
            get
            {
                return (string)DataTypeConverter.ConvertTo(VidDataType.String, Value);
            }
            set
            {
                Value = value;
            }
        }

        public event EventHandler ValueChanged;
    }

    [DataContract]
    public class StatusVariable : StatusVariableValue
    {
        public StatusVariable()
        {

        }
        public StatusVariable(String svid, VidDataType dataType, string description)
        {
            Name = svid;
            Description = description;
            DataType = dataType;
            Minimum = null;
            Maximum = null;
            Units = null;
        }
        public StatusVariable(String svid, VidDataType dataType, string description, object minimum, object maximum, string units)
        {
            Name = svid;
            Description = description;
            DataType = dataType;
            Minimum = minimum;
            Maximum = maximum;
            Units = units;
        }

        [XmlAttribute]
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [XmlAttribute]
        [DataMember]
        public VidDataType DataType
        {
            get;
            set;
        }

        [IgnoreDataMemberAttribute]
        public object Minimum
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Minimum")]
        [DataMember(Name = "Minimum")]
        public string MinimumAsString
        {
            get
            {
                return (string)DataTypeConverter.ConvertTo(VidDataType.String, Minimum);
            }
            set
            {
                Minimum = DataTypeConverter.ConvertTo(DataType, value);
            }
        }

        [XmlIgnore]
        [IgnoreDataMemberAttribute]
        public object Maximum
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Maximum")]
        [DataMember(Name = "Maximum")]
        public string MaximumAsString
        {
            get
            {
                return (string)DataTypeConverter.ConvertTo(VidDataType.String, Maximum);
            }
            set
            {
                Maximum = DataTypeConverter.ConvertTo(DataType, value);
            }
        }

        [DataMember]
        public string Units
        {
            get;
            set;
        }

    }
}
