using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Interlock;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    [Serializable]
    [DataContract]
    public class DMTChamberConfig : ChamberConfig
    {
        [DataMember]
        public Interlocks Interlocks { get; set; }
    }
}
