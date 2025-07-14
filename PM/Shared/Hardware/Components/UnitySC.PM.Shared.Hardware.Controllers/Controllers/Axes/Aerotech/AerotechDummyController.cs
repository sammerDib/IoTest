using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Aerotech.Common;
using Aerotech.Ensemble;
using Aerotech.Ensemble.Status;
using Aerotech.Ensemble.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Referentials.Interface.Positions;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class AerotechDummyController : MotorController, IDisposable
    {
        #region Fields

        public new String DeviceID => _aerotechControllerConfig?.DeviceID;
        private AerotechControllerConfig _aerotechControllerConfig;
        private Dictionary<String, AxisMask> _axesIDLinks = new Dictionary<string, AxisMask>();

        protected System.Threading.Tasks.Task HandleTaskWaitMotionEnd;
        private const double DelayBeforeResetCtrl = 4000;
        private bool _serviceSpeedEnabled = false;
        private bool _connected = false;
        private List<IAxis> _axesStateTarget = new List<IAxis>();

        private const int SleepForMoveToStart = 250; // ms
        private const int TimeoutWaitMotionEnd = 20000; // ms
        private TaskState _currentProgramTaskState = TaskState.Error;

        public event Action<AerotechTaskEnded> OnProgramTaskEnded;

        private bool _isExternalFaultDetectedOnSensor = false;
        private Dictionary<AxisMask, AxisDiagPacket> _previousAxisDataDico = new Dictionary<AxisMask, AxisDiagPacket>();
        private Dictionary<AxisMask, AerotechAxisData> _axisDataDico = new Dictionary<AxisMask, AerotechAxisData>();
#pragma warning disable CS0649 // Default value. Will be deleted in the near future
        private readonly AxisMask _allControlledAxesMask;

        #endregion Fields

        #region Constructors

        public AerotechDummyController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            if (controllerConfig is AerotechControllerConfig)
            {
                _aerotechControllerConfig = (AerotechControllerConfig)controllerConfig;
            }
            else
                throw (new Exception(FormatMessage("Bad controller configuration type. Controller creation failed !")));

            String atechAxisID = String.Empty;
            try
            {
                // Create AxisID list => index of list match with
                foreach (var atechAxisIDLink in _aerotechControllerConfig.AerotechAxisIDLinks)
                {
                    atechAxisID = atechAxisIDLink.AxisID;
                    _axesIDLinks.Add(atechAxisID, GetAxisID(atechAxisIDLink.AerotechID));
                }

                // Create target
                foreach (var axis in ControlledAxesList)
                {
                    AerotechAxis newAxis = new AerotechAxis(axis.AxisConfiguration, logger);
                    _axesStateTarget.Add(newAxis);
                }
            }
            catch (Exception)
            {
                throw (new Exception(FormatMessage("Bad axis [" + atechAxisID + "] parameters in configuration. Controller creation failed !")));
            }
        }

        #endregion Constructors

        #region Properties

        public bool LoadingUnloadingPosition { get; set; }
        public IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();
        public Dictionary<String, AxisMask> ATECHID { get => _axesIDLinks; }
        public bool ServiceSpeedEnabled { get => _serviceSpeedEnabled; set => _serviceSpeedEnabled = value; }
        public bool IsExternalHardwareMoveEnable { get; private set; }

        #endregion Properties

        #region AxesContrllerBase

        public override void CheckControllerCommunication()
        {
            if (_connected)
                return;

            string errorMsg = FormatMessage("Connexion with aerotech controller is interrupted");
            Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public override void Disconnect()
        {
            CheckControllerCommunication();
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
                    CheckServiceSpeed(axis, ref speed); // Check and adjust speed according to service mode enabled
                    ChangeAccelerationAxisIfDifferent(axis, accel);
                    index++;
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - Exception: " + ex.Message));
                throw;
            }
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> commandCoords, List<IAxis> axesList, List<double> speeds, List<double> accels)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();
            try
            {
                if (!IsExternalHardwareMoveEnable)
                    throw new Exception("SetPosAxisWithSpeedAndAccel move is not allowed by external hardware");

                int indexPos = 0;
                var PosList = new List<double>();
                string positionText = string.Empty;
                foreach (IAxis axis in axesList)
                {
                    if (axis.AxisConfiguration is AerotechAxisConfig aerotechAxisConfig)
                    {
                        double Position = commandCoords[indexPos] + aerotechAxisConfig.PositionZero.Millimeters;
                        CheckAxisLimits(axis, Position);
                        PosList.Add(Position * aerotechAxisConfig.MotorDirection);
                        indexPos++;
                    }
                    else
                        throw new Exception("AxisConfig is not an ACSAxisConfig - check configuration");
                }

                AxisMask AxesMask = GetAllAxisInMask(axesList);
                String allPositionText = String.Join(", ", axesList.Select(a => $"{a.AxisID.ToString()} : {PosList[axesList.IndexOf(a)]}").ToArray());

                SetSpeedAccelAxis(axesList, speeds, accels);

                if (State.Status == DeviceStatus.Ready)
                {
                    if (MoveAllowedByController())
                    {
                        Logger?.Information($"Performing Absolute Move, {positionText}");
                        //_aerotechController.Commands.Motion.MoveAbs(AxesMask, PosList.ToArray(), speeds.ToArray());

                        HandleTaskWaitMotionEnd = new System.Threading.Tasks.Task(() => { TaskWaitMotionEnd(axesList, commandCoords); }, TaskCreationOptions.LongRunning);
                        HandleTaskWaitMotionEnd.Start();
                    }
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage("SetPosAxisWithSpeedAndAccel - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("SetPosAxisWithSpeedAndAccel - Exception: " + Ex.Message));
                throw;
            }
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

        private void TaskWaitMotionEnd(List<IAxis> axesList, List<double> target)
        {
            try
            {
                AxisMask axisMask = GetAllAxisInMask(axesList);
                String allPositionText = String.Join(", ", axesList.Select(a => $"{a.AxisID.ToString()} : {target[axesList.IndexOf(a)]}").ToArray());

                //Wait end of motion
                //_aerotechController.Commands.Motion.WaitForMotionDone(WaitOption.InPosition, axisMask);

                Logger?.Information($"Set position target reached, {allPositionText}");
            }
            catch (AerotechException ex)
            {
                string errorMsg = FormatMessage($" TaskWaitMotionEnd Failed - {ex.Message}");
                Logger?.Error(errorMsg);
                RaiseErrorEvent(new Message(MessageLevel.Error, errorMsg, "", DeviceID));
                throw;
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
                targetReached = IsTargetReached(axesList, target);

                RaiseMotionEndEvent(targetReached);
                if (!targetReached)
                {
                    string errorMsg = FormatMessage($" TaskWaitMotionEnd - Target position not reached");
                    // It is not an error if the user stopped the move
                    //RaiseErrorEvent(new Message(MessageLevel.Information, errorMsg, "", DeviceName));
                }

                double xPos = GetAxisFromAxisID("X").CurrentPos.Millimeters;
                double tPos = GetAxisFromAxisID("T").CurrentPos.Millimeters;
                RaisePositionChangedEvent(new XTPosition(new MotorReferential(), xPos, tPos));
            }
        }

        private string FormatMessage(string message)
        {
            return ($"[{DeviceID}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }

        /// <summary>
        /// check if a move is possible for the stage
        /// </summary>
        /// <returns>True if the movement is possible, false otherwise</returns>
        protected bool MoveAllowedByController()
        {
            try
            {
                int nbRetries = 0;

                while (nbRetries < 3)
                {
                    //ControllerDiagPacket DPacket = _aerotechController.DataCollection.RetrieveDiagnostics(false);
                    //bool oneIsActive = false;
                    //foreach (var axis in AxesList)
                    //{
                    //    oneIsActive = oneIsActive || DPacket[ATECHID[axis.AxisID]].AxisStatus.MoveActive;
                    //}
                    //if (oneIsActive)
                    //{
                    //    nbRetries++;
                    //    Thread.Sleep(1000);
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                }
                return false;
            }
            catch (Exception Ex)
            {
                Logger?.Error(Ex, "MoveAllowed()");
                throw;
            }
        }

        protected void RaiseOnProgramTaskEnded(AerotechTaskEnded state)
        {
            Logger.Information($"Raising Task Ended event with result : {state}");
            try
            {
                OnProgramTaskEnded?.Invoke(state);
            }
            catch (Exception Ex)
            {
                Logger.Error($"While notifying OnProgramTaskEnded : " + Ex.Message + Ex.StackTrace);
                throw;
            }
        }

        #endregion Aerotech methods

        #region Public methods

        public AxisMask GetAxisID(String axisID)
        {
            AxisMask result = AxisMask.None;
            bool succeed = Enum.TryParse<AxisMask>(axisID, out result);
            if (succeed)
                return result;
            else
                throw new Exception("Axis ID is invalid. Check controller configuration");
        }

        public override void Connect()
        {
            try
            {
                try
                {
                    //ATechController.Connect();
                }
                catch (Exception Ex)
                {
                    Logger?.Information("Unable to scan controller, attempting again: " + Ex.Message);
                    //des fois un appel à cette fonction plante lorsque les tables n'ont pas été déconnectées correctement.
                    //ATechController.Connect();
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("OpenEthernetConnection - Exception: " + Ex.Message));
                throw;
            }
        }

        public void DoHome(bool waitEndOfMove)
        {
            Logger?.Information("Homing stage...");

            try
            {
                AxisMask axisMaskList = AxisMask.None;
                foreach (IAxis axis in ControlledAxesList)
                {
                    axisMaskList = axisMaskList | ATECHID[axis.AxisID];
                }

                if (MoveAllowedByController())
                {
                    //_aerotechController.Commands.Motion.Home(axisMaskList);
                    Logger?.Information("Homing done");
                }
                else
                {
                    Logger?.Error("Homing not allowed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(Ex, "DoHome(" + waitEndOfMove + ")");
                throw;
            }
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            CheckControllerCommunication();

            Logger?.Information("Homing stage...");

            try
            {
                AxisMask axisMaskList = AxisMask.None;
                foreach (IAxis axis in ControlledAxesList)
                {
                    axisMaskList = axisMaskList | ATECHID[axis.AxisID];
                }

                if (MoveAllowedByController())
                {
                    //_aerotechController.Commands.Motion.Home(axisMaskList);
                    Logger?.Information("Homing done");
                }
                else
                {
                    Logger?.Error("Homing not allowed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(Ex, "InitializationAllAxes()");
                initErrors.Add(new Message(MessageLevel.Error, Ex.Message, DeviceID));
            }

            if (initErrors.Any(message => message.Level == MessageLevel.Fatal) ||
                initErrors.Any(message => message.Level == MessageLevel.Error))
                State = new DeviceState(Service.Interface.DeviceStatus.Error);
            else
                State = new DeviceState(Service.Interface.DeviceStatus.Ready);
        }

        public override bool ResetController()
        {
            CheckControllerCommunication();
            try
            {
                //_aerotechController.Reset(false);

                EnableAxis(ControlledAxesList);
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage("ResetAxis - AerotechException: " + ex.Message));
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
        }

        public override void Land()
        {
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double stepMillimeters)
        {
            if (stepMillimeters == 0)
                return;
            List<IAxis> axesList = new List<IAxis> { axis };
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();
            try
            {
                SetSpeedAxis(axesList, new List<AxisSpeed>() { speed }); // Set speed and acceleration is adjusted according to this speed
                //_aerotechController.Commands.Motion.MoveInc(ATECHID[axis.AxisID], stepMillimeters);

                HandleTaskWaitMotionEnd = new System.Threading.Tasks.Task(() => { TaskWaitMotionEnd(new List<IAxis>() { axis }, new List<double>() { stepMillimeters }); }, TaskCreationOptions.LongRunning);
                HandleTaskWaitMotionEnd.Start();
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"MoveIncremental - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"MoveIncremental - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        public void Dispose()
        {
            CheckControllerCommunication();

            try
            {
                //_aerotechController.ControlCenter.Diagnostics.Suspend();
                //Controller.Disconnect();
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"Dispose - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"Dispose - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        private AxisMask GetAllAxisInMask(List<IAxis> axesList)
        {
            AxisMask axesMask = AxisMask.None;
            foreach (IAxis axis in axesList)
            {
                axesMask = axesMask & ATECHID[axis.AxisID];
            }
            return axesMask;
        }

        public override void EnableAxis(List<IAxis> axesList)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            try
            {
                AxisMask axesMask = GetAllAxisInMask(axesList);
                //_aerotechController.Commands.Motion.Enable(axesMask);

                // check all axes enabled correctly
                DateTime startTime = DateTime.Now;
                bool timeout = false;
                bool allAxesEnabled = true;
                while (!timeout)
                {
                    allAxesEnabled = true;
                    foreach (IAxis axis in axesList)
                    {
                        allAxesEnabled = allAxesEnabled & axis.Enabled;
                    }
                    if (allAxesEnabled)
                    {
                        Logger.Information($"{axesMask.ToString()} axes are enabled successfully");
                        break;
                    }
                    timeout = DateTime.Now.Subtract(startTime).TotalSeconds > 5;
                }
                if (timeout && !allAxesEnabled)
                {
                    foreach (IAxis axis in axesList)
                    {
                        if (!axis.Enabled)
                            Logger.Information($"{axis.AxisID} axis enable failed");
                    }
                    throw (new Exception(FormatMessage("Enable all axes failed Timeout Error.")));
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"Enable Axis - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"Enable Axis - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        public override void DisableAxis(List<IAxis> axesList)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();
            try
            {
                AxisMask axesMask = GetAllAxisInMask(axesList);
                //_aerotechController.Commands.Motion.Disable(axesMask);

                // check all axes disabled correctly
                DateTime startTime = DateTime.Now;
                bool timeout = false;
                bool allAxesDisabled = true;
                while (!timeout)
                {
                    allAxesDisabled = true;
                    foreach (IAxis axis in axesList)
                    {
                        allAxesDisabled = allAxesDisabled & !axis.Enabled;
                    }
                    if (allAxesDisabled)
                    {
                        Logger.Information($"{axesMask.ToString()} axes are disabled successfully");
                        break;
                    }
                    timeout = DateTime.Now.Subtract(startTime).TotalSeconds > 5;
                }
                if (timeout && !allAxesDisabled)
                {
                    foreach (IAxis axis in axesList)
                    {
                        if (axis.Enabled)
                            Logger.Information($"{axis.AxisID} axis disable failed");
                    }
                    throw (new Exception(FormatMessage("Disable all axes failed Timeout Error.")));
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"Disable Axis - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"Disable Axis - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        /// <summary>
        /// StopAxesMotion aborts the task and move in progress
        /// </summary>
        public override void StopAxesMotion()
        {
            CheckControllerCommunication();
            try
            {
                //_aerotechController.Tasks.StopPrograms();
                Thread.Sleep(1000);
                AbortMove();
                Thread.Sleep(1000);
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"StopAxisMotion - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"StopAxisMotion - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed axisSpeed, double oneCoordinate)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();

            try
            {
                if (axis is AerotechAxis atechAxis)
                {
                    double position = atechAxis.ComputePositionInControllerFrame(oneCoordinate);
                    CheckAxisLimits(atechAxis, position); //Contrôle de la position demandée
                    SetSpeedAxis(new List<IAxis>() { axis }, new List<AxisSpeed>() { axisSpeed });
                    //_aerotechController.Commands.Motion.Setup.Absolute();
                    //_aerotechController.Commands.Motion.Linear(ATECHID[axis.AxisID], position * atechAxis.AerotechAxisConfig.MotorDirection);
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error($"LinearMotionSingleAxis - AerotechException:  {ex.Message} - {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error($"LinearMotionSingleAxis - Exception:  {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axesList, AxisSpeed axisSpeed, List<double> coordsList)
        {
            CheckAxesListIsValid(axesList);
            CheckControllerCommunication();

            try
            {
                List<AxisSpeed> speedList = new List<AxisSpeed>();
                AxisMask axesMask = GetAllAxisInMask(axesList);

                for (int i = 0; i < axesList.Count; i++)
                {
                    // Add to axisToMoveIdList
                    if (axesList[i] is AerotechAxis atechAxis)
                    {
                        // Update coordsList
                        double coord = atechAxis.ComputePositionInControllerFrame(coordsList[i]);
                        CheckAxisLimits(atechAxis, coord);
                        coordsList[i] = coord * atechAxis.AerotechAxisConfig.MotorDirection;
                        speedList.Add(axisSpeed);
                    }
                    else
                        throw new Exception($"{axesList[i].AxisID} axis type is invalid in LinearMotionMultipleAxis()");
                }

                SetSpeedAxis(axesList, speedList);
                SetMotionMode(true);
                //_aerotechController.Commands.Motion.Linear(axesMask, coordsList.ToArray());
            }
            catch (AerotechException ex)
            {
                Logger?.Error($"LinearMotionMultipleAxis - AerotechException:  {ex.Message} - {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error($"LinearMotionMultipleAxis - Exception:  {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        private void SetMotionMode(bool absolute)
        {
            double mode = 0; // _aerotechController.Commands.Status.GetMode(ModeType.MotionMode); // Inc = 0 / Absolute = 1

            if (absolute && (mode == 1)) return;
            if (!absolute && (mode == 0)) return;

            // If not already activated
            if (absolute)
            {
                // _aerotechController.Commands.Motion.Setup.Absolute();
            }
            else
            {
                // _aerotechController.Commands.Motion.Setup.Incremental();
            }
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            CheckControllerCommunication();

            //lock (Channel)
            {
                try
                {
                    Thread.Sleep(SleepForMoveToStart); // Wait for move to start
                    foreach (IAxis axis in ControlledAxesList)
                    {
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
                catch (AerotechException ex)
                {
                    Logger?.Error(FormatMessage("WaitMotionEnd - AerotechException: " + ex.Message));
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

            if (axis is AerotechAxis atechAxis)
            {
                try
                {
                    if (!_serviceSpeedEnabled)
                        return;

                    if (speed > atechAxis.AerotechAxisConfig.MaxSpeedService)
                        speed = atechAxis.AerotechAxisConfig.MaxSpeedService;
                }
                catch (Exception Ex)
                {
                    Logger?.Error(FormatMessage($"CheckServiceSpeed - Exception: {Ex.Message}"));
                    throw;
                }
            }
            else
                throw new Exception("Bad axis type in parameters in CheckServiceSpeed()");
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
                    ChangeAccelerationAxisIfDifferent(axis, accel);
                    index++;
                }
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("SetSpeedAxis - Exception: " + Ex.Message));
                throw;
            }
        }

        private void ChangeAccelerationAxisIfDifferent(IAxis axis, double accel)
        {
            CheckAxesListIsValid(new List<IAxis> { axis });
            CheckControllerCommunication();
            if (axis is AerotechAxis atechAxis)
            {
                try
                {
                    if (atechAxis.CurrentAccel == accel)
                        return;
                    atechAxis.CurrentAccel = accel;
                    //_aerotechController.Commands.Motion.Setup.RampRate(ATECHID[axis.AxisID], accel);
                }
                catch (AerotechException ex)
                {
                    Logger?.Error(FormatMessage("ChangeSpeedIfDifferent - AerotechException: " + ex.Message));
                    throw;
                }
                catch (Exception ex)
                {
                    Logger?.Error(FormatMessage("ChangeSpeedIfDifferent - Exception: " + ex.Message));
                    throw;
                }
            }
            else
                throw new Exception("Bad axis type in parameters in ChangeAccelerationAxisIfDifferent()");
        }

        private void InitAllAxisSpeedsAcceleration(AxisSpeed axisSpeed)
        {
            CheckControllerCommunication();

            try
            {
                List<AxisSpeed> speedList = new List<AxisSpeed>();
                // Init acceleration
                foreach (var axis in ControlledAxesList)
                {
                    if (axis is AerotechAxis atechAxis)
                    {
                        AerotechAxisConfig atechAxisConfig = atechAxis.AerotechAxisConfig;
                        //_aerotechController.Commands.Motion.Setup.RampRate(ATECHID[axis.AxisID], atechAxisConfig.AccelNormal);
                        Logger.Information($"[Axis move control parameters accuracy of {axis.AxisID} in controller]");
                        //Check controller parameters for WaitingForMotionDone() with UnityControl configuration
                        //TypedParameter<double> inPositionDistance = _aerotechController.Parameters.Axes[ATECHID[atechAxis.AxisID]].Motion.InPosition.InPositionDistance;
                        //Logger.Information($"InPositionDistance = {inPositionDistance.Value.ToString()}");
                        //TypedParameter<int> inPositionTime = _aerotechController.Parameters.Axes[ATECHID[atechAxis.AxisID]].Motion.InPosition.InPositionTime;
                        //Logger.Information($"InPositionTime = {inPositionTime.Value.ToString()}");

                        speedList.Add(axisSpeed);
                    }
                    else
                        throw new Exception($"{axis.AxisID} axis type does not allow to get axis configuration as expected. AerotechAxis is needed.");
                }

                SetSpeedAxis(ControlledAxesList, speedList);
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage("InitAllAxisSpeeds - AerotechException: " + ex.Message));
                throw;
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitAllAxisSpeeds - Exception: " + Ex.Message));
                throw;
            }
        }

        private void RaiseStateChanged()
        {
            bool AllAxisEnabled = true;
            bool OneAxisIsMoving = false;

            foreach (var axis in ControlledAxesList)
            {
                AllAxisEnabled &= axis.Enabled;
                OneAxisIsMoving |= axis.Moving;
            }
            RaiseStateChangedEvent(new AxesState(AllAxisEnabled, OneAxisIsMoving));
        }

        private bool IsNewAxisState()
        {
            bool newState = false;
            foreach (IAxis axis in ControlledAxesList)
            {
                if ((axis.Enabled != axis.EnabledPrev) || (axis.Moving != axis.MovingPrev))
                    newState = true;

                axis.EnabledPrev = axis.Enabled;
                axis.MovingPrev = axis.Moving;
            }
            return newState;
        }

        /// <summary>
        /// Aborts any movement in progress but NOT a task
        /// </summary>
        /// <returns>Controller status after the operation</returns>
        public virtual void AbortMove()
        {
            CheckControllerCommunication();
            if (_allControlledAxesMask == AxisMask.None) return;
            try
            {
                //_aerotechController.Commands.Motion.Abort(_allControlledAxesMask);
            }
            catch (AerotechException ex)
            {
                Logger?.Error(FormatMessage($"StopAxisMotion - AerotechException: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error(FormatMessage($"StopAxisMotion - Exception: {ex.Message} - {ex.StackTrace}"));
                throw;
            }
        }

        public IAxis GetAxisFromAxisID(String axisID)
        {
            IAxis axis = ControlledAxesList.Find(a => a.AxisID == axisID);
            if (axis is null)
                throw new Exception(axisID + " axis not found in configuration");
            return axis;
        }

        private void RaiseEventIfPositionChanged(double previousPosition)
        {
            bool newState = false;
            foreach (ACSAxis axis in ControlledAxesList)
            {
                if (!axis.CurrentPos.Near(previousPosition.Millimeters(), axis.DistanceThresholdForNotification))
                {
                    newState = true;
                }
            }

            if (newState)
            {
                double xPos = GetAxisFromAxisID("X").CurrentPos.Millimeters;
                double tPos = GetAxisFromAxisID("T").CurrentPos.Millimeters;
                RaisePositionChangedEvent(new XTPosition(new MotorReferential(), xPos, tPos));
            }
        }

        public void ExternalHardwareMoveEnable(bool enabled)
        {
            IsExternalHardwareMoveEnable = enabled;
        }

        public void Init()
        {
            List<Message> errorList = new List<Message>();
            Init(errorList);

            if ((errorList.Count > 0) && (errorList.Any(message => message.Level == MessageLevel.Fatal) ||
                errorList.Any(message => message.Level == MessageLevel.Error)))
            {
                throw new Exception(errorList[0].Level.ToString() + " - " + errorList[0].UserContent);
            }
        }

        public override void Init(List<Message> errorList)
        {
            try
            {
                Logger?.Information("Connecting to stage...");
                Connect();

                //if (ATechController.ConnectedControllers.Count > 0)
                //{
                //    bool controllerFound = false;
                //    foreach (ATechController EnsembleController in ATechController.ConnectedControllers)
                //    {
                //        try
                //        {
                //            if (EnsembleController.Information.Name == _aerotechControllerConfig.DeviceID)
                //            {
                //                controllerFound = true;
                //                Logger?.Information("Stage found, configuring parameters");
                //                // Activation axes
                //                _aerotechController = EnsembleController;
                //                _aerotechController.Tasks.StopPrograms();

                //                // Create the list of all axes used identified by aerotech ID

                //                List<AxisSpeed> speedList = new List<AxisSpeed>();
                //                foreach (var axis in ControlledAxesList)
                //                {
                //                    _allControlledAxesMask |= ATECHID[axis.AxisID];
                //                    speedList.Add(AxisSpeed.Normal);
                //                }

                //                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.Abort();
                //                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.FaultAck();
                //                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.Enable();

                //                _aerotechController.Commands.Motion.WaitMode(WaitType.NoWait); // all command called from Commands.Motion.XXXXX() are asynchrone

                //                SetSpeedAxis(ControlledAxesList, speedList);

                //                foreach (var axis in ControlledAxesList)
                //                {
                //                    // Create first axisStatus for each axis in list
                //                    // Display parameters values used in controller to detect end of move in for WaitingForMotionDone() in log
                //                    Logger.Information($"[Axis move control parameters accuracy of {axis.AxisID} in controller]");
                //                    TypedParameter<double> inPositionDistance = _aerotechController.Parameters.Axes[ATECHID[axis.AxisID]].Motion.InPosition.InPositionDistance;
                //                    Logger.Information($"InPositionDistance = {inPositionDistance.Value.ToString()}");
                //                    _axisDataDico[ATECHID[axis.AxisID]].PostionEpsilon = inPositionDistance.Value;
                //                    TypedParameter<int> inPositionTime = _aerotechController.Parameters.Axes[ATECHID[axis.AxisID]].Motion.InPosition.InPositionTime;
                //                    Logger.Information($"InPositionTime = {inPositionTime.Value.ToString()}");
                //                }

                //                // Reset PSO system to be sure to have good condition starting - PSO on one axis only
                //                int psoAxisNumber = -1;
                //                foreach (var axis in ControlledAxesList)
                //                {
                //                    if (axis is AerotechAxisConfig atechAxis)
                //                    {
                //                        if (atechAxis.UsedPSO)
                //                        {
                //                            psoAxisNumber = _aerotechController.Information.Axes[ATECHID[axis.AxisID]].Number;
                //                            break;
                //                        }
                //                    }
                //                    else
                //                        throw new Exception($"{axis.AxisID} axis configuration type does not allow to read if PSO enabled. AerotechAxisConfig is needed.");
                //                }
                //                if (psoAxisNumber >= 0)
                //                {
                //                    _aerotechController.Commands.PSO.Control(psoAxisNumber, PsoMode.Reset);
                //                }

                //                DiagPacketPoller.RefreshInterval = _aerotechControllerConfig.RefreshDiagRate;
                //                _aerotechController.ControlCenter.Diagnostics.NewDiagPacketArrived += new EventHandler<NewDiagPacketArrivedEventArgs>(Diagnostics_NewDiagPacketArrived);

                //                // Check current axes state
                //                ControllerDiagPacket controllerDiagPacket = _aerotechController.DataCollection.RetrieveDiagnostics();
                //                _aerotechController.Commands.Axes[_allControlledAxesMask].Motion.FaultAck();
                //                bool oneAxisIsInFault = false;
                //                bool oneAxisIsMoving = false;
                //                bool allAxesAreHomed = true;
                //                bool allAxesAreEnabled = true;
                //                foreach (var axis in ControlledAxesList)
                //                {
                //                    oneAxisIsInFault |= !controllerDiagPacket[ATECHID[axis.AxisID]].AxisFault.None;
                //                    oneAxisIsMoving |= controllerDiagPacket[ATECHID[axis.AxisID]].AxisStatus.MoveActive;
                //                    allAxesAreHomed |= controllerDiagPacket[ATECHID[axis.AxisID]].AxisStatus.Homed;
                //                    allAxesAreEnabled |= controllerDiagPacket[ATECHID[axis.AxisID]].AxisStatus.Enabled;
                //                }
                //                if (!oneAxisIsInFault)
                //                {
                //                    if (allAxesAreHomed)
                //                    {
                //                        Logger?.Information("Connection to controllers successfully set up");
                //                        State = new DeviceState(Service.Interface.DeviceStatus.Ready);
                //                    }
                //                    else
                //                    {
                //                        Logger?.Warning("One or more axes are not homed");
                //                        State = new DeviceState(Service.Interface.DeviceStatus.Warning);
                //                    }
                //                }
                //                else
                //                {
                //                    Logger?.Error("One or more axes is in fault, acknowledge it before any use");
                //                    State = new DeviceState(Service.Interface.DeviceStatus.Error);
                //                    RaiseStateChangedEvent(new AxesState(allAxesAreEnabled, oneAxisIsMoving));
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            Logger.Error($"Error while configuring controller {_aerotechControllerConfig.DeviceID} : {ex.Message}");
                //            State = new DeviceState(Service.Interface.DeviceStatus.Error);
                //        }
                //    }
                //    if (!controllerFound)
                //    {
                //        Logger.Error($"No controller with name {_aerotechControllerConfig.DeviceID} found, check if you map one to this computer.");
                //        State = new DeviceState(Service.Interface.DeviceStatus.Error);
                //    }
                //}
                //else
                //{
                //    Logger.Error($"No controller found on the network, check if you map one to this computer.");
                //    State = new DeviceState(Service.Interface.DeviceStatus.Error);
                //}
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception in AerotechController.Init() :  {ex.Message} - {ex.StackTrace}");
                State = new DeviceState(Service.Interface.DeviceStatus.Error);
            }
        }

        protected virtual void Diagnostics_NewDiagPacketArrived(object sender, NewDiagPacketArrivedEventArgs e)
        {
            try
            {
                bool allAxesExternalFaultActive = false;
                bool oneAxisStateChangedAtLeast = false;

                foreach (var axis in ControlledAxesList)
                {
                    var currAxis = _axesIDLinks[axis.AxisID];
                    if (!e.Data[currAxis].AxisFault.None)
                    {
                        string errorDesc = e.Data[currAxis].AxisFault.ToString();
                        axis.DeviceError = new Message(MessageLevel.Error, errorDesc, errorDesc, "Axis id: " + axis.AxisID);
                    }

                    if (axis is AerotechAxis atechAxis)
                    {
                        //vérification automatique suite à la fermeture porte pour le chargement. Pour toute autre erreur un acquittement explicite est requis
                        allAxesExternalFaultActive &= (e.Data[currAxis].AxisFault.ActiveBits.Count == 1) && (e.Data[currAxis].AxisFault.ExternalFault);

                        atechAxis.Enabled = e.Data[currAxis].AxisStatus.Enabled;
                        atechAxis.Initialized = e.Data[currAxis].AxisStatus.Homed;
                        atechAxis.CurrentPos = e.Data[currAxis].PositionFeedback.Millimeters();
                        atechAxis.Fault = e.Data[currAxis].AxisFault;
                        atechAxis.Status = e.Data[currAxis].AxisStatus;
                        atechAxis.Moving = e.Data[currAxis].AxisStatus.MoveActive;

                        AxisDiagPacket previousAxisData;
                        bool found = _previousAxisDataDico.TryGetValue(currAxis, out previousAxisData);
                        if (!found)
                        {
                            _previousAxisDataDico.Add(currAxis, e.Data[currAxis]);
                            continue;
                        }
                        oneAxisStateChangedAtLeast |= (previousAxisData.AxisStatus.Enabled != e.Data[currAxis].AxisStatus.Enabled);         // Detection if axis disabled
                        oneAxisStateChangedAtLeast |= (previousAxisData.AxisStatus.MoveActive != e.Data[currAxis].AxisStatus.MoveActive);   // Detection if axis is moving

                        previousAxisData = e.Data[currAxis];
                    }
                }

                if (oneAxisStateChangedAtLeast) RaiseStateChanged(); // Update AxisSate to subscribers

                _isExternalFaultDetectedOnSensor = !(e.Data[_aerotechControllerConfig.ExternalFaultAxisMask].DigitalInput0 > 0); // Signal door state tested > 0 = Closed
                //check Task status to raise associated event
                TaskState NewState = e.Controller.Tasks[TaskId.T01].State;
                if (_currentProgramTaskState == TaskState.ProgramRunning && NewState == TaskState.ProgramComplete)
                {
                    //a program has completed
                    RaiseOnProgramTaskEnded(AerotechTaskEnded.Succeed);
                }
                else if (_currentProgramTaskState == TaskState.ProgramRunning && NewState == TaskState.Error)
                {
                    //a program has completed but an error occurred
                    RaiseOnProgramTaskEnded(IsExternalFaultDetectedOnSensor ? AerotechTaskEnded.ExternalFaultDetected : AerotechTaskEnded.Error);
                }
                if (allAxesExternalFaultActive && !IsExternalFaultDetectedOnSensor)   // All axis in Fault AND Door closed => Acknowledge all fault to release table
                {
                    e.Controller.Commands.AcknowledgeAll(); // Acknowledge fault on all axes in controller
                }
                _currentProgramTaskState = NewState;
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

        public void InitializeUpdate()
        {
        }

        public override bool IsLanded()
        {
            return false; // never landed
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
                if (!(axis is AerotechAxis))
                {
                    // Error
                    String errorMsg = "An axis type is not AerotechAxis";
                    Logger?.Error(errorMsg);
                    throw (new Exception(errorMsg));
                }
            }
            return true;
        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            var position = axis.CurrentPos;
            var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
            return new TimestampedPosition(position, highResolutionDateTime);
        }

        public override void RefreshAxisState(IAxis axis)
        {
        }

        #endregion Private/Protected methods

        #region Position management

        private void MoveTask()
        {
            //int currentMoveNumber = 1;
            //int nbMoves = 100;

            try
            {
                //    double stepX = (_targetAxesPosition.X - _currentAxesPosition.X) / nbMoves;
                //    double stepY = (_targetAxesPosition.Y - _currentAxesPosition.Y) / nbMoves;
                //    double stepZTop = (_targetAxesPosition.ZTop - _currentAxesPosition.ZTop) / nbMoves;
                //    double stepZBottom = (_targetAxesPosition.ZBottom - _currentAxesPosition.ZBottom) / nbMoves;
                //    var stepsZPiezo = new Dictionary<string, double>(); // <axisID, stepZPiezo>

                //    foreach (var targetPiezoPosition in _targetAxesPosition.ZPiezoPositions)
                //    {
                //        string axisID = targetPiezoPosition.AxisID;
                //        double currentPiezoPosition = _currentAxesPosition.GetPiezoPosition(axisID).Position;
                //        double stepZPiezo = (targetPiezoPosition.Position - currentPiezoPosition) / nbMoves;
                //        stepsZPiezo.Add(targetPiezoPosition.AxisID, stepZPiezo);
                //    }

                //    // Early return if there is no move to do
                //    if (IsTargetReached())
                //    {
                //        _logger.Information(FormatMessage("Already on target."));
                //        return;
                //    }

                //    // Moving loop: increment _currentAxesPosition based on the step values and converge to the _targetAxesPosition
                //    while ((currentMoveNumber <= nbMoves) && (!_movementStopped))
                //    {
                //        if (stepX != 0) _currentAxesPosition.X += stepX;
                //        if (stepY != 0) _currentAxesPosition.Y += stepY;
                //        if (stepZTop != 0) _currentAxesPosition.ZTop += stepZTop;
                //        if (stepZBottom != 0) _currentAxesPosition.ZBottom += stepZBottom;

                //        foreach (var stepZPiezo in stepsZPiezo)
                //        {
                //            if (stepZPiezo.Value != 0)
                //            {
                //                var referential = _currentAxesPosition.Referential;
                //                string axisID = stepZPiezo.Key;
                //                double updatedPosition = _currentAxesPosition.GetPiezoPosition(axisID).Position + stepZPiezo.Value;

                //                _currentAxesPosition.AddOrUpdateZPiezoPosition(new ZPiezoPosition(referential, axisID, Length.FromMicrometers(updatedPosition)));
                //            }
                //        }

                //        Thread.Sleep(GetSleepFromSpeed(_axisSpeed));
                //        currentMoveNumber++;
                //    }

                //    _currentAxesPosition = (AnaPosition)_targetAxesPosition.Clone();
                //    _logger.Information(FormatMessage($"Target position reached: {_currentAxesPosition}"));
            }
            catch (Exception)
            {
            }
            finally
            {
                //    _currentState.OneAxisIsMoving = false;
                //    _logger.Information(FormatMessage("Move terminated."));
            }
        }

        public void StartMove(IAxis axisMove, AxisSpeed speed, bool positiveSense)
        {
            throw new NotImplementedException();
        }

        public void StopMove(IAxis axisMove)
        {
            throw new NotImplementedException();
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
            //Nothing
        }

        public override void InitZTopFocus()
        {
            //Nothing
        }

        public override void InitZBottomFocus()
        {
            //Nothing
        }

        #endregion Position management
    }
}
