using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class XYCalibrationTest : XYCalibrationData
    {
        [DataMember]
        public List<Correction> BadPoints { get; set; } = new List<Correction>();

        public bool IsValid => BadPoints is null || BadPoints.IsEmpty();
    }
}
