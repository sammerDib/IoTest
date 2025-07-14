using System;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class AutoExposureResult : IFlowResult
    {

        [XmlIgnore] public ServiceImage ResultImage { get; set; }

        public double ExposureTimeMs { get; set; }

        public int CurrentStep { get; set; }

        public int TotalSteps { get; set; }
        
        public Side WaferSide { get; set; }

        public FlowStatus Status { get; set; }
    }
}
