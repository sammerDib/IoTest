using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Global
{
    [DataContract]
    public class GlobalDevice : ICloneable, IDevice
    {
        [DataMember]
        public DeviceFamily Family { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DeviceState State { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public object Clone()
        {
            var device = (GlobalDevice)MemberwiseClone();
            device.State = (DeviceState)State.Clone();
            return device;
        }
    }
}
