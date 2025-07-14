using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    [ServiceContract]
    public interface IChuckServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(ChuckState state);
    }
}
