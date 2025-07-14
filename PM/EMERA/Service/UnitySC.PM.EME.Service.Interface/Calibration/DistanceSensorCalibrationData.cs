using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    public class DistanceSensorCalibrationData : ICalibrationData
    {
        [DataMember]
        public Length OffsetX { get; set; }
        [DataMember]
        public Length OffsetY { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
    }
}
