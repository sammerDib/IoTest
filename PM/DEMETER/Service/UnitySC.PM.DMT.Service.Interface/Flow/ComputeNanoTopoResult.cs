using System;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class ComputeNanoTopoResult : IFlowResult
    {
        public FlowStatus Status { get; set; }

        [XmlIgnore]
        public ImageData NanoTopoImage { get; set; }
    }
}
