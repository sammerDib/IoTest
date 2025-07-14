using System.Runtime.Serialization;
using System;
using System.Xml.Serialization;

namespace UnitySC.Shared.Format.Metro.TSV
{
    [Serializable]
    [DataContract]
    public class TSVDieResult : MeasureDieResult
    {
        [XmlElement("Copla")]
        [DataMember]
        public BestFitPlan BestFitPlan { get; set; }
    }
}
