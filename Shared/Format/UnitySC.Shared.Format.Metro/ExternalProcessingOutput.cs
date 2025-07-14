using UnitySC.Shared.Tools.Tolerances;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Format.Metro
{
    [DataContract]
    public class ExternalProcessingOutput
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double OutputTarget { get; set; }
        [DataMember]
        public Tolerance OutputTolerance { get; set; }
    }
}
