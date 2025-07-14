using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class RelianceAxisConfig : MotorizedAxisConfig
    {
        [DataMember]
        public string RelianceAxisID { get; set; }
    }
}
