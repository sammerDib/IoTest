using System.Xml.Serialization;

namespace UnitySC.Shared.Format.Metro
{
    public enum GlobalState
    {
        [XmlEnum(Name = "Error")]
        Error,

        [XmlEnum(Name = "Success")]
        Success,

        [XmlEnum(Name = "Partial")]
        Partial,

        [XmlEnum(Name = "Rework")]
        Rework,

        [XmlEnum(Name = "Reject")]
        Reject
    }
}