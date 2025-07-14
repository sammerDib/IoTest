using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Core.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CompatibilityService : BaseService, ICompatibilityService
    {
        private readonly CompatibilityManager _compatibilityManager;

        public CompatibilityService(ILogger<CompatibilityService> logger, CompatibilityManager compatibilityManager) : base(logger, ExceptionType.CompatibilityException)
        {
            _compatibilityManager = compatibilityManager;
        }

        public Response<ProbeCompatibility> GetProbeCompatibility()
        {
            return InvokeDataResponse(() =>
            {
                return _compatibilityManager.Probe;
            });
        }

        public Response<VoidResult> SaveProbeCompatibility(ProbeCompatibility probeCompatibility)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _compatibilityManager.Probe = probeCompatibility;
                _compatibilityManager.SaveProbe();
            });
        }

#pragma warning disable CS0162 //Code not accessible

        public Response<ProbeCompatibilityResult> TestProbeCompatibility(ProbeCompatibilityTestInput input)
        {
            return InvokeDataResponse(() =>
            {
                throw new NotImplementedException("TestProbeCompatibility not implemented");
                return new ProbeCompatibilityResult();
            });
        }
    }
}
