using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class RCMControllerConfig : ControllerConfig
    {
        [DataMember]
        public string PortName { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        /// <summary>
        /// Number of pulses the motor need to achieve one revolution.
        /// Unit: pulses per revolution.
        /// </summary>
        [DataMember]
        public int MotorResolution { get; set; }

        /// <summary>
        /// Time delay between successive polling calls.
        /// </summary>
        public TimeSpan PollingDelay { get; set; } = TimeSpan.FromMilliseconds(200);
    }
}
