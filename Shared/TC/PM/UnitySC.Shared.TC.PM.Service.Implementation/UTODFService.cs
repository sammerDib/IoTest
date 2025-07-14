using System.Collections.Generic;
using System.ServiceModel;

using GalaSoft.MvvmLight.Messaging;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.TC.API.Operations.Implementation;
using UnitySC.Shared.TC.API.Service.Interface;
using UnitySC.Shared.TC.API.Shared;
using System;
using UnitySC.Shared.TC.API.Shared.Types;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

using tc_Material = UnitySC.Shared.TC.API.Shared.Material;
using dto_Material = UnitySC.DataAccess.Dto.Material;
using UnitySC.Shared.TC.API.Operations.Interface;
using UnitySC.DataAccess.Dto;

namespace UnitySC.Shared.TC.API.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]    
    public class UTODFService : DuplexServiceBase<IUTODFServiceCB>, IUTODFService, IUTODFServiceCB,
        IAlarmServiceCB
    {        
        private IUTODFOperations _utoOperations;
        public IUTODFOperations UTOOperations { get => _utoOperations; set => _utoOperations = value; }

        public UTODFService(ILogger logger) : base(logger, ExceptionType.UTOException)
        { 
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            //_utoOperations = ClassLocator.Default.GetInstance<UTODFOperations>();
        }

        public override void Init()
        {
            UTOOperations = ClassLocator.Default.GetInstance<IUTODFOperations>();
            UTOOperations.RecipeOperations.Init();
            UTOOperations.AlarmOperations.Init();           
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to UTO change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to UTO change"));
            });
        }

        public void OnChangedDFStatus(string value)
        {
            _logger.Error("OnChangedDFStatus test !!");
        }

        Response<List<Alarm>> IAlarmService.GetAllAlarms()
        {
            return InvokeDataResponse<List<Alarm>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("Get active alarms for UTO");
                    List<Alarm> alarms = _utoOperations.AlarmOperations.GetAllAlarms();
                    messageContainer.Add(new Message(MessageLevel.Information, "Alarms list sent = " + string.Join(",", alarms)));
                    return alarms;
                }
                else
                {
                    _logger.Error("alarmsOperation is null !!");
                    return new List<Alarm>();
                }
            });
        }

        Response<List<CommonEvent>> ICommonEventService.GetAll()
        {
            return InvokeDataResponse<List<CommonEvent>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("Get all CEID for UTO");
                    List<CommonEvent> ceids = _utoOperations.CEOperations.CEGetAll();
                    messageContainer.Add(new Message(MessageLevel.Information, "CEID List sent = " + string.Join(",", ceids)));
                    return ceids;
                }
                else
                {
                    _logger.Error("ceOperations is null !!");
                    return new List<CommonEvent>();
                }
            });
        }

        Response<List<EquipmentConstant>> IEquipmentConstantService.ECGetAllRequest()
        {
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("Get all CEID for UTO");
                    List<EquipmentConstant> ecids = _utoOperations.ECOperations.ECGetAllRequest();
                    messageContainer.Add(new Message(MessageLevel.Information, "ECID list sent = " + string.Join(",", ecids)));
                    return ecids;
                }
                else
                {
                    _logger.Error("_ecOperations is null !!");
                    return new List<EquipmentConstant>();
                }
            });
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> ids)
        { 
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("Get ECID for UTO");
                    List<EquipmentConstant> ecids= _utoOperations.ECOperations.ECGetRequest(ids);
                    messageContainer.Add(new Message(MessageLevel.Information, "ECIDs sent"));
                    return ecids;
                }
                else
                {
                    _logger.Error("_ecOperations is null !!");
                    return null;
                }
            });
        }

        Response<bool> IEquipmentConstantService.ECSetRequest(EquipmentConstant ecid)
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("Set ECID from UTO");
                    bool status = _utoOperations.ECOperations.ECSetRequest(ecid);
                    messageContainer.Add(new Message(MessageLevel.Information, $"ECID set from UTO: {ecid.Name} = {ecid.ValueAsString} [status {status.ToString()}]"));
                    return status;
                }
                else
                {
                    _logger.Error("_ecOperations is null !!");
                    return false;
                }
            });
        }


        Response<VoidResult> IRecipeDFService.SelectRecipe(DataflowFullInfo dfRecipe)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("SelectRecipe from UTO");
                    _utoOperations.RecipeOperations.SelectRecipe(dfRecipe);
                    messageContainer.Add(new Message(MessageLevel.Information, $"SelectRecipe {dfRecipe.Name} from UTO"));
                }
                else
                {
                    _logger.Error("_recipeOperations is null !!");
                }
            });
        }

        Response<VoidResult> IRecipeDFService.StartJob_Material(tc_Material material)
        {
            return InvokeVoidResponse(messageContainer =>
            { 
                if (_utoOperations != null)
                {
                    _logger.Information("StartJob_Material from UTO");
                    _utoOperations.RecipeOperations.StartJob_Material(material);
                    messageContainer.Add(new Message(MessageLevel.Information, $"Start JobID {material.ProcessJobID} with a material LP{material.LoadportID}.S{material.SlotID} from UTO")); 
                }
                else
                {
                    _logger.Error("_recipeOperations is null !!"); 
                }
            });
        }
        public Response<UTOJobProgram> StartRecipeDF(DataflowFullInfo dfRecipe, List<tc_Material> materials)
        {
            return InvokeDataResponse<UTOJobProgram>(messageContainer =>
            {
                UTOJobProgram newJobProgram;
                if (_utoOperations != null)
                {
                    _logger.Information("StartRecipeDF from UTO");
                    newJobProgram = _utoOperations.RecipeOperations.StartRecipeDF(dfRecipe, materials);
                    messageContainer.Add(new Message(MessageLevel.Information, $"StartRecipe {dfRecipe?.Name} from UTO"));
                    
                    //DFRecipeProcessStarted(dfRecipe);
                    return newJobProgram;
                }
                else
                {
                    _logger.Error("_recipeOperations is null !!");
                    return new UTOJobProgram();
                }
            });
        }

        Response<VoidResult> IRecipeDFService.AbortRecipe(DataflowFullInfo dfRecipe )
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("AbortRecipe from UTO");
                    _utoOperations.RecipeOperations.AbortRecipe(dfRecipe);
                    messageContainer.Add(new Message(MessageLevel.Information, $"AbortRecipe {dfRecipe.Name} from UTO"));
                }
                else
                {
                    _logger.Error("_recipeOperations is null !!");
                }
            });
        }

        Response<List<DataflowFullInfo>> IRecipeDFService.GetAllDataflowRecipe(List<ActorType> actors)
        {
            return InvokeDataResponse<List<DataflowFullInfo>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("GetAllDataflowRecipe from UTO");
                    int toolId = 4; // to get from configuration
                    List<DataflowFullInfo> recipes = _utoOperations.RecipeOperations.GetAllRecipe(actors , toolId);                    
                    messageContainer.Add(new Message(MessageLevel.Information, "GetAllRecipe " + string.Join(",", recipes + " from UTO")));
                    return recipes;
                }
                else
                {
                    _logger.Error("_utoOperations is null !!");
                    return new List<DataflowFullInfo>();
                }
            });
        }

        Response<List<StatusVariable>> IStatusVariableService.SVGetAllRequest()
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("SVGetAllRequest from UTO");
                    List<StatusVariable> svids = _utoOperations.SVOperations.SVGetAllRequest();
                    messageContainer.Add(new Message(MessageLevel.Information, "SVGetAllRequest " + string.Join(",", svids + " from UTO")));
                    return svids;
                }
                else
                {
                    _logger.Error("_svOperations is null !!");
                    return new List<StatusVariable>();
                }
            });
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> ids)
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                if (_utoOperations != null)
                {
                    _logger.Information("SVGetRequest from UTO");
                    List<StatusVariable> sids = _utoOperations.SVOperations.SVGetRequest(ids);
                    messageContainer.Add(new Message(MessageLevel.Information, "SVGetRequest " + String.Join(",", sids) + " from UTO"));
                    return sids;
                }
                else
                {
                    _logger.Error("_svOperations is null !!");
                    return null;
                }
            });
        }
         

        public void SetAlarm(int alarmID)
        {
            SetAlarm(new List<int> { alarmID });
        }

        public void SetAlarm(List<int> alarms)
        {
            InvokeCallback(i => i.SetAlarm(alarms));
        }

        public void ResetAlarm(int alarmID)
        {
            ResetAlarm(new List<int>() { alarmID });
        }

        public void ResetAlarm(List<int> alarms)
        {
            InvokeCallback(i => i.ResetAlarm(alarms));
        }

        public void FireEvent(CommonEvent ecid)
        {
            InvokeCallback(i => i.FireEvent(ecid));
        }


        public bool SetECValue(EquipmentConstant equipmentConstant)
        {
            return SetECValues(new List<EquipmentConstant> { equipmentConstant });
        }

        public bool SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            bool result = false;
            InvokeCallback(i => result = i.SetECValues(equipmentConstants));
            return result;
        }

        public void DFRecipeProcessStarted(DataflowFullInfo dfRrecipe)
        {
            InvokeCallback(i => i.DFRecipeProcessStarted(dfRrecipe));
        }

        public void DFRecipeProcessComplete(DataflowFullInfo dfRrecipe, string status)
        {
            InvokeCallback(i => i.DFRecipeProcessComplete(dfRrecipe, status));
        }

        public void DFRecipeAdded(DataflowFullInfo dfRrecipe)
        {
            InvokeCallback(i => i.DFRecipeAdded(dfRrecipe));
        }

        public void DFRecipeDeleted(DataflowFullInfo dfRrecipe)
        {
            InvokeCallback(i => i.DFRecipeDeleted(dfRrecipe));
        }

        public void PMRecipeProcessStarted(ActorType pmType, DataflowFullInfo pmRrecipe)
        {
            InvokeCallback(i => i.PMRecipeProcessStarted(pmType, pmRrecipe));
        }

        public void PMRecipeProcessComplete(ActorType pmType, DataflowFullInfo pmRecipe, string status)
        {
            InvokeCallback(i => i.PMRecipeProcessComplete(pmType, pmRecipe, status));
        }

        public bool SVSetMessage(List<StatusVariable> statusVariables)
        {
            bool result = false;
            InvokeCallback(i => result = i.SVSetMessage(statusVariables));
            return result;
        }

        public bool SVSetMessage(StatusVariable statusVariables)
        {
            return SVSetMessage(new List<StatusVariable>() {statusVariables});
        }

         

    }
}
