using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class AcquirePhaseImagesForPeriodAndDirectionResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore] public List<ServiceImage> TemporaryResults = new List<ServiceImage>();

        public Measure.Fringe Fringe;

        public int Period;

        public double ExposureTimeMs;

        public FringesDisplacement FringesDisplacementDirection;
    }
}
