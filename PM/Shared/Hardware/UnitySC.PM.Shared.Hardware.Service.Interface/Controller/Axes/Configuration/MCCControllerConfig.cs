using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class MCCControllerConfig : ControllerConfig
    {
        [DataMember]
        public string PortName { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        [DataMember]
        public int Address { get; set; }

        /// <summary>
        /// Time delay between successive polling calls.
        /// </summary>
        public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMilliseconds(200);
    }
}
