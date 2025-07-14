using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Host
{
    public class MountainsServer : BaseServer
    {
        private IMountainsGatewayService _mountainsGatewayService;

        public MountainsServer(ILogger logger) : base(logger)
        {
            _mountainsGatewayService = ClassLocator.Default.GetInstance<IMountainsGatewayService>();
        }

        public override void Start()
        {
            StartService((BaseService)_mountainsGatewayService);
        }

        public override void Stop()
        {
            StopAllServiceHost();
        }
    }
}
