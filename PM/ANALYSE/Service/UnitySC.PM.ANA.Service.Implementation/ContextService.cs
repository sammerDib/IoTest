using System.ServiceModel;

using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ContextService : BaseService, IContextService
    {
        private readonly IContextManager _contextManager;

        public ContextService(IContextManager contextManager, ILogger<CompatibilityService> logger) : base(logger,
            ExceptionType.ContextException)
        {
            _contextManager = contextManager;
        }

        public Response<VoidResult> Apply(ANAContextBase context)
        {
            return InvokeVoidResponse(_ => _contextManager.Apply(context));
        }

        public Response<LightsContext> GetLights()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<LightsContext>());
        }

      

        public Response<XYPositionContext> GetXYPosition()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<XYPositionContext>());
        }

        public Response<TopImageAcquisitionContext> GetTopImageAcquisitionContext()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<TopImageAcquisitionContext>());
        }

        public Response<TopObjectiveContext> GetTopObjectiveContext()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<TopObjectiveContext>());
        }

        public Response<BottomImageAcquisitionContext> GetBottomImageAcquisitionContext()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<BottomImageAcquisitionContext>());
        }

        public Response<BottomObjectiveContext> GetBottomObjectiveContext()
        {
            return InvokeDataResponse(_ => _contextManager.GetCurrent<BottomObjectiveContext>());
        }
    }
}
