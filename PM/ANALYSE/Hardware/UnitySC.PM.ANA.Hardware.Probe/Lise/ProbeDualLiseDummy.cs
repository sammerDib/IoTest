using System.Threading;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeDualLiseDummy : ProbeDualLise
    {
        public ProbeDualLiseDummy(ProbeDualLiseConfig config, ProbeLiseConfig configUp, ProbeLiseConfig configDown, ILogger logger) : base(config, configUp, configDown,logger:logger)
        {
        }

        public override void Init()
        {
            Logger.Information("Init ProbeDualLise as dummy");

            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            
            var loggerProbeTop = logFacto.CreateHardwareLogger(ConfigUp.LogLevel.ToString(), Family.ToString(), $"{DeviceID} {ConfigUp.ModulePosition}");
            var loggerProbeBot = logFacto.CreateHardwareLogger(ConfigDown.LogLevel.ToString(), Family.ToString(), $"{DeviceID} {ConfigDown.ModulePosition}");
            {
                ProbeLiseUp = new ProbeLiseDummy(ConfigUp, loggerProbeTop);
                ProbeLiseDown = new ProbeLiseDummy(ConfigDown, loggerProbeBot);

                ProbeLiseUp.Init();
                ProbeLiseDown.Init();

                Status = ProbeStatus.Initialized;
            }
        }

        public override void Shutdown()
        {
            ProbeLiseUp?.Shutdown();
            ProbeLiseDown?.Shutdown();
        }
    }
}
