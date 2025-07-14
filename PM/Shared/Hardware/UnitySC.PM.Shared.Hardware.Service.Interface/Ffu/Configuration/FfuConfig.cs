using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ffu
{
    [Serializable]
    [XmlInclude(typeof(Astrofan612FfuConfig))]
    [DataContract]
    public class FfuConfig : DeviceBaseConfig
    {
    }
}
