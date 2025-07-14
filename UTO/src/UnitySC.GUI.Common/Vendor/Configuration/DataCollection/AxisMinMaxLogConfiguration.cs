using System;
using System.Runtime.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class AxisMinMaxLogConfiguration
    {
        [DataMember]
        public string UnitName { get; set; }

        [DataMember]
        public string UnitAbbreviation { get; set; }

        [DataMember]
        public double Min { get; set; }

        [DataMember]
        public double Max { get; set; }

        [DataMember]
        public bool IsLogarithmic { get; set; }
    }
}
