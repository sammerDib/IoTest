using System;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class AdjustCurvatureDynamicsForRawCurvatureMapResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore]
        public ImageData CurvatureMap;

        public Measure.Fringe Fringe;

        public int Period;

        public FringesDisplacement FringesDisplacementDirection;
    }
}
