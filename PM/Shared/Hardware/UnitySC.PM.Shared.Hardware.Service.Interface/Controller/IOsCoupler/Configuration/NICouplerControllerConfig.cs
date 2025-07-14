using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class NICouplerControllerConfig : IOControllerConfig
    {
        [DataMember]
        public string NIDeviceType { get; set; }

        [DataMember]
        public string Port { get; set; }
    }
}
