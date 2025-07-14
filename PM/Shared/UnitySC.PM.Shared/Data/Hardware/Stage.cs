using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Data.Hardware
{
    [DataContract(Namespace = "")]
    public class Stage
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
    }
}