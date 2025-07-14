using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.EME.Service.Interface.Light
{
    [Serializable]
    public class EMELightConfig : DeviceBaseConfig
    {
        public string Description { get; set; }

        public bool IsMainLight { get; set; }

        public EMELightType Type { get; set; }
    }
}
