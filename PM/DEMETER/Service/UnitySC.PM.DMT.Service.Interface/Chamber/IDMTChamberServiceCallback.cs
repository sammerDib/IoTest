using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Chamber
{
    [ServiceContract]
    public interface IDMTChamberServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateIsInMaintenanceCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void UpdateArmNotExtendedCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void UpdateEfemSlitDoorOpenPositionCallback(bool value);

        [OperationContract(IsOneWay = true)]
        void UpdateIsReadyToLoadUnloadCallback(bool value);        

        [OperationContract(IsOneWay = true)]
        void UpdateInterlocksCallback(InterlockMessage interlock);

        [OperationContract(IsOneWay = true)]
        void UpdateSlitDoorPositionCallback(SlitDoorPosition position);

        [OperationContract(IsOneWay = true)]
        void UpdateSlitDoorOpenPositionCallback(bool position);

        [OperationContract(IsOneWay = true)]
        void UpdateSlitDoorClosePositionCallback(bool position);

        [OperationContract(IsOneWay = true)]
        void UpdateCdaPneumaticValveCallback(bool valveIsOpened);
    }
}
