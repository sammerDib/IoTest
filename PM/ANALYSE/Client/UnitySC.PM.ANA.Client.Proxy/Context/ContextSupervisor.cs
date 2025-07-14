using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Context
{
    public class ContextSupervisor : IContextService
    {
        private ServiceInvoker<IContextService> _contextService;

        public ContextSupervisor(IMessenger messenger)
        {
            _contextService = new ServiceInvoker<IContextService>("ANALYSEContextService",
                ClassLocator.Default.GetInstance<SerilogLogger<IContextService>>(), messenger,
                ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
        }

        public Response<XYPositionContext> GetXYPosition()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetXYPosition());
        }

        public Response<VoidResult> Apply(ANAContextBase context)
        {
            _contextService.TryInvokeAndGetMessages(service => service.Apply(context));
            return null;
        }

        public Response<LightsContext> GetLights()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetLights());
        }

        public Response<TopImageAcquisitionContext> GetTopImageAcquisitionContext()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetTopImageAcquisitionContext());
        }

        public Response<TopObjectiveContext> GetTopObjectiveContext()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetTopObjectiveContext());
        }

        public Response<BottomImageAcquisitionContext> GetBottomImageAcquisitionContext()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetBottomImageAcquisitionContext());
        }

        public Response<BottomObjectiveContext> GetBottomObjectiveContext()
        {
            return _contextService.TryInvokeAndGetMessages(service => service.GetBottomObjectiveContext());
        }
    }
}
