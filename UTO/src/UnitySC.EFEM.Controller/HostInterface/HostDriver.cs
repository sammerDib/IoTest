using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Communication.TCPIP;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Commands;
using UnitySC.EFEM.Controller.HostInterface.Configuration;
using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Material;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;
using LightState = Agileo.SemiDefinitions.LightState;
using OperationMode = UnitySC.Equipment.Abstractions.Devices.Efem.Enums.OperationMode;

namespace UnitySC.EFEM.Controller.HostInterface
{
    /// <summary>
    /// Class responsible to drive the communication with Host controller.
    /// </summary>
    /// <remarks>
    /// Creates and configures the communication pipe (Tcp/Ip).
    /// Delegates behavior to specific <see cref="BaseCommand"/> instances.
    /// </remarks>
    public class HostDriver : Notifier, IEquipmentFacade, ICommunication, IDisposable
    {
        #region Fields

        private Dictionary<int, bool> _hoAvlbFallOnLoadPorts = new();

        private readonly TCPPostman _tcpPostman;

        private readonly IMacroCommandSubscriber _initializeCommandListener;

        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _readCarrierIdByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _dockCommandListenersByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _undockCommandListenersByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _getWaferSizeByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _enableOrDisableE84ByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _performWaferMappingByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _getMappingPatternByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _closeDoorByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _clampByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _setLightByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _getE84InputsByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _getE84OutputsByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _resetE84ByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _abortE84ByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _setCarrierTypeByLp;
        private readonly Dictionary<Constants.Port, IMacroCommandSubscriber> _getCarrierTypeByLp;

        private readonly IMacroCommandSubscriber _homeCommandListener;
        private readonly IMacroCommandSubscriber _pickCommandListener;
        private readonly IMacroCommandSubscriber _armClampUnclampCommandListener;
        private readonly IMacroCommandSubscriber _placeCommandListener;
        private readonly IMacroCommandSubscriber _preparePickCommandListener;
        private readonly IMacroCommandSubscriber _preparePlaceCommandListener;
        private readonly IMacroCommandSubscriber _setRobotSpeedListener;
        private readonly IMacroCommandSubscriber _getWaferOnArmListener;

        private readonly IMacroCommandSubscriber _alignCommandListener;
        private readonly IMacroCommandSubscriber _setAlignerChuckStateCommandListener;
        private readonly IMacroCommandSubscriber _getOcrRecipesCommandListener;
        private readonly IMacroCommandSubscriber _readWaferIdCommandListener;
        private readonly IMacroCommandSubscriber _readWaferIdOnlyCommandListener;
        private readonly IMacroCommandSubscriber _centeringCommandListener;

        private readonly IMacroCommandSubscriber _getPressureCommandListener;
        private readonly IMacroCommandSubscriber _setLightTowerStateCommandListener;
        private readonly IMacroCommandSubscriber _getGeneralStatusesCommandListener;
        private readonly IMacroCommandSubscriber _setBuzzerCommandCommandListener;
        private readonly IMacroCommandSubscriber _setTimeCommandCommandListener;
        private readonly IMacroCommandSubscriber _setFfuRpmCommandListener;
        private readonly IMacroCommandSubscriber _getFfuRpmCommandListener;
        private readonly IMacroCommandSubscriber _setE84TimeoutsCommandListener;

        private readonly IMacroCommandSubscriber _eventSender;

        private readonly EfemEquipmentManager _equipmentManager;

        #endregion Fields

        #region Constructor

        public HostDriver()
        {
            Logger = Agileo.Common.Logging.Logger.GetLogger(nameof(HostDriver));

            Logger.Debug("Creation of Tcp Postman...");

            _equipmentManager = App.EfemAppInstance.EfemEquipmentManager;

            foreach (var loadPort in _equipmentManager.LoadPorts.Values)
            {
                IsE84EnabledOnLoadPorts.Add(loadPort.InstanceId, false);
                _hoAvlbFallOnLoadPorts.Add(loadPort.InstanceId, false);
            }

            _tcpPostman = new TCPPostman(this, false);
            _tcpPostman.CommunicationIsEstablished += TcpPostman_CommunicationIsEstablished;
            _tcpPostman.CommunicationClosed        += TcpPostman_CommunicationClosed;

            // as postman is a base class defined in utilities we have to pass proper tracer
            _tcpPostman.SetTracer(Logger.Name);

            Logger.Debug("Tcp Postman created.");

            // Create subscribers for all commands that can be received from remote controller
            _initializeCommandListener = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            var loadPortIds             = _equipmentManager.LoadPorts.Keys;
            _readCarrierIdByLp          = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _dockCommandListenersByLp   = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _undockCommandListenersByLp = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _getWaferSizeByLp           = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _enableOrDisableE84ByLp     = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _performWaferMappingByLp    = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _getMappingPatternByLp      = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _closeDoorByLp              = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _clampByLp                  = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _setLightByLp               = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _getE84InputsByLp           = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _getE84OutputsByLp          = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _resetE84ByLp = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _abortE84ByLp = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _setCarrierTypeByLp = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);
            _getCarrierTypeByLp = new Dictionary<Constants.Port, IMacroCommandSubscriber>(loadPortIds.Count);

            foreach (var port in loadPortIds)
            {
                _readCarrierIdByLp         .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _dockCommandListenersByLp  .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _undockCommandListenersByLp.Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _getWaferSizeByLp          .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _enableOrDisableE84ByLp    .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _performWaferMappingByLp   .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _getMappingPatternByLp     .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _closeDoorByLp             .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _clampByLp                 .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _setLightByLp              .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _getE84InputsByLp          .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _getE84OutputsByLp         .Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _resetE84ByLp.Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _abortE84ByLp.Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _setCarrierTypeByLp.Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _getCarrierTypeByLp.Add(Constants.ToPort(port), _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
            }

            _homeCommandListener            = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _pickCommandListener            = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _armClampUnclampCommandListener = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _placeCommandListener           = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _preparePickCommandListener     = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _preparePlaceCommandListener    = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setRobotSpeedListener          = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _getWaferOnArmListener          = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            _alignCommandListener                = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setAlignerChuckStateCommandListener = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _getOcrRecipesCommandListener        = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _readWaferIdCommandListener          = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _readWaferIdOnlyCommandListener      = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _centeringCommandListener            = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            _getPressureCommandListener        = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setLightTowerStateCommandListener = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _getGeneralStatusesCommandListener = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setBuzzerCommandCommandListener   = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setTimeCommandCommandListener     = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setFfuRpmCommandListener          = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _getFfuRpmCommandListener          = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _setE84TimeoutsCommandListener     = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            _eventSender = _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener);

            SubscribeToEquipmentEvents();
        }

        #endregion Constructor

        #region Properties

        private ILogger Logger { get; }

        public Dictionary<int, bool> IsE84EnabledOnLoadPorts { get; set; } = new();

        public bool IsRobotSequenceForOcrReadingInProgress { get; set; }

        public object LockRobotCommand { get; } = new();

        #endregion Properties

        #region Methods

        public void Setup(HostConfiguration configuration)
        {
            _tcpPostman.EndReplyIndicator                    = Constants.EndOfFrame;
            _tcpPostman.StopDecodingOnFirstEndReplyIndicator = true;
            _tcpPostman.CommandPostfix                       = Constants.EndOfFrame;
            _tcpPostman.RemoteIPAddress                      = IPAddress.Parse(configuration.IpAddress);
            _tcpPostman.PortIndex                            = (int)configuration.TcpPort;
        }

        private void SendGeneralStatus()
        {
            if (!IsCommunicationEnabled)
                return;

            var efem = _equipmentManager.Efem;
            var status = new GeneralStatus(
                efem.OperationMode,
                efem.RobotStatus, efem.RobotSpeed,
                efem.LoadPortStatus1, efem.IsLoadPort1CarrierPresent,
                efem.LoadPortStatus2, efem.IsLoadPort2CarrierPresent,
                efem.LoadPortStatus3, efem.IsLoadPort3CarrierPresent,
                efem.LoadPortStatus4, efem.IsLoadPort4CarrierPresent,
                efem.AlignerStatus, efem.IsAlignerCarrierPresent,
                efem.SafetyDoorSensor,
                efem.VacuumSensor,
                efem.AirSensor);
            var generalStatusEvt = GetGeneralStatusesCommand.NewEvent(status, _tcpPostman, this, Logger, _equipmentManager);
            _eventSender.AddMacro(generalStatusEvt);
        }

        private void SendSystemInputs()
        {
            if (!IsCommunicationEnabled)
                return;

            var efem = _equipmentManager.Efem;
            var status = new SystemStatus(
                efem.OperationMode == OperationMode.Remote,
                efem.FfuAlarm,
                efem.VacuumSensor, efem.AirSensor,
                efem.IonizerAirState, efem.IonizerAlarm,
                efem.SafetyDoorSensor, efem.LightCurtainBeam,
                efem.Interlock);
            var systemStatusEvent = SystemInputEvent.NewEvent(status, _tcpPostman, this, Logger, _equipmentManager);
            _eventSender.AddMacro(systemStatusEvent);
        }

        private void SendE84InputSignalsEvent(E84InputsStatus status)
        {
            if (!IsCommunicationEnabled)
                return;

            var e84InputSignals = GetE84InputSignalsCommand.NewEvent(status, _tcpPostman, this, Logger, _equipmentManager);
            _eventSender.AddMacro(e84InputSignals);
        }

        private void SendE84OutputSignalsEvent(E84OutputsStatus status)
        {
            if (!IsCommunicationEnabled)
                return;

            var e84OutputSignals = GetE84OutputSignalsCommand.NewEvent(status, _tcpPostman, this, Logger, _equipmentManager);
            _eventSender.AddMacro(e84OutputSignals);
        }

        private void SendE84ErrorEvent(Constants.Port loadPortId, Error error)
        {
            if (!IsCommunicationEnabled)
                return;

            var evt = GetE84ErrorCommand.NewEvent(loadPortId, error, _tcpPostman, this, Logger, _equipmentManager);
            _eventSender.AddMacro(evt);
        }

        #endregion Methods

        #region Event Handlers

        private void TcpPostman_CommunicationIsEstablished(object sender, EventArgs e)
        {
            try
            {
                // Security in case of component in phase of clean-up
                if (_tcpPostman == null) { return; }

                OnCommunicationEstablished();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void TcpPostman_CommunicationClosed(object sender, EventArgs e)
        {
            try
            {
                // Security in case of component in phase of clean-up
                if (_tcpPostman == null) { return; }

                DisableListeners(); // TODO Check if disabling listener here is ok: When client disconnect and reconnect, listeners should still be active!!

                OnCommunicationClosed();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void Efem_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                switch (e.Status.Name)
                {
                    case nameof(IEfem.RobotStatus):
                    case nameof(IEfem.RobotSpeed):
                    case nameof(IEfem.LoadPortStatus1):
                    case nameof(IEfem.IsLoadPort1CarrierPresent):
                    case nameof(IEfem.LoadPortStatus2):
                    case nameof(IEfem.IsLoadPort2CarrierPresent):
                    case nameof(IEfem.LoadPortStatus3):
                    case nameof(IEfem.IsLoadPort3CarrierPresent):
                    case nameof(IEfem.LoadPortStatus4):
                    case nameof(IEfem.IsLoadPort4CarrierPresent):
                    case nameof(IEfem.AlignerStatus):
                    case nameof(IEfem.IsAlignerCarrierPresent):
                        SendGeneralStatus();
                        break;

                    case nameof(IEfem.OperationMode):
                    case nameof(IEfem.SafetyDoorSensor):
                    case nameof(IEfem.VacuumSensor):
                    case nameof(IEfem.AirSensor):
                        SendGeneralStatus();
                        SendSystemInputs();
                        break;

                    case nameof(IEfem.FfuAlarm):
                    case nameof(IEfem.IonizerAirState):
                    case nameof(IEfem.IonizerAlarm):
                    case nameof(IEfem.LightCurtainBeam):
                    case nameof(IEfem.Interlock):
                        SendSystemInputs();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occurred during general statuses event sending.");
            }
        }

        private void LoadPort_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (!IsCommunicationEnabled)
                return;

            try
            {
                if (sender is not LoadPort loadPort)
                {
                    return;
                }

                // Treat command only when done
                if (e.NewState is not ExecutionState.Success and not ExecutionState.Failed)
                {
                    return;
                }

                switch (e.Execution.Context.Command.Name)
                {
                    case nameof(ILoadPort.Open):
                    case nameof(ILoadPort.Unclamp):
                        // To imitate the old RORZE EFEM Controller, we have to send information about carrier to HOST
                        var evt = GetCarrierCapacityAndSizeCommand.NewEvent(
                            Constants.ToPort(loadPort.InstanceId),
                            loadPort.Carrier.Capacity,
                            loadPort.Carrier.SampleSize,
                            _tcpPostman,
                            this,
                            Logger,
                            _equipmentManager);
                        _eventSender.AddMacro(evt);
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (!IsCommunicationEnabled || App.EfemAppInstance.ControlState == ControlState.Local)
            {
                if (loadPort.Configuration.HandOffType == HandOffType.Button
                    && e.Status.Name == nameof(ILoadPort.IsHandOffButtonPressed)
                    && loadPort.IsHandOffButtonPressed
                    && loadPort.HandOffLightState == LightState.On)
                {
                    loadPort.ClampAsync();
                }
                return;
            }

            try
            {
                switch (e.Status.Name)
                {
                    case nameof(ILoadPort.CarrierPresence):
                    case nameof(ILoadPort.IsHandOffButtonPressed):
                    {
                        var evtToHost = CarrierPresenceCommand.NewEvent(
                            Constants.ToPort(loadPort.InstanceId),
                            loadPort.CarrierPresence,
                            loadPort.IsHandOffButtonPressed,
                            _tcpPostman,
                            this,
                            Logger,
                            _equipmentManager);
                        _eventSender.AddMacro(evtToHost);
                        break;
                    }

                    case nameof(ILoadPort.I_VALID):
                    case nameof(ILoadPort.I_CS_0):
                    case nameof(ILoadPort.I_CS_1):
                    case nameof(ILoadPort.I_TR_REQ):
                    case nameof(ILoadPort.I_BUSY):
                    case nameof(ILoadPort.I_COMPT):
                    case nameof(ILoadPort.I_CONT):
                        var inputStatus = new E84InputsStatus(
                            Constants.ToPort(loadPort.InstanceId),
                            loadPort.I_VALID,
                            loadPort.I_CS_0,
                            loadPort.I_CS_1,
                            loadPort.I_TR_REQ,
                            loadPort.I_BUSY,
                            loadPort.I_COMPT,
                            loadPort.I_CONT);
                        SendE84InputSignalsEvent(inputStatus);
                        break;

                    case nameof(ILoadPort.O_L_REQ):
                    case nameof(ILoadPort.O_U_REQ):
                    case nameof(ILoadPort.O_READY):
                    case nameof(ILoadPort.O_HO_AVBL):
                    case nameof(ILoadPort.O_ES):
                        var outputStatus = new E84OutputsStatus(
                            Constants.ToPort(loadPort.InstanceId),
                            loadPort.O_L_REQ,
                            loadPort.O_U_REQ,
                            loadPort.O_READY,
                            loadPort.O_HO_AVBL,
                            loadPort.O_ES);
                        SendE84OutputSignalsEvent(outputStatus);

                        if (e.Status.Name == nameof(ILoadPort.O_HO_AVBL)
                            && IsE84EnabledOnLoadPorts[loadPort.InstanceId]
                            && !loadPort.O_HO_AVBL)
                        {
                            _hoAvlbFallOnLoadPorts[loadPort.InstanceId] = true;
                        }
                        break;

                    case nameof(IRV201.OperationStatus):
                        if (loadPort is RV201 rv201lp
                            && rv201lp.OperationStatus == OperationStatus.Stop
                            && rv201lp.ErrorCode == "00"
                            && IsE84EnabledOnLoadPorts[loadPort.InstanceId]
                            && _hoAvlbFallOnLoadPorts[loadPort.InstanceId])
                        {
                            _hoAvlbFallOnLoadPorts[loadPort.InstanceId] = false;
                            Task.Factory.StartNew(
                                () =>
                                {
                                    if (loadPort.CarrierPresence == CassettePresence.Absent)
                                    {
                                        loadPort.RequestLoad();
                                    }
                                    else
                                    {
                                        ForceCarrierPresenceInCaseOfError(loadPort);
                                    }
                                });
                        }
                        break;
                    case nameof(ILoadPort.CarrierTypeNumber):
                        if (loadPort.Configuration.IsManualCarrierType)
                        {
                            var carrierTypeEvent = GetCarrierTypeCommand.NewEvent(
                                Constants.ToPort(loadPort.InstanceId),
                                loadPort.CarrierTypeNumber,
                                _tcpPostman,
                                this,
                                Logger,
                                _equipmentManager);
                            _eventSender.AddMacro(carrierTypeEvent);
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "An error occurred while sending carrier presence data to the HOST.");
            }
        }

        private void LoadPort_E84ErrorOccurred(object sender, E84ErrorOccurredEventArgs e)
        {
            if (!IsCommunicationEnabled)
                return;

            try
            {
                if (sender is not LoadPort loadPort)
                {
                    return;
                }

                Error error;
                switch (e.Error)
                {
                    case E84Errors.Tp1Timeout:
                        error = Constants.Errors[ErrorCode.E84Tp1Timeout];
                        break;

                    case E84Errors.Tp2Timeout:
                        error = Constants.Errors[ErrorCode.E84Tp2Timeout];
                        break;

                    case E84Errors.Tp3Timeout:
                        error = Constants.Errors[ErrorCode.E84Tp3Timeout];
                        break;

                    case E84Errors.Tp4Timeout:
                        error = Constants.Errors[ErrorCode.E84Tp4Timeout];
                        break;

                    case E84Errors.Tp5Timeout:
                        error = Constants.Errors[ErrorCode.E84Tp5Timeout];
                        break;

                    case E84Errors.SignalError:
                        error = Constants.Errors[ErrorCode.E84SignalError];
                        break;

                    default:
                        return;
                }

                if (error != null)
                {
                    SendE84ErrorEvent(Constants.ToPort(loadPort.InstanceId), error);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "An error occurred while sending E84 error to the HOST.");
            }
        }

        private void LoadPort_CarrierPlaced(object sender, CarrierEventArgs e)
        {
            if (!IsCommunicationEnabled)
                return;

            try
            {
                if (sender is not LoadPort loadPort)
                {
                    return;
                }

                var carrier = e?.Carrier;
                if (carrier == null)
                {
                    return;
                }

                carrier.SlotMapChanged += Carrier_SlotMapChanged;

                // Notify new carrier capacity and size
                var evt = GetCarrierCapacityAndSizeCommand.NewEvent(
                    Constants.ToPort(loadPort.InstanceId),
                    carrier.Capacity,
                    carrier.SampleSize,
                    _tcpPostman,
                    this,
                    Logger,
                    _equipmentManager);
                _eventSender.AddMacro(evt);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void LoadPort_CarrierRemoved(object sender, CarrierEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            var carrier = e?.Carrier;
            if (carrier != null)
            {
                carrier.SlotMapChanged -= Carrier_SlotMapChanged;
            }
        }

        private void Carrier_SlotMapChanged(object sender, SlotMapEventArgs e)
        {
            if (!IsCommunicationEnabled || sender is not Carrier carrier)
                return;

            try
            {
                // Create event
                var cmd = GetMappingPatternCommand.NewEvent(
                    Constants.ToPort((carrier.Location.Container as LoadPort)?.InstanceId ?? 0),
                    e.SlotMap,
                    _tcpPostman,
                    this,
                    Logger,
                    _equipmentManager);

                // Send event
                _eventSender.AddMacro(cmd);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "An error occurred while sending mapping data to the HOST.");
            }
        }

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (!IsCommunicationEnabled || sender is not Robot robot)
                return;

            try
            {
                var commandName = e.Execution.Context.Command.Name;

                if (e.NewState == ExecutionState.Success
                    && (commandName.Equals(nameof(IRobot.Pick)) || commandName.Equals(nameof(IRobot.Place))))
                {
                    var robotArm = (RobotArm)e.Execution.Context.GetArgument("arm");
                    var waferArmEvt = GetWaferPresenceOnArmCommand.NewEvent(
                        RobotArmConverter.ToRobotArm(robotArm),
                        robot.UpperArmHistory,
                        robot.LowerArmHistory,
                        _tcpPostman,
                        this,
                        Logger,
                        _equipmentManager);

                    _eventSender.AddMacro(waferArmEvt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while sending wafer on arm presence data to the HOST.");
            }
        }

        #endregion Event Handlers

        #region Subscriber Handling

        public void SubscribeToEquipmentEvents()
        {
            _equipmentManager.Efem.StatusValueChanged            += Efem_StatusValueChanged;
            _equipmentManager.Robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;

            foreach (var loadPort in _equipmentManager.LoadPorts.Values)
            {
                loadPort.StatusValueChanged += LoadPort_StatusValueChanged;
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
                loadPort.CarrierPlaced += LoadPort_CarrierPlaced;
                loadPort.CarrierRemoved += LoadPort_CarrierRemoved;
                loadPort.E84ErrorOccurred += LoadPort_E84ErrorOccurred;
            }
        }

        public void UnsubscribeToEquipmentEvents()
        {
            _equipmentManager.Efem.StatusValueChanged            -= Efem_StatusValueChanged;
            _equipmentManager.Robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;

            foreach (var loadPort in _equipmentManager.LoadPorts.Values)
            {
                loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
                loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                loadPort.CarrierPlaced -= LoadPort_CarrierPlaced;
                loadPort.CarrierRemoved -= LoadPort_CarrierRemoved;
                loadPort.E84ErrorOccurred -= LoadPort_E84ErrorOccurred;
            }
        }

        #endregion Subscriber Handling

        #region Internal Methods

        internal void ForceCarrierPresenceInCaseOfError(LoadPort loadPort)
        {
            var fakeEvtToHost = CarrierPresenceCommand.NewEvent(
                Constants.ToPort(loadPort.InstanceId),
                CassettePresence.Correctly,
                loadPort.IsHandOffButtonPressed,
                _tcpPostman,
                this,
                Logger,
                _equipmentManager);
            _eventSender.AddMacro(fakeEvtToHost);

            var realEvtToHost = CarrierPresenceCommand.NewEvent(
                Constants.ToPort(loadPort.InstanceId),
                loadPort.CarrierPresence,
                loadPort.IsHandOffButtonPressed,
                _tcpPostman,
                this,
                Logger,
                _equipmentManager);
            _eventSender.AddMacro(realEvtToHost);
        }

        #endregion

        #region IEquipmentFacade

        public void RegisterAlarmist(string baseAlarmSource, int baseALID)
        {
            Logger.Warning("/!\\ Not fully managed - RegisterAlarmist: "
                           + $"src={baseAlarmSource} ; "
                           + $"baseALID={baseALID}");
        }

        public void SendCommunicationLogEvent(int communicatorId, bool isOut, string message, DateTime dateTime)
        {
            OnMessageExchanged(new MessageExchangedEventArgs(isOut, message, dateTime));
        }

        public void SendEquipmentEvent(int eventId, EventArgs args)
        {
            Logger.Warning("/!\\ Not fully managed - SendEquipmentEvent: "
                           + $"eventId={eventId} ; "
                           + $"args={args}");
        }

        public void SendEquipmentAlarm(
            byte deviceNumber,
            bool isGetErrorStatus,
            string alarmKey,
            params object[] substitutionParam)
        {
            Logger.Warning(
                "/!\\ Not fully managed - SendEquipmentAlarm: "
                + $"deviceNumber={deviceNumber} ; "
                + $"isGetErrorStatus={isGetErrorStatus} ; "
                + $"alarmKey={alarmKey} ; "
                + $"substitutionParam={substitutionParam}");
        }

        #endregion IEquipmentFacade

        #region ICommunication

        public void ClearCommandsQueue()
        {
            // Probably nothing to do since we aren't sending commands (just acknowledge and event to remote controller)
        }

        public bool EnableCommunications()
        {
            if (_tcpPostman == null) { return false; }

            if (_tcpPostman.IsConnected) { return true; }

            EnableListeners(); // Should be done before connecting TCP, so we can receive commands as soon as connection is established
            _tcpPostman.Connect();
            return false; // Client connection should be awaited first
        }

        public void Disconnect()
        {
            // Caution: allow to disconnect (stop) server even if not connected to anyone
            if (_tcpPostman == null) { return; }

            _tcpPostman.Disconnect();
            if (_tcpPostman.IsConnected)
            {
                Logger.Error("Failed to disconnect TCP communication.");
            }
        }

        /// <summary>
        /// Indicates if communication is already established (<see langword="true"/>) or not (<see langword="false"/>).
        /// </summary>
        public bool IsCommunicationEnabled { get { return _tcpPostman?.IsConnected ?? false; } }

        public event EventHandler CommunicationEstablished;

        public event EventHandler CommunicationClosed;

        public event EventHandler<MessageExchangedEventArgs> MessageExchanged;

        /// <summary>
        /// Sends the <see cref="MessageExchanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="MessageExchangedEventArgs"/> to be attached with the event.</param>
        protected virtual void OnMessageExchanged(MessageExchangedEventArgs args)
        {
            try
            {
                var messageDirectionRepresentation = args.IsOut ? "[SENT]" : "[RECV]";
                var text = $"{messageDirectionRepresentation} {args.Message.Replace("\r", "")}";
                TraceManager.Instance().Trace("Com - " + $"HOST", TraceLevelType.Info, text);

                if (MessageExchanged != null)
                {
                    MessageExchanged(this, args);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occurred during event sending.");
            }
        }

        /// <summary>
        /// Sends the <see cref="CommunicationClosed"/> event.
        /// </summary>
        private void OnCommunicationClosed()
        {
            try
            {
                OnPropertyChanged(nameof(IsCommunicationEnabled));
                CommunicationClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occurred during event sending.");
            }
        }

        /// <summary>
        /// Sends the <see cref="CommunicationEstablished"/> event.
        /// </summary>
        private void OnCommunicationEstablished()
        {
            try
            {
                OnPropertyChanged(nameof(IsCommunicationEnabled));
                CommunicationEstablished?.Invoke(this, EventArgs.Empty);
                SendGeneralStatus();
                SendSystemInputs();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occurred during event sending.");
            }
        }

        #endregion ICommunication

        #region Communication

        private void EnableListeners()
        {
            Logger.Debug("Listeners are Enabling");

            _initializeCommandListener.AddMacro(new InitializeCommand(_tcpPostman, this, Logger, _equipmentManager));

            foreach (var lp in _dockCommandListenersByLp.Keys)
            {
                _readCarrierIdByLp[lp]         .AddMacro(new ReadCarrierIdCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _dockCommandListenersByLp[lp]  .AddMacro(new DockCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _undockCommandListenersByLp[lp].AddMacro(new UndockCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _getWaferSizeByLp[lp]          .AddMacro(new GetWaferSizeInLoadPortCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _enableOrDisableE84ByLp[lp]    .AddMacro(new EnableOrDisableE84Command(lp, _tcpPostman, this, Logger, _equipmentManager));
                _performWaferMappingByLp[lp]   .AddMacro(new PerformWaferMappingCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _getMappingPatternByLp[lp]     .AddMacro(new GetMappingPatternCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _closeDoorByLp[lp]             .AddMacro(new CloseDoorCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _clampByLp[lp]                 .AddMacro(new ClampOnLoadPortCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _setLightByLp[lp]              .AddMacro(new SetLpLightCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _getE84InputsByLp[lp]          .AddMacro(new GetE84InputSignalsCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _getE84OutputsByLp[lp]         .AddMacro(new GetE84OutputSignalsCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _resetE84ByLp[lp].AddMacro(new ResetE84Command(lp, _tcpPostman, this, Logger, _equipmentManager));
                _abortE84ByLp[lp].AddMacro(new AbortE84Command(lp, _tcpPostman, this, Logger, _equipmentManager));
                _setCarrierTypeByLp[lp].AddMacro(new SetCarrierTypeCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
                _getCarrierTypeByLp[lp].AddMacro(new GetCarrierTypeCommand(lp, _tcpPostman, this, Logger, _equipmentManager));
            }

            _homeCommandListener           .AddMacro(new HomeCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _pickCommandListener           .AddMacro(new PickCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _armClampUnclampCommandListener.AddMacro(new ClampOnArmCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _placeCommandListener          .AddMacro(new PlaceCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _preparePickCommandListener    .AddMacro(new PreparePickCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _preparePlaceCommandListener   .AddMacro(new PreparePlaceCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _setRobotSpeedListener         .AddMacro(new SetRobotSpeedCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _getWaferOnArmListener         .AddMacro(GetWaferPresenceOnArmCommand.NewOrder(_tcpPostman, this, Logger, _equipmentManager));

            _alignCommandListener               .AddMacro(new AlignCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setAlignerChuckStateCommandListener.AddMacro(new SetAlignerChuckStateCommand(_tcpPostman, this, Logger, _equipmentManager));
            _getOcrRecipesCommandListener       .AddMacro(new GetOcrRecipesCommand(_tcpPostman, this, Logger, _equipmentManager));
            _readWaferIdCommandListener         .AddMacro(new ReadWaferIdCommand(_tcpPostman, this, Logger, _equipmentManager, this));
            _readWaferIdOnlyCommandListener     .AddMacro(new ReadWaferIdOnlyCommand(_tcpPostman, this, Logger, _equipmentManager));
            _centeringCommandListener           .AddMacro(new CenteringCommand(_tcpPostman, this, Logger, _equipmentManager));

            _getPressureCommandListener       .AddMacro(new GetPressureCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setLightTowerStateCommandListener.AddMacro(new SetLightTowerStateCommand(_tcpPostman, this, Logger, _equipmentManager));
            _getGeneralStatusesCommandListener.AddMacro(new GetGeneralStatusesCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setBuzzerCommandCommandListener  .AddMacro(new SetBuzzerCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setTimeCommandCommandListener    .AddMacro(new SetTimeCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setFfuRpmCommandListener         .AddMacro(new SetFfuRpmCommand(_tcpPostman, this, Logger, _equipmentManager));
            _getFfuRpmCommandListener         .AddMacro(new GetFfuRpmCommand(_tcpPostman, this, Logger, _equipmentManager));
            _setE84TimeoutsCommandListener    .AddMacro(new SetE84TimeoutsCommand(_tcpPostman, this, Logger, _equipmentManager));

            Logger.Debug("Listeners are Enabled");
        }

        private void DisableListeners()
        {
            Logger.Debug("Listeners are Disabling");

            _tcpPostman.DiscardOpenTransactions(_initializeCommandListener);

            foreach (var lp in _dockCommandListenersByLp.Keys)
            {
                _tcpPostman.DiscardOpenTransactions(_readCarrierIdByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_dockCommandListenersByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_undockCommandListenersByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_getWaferSizeByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_enableOrDisableE84ByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_performWaferMappingByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_getMappingPatternByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_closeDoorByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_clampByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_setLightByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_getE84InputsByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_getE84OutputsByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_resetE84ByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_abortE84ByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_setCarrierTypeByLp[lp]);
                _tcpPostman.DiscardOpenTransactions(_getCarrierTypeByLp[lp]);
            }

            _tcpPostman.DiscardOpenTransactions(_homeCommandListener);
            _tcpPostman.DiscardOpenTransactions(_pickCommandListener);
            _tcpPostman.DiscardOpenTransactions(_armClampUnclampCommandListener);
            _tcpPostman.DiscardOpenTransactions(_placeCommandListener);
            _tcpPostman.DiscardOpenTransactions(_preparePickCommandListener);
            _tcpPostman.DiscardOpenTransactions(_preparePlaceCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setRobotSpeedListener);
            _tcpPostman.DiscardOpenTransactions(_getWaferOnArmListener);

            _tcpPostman.DiscardOpenTransactions(_alignCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setAlignerChuckStateCommandListener);
            _tcpPostman.DiscardOpenTransactions(_getOcrRecipesCommandListener);
            _tcpPostman.DiscardOpenTransactions(_readWaferIdCommandListener);
            _tcpPostman.DiscardOpenTransactions(_readWaferIdOnlyCommandListener);
            _tcpPostman.DiscardOpenTransactions(_centeringCommandListener);

            _tcpPostman.DiscardOpenTransactions(_getPressureCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setLightTowerStateCommandListener);
            _tcpPostman.DiscardOpenTransactions(_getGeneralStatusesCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setBuzzerCommandCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setTimeCommandCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setFfuRpmCommandListener);
            _tcpPostman.DiscardOpenTransactions(_getFfuRpmCommandListener);
            _tcpPostman.DiscardOpenTransactions(_setE84TimeoutsCommandListener);

            Logger.Debug("Listeners are Disabled");
        }

        #endregion Communication

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            UnsubscribeToEquipmentEvents();

            DisableListeners();

            _tcpPostman.CommunicationIsEstablished -= TcpPostman_CommunicationIsEstablished;
            _tcpPostman.CommunicationClosed        -= TcpPostman_CommunicationClosed;

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
