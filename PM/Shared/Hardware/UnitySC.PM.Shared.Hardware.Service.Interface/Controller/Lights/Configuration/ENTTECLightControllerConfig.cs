using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class ENTTECLightControllerConfig : ControllerConfig
    {
        public List<LightIdLink> LightIdLinks { get; set; }
    }
}
