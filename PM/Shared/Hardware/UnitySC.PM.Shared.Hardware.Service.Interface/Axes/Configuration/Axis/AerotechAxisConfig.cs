using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class AerotechAxisConfig : MotorizedAxisConfig
    {
        [DataMember]
        public int MaxSpeedService { get; set; }

        [DataMember]
        public int MaxAccelerationService { get; set; }

        [DataMember]
        public bool Disabled { get; set; }

        [DataMember]
        public bool UsedPSO { get; set; }
    }
}
