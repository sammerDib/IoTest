using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class LiseAutofocusCalibration
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string ObjectiveName { get; set; }

        [DataMember]
        public double MinGain { get; set; }

        [DataMember]
        public double MaxGain { get; set; }

        [DataMember]
        public double ZPosition { get; set; }
    }
}
