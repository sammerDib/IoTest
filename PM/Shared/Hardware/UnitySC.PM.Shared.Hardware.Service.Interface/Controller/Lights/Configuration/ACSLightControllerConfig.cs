using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable()]
    [DataContract]
    public class ACSLightControllerConfig : ControllerConfig
    {
        [DataMember]
        public EthernetCom EthernetCom { get; set; }
    }
}
