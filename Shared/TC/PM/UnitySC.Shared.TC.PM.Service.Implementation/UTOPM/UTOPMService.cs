using System.Collections.Generic;
using System.ServiceModel;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UTOPMService : DuplexServiceBase<IUTOPMServiceCB>, IUTOPMService, IUTOPMServiceCB,
                                                IAlarmServiceCB, IStatusVariableServiceCB, IMaterialServiceCB, ICommonEventServiceCB, IEquipmentConstantServiceCB
    {
        private object _lock = new object();
        private IUTOPMOperations _utopmOperations;
        private IPMTCManager _pmtcManager;
        public IUTOPMOperations UTOPMOperations { get => _utopmOperations; set => _utopmOperations = value; }


        public UTOPMService(ILogger logger) : base(logger, ExceptionType.UTOException)
        {
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        public override void Init()
        {
            UTOPMOperations = ClassLocator.Default.GetInstance<IUTOPMOperations>();
            UTOPMOperations.MaterialOperations.Init();
            UTOPMOperations.AlarmOperations.Init();

            var configuration = ClassLocator.Default.GetInstance<IAutomationConfiguration>();
            UTOPMOperations.CEOperations.Init(configuration.CEConfigurationFilePath);

            _pmtcManager = ClassLocator.Default.GetInstance<IPMTCManager>();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    base.Unsubscribe();
                    messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to UTO change"));
                }
            });
        }

        //ANALYSE
        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("UTO subscribed to PM");
                    base.Subscribe();
                    messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to UTO change"));
                }
            });
        }

        #region Call from UTO

        public Response<List<Alarm>> GetAllAlarms()
        {
            return InvokeDataResponse<List<Alarm>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Get active alarms for UTO");
                        List<Alarm> alarms = UTOPMOperations.AlarmOperations.GetAllAlarms();
                        messageContainer.Add(new Message(MessageLevel.Information, "Alarms list sent = " + string.Join(",", alarms)));
                        return alarms;
                    }
                    else
                    {
                        _logger.Error("UTOPMOperations is null !!");
                        return new List<Alarm>();
                    }
                }
            });
        }
        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Request all FDCs update from UTO");
                        ClassLocator.Default.GetInstance<FDCManager>().RequestAllFDCsUpdate();
                        messageContainer.Add(new Message(MessageLevel.Information, "Request all FDCs update"));
                    }
                    else
                    {
                        _logger.Error("UTOPMOperations is null !!");
                    }
                }
            });
        }

        public Response<VoidResult> NotifyAlarmChanged(Alarm alarm)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Reset Alarm from UTO");
                        UTOPMOperations.AlarmOperations.NotifyAlarmChanged(alarm);
                        messageContainer.Add(new Message(MessageLevel.Information, $"Alarm {alarm.Name} is cleared"));
                    }
                    else
                    {
                        _logger.Error("UTOPMOperations is null !!");
                    }
                }
            });
        }

        public Response<List<CommonEvent>> GetAll()
        {
            return InvokeDataResponse<List<CommonEvent>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Get all CEID for UTO");
                        List<CommonEvent> ceids = UTOPMOperations.CEOperations.CEGetAll();
                        messageContainer.Add(new Message(MessageLevel.Information, "CEID List sent = " + string.Join(",", ceids)));
                        return ceids;
                    }
                    else
                    {
                        _logger.Error("ceOperations is null !!");
                        return new List<CommonEvent>();
                    }
                }
            });
        }

        public Response<List<EquipmentConstant>> ECGetAllRequest()
        {
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Get all CEID for UTO");
                        List<EquipmentConstant> ecids = UTOPMOperations.ECOperations.ECGetAllRequest();
                        messageContainer.Add(new Message(MessageLevel.Information, "ECID list sent = " + string.Join(",", ecids)));
                        return ecids;
                    }
                    else
                    {
                        _logger.Error("_ecOperations is null !!");
                        return new List<EquipmentConstant>();
                    }
                }
            });
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> ids)
        {
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Get ECID for UTO");
                        List<EquipmentConstant> ecids = UTOPMOperations.ECOperations.ECGetRequest(ids);
                        messageContainer.Add(new Message(MessageLevel.Information, "ECIDs sent."));
                        return ecids;
                    }
                    else
                    {
                        _logger.Error("_ecOperations is null !!");
                        return null;
                    }
                }
            });
        }

        public Response<bool> ECSetRequest(EquipmentConstant ecid)
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Set ECID from UTO");
                        bool status = UTOPMOperations.ECOperations.ECSetRequest(ecid);
                        messageContainer.Add(new Message(MessageLevel.Information, $"ECID set from UTO: {ecid.Name} = {ecid.ValueAsString} [status {status.ToString()}]"));
                        return status;
                    }
                    else
                    {
                        _logger.Error("_ecOperations is null !!");
                        return false;
                    }
                }
            });
        }

        public Response<bool> PrepareForTransfer(TransferType transferType, MaterialTypeInfo materialTypeInfo)
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    bool result = false;
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("PrepareForTransfer from UTO");
                        result = UTOPMOperations.MaterialOperations.PrepareForTransfer(transferType, materialTypeInfo);
                        messageContainer.Add(new Message(MessageLevel.Information, $"PrepareForTransfer from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                    return result;
                }
            });
        }

        public Response<Material> UnloadMaterial()
        {
            return InvokeDataResponse<Material>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    Material material = null;
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("UnloadMaterial from UTO");
                        material = UTOPMOperations.MaterialOperations.UnloadMaterial();
                        messageContainer.Add(new Message(MessageLevel.Information, $"UnloadMaterial from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                    return material;
                }
            });
        }

        public Response<VoidResult> LoadMaterial(Material wafer)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();

                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("LoadMaterial from UTO");
                        UTOPMOperations.MaterialOperations.LoadMaterial(wafer);
                        messageContainer.Add(new Message(MessageLevel.Information, $"LoadMaterial from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                }
            });
        }

        public Response<VoidResult> PostTransfer()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("PostTransfer from UTO");
                        UTOPMOperations.MaterialOperations.PostTransfer();
                        messageContainer.Add(new Message(MessageLevel.Information, $"PostTransfer from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                }
            });
        }

        public Response<VoidResult> StartRecipe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("StartRecipe from UTO");
                        UTOPMOperations.MaterialOperations.StartRecipe();
                        messageContainer.Add(new Message(MessageLevel.Information, $"StartRecipe from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                }
            });
        }

        public Response<VoidResult> AbortRecipe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("AbortRecipe from UTO");
                        UTOPMOperations.MaterialOperations.AbortRecipe();
                        messageContainer.Add(new Message(MessageLevel.Information, $"AbortRecipe from UTO"));
                    }
                    else
                    {
                        _logger.Error("_materialOperations is null !!");
                    }
                }
            });
        }

        public Response<List<StatusVariable>> SVGetAllRequest()
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                AutoSubscribe();

                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Verbose("SVGetAllRequest from UTO");
                        List<StatusVariable> svids = UTOPMOperations.SVOperations.SVGetAllRequest();
                        messageContainer.Add(new Message(MessageLevel.Information, "SVGetAllRequest " + string.Join(",", svids + " from UTO")));
                        return svids;
                    }
                    else
                    {
                        _logger.Error("_svOperations is null !!");
                        return new List<StatusVariable>();
                    }
                }
            });
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> ids)
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("SVGetRequest from UTO");
                        List<StatusVariable> svids = UTOPMOperations.SVOperations.SVGetRequest(ids);
                        messageContainer.Add(new Message(MessageLevel.Information, "SVGetRequest " + string.Join(",", ids) + " from UTO"));
                        return svids;
                    }
                    else
                    {
                        _logger.Error("_svOperations is null !!");
                        return null;
                    }
                }
            });
        }

        public Response<bool> Initialization()
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                AutoSubscribe();

                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("SVSetRequest from UTO");
                        bool status = UTOPMOperations.MaterialOperations.PMInitialization();
                        messageContainer.Add(new Message(MessageLevel.Information, $"SVSetRequest from UTO: status = {status}"));
                        return status;
                    }
                    else
                    {
                        _logger.Error("UTOOperations is null !!");
                        return false;
                    }
                }
            });
        }

        public Response<double> GetAlignmentAngle()
        {
            return InvokeDataResponse<double>(messageContainer =>
            {
                AutoSubscribe();

                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        double angle = ClassLocator.Default.GetInstance<ModuleConfiguration>().PrealignementAngle;
                        _logger.Information($"GetAlignmentAngle from UTO: angle = {angle}°");
                        messageContainer.Add(new Message(MessageLevel.Information, $"GetAlignmentAngle from UTO: angle = {angle}°"));
                        return angle;
                    }
                    else
                    {
                        _logger.Error("UTOOperations is null !!");
                        return 0;
                    }
                }
            });
        }
        #endregion Call from UTO

        #region Callback to UTO

        public void SetAlarm(Alarm alarm)
        {
            InvokeCallback(i => i.SetAlarm(alarm));
        }

        public void SetAlarm(List<Alarm> alarms)
        {
            InvokeCallback(i => i.SetAlarm(alarms));
        }

        public void ResetAlarm(Alarm alarm)
        {
            InvokeCallback(i => i.ResetAlarm(alarm));
        }

        public void ResetAlarm(List<Alarm> alarms)
        {
            InvokeCallback(i => i.ResetAlarm(alarms));
        }

        public void FireEvent(CommonEvent ceid)
        {
            InvokeCallback(i => i.FireEvent(ceid));
        }

        public void SetECValue(EquipmentConstant equipmentConstant)
        {
            InvokeCallback(i => i.SetECValue(equipmentConstant));
        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            InvokeCallback(i => i.SetECValues(equipmentConstants));
        }

        //ANALYSE
        public void PMReadyToTransfer()
        {
            _logger.Debug("Notify PMReadyToTransfer");
            InvokeCallback(i => i.PMReadyToTransfer());
        }

        public void SVSetMessage(List<StatusVariable> statusVariables)
        {
            InvokeCallback(i => i.SVSetMessage(statusVariables));
        }

        public void SVSetMessage(StatusVariable statusVariable)
        {
            SVSetMessage(new List<StatusVariable> { statusVariable });
        }

        #endregion Callback to UTO

        public void Connect()
        {
        }

        public void Disconnect()
        {
        }


        public bool AskAreYouThere()
        {
            bool result = false;
            if ((GetNbClientsConnected() > 0))
            {
                result = true;
            }
            return result;
        }

        private void AutoSubscribe()
        {
            if (GetNbClientsConnected() <= 0)
            {
                lock (_lock)
                    base.Subscribe();
                _logger.Information("UTO subscribed to PM");
            }
        }

        public Response<bool> AreYouThere()
        {
            return new Response<bool>() { Result = true };
        }

        public void StopCancelAllJobs()
        {
            throw new System.NotImplementedException();
        }

        public Response<List<Length>> GetSupportedWaferDimensions()
        {
            return InvokeDataResponse<List<Length>>(messageContainer =>
            {
                AutoSubscribe();
                lock (_lock)
                {
                    if (UTOPMOperations != null)
                    {
                        _logger.Information("Get active alarms for UTO");
                        var diametersSupported = _pmtcManager.GetMaterialDiametersSupported();
                        messageContainer.Add(new Message(MessageLevel.Information, "Diameters Supported list sent = " + string.Join(",", diametersSupported)));
                        return diametersSupported;
                    }
                    else
                    {
                        _logger.Error("UTOPMOperations is null !!");
                        return new List<Length>();
                    }
                }
            });
            
        }
    }
}
