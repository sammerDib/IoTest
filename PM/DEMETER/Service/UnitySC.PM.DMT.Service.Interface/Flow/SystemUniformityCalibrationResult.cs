using System;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SystemUniformityCalibrationResult : IFlowResult
    {
        public SystemUniformityCalibrationResult()
        { }

        [XmlIgnore]
        public ImageData UnifomityCorrection {  get; set; }
        public FlowStatus Status { get; set; }
    }
}
