using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{

    [DataContract]
    public class ProbeMaterialBase
    {
        [DataMember]
        public string ProbeId { get; set; }

 
    }


    [DataContract]
    public class ProbeWithObjectivesMaterial:ProbeMaterialBase
    {
  
        [DataMember]
        public List<string> CompatibleObjectives { get; set; }
    }

    [DataContract]
    public class DualProbeWithObjectivesMaterial : ProbeMaterialBase
    {
        [DataMember]
        public ProbeWithObjectivesMaterial UpProbe { get; set; }

        [DataMember]
        public ProbeWithObjectivesMaterial DownProbe { get; set; }
    }
}
