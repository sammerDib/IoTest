using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    [Serializable]
    [DataContract]
    public class CNCMotionControllerConfig : ControllerConfig
    {
        [DataMember]
        public SerialCom SerialCom { get; set; }
    }
}
