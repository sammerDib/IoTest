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
    public class ACSDummyController : MotorController, IChuckController, IControllerIO, IDisposable, IChamberController
    {
        #region Fields

        private Length _signleSizeSupported = new Length(300, LengthUnit.Millimeter);
        public new String DeviceID => _controllerConfig?.DeviceID;
        private Task _pollTask;
        private const double DelayBeforeResetCtrl = 4000;
        private ACSControllerConfig _controllerConfig;
        private Dictionary<String, Axis> _axesIDLinks = new Dictionary<string, Axis>();
        private bool _isLanded;
        private bool _landingInProgress;
        private bool _waferClampState;
        protected Task HandleTaskWaitMotionEnd;

        public Dictionary<string, Input> NameToInput { get; set; }
        public Dictionary<string, Output> NameToOutput { get; set; }
        private Dictionary<string, bool> _digitalIOStates;
        private Dictionary<string, double> _analogIOStates;
        private IGlobalStatusServer _globalStatusServer;
        private CancellationTokenSource _cancelationTokenForIOsTask;

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

        private ControllerBuffer[] _embeddedProgramBuffers = new ControllerBuffer[] {
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
        private const int SleepForMoveToStart = 250;    // ms
        private const int TimeoutWaitMotionEnd = 20000; // ms

        private struct Timeouts
        {
            public const int TimeoutEnableMotor = 2000;        // ms
            public const int TimeoutLandAirBearing = 5000;     // ms
            public const int TimeoutInitFocus = 60000;         // ms
            public const int TimeoutResetFault = 30000;        // ms
            public const int TimeoutInitProcess = 5000;        // ms
            public const int TimeoutInitTableAxes = 120000;    // ms
            public const int TimeoutInitAirBearing = 60000;    // ms
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
        }

        private struct Commands
        {
            public const string Reset = "ResetFAULT";
            public const string Land = "LandAirbearing";
            public const string InitProcess = "ACS_Init_Process";
            public const string InitLand = "ACS_Init_Airbearing";
            public const string InitTableAxes = "ACS_Init_WaferStage";
            public const string InitBottomObjective = "ACS_Init_LOH_focus";
            public const string InitTopObjective = "ACS_Init_UOH_focus";
            public const string Clamp = "WaferClamp";
        };

        private struct Status
        {
            public const string Fault = "P_FAULT";
            public const string Land = "LandAirbearingStatus";
            public const string InitProcess = "ACS_Stat_Process";
            public const string InitLand = "ACS_Stat_Airbearing";
            public const string InitTableAxes = "ACS_Stat_WaferStage";
            public const string InitBottomObjective = "ACS_Stat_LOH_focus";
            public const string InitTopObjective = "ACS_Stat_UOH_focus";
            public const string Clamp = "WaferClamp_sensor";
            public const string ServiceMode = "ServiceModeOn";
        };

        protected internal Object Channel;
        private object _channelSync;

        private CancellationTokenSource _cancelationToken;

#pragma warning disable CS0067 //Event used but not detected by compiler

        public event ChuckStateChangedDelegate ChuckStateChangedEvent;

        #endregion Fields

        #region Constructors

        public ACSDummyController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            if (controllerConfig is ACSControllerConfig)
            {
                _controllerConfig = (ACSControllerConfig)controllerConfig;
            }
            else
                throw (new Exception(FormatMessage("Bad controller configuration type. Controller creation failed !")));

            _channelSync = new object();
            Channel = null;
            _landingInProgress = false;

            //Init Input and Output list
            NameToInput = new Dictionary<string, Input>();
            NameToOutput = new Dictionary<string, Output>();
            _digitalIOStates = new Dictionary<string, bool>();
            _analogIOStates = new Dictionary<string, double>();
            _globalStatusServer = globalStatusServer;

            if (controllerConfig is IOControllerConfig ioControllerConfig && ioControllerConfig != null)
            {
                NameToInput = ioControllerConfig.GetInputs();
                NameToOutput = ioControllerConfig.GetOutputs();
            }

            foreach (var acsAxisIDLink in _controllerConfig.ACSAxisIDLinks)
            {
                _axesIDLinks.Add(acsAxisIDLink.AxisID, GetAxisID(acsAxisIDLink.ACSID));
            }
        }

        #endregion Constructors

        #region Properties

        public ANAChuckConfig Configuration { get; set; }
        public Dictionary<String, Axis> ACSID { get => _axesIDLinks; }

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
            if (_cancelationToken != null)
            {
                _cancelationToken.Cancel();
                _pollTask.Wait(Timeouts.TimeoutPollTaskTermiante);
                //_pollTask.WaitEndOfTask(FogEventLog.Orig.Hardware, "Terminate", 100); // TODO (Sogilis): restore
                _cancelationToken = null;
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
                return;
            }
        }

        public override void Disconnect()
        {
            CheckControllerCommunication();
            //Channel.CloseComm(); //Fermeture de la connexion du controlleur
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            CheckControllerCommunication();

            //TerminateOtherProcess(); // TODO (Sogilis): restore ?
            lock (_channelSync)
            {
                try
                {
                    ReadStatus();
                    ResetFaults();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }

                try
                {
                    InitLanding();
                    StopLanding();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Fatal, Ex.Message, DeviceID));
                }

                try
                {
                    InitAllAxisSpeeds(AxisSpeed.Normal);
                    InitStatProcess();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }

                try
                {
                    InitZBottomFocus();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }

                try
                {
                    InitZTopFocus();
                    InitializeInput();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Warning, Ex.Message, DeviceID));
                }

                try
                {
                    InitTableAxes();
                    EnableAxis(ControlledAxesList);
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Error, Ex.Message, DeviceID));
                }
                try
                {
                    StartMotorsPolling();
                }
                catch (Exception Ex)
                {
                    initErrors.Add(new Message(MessageLevel.Error, Ex.Message, DeviceID));
                }
            }
            if (initErrors.Any(message => message.Level == MessageLevel.Fatal) ||
                initErrors.Any(message => message.Level == MessageLevel.Error))
                State = new DeviceState(Service.Interface.DeviceStatus.Error);
            else
                State = new DeviceState(Service.Interface.DeviceStatus.Ready);
        }

        private void InitializeInput()
        {
            foreach (var initialInput in NameToInput)
            {
                if (initialInput.Value is DigitalInput)
                {
                    _digitalIOStates[initialInput.Key] = true;
                }
                if (initialInput.Value is AnalogInput)
                {
                    _analogIOStates[initialInput.Key] = 40.0;
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
                    var PFault = 0;// (int)Channel.ReadVariable(Status.Fault);
                    if (PFault != 0)
                    {
                        //Desactivate(MotionAxisList);
                        ResetFaults();
                        Success = SpinWait.SpinUntil(() =>
                        {
                            PFault = 0; // (int)Channel.ReadVariable(Status.Fault);
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
                //lock (_channelSync)
                //{
                //    if (IsAirbearingActive() == false)
                //        return;

                //   // Channel.WriteVariable(Actions.ActionLStopLanding, Commands.Land);
                //    bool Success = SpinWait.SpinUntil(() =>
                //    {
                //        return IsAirbearingActive() == false;
                //    }
                //   , Timeouts.TimeoutLandAirBearing);
                //    if (!Success)
                //        throw (new Exception(FormatMessage("StartAirBearing - Start air bearing failed")));
                //}
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("StartAirBearing - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("StartAirBearing - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void Land()
        {
            _landingInProgress = true;
            CheckControllerCommunication();

            try
            {
                //if (IsAirbearingActive() == true)
                //    return;

                //// Motors using air bearing are disabled in controller embedded program
                //// Moves are freezed by conroller embedded program before stopping air bearing
                //Channel.WriteVariable(Actions.ActionLand, Commands.Land); // Land (= stop air bearing)
                //bool Success = SpinWait.SpinUntil(() =>
                //{
                //    return IsAirbearingActive() == true;
                //}
                //, Timeouts.TimeoutLandAirBearing);
                //if (!Success)
                //{
                //    throw (new Exception(FormatMessage("Land Failed")));
                //}
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
                            axis.CurrentPos = 0.Millimeters(); // (Channel.GetFPosition(ACSID[axis.AxisID]) - axisConfig.PositionZero) * axisConfig.MotorDirection; //Récupère la position de l'axe
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
                    var position = 0; // (Channel.GetFPosition(ACSID[axis.AxisID]) - axisConfig.PositionZero) * axisConfig.MotorDirection; //Récupère la position de l'axe axis
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
                    if (IsLanded() == true)
                        throw (new Exception(FormatMessage("Move impossible without airbearing!")));

                    if (axis.AxisConfiguration is ACSAxisConfig)
                    {
                        var nextPosition = axis.CurrentPos.Millimeters + stepMillimeters;
                        CheckAxisLimits(axis, nextPosition);
                        //Channel.ToPoint(MotionFlags.ACSC_AMF_RELATIVE, ACSID[axis.AxisID], stepMillimeters * ((ACSAxisConfig)axis.AxisConfiguration).MotorDirection);
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

            try
            {
                lock (_channelSync)
                {
                    int indexPos = 0;
                    var PosList = new List<double>();
                    var AxisIndexList = new List<Axis>();
                    foreach (IAxis axis in axesList)
                    {
                        if (axis.AxisConfiguration is ACSAxisConfig)
                        {
                            var acsAxisConfig = (ACSAxisConfig)axis.AxisConfiguration;
                            double Position = commandCoords[indexPos] + acsAxisConfig.PositionZero.Millimeters;
                            CheckAxisLimits(axis, Position);
                            PosList.Add(Position * acsAxisConfig.MotorDirection);
                            AxisIndexList.Add(ACSID[axis.AxisID]);
                            indexPos++;
                        }
                        else
                            throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
                    }
                    AxisIndexList.Add(ACS.SPiiPlusNET.Axis.ACSC_NONE);
                    PosList.Add(-1);

                    SetSpeedAccelAxis(axesList, speeds, accels);

                    if (IsLanded() == true)
                        throw (new Exception(FormatMessage("Move impossible without airbearing!")));

                    //_channel.ToPointM(MotionFlags.ACSC_NONE, AxisIndexList.ToArray(), PosList.ToArray()); // "MotionFlags.ACSC_NONE" is required for absolute moves
                    //  "MotionFlags.ACSC_AMF_MAXIMUM" : Multi-axis motion does not use the motion parameters from the leading axis but calculates the maximum allowed motion velocity, acceleration, deceleration and jerk of the involved axes
                    // Channel.ToPointM(MotionFlags.ACSC_AMF_MAXIMUM, AxisIndexList.ToArray(), PosList.ToArray());

                    HandleTaskWaitMotionEnd = new Task(() => { TaskWaitMotionEnd(axesList, commandCoords); }, TaskCreationOptions.LongRunning);
                    HandleTaskWaitMotionEnd.Start();
                }
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

        public override void SetPosAxis(List<double> coordsList, List<IAxis> axesList, List<AxisSpeed> speedsList)
        {
            int index = 0;
            var speedsConvertedToDouble = new List<double>();
            var accelsConvertedToDouble = new List<double>();
            foreach (IAxis axis in axesList)
            {
                ConvertSpeedEnum(axis, speedsList[index], out double speed, out double accel);
                speedsConvertedToDouble.Add(speed);
                accelsConvertedToDouble.Add(accel);
                index++;
            }

            SetPosAxisWithSpeedAndAccel(coordsList, axesList, speedsConvertedToDouble, accelsConvertedToDouble);
        }

        public void Dispose()
        {
            CheckControllerCommunication();

            try
            {
                //Channel.CloseComm(); //Fermeture de la connexion du controlleur
                Channel = null;
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
                    RefreshAxisState(axis);

                    if (!axis.Enabled)
                        throw (new Exception(FormatMessage("Activation of Axis" + axis.AxisID + " Timeout Error ")));
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
                            //Channel.Disable(ACSID[axisToDisable.AxisID]);
                            //Channel.WaitMotorEnabled(ACSID[axisToDisable.AxisID], disabledState, Timeouts.TimeoutEnableMotor); // Waits end of disabling
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
                    //var AxisIndexList = new List<ACS.SPiiPlusNET.Axis>();
                    //foreach (IAxis axis in ControlledAxesList)
                    //    AxisIndexList.Add(ACSID[axis.AxisID]);

                    //AxisIndexList.Add(Axis.ACSC_NONE);
                    //Channel.HaltM(AxisIndexList.ToArray()); // Stop axis move
                    //Channel.EndSequenceM(AxisIndexList.ToArray());
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
                    //var axisToMoveIdList = new List<ACS.SPiiPlusNET.Axis>();
                    //int index = 0;
                    //var ServiceModeIsEnabled = (int)Channel.ReadVariable(Status.ServiceMode);
                    //double speed = 0, accel = 0;

                    //foreach (ACSAxis acsAxis in axesList)
                    //{
                    //    axisToMoveIdList.Add(ACSID[acsAxis.AxisID]);
                    //    double coord = acsAxis.ComputePositionInControllerFrame(coordsList[index]);
                    //    CheckAxisLimits(acsAxis, coord);
                    //    coordsList[index] = coord * acsAxis.ACSAxisConfig.MotorDirection;
                    //    ConvertSpeedEnum(acsAxis, axisSpeed, out speed, out accel);

                    //    // If service mode enabled, keep the minimal speed service from all axis
                    //    if (ServiceModeIsEnabled != 0)
                    //        if (speed > acsAxis.ACSAxisConfig.MaxSpeedService)
                    //            speed = acsAxis.ACSAxisConfig.MaxSpeedService;

                    //    index++;
                    //}

                    //axisToMoveIdList.Add(ACS.SPiiPlusNET.Axis.ACSC_NONE); // After the last axis, one additional element must be included that contains –1 and marks the end of the array.
                    //coordsList.Add(-1); // The point must specifyavalue for each element of axesexcept the last –1 element.

                    //// MotionFlags.ACSC_AMF_VELOCITY: the motion will use velocity specified by the Velocity argument instead of the default velocity.
                    //// endVelocity specifies speed when reaching target point (move will not stop at target point)
                    //Channel.ExtToPointM(MotionFlags.ACSC_AMF_VELOCITY, axisToMoveIdList.ToArray(), coordsList.ToArray(), speed, speed);
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
                    MotorStates MotorState = MotorStates.ACSC_MST_ENABLE; // Channel.GetMotorState(ACSID[acsAxis.AxisID]);
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

            //lock (_channelSync)
            {
                try
                {
                    Thread.Sleep(SleepForMoveToStart); // Wait for move to start
                    foreach (IAxis axis in ControlledAxesList)
                    {
                        RefreshAxisState(axis);
                        if (axis.Moving == false)
                            continue;

                        var startWaitingTime = DateTime.Now;

                        while ((axis.Moving) && ((DateTime.Now - startWaitingTime).TotalMilliseconds < timeout))
                        {
                            //Channel.WaitMotionEnd(ACSID[axis.AxisID], timeout);
                            RefreshAxisState(axis);
                            Thread.Sleep(10);
                        }
                    }
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
        }

        #endregion Public methods

        #region Private/Protected methods

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();
            ACSAxis acsAxis = axis as ACSAxis;
            //lock (_channelSync)
            //{
            //    try
            //    {
            //        var ServiceModeIsEnabled = (int)Channel.ReadVariable(Status.ServiceMode);
            //        if (ServiceModeIsEnabled == 0)
            //            return;

            //        if (speed > acsAxis.ACSAxisConfig.MaxSpeedService)
            //            speed = acsAxis.ACSAxisConfig.MaxSpeedService;
            //    }
            //    catch (ACSException ACSEx)
            //    {
            //        Logger?.Error(FormatMessage("CheckServiceSpeed - ACSException: " + ACSEx.Message));
            //        throw;
            //    }
            //    catch (Exception Ex)
            //    {
            //        Logger?.Error(FormatMessage("CheckServiceSpeed - Exception: " + Ex.Message));
            //        throw;
            //    }
            //}
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            // No need  - CheckAxesListIsValid(axisList);
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

        public override void CheckControllerCommunication()
        {
            if (Channel != null)
                return;

            string errorMsg = FormatMessage("Communication channel to ACS is null");
            Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        private void InitStatProcess()
        {
            CheckControllerCommunication();

            try
            {
                //lock (_channelSync)
                //{
                //    var Stat_Init_Process = (int)Channel.ReadVariable(Status.InitProcess);
                //    bool Success = true;
                //    if (Stat_Init_Process != 1)
                //    {
                //        Channel.WriteVariable(Actions.ActionStartInit, Commands.InitProcess);
                //        Success = SpinWait.SpinUntil(() =>
                //        {
                //            Stat_Init_Process = (int)Channel.ReadVariable(Status.InitProcess);
                //            return Stat_Init_Process == 1;
                //        }, Timeouts.TimeoutInitProcess
                //        );
                //    }
                //    if (!Success)
                //        throw (new Exception(FormatMessage("Failed to set ACS_Stat_Process")));
                //}
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitStatProcess - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitStatProcess - Exception: " + Ex.Message));
                throw;
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

        private void InitLanding()
        {
            CheckControllerCommunication();

            try
            {
                //lock (_channelSync)
                //{
                //    bool Success = true;
                //    var Stat_Airbearing = (int)Channel.ReadVariable(Status.InitLand);
                //    if (Stat_Airbearing != 1)
                //    {
                //        System.Threading.Thread.Sleep(1000); //Wait to be sure that Init process script is finished.
                //        Channel.WriteVariable(Actions.ActionStartInit, Commands.InitLand);
                //        Success = SpinWait.SpinUntil(() =>
                //        {
                //            Stat_Airbearing = (int)Channel.ReadVariable(Status.InitLand);
                //            return Stat_Airbearing == 1;
                //        }, Timeouts.TimeoutInitAirBearing
                //        );
                //    }
                //    if (!Success)
                //        throw (new Exception(FormatMessage("Failed to set ACS_Stat_Airbearing")));
                //}
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitAirBearing - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitAirBearing - Exception: " + Ex.Message));
                throw;
            }
        }

        private void InitTableAxes()
        {
            CheckControllerCommunication();

            try
            {
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("InitWaferStage - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitWaferStage - Exception: " + Ex.Message));
                throw;
            }
        }

        private void ReadStatus()
        {
            CheckControllerCommunication();

            try
            {
                //lock (_channelSync)
                //{
                //    var Landing = (int)Channel.ReadVariable(Status.Land);
                //    string message = FormatMessage("LandAirbearingStatus: " + (Landing == 1 ? "LANDED" : "NOT LANDED"));
                //    Console.WriteLine(message);
                //    Logger.Information(message);

                //    var Stat_Init_Process = (int)Channel.ReadVariable(Status.InitProcess);
                //    message = FormatMessage("ACS_Stat_Process: " + (Stat_Init_Process == 1 ? "Process functions initialized" : "Process functions NOT initialized"));
                //    Console.WriteLine(message);
                //    Logger.Information(message);

                //    var ServiceMode = (int)Channel.ReadVariable(Status.ServiceMode);
                //    Console.WriteLine(FormatMessage("ServiceMode: " + (ServiceMode == 1 ? "ON" : "OFF")));
                //    Console.WriteLine(message);
                //    Logger.Information(message);
                //}
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
            //int timeout = 0;
            // try to get messages that the controller sends on its own initiative and not as a response to a command.
            try
            {
                //lock (_channelSync)
                //{
                //    string MsgFromController = Channel.GetSingleMessage(timeout);
                //    if ((!string.IsNullOrWhiteSpace(MsgFromController))
                //        && (MsgFromController.CompareTo(previousMsg) != 0))
                //    {
                //        Logger?.Information((FormatMessage("TryGetMessageFromController - Message: " + MsgFromController)));
                //        previousMsg = MsgFromController;
                //    }
                //}
            }
            catch (ACSException ACSEx)
            {
                if (ACSEx.ErrorCode == (int)ACS.SPiiPlusNET.ErrorCodes.ACSC_TIMEOUT) // Timeout due to no message to read, so it is OK
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

        private bool GetCustomFaults()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    return false;
                }
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

        private bool GetProgramFaults()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    bool isInError = false;

                    // Get program faults
                    //for (int indice = 0; indice < _embeddedProgramBuffers.Length; indice++)
                    //{
                    //    int programFault = Channel.GetProgramError(_embeddedProgramBuffers[indice].ProgramBufferId);
                    //    if ((programFault != 0) && (programFault != _embeddedProgramBuffers[indice].ProgramFault))
                    //    {
                    //        isInError = true;
                    //        string errorMsg = FormatMessage("GetProgramFaults - Buffer " + _embeddedProgramBuffers[indice].ProgramBufferId + " Error " + programFault + " " + Channel.GetErrorString(programFault));
                    //        Logger?.Error(errorMsg);
                    //        RaiseErrorEvent(new Message(MessageLevel.Warning, errorMsg, errorMsg, DeviceID));
                    //    }
                    //    _embeddedProgramBuffers[indice].ProgramFault = programFault;
                    //}
                    return isInError;
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

        private bool GetAxisErrors()
        {
            CheckControllerCommunication();
            SafetyControlMasks ACSAxisFault = SafetyControlMasks.ACSC_NONE;
            try
            {
                lock (_channelSync)
                {
                    bool isInError = false;

                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        // Get motors or system faults. Use ACSC_SAFETY_*** mask to determine error
                        ACSAxisFault = SafetyControlMasks.ACSC_NONE; //  Channel.GetFault(ACSID[axis.AxisID]);
                        if (ACSAxisFault != SafetyControlMasks.ACSC_NONE)
                        {
                            isInError = true;
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
                    return isInError;
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

        private bool GetMotorsErrors()
        {
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    bool isInError = false;
                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        // Get reason for motor disabling
                        int motorFault = 0; // Channel.GetMotorError(ACSID[axis.AxisID]);
                        if (motorFault != 0)
                        {
                            isInError = true;
                            if (axis.MotorError != motorFault)
                            {
                                string message = FormatMessage("GetAxisFaults - motorFault: " + motorFault + " "); // + Channel.GetErrorString(motorFault));
                                Logger?.Error(message);
                                axis.DeviceError = new Message(MessageLevel.Error, message, message, "Axis id: " + ACSID[axis.AxisID]);
                                RaiseErrorEvent(axis.DeviceError);
                            }
                        }
                        axis.MotorError = motorFault;
                    }
                    return isInError;
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

        private bool GetMotionErrors()
        {
            CheckControllerCommunication();
            try
            {
                lock (_channelSync)
                {
                    bool isInError = false;
                    foreach (ACSAxis axis in ControlledAxesList)
                    {
                        // Get reason for motor disabling
                        var motionFault = 0; // Channel.GetMotionError(ACSID[axis.AxisID]);

                        // Codes from 5000 to 5008 do not indicate an error. They report normal motion termination
                        // Codes from 5009 and higher appear when a motion is terminated or a motor is disabled due to a fault detected by the controller.
                        // (See §10.1.1 of "ACPSL Programmer's guide" rev 2.70)
                        if (motionFault >= 5009)
                        {
                            isInError = true;
                            if (axis.MotionError != motionFault)
                            {
                                string motionFaultMessage = FormatMessage("GetAxisFaults - motionFault: " + motionFault + " ");// + Channel.GetErrorString(motionFault));
                                Logger?.Error(motionFaultMessage);
                                axis.DeviceError = new Message(MessageLevel.Error, motionFaultMessage, motionFaultMessage, "Axis id: " + ACSID[axis.AxisID]);
                                RaiseErrorEvent(axis.DeviceError);
                            }
                        }
                        axis.MotionError = motionFault;
                    }
                    return isInError;
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

        private void RefreshState(bool forceRefresh = false)
        {
            foreach (var axis in ControlledAxesList)
                RefreshAxisState(axis);

            var newState = IsNewAxisState() || IsNewLandedState() || IsNewWaferClampState();

            if (newState || forceRefresh)
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
        }

        private bool IsNewAxisState()
        {
            bool newState = false;
            foreach (IAxis axis in ControlledAxesList)
            {
                //if (!axis.Enabled) // Commented because it causes reset axis after axes landing
                //    isInError = true;

                if ((axis.Enabled != axis.EnabledPrev) || (axis.Moving != axis.MovingPrev))
                    newState = true;

                axis.EnabledPrev = axis.Enabled;
                axis.MovingPrev = axis.Moving;
            }
            return newState;
        }

        private bool IsNewLandedState()
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
                //lock (_channelSync)
                //{
                //    Channel.WriteVariable(Actions.ActionReset, Commands.Reset);
                //}
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

        private async Task TaskMotorsPollingAsync()
        {
            string previousMsg = "";
            Stopwatch stopWatch = new Stopwatch();
            var isInError = false;
            while (_cancelationToken.IsCancellationRequested == false)
            {
                try
                {
                    //lock (_channelSync)
                    {
                        isInError = false;
                        isInError |= GetCustomFaults();
                        isInError |= GetAxisErrors();
                        isInError |= GetMotionErrors();
                        isInError |= GetMotorsErrors();
                        isInError |= GetProgramFaults();

                        RefreshState();

                        if (!_isLanded && !_landingInProgress) // Axis are disabled when landed, it is an expected behaviour
                            foreach (IAxis axis in ControlledAxesList)
                                if (!axis.Enabled)
                                    isInError = true;

                        var NoMotorsMoving = CheckNoMotorMoving();

                        if (isInError && NoMotorsMoving)
                        {
                            if (stopWatch.ElapsedMilliseconds < DelayBeforeResetCtrl) // delay to give time to the controller to manage the error
                                continue;

                            ResetFaultsAfterDelay(ref isInError);
                        }
                        else
                            stopWatch.Restart(); // If there is no error, reset chrono used for a pause before reseting errors

                        TryGetMessageFromController(ref previousMsg);
                    }
                    RaiseEventIfPositionChanged();
                    await Task.Delay(10);
                }
                catch (AggregateException aex)
                {
                    var errorMsg = FormatMessage($"TaskMotorsPolling : {aex.Flatten().Message}");
                    Logger?.Error(errorMsg);
                    RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                }
                catch (Exception Ex)
                {
                    var errorMsg = FormatMessage($"TaskMotorsPolling : {Ex.Message}");
                    Logger?.Error(errorMsg);
                    RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                }
            }
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
            bool newState = false;

            foreach (ACSAxis axis in ControlledAxesList)
            {
                Length previousPos = axis.CurrentPos;
                RefreshCurrentPos(new List<IAxis>() { axis });
                if (!axis.CurrentPos.Near(previousPos, axis.DistanceThresholdForNotification))
                {
                    newState = true;
                }
            }

            if (newState)
            {
                double xPos = GetAxisFromAxisID("X").CurrentPos.Millimeters;
                double yPos = GetAxisFromAxisID("Y").CurrentPos.Millimeters;
                double zTopPos = GetAxisFromAxisID("ZTop").CurrentPos.Millimeters;
                double zBottomPos = GetAxisFromAxisID("ZBottom").CurrentPos.Millimeters;
                RaisePositionChangedEvent(new XYZTopZBottomPosition(new MotorReferential(), xPos, yPos, zTopPos, zBottomPos));
            }
        }

        private void StartMotorsPolling()
        {
            CheckControllerCommunication();

            _cancelationToken = new CancellationTokenSource();
            try
            {
                //lock (_channelSync)
                //{
                //    Channel.OpenMessageBuffer(MessageBufferSize);
                //}
            }
            catch (ACSException ACSEx)
            {
                Logger?.Error(FormatMessage("StartMotorsPolling - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("StartMotorsPolling - Exception: " + Ex.Message));
                throw;
            }

            _pollTask = new Task(async () => { await TaskMotorsPollingAsync(); }, TaskCreationOptions.LongRunning);
            _pollTask.Start();
        }

        private void StartCountdownForWaferClamp()
        {
            CheckControllerCommunication();

            _cancelationToken = new CancellationTokenSource();

            Task countdownWaferClampTask = new Task(() =>
            {
                bool Success = SpinWait.SpinUntil(() =>
                {
                    return RefreshIsWaferClamped() == true;
                }
                , Timeouts.TimeoutClampWafer);
                RefreshState(forceRefresh: true);

                if (!Success)
                {
                    string message = FormatMessage("Clamp wafer Failed");
                    RaiseErrorEvent(new Message(MessageLevel.Error, message, message, DeviceID));
                }
            }, TaskCreationOptions.LongRunning);
            countdownWaferClampTask.Start();
        }

        private void StartCountdownForWaferRelease()
        {
            CheckControllerCommunication();

            _cancelationToken = new CancellationTokenSource();

            Task countdownWaferClampTask = new Task(() =>
            {
                bool Success = SpinWait.SpinUntil(() =>
                {
                    return RefreshIsWaferClamped() == false;
                }
                , Timeouts.TimeoutClampWafer);
                RefreshState(forceRefresh: true);

                if (!Success)
                {
                    string message = FormatMessage("Release wafer Failed");
                    RaiseErrorEvent(new Message(MessageLevel.Error, message, message, DeviceID));
                }
            }, TaskCreationOptions.LongRunning);
            countdownWaferClampTask.Start();
        }

        public void ClampWafer(string cmdValve)
        {
            CheckControllerCommunication();

            try
            {
                if (RefreshIsWaferClamped() == true)
                    return;

                // Channel.WriteVariable(Actions.ActionWaferClamp, cmdValve);

                //bool Success = SpinWait.SpinUntil(() =>
                //{
                //    return RefreshIsWaferClamped() == true;
                //}
                //, Timeouts.TimeoutClampWafer);
                //if (!Success)
                //{
                //    throw (new Exception("ClampWafer Failed"));
                //}
                //bool noMotorsMoving = false;
                //bool isInError = false;
                //RefreshAndRaiseStateEventsForAxesAndWaferClamp(ref noMotorsMoving, ref isInError, true);
                StartCountdownForWaferClamp();
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

        public void ReleaseWafer(string valve)
        {
            CheckControllerCommunication();

            try
            {
                if (RefreshIsWaferClamped() == false)
                    return;

                //Channel.WriteVariable(Actions.ActionWaferRelease, valve);
                StartCountdownForWaferRelease();
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

        private void TerminateOtherProcess()
        {
            //string ProcessName = Process.GetCurrentProcess().ProcessName;
            //ACSC_CONNECTION_DESC[] connectionList = Channel.GetConnectionsList();
            //for (int index = 0; index < connectionList.Length; index++)
            //{
            //    if (connectionList[index].Application.Contains(ProcessName))
            //    {
            //        Channel.TerminateConnection(connectionList[index]);
            //    }
            //}
        }

        private void OpenEthernetConnection()
        {
            try
            {
                string ethernetIP = _controllerConfig.EthernetCom != null && !string.IsNullOrWhiteSpace(_controllerConfig.EthernetCom.IP) ? _controllerConfig.EthernetCom.IP : string.Empty;
                int ethernetPort = _controllerConfig.EthernetCom != null ? _controllerConfig.EthernetCom.Port : 0;
                Channel = new Object();

                CheckControllerCommunication();

                if (string.IsNullOrEmpty(ethernetIP))
                    throw (new Exception(FormatMessage("String ethernetIP is empty")));

                if (ethernetPort == 0)
                    throw (new Exception(FormatMessage("ethernetPort is empty")));

                //connect channel
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
                if ((acsAxis.ACSAxisConfig.CurrentSpeed == speed)
                 && (acsAxis.ACSAxisConfig.CurrentAccel == accel))
                    return;

                acsAxis.ACSAxisConfig.CurrentSpeed = speed;
                acsAxis.ACSAxisConfig.CurrentAccel = accel;
                //lock (_channelSync)
                //{
                //    Channel.SetVelocity(ACSID[acsAxis.AxisID], acsAxis.ACSAxisConfig.CurrentSpeed);
                //    Channel.SetAcceleration(ACSID[acsAxis.AxisID], acsAxis.ACSAxisConfig.CurrentAccel);
                //}
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

        private void ResetFaultsAfterDelay(ref bool isInError)
        {
            ResetFaults();
            EnableAxis(ControlledAxesList);
            isInError = false;
            Logger?.Information(FormatMessage("TaskMotorsPolling - Motion controller fault is reset"));
        }

        public async Task StartRefreshIOStatesTask()
        {
            CheckControllerCommunication();
            // terminate any previous pending tasks
            _cancelationTokenForIOsTask?.Cancel();
            _cancelationTokenForIOsTask = new CancellationTokenSource();

            await IORefreshTaskAsync();
        }

        private async Task IORefreshTaskAsync()
        {
            // keep tokesource before change
            var cancelToken = _cancelationTokenForIOsTask.Token;

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
                            double epsilon = 0.01;
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

        #region IChuck

        public override void Init(List<Message> errorList)
        {
            try
            {
                Logger.Information("Init ACSController as dummy");
                _globalStatusServer.SetToolModeStatus(ToolMode.Run);
                Connect();
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

        public void ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            foreach (var valve in _controllerConfig.WaferClampList)
            {
                if (valve.Available && (wafer.Diameter == valve.WaferSize))
                {
                    ClampWafer(valve.ValveName);
                }
            }
        }

        public void ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            CheckControllerCommunication();

            try
            {
                if (RefreshIsWaferClamped() == false)
                    return;

                string valveName = FindValveName(wafer.Diameter.Millimeters);
                WaitForWaferReleaseAsync();
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

        private void WaitForWaferReleaseAsync()
        {
            CheckControllerCommunication();

            bool succes = SpinWait.SpinUntil(() => RefreshIsWaferClamped(), Timeouts.TimeoutClampWafer);

            if (!succes)
            {
                string message = FormatMessage("Release wafer Failed");
                Logger.Error(message);
                RaiseErrorEvent(new Message(MessageLevel.Error, message, message, DeviceID));
                return;
            }
            Logger.Information(FormatMessage("Wafer is Released"));
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

        public override bool IsLanded()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    return false; //  ((int)Channel.ReadVariable(Status.Land) != 0);
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

        public bool RefreshIsWaferClamped()
        {
            CheckControllerCommunication();

            try
            {
                lock (_channelSync)
                {
                    return true; // ((int)Channel.ReadVariable(Status.Clamp) != 0);
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

        public void InitializationChuck(List<Message> initErrors)
        {
            throw new NotImplementedException();
        }

        public void PinLift_Up()
        {
        }

        public void PinLift_Down()
        {
        }

        public double AnalogRead(AnalogInput input)
        {
            return 12.5;
        }

        public void AnalogWrite(AnalogOutput output, double value)
        {
        }

        public bool DigitalRead(DigitalInput input)
        {
            return true;
        }

        public void DigitalWrite(DigitalOutput output, bool value)
        {
        }

        public Input GetInput(string name)
        {
            if (!NameToInput.ContainsKey(name))
            {
                Logger.Error($"{name} is not known in the configuration. Check in the AnaHardwareConfiguration file.");
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

        private ChuckState CreateChuckState(bool clamped, MaterialPresence presence)
        {
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(_signleSizeSupported, clamped);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(_signleSizeSupported, presence);
            return new ChuckState(clampStates, presenceStates);
        }

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        public ANAChuckConfig ChuckConfiguration { get; set; }
        public OnOffUnk WaferPresent { get; set; }
        public bool IsChuckStateChangedEventSet { get => (ChuckStateChangedEvent != null); }

        public bool CheckWaferPresence()
        {
            return true;
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
        }

        public override void InitZTopFocus()
        {
            Logger.Information("Initialize ZTop focus (UOH)");
            Logger.Information("Initialize ZTop focus (UOH) is completed");
        }

        public override void InitZBottomFocus()
        {
            Logger.Information("Initialize ZBottom focus (LOH)");
            Logger.Information("Initialize ZBottom focus (LOH) is completed");
        }

        public void StopRefreshIOStatesTask()
        {
            _cancelationTokenForIOsTask?.Cancel();
        }

        public bool CdaIsReady()
        {
            throw new NotImplementedException();
        }

        public bool IsInMaintenance()
        {
            throw new NotImplementedException();
        }

        public bool PrepareToTransferState()
        {
            throw new NotImplementedException();
        }

        #endregion IChuck
    }

    #endregion Private/Protected methods
}
