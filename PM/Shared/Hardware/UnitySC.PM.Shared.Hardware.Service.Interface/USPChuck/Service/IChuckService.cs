using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    [ServiceContract(CallbackContract = typeof(IChuckServiceCallback))]
    [ServiceKnownType(typeof(ANAChuckConfig))]
    [ServiceKnownType(typeof(DMTChuckConfig))]
    // The positions below are needed because of ChuckBaseConfig and SubstrateSlotConfig
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(AnaPosition))]
    public interface IChuckService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChuckChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToChuckChanges();

        [OperationContract]
        Response<ChuckBaseConfig> GetChuckConfiguration();

        [OperationContract]
        Response<ChuckState> GetCurrentState();

        [OperationContract]
        Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer);

        [OperationContract]
        Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer);

        [OperationContract]
        Task<Response<VoidResult>> ResetAirbearing();

        [OperationContract]
        Task<Response<VoidResult>> ResetWaferStage();

        [OperationContract]
        Response<Dictionary<string, float>> GetSensorValues();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();

        [OperationContract]
        Response<MaterialPresence> GetWaferPresence();
    }
}
