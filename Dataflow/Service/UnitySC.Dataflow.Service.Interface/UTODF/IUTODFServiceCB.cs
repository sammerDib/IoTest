using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Service.Interface
{
    // DF Serveur -> UTO Client
    [ServiceContract]
    [ServiceKnownType(typeof(ANADataCollection))]
    public interface IUTODFServiceCB
    {
        [OperationContract(IsOneWay = true)]
        void OnChangedDFStatus(string value);

        #region IAlarmServiceCB

        [OperationContract(Name = "SetAlarmIDList", IsOneWay = true)]
        void SetAlarm(List<Alarm> alarms);

        [OperationContract(Name = "ResetSetAlarmIDList", IsOneWay = true)]
        void ResetAlarm(List<Alarm> alarms);

        [OperationContract(IsOneWay = true)]
        void StopCancelAllJobs();

        #endregion IAlarmServiceCB

        #region ICommonEventServiceCB

        [OperationContract(IsOneWay = true)]
        void FireEvent(CommonEvent ce);

        [OperationContract(Name = "NotifyFDCCollectionChanged", IsOneWay = true)]
        void NotifyFDCCollectionChanged(List<FDCData> fdcDataCollection);

        #endregion ICommonEventServiceCB

        #region IEquipmentConstantServiceCB

        [OperationContract(IsOneWay = true)]
        void SetECValues(List<EquipmentConstant> equipmentConstants);

        #endregion IEquipmentConstantServiceCB

        #region IRecipeDFServiceCB

        // DataFlow
        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessStarted(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessComplete(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status);

        [OperationContract(IsOneWay = true)]
        void DFRecipeAdded(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract(IsOneWay = true)]
        void DFRecipeDeleted(DataflowRecipeInfo dfRecipeInfo);

        // PM
        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessStarted(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer);

        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status);

        [OperationContract(IsOneWay = true)]
        void PMRecipeAcquisitionComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status);
        #endregion IRecipeDFServiceCB

        #region IStatusVariableServiceCB

        [OperationContract(Name = "SVSetMessage_List", IsOneWay = true)]
        void SVSetMessage(List<StatusVariable> statusVariables);

        #endregion IStatusVariableServiceCB
    }
}
