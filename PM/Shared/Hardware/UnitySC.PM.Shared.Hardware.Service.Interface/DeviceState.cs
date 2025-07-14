using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [DataContract(Namespace = "")]
    public class DeviceState : ICloneable
    {
        /// <summary>
        /// Current status of device
        /// </summary>
        [DataMember]
        public DeviceStatus Status { get; private set; }

        /// <summary>
        /// Current status message of device
        /// </summary>
        [DataMember]
        public string StatusMessage { get; private set; }

        public DeviceState(DeviceStatus status, string statusMessage = null)
        {
            Status = status;
            StatusMessage = statusMessage;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum DeviceStatus
    {
        Unknown = 0,
        Ready,
        Starting,
        Busy,
        Error,
        Warning
    }
}