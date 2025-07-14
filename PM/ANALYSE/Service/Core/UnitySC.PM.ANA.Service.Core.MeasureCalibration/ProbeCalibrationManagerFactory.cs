using System.Threading;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Service.Core.MeasureCalibration
{
    public static class ProbeCalibrationManagerFactory
    {
        public static IProbeCalibrationManager CreateCalibrationManager(ProbeConfigBase config, CancellationToken cancellationToken)
        {
            switch (config)
            {
                case ProbeDualLiseConfig probeDualLiseConfig:
                    return new ProbeCalibrationManagerLise(probeDualLiseConfig.DeviceID, cancellationToken, 10);

                case ProbeLiseHFConfig probeLiseHFConfig:
                    return new ProbeCalibrationManagerLiseHF(probeLiseHFConfig, cancellationToken);
            }

            return null;
        }
    }
}
