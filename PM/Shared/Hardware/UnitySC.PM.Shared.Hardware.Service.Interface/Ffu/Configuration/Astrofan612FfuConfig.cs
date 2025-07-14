using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ffu
{
    [Serializable]
    [DataContract]
    public class Astrofan612FfuConfig : FfuConfig
    {
        [DataMember]
        public ushort NormalRunningSpeed_percentage { get; set; }

        [DataMember]
        public Pressure PressureRange { get; set; }

        [DataMember]
        public InClosedLoopSpeedControl InClosedLoopSpeedControl { get; set; }

        [DataMember]
        public EventRaised WarningRaised { get; set; }

        [DataMember]
        public EventRaised AlarmRaised { get; set; }
    }

    [Serializable]
    [DataContract]
    public class Pressure
    {
        [DataMember]
        public double MinPressure_millibar { get; set; }

        [DataMember]
        public double MaxPressure_millibar { get; set; }
    }

    [Serializable]
    [DataContract]
    public class InClosedLoopSpeedControl
    {
        [DataMember]
        public int ObservationPeriodBeforeChange_sec { get; set; }

        [DataMember]
        public double IncreaseSpeed_percentage { get; set; }

        [DataMember]
        public double DecreaseSpeed_percentage { get; set; }
    }

    [Serializable]
    [DataContract]
    public class EventRaised
    {
        [DataMember]
        public double SpeedUpper_percentage { get; set; }

        [DataMember]
        public int TimeDuringRotatesAtThisSpeed_min { get; set; }
    }
}
