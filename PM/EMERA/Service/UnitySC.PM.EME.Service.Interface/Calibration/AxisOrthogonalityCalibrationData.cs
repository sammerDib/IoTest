using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    public class AxisOrthogonalityCalibrationData : ICalibrationData
    {
        [DataMember]
        public Angle AngleX { get; set; }
        [DataMember]
        public Angle AngleY { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
    }
}
