using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class OpcControllerConfig : ControllerConfig
    {
        [DataMember]
        public SerialCom SerialCom { get; set; }

        [DataMember]
        public Ethercat Ethercat { get; set; }

        [DataMember]
        public OpcCom OpcCom { get; set; }
    }
}
