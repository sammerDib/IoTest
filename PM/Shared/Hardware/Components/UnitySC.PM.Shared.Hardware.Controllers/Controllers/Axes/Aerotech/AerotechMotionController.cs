using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Aerotech.Common;
using Aerotech.Ensemble;
using Aerotech.Ensemble.Commands;
using Aerotech.Ensemble.Parameters;
using Aerotech.Ensemble.Status;
using Aerotech.Ensemble.Tasks;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using ATechController = Aerotech.Ensemble.Controller;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class AerotechMotionController : MotionControllerBase, IMotion, IPositionSynchronizedOutput, IDisposable
    {
        private readonly AerotechControllerConfig _aerotechControllerConfig;
        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly Dictionary<string, int> _axisIdToAerotechIndex = new Dictionary<string, int>();
        private ATechController _aerotechController;
        private TaskState _currentProgramTaskState = TaskState.Error;

        public event Action<AerotechTaskEnded> OnProgramTaskEnded;

        private bool _isExternalFaultDetectedOnSensor = false;
        private Dictionary<AxisMask, AxisDiagPacket> _previousAxisDataDico = new Dictionary<AxisMask, AxisDiagPacket>();
        private Dictionary<AxisMask, AerotechAxisData> _axisDataDico = new Dictionary<AxisMask, AerotechAxisData>();
        private AxisMask _allControlledAxesMask;
        private int _psoAxisNumber = -1;
        private bool _previousAnyAxisMoving = false;

        private ConcurrentDictionary<string, Length> _axisPositions = new ConcurrentDictionary<string, Length>();
        private object _axisPositionsLock = new object();

        public AerotechMotionController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            ClassLocator.Default.GetInstance<ILogger>();

            if (controllerConfig is AerotechControllerConfig)
            {
                _aerotechControllerConfig = (AerotechControllerConfig)controllerConfig;
            }
            else
                throw (new Exception(FormatMessage("Bad controller configuration type. Controller creation failed !")));

            string atechAxisID = string.Empty;
            try
            {
                // Creating dictionaries to match axisId wih aerotech ids
                foreach (var atechAxisIDLink in _aerotechControllerConfig.AerotechAxisIDLinks)
                {
                    atechAxisID = atechAxisIDLink.AxisID;
                    _axisIdToAxisMasks.Add(atechAxisID, GetAxisID(atechAxisIDLink.AerotechID));
                    _axisIdToAerotechIndex.Add(atechAxisID, atechAxisIDLink.AerotechIndex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(FormatMessage(
                    $"Bad axis [{atechAxisID}] parameters in configuration. Controller creation failed ! Exception Message : {ex.Message}"));
            }
        }

        #region AxesContrllerBase

        public override void CheckControllerIsConnected()
        {
            if (_aerotechController.IsConnected)
                return;

            string errorMsg = FormatMessage("Connexion with aerotech controller is interrupted");
            Logger.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public override void Connect()
        {
            try
            {
                try
                {
                    ATechController.Connect();
                }
                catch (Exception Ex)
                {
                    Logger.Information("Unable to scan controller, attempting again: " + Ex.Message);
                    //Calling this function may sometimes crash when tables were not disconnected properly
                    ATechController.Connect();
                }
            }
            catch (Exception Ex)
            {
                Logger.Error(FormatMessage("OpenEthernetConnection - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void Disconnect()
        {
            CheckControllerIsConnected();
            _aerotechController.ControlCenter.Diagnostics.Suspend();
            ATechController.Disconnect();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        #endregion AxesContrllerBase

        #region Aerotech methods

        private bool IsTargetReached(List<IAxis> axesList, List<double> target)
        {
            int index = 0;
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

        /// <summary>
        /// check if a move is possible for the stage
        /// </summary>
        /// <returns>True if the movement is possible, false otherwise</returns>
        private bool IsMoveAllowedByController()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    ControllerDiagPacket DPacket = _aerotechController.DataCollection.RetrieveDiagnostics(false);
                    bool isOneIsActive =
                        AxisList.Any(axis => DPacket[_axisIdToAxisMasks[axis.AxisID]].AxisStatus.MoveActive);

                    if (isOneIsActive)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, "MoveAllowedByController()");
                throw;
            }
        }

        private void RaiseOnProgramTaskEnded(AerotechTaskEnded state)
        {
            Logger.Information($"Raising Task Ended event with result : {state}");
            try
            {
                OnProgramTaskEnded?.Invoke(state);
            }
            catch (Exception ex)
            {
                Logger.Error($"While notifying OnProgramTaskEnded : " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        #endregion Aerotech methods

        #region Public methods

        public override void Init(List<Message> errorList)
        {
            try
            {
                Logger.Information("Connecting to stage...");
                Connect();

                if (ATechController.ConnectedControllers.Count > 0)
                {
                    bool controllerFound = false;
                    foreach (ATechController EnsembleController in ATechController.ConnectedControllers)
                    {
                        try
                        {
                            if (EnsembleController.Information.Name == _aerotechControllerConfig.DeviceID)
                            {
                                controllerFound = true;
                                Logger.Information("Stage found, configuring parameters");
                                // Activation axes
                                _aerotechController = EnsembleController;
                                _aerotechController.Tasks.StopPrograms();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(
                                $"Error while configuring controller {_aerotechControllerConfig.DeviceID} : {ex.Message}");
                            State = new DeviceState(DeviceStatus.Error);
                        }
                    }

                    if (!controllerFound)
                    {
                        Logger.Error(
                            $"No controller with name {_aerotechControllerConfig.DeviceID} found, check if you map one to this computer.");
                        State = new DeviceState(DeviceStatus.Error);
                    }
                }
                else
                {
                    Logger.Error($"No controller found on the network, check if you map one to this computer.");
                    State = new DeviceState(DeviceStatus.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception in AerotechController.Init() :  {ex.Message} - {ex.StackTrace}");
                State = new DeviceState(DeviceStatus.Error);
            }
        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
            CheckControllerIsConnected();
            try
            {
                // Create the list of all axes used identified by aerotech ID
                List<AxisSpeed> speedList = new List<AxisSpeed>();
                foreach (var axis in AxisList)
                {
                    _allControlledAxesMask |= _axisIdToAxisMasks[axis.AxisID];
                    speedList.Add(AxisSpeed.Normal);
                    _axisDataDico[_axisIdToAxisMasks[axis.AxisID]] = new AerotechAxisData();
                }

                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.Abort();
                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.FaultAck();
                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.Enable();

                _aerotechController.Commands.Motion
                    .WaitMode(WaitType.NoWait); // all command called from Commands.Motion.XXXXX() are asynchrone

                //TODO SetSpeedAxisAndAdjustAcceleration(AxisList, speedList);

                foreach (var axis in AxisList)
                {
                    // Create first axisStatus for each axis in list
                    // Display parameters values used in controller to detect end of move in for WaitingForMotionDone() in log
                    Logger.Information($"[Axis move control parameters accuracy of {axis.AxisID} in controller]");
                    TypedParameter<double> inPositionDistance = _aerotechController.Parameters
                        .Axes[_axisIdToAxisMasks[axis.AxisID]].Motion.InPosition.InPositionDistance;
                    Logger.Information($"InPositionDistance = {inPositionDistance.Value.ToString()}");
                    _axisDataDico[_axisIdToAxisMasks[axis.AxisID]].PositionEpsilon = inPositionDistance.Value;
                    TypedParameter<int> inPositionTime = _aerotechController.Parameters
                        .Axes[_axisIdToAxisMasks[axis.AxisID]]
                        .Motion.InPosition.InPositionTime;
                    Logger.Information($"InPositionTime = {inPositionTime.Value.ToString()}");
                }

                //Finding PSO Axis
                foreach (var axis in AxisList)
                {
                    if (axis.AxisConfiguration is AerotechAxisConfig atechAxis)
                    {
                        if (atechAxis.UsedPSO)
                        {
                            _psoAxisNumber = _aerotechController.Information.Axes[_axisIdToAxisMasks[axis.AxisID]]
                                .Number;
                            break;
                        }
                    }
                    else
                        throw new Exception(
                            $"{axis.AxisID} axis configuration type does not allow to read if PSO enabled. AerotechAxisConfig is needed.");
                }

                if (_psoAxisNumber >= 0)
                {
                    // Reset PSO system to be sure to have good condition starting - PSO on one axis only
                    _aerotechController.Commands.PSO.Control(_psoAxisNumber, PsoMode.Reset);
                }

                DiagPacketPoller.RefreshInterval = _aerotechControllerConfig.RefreshDiagRate;
                _aerotechController.ControlCenter.Diagnostics.NewDiagPacketArrived +=
                    new EventHandler<NewDiagPacketArrivedEventArgs>(Diagnostics_NewDiagPacketArrived);

                // Check current axes state
                ControllerDiagPacket controllerDiagPacket = _aerotechController.DataCollection.RetrieveDiagnostics();
                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.FaultAck();
                bool oneAxisIsInFault = false;
                bool oneAxisIsMoving = false;
                bool allAxesAreHomed = true;
                bool allAxesAreEnabled = true;
                foreach (var axis in AxisList)
                {
                    oneAxisIsInFault |= !controllerDiagPacket[_axisIdToAxisMasks[axis.AxisID]].AxisFault.None;
                    oneAxisIsMoving |= controllerDiagPacket[_axisIdToAxisMasks[axis.AxisID]].AxisStatus.MoveActive;
                    allAxesAreHomed |= controllerDiagPacket[_axisIdToAxisMasks[axis.AxisID]].AxisStatus.Homed;
                    allAxesAreEnabled |= controllerDiagPacket[_axisIdToAxisMasks[axis.AxisID]].AxisStatus.Enabled;
                }

                if (!oneAxisIsInFault)
                {
                    if (allAxesAreHomed)
                    {
                        Logger.Information("Connection to controllers successfully set up");
                        State = new DeviceState(DeviceStatus.Ready);
                    }
                    else
                    {
                        Logger.Warning("One or more axes are not homed");
                        State = new DeviceState(DeviceStatus.Warning);
                    }
                }
                else
                {
                    Logger.Error("One or more axes is in fault, acknowledge it before any use");
                    State = new DeviceState(DeviceStatus.Error);
                    RaiseStateChangedEvent(new AxesState(allAxesAreEnabled, oneAxisIsMoving));
                }


                AxisMask axisMaskList = AxisMask.None;
                foreach (IAxis axis in AxisList)
                {
                    axisMaskList = axisMaskList | _axisIdToAxisMasks[axis.AxisID];
                }

                if (!allAxesAreHomed)
                {
                    Logger.Information("Homing stage...");
                    if (IsMoveAllowedByController())
                    {
                        _aerotechController.Commands.Motion.Home(axisMaskList);
                        Logger.Information("Homing done");
                    }
                    else
                    {
                        Logger.Error("Homing not allowed");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "InitializationAllAxes()");
                initErrors.Add(new Message(MessageLevel.Error, ex.Message, DeviceID));
            }

            if (initErrors.Any(message => message.Level == MessageLevel.Fatal) ||
                initErrors.Any(message => message.Level == MessageLevel.Error))
                State = new DeviceState(DeviceStatus.Error);
            else
                State = new DeviceState(DeviceStatus.Ready);
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public AxisMask GetAxisID(string axisId)
        {
            bool succeed = Enum.TryParse<AxisMask>(axisId, out var result);
            if (succeed)
                return result;
            else
                throw new Exception("Axis ID is invalid. Check controller configuration");
        }

        public override void HomeAllAxes()
        {
            Logger.Information("Homing stage...");

            try
            {
                AxisMask axisMaskList = AxisMask.None;
                foreach (IAxis axis in AxisList)
                {
                    axisMaskList = axisMaskList | _axisIdToAxisMasks[axis.AxisID];
                }

                if (IsMoveAllowedByController())
                {
                    _aerotechController.Commands.Motion.Home(axisMaskList);
                    Logger.Information("Homing done");
                }
                else
                {
                    Logger.Error("Homing not allowed");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Homing encountered an error)");
                throw;
            }
        }

        public override PositionBase GetPosition()
        {
            throw new NotImplementedException();
        }

        public override void RefreshAxisState(IAxis axis)
        {
            //Done in Diagnostics_NewDiagPacketArrived at every package arrival
        }

        public override bool ResetController()
        {
            CheckControllerIsConnected();
            try
            {
                _aerotechController.Reset(false);
            }
            catch (AerotechException ex)
            {
                Logger.Error(FormatMessage("ResetAxis - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage("ResetAxis - Exception: " + ex.Message));
                throw;
            }
            return true;
        }

        public void Dispose()
        {
            CheckControllerIsConnected();

            try
            {
                _aerotechController.ControlCenter.Diagnostics.Suspend();
                ATechController.Disconnect();
            }
            catch (AerotechException ex)
            {
                Logger.Error(FormatMessage($"Dispose - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage($"Dispose - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        private AxisMask GetAllAxisInMask(List<IAxis> axesList)
        {
            AxisMask axesMask = AxisMask.None;
            foreach (IAxis axis in axesList)
            {
                axesMask = axesMask | _axisIdToAxisMasks[axis.AxisID];
            }

            return axesMask;
        }

        /// <summary>
        /// StopAxesMotion aborts the task and move in progress
        /// </summary>
        public override void StopAllMotion()
        {
            CheckControllerIsConnected();
            try
            {
                foreach (var axisId in _axisIdToAxisMasks.Keys)
                {
                    StopAxis(axisId);
                }

                Thread.Sleep(1000);
            }
            catch (AerotechException ex)
            {
                Logger.Error(FormatMessage($"StopAxisMotion - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage($"StopAxisMotion - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            CheckControllerIsConnected();
            try
            {
                AxisMask axisMask = GetAllAxisInMask(AxisList);
                _aerotechController.Commands.Motion.WaitMode(WaitType.MoveDone);
                //Wait end of motion
                var motionEnded =
                    _aerotechController.Commands.Motion.WaitForMotionDone(WaitOption.MoveDone, axisMask, timeout);

                if (!motionEnded)
                    Logger.Warning(FormatMessage("WaitMotionEnd - Waiting for motion ending was in TIMEOUT"));
                else
                {
                    Logger.Information(FormatMessage("WaitMotionEnd - Motion ending was signaled successfully"));
                }
            }
            catch (AerotechException ex)
            {
                Logger.Error(FormatMessage("WaitMotionEnd - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage("WaitMotionEnd - Exception: " + ex.Message));
                throw;
            }
        }

        #endregion Public methods

        private void RaiseStateChanged()
        {
            bool allAxisEnabled = true;
            bool oneAxisIsMoving = false;

            foreach (var axis in AxisList)
            {
                allAxisEnabled &= axis.Enabled;
                oneAxisIsMoving |= axis.Moving;
            }
            RaiseStateChangedEvent(new AxesState(allAxisEnabled, oneAxisIsMoving, false));
        }

        public IAxis GetAxisFromAxisID(string axisID)
        {
            IAxis axis = AxisList.Find(a => a.AxisID == axisID);
            if (axis is null)
                throw new Exception(axisID + " axis not found in configuration");
            return axis;
        }


        protected virtual void Diagnostics_NewDiagPacketArrived(object sender, NewDiagPacketArrivedEventArgs e)
        {
            try
            {
                bool allAxesExternalFaultActive = false;
                bool oneAxisStateChangedAtLeast = false;
                bool anyAxisMoving = false;

                foreach (var axis in AxisList)
                {
                    var currAxis = _axisIdToAxisMasks[axis.AxisID];
                    if (!e.Data[currAxis].AxisFault.None)
                    {
                        string errorDesc = e.Data[currAxis].AxisFault.ToString();
                        axis.DeviceError = new Message(MessageLevel.Error, errorDesc, errorDesc,
                            "Axis id: " + axis.AxisID);
                    }

                    if (axis is AerotechAxis atechAxis)
                    {
                        //Automatic verification following slit door closing after loading. An explicit acknowledgement is needed for any other type of error
                        allAxesExternalFaultActive &= (e.Data[currAxis].AxisFault.ActiveBits.Count == 1) &&
                                                      (e.Data[currAxis].AxisFault.ExternalFault);

                        atechAxis.Enabled = e.Data[currAxis].AxisStatus.Enabled;
                        atechAxis.Initialized = e.Data[currAxis].AxisStatus.Homed;
                        atechAxis.CurrentPos = e.Data[currAxis].PositionFeedback.Millimeters();
                        atechAxis.Fault = e.Data[currAxis].AxisFault;
                        atechAxis.Status = e.Data[currAxis].AxisStatus;
                        atechAxis.Moving = e.Data[currAxis].AxisStatus.MoveActive;

                        // Check if any axis is moving
                        anyAxisMoving |= e.Data[currAxis].AxisStatus.MoveActive;

                        UpdateAxisPositionDictionary(atechAxis.AxisID, atechAxis.CurrentPos);

                        bool found = _previousAxisDataDico.TryGetValue(currAxis, out var previousAxisData);
                        if (!found)
                        {
                            _previousAxisDataDico.Add(currAxis, e.Data[currAxis]);
                            continue;
                        }
                        oneAxisStateChangedAtLeast |=
                            (previousAxisData.AxisStatus.Enabled !=
                             e.Data[currAxis].AxisStatus.Enabled); // Detection if axis disabled
                        oneAxisStateChangedAtLeast |= (previousAxisData.AxisStatus.MoveActive !=
                                                       e.Data[currAxis].AxisStatus
                                                           .MoveActive); // Detection if axis is moving

                        previousAxisData = e.Data[currAxis];
                    }
                }

                if (oneAxisStateChangedAtLeast)
                {
                    RaiseStateChanged(); // Update AxisSate to subscribers
                }
                if (anyAxisMoving != _previousAnyAxisMoving)
                {
                    RaiseStateChanged(); // Update AxisSate to subscribers
                }

                // Update the previous state of the axes
                _previousAnyAxisMoving = anyAxisMoving;

                _isExternalFaultDetectedOnSensor =
                    !(e.Data[_aerotechControllerConfig.ExternalFaultAxisMask].DigitalInput0 >
                      0); // Signal door state tested > 0 = Closed
                //check Task status to raise associated event
                TaskState newState = e.Controller.Tasks[TaskId.T01].State;
                if (_currentProgramTaskState == TaskState.ProgramRunning && newState == TaskState.ProgramComplete)
                {
                    //a program has completed
                    RaiseOnProgramTaskEnded(AerotechTaskEnded.Succeed);
                }
                else if (_currentProgramTaskState == TaskState.ProgramRunning && newState == TaskState.Error)
                {
                    //a program has completed but an error occurred
                    RaiseOnProgramTaskEnded(IsExternalFaultDetectedOnSensor
                        ? AerotechTaskEnded.ExternalFaultDetected
                        : AerotechTaskEnded.Error);
                }

                if (allAxesExternalFaultActive &&
                    !IsExternalFaultDetectedOnSensor) // All axis in Fault AND Door closed => Acknowledge all fault to release table
                {
                    e.Controller.Commands.AcknowledgeAll(); // Acknowledge fault on all axes in controller
                }

                _currentProgramTaskState = newState;
            }
            catch (Exception Ex)
            {
                Logger.Error($"The following error occurred during diagnostic polling : " + Ex.Message);
            }
        }

        private bool IsExternalFaultDetectedOnSensor
        {
            get { return _isExternalFaultDetectedOnSensor; }
        }

        #region IPositionSynchronizedOutput

        public void DisablePSO()
        {
            if (_psoAxisNumber == -1) return;
            _aerotechController.Commands.PSO.Control(_psoAxisNumber, PsoMode.Reset);
        }

        public void SetPSOInFixedWindowMode(double beginPosition, double endPosition, double pixelDegSize)
        {
            if (_psoAxisNumber == -1) return;
            try
            {
                int countsPerPixels = (int)Math.Round(pixelDegSize *
                    _aerotechController.Parameters.Axes[_psoAxisNumber].Units.CountsPerUnit.Value / _aerotechController
                        .Parameters.Axes[_psoAxisNumber].Feedback.Multiplier.EmulatedQuadratureDivider.Value);

                _aerotechController.Commands.PSO.TrackReset(_psoAxisNumber, 0);
                _aerotechController.Commands.PSO.TrackInput(_psoAxisNumber, 3);
                _aerotechController.Commands.PSO.DistanceFixed(_psoAxisNumber, countsPerPixels);
                _aerotechController.Commands.PSO.WindowInput(_psoAxisNumber, 1, 3);
                _aerotechController.Commands.PSO.WindowRange(_psoAxisNumber, 1,
                    Math.Round(beginPosition *
                               _aerotechController.Parameters.Axes[_psoAxisNumber].Units.CountsPerUnit.Value /
                               _aerotechController.Parameters.Axes[_psoAxisNumber].Feedback.Multiplier
                                   .EmulatedQuadratureDivider.Value),
                    Math.Round((endPosition) *
                               _aerotechController.Parameters.Axes[_psoAxisNumber].Units.CountsPerUnit.Value /
                               _aerotechController.Parameters.Axes[_psoAxisNumber].Feedback.Multiplier
                                   .EmulatedQuadratureDivider.Value));
                _aerotechController.Commands.PSO.Pulse(_psoAxisNumber, 5, 1);
                _aerotechController.Commands.PSO.OutputPulseWindowMask(_psoAxisNumber);
                _aerotechController.Commands.PSO.Control(_psoAxisNumber, PsoMode.Arm);
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage("Error in SetPSOInFixedWindowMode(" + beginPosition + "," + endPosition +
                                           "," + pixelDegSize + ") " + ex.Message));
                throw;
            }
        }

        public double GetNearestPSOPixelSize(double pixelSizeTarget, double waferDiameter)
        {
            if (_psoAxisNumber == -1) return -1;
            double pixelSizeInDeg = 360.0 / (waferDiameter * Math.PI * 1000.0 / pixelSizeTarget);
            int countsPerPixels =
                (int)(Math.Round(pixelSizeInDeg *
                                 _aerotechController.Parameters.Axes[_psoAxisNumber].Units.CountsPerUnit.Value /
                                 _aerotechController
                                     .Parameters.Axes[_psoAxisNumber].Feedback.Multiplier.EmulatedQuadratureDivider
                                     .Value) *
                      _aerotechController.Parameters.Axes[_psoAxisNumber].Feedback.Multiplier.EmulatedQuadratureDivider
                          .Value);
            return (countsPerPixels / _aerotechController.Parameters.Axes[_psoAxisNumber].Units.CountsPerUnit.Value) *
                waferDiameter * Math.PI * 1000.0 / 360.0;
        }

        #endregion IPositionSynchronizedOutput

        #region Private Methods

        private void StopAxis(string axisId)
        {
            CheckControllerIsConnected();
            if (_allControlledAxesMask == AxisMask.None) return;
            try
            {
                _aerotechController.Commands.Motion.Abort(_axisIdToAerotechIndex[axisId]);
            }
            catch (AerotechException ex)
            {
                Logger.Error(FormatMessage($"StopAxisMotion - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(FormatMessage($"StopAxisMotion - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        private void ChangeAxisAccelerationIfDifferent(IAxis axis, double accel)
        {
            CheckControllerIsConnected();
            if (axis is AerotechAxis atechAxis)
            {
                try
                {
                    if (atechAxis.CurrentAccel == accel)
                        return;
                    atechAxis.CurrentAccel = accel;
                    _aerotechController.Commands.Motion.Setup.RampRate(_axisIdToAxisMasks[axis.AxisID],
                        atechAxis.CurrentAccel);
                }
                catch (AerotechException ex)
                {
                    Logger.Error(FormatMessage("ChangeSpeedIfDifferent - AerotechException: " + ex.Message));
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Error(FormatMessage("ChangeSpeedIfDifferent - Exception: " + ex.Message));
                    throw;
                }
            }
            else
                throw new Exception("Bad axis type in parameters in ChangeAccelerationAxisIfDifferent()");
        }

        private string FormatMessage(string message)
        {
            return ($"[{DeviceID}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }

        #endregion Private Methods

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                if (!(move.Accel is null))
                {
                    ChangeAxisAccelerationIfDifferent(GetAxisFromAxisID(move.AxisId), move.Accel.MillimetersPerSecondSquared);
                }
                if (move.Speed is null)
                {
                    var associatedAxisParameters = _aerotechController.Parameters.Axes[_axisIdToAxisMasks[move.AxisId]];
                    move.Speed = associatedAxisParameters.Motion.DefaultSpeed.Value.MillimetersPerSecond();
                }
            }

            var axisIDs = moves.Select(move => _axisIdToAerotechIndex[move.AxisId]).ToArray();
            var positions = moves.Select(move => move is RotationalAxisMove rotationalMove ? rotationalMove.AnglePosition.Degrees : move.Position.Millimeters).ToArray();
            var speeds = moves.Select(move => move.Speed.MillimetersPerSecond).ToArray();

            if (IsMoveAllowedByController())
            {
                _aerotechController.Commands.Motion.MoveAbs(axisIDs, positions, speeds);
                foreach (var move in moves)
                {
                    if (move is RotationalAxisMove rotationalAxisMove) ChangeAxisPosition(rotationalAxisMove.AxisId, rotationalAxisMove.AnglePosition.Degrees);
                    else ChangeAxisPosition(move.AxisId, move.Position.Millimeters);
                }
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                if (!(move.Accel is null))
                {
                    ChangeAxisAccelerationIfDifferent(GetAxisFromAxisID(move.AxisId), move.Accel.MillimetersPerSecondSquared);
                }
                if (move.Speed is null)
                {
                    var associatedAxisParameters = _aerotechController.Parameters.Axes[_axisIdToAxisMasks[move.AxisId]];
                    move.Speed = associatedAxisParameters.Motion.DefaultSpeed.Value.MillimetersPerSecond();
                }
            }

            var axisIDs = moves.Select(move => _axisIdToAerotechIndex[move.AxisId]).ToArray();
            var positions = moves.Select(move => move is RotationalAxisMove rotationalMove ? rotationalMove.AnglePosition.Degrees : move.Position.Millimeters).ToArray();
            var speeds = moves.Select(move => move.Speed.MillimetersPerSecond).ToArray();

            if (IsMoveAllowedByController())
            {
                _aerotechController.Commands.Motion.MoveInc(axisIDs, positions, speeds);
                foreach (var move in moves)
                {
                    if (move is RotationalAxisMove rotationalAxisMove) ChangeAxisPosition(rotationalAxisMove.AxisId, rotationalAxisMove.AnglePosition.Degrees);
                    else ChangeAxisPosition(move.AxisId, move.Position.Millimeters);
                }
            }
        }
        private void UpdateAxisPositionDictionary(string axisID, Length currentPos)
        {
            const double epsilon = 0.0001;
            bool raiseUpdateAfterValueApplied = false;
            _axisPositions.AddOrUpdate(axisID, currentPos, (key, oldValue) =>
            {                
                if (!oldValue.Millimeters.Near(currentPos.Millimeters, epsilon))
                {
                    raiseUpdateAfterValueApplied = true;
                    return currentPos;
                }
                // If the key does not yet exist, save the initial value.
                return oldValue;
            });

            if(raiseUpdateAfterValueApplied) ChangeAxisPosition(axisID, currentPos.Millimeters);
        }
        private void ChangeAxisPosition(string axisId, double position)
        {
            switch (axisId)
            {
                case "X":
                    RaisePositionChangedEvent(new XPosition(new MotorReferential(), position));
                    break;
                case "Y":
                    RaisePositionChangedEvent(new YPosition(new MotorReferential(), position));
                    break;
                case "Z":
                    RaisePositionChangedEvent(new ZPosition(new MotorReferential(), position));
                    break;
                default:
                    break;
            }
        }
        public override void RefreshCurrentPos(List<IAxis> axesList)
        {
            CheckControllerIsConnected();
            foreach (var axis in axesList)
            {
                if (axis is AerotechAxis atechAxis)
                {
                    if (_axisPositions.TryGetValue(axis.AxisID, out var position))
                    {
                        axis.CurrentPos = position;
                    }
                }
            }
        }
    }
}
