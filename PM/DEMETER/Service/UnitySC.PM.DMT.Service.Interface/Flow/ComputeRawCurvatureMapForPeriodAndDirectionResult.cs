using System;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    
    [Serializable]
    public class ComputeRawCurvatureMapForPeriodAndDirectionResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore]
        public ImageData RawCurvatureMap;

        [XmlIgnore]
        public ImageData Mask;

        public Measure.Fringe Fringe;

        public int Period;

        public FringesDisplacement FringesDisplacementDirection;
    }
}
