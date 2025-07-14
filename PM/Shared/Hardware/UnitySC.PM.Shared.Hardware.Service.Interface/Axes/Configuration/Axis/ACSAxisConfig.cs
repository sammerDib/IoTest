using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class ACSAxisConfig : MotorizedAxisConfig
    {
        [DataMember]
        public List<ACSAxisIDLink> ACSAxisIDLinks;
        [DataMember]
        public double CurrentSpeed { get; set; }
        [DataMember]
        public double CurrentAccel { get; set; }
        [DataMember]
        public int MaxSpeedService { get; set; }
        [DataMember]
        public bool UsedInLanding { get; set; }
        [DataMember]
        public bool Disabled { get; set; }
        [DataMember]
        public bool UseSoftwareDisable { get; set; }
        [DataMember]
        public bool SofwareDisabled { get; set; }
        [DataMember]
        public Length CurrentPos { get; set; }
    }
}
