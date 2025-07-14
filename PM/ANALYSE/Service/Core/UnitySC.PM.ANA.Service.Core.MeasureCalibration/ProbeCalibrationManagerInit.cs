using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Shared.Interface;

namespace UnitySC.PM.ANA.Service.Core.MeasureCalibration
{
    public class ProbeCalibrationManagerInit : IProbeCalibrationManagerInit
    {
        public void InitializeCalibrationManagers(List<IProbe> probes)
        {
            foreach (var probe in probes)
            {
                probe.CalibrationManager = ProbeCalibrationManagerFactory.CreateCalibrationManager(probe.Configuration as ProbeConfigBase, new System.Threading.CancellationToken());
            }
        }
    }
}
