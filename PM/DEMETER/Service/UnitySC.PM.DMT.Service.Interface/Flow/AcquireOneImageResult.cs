using System;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class AcquireOneImageResult : IFlowResult
    {

        [XmlIgnore]
        public USPImageMil AcquiredImage { get; set; }

        public double ExposureTimeMs { get; set; }

        public FlowStatus Status { get; set; }
    }
}
