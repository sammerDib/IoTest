using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Service.Interface.Chamber
{
    [ServiceContract]
    public interface IEMEChamberServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateIsInMaintenanceCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void UpdateArmNotExtendedCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void UpdateInterlocksCallback(InterlockMessage interlock);

        [OperationContract(IsOneWay = true)]
        void UpdateSlitDoorPositionCallback(SlitDoorPosition position);
    }
}
