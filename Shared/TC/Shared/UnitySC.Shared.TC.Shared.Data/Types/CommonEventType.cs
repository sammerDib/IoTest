using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract]
    public class DataVariableValue
    {
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
    public class DataVariable : DataVariableValue
    {
        public DataVariable()
        {

        }

        [XmlAttribute]
        [DataMember]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute]
        [DataMember]
        public string Name
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

        [XmlAttribute]
        [DataMember]
        public VidDataType DataType
        {
            get;
            set;
        }

        [XmlAttribute]
        [DataMember]
        public string Units
        {
            get;
            set;
        }
    }

    [XmlRoot("CE")]
    [DataContract]
    public class CommonEvent
    {
        public CommonEvent()
        {
        }

        [XmlAttribute]
        [DataMember]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public SecsVariableList DataVariables
        {
            get;
            set;
        } = new SecsVariableList();

        [XmlAttribute]
        [DataMember]
        public string Description
        {
            get;
            set;
        }

    }
}
