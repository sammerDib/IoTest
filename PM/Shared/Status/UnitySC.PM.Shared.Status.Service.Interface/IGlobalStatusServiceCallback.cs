using System.ServiceModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Status.Service.Interface
{
    [ServiceContract]
    public interface IGlobalStatusServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void GlobalStatusChangedCallback(GlobalStatus globalStatus);

        [OperationContract(IsOneWay = true)]
        void ToolModeChangedCallback(ToolMode toolMode);
    }
}
