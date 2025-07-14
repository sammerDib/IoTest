using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Service.Shared.Interface
{
    public interface IProbeCalibrationManagerInit
    {
        void InitializeCalibrationManagers(List<IProbe> probes);
    }
}
