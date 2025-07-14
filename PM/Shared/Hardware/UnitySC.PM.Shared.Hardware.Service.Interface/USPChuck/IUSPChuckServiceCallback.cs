using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [ServiceContract]
    public interface IUSPChuckServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateWaferPresenceCallback(MaterialPresence waferPresence);

        [OperationContract(IsOneWay = true)]
        void UpdateChuckIsInLoadingPositionCallback(bool loadingPosition);

        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(ChuckState state);
    }
}
