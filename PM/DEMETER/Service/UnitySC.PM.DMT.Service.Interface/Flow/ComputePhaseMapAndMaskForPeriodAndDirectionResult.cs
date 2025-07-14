using System;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    
    [Serializable]
    public class ComputePhaseMapAndMaskForPeriodAndDirectionResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore]
        public PSDResult PsdResult;

        public Measure.Fringe Fringe;

        public int Period;

        public FringesDisplacement FringesDisplacementDirection;
    }
}
