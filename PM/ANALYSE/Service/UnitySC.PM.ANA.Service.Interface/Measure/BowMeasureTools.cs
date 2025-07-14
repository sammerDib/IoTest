using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    [KnownType(typeof(ProbeWithObjectivesMaterial))]
    [KnownType(typeof(DualProbeWithObjectivesMaterial))]
    public class BowMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<XYPosition> DefaultReferencePlanePositions { get; set; }

        [DataMember]
        public List<ProbeMaterialBase> CompatibleProbes { get; set; }
    }
}
