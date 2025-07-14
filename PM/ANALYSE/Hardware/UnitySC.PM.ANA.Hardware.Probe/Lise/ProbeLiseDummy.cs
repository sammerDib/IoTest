using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public class ProbeLiseDummy : ProbeLise
    {
        public ProbeLiseDllProxyDummy ProbeLiseProxyDummy { get; set; }

        public ProbeLiseDummy(ProbeLiseConfig config, ILogger logger) : base(config, logger)
        {
            ProbeLiseDllProxy = ProbeLiseDllProxyDummy.Instance;
        }

        public override void Init()
        {
            Logger.Information("Init ProbeLise as dummy");
            return;
        }

        public override void Shutdown()
        {
            return;
        }
    }
}
