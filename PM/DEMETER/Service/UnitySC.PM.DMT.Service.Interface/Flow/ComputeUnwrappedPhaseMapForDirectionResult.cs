using System;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class ComputeUnwrappedPhaseMapForDirectionResult : IFlowResult
    {
        public Measure.Fringe Fringe;

        public FringesDisplacement FringesDisplacementDirection;

        [XmlIgnore]
        public ImageData UnwrappedPhaseMap;

        public FlowStatus Status { get; set; }
    }
}
