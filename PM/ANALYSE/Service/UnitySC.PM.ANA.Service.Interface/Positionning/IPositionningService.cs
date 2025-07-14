using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Positionning
{
    [ServiceContract(CallbackContract = typeof(IPositionningServiceCallback))]
    public interface IPositionningService
    {
        [OperationContract]
        Response<CompositePosition> StartConversion(CompositePosition position, Referential targetReferential);
    }
}
