using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

using Agileo.Common.Communication;
using Agileo.Common.Communication.TCPIP;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.EquipmentController.Simulator.Driver.Commands;
using UnitySC.EquipmentController.Simulator.Driver.EventArgs;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;
using UnitySC.EquipmentController.Simulator.EquipmentData;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;

using LightState = UnitySC.EFEM.Controller.HostInterface.Enums.LightState;

namespace UnitySC.EquipmentController.Simulator.Driver
{
    internal class EfemDriver : IEquipmentFacade, IDeviceDriver
    {
        #region Fields

        #region DeviceCommandSubscriber
        private readonly IMacroCommandSubscriber _commandSubscriber;
        private readonly IMacroCommandSubscriber _efemCommandSubscriber;
        private readonly IMacroCommandSubscriber _robotCommandSubscriber;
        private readonly IMacroCommandSubscriber _alignerCommandSubscriber;
        private readonly SortedList<Constants.Port, IMacroCommandSubscriber> _commandSubscribersByLp;
        #endregion
        
        private readonly IMacroCommandSubscriber _generalStatusEventSubscriber;
        private readonly IMacroCommandSubscriber _systemStatusEventSubscriber;
        private readonly SortedList<Constants.Port, IMacroCommandSubscriber> _carrierPlacementEventSubscribers; // One per load port
        private readonly SortedList<Constants.Port, IMacroCommandSubscriber> _mappingPatternEventSubscribers; // One per load port
        private readonly SortedList<Constants.Port, IMacroCommandSubscriber> _carrierCapacityAndSizeEventSubscribers; // One per load port
        private readonly SortedList<Constants.Port, IMacroCommandSubscriber> _e84ErrorEventSubscribers; // One per load port
        private readonly IMacroCommandSubscriber _armHistoryAndWaferPresenceEventSubscriber;
        private readonly TCPPostman _tcpPostman;

        private readonly EfemData _efemData;

        #endregion Fields

        #region Constructor

        public EfemDriver(string ipAddress, int port, TimeSpan connectionTimeout, EfemData efemData)
        {
            _efemData = efemData;
            Logger = Agileo.Common.Logging.Logger.GetLogger(nameof(EfemDriver));

            Logger.Info("Start creation of EFEM facade...");

            // Drives first and only one EFEM
            Category = "EFEM";
            Port = 1;

            Logger.Info("Creation of Tcp Postman...");

            _tcpPostman = new TCPPostman(this, true)
            {
                RemoteIPAddress                      = IPAddress.Parse(ipAddress),
                PortIndex                            = port,
                ConnectionTimeout                    = (int)connectionTimeout.TotalMilliseconds,
                EndReplyIndicator                    = Constants.EndOfFrame,
                StopDecodingOnFirstEndReplyIndicator = true,
                CommandPostfix                       = Constants.EndOfFrame
            };
            _tcpPostman.CommunicationIsEstablished += TcpPostman_CommunicationIsEstablished;
            _tcpPostman.CommunicationClosed        += TcpPostman_CommunicationClosed;

            //as postman is a base class defined in utilities we have to pass proper tracer
            //instead of instantiating them on the place where they are used
            _tcpPostman.SetTracer(Logger.Name);

            _commandSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener);
            _efemCommandSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener);
            _alignerCommandSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener);
            _robotCommandSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener);

            // Subscribe to equipment event
            // Global EFEM events
            _generalStatusEventSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _systemStatusEventSubscriber  = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            // LoadPort events
            // Create one subscriber per event and per device instance
            _carrierPlacementEventSubscribers       = new SortedList<Constants.Port, IMacroCommandSubscriber>();
            _mappingPatternEventSubscribers         = new SortedList<Constants.Port, IMacroCommandSubscriber>();
            _carrierCapacityAndSizeEventSubscribers = new SortedList<Constants.Port, IMacroCommandSubscriber>();
            _e84ErrorEventSubscribers               = new SortedList<Constants.Port, IMacroCommandSubscriber>();
            _commandSubscribersByLp                 = new SortedList<Constants.Port, IMacroCommandSubscriber>();
            foreach (var portAndData in _efemData.LoadPortsData)
            {
                _carrierPlacementEventSubscribers.Add(portAndData.Key,
                    _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _mappingPatternEventSubscribers.Add(portAndData.Key,
                    _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _carrierCapacityAndSizeEventSubscribers.Add(portAndData.Key,
                    _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _e84ErrorEventSubscribers.Add(portAndData.Key,
                    _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage));
                _commandSubscribersByLp.Add(portAndData.Key, 
                    _tcpPostman.AddReplySubscriber(SubscriberType.SenderAndListener));
            }

            // Robot events
            _armHistoryAndWaferPresenceEventSubscriber = _tcpPostman.AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            Logger.Info("Tcp Postman created.");

            Logger.Info("EFEM Facade created.");
        }

        #endregion Constructor

        #region Properties

        public ILogger Logger { get; }

        #endregion Properties

        #region Commands

        #region Global commands

        public void Initialize()
        {
            Logger.Info($"Sending command {nameof(Initialize)}.");

            var command = InitializeCommand.NewOrder(_tcpPostman, this, Logger);
            _efemCommandSubscriber.AddMacro(command);
        }

        public void Initialize(Constants.Unit unit)
        {
            Logger.Info($"Sending command {nameof(Initialize)} to {unit}.");

            var command = InitializeCommand.NewOrder(unit, _tcpPostman, this, Logger);
            switch (unit)
            {
                case Constants.Unit.Aligner:
                    _alignerCommandSubscriber.AddMacro(command);
                    break;
                case Constants.Unit.Robot:
                    _robotCommandSubscriber.AddMacro(command);
                    break;
                case Constants.Unit.LP1:
                    _commandSubscribersByLp[Constants.Port.LP1].AddMacro(command);
                    break;
                case Constants.Unit.LP2:
                    _commandSubscribersByLp[Constants.Port.LP2].AddMacro(command);
                    break;
                case Constants.Unit.LP3:
                    _commandSubscribersByLp[Constants.Port.LP3].AddMacro(command);
                    break;
                case Constants.Unit.LP4:
                    _commandSubscribersByLp[Constants.Port.LP4].AddMacro(command);
                    break;
                default:
                    _efemCommandSubscriber.AddMacro(command);
                    break;
            }
        }

        public void GetPressure()
        {
            Logger.Info($"Sending command {nameof(GetPressure)}.");

            var command = GetPressureCommand.NewOrder(_tcpPostman, this, Logger);
            _efemCommandSubscriber.AddMacro(command);
        }

        public void SetLightTowerState(LightState redLight, LightState orangeLight, LightState greenLight, LightState blueLight)
        {
            Logger.Info($"Sending command {nameof(SetLightTowerState)}.");

            var command = SetLightTowerStateCommand.NewOrder(redLight, orangeLight, greenLight, blueLight, _tcpPostman, this, Logger);
            _efemCommandSubscriber.AddMacro(command);
        }

        public void GetStat()
        {
            Logger.Info($"Sending command {nameof(GetStat)}.");

            var command = GetGeneralStatusesCommand.NewOrder(_tcpPostman, this, Logger);
            _efemCommandSubscriber.AddMacro(command);
        }

        public void SetBuzzer(BuzzerState isBuzzerOn)
        {
            Logger.Info($"Sending command {nameof(SetBuzzer)}.");

            var command = SetBuzzerCommand.NewOrder(isBuzzerOn, _tcpPostman, this, Logger);
            _efemCommandSubscriber.AddMacro(command);
        }

        public void SetTime(string time)
        {
            Logger.Info($"Sending command {nameof(SetTime)}.");

            var command = SetTimeCommand.NewOrder(time, _tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(command);
        }

        /// <summary>
        /// Allow to send a custom command without automatic behavior or completion (except the frame end symbol).
        /// It is useful for testing error cases.
        /// </summary>
        /// <param name="message">The message to send to the EFEM Controller.</param>
        internal void SendCustomMessage(string message)
        {
            var customCmd = new CustomCommand(message, _tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(customCmd);
        }

        public void SetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5)
        {
            Logger.Info($"Sending command {nameof(SetE84Timeouts)}.");

            var command = SetE84TimeoutsCommand.NewOrder(tp1, tp2, tp3, tp4, tp5, _tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(command);
        }
        #endregion Global commands

        #region LoadPort Commands
        public void GetWaferSize(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(GetWaferSize)} to {loadPortId}.");

            var command = GetWaferSizeInLoadPortCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void GetCarrierType(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(GetCarrierType)} to {loadPortId}.");

            var command = GetCarrierTypeCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void GetMappingPattern(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(GetMappingPattern)} to {loadPortId}.");

            var command = GetMappingPatternCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void PerformWaferMapping(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(PerformWaferMapping)} to {loadPortId}.");

            var command = PerformWaferMappingCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void Dock(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(Dock)} to {loadPortId}.");

            var command = DockCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void Undock(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(Undock)} to {loadPortId}.");

            var command = UndockCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void ReadCarrierID(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(ReadCarrierID)} to {loadPortId}.");

            var command = ReadCarrierIdCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(command);
        }

        public void EnableE84(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(EnableE84)} to {loadPortId}.");

            var command = EnableOrDisableE84.NewOrder(loadPortId, true, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void DisableE84(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(DisableE84)} to {loadPortId}.");

            var command = EnableOrDisableE84.NewOrder(loadPortId, false, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void ResetE84(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(ResetE84)} to {loadPortId}.");

            var command = ResetE84Command.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void AbortE84(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(AbortE84)} to {loadPortId}.");

            var command = AbortE84Command.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void GetE84InputSignals(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(GetE84InputSignals)} to {loadPortId}.");

            var command = GetE84InputSignalsCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void GetE84OutputSignals(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(GetE84OutputSignals)} to {loadPortId}.");

            var command = GetE84OutputSignalsCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void CloseDoor(Constants.Port loadPortId)
        {
            Logger.Info($"Sending command {nameof(CloseDoor)} to {loadPortId}.");

            var command = CloseDoorCommand.NewOrder(loadPortId, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void ClampLp(Constants.Port loadPortId, bool isReverseOperation)
        {
            Logger.Info($"Sending command {nameof(ClampLp)} to {loadPortId}.");

            var command = ClampOnLoadPortCommand.NewOrder(loadPortId, isReverseOperation, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void SetLpLight(Constants.Port loadPortId, bool loadLightState, bool unloadLightState, bool manualLightState)
        {
            Logger.Info($"Sending command {nameof(SetLpLight)} to {loadPortId}.");

            var command = SetLpLightCommand.NewOrder(loadPortId, loadLightState, unloadLightState, manualLightState, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        public void SetCarrierType(Constants.Port loadPortId, uint carrierType)
        {
            Logger.Info($"Sending command {nameof(loadPortId)} to {loadPortId} with carrier type = {carrierType}.");

            var command = SetCarrierTypeCommand.NewOrder(loadPortId, carrierType, _tcpPostman, this, Logger);
            _commandSubscribersByLp[loadPortId].AddMacro(command);
        }

        #endregion LoadPort Commands

        #region Robot Commands

        public void Home()
        {
            Logger.Info($"Sending command {nameof(HomeCommand)} to robot.");

            var command = new HomeCommand(_tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void Pick(Constants.Arm arm, Constants.Stage stage, uint slot)
        {
            Logger.Info($"Sending command {nameof(PickCommand)} to robot.");

            var command = PickCommand.NewOrder(arm, stage, slot, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void SetRobotSpeed(char speed)
        {
            Logger.Info($"Sending command {nameof(SetRobotSpeedCommand)} to robot.");

            var command = new SetRobotSpeedCommand(speed, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void GetWaferPresenceOnArm()
        {
            Logger.Info($"Sending command {nameof(GetWaferPresenceOnArmCommand)} to robot.");

            var command = GetWaferPresenceOnArmCommand.NewOrder(_tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void PreparePick(Constants.Arm arm, Constants.Stage stage, uint slot)
        {
            Logger.Info($"Sending command {nameof(PreparePickCommand)} to robot.");

            var command = PreparePickCommand.NewOrder(arm, stage, slot, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void PreparePlace(Constants.Arm arm, Constants.Stage stage, uint slot)
        {
            Logger.Info($"Sending command {nameof(PreparePlaceCommand)} to robot.");

            var command = PreparePlaceCommand.NewOrder(arm, stage, slot, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void Place(Constants.Arm arm, Constants.Stage stage, uint slot)
        {
            Logger.Info($"Sending command {nameof(PlaceCommand)} to robot.");

            var command = PlaceCommand.NewOrder(arm, stage, slot, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void Clamp(Constants.Arm arm)
        {
            Logger.Info($"Sending command {nameof(ClampOnArmCommand)} to robot.");

            var command = ClampOnArmCommand.NewOrder(arm, false, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        public void Unclamp(Constants.Arm arm)
        {
            Logger.Info($"Sending command {nameof(ClampOnArmCommand)} to robot.");

            var command = ClampOnArmCommand.NewOrder(arm, true, _tcpPostman, this, Logger);
            _robotCommandSubscriber.AddMacro(command);
        }

        #endregion Robot Commands

        #region Aligner Commands

        public void ChuckOnAligner()
        {
            Logger.Info($"Sending command {nameof(SetAlignerChuckStateCommand)} to robot.");

            var command = SetAlignerChuckStateCommand.NewOrder(false, _tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }

        public void ChuckOffAligner()
        {
            Logger.Info($"Sending command {nameof(SetAlignerChuckStateCommand)} to robot.");

            var command = SetAlignerChuckStateCommand.NewOrder(true, _tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }

        public void Align(Angle angle, AlignType alignType = AlignType.AlignWaferWithoutCheckingSubO_FlatLocation)
        {
            Logger.Info($"Sending command {nameof(AlignCommand)} to aligner.");

            var command = AlignCommand.NewOrder(angle, alignType, _tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }

        public void Centering()
        {
            Logger.Info($"Sending command {nameof(CenteringCommand)} to aligner.");

            var command = CenteringCommand.NewOrder(_tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }

        public void GetRecipeNames(SubstrateSide side)
        {
            Logger.Info($"Sending command {nameof(GetOcrRecipesCommand)} to aligner.");

            _efemData.OcrData.ReceivedSide = null;

            var command = GetOcrRecipesCommand.NewOrder(side, _tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }

        public void ReadId(SubstrateSide side, string frontRecipeName, string backRecipeName, SubstrateTypeRDID type)
        {
            Logger.Info($"Sending command {nameof(ReadWaferIdCommand)} to aligner.");
            
            var command = ReadWaferIdCommand.NewOrder(side, frontRecipeName, backRecipeName, type, _tcpPostman, this, Logger);
            _alignerCommandSubscriber.AddMacro(command);
        }
        #endregion Aligner Commands

        #region FFU Commands

        public void SetFfuRpm(int setPoint)
        {
            Logger.Info($"Sending command {nameof(SetFfuRpmCommand)} to Efem Controller.");

            var command = SetFfuRpmCommand.NewOrder(setPoint, _tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(command);
        }

        public void GetFfuRpm()
        {
            Logger.Info($"Sending command {nameof(GetFfuRpmCommand)} to Efem Controller.");

            var command = GetFfuRpmCommand.NewOrder(_tcpPostman, this, Logger);
            _commandSubscriber.AddMacro(command);
        }

        #endregion FFU Commands

        #endregion Commands

        #region Event Handlers

        private void TcpPostman_CommunicationClosed(object sender, System.EventArgs e)
        {
            try
            {
                // Security in case of component in phase of clean-up
                if (_tcpPostman == null) { return; }

                DisableListeners();
                ClearCommandsQueue();

                // Notify drivers about loss of communication
                OnCommunicationClosed();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void TcpPostman_CommunicationIsEstablished(object sender, System.EventArgs e)
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

        #endregion Event Handlers

        #region IEquipmentFacade

        void IEquipmentFacade.RegisterAlarmist(string baseAlarmSource, int baseALID)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        void IEquipmentFacade.SendCommunicationLogEvent(
            int communicatorID,
            bool isOut,
            string message,
            DateTime dateTime)
        {
            OnMessageExchanged(new MessageExchangedEventArgs(isOut, message, dateTime));
        }

        void IEquipmentFacade.SendEquipmentAlarm(
            byte deviceNumber,
            bool isGetErrorStatus,
            string alarmKey,
            params object[] substitutionParam)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        void IEquipmentFacade.SendEquipmentEvent(int eventID, System.EventArgs args)
        {
            if (!Enum.IsDefined(typeof(EventsToFacade), eventID))
            {
                Logger.Warning(string.Format(CultureInfo.InvariantCulture, $"Communication - SendEquipmentEvent called with unknown event ID: ('{eventID}')."));
                return;
            }

            var eventFromEq = (EventsToFacade)eventID;
            
            switch (eventFromEq)
            {
                case EventsToFacade.CmdCompleteWithError:
                    // TODO
                    break;
                case EventsToFacade.GeneralStatusReceived:
                    var status = ((StatusEventArgs<GeneralStatus>)args).Status;

                    _efemData.OperationMode    = status.OperationMode;
                    _efemData.SafetyDoorSensor = status.SafetyDoorSensor;
                    _efemData.VacuumSensor     = status.VacuumSensor;
                    _efemData.AirSensor        = status.AirSensor;

                    _efemData.RobotData.RobotStatus = status.RobotStatus;
                    _efemData.RobotData.RobotSpeed  = status.RobotSpeed;
                    
                    _efemData.LoadPortsData[Constants.Port.LP1].LoadPortStatus = status.LoadPortStatus1;
                    _efemData.LoadPortsData[Constants.Port.LP2].LoadPortStatus = status.LoadPortStatus2;
                    _efemData.LoadPortsData[Constants.Port.LP3].LoadPortStatus = status.LoadPortStatus3;
                    _efemData.LoadPortsData[Constants.Port.LP4].LoadPortStatus = status.LoadPortStatus4;
                    _efemData.LoadPortsData[Constants.Port.LP1].IsCarrierPresent = status.IsLoadPort1CarrierPresent;
                    _efemData.LoadPortsData[Constants.Port.LP2].IsCarrierPresent = status.IsLoadPort2CarrierPresent;
                    _efemData.LoadPortsData[Constants.Port.LP3].IsCarrierPresent = status.IsLoadPort3CarrierPresent;
                    _efemData.LoadPortsData[Constants.Port.LP4].IsCarrierPresent = status.IsLoadPort4CarrierPresent;

                    _efemData.AlignerData.AlignerStatus           = status.AlignerStatus;
                    _efemData.AlignerData.IsAlignerCarrierPresent = status.IsAlignerCarrierPresent;

                    break;
                case EventsToFacade.SystemStatusReceived:
                    break;
                case EventsToFacade.MappingReceived:
                    if (args is MappingPatternEventArgs mappingPatternEventArgs)
                    {
                        _efemData.LoadPortsData[mappingPatternEventArgs.Port].MappingData =
                            mappingPatternEventArgs.SlotStates;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.MappingReceived}\"."
                                     + $"Expected event type is \"{nameof(MappingPatternEventArgs)}\"");
                    }

                    break;
                case EventsToFacade.CarrierPresenceReceived:
                    if (args is StatusEventArgs<CarrierPresenceStatus> carrierPresenceEvtArgs)
                    {
                        var carrierStatus = carrierPresenceEvtArgs.Status;
                        _efemData.LoadPortsData[carrierStatus.Port].IsCarrierPresent         = carrierStatus.Presence;
                        _efemData.LoadPortsData[carrierStatus.Port].IsCarrierCorrectlyPlaced = carrierStatus.Placement;
                        _efemData.LoadPortsData[carrierStatus.Port].IsHandOffBtnPressed      = carrierStatus.HandOffButtonPressed;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.CarrierPresenceReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<CarrierPresenceStatus>)}\"");
                    }

                    break;
                case EventsToFacade.ArmHistoryAndWaferPresenceReceived:
                    if (args is ArmHistoryAndWaferPresenceEventArgs armHistoryAndWaferPresenceEventArgs)
                    {
                        _efemData.RobotData.LastAction          = armHistoryAndWaferPresenceEventArgs.LastAction;
                        _efemData.RobotData.IsPresentOnUpperArm = armHistoryAndWaferPresenceEventArgs.IsPresentOnUpperArm;
                        _efemData.RobotData.StageUpperArm       = armHistoryAndWaferPresenceEventArgs.StageUpperArm;
                        _efemData.RobotData.SlotUpperArm        = armHistoryAndWaferPresenceEventArgs.SlotUpperArm;
                        _efemData.RobotData.IsPresentOnLowerArm = armHistoryAndWaferPresenceEventArgs.IsPresentOnLowerArm;
                        _efemData.RobotData.StageLowerArm       = armHistoryAndWaferPresenceEventArgs.StageLowerArm;
                        _efemData.RobotData.SlotLowerArm        = armHistoryAndWaferPresenceEventArgs.SlotLowerArm;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.ArmHistoryAndWaferPresenceReceived}\"."
                                     + $"Expected event type is \"{nameof(ArmHistoryAndWaferPresenceEventArgs)}\"");
                    }

                    break;
                case EventsToFacade.WaferIdReceived:
                    if (args is StatusEventArgs<WaferIdStatus> waferIdEvtArgs)
                    {
                        var waferIdStatus = waferIdEvtArgs.Status;
                        _efemData.AlignerData.WaferIdFrontSide = waferIdStatus.WaferIdFrontSide;
                        _efemData.AlignerData.WaferIdBackSide = waferIdStatus.WaferIdBackSide;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.WaferIdReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<WaferIdStatus>)}\"");
                    }

                    break;
                case EventsToFacade.WaferSizeReceived:
                    if (args is StatusEventArgs<WaferSizeStatus> waferSizeEvtArgs)
                    {
                        var waferSizeStatus = waferSizeEvtArgs.Status;
                        _efemData.LoadPortsData[waferSizeStatus.Port].WaferSize = waferSizeStatus.WaferSize;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.WaferSizeReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<WaferSizeStatus>)}\"");
                    }
                    
                    break;
                case EventsToFacade.CarrierTypeReceived:
                    if (args is StatusEventArgs<CarrierTypeStatus> carrierTypeEvtArgs)
                    {
                        var carrierTypeStatus = carrierTypeEvtArgs.Status;
                        _efemData.LoadPortsData[carrierTypeStatus.Port].CarrierType =
                            carrierTypeStatus.CarrierType;
                    }
                    else
                    {
                        Logger.Error($"CarrierTypeReceived: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.WaferSizeReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<CarrierTypeStatus>)}\"");
                    }
                    break;
                case EventsToFacade.CarrierIdReceived:
                    if (args is StatusEventArgs<CarrierIdStatus> carrierIdEvtArgs)
                    {
                        var carrierIdStatus = carrierIdEvtArgs.Status;
                        _efemData.LoadPortsData[carrierIdStatus.Port].CarrierID = carrierIdStatus.CarrierId;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.CarrierIdReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<CarrierIdStatus>)}\"");
                    }

                    break;
                case EventsToFacade.EfemPressureReceived:
                    if (args is StatusEventArgs<PressureStatus> pressureEvtArgs)
                    {
                        var pressureStatus = pressureEvtArgs.Status;
                        _efemData.Pressure = pressureStatus.Pressure;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.EfemPressureReceived}\"."
                                     + $"Expected event type is \"{nameof(StatusEventArgs<PressureStatus>)}\"");
                    }

                    break;
                case EventsToFacade.OcrRecipesReceived:
                    if (args is OcrRecipesReceivedEventArgs ocrRecipesReceivedEventArgs)
                    {
                        if (ocrRecipesReceivedEventArgs.OcrRecipesFront.Any())
                        {
                            _efemData.OcrData.OcrRecipesFront.Clear();
                            foreach (var kvp in ocrRecipesReceivedEventArgs.OcrRecipesFront)
                            {
                                _efemData.OcrData.OcrRecipesFront.Add(kvp.Key, kvp.Value);
                            }
                        }

                        if (ocrRecipesReceivedEventArgs.OcrRecipesBack.Any())
                        {
                            _efemData.OcrData.OcrRecipesBack.Clear();
                            foreach (var kvp in ocrRecipesReceivedEventArgs.OcrRecipesBack)
                            {
                                _efemData.OcrData.OcrRecipesBack.Add(kvp.Key, kvp.Value);
                            }
                        }

                        _efemData.OcrData.ReceivedSide = ocrRecipesReceivedEventArgs.RequestedSide;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.OcrRecipesReceived}\"."
                                     + $"Expected event type is \"{nameof(OcrRecipesReceivedEventArgs)}\"");
                    }

                    break;
                case EventsToFacade.FfuSpeedReceived:
                    if (args is FfuSpeedReceivedEventArgs ffuSpeedReceivedEventArgs)
                    {
                        _efemData.FfuSpeed = ffuSpeedReceivedEventArgs.SpeedRpm;
                    }
                    else
                    {
                        Logger.Error($"SendEquipmentEvent: Event args \"{args}\" not compatible with event "
                                     + $"\"{EventsToFacade.FfuSpeedReceived}\"."
                                     + $"Expected event type is \"{nameof(FfuSpeedReceivedEventArgs)}\"");
                    }

                    break;
                default:
                    Logger.Error($"SendEquipmentEvent: Not managed event ID received: eventID={eventFromEq} ; EventArgs={args}");
                    break;
            }

        }

        #endregion IEquipmentFacade

        #region ICommunication

        /// <summary>
        /// Notifies that communication with the device is closed.
        /// </summary>
        public event EventHandler CommunicationClosed;

        /// <summary>
        /// Notifies that communication with the device is open.
        /// </summary>
        public event EventHandler CommunicationEstablished;

        /// <summary>
        /// Notifies that a message has been exchanged between the driver and the physical device.
        /// </summary>
        public event EventHandler<MessageExchangedEventArgs> MessageExchanged;

        /// <summary>
        /// Indicates if communication is already established (<see langword="true"/>) or not (<see langword="false"/>).
        /// </summary>
        public bool IsCommunicationEnabled { get { return _tcpPostman?.IsConnected ?? false; } }

        /// <summary>
        /// Flush the queue holding commands to be sent to the device.
        /// All subscribers registered to the PostMan will be discarded
        /// </summary>
        /// <remarks>In case a command is in progress when this method is called, the command's completion will NOT be notified.</remarks>
        public void ClearCommandsQueue()
        {
            DiscardOpenTransactions(_commandSubscriber);
            DiscardOpenTransactions(_efemCommandSubscriber);
            DiscardOpenTransactions(_robotCommandSubscriber);
            DiscardOpenTransactions(_alignerCommandSubscriber);
            foreach (var commandSubscriber in _commandSubscribersByLp.Values)
                _tcpPostman.DiscardOpenTransactions(commandSubscriber);
        }

        /// <summary>
        /// Close communication with the device
        /// </summary>
        public void Disconnect()
        {
            if (_tcpPostman == null || !_tcpPostman.IsConnected) { return; }

            _tcpPostman.Disconnect();
            if (_tcpPostman.IsConnected)
            {
                Logger.Error("Failed to disconnect TCP communication.");
            }
        }

        /// <summary>
        /// Establish communication with the device
        /// </summary>
        /// <returns>
        /// <see langword="true"/> in case of success.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool EnableCommunications()
        {
            lock (this)
            {
                if (_tcpPostman == null) { return false; }

                if (_tcpPostman.IsConnected) { return true; }

                EnableListeners(); //Should be done before connecting TCP, so we can receive messages as soon as connection is established
                _tcpPostman.Connect();
                return _tcpPostman.IsConnected;
            }
        }

        /// <summary>
        /// Sends the <see cref="CommunicationClosed"/> event.
        /// </summary>
        private void OnCommunicationClosed()
        {
            try
            {
                CommunicationClosed?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Sends the <see cref="CommunicationEstablished"/> event.
        /// </summary>
        private void OnCommunicationEstablished()
        {
            try
            {
                CommunicationEstablished?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Sends the <see cref="MessageExchanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="MessageExchangedEventArgs"/> to be attached with the event.</param>
        private void OnMessageExchanged(MessageExchangedEventArgs args)
        {
            try
            {
                MessageExchanged?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion ICommunication

        #region Communication

        /// <summary>
        /// Clear the subscriber of macro command
        /// </summary>
        /// <param name="subscriberToClean">subscriber to remove</param>
        public void DiscardOpenTransactions(IMacroCommandSubscriber subscriberToClean)
        {
            Logger.Debug($"DiscardOpenTransactions on subscriber of type '{subscriberToClean.Type}'.");
            _tcpPostman.DiscardOpenTransactions(subscriberToClean);
        }

        private void DisableListeners()
        {
            Logger.Debug("Listeners are Disabling");

            _tcpPostman.DiscardOpenTransactions(_generalStatusEventSubscriber);
            _tcpPostman.DiscardOpenTransactions(_systemStatusEventSubscriber);

            foreach (var carrierPlacementEventSubscriber in _carrierPlacementEventSubscribers.Values)
                _tcpPostman.DiscardOpenTransactions(carrierPlacementEventSubscriber);
            foreach (var mappingPatternEventSubscriber in _mappingPatternEventSubscribers.Values)
                _tcpPostman.DiscardOpenTransactions(mappingPatternEventSubscriber);
            foreach (var carrierCapacityAndSizeEventSubscriber in _carrierCapacityAndSizeEventSubscribers.Values)
                _tcpPostman.DiscardOpenTransactions(carrierCapacityAndSizeEventSubscriber);            
            foreach (var e84ErrorEventEventSubscriber in _e84ErrorEventSubscribers.Values)
                _tcpPostman.DiscardOpenTransactions(e84ErrorEventEventSubscriber);

            _tcpPostman.DiscardOpenTransactions(_armHistoryAndWaferPresenceEventSubscriber);

            Logger.Debug("Listeners are Disabled");
        }

        private void EnableListeners()
        {
            Logger.Debug("Listeners are Enabling");

            var generalStatusEvt = GetGeneralStatusesCommand.NewEvent(_tcpPostman, this, Logger);
            _generalStatusEventSubscriber.AddMacro(generalStatusEvt);

            var systemStatusEvt = SystemInputEvent.NewEvent(_tcpPostman, this, Logger);
            _systemStatusEventSubscriber.AddMacro(systemStatusEvt);

            foreach (var portAndData in _efemData.LoadPortsData)
            {
                var carrierCommand = CarrierPresenceCommand.NewEvent(portAndData.Key, _tcpPostman, this, Logger);
                _carrierPlacementEventSubscribers[portAndData.Key].AddMacro(carrierCommand);

                var mappingCommand = GetMappingPatternCommand.NewEvent(portAndData.Key, _tcpPostman, this, Logger);
                _mappingPatternEventSubscribers[portAndData.Key].AddMacro(mappingCommand);

                var carrierCapacityAndSizeCommand = GetCarrierCapacityAndSizeCommand.NewEvent(portAndData.Key, _tcpPostman, this, Logger);
                _carrierCapacityAndSizeEventSubscribers[portAndData.Key].AddMacro(carrierCapacityAndSizeCommand);                
                
                var e84ErrorCommand = GetE84ErrorCommand.NewEvent(portAndData.Key, _tcpPostman, this, Logger);
                _e84ErrorEventSubscribers[portAndData.Key].AddMacro(e84ErrorCommand);
            }

            var waferPresenceOnArmEvt = GetWaferPresenceOnArmCommand.NewEvent(_tcpPostman, this, Logger);
            _armHistoryAndWaferPresenceEventSubscriber.AddMacro(waferPresenceOnArmEvt);

            Logger.Debug("Listeners are Enabled");
        }

        #endregion Communication

        #region IDeviceDriver

        /// <summary>
        /// Notifies that a command ended.
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandDone;

        /// <summary>
        /// Notifies when an error occurred while driving the device.
        /// </summary>
        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// The kind of device (Aligner, Robot...).
        /// </summary>
        /// <remarks>Should correspond to one field of <see cref="DrivenDevices"/>.</remarks>
        public string Category { get; }

        /// <summary>
        /// Port's number of the device.
        /// Used to differentiate device(s) of same type in the tool, in case there are more than one.
        /// </summary>
        /// <remarks>By default, in case there is only one device, should be "1".</remarks>
        public byte Port { get; }

        void IDeviceDriver.EmergencyStop()
        {
            throw new NotImplementedException();
        }

        ReadOnlyCollection<Error> IDeviceDriver.GetPotentialErrors()
        {
            throw new NotImplementedException();
        }

        void IDeviceDriver.Initialization()
        {
            Initialize();
        }

        /// <summary>
        /// Sends the <see cref="CommandDone"/> event.
        /// </summary>
        /// <param name="args">The <see cref="CommandEventArgs"/> to be attached with the event.</param>
        protected virtual void OnCommandDone(CommandEventArgs args)
        {
            try
            {
                var msg = new StringBuilder().AppendLine($"Command {args.Name} Done [{args.Status}]");
                if (args.Error != null)
                {
                    msg.Append(args.Error);
                }

                Logger.Info(msg.ToString());

                CommandDone?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Sends the <see cref="ErrorOccurred"/> event.
        /// </summary>
        /// <param name="args">The <see cref="ErrorOccurredEventArgs"/> to be attached with the event.</param>
        protected virtual void OnErrorOccurred(ErrorOccurredEventArgs args)
        {
            try
            {
                Logger.Info($"Error Occurred on Command {args.CommandInError}: {args}");

                ErrorOccurred?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion IDeviceDriver
    }
}
