using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.TC.Shared.Data
{

    [XmlRoot("EC")]
    [DataContract]
    public class EquipmentConstantValue
    {
        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [XmlIgnore]
        public object Value
        {
            get;
            set;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlAttribute(AttributeName = "Value")]
        [DataMember]
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
    }

    [DataContract]
    public class EquipmentConstant : EquipmentConstantValue
    {
        public EquipmentConstant()
        {

        }
        public EquipmentConstant(String svid, VidDataType dataType, string description, object defaultValue)
        {
            Name = svid;
            DataType = dataType;
            Minimum = null;
            Maximum = null;
            Units = null;
            Description = description;
            DefaultValue = defaultValue;
        }
        public EquipmentConstant(String svid, VidDataType dataType, object minimum, object maximum, string units, string description, object defaultValue)
        {
            Name = svid;
            DataType = dataType;
            Minimum = minimum;
            Maximum = maximum;
            Units = units;
            Description = description;
            DefaultValue = defaultValue;
        }
        [XmlAttribute]
        [DataMember]
        public VidDataType DataType
        {
            get;
            set;
        }

        [XmlIgnore]
        [DataMember]
        public object Minimum
        {
            get;
            set;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlAttribute(AttributeName = "Minimum")]
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
        [DataMember]
        public object Maximum
        {
            get;
            set;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlAttribute(AttributeName = "Maximum")]
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

        [XmlAttribute]
        [DataMember]
        public string Units
        {
            get;
            set;
        }

        [XmlAttribute]
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [XmlIgnore]
        public object DefaultValue
        {
            get;
            set;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlAttribute(AttributeName = "DefaultValue")]
        [DataMember]
        public string DefaultValueAsString
        {
            get
            {
                return (string)DataTypeConverter.ConvertTo(VidDataType.String, DefaultValue);
            }
            set
            {
                DefaultValue = DataTypeConverter.ConvertTo(DataType, value);
            }
        }

        [XmlIgnore]
        public override string ValueAsString
        {
            get
            {
                return base.ValueAsString;
            }
            set
            {
                base.Value = DataTypeConverter.ConvertTo(DataType, value);
            }
        }
    }
}
