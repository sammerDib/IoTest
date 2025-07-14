using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;

using Agileo.Common.Logging;

using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Equipment.Abstractions;
using UnitySC.Shared.Data;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;
namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Driver
{
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public class UnityProcessModuleDriver : WcfDriver<IUTOPMService>, IUTOPMService, IUTOPMServiceCB
    {
        #region Fields

        private readonly Timer _areYouThereTimer;
        private bool _firstConnection;
        private bool _areYouThereInProgress;
        private uint _currentRetryNumber;
        private readonly uint _maxRetryNumber;
        #endregion Fields

        #region Events

        public delegate bool EquipmentConstantChangedDelegate(
            object sender,
            EquipmentConstantChangedEventArgs e);

        public delegate bool StatusVariableChangedDelegate(
            object sender,
            StatusVariableChangedEventArgs e);

        public event EventHandler PmReadyToTransfer;

        public event EventHandler<AlarmRaisedEventArgs> AlarmRaised;

        public event EventHandler<AlarmClearedEventArgs> AlarmCleared;

        public event EquipmentConstantChangedDelegate EquipmentConstantChanged;

        public event EventHandler<EventFiredEventArgs> EventFired;

        public event StatusVariableChangedDelegate StatusVariableChanged;

        #endregion Events

        #region Constructor

        public UnityProcessModuleDriver(WcfConfiguration config, ILogger logger)
            : base(config, logger)
        {
            var timeout = TimeSpan.FromSeconds(config.WcfCommunicationCheckDelay);
            _areYouThereTimer = new Timer(timeout.TotalMilliseconds);
            _areYouThereTimer.Elapsed += AreYouThereTimer_Elapsed; //modif rti
            _maxRetryNumber = config.WcfRetryNumber;
        }

        #endregion Constructor

        #region Overrides

        public override bool Connect()
        {
            try
            {
                SubscribeToChanges();
                IsConnected = true;
                _areYouThereTimer.Start();
                _firstConnection = true;
            }
            catch (Exception ex)
            {
                _logger.Debug($"Connect failed in PM driver - {ex.Message}");
                IsConnected = false;
                throw;
            }

            return IsConnected;
        }

        public override void Disconnect()
        {
            try
            {
                _areYouThereTimer.Stop();
                _areYouThereInProgress = false;
                _firstConnection = false;
                UnSubscribeToChanges();
                _serviceInvoker.DisposeChannel();
            }
            catch
            {
                _logger.Debug("Disconnect failed in PM driver");
            }

            IsConnected = false;
        }

        #endregion Overrides

        #region IUTOPMService

        #region IUTOPMService Subscription

        public Response<VoidResult> SubscribeToChanges()
        {
            _logger.Debug($"Service {nameof(SubscribeToChanges)} has been called");

            var resp = new Response<VoidResult>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.SubscribeToChanges());
                _logger.Debug("Subscribed");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"PM UTO subscribe error - {ex.Message}");
                throw;
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            _logger.Debug($"Service {nameof(UnSubscribeToChanges)} has been called");

            var resp = new Response<VoidResult>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
                _logger.Debug("Unsubscribed");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"PM UTO UnSubscribe error - {ex.Message}");
            }

            return resp;
        }

        public Response<bool> AreYouThere()
        {
            if (_firstConnection || !IsConnected)
            {
                _logger.Debug($"Service {nameof(AreYouThere)} has been called. Server is {(IsConnected ? "connected" : "not connected")}");
                _firstConnection = false;
            }

            var resp = new Response<bool>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.AreYouThere());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"PM UTO AreYouThere error : {ex.Message}");
            }

            return resp;
        }

        private void AreYouThereTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsConnected) return;

            if (_areYouThereInProgress)
            {
                return;
            }
            try
            {
                _areYouThereInProgress = true;
                var isThere = AreYouThere().Result;
                if (isThere)
                {
                    _currentRetryNumber = 0;
                    return;
                }

                _currentRetryNumber++;
                if (_currentRetryNumber <= _maxRetryNumber)
                {
                    _logger.Debug($"Server is not detected anymore. Keep PM as connected. retry #{_currentRetryNumber}/{_maxRetryNumber}");
                    return;
                }

                IsConnected = false;
                _logger.Debug("Server is not detected anymore after several retries. Notify connection closed.");
            }
            finally
            {
                _areYouThereInProgress = false;
            }
        }

        #endregion IUTOPMService Subscription

        #region IAlarmService

        public Response<List<Alarm>> GetAllAlarms()
        {
            _logger.Debug($"Service {nameof(GetAllAlarms)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.GetAllAlarms());
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAllAlarms)}: " + ex);
                return new Response<List<Alarm>>();
            }
        }

        public Response<VoidResult> NotifyAlarmChanged(Alarm alarm)
        {
            _logger.Debug($"Service {nameof(NotifyAlarmChanged)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.NotifyAlarmChanged(alarm));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(NotifyAlarmChanged)}: " + ex);
                return new Response<VoidResult>();
            }
        }

        #endregion IAlarmService

        #region IAlarmServiceCB

        public void SetAlarm(Alarm alarm)
        {
            _logger.Debug($"Callback {nameof(SetAlarm)} has been called by PM");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmRaised?.Invoke(
                                this,
                                new AlarmRaisedEventArgs(new List<Alarm>() { alarm }));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"{nameof(SetAlarm)}: " + ex);
                        }
                    })
                .Wait();
        }

        public void SetAlarm(List<Alarm> alarms)
        {
            _logger.Debug($"Callback {nameof(SetAlarm)} has been called by PM");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmRaised?.Invoke(this, new AlarmRaisedEventArgs(alarms));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"{nameof(SetAlarm)}: " + ex);
                        }
                    })
                .Wait();
        }

        void IUTOPMServiceCB.ResetAlarm(Alarm alarm)
        {
            _logger.Debug($"Callback {nameof(IUTOPMServiceCB.ResetAlarm)} has been called by PM");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmCleared?.Invoke(
                                this,
                                new AlarmClearedEventArgs(new List<Alarm>() { alarm }));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"{nameof(IUTOPMServiceCB.ResetAlarm)}: " + ex);
                        }
                    })
                .Wait();
        }

        void IUTOPMServiceCB.ResetAlarm(List<Alarm> alarms)
        {
            _logger.Debug($"Callback {nameof(IUTOPMServiceCB.ResetAlarm)} has been called by PM");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmCleared?.Invoke(this, new AlarmClearedEventArgs(alarms));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"{nameof(IUTOPMServiceCB.ResetAlarm)}: " + ex);
                        }
                    })
                .Wait();
        }

        #endregion IAlarmServiceCB

        #region ICommonEventService

        public Response<List<CommonEvent>> GetAll()
        {
            _logger.Debug($"Service {nameof(GetAll)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.GetAll());
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAll)}: " + ex);
                return new Response<List<CommonEvent>>();
            }
        }

        #endregion ICommonEventService

        #region ICommonEventServiceCB

        public void FireEvent(CommonEvent ecid)
        {
            _logger.Debug($"Callback {nameof(FireEvent)} has been called by PM");

            Task.Run(
                () =>
                {
                    try
                    {
                        EventFired?.Invoke(this, new EventFiredEventArgs(ecid));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"{nameof(FireEvent)}: " + ex);
                    }
                });
        }

        #endregion ICommonEventServiceCB

        #region IEquipmentConstantService

        public Response<List<EquipmentConstant>> ECGetAllRequest()
        {
            _logger.Debug($"Service {nameof(ECGetAllRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECGetAllRequest());
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(ECGetAllRequest)}: " + ex);
                return new Response<List<EquipmentConstant>>();
            }
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> id)
        {
            _logger.Debug($"Service {nameof(ECGetRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECGetRequest(id));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(ECGetRequest)}: " + ex);
                return new Response<List<EquipmentConstant>>();
            }
        }

        public Response<bool> ECSetRequest(EquipmentConstant ecid)
        {
            _logger.Debug($"Service {nameof(ECSetRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECSetRequest(ecid));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(ECSetRequest)}: " + ex);
                return new Response<bool>();
            }
        }

        #endregion IEquipmentConstantService

        #region IEquipmentConstantServiceCB

        public void SetECValue(EquipmentConstant equipmentConstant)
        {
            _logger.Debug($"Callback {nameof(SetECValue)} has been called by PM");

            Task.Run(
                () =>
                {
                    EquipmentConstantChanged?.Invoke(
                        this,
                        new EquipmentConstantChangedEventArgs(
                            new List<EquipmentConstant>() { equipmentConstant }));
                });
        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            _logger.Debug($"Callback {nameof(SetECValues)} has been called by PM");

            Task.Run(
                () =>
                {
                    EquipmentConstantChanged?.Invoke(
                        this,
                        new EquipmentConstantChangedEventArgs(equipmentConstants));
                });
        }

        #endregion IEquipmentConstantServiceCB

        #region IStatusVariableService

        public Response<List<StatusVariable>> SVGetAllRequest()
        {
            _logger.Debug($"Service {nameof(SVGetAllRequest)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.SVGetAllRequest());
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> id)
        {
            _logger.Debug($"Service {nameof(SVGetRequest)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.SVGetRequest(id));
        }

        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            _logger.Debug($"Service {nameof(RequestAllFDCsUpdate)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.RequestAllFDCsUpdate());
        }

        #endregion IStatusVariableService

        #region IStatusVariableServiceCB

        public void SVSetMessage(List<StatusVariable> statusVariables)
        {
            _logger.Debug($"Callback {nameof(SVSetMessage)} has been called by PM");

            Task.Run(
                () => StatusVariableChanged?.Invoke(
                    this,
                    new StatusVariableChangedEventArgs(statusVariables)));
        }

        public void SVSetMessage(StatusVariable statusVariable)
        {
            _logger.Debug($"Callback {nameof(SVSetMessage)} has been called by PM");

            Task.Run(
                () => StatusVariableChanged?.Invoke(
                    this,
                    new StatusVariableChangedEventArgs(
                        new List<StatusVariable>() { statusVariable })));
        }

        #endregion IStatusVariableServiceCB

        #region IMaterialService

        public Response<bool> Initialization()
        {
            _logger.Debug($"Service {nameof(Initialization)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.Initialization());
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(Initialization)}: " + ex);
                return new Response<bool>();
            }
        }

        public Response<bool> PrepareForTransfer(TransferType transferType, MaterialTypeInfo materialTypeInfo)
        {
            _logger.Debug($"Service {nameof(PrepareForTransfer)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.PrepareForTransfer(transferType, materialTypeInfo));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(PrepareForTransfer)}: " + ex);
                return new Response<bool>();
            }
        }

        public Response<Material> UnloadMaterial()
        {
            _logger.Debug($"Service {nameof(UnloadMaterial)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.UnloadMaterial());
        }

        public Response<VoidResult> LoadMaterial(Material wafer)
        {
            _logger.Debug($"Service {nameof(LoadMaterial)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.LoadMaterial(wafer));
        }

        public Response<VoidResult> PostTransfer()
        {
            _logger.Debug($"Service {nameof(PostTransfer)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.PostTransfer());
        }

        public Response<VoidResult> StartRecipe()
        {
            _logger.Debug($"Service {nameof(StartRecipe)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.StartRecipe());
        }

        public Response<VoidResult> AbortRecipe()
        {
            _logger.Debug($"Service {nameof(AbortRecipe)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.AbortRecipe());
        }

        public Response<double> GetAlignmentAngle()
        {
            _logger.Debug($"Service {nameof(GetAlignmentAngle)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.GetAlignmentAngle());
        }

        public Response<List<Length>> GetSupportedWaferDimensions()
        {
            _logger.Debug($"Service {nameof(GetSupportedWaferDimensions)} has been called");

            return _serviceInvoker.InvokeAndGetMessages(s => s.GetSupportedWaferDimensions());
        }

        #endregion IMaterialService

        public int GetNbClientsConnected()
        {
            _logger.Debug($"Service {nameof(GetNbClientsConnected)} has been called");

            return 1;
        }

        #endregion IUTOPMService

        #region IUTOPMServiceCB

        public void PMReadyToTransfer()
        {
            _logger.Debug($"Callback {nameof(PMReadyToTransfer)} has been called by PM");

            Task.Run(() => PmReadyToTransfer?.Invoke(this, EventArgs.Empty));
        }

        public bool AskAreYouThere()
        {
            return true;
        }

        #endregion IUTOPMServiceCB
    }
}
