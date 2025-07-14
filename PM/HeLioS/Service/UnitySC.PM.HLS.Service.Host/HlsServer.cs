using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.HLS.Hardware.Manager;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.HLS.Service.Host
{
    public class HlsServer : BaseServer
    {
        private ILogger _logger;
        private IGlobalStatusService _globalStatusService;
        private IPMUserService _pmUserService;

        public HlsServer(ILogger logger) : base(logger)
        {
            _logger = logger;
            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();
        }

        public override void Start()
        {
            StartService((BaseService)_globalStatusService);
            StartService((BaseService)_pmUserService);
        }

        public override void Stop()
        {
            var hlsHM = ClassLocator.Default.GetInstance<HlsHardwareManager>();
            hlsHM.Shutdown();
            StopAllServiceHost();
        }
    }
}
