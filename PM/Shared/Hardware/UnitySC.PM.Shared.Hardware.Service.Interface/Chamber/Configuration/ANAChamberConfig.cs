using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;

namespace UnitySC.PM.Shared.Hardware.Chamber
{
    [Serializable]
    [DataContract]
    public class ANAChamberConfig : ChamberConfig
    {
        [DataMember]
        public string FFUFilter { get; set; }

        [DataMember]
        public string RobotIsOut { get; set; }

        [DataMember]
        public string PrepareToTransfer { get; set; }

        [DataMember]
        public string EMOPushed { get; set; }

        [DataMember]
        public int StabilisationFFUSwitchOn_ms { get; set; }

        [DataMember]
        public int StabilisationFFUSwitchOff_ms { get; set; }
    }
}
