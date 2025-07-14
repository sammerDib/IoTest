using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface ISingleProbeConfig : IProbeConfig
    {
        List<LightConfig> Lights { get; set; }

        List<CameraConfigBase> Cameras { get; set; }
    }
}
