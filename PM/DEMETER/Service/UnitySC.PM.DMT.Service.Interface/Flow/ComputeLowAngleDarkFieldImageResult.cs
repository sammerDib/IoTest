using System;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    
    [Serializable]
    public class ComputeLowAngleDarkFieldImageResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore] 
        public ImageData DarkImage;

        public int Period;

        public Measure.Fringe Fringe;
    }
}
