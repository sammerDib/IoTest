using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.API.Shared;

namespace UnitySC.Shared.TC.API.Service.Interface
{
    // DF Serveur -> UTO Client
    [ServiceContract]
    public interface IUTODFServiceCB //: IAlarmServiceCB, ICommonEventServiceCB, IEquipmentConstantServiceCB,IRecipeDFServiceCB, IStatusVariableServiceCB
    {
        [OperationContract(IsOneWay = true)]
        void OnChangedDFStatus(string value);

        #region ICommonEventServiceCB

        [OperationContract(IsOneWay = true)]
        void FireEvent(CommonEvent ecid);
        #endregion

        #region IAlarmServiceCB

        [OperationContract(Name = "SetAlarmIDList", IsOneWay = true)]
        void SetAlarm(List<int> alarms);
        [OperationContract(Name = "ResetSetAlarmIDList", IsOneWay = true)]
        void ResetAlarm(List<int> alarms);

        #endregion

        #region ICommonEventServiceCB

        [OperationContract]
        bool SetECValues(List<EquipmentConstant> equipmentConstants);
        #endregion

        #region IRecipeDFServiceCB
        // DataFlow
        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessStarted(DataflowFullInfo dfRecipe);
        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessComplete(DataflowFullInfo dfRecipe, String status);
        [OperationContract(IsOneWay = true)]
        void DFRecipeAdded(DataflowFullInfo dfRecipe);
        [OperationContract(IsOneWay = true)]
        void DFRecipeDeleted(DataflowFullInfo dfRecipe);

        // PM
        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessStarted(ActorType pmType, DataflowFullInfo pmRrecipe);
        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessComplete(ActorType pmType, DataflowFullInfo pmRrecipe, String status);

        #endregion

        #region IStatusVariableServiceCB
        [OperationContract(Name = "SVSetMessage_List")]
        bool SVSetMessage(List<StatusVariable> statusVariables);
        #endregion
       
    }
}
