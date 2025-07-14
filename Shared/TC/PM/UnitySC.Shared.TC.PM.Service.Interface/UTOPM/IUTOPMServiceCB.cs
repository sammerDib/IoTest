using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.PM.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface IUTOPMServiceCB

    {

        #region IMaterialServiceCB

        [OperationContract(IsOneWay = true)]
        void PMReadyToTransfer();               // PM en position chargement

        #endregion IMaterialServiceCB

        #region IStatusVariableServiceCB

        [OperationContract(Name = "SVSetMessage_List", IsOneWay = true)]
        void SVSetMessage(List<StatusVariable> statusVariables);

        [OperationContract(Name = "SVSetMessage", IsOneWay = true)]
        void SVSetMessage(StatusVariable statusVariable);

        #endregion IStatusVariableServiceCB

        #region IEquipmentConstantServiceCB

        [OperationContract(IsOneWay = true)]
        void SetECValue(EquipmentConstant equipmentConstant);

        [OperationContract(IsOneWay = true)]
        void SetECValues(List<EquipmentConstant> equipmentConstants);

        #endregion IEquipmentConstantServiceCB

        #region ICollectionEventServiceCB

        [OperationContract(IsOneWay = true)]
        void FireEvent(CommonEvent ecid);

        #endregion ICollectionEventServiceCB

        #region IAlarmServiceCB

        // Pour envoyer un message
        [OperationContract(Name = "SetAlarmID", IsOneWay = true)]
        void SetAlarm(Alarm alarm);

        [OperationContract(Name = "SetAlarmIDList", IsOneWay = true)]
        void SetAlarm(List<Alarm> alarms);

        [OperationContract(Name = "ResetSetAlarmID", IsOneWay = true)]
        void ResetAlarm(Alarm alarm);

        [OperationContract(Name = "ResetSetAlarmIDList", IsOneWay = true)]
        void ResetAlarm(List<Alarm> alarms);

        #endregion IAlarmServiceCB

        bool AskAreYouThere();
    }
}
