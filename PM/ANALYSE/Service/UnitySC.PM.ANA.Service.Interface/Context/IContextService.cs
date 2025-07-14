using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [ServiceContract]
    public interface IContextService
    {
        [OperationContract]
        Response<VoidResult> Apply(ANAContextBase context);

        [OperationContract]
        Response<LightsContext> GetLights();

        [OperationContract]
        Response<XYPositionContext> GetXYPosition();

        [OperationContract]
        Response<TopImageAcquisitionContext> GetTopImageAcquisitionContext();

        [OperationContract]
        Response<TopObjectiveContext> GetTopObjectiveContext();

        [OperationContract]
        Response<BottomImageAcquisitionContext> GetBottomImageAcquisitionContext();

        [OperationContract]
        Response<BottomObjectiveContext> GetBottomObjectiveContext();
    }
}
