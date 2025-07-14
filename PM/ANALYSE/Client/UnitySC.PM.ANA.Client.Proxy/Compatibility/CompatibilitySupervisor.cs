using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Compatibility
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CompatibilitySupervisor
    {
        private ILogger _logger;
        private ServiceInvoker<ICompatibilityService> _compatibilityService;

        public CompatibilitySupervisor(ILogger<CompatibilitySupervisor> logger, IMessenger messenger)
        {
            _logger = logger;
            _compatibilityService = new ServiceInvoker<ICompatibilityService>("ANALYSECompatibilityService", ClassLocator.Default.GetInstance<SerilogLogger<ICompatibilityService>>(), messenger, ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
        }

        public ProbeCompatibility GetProbeCompatibility()
        {
            return _compatibilityService.TryInvokeAndGetMessages(x => x.GetProbeCompatibility())?.Result;
        }

        public void SaveProbeCompatibility(ProbeCompatibility probeCompatibility)
        {
            _compatibilityService.TryInvokeAndGetMessages(x => x.SaveProbeCompatibility(probeCompatibility));
        }

        public ProbeCompatibilityResult TestProbeCompatibility(ProbeCompatibilityTestInput input)
        {
            return _compatibilityService.TryInvokeAndGetMessages(x => x.TestProbeCompatibility(input))?.Result;
        }
    }
}
