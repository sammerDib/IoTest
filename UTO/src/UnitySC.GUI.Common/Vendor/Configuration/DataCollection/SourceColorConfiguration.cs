using System;
using System.Runtime.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class SourceColorConfiguration
    {
        [DataMember]
        public string SourceName { get; set; }

        [DataMember]
        public string SourceColor { get; set; }

        [DataMember]
        public string QuantityType { get; set; }

        [DataMember]
        public string QuantityAbbreviation { get; set; }
    }
}
