using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Referential
{
    public class ReferentialSupervisor : IReferentialService
    {
        private ILogger _logger;
        private ServiceInvoker<IReferentialService> _referentialService;

        public ReferentialSupervisor(ILogger<ReferentialSupervisor> logger, IMessenger messenger)
        {
            _referentialService = new ServiceInvoker<IReferentialService>("HARDWAREReferentialService", ClassLocator.Default.GetInstance<SerilogLogger<IReferentialService>>(), messenger,
                ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HardwareControl));
            _logger = logger;
        }

        public Response<PositionBase> ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.ConvertTo(positionToConvert, referentialTo));
        }

        public Response<VoidResult> DeleteSettings(ReferentialTag referentialTag)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.DeleteSettings(referentialTag));
        }

        public Response<VoidResult> DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.DisableReferentialConverter(referentialTag1, referentialTag2));
        }

        public Response<VoidResult> EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.EnableReferentialConverter(referentialTag1, referentialTag2));
        }

        public Response<ReferentialSettingsBase> GetSettings(ReferentialTag referentialTag)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.GetSettings(referentialTag));
        }

        public Response<VoidResult> SetSettings(ReferentialSettingsBase settings)
        {
            return _referentialService.TryInvokeAndGetMessages(s => s.SetSettings(settings));
        }
    }
}
