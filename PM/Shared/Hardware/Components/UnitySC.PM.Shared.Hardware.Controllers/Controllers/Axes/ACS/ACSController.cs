using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ACS.SPiiPlusNET;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class ACSController : MotorController, IChuckClamp, IChuckMaterialPresence, IChuckAirBearing, IChuckController, IChamberController, IChamberFFUControl, IControllerIO, IDisposable
    {
        #region Fields
        private Length _singleSizeSupported = new Length(300, LengthUnit.Millimeter);
        public new string DeviceID => _controllerConfig?.DeviceID;
        private Task _pollTask;
        private readonly ACSControllerConfig _controllerConfig;
        private readonly Dictionary<string, Axis> _axesIDLinks = new Dictionary<string, Axis>();
        private bool _isLanded;
        private bool _landingInProgress;
        private bool _waferClampState;
        private MaterialPresence _lastWaferPresenceState = MaterialPresence.Unknown;
        private bool _taskResetAxisIsRunning;
        private bool _hardwareResetInProgress;
        protected Task HandleTaskWaitMotionEnd;
        public Dictionary<string, Input> NameToInput { get; set; }
        public Dictionary<string, Output> NameToOutput { get; set; }
        private Dictionary<string, bool> _doorListState = new Dictionary<string, bool>();
        private bool _isToolModeIOAvailable = false;
        private ToolMode _toolModeState = ToolMode.Unknown;
        private bool _isAckModeIOAvailable = false;

        private bool _waferStageInitIsMandatory = false;
        private bool _ffuOn;

        private bool _isWaferPresenceDefined = false;

        private struct ControllerBuffer
        {
            public ControllerBuffer(ProgramBuffer programBufferId, int programFault)
            {
                ProgramBufferId = programBufferId;
                ProgramFault = programFault;
            }

            public ProgramBuffer ProgramBufferId { get; set; }
            public int ProgramFault { get; set; }
        }

        private readonly ControllerBuffer[] _embeddedProgramBuffers = new ControllerBuffer[] {
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_1, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_2, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_3, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_4, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_5, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_6, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_7, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_10, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_11, 0),
                       new ControllerBuffer(ProgramBuffer.ACSC_BUFFER_12, 0) };

        [Flags]
        private enum FaultCodes
        {
            AirbearingPressure_ErrorCode = 1,		//!Airbearing Pressure Not Ok
            AirbearingVacuum0_ErrorCode = 2,		//!Airbearing Vacuum0 Not Ok
            AirbearingVacuum1_ErrorCode = 4,		//!Airbearing Vacuum1 Not Ok
            AirSupplyPresent_ErrorCode = 8,		    //!Air supply pressure Not Ok
            VacuumSupplyPresent_ErrorCode = 16,		//!Vacuum supply pressure Not Ok
            MotorTemperatureY0_ErrorCode = 32,		//!Motortemperature Y0 too high
            MotorTemperatureY1_ErrorCode = 64,		//!Motortemperature Y1 too high
            MotorTemperatureX_ErrorCode = 128,	    //!Motortemperature X too high
            WaferClamp_ErrorCode = 256,	            //!WaferClamp not Ok during clamping)
            Power_ErrorCode = 1024,	                //!One or more of the Power Supplies Not Ok
            softEmergencyStop_ErrorCode = 2048	    //!softEmergencyStop triggered by software
        }

        private const int MessageBufferSize = 1000;
        private const int TimeoutWaitMotionEnd = 20000; // ms

        private struct Timeouts
        {
            public const int TimeoutEnableMotor = 2000;        // ms
            public const int TimeoutLandAirBearing = 5000;     // ms
            public const int TimeoutInitFocus = 60000;         // ms
            public const int TimeoutResetFault = 30000;        // ms
            public const int TimeoutInitProcess = 5000;        // ms
            public const int TimeoutInitWaferStage = 120000;   // ms
            public const int TimeoutInitAirbearing = 60000;    // ms
            public const int TimeoutClampWafer = 5000;         // ms
            public const int TimeoutPollTaskTermiante = 1000;  // ms
        }

        private struct Actions
        {
            public const int ActionLand = 1;
            public const int ActionLStopLanding = 0;
            public const int ActionStartInit = 1;
            public const int ActionReset = 1;
            public const int ActionWaferClamp = 1;
            public const int ActionWaferRelease = 0;
            public const int ActionTurnOffFFU = 1;
            public const int ActionTurnOnFFU = 0;
        }

        //The order of land, clamp and ffu are in the configation file
        private class Commands
        {
            public const string Reset = "ResetFAULT";
            public const string Land = "LandAirbearing";
            public const string InitProcess = "ACS_Init_Process";
            public const string InitAirbearing = "ACS_Init_Airbearing";
            public const string InitTableAxes = "ACS_Init_WaferStage";
            public const string InitBottomObjective = "ACS_Init_LOH_focus";
            public const string InitTopObjective = "ACS_Init_UOH_focus";
            public const string Clamp = "WaferClamp";
            public const string FFUPower = "FFU_Power";
            public const string ServiceLight = "ServiceLight";
        };

        private struct Status
        {
            public const string Fault = "P_FAULT";
            public const string Land = "LandAirbearingStatus";
            public const string InitProcess = "ACS_Stat_Process";
            public const string Airbearing = "ACS_Stat_Airbearing";
            public const string InitTableAxes = "ACS_Stat_WaferStage";
            public const string InitBottomObjective = "ACS_Stat_LOH_focus";
            public const string InitTopObjective = "ACS_Stat_UOH_focus";
            public const string Clamp = "WaferClamp_sensor";
            public const string ServiceMode = "ServiceModeOn";
            public const string AcknowledgeAlarm = "AcknowledgeAlarm";
            public const string EMOPushed = "S_FAULT";
            public const string WaferPresence = "WaferPresence";

            public const string AirbearingVacuumSensor0 = "AirbearingVacuumSensor0";
            public const string AirbearingVacuumSensor1 = "AirbearingVacuumSensor1";
            public const string AirbearingPressureSensor = "AirbearingPressureSensor";
            public const string VacuumSensorLimit0 = "VacuumSensor0Limit";
            public const string VacuumSensorLimit1 = "VacuumSensor1Limit";

            // TODO: The functional tests are ready to test the two variables below as soon as the machine allows it
            public const string RobotIsOutService = "EFEM_RobotArmIsExtended";

            public const string PrepareToTransfert = "WEX_sensor";
        };

        private int _customFaultPrev;

        protected internal Api Channel;
        private readonly object _channelSync;

        private CancellationTokenSource _cancelationToken = new CancellationTokenSource();
        private CancellationTokenSource _cancelationTokenForIOsTask;

        public event ChuckStateChangedDelegate ChuckStateChangedEvent;

        #endregion Fields

        #region Constructors

        public ACSController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            _globalStatusServer = globalStatusServer;

            if (controllerConfig is ACSControllerConfig acsControllerConfig)
            {
                _controllerConfig = acsControllerConfig;
            }
            else
                throw (new Exception(FormatMessage("Bad controller configuration type. Controller creation failed !")));

            _channelSync = new object();
            Channel = null;

            _landingInProgress = false;
            string acsAxisID = string.Empty;

            //Init Input and Output list
            NameToInput = new Dictionary<string, Input>();
            NameToOutput = new Dictionary<string, Output>();
            _digitalIOStates = new Dictionary<string, bool>();
            _analogIOStates = new Dictionary<string, double>();

            if (controllerConfig is IOControllerConfig ioControllerConfig && ioControllerConfig != null)
            {
                NameToInput = ioControllerConfig.GetInputs();
                NameToOutput = ioControllerConfig.GetOutputs();
            }
            try
            {
                // Create AxisID list => index of list match with
                foreach (var acsAxisIDLink in _controllerConfig.ACSAxisIDLinks)
                {
                    acsAxisID = acsAxisIDLink.AxisID;
                    _axesIDLinks.Add(acsAxisID, GetAxisID(acsAxisIDLink.ACSID));
                }
            }
            catch (Exception)
            {
                throw (new Exception(FormatMessage("Bad axis [" + acsAxisID + "] parameters in configuration. Controller creation failed !")));
            }
            if (_controllerConfig.IOList.Any(io => io.Name == Status.WaferPresence && io.IsEnabled))
            {
                _isWaferPresenceDefined = true;
                Logger.Information("The property WaferPresence IO has been found in the configuration.");
            }
        }

        private void InitializeInput()
        {
            foreach (var initialInput in NameToInput)
            {
                if (initialInput.Value.IsEnabled)
                {
                    if (initialInput.Value is DigitalInput digitalInput)
                    {
                        _digitalIOStates[initialInput.Key] = DigitalRead(digitalInput);
                    }
                    if (initialInput.Value is AnalogInput analogInput)
                    {
                        _analogIOStates[initialInput.Key] = AnalogRead(analogInput);
                    }
                }
                else
                {
                    Logger?.Warning($"[ACSController] Input <{initialInput.Key}> is disabled by configuration");
                }
            }
        }

        #endregion Constructors

        #region Properties

        public bool LoadingUnloadingPosition { get; set; }
        public IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();
        public ANAChuckConfig ChuckConfiguration { get; set; }
        public Dictionary<string, Axis> ACSID { get => _axesIDLinks; }

        private readonly IGlobalStatusServer _globalStatusServer;

        private Dictionary<string, bool> _digitalIOStates;
        private Dictionary<string, double> _analogIOStates;

        // FIXME use ACSAxes instead of ControlledAxesList
        public List<ACSAxis> ACSAxes => ControlledAxesList.Cast<ACSAxis>().ToList();

        #endregion Properties

        #region Public methods

        public Axis GetAxisID(String axisID)
        {
            Axis result = Axis.ACSC_NONE;
            bool succeed = Enum.TryParse<Axis>(axisID, out result);
            if (succeed)
                return result;
            else
                throw new Exception("Axis ID is invalid. Check controller configuration");
        }

        public void Terminate()
        {
            if (_cancelationToken != null && _pollTask != null)
            {
                if (!_pollTask.IsFaulted)
                {
                    _cancelationToken.Cancel();
                    _pollTask.Wait(Timeouts.TimeoutPollTaskTermiante * 5);
                    Logger.Information("Terminated TaskControllerPollingAsync");
                }
            }
        }

        public override void Connect()
        {
            try
            {
                OpenEthernetConnection();
                if (Channel == null)
                    throw (new Exception(FormatMessage("Controller initialization Process fails")));
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Connect - Exception: " + Ex.Message));
            }
        }

        public override void Disconnect()
        {
            Logger?.Information("Close Message buffer");
            Terminate();
            Channel?.CloseMessageBuffer();

            Logger?.Information("Close communication controller");
            Channel?.CloseComm();
            Channel = null;
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    OpenMessageBuffer();
                }
                catch (SafetyException Ex)
                {
                    throw Ex;
                }
                catch (Exception Ex)
                {
                    Logger.Error(Ex.Message);
                }
                try
                {
                    DoorsStateList();
                    ReadStatus();

                    CheckIfAckAlarmIsPresentInConfigurationFile();
                    CheckToolModeIsPresentInConfigurationFile();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }
                try
                {
                    InitAirbearing();
                    ControlAirbearingPressure();
                    StopLanding();
                }
                catch (SafetyException Ex)
                {
                    throw Ex;
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Fatal, Ex.Message, DeviceID));
                }
                try
                {
                    InitProcess();
                    InitZBottomFocus();
                    InitZTopFocus();
                    CheckAndStopHomingBufferIfRunning();
                    if (_waferStageInitIsMandatory)
                    {
                        InitTableAxes();
                        _waferStageInitIsMandatory = false;
                    }
                }
                catch (AxisSafetyException ex)
                {
                    throw ex;
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }

                if (initErrors.Any(message => message.Level == MessageLevel.Fatal) ||
                    initErrors.Any(message => message.Level == MessageLevel.Error))
                {
                    State = new DeviceState(DeviceStatus.Error);
                }
                else
                {
                    State = new DeviceState(DeviceStatus.Ready);
                }
            }
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            CheckControllerCommunication();

            try
            {
                InitAllAxisSpeeds(AxisSpeed.Normal);
            }
            catch (Exception Ex)
            {
                initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
            }

            try
            {
                EnableAxis(ControlledAxesList);
                RefreshCurrentPos(ControlledAxesList);
                ACSAxes.ForEach(axis => axis.LastNotifiedPosition = axis.CurrentPos);
            }
            catch (Exception Ex)
            {
                initErrors.Add(new Message(MessageLevel.Error, Ex.Message, DeviceID));
            }

            if (initErrors.Any(message => message.Level == MessageLevel.Fatal) ||
                initErrors.Any(message => message.Level == MessageLevel.Error))
            {
                State = new DeviceState(DeviceStatus.Error);
            }
            else
            {
                State = new DeviceState(DeviceStatus.Ready);

                StartMotorsPolling();
            }
        }

        private bool AxisBlocked()
        {
            foreach (ACSAxis axis in ControlledAxesList)
            {
                var aCSAxisFault = Channel.GetFault(ACSID[axis.AxisID]);

                if (aCSAxisFault == SafetyControlMasks.ACSC_SAFETY_ES)
                {
                    //On the 2238 => when switching from maintenance mode to run the ACS removes this error
                    //On the 2229 => when switching from maintenance mode to run and run to maitenance the ACS removes this error
                    return true;
                }
                int motorFault = Channel.GetMotorError(ACSID[axis.AxisID]);

                if (motorFault == 5007 || motorFault == 5023 || motorFault == 5033)
                {
                    //On the 2229 => when switching from maintenance mode to run the ACS removes this error
                    return true;
                }

                if (!_isLanded && !_landingInProgress && !axis.Enabled && axis.Moving)
                {
                    return true;
                }
            }

            return false;
        }

        private List<string> DoorsEnabledList()
        {
            CheckControllerCommunication();
            return (from input in NameToInput
                    where input.Key.Contains("Door")
                    select input.Value.CommandName).ToList();
        }

        private void DoorsStateList()
        {
            var dicoDoor = new Dictionary<string, bool>();
            foreach (var doorEnabled in DoorsEnabledList())
            {
                bool isClosed = ReadVariable(doorEnabled);
                dicoDoor.Add(doorEnabled, isClosed);
            }
            _doorListState = dicoDoor;
        }

        private void CheckDoorStates()
        {
            var doorkeys = _doorListState.Keys.ToList();
            foreach (string doorkey in doorkeys)
            {
                bool previousState = _doorListState[doorkey];
                bool actualState = ReadVariable(doorkey);
                if (actualState != previousState)
                {
                    Logger.Warning($"Door {doorkey} state changed. Door closed status is now {actualState}");
                    _doorListState[doorkey] = actualState;
                }
            }
        }

        public void InitializationChuck(List<Message> initErrors)
        {
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    ReadStatus();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }
            }
        }

        public override bool ResetController()
        {
            bool Success = true;
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    var PFault = (int)Channel.ReadVariable(Status.Fault);
                    if (PFault != 0)
                    {
                        //Desactivate(MotionAxisList);
                        ResetFaults();
                        Success = SpinWait.SpinUntil(() =>
                        {
                            PFault = (int)Channel.ReadVariable(Status.Fault);
                            return PFault == 0;
                        }
                        , Timeouts.TimeoutResetFault);
                    }
                }
                if (!Success)
                {
                    Logger?.Error(FormatMessage("Reset Failed : unable to read P_FAULT after ResetFAULT."));
                    return false;
                }
                EnableAxis(ControlledAxesList);
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ResetAxis - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ResetAxis - Exception: " + Ex.Message));
                throw;
            }
            return true;
        }

        public override void StopLanding()
        {
            _landingInProgress = false;
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    if (_isLanded == false)
                    {
                        Logger.Information("Action : Skip Stop landing - already OFF");
                        return;
                    }
                    Logger.Information("Action : Stop landing - Started");
                    Channel.WriteVariable(Actions.ActionLStopLanding, Commands.Land);
                    bool Success = SpinWait.SpinUntil(() =>
                    {
                        return IsLanded() == false;
                    }
                   , Timeouts.TimeoutLandAirBearing);
                    if (!Success)
                    {
                        Logger.Information("Action : Stop landing - FAILED");
                        throw (new Exception(FormatMessage("Stop landing - Stop landing failed")));
                    }
                    else
                        Logger.Information("Action : Stop landing - Successfully completed");
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Stop landing - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Stop landing - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void Land()
        {
            _landingInProgress = true;
            CheckControllerCommunication();

            try
            {
                if (IsLanded())
                    return;

                // Motors using air bearing are disabled in controller embedded program Moves are
                // freezed by conroller embedded program before stopping air bearing
                Channel.WriteVariable(Actions.ActionLand, Commands.Land); // Land (= stop air bearing)
                bool Success = SpinWait.SpinUntil(() => { return IsLanded(); }, Timeouts.TimeoutLandAirBearing);
                if (!Success)
                {
                    throw (new Exception(FormatMessage("Land Failed")));
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Land - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Land - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void RefreshCurrentPos(List<IAxis> axisList)
        {
            CheckAxesListIsValid(axisList);
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    foreach (var axis in axisList)
                    {
                        if (axis.AxisConfiguration is ACSAxisConfig)
                        {
                            var axisConfig = (ACSAxisConfig)axis.AxisConfiguration;
                            axis.CurrentPos = ((Channel.GetFPosition(ACSID[axis.AxisID]) - axisConfig.PositionZero.Millimeters) * axisConfig.MotorDirection).Millimeters(); //Récupère la position de l'axe
                        }
                        else
                            throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
                    }
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("RefreshCurrentPos - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("RefreshCurrentPos - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            try
            {
                if (axis.AxisConfiguration is ACSAxisConfig)
                {
                    var axisConfig = (ACSAxisConfig)axis.AxisConfiguration;
                    var position = (Channel.GetFPosition(ACSID[axis.AxisID]) - axisConfig.PositionZero.Millimeters) * axisConfig.MotorDirection; //Récupère la position de l'axe axis
                    var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
                    return new TimestampedPosition(position.Millimeters(), highResolutionDateTime);
                }
                else
                    throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("RefreshCurrentPos - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("RefreshCurrentPos - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double stepMillimeters)
        {
            if (stepMillimeters == 0)
                return;
            List<IAxis> axesList = new List<IAxis> { axis };
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    SetSpeedAxis(axesList, new List<AxisSpeed>() { speed });
                    CheckAxisIsNotLanded(axesList);

                    if (axis.AxisConfiguration is ACSAxisConfig)
                    {
                        var nextPosition = axis.CurrentPos.Millimeters + stepMillimeters;
                        var maxPosition = axis.AxisConfiguration.PositionMax.Millimeters;
                        var minPosition = axis.AxisConfiguration.PositionMin.Millimeters;

                        if (nextPosition > maxPosition)
                        {
                            stepMillimeters = maxPosition - axis.CurrentPos.Millimeters;
                        }
                        else if (nextPosition < minPosition)
                        {
                            stepMillimeters = minPosition - axis.CurrentPos.Millimeters;
                        }

                        Channel.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, ACSID[axis.AxisID], stepMillimeters * ((ACSAxisConfig)axis.AxisConfiguration).MotorDirection);
                    }
                    else
                        throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("MoveIncremental - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("MoveIncremental - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> commandCoords, List<IAxis> axesList, List<double> speeds, List<double> accels)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            double epsilon_mm = _controllerConfig.PositionLimitEpsilon?.Millimeters ?? 0.01;

            lock (_channelSync)
            {
                try
                {
                    int indexPos = 0;
                    var posList = new List<double>();
                    var axisIndexList = new List<Axis>();
                    foreach (var axis in axesList)
                    {
                        if (!(axis.AxisConfiguration is ACSAxisConfig))
                        {
                            throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
                        }

                        var acsAxisConfig = (ACSAxisConfig)axis.AxisConfiguration;
                        double position = commandCoords[indexPos] + acsAxisConfig.PositionZero.Millimeters;
                        if (position > acsAxisConfig.PositionMax.Millimeters + epsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {position:0.000} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax.Millimeters:0.000} mm");
                            position = acsAxisConfig.PositionMax.Millimeters;
                        }
                        if (position < acsAxisConfig.PositionMin.Millimeters - epsilon_mm)
                        {
                            Logger.Error($"CheckAxisLimits for {axis.AxisID} : {position:0.000} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin.Millimeters:0.000} mm");
                            position = acsAxisConfig.PositionMin.Millimeters;
                        }
                        posList.Add(position * acsAxisConfig.MotorDirection);
                        axisIndexList.Add(ACSID[axis.AxisID]);
                        indexPos++;
                    }
                    axisIndexList.Add(Axis.ACSC_NONE);
                    posList.Add(-1);

                    SetSpeedAccelAxis(axesList, speeds, accels);
                    CheckAxisIsNotLanded(axesList);

                    //_channel.ToPointM(MotionFlags.ACSC_NONE, axisIndexList.ToArray(), posList.ToArray()); // "MotionFlags.ACSC_NONE" is required for absolute moves
                    //  "MotionFlags.ACSC_AMF_MAXIMUM" : Multi-axis motion does not use the motion parameters from the leading axis but calculates the maximum allowed motion velocity, acceleration, deceleration and jerk of the involved axes
                    Channel.ToPointM(MotionFlags.ACSC_AMF_MAXIMUM, axisIndexList.ToArray(), posList.ToArray());

                    HandleTaskWaitMotionEnd = new Task(() => { TaskWaitMotionEnd(axesList, commandCoords); }, TaskCreationOptions.LongRunning);
                    HandleTaskWaitMotionEnd.Start();
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("SetPosAxisWithSpeedAndAccel - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("SetPosAxisWithSpeedAndAccel - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        public void Dispose()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    Logger.Information("ACSController. Close communication controller");
                    Channel.CloseComm(); //Fermeture de la connexion du controlleur
                    Channel = null;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Dispose - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Dispose - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void EnableAxis(List<IAxis> axesList)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            try
            {
                foreach (IAxis axis in axesList)
                {
                    lock (_channelSync)
                    {
                        Channel.Enable(ACSID[axis.AxisID]);
                        Channel.WaitMotorEnabled(ACSID[axis.AxisID], (int)MotorStates.ACSC_MST_ENABLE, Timeouts.TimeoutEnableMotor);
                    }

                    RefreshAxisState(axis);

                    if (!axis.Enabled)
                        throw (new Exception(FormatMessage("Activation of Axis" + axis.AxisID + " Timeout Error ")));
                    else
                        Logger.Information("Action : " + axis.AxisID + " axis enabled - successfully completed");
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("EnableAxis - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("EnableAxis - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void DisableAxis(List<IAxis> axesList)
        {
            const int disabledState = 0;
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();
            try
            {
                foreach (IAxis axisToDisable in axesList)
                {
                    lock (_channelSync)
                    {
                        if (axisToDisable.Enabled == false)
                            continue;

                        if (axisToDisable.IsLandingUsed)
                        {
                            Land(); // Stops air bearing and disable according axis
                        }
                        else
                        {
                            Channel.Disable(ACSID[axisToDisable.AxisID]);
                            Channel.WaitMotorEnabled(ACSID[axisToDisable.AxisID], disabledState, Timeouts.TimeoutEnableMotor); // Waits end of disabling
                        }
                    }
                    RefreshAxisState(axisToDisable);

                    if (axisToDisable.Enabled == true) // Disabling failed
                        throw (new Exception(FormatMessage("DisableAxis - Desactivation of Axis " + ACSID[axisToDisable.AxisID])));
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("DisableAxis - ACSException: " + ACSEx.Message));
                Logger?.Error(FormatMessage("DisableAxis [StackTrace]:" + ACSEx.StackTrace));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("DisableAxis - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void StopAxesMotion()
        {
            CheckControllerCommunication();

            if (ControlledAxesList == null)
                throw (new Exception(FormatMessage("Function StopAxisMotion failed, axisList is null")));

            lock (_channelSync)
            {
                try
                {
                    //_channel.Halt(ACSID[axis.AxisID]); // Stop axis move
                    var AxisIndexList = new List<ACS.SPiiPlusNET.Axis>();
                    foreach (IAxis axis in ControlledAxesList)
                        AxisIndexList.Add(ACSID[axis.AxisID]);

                    AxisIndexList.Add(Axis.ACSC_NONE);
                    Channel.HaltM(AxisIndexList.ToArray()); // Stop axis move
                    Channel.EndSequenceM(AxisIndexList.ToArray());
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("StopAxisMotion - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("StopAxisMotion - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed axisSpeed, double oneCoordinate)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    ACSAxis acsAxisToMove = (ACSAxis)axis;
                    var AxisConfig = acsAxisToMove.ACSAxisConfig;

                    double Position = acsAxisToMove.ComputePositionInControllerFrame(oneCoordinate);
                    CheckAxisLimits(acsAxisToMove, Position); //Contrôle de la position demandée

                    double speed, accel = 0;
                    ConvertSpeedEnum(axis, axisSpeed, out speed, out accel);
                    var ServiceModeIsEnabled = (int)Channel.ReadVariable(Status.ServiceMode);
                    if (ServiceModeIsEnabled != 0)
                        if (speed > AxisConfig.MaxSpeedService)
                            speed = AxisConfig.MaxSpeedService;

                    Channel.ExtToPoint(MotionFlags.ACSC_AMF_VELOCITY, ACSID[acsAxisToMove.AxisID], Position * acsAxisToMove.ACSAxisConfig.MotorDirection, speed, speed);
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error("LinearMotionSingleAxis - ACSException: " + ACSEx.Message);
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error("LinearMotionSingleAxis - Exception: " + Ex.Message);
                    throw;
                }
            }
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axesList, AxisSpeed axisSpeed, List<double> coordsList)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    var axisToMoveIdList = new List<ACS.SPiiPlusNET.Axis>();
                    var ServiceModeIsEnabled = (int)Channel.ReadVariable(Status.ServiceMode);
                    double speed = 0, accel = 0;

                    for (int i = 0; i < axesList.Count; i++)
                    {
                        // Add to axisToMoveIdList
                        ACSAxis acsAxis = null;
                        if (axesList[i] is ACSAxis)
                            acsAxis = (ACSAxis)axesList[i];
                        else
                            throw new Exception("Bad Axis type in LinearMotionMultipleAxis()");
                        axisToMoveIdList.Add(ACSID[acsAxis.AxisID]);

                        // Update coordsList
                        double coord = acsAxis.ComputePositionInControllerFrame(coordsList[i]);
                        CheckAxisLimits(acsAxis, coord);
                        coordsList[i] = coord * acsAxis.ACSAxisConfig.MotorDirection;

                        // Update speed
                        ConvertSpeedEnum(acsAxis, axisSpeed, out speed, out accel);
                        // If service mode enabled, keep the minimal speed service from all axis
                        if (ServiceModeIsEnabled != 0)
                            if (speed > acsAxis.ACSAxisConfig.MaxSpeedService)
                                speed = acsAxis.ACSAxisConfig.MaxSpeedService;
                    }

                    // End axis for command
                    axisToMoveIdList.Add(ACS.SPiiPlusNET.Axis.ACSC_NONE); // After the last axis, one additional element must be included that contains –1 and marks the end of the array.
                    coordsList.Add(-1); // The point must specifyavalue for each element of axesexcept the last –1 element.

                    // MotionFlags.ACSC_AMF_VELOCITY: the motion will use velocity specified by the
                    // Velocity argument instead of the default velocity. endVelocity specifies
                    // speed when reaching target point (move will not stop at target point)
                    Channel.ExtToPointM(MotionFlags.ACSC_AMF_VELOCITY, axisToMoveIdList.ToArray(), coordsList.ToArray(), speed, speed);
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error("LinearMotionMultipleAxis - ACSException: " + ACSEx.Message);
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error("LinearMotionMultipleAxis - Exception: " + Ex.Message);
                    throw;
                }
            }
        }

        public override void RefreshAxisState(IAxis axis)
        {
            CheckAxesListIsValid(new List<IAxis>() { axis });
            CheckControllerCommunication();
            try
            {
                ACSAxis acsAxis = axis as ACSAxis;
                lock (_channelSync)
                {
                    MotorStates MotorState = Channel.GetMotorState(ACSID[acsAxis.AxisID]);
                    // Apply mask ACSC_MST_ENABLE = 0x00000001 to check if motor is enabled
                    axis.Enabled = Convert.ToBoolean(MotorState & MotorStates.ACSC_MST_ENABLE);
                    // Apply mask ACSC_MST_ENABLE = 0x00100000 to check if motor is moving
                    axis.Moving = Convert.ToBoolean(MotorState & MotorStates.ACSC_MST_MOVE);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("RefreshAxisState - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("RefreshAxisState - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            CheckControllerCommunication();
            try
            {
                var endWatingTime = DateTime.Now.AddMilliseconds(timeout);

                foreach (IAxis axis in ControlledAxesList)
                {
                    var axesTimeout = (int)(endWatingTime - DateTime.Now).TotalMilliseconds;

                    if (axesTimeout <= 0)
                        axesTimeout = 1;
                    Channel.WaitMotionEnd(ACSID[axis.AxisID], axesTimeout);
                }

                if (waitStabilization)
                    Thread.Sleep(AxesConfiguration.StabilizationTimems);
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("WaitMotionEnd - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("WaitMotionEnd - Exception: " + Ex.Message));
                throw;
            }
        }

        #endregion Public methods

        #region Private/Protected methods

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();
            ACSAxis acsAxis = axis as ACSAxis;
            lock (_channelSync)
            {
                try
                {
                    var ServiceModeIsEnabled = (int)Channel.ReadVariable(Status.ServiceMode);
                    if (ServiceModeIsEnabled == 0)
                        return;

                    if (speed > acsAxis.ACSAxisConfig.MaxSpeedService)
                        speed = acsAxis.ACSAxisConfig.MaxSpeedService;
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("CheckServiceSpeed - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("CheckServiceSpeed - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            // No need - CheckAxesListIsValid(axisList);
            int index = 0;
            try
            {
                if (axisList.Count != speedsList.Count)
                {
                    string errorMessage = FormatMessage("SetSpeedAxis: Each axis speed must be specified (axis list size != speeds list size");
                    Logger?.Error(errorMessage);
                    throw (new Exception(errorMessage));
                }

                foreach (IAxis axis in axisList)
                {
                    double speed = speedsList[index];
                    double accel = accelsList[index];
                    CheckServiceSpeed(axis, ref speed);
                    ChangeSpeedIfDifferent(axis, speed, accel);
                    index++;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            int index = 0;
            try
            {
                if (axisList.Count != speedsList.Count)
                {
                    string errorMessage = FormatMessage("SetSpeedAxis: Each axis speed must be specified (axis list size != speeds list size");
                    Logger?.Error(errorMessage);
                    throw (new Exception(errorMessage));
                }

                foreach (IAxis axis in axisList)
                {
                    double speed = 0;
                    double accel = 0;
                    ConvertSpeedEnum(axis, speedsList[index], out speed, out accel);
                    CheckServiceSpeed(axis, ref speed);
                    ChangeSpeedIfDifferent(axis, speed, accel);
                    index++;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckAxisIsNotLanded(List<IAxis> axesList)
        {
            if (IsLanded() == true)
            {
                foreach (var axis in axesList)
                {
                    if (axis.IsLandingUsed == true)
                    {
                        throw (new Exception(FormatMessage("Move impossible without airbearing!")));
                    }
                }
            }
        }

        public override void CheckControllerCommunication()
        {
            if (Channel != null)
                return;

            string errorMsg = FormatMessage("Communication channel to ACS is null");
            Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public void InitProcess()
        {
            CheckControllerCommunication();

            try
            {
                _hardwareResetInProgress = true;
                Logger.Information("Action : Init stat process - Started");
                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionStartInit, Commands.InitProcess);
                }

                Channel.WaitProgramEnd(ProgramBuffer.ACSC_BUFFER_3, Timeouts.TimeoutInitProcess);
                Thread.Sleep(AxesConfiguration is null ? 500 : AxesConfiguration.StabilizationTimems);

                lock (_channelSync)
                {
                    int processIsInitialized = (int)Channel.ReadVariable(Status.InitProcess);

                    if (processIsInitialized != 1)
                    {
                        Logger.Error("Action : Init stat process - FAILED !");
                        throw (new SafetyException(FormatMessage("Failed to set ACS_Stat_Process")));
                    }
                    else
                    {
                        Logger.Information("Action : Init stat process - Successfully completed");
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Init stat process - ACSException: " + ACSEx.Message));
                throw new SafetyException("Init stat process - FAILED ");
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitStatProcess - Exception: " + Ex.Message));
                throw;
            }
            finally
            {
                _hardwareResetInProgress = false;
            }
        }

        private void InitAllAxisSpeeds(AxisSpeed axisSpeed)
        {
            CheckControllerCommunication();

            try
            {
                List<AxisSpeed> speedList = new List<AxisSpeed>();
                foreach (var axis in ControlledAxesList)
                {
                    speedList.Add(axisSpeed);
                }

                lock (_channelSync)
                {
                    SetSpeedAxis(ControlledAxesList, speedList);
                }
                foreach (IAxis Axis in ControlledAxesList)
                {
                    Axis.Initialized = true;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitAllAxisSpeeds - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitAllAxisSpeeds - Exception: " + Ex.Message));
                throw;
            }
        }

        public void InitAirbearing()
        {
            CheckControllerCommunication();

            try
            {
                _hardwareResetInProgress = true;
                Logger.Information("Action : Init Airbearing - Started");
                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionStartInit, Commands.InitAirbearing);
                }

                //The buffer 3 is the buffer used during actions on the vacuum.
                Channel.WaitProgramEnd(ProgramBuffer.ACSC_BUFFER_3, Timeouts.TimeoutInitWaferStage);
                Thread.Sleep(AxesConfiguration is null? 500 : AxesConfiguration.StabilizationTimems);

                lock (_channelSync)
                {
                    int InitAirbearing = (int)Channel.ReadVariable(Status.Airbearing);

                    if (InitAirbearing != 1)
                    {
                        Logger.Error("Init airbearing failed");
                        throw (new Exception(FormatMessage("Failed to set ACS_Stat_Airbearing")));
                    }
                }

                Logger.Information("Action : Init airbearing - Successfully completed");
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitAirbearing - ACSException: " + ACSEx.Message));

                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitAirbearing - Exception: " + Ex.Message));
                throw;
            }
            finally
            {
                _hardwareResetInProgress = false;
            }
        }

        public override void InitZBottomFocus()
        {
            CheckControllerCommunication();

            try
            {
                _hardwareResetInProgress = true;
                Logger.Information("Action : Init Z bottom focus - Started");

                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionStartInit, Commands.InitBottomObjective);
                }

                Channel.WaitProgramEnd(ProgramBuffer.ACSC_BUFFER_6, Timeouts.TimeoutInitFocus);
                Thread.Sleep(AxesConfiguration is null ? 500 : AxesConfiguration.StabilizationTimems);

                lock (_channelSync)
                {
                    int stateZBottomFocus = (int)Channel.ReadVariable(Status.InitBottomObjective);
                    if (stateZBottomFocus != 1)
                    {
                        Logger.Information("Action : Init Z bottom focus - FAILED");
                        throw (new Exception(FormatMessage("Failed to set ACS_Stat_LOH_focus")));
                    }
                    else
                    {
                        Logger.Information("Action : Init Z bottom focus - Successfully completed");
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitLohFocus - ACSException: " + ACSEx.Message));
                throw new SafetyException("Init Z bottom focus - FAILED ");
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitLohFocus - Exception: " + Ex.Message));
                throw;
            }
            finally
            {
                _hardwareResetInProgress = false;
            }
        }

        public override void InitZTopFocus()
        {
            CheckControllerCommunication();

            try
            {
                _hardwareResetInProgress = true;
                Logger.Information("Action : Init Z top focus - Started");

                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionStartInit, Commands.InitTopObjective);
                }

                Channel.WaitProgramEnd(ProgramBuffer.ACSC_BUFFER_6, Timeouts.TimeoutInitFocus);
                Thread.Sleep(AxesConfiguration is null ? 500 : AxesConfiguration.StabilizationTimems);

                lock (_channelSync)
                {
                    int Stat_Z_Top = (int)Channel.ReadVariable(Status.InitTopObjective);
                    if (Stat_Z_Top != 1)
                    {
                        Logger.Information("Action : Init Z top focus - FAILED !");
                        throw (new Exception(FormatMessage("Failed to set ACS_Stat_UOH_focus")));
                    }
                    else
                    {
                        Logger.Information("Action : Init Z top focus - Successfully completed");
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitUohFocus - ACSException: " + ACSEx.Message));
                Logger.Error("Action : Init Z top focus - FAILED !");
                throw (new AxisSafetyException(FormatMessage("Failed to set ACS_Stat_UOH_focus")));
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitUohFocus - Exception: " + Ex.Message));
                throw;
            }
            finally
            {
                _hardwareResetInProgress = false;
            }
        }

        public void InitTableAxes()
        {
            CheckControllerCommunication();

            try
            {
                _hardwareResetInProgress = true;
                Logger.Information("Action : Init XY axes - Started, Please wait, the operation may take a few minutes");

                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionStartInit, Commands.InitTableAxes);
                }

                //Time needed to write the buffer corresponding to the initialization of the table.
                Channel.WaitProgramEnd(ProgramBuffer.ACSC_BUFFER_4, Timeouts.TimeoutInitWaferStage);
                Thread.Sleep(AxesConfiguration is null ? 500 : AxesConfiguration.StabilizationTimems);

                Logger.Information("Action : Init XY axes - Successfully completed");

                int Stat_X = (int)Channel.ReadVariable(Status.InitTableAxes);
                if (Stat_X != 1)
                {
                    Logger.Error("Failed to init table axes");
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitTableAxes - ACSException: " + ACSEx.Message));
                throw (new Exception(FormatMessage("Failed to set ACS_Stat_WaferStage")));
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitTableAxes - Exception: " + Ex.Message));

                throw;
            }
            finally
            {
                _hardwareResetInProgress = false;
            }
        }

        private void ReadStatus()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    var Landing = (int)Channel.ReadVariable(Status.Land);
                    string message = FormatMessage("Read: LandAirbearingStatus = " + (Landing == 1 ? "LANDED" : "NOT LANDED"));
                    Logger.Information(message);

                    var Stat_Init_Process = (int)Channel.ReadVariable(Status.InitProcess);
                    message = FormatMessage("Read: Stat Process = " + (Stat_Init_Process == 1 ? "Process functions initialized" : "Process functions NOT initialized"));
                    Logger.Information(message);

                    var ServiceMode = (int)Channel.ReadVariable(Status.ServiceMode);
                    message = FormatMessage("Read: Service Mode = " + (ServiceMode == 1 ? "ON" : "OFF"));
                    Logger.Information(message);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ReadStatus - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ReadStatus - Exception: " + Ex.Message));
                throw;
            }
        }

        private void TryGetMessageFromController(ref string previousMsg)
        {
            // try to get messages that the controller sends on its own initiative and not as a
            // response to a command.
            try
            {
                lock (_channelSync)
                {
                    bool bClear = true;
                    string MsgFromController = Channel.GetMessage(bClear);
                    if ((!string.IsNullOrWhiteSpace(MsgFromController))
                        && (MsgFromController.CompareTo(previousMsg) != 0))
                    {
                        Logger?.Information((FormatMessage("TryGetMessageFromController - Message: " + MsgFromController)));
                        previousMsg = MsgFromController;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                if (ACSEx.ErrorCode == (int)ErrorCodes.ACSC_TIMEOUT) // Timeout due to no message to read, so it is OK
                {
                    return;
                }
                else
                {
                    Logger?.Error(FormatMessage("TryGetMessageFromController - ACSException: " + ACSEx.Message));
                    throw;
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("TryGetMessageFromController - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckCustomFaults()
        {
            CheckControllerCommunication();

            lock (_channelSync)
            {
                try
                {
                    // Check if there are faults Get custom faults, defined by Unity-SC in "ACSEmbedded.prg"
                    int customFault = (int)Channel.ReadVariable(Status.Fault);

                    if (customFault != 0)
                    {
                        if (((FaultCodes)customFault).HasFlag(FaultCodes.AirbearingPressure_ErrorCode))
                        {
                            ControlAirbearingPressure();
                        }
                        if (customFault != _customFaultPrev)
                        {
                            string errorMsg = (FormatMessage("GetCustomFaults - Motion controller custom fault :" + (FaultCodes)customFault));
                            Logger?.Error(errorMsg);
                            RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, errorMsg, DeviceID));
                        }
                    }
                    _customFaultPrev = customFault;
                }
                catch (SafetyException Ex)
                {
                    Logger?.Error(FormatMessage("GetCustomFaults - Exception: " + Ex.Message));
                    throw Ex;
                }
                catch (ACSException ACSEx)
                {
                    Logger?.Error(FormatMessage("GetCustomFaults - ACSException: " + ACSEx.Message));
                    throw;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage("GetCustomFaults - Exception: " + Ex.Message));
                    throw;
                }
            }
        }

        private void ControlAirbearingPressure()
        {
            int faultCode = (int)Channel.ReadVariable(Status.Fault);
            if (((FaultCodes)faultCode).HasFlag(FaultCodes.AirbearingPressure_ErrorCode))
            {
                //Test if it's a transitional state.
                Thread.Sleep(2000);

                faultCode = (int)Channel.ReadVariable(Status.Fault);
                if (((FaultCodes)faultCode).HasFlag(FaultCodes.AirbearingPressure_ErrorCode))
                {
                    LogPressureErrorsIfExceededLimits();

                    Logger.Error("Pressure fault detected");

                    string errMessage = "HW SAFETY DISABLED ANY STAGE MOVES : Default pression in error";
                    Logger.Error(errMessage);
                    throw new SafetyException($"{errMessage}");
                }
            }
        }

        private void LogPressureErrorsIfExceededLimits()
        {
            object sensor0Limit;
            object sensor1Limit;
            Dictionary<string, float> pressures;
            lock (_channelSync)
            {
                pressures = GetAirBearingPressuresValues();

                sensor0Limit = Channel.ReadVariable(Status.VacuumSensorLimit0);
                sensor1Limit = Channel.ReadVariable(Status.VacuumSensorLimit1);
            }
            if (!float.TryParse(sensor0Limit.ToString(), out float sensor0Limit_berg))
            {
                Logger.Error($"[ACSController] Cannot parse vacuum sensor 0 value <{sensor0Limit}>");
                sensor0Limit_berg = float.MaxValue;
            }
            if (!float.TryParse(sensor1Limit.ToString(), out float sensor1Limit_berg))
            {
                Logger.Error($"[ACSController] Cannot parse vacuum sensor 1 value <{sensor1Limit}>");
                sensor1Limit_berg = float.MaxValue;
            }

            foreach (var pressure in pressures)
            {
                if (pressure.Key == Status.AirbearingVacuumSensor0 && pressure.Value < sensor0Limit_berg)
                {
                    string message = $"[ACSController] Actual pression for {pressure.Key} is : {pressure.Value}" +
                        $"Pression limit is {sensor0Limit} berg";
                    Logger.Error(message);
                }
                if (pressure.Key == Status.AirbearingVacuumSensor1 && pressure.Value < sensor1Limit_berg)
                {
                    string message = $"[ACSController] Actual pression for {pressure.Key} is : {pressure.Value}" +
                        $"Pression limit is {sensor1Limit} berg";
                    Logger.Error(message);
                }
            }
        }

        private void CheckAndStopHomingBufferIfRunning()
        {
            CheckControllerCommunication();

            try
            {
                // Checks if buffer 4, responsible for axis homing, is currently running.
                // In case it is running, stop it to prevent any issues during startup.
                lock (_channelSync)
                {
                    var programState = Channel.GetProgramState(ProgramBuffer.ACSC_BUFFER_4);
                    if (programState != ProgramStates.ACSC_PST_COMPILED)
                    {
                        string message = "[ACSController] Buffer number 4 is already running. Stop.";
                        Logger.Warning(message);
                        Channel.StopBuffer(ProgramBuffer.ACSC_BUFFER_4);
                        _waferStageInitIsMandatory = true;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("[ACSController] CheckAndStopHomingBufferIfRunning - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("[ACSController] CheckAndStopHomingBufferIfRunning - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckProgramFaults()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    // Get program faults
                    for (int indice = 0; indice < _embeddedProgramBuffers.Length; indice++)
                    {
                        int programFault = Channel.GetProgramError(_embeddedProgramBuffers[indice].ProgramBufferId);
                        if (programFault != 0)
                        {
                            string error = Channel.GetErrorString(programFault);
                            Logger.Error(error);
                        }
                        if ((programFault != 0) && (programFault != _embeddedProgramBuffers[indice].ProgramFault))
                        {
                            string errorMsg = FormatMessage("GetProgramFaults - Buffer " + _embeddedProgramBuffers[indice].ProgramBufferId + " Error " + programFault + " " + Channel.GetErrorString(programFault));
                            Logger?.Error(errorMsg);
                            RaiseErrorEvent(new Message(MessageLevel.Warning, errorMsg, errorMsg, DeviceID));
                        }
                        _embeddedProgramBuffers[indice].ProgramFault = programFault;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("GetProgramFaults - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("GetProgramFaults - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckAxisErrors()
        {
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        var ACSAxisFault = Channel.GetFault(ACSID[axis.AxisID]);
                        if (ACSAxisFault != SafetyControlMasks.ACSC_NONE)
                        {
                            if (axis.Fault != ACSAxisFault)
                            {
                                string message = FormatMessage("GetAxisFaults - GetFault: " + ACSAxisFault);// TODO: translation should come from xml
                                Logger?.Error(message);

                                axis.DeviceError = new Message(MessageLevel.Error, message, message, "Axis id: " + ACSID[axis.AxisID]);
                                RaiseErrorEvent(axis.DeviceError);
                            }
                        }
                        axis.Fault = ACSAxisFault;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (AxisSafetyException Ex)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - AxisSafetyException: " + Ex.Message));
                throw Ex;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckMotorsErrors()
        {
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        // Get reason for motor disabling
                        int motorFault = Channel.GetMotorError(ACSID[axis.AxisID]);

                        if (motorFault != 0)
                        {
                            if (axis.MotorError != motorFault)
                            {
                                string message = FormatMessage("GetAxisFaults - motorFault: " + motorFault + " " + Channel.GetErrorString(motorFault));
                                Logger?.Error(message);
                                axis.DeviceError = new Message(MessageLevel.Error, message, message, "Axis id: " + ACSID[axis.AxisID]);
                                RaiseErrorEvent(axis.DeviceError);
                            }
                        }
                        axis.MotorError = motorFault;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (AxisSafetyException ex)
            {
                throw ex;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckMotionErrors()
        {
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        // Get reason for motor disabling
                        var motionFault = Channel.GetMotionError(ACSID[axis.AxisID]);

                        // Codes from 5000 to 5008 do not indicate an error. They report normal
                        // motion termination Codes from 5009 and higher appear when a motion is
                        // terminated or a motor is disabled due to a fault detected by the
                        // controller. (See §10.1.1 of "ACPSL Programmer's guide" rev 2.70)
                        if (motionFault >= 5009)
                        {
                            if (axis.MotionError != motionFault)
                            {
                                string motionFaultMessage = FormatMessage("GetAxisFaults - motionFault: " + motionFault + " " + Channel.GetErrorString(motionFault));
                                Logger?.Error(motionFaultMessage);
                                axis.DeviceError = new Message(MessageLevel.Error, motionFaultMessage, motionFaultMessage, "Axis id: " + ACSID[axis.AxisID]);
                                RaiseErrorEvent(axis.DeviceError);
                            }
                        }
                        axis.MotionError = motionFault;
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("GetAxisFaults - Exception: " + Ex.Message));
                throw;
            }
        }

        private void RefreshAndRaiseStateEventsForAxesAndWaferClamp(bool forceRefresh = false)
        {
            foreach (var axis in ControlledAxesList)
                RefreshAxisState(axis);

            bool newAxesState = CheckIfAxisStateHasChanged() || CheckIfLandedStateHasChanged();
            bool newChuckState = IsNewWaferClampState();

            if (newAxesState || forceRefresh)
            {
                bool AllAxisEnabled = true;
                bool OneAxisIsMoving = false;

                foreach (var axis in ControlledAxesList)
                {
                    AllAxisEnabled &= axis.Enabled;
                    OneAxisIsMoving |= axis.Moving;
                }
                RaiseStateChangedEvent(new AxesState(AllAxisEnabled, OneAxisIsMoving, _isLanded));
            }
            if (newChuckState || forceRefresh)
            {
                bool waferClamped = RefreshIsWaferClamped();
                RaiseStateChangedEvent(CreateChuckState(waferClamped, waferClamped ? MaterialPresence.Present : MaterialPresence.NotPresent));
            }
        }

        public ChuckState CreateChuckState(bool clamped, MaterialPresence presence)
        {            
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(_singleSizeSupported, clamped);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(_singleSizeSupported, presence );
            return new ChuckState(clampStates, presenceStates);
        }

        private bool CheckIfAxisStateHasChanged()
        {
            bool hasChanged = false;

            foreach (var axis in ControlledAxesList)
            {
                if ((axis.Enabled != axis.EnabledPrev) || (axis.Moving != axis.MovingPrev))
                {
                    hasChanged = true;
                    break;
                }

                axis.EnabledPrev = axis.Enabled;
                axis.MovingPrev = axis.Moving;
            }

            return hasChanged;
        }

        private bool CheckIfLandedStateHasChanged()
        {
            bool isLandedStatePrev = _isLanded;
            _isLanded = IsLanded();
            if (isLandedStatePrev != _isLanded)
                return true;

            return false;
        }

        private bool IsNewWaferClampState()
        {
            bool waferClampStatePrev = _waferClampState;
            _waferClampState = RefreshIsWaferClamped();
            if (waferClampStatePrev != _waferClampState)
                return true;

            return false;
        }

        private void ResetFaults()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    Channel.WriteVariable(Actions.ActionReset, Commands.Reset);
                    Logger.Information("Action : Reset Fault - Successfully completed");
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ResetFaults - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ResetFaults - Exception: " + Ex.Message));
                throw;
            }
        }

        public List<string> GetEnabledPressureSensors()
        {
            var sensorList = new List<string>();
            foreach (var io in _controllerConfig.IOList)
            {
                if (io is AnalogInput analogInput)
                {
                    if (analogInput.IsEnabled && analogInput.CommandName.Contains("Airbearing"))
                    {
                        sensorList.Add(analogInput.CommandName);
                    }
                }
            }
            return sensorList;
        }

        public Dictionary<string, float> GetAirBearingPressuresValues()
        {
            lock (_channelSync)
            {
                var airPressureDico = new Dictionary<string, float>();
                foreach (string airbearingName in GetEnabledPressureSensors())
                {
                    object pressure = Channel.ReadVariable(airbearingName);
                    float.TryParse(pressure.ToString(), out float pressure_berg);
                    airPressureDico.Add(airbearingName, pressure_berg);
                }
                return airPressureDico;
            }
        }

        private async Task TaskControllerPollingAsync()
        {
            string previousMsg = "";
            bool axesBlocked;
            if (_cancelationToken == null)
            {
                return;
            }
            Logger.Information("Starting TaskControllerPollingAsync");
            while (!_cancelationToken.IsCancellationRequested)
            {
                try
                {
                    axesBlocked = AxisBlocked();
                    if (axesBlocked && !_taskResetAxisIsRunning && !_hardwareResetInProgress && _globalStatusServer.GetGlobalState() != PMGlobalStates.Error)
                    {
     
                        AskTaskResetAxis();
                    }

                    CheckDoorStates();
                    CheckWaferPresenceStateAndNotifyChanges();

                    CheckCustomFaults();
                    CheckAxisErrors();
                    CheckMotionErrors();
                    CheckMotorsErrors();

                    CheckProgramFaults();

                    RefreshAndRaiseStateEventsForAxesAndWaferClamp();
                    RaiseEventIfToolModeChanged();

                    TryGetMessageFromController(ref previousMsg);
                    RaiseEventIfPositionChanged();
                }
                catch (SafetyException Ex)
                {
                    string errorMsg = FormatMessage($"TaskMotorsPolling Safety Exception : {Ex.Message}");
                    Logger?.Error(errorMsg);
                    RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                    throw Ex;
                }
                catch (AggregateException aex)
                {
                    string errorMsg = FormatMessage($"TaskMotorsPolling aex: {aex.Flatten().Message}");
                    Logger?.Error(errorMsg);
                    RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                }
                catch (Exception Ex)
                {
                    string errorMsg = FormatMessage($"TaskMotorsPolling : {Ex.Message}");
                    Logger?.Error(errorMsg);
                    RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                }
                finally
                {
                    await Task.Delay(100);
                }
            }
        }

        private MaterialPresence ReadMaterialPresence()
        {
            if (!_isWaferPresenceDefined)
            {
                return MaterialPresence.Unknown;
            }
            try
            {
                lock (_channelSync)
                {
                    int waferPresent = (int)Channel.ReadVariable(Status.WaferPresence);
                    // Now
                    //1 => wafer is not present.
                    //0 => wafer present
                    if (waferPresent == 0)
                    {
                        return MaterialPresence.Present;
                    }
                    return MaterialPresence.NotPresent;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ReadMaterialPresence - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ReadMaterialPresence - Exception: " + Ex.Message));
                throw;
            }
        }


        private void CheckWaferPresenceStateAndNotifyChanges()
        {            
            var presenceRead = ReadMaterialPresence();            
            if (_lastWaferPresenceState != presenceRead)
            {
                _lastWaferPresenceState = presenceRead;
                Messenger.Send(new WaferPresenceMessage { Diameter = _singleSizeSupported, WaferPresence = _lastWaferPresenceState });
            }
        }

        private void AskTaskResetAxis()
        {
            if (!_taskResetAxisIsRunning)
            {
                _taskResetAxisIsRunning = true;

                Task.Run(async () =>
                {
                    Logger?.Debug("Reset Axis request started");

                    string mess = $"Axis blocked. A complete initialization is required";
                    _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.ErrorHandling, new Message(MessageLevel.Error, mess)));

                    bool askForResetAxis = true;
                    bool axisBlocked = true;
                    while (askForResetAxis || axisBlocked)
                    {
                        // PMGlobalStates != ErrorHandling means that the error has been addressed by user
                        if (_globalStatusServer.GetGlobalState() != PMGlobalStates.ErrorHandling)
                        {
                            askForResetAxis = false;
                        }
                        else
                        {
                            // We don't want to change the PMGlobalStates since we already sent one time the
                            // ErrorHandling PMGlobalStates. So, we only send the message and wait a user
                            // interaction. The error message will be sent until the user handle it.
                            _globalStatusServer.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Error, mess)));
                        }
                        await Task.Delay(1000);

                        axisBlocked = AxisBlocked(); // refresh axis blocked status 
                    }

                    Logger?.Debug("Reset Axis request has been handled");

                    _taskResetAxisIsRunning = false;
                });
            }
            else
            {
                Logger?.Warning($"AskTaskResetAxis is still running");
            }
        }

        public async Task StartRefreshIOStatesTask()
        {
            CheckControllerCommunication();
            // terminate any previous pending tasks
            _cancelationTokenForIOsTask?.Cancel();
            _cancelationTokenForIOsTask = new CancellationTokenSource();
            Logger.Debug("Terminated StartRefreshIOStatesTask");
            await RefreshIOStatesAsync();
        }

        private async Task RefreshIOStatesAsync()
        {
            // keep tokesource before change
            var cancelToken = _cancelationTokenForIOsTask.Token;

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(250, cancelToken);

                    foreach (var input in NameToInput)
                    {
                        if (cancelToken.IsCancellationRequested)
                            break;

                        if (!input.Value.IsEnabled)
                            continue; // do not read disabled IO

                        if (input.Value is DigitalInput digitalInput)
                        {
                            if (_digitalIOStates.ContainsKey(input.Key))
                            {
                                bool currentState = await Task.Run(() => DigitalRead(digitalInput));

                                if (currentState != _digitalIOStates[input.Key])
                                {
                                    _digitalIOStates[input.Key] = currentState;
                                    UpdateDigitalAttribute(digitalInput, currentState);
                                }
                            }
                        }
                        else if (input.Value is AnalogInput analogInput)
                        {
                            if (_analogIOStates.ContainsKey(input.Key))
                            {
                                double currentValue = AnalogRead(analogInput);
                                double epsilon = 0.1;
                                double difference = Math.Abs(currentValue - _analogIOStates[input.Key]);
                                if (difference >= epsilon)
                                {
                                    _analogIOStates[input.Key] = currentValue;
                                    UpdateAnalogAttribute(analogInput, currentValue);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // could be due token cancellation in Task.delay
            }
            finally
            {
                Logger.Debug("[ACSController] Terminated StopRefreshIOStatesTask");
            }
        }

        private void UpdateDigitalAttribute(DigitalInput digitalInput, bool currentState)
        {
            var dataAttributeValue = new DataAttribute(digitalInput.Name, AttributeType.DigitalIO, "DI", DeviceID, digitalInput.Address.Module, digitalInput.Address.Channel);
            dataAttributeValue.Changed = true;
            dataAttributeValue.DigitalValue = currentState;
            //Notify the service via the callBack function that the IO has been changed
            Messenger.Send(new DataAttributesControllerMessage()
            {
                DataAttributes = new List<DataAttribute> { dataAttributeValue }
            });
        }

        private void UpdateAnalogAttribute(AnalogInput analogInput, double currentValue)
        {
            var dataAttributeValue = new DataAttribute(analogInput.Name, AttributeType.AnalogicIO, "AI", DeviceID, analogInput.Address.Module, analogInput.Address.Channel);
            dataAttributeValue.Changed = true;
            dataAttributeValue.AnalogValue = currentValue;
            //Notify the service via the callBack function that the IO has been changed
            Messenger.Send(new DataAttributesControllerMessage()
            {
                DataAttributes = new List<DataAttribute> { dataAttributeValue }
            });
        }

        public IAxis GetAxisFromAxisID(String axisID)
        {
            IAxis axis = ControlledAxesList.Find(a => a.AxisID == axisID);
            if (axis is null)
                throw new Exception(axisID + " axis not found in configuration");
            return axis;
        }

        private void RaiseEventIfPositionChanged()
        {
            RefreshCurrentPos(ControlledAxesList);

            bool positionChanged = ACSAxes.Any(axis => !axis.CurrentPos.Near(axis.LastNotifiedPosition, axis.DistanceThresholdForNotification));
            if (positionChanged)
            {
                double xPos = GetAxisFromAxisID("X").CurrentPos.Millimeters;
                double yPos = GetAxisFromAxisID("Y").CurrentPos.Millimeters;
                double zTopPos = GetAxisFromAxisID("ZTop").CurrentPos.Millimeters;
                double zBottomPos = GetAxisFromAxisID("ZBottom").CurrentPos.Millimeters;
                RaisePositionChangedEvent(new XYZTopZBottomPosition(new MotorReferential(), xPos, yPos, zTopPos, zBottomPos));

                ACSAxes.ForEach(axis => axis.LastNotifiedPosition = axis.CurrentPos);
            }
        }

        private void StartMotorsPolling()
        {
            CheckControllerCommunication();

            // Always try to stop the pollTask before run a new one
            Terminate();

            _cancelationToken = new CancellationTokenSource();
            _pollTask = Task.Run(async () =>
            {
                try
                {
                    _pollTask = TaskControllerPollingAsync();
                    await _pollTask;
                }
                catch (AxisSafetyException)
                {
                    _cancelationToken.Cancel();
                    Logger.Warning("Close communication ACS controller TaskMotorPolling");
                    Channel?.CloseMessageBuffer();
                    _globalStatusServer.SetGlobalState(PMGlobalStates.Error);
                }
                catch (SafetyException)
                {
                    _cancelationToken.Cancel();
                    Logger.Warning("Close communication ACS controller TaskMotorPolling");
                    Channel?.CloseMessageBuffer();
                    _globalStatusServer.SetGlobalState(PMGlobalStates.Error);
                }
            }, _cancelationToken.Token);
        }

        private void OpenMessageBuffer()
        {
            try
            {
                lock (_channelSync)
                {
                    Logger.Information("Open communication ACS controller TaskMotorPolling");
                    Channel?.CloseMessageBuffer();
                    Channel.OpenMessageBuffer(MessageBufferSize);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("OpenMessageBuffer - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("OpenMessageBuffer - Exception: " + Ex.Message));
                throw;
            }
        }

        private string FindValveName(double waferSize)
        {
            foreach (var clamp in _controllerConfig.WaferClampList)
            {
                if (clamp.Available && (clamp.WaferSize.Millimeters == waferSize))
                {
                    return clamp.ValveName;
                }
            }
            string message = $"No valve available in the configuration for wafer : {waferSize}.";
            Logger.Error(message);
            throw new Exception(message);
        }

        public void ClampWafer(Length size)
        {
            CheckControllerCommunication();

            try
            {
                if (RefreshIsWaferClamped() == true)
                {
                    Logger.Information("The wafer cannot be clamped");
                    return;
                }

                string valveName = FindValveName(size.Millimeters);
                Channel.WriteVariable(Actions.ActionWaferClamp, valveName);

                bool Success = SpinWait.SpinUntil(() =>
                {
                    return RefreshIsWaferClamped() == true;
                }
                , Timeouts.TimeoutClampWafer);
                if (!Success)
                {
                    Channel.WriteVariable(Actions.ActionWaferRelease, valveName);
                    string message = FormatMessage("Clamp wafer Failed");
                    RaiseErrorEvent(new Message(MessageLevel.Error, message, message, DeviceID));
                }
                RefreshAndRaiseStateEventsForAxesAndWaferClamp(forceRefresh: true);
                Logger.Information("wafer is clamped");
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ClampWafer - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ClampWafer - Exception: " + Ex.Message));
                throw;
            }
        }

        public void ReleaseWafer(Length size)
        {
            CheckControllerCommunication();

            try
            {
                if (RefreshIsWaferClamped() == false)
                    return;

                string valveName = FindValveName(size.Millimeters);
                Channel.WriteVariable(Actions.ActionWaferRelease, valveName);
                WaitForWaferRelease();
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ReleaseWafer - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ReleaseWafer - Exception: " + Ex.Message));
                throw;
            }
        }

        private void WaitForWaferRelease()
        {
            CheckControllerCommunication();

            bool succes = SpinWait.SpinUntil(() => RefreshIsWaferClamped(), Timeouts.TimeoutClampWafer);

            RefreshAndRaiseStateEventsForAxesAndWaferClamp(forceRefresh: true);

            if (!succes)
            {
                string message = FormatMessage("Release wafer Failed");
                Logger.Error(message);
                RaiseErrorEvent(new Message(MessageLevel.Error, message, message, DeviceID));
                return;
            }
            Logger.Information(FormatMessage("Wafer is Released"));
        }

        private void TerminateOtherProcess()
        {
            string ProcessName = Process.GetCurrentProcess().ProcessName;
            ACSC_CONNECTION_DESC[] connectionList = Channel.GetConnectionsList();
            for (int index = 0; index < connectionList.Length; index++)
            {
                if (connectionList[index].Application.Contains(ProcessName))
                {
                    Channel.TerminateConnection(connectionList[index]);
                }
            }
        }

        private void OpenEthernetConnection()
        {
            try
            {
                string ethernetIP = _controllerConfig.EthernetCom != null && !string.IsNullOrWhiteSpace(_controllerConfig.EthernetCom.IP) ? _controllerConfig.EthernetCom.IP : string.Empty;
                int ethernetPort = _controllerConfig.EthernetCom != null ? _controllerConfig.EthernetCom.Port : 0;
                lock (_channelSync)
                {
                    Channel = new Api();
                }

                CheckControllerCommunication();

                if (string.IsNullOrEmpty(ethernetIP))
                    throw (new Exception(FormatMessage("String ethernetIP is empty")));

                if (ethernetPort == 0)
                    throw (new Exception(FormatMessage("ethernetPort is empty")));

                lock (_channelSync)
                {
                    TerminateOtherProcess();
                    Channel.OpenCommEthernetTCP(ethernetIP, ethernetPort);
                    Channel.SetTimeout(25000);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("OpenEthernetConnection - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("OpenEthernetConnection - Exception: " + Ex.Message));
                throw;
            }
        }

        private void ChangeSpeedIfDifferent(IAxis axis, double speed, double accel)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();
            ACSAxis acsAxis = axis as ACSAxis;
            try
            {
                lock (Channel)
                {
                    double velocityACS = Channel.GetVelocity(ACSID[acsAxis.AxisID]);
                    double accelerationACS = Channel.GetAcceleration(ACSID[acsAxis.AxisID]);

                    if ((velocityACS == speed) && (accelerationACS == accel))
                    {
                        return;
                    }

                    acsAxis.ACSAxisConfig.CurrentSpeed = speed;
                    acsAxis.ACSAxisConfig.CurrentAccel = accel;
                    Channel.SetVelocity(ACSID[acsAxis.AxisID], acsAxis.ACSAxisConfig.CurrentSpeed);
                    Channel.SetAcceleration(ACSID[acsAxis.AxisID], acsAxis.ACSAxisConfig.CurrentAccel);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("ChangeSpeedIfDifferent - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("ChangeSpeedIfDifferent - Exception: " + Ex.Message));
                throw;
            }
        }

        private bool IsTargetReached(List<IAxis> axesList, List<double> target)
        {
            int index = 0;

            RefreshCurrentPos(axesList);
            foreach (ACSAxis axis in axesList)
            {
                if (!axis.CurrentPos.Near(target[index].Millimeters(), axis.DistanceThresholdForNotification))
                {
                    return false;
                }

                index++;
            }
            return true;
        }

        private void TaskWaitMotionEnd(List<IAxis> axisList, List<double> target)
        {
            try
            {
                WaitMotionEnd(TimeoutWaitMotionEnd);
            }
            catch (Exception ex)
            {
                string errorMsg = FormatMessage($" TaskWaitMotionEnd Failed - {ex.Message}");
                Logger?.Error(errorMsg);
                RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                throw;
            }
            finally
            {
                bool targetReached = false;
                lock (_channelSync)
                {
                    targetReached = IsTargetReached(axisList, target);
                }

                RaiseMotionEndEvent(targetReached);
                if (!targetReached)
                {
                    string errorMsg = FormatMessage($" TaskWaitMotionEnd - Target position not reached");
                    // It is not an error if the user stopped the move
                    //RaiseErrorEvent(new Message(MessageLevel.Information, errorMsg, "", DeviceName));
                }

                double xPos = GetAxisFromAxisID("X").CurrentPos.Millimeters;
                double yPos = GetAxisFromAxisID("Y").CurrentPos.Millimeters;
                double zTopPos = GetAxisFromAxisID("ZTop").CurrentPos.Millimeters;
                double zBottomPos = GetAxisFromAxisID("ZBottom").CurrentPos.Millimeters;
                RaisePositionChangedEvent(new XYZTopZBottomPosition(new MotorReferential(), xPos, yPos, zTopPos, zBottomPos));
            }
        }

        private string FormatMessage(string message)
        {
            return ($"[{DeviceID}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }

        #region IChuck

        public override void Init(List<Message> errorList)
        {
            try
            {
                Connect();
                InitializeInput();

                InitControllerAxes(errorList);
            }
            catch (SafetyException Ex)
            {
                throw Ex;
            }
            catch (AxisSafetyException Ex)
            {
                throw Ex;
            }
            catch (Exception Ex)
            {
                errorList.Add(new Message(MessageLevel.Error, "Connection failed : " + Ex.Message, DeviceID));
            }
        }

        public void InitializeUpdate()
        {
            throw new NotImplementedException();
        }

        public override bool IsLanded()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    _isLanded = ((int)Channel.ReadVariable(Status.Land) != 0);
                    return _isLanded;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Read AirBearingStatus - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Read AirBearingStatus - Exception: " + Ex.Message));
                throw;
            }
        }

        public MaterialPresence CheckWaferPresence(Length size)
        {
            if (size.Millimeters != _singleSizeSupported.Millimeters)
                return MaterialPresence.Unknown;

            CheckControllerCommunication();
            try
            {
                var presenceRead = ReadMaterialPresence();
                return presenceRead;
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Check WaferPresence - ACSException: " + ACSEx.Message));
                return MaterialPresence.Unknown;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Check WaferPresence - Exception: " + Ex.Message + Ex.StackTrace));
                return MaterialPresence.Unknown;
            }
        }

        public bool RefreshIsWaferClamped()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    return ((int)Channel.ReadVariable(Status.Clamp) != 0);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("RefreshWaferClampState - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("RefreshWaferClampState - Exception: " + Ex.Message));
                throw;
            }
        }

        public void SetError(string message)
        {
            throw new NotImplementedException();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            foreach (var axis in axesList)
            {
                if (!(axis is ACSAxis))
                {
                    // Error
                    String errorMsg = "An axis type is not ACSAxis";
                    Logger?.Error(errorMsg);
                    throw (new Exception(errorMsg));
                }
            }
            return true;
        }

        public ChuckState GetState()
        {
            bool waferClampState = false;

            CheckControllerCommunication();
            waferClampState = RefreshIsWaferClamped();

            return CreateChuckState(waferClampState, waferClampState ? MaterialPresence.Present : MaterialPresence.NotPresent); // wafer presense = wafer clamped in ACS controller
        }

        public bool IsChuckStateChangedEventSet { get => (ChuckStateChangedEvent != null); }
      
        protected void RaiseStateChangedEvent(ChuckState chuckState)
        {
            ChuckStateChangedEvent?.Invoke(chuckState);
        }

        #endregion IChuck

        #region IO

        public void SetFFUValue(int value)
        {
            try
            {
                lock (_channelSync)
                {
                    //if 1 off
                    //if 0 on
                    Channel.WriteVariable(value, Commands.FFUPower);
                    _ffuOn = (value == Actions.ActionTurnOnFFU);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("RefreshWaferClampState - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("RefreshWaferClampState - Exception: " + Ex.Message));
                throw;
            }
        }

        private bool ReadVariable(string variable)
        {
            try
            {
                lock (_channelSync)
                {
                    int readedValue = (int)Channel.ReadVariable(variable);
                    if (readedValue == 0)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage($"ReadVariable-<{variable}> - ACSException: {ACSEx.Message}"));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage($"ReadVariable-<{variable}> - Exception: {Ex.Message}"));
                throw;
            }
        }

        private double ReadAnalogVariable(string variable)
        {
            try
            {
                lock (_channelSync)
                {
                    double analogvalue = (double)Channel.ReadVariable(variable);
                    return analogvalue;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage($"ReadAnalogVariable-<{variable}> - ACSException: {ACSEx.Message}"));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage($"ReadAnalogVariable-<{variable}> - Exception: {Ex.Message}"));
                throw;
            }
        }

        public bool GetEMOPushValue()
        {
            CheckControllerCommunication();

            return ReadVariable(Status.EMOPushed);
        }

        public bool GetPrepareToTransfertValue()
        {
            CheckControllerCommunication();

            return ReadVariable(Status.PrepareToTransfert);
        }

        public bool GetRobotIsOutValue()
        {
            CheckControllerCommunication();

            return ReadVariable(Status.RobotIsOutService);
        }

        public Input GetInput(string name)
        {
            if (!NameToInput.ContainsKey(name))
            {
                Logger.Error($"<{name}> is not known IO in the configuration. Check in the AnaHardwareConfiguration file.");
            }
            return NameToInput[name];
        }

        public Output GetOutput(string name)
        {
            if (NameToOutput.ContainsKey(name))
            {
                return NameToOutput[name];
            }
            throw new Exception(
                $"A device tried to get an Output named: {name} but it does not exist in the {ControllerConfiguration.Name} controller configuration");
        }

        public double AnalogRead(AnalogInput input)
        {
            try
            {
                lock (_channelSync)
                {
                    CheckControllerCommunication();
                    return ReadAnalogVariable(input.CommandName);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error($"[ACSController] Error during Analog read variable  <{input.CommandName}> {ACSEx.Message}");
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error($"[ACSController] Error during Analog read variable <{input.CommandName}> {Ex.Message}");
                throw;
            }
        }

        public void AnalogWrite(AnalogOutput output, double value)
        {
            //For the moment no analogic values are read
            throw new NotImplementedException();
        }

        public bool DigitalRead(DigitalInput input)
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    bool state = ReadVariable(input.CommandName);
                    if (input.CommandName == Status.Land)
                    {
                        _isLanded = state;
                    }
                    return state;
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error($"[ACSController] Error during Digital read variable <{input.CommandName}> {ACSEx.Message}");
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error($"[ACSController] Error during Digital read variable <{input.CommandName}> {Ex.Message}");
                throw;
            }
        }

        public void DigitalWrite(DigitalOutput output, bool value)
        {
            try
            {
                lock (_channelSync)
                {
                    Channel.WriteVariable(value, output.CommandName);
                    //Notify the service via the callBack function that the IO has been changed
                    bool editedValue = ReadVariable(output.CommandName);
                    var dataAttributeValue = new DataAttribute(output.Name, AttributeType.DigitalIO, "DO", DeviceID, output.Address.Module, output.Address.Channel);
                    dataAttributeValue.Changed = editedValue != value;
                    dataAttributeValue.DigitalValue = value;
                    if (dataAttributeValue.Changed)
                    {
                        Messenger.Send(new DataAttributesControllerMessage()
                        {
                            DataAttributes = new List<DataAttribute> { dataAttributeValue }
                        });
                    }
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage($"{output.Name} - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage($"{output.Name} - Exception: " + Ex.Message));
                throw;
            }
        }

        public void StopRefreshIOStatesTask()
        {
            _cancelationTokenForIOsTask?.Cancel();
            Logger.Debug("Terminated StopRefreshIOStatesTask");
        }

        public void ManagePrincipalChamberLight(bool value)
        {
            //Controls the general lighting in the chamber
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    Logger.Information("Active service light");
                    int action = value ? 1 : 0; // 1 = Turn on, 0 = Turn off
                    Channel.WriteVariable(action, Commands.ServiceLight);
                    Logger.Information("Action : Service light - Successfully completed");
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("Service light - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("Service Light - Exception: " + Ex.Message));
                throw;
            }
        }

        private void CheckIfAckAlarmIsPresentInConfigurationFile()
        {
            if (_controllerConfig.IOList.OfType<DigitalInput>()
                                .Any(input => input.CommandName == Status.AcknowledgeAlarm && input.IsEnabled))
            {
                _isAckModeIOAvailable = true;
            }
            else
            {
                Logger.Debug("The IO AckAlarm is not known or is not activated in the configuration file");
            }
        }

        private void CheckToolModeIsPresentInConfigurationFile()
        {
            if (_controllerConfig.IOList.OfType<DigitalInput>()
                                .Any(input => input.CommandName == Status.ServiceMode && input.IsEnabled))
            {
                _isToolModeIOAvailable = true;
            }
            else
            {
                Logger.Debug("The IO serviceMode is not known or is not activated in the configuration file");
            }
        }

        public void RaiseEventIfToolModeChanged()
        {
            CheckControllerCommunication();
            try
            {
                if (CheckIfToolModeStateHasChanged())
                {
                    _globalStatusServer.SetToolModeStatus(_toolModeState);
                }
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage(" RaiseEventIfToolModeChanged- ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("RaiseEventIfToolModeChanged - Exception: " + Ex.Message));
                throw;
            }
        }

        private bool CheckIfToolModeStateHasChanged()
        {
            var newToolMode = GetToolMode();
            if (_toolModeState != newToolMode)
            {
                _toolModeState = newToolMode;
                return true;
            }
            return false;
        }

        private ToolMode GetToolMode()
        {
            lock (_channelSync)
            {
                var newToolMode = ToolMode.Unknown;
                if (_isAckModeIOAvailable)
                {
                    bool ackAlarm = ((int)Channel.ReadVariable(Status.AcknowledgeAlarm) != 1);
                    if (ackAlarm)
                    {
                        newToolMode = ToolMode.AcknowledgeAlarm;
                        return newToolMode;
                    }
                }

                if (_isToolModeIOAvailable)
                {
                    int serviceMode = (int)Channel.ReadVariable(Status.ServiceMode);
                    switch (serviceMode)
                    {
                        case 0:
                            newToolMode = ToolMode.Run;
                            break;

                        case 1:
                            newToolMode = ToolMode.Maintenance;
                            break;

                        default:
                            newToolMode = ToolMode.Unknown;
                            break;
                    }
                }
                return newToolMode;
            }
        }

        public void TurnOnFFU()
        {
            SetFFUValue(Actions.ActionTurnOnFFU);
        }

        public void TurnOffFFU()
        {
            SetFFUValue(Actions.ActionTurnOffFFU);
        }

        public bool GetFFUErrorState()
        {
            throw new NotImplementedException("Get FFU error state not implemented");
        }

        public bool FFUState()
        {
            return _ffuOn;
        }

        #endregion IO
    }

    #endregion Private/Protected methods
}
