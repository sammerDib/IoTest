using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeModuleConfig : IDeviceConfiguration
    {
        ProbeModuleSettings ProbeModuleSettings { get; set; }
        List<string> LightsID { get; set; }
        List<string> CamerasID { get; set; }
        string ObjectivesSelectorID { get; set; }
        List<ProbeConfigBase> Probes { get; set; }
    }
}