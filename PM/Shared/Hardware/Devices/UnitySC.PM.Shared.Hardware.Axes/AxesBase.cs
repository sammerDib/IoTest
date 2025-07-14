using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public abstract class AxesBase : DeviceBase, IAxes
    {
        #region Fields

        private AxesConfig _axesConfiguration;
        private IGlobalStatusServer _globalStatusServiceCallback;
        protected ILogger Logger;
        protected readonly IReferentialManager ReferentialManager;
        protected bool Initialized = false;

        private List<IAxesController> _axesControllers = new List<IAxesController>();
        private List<IAxis> _axes = new List<IAxis>();
        private IMessenger _messenger;

        protected static string NOT_A_VALID_POSITION = $"Received unsupported position which is not a valid known position type";
        protected static string NOT_A_VALID_MOVE = $"Received unsupported move which is not a valid known move type";

        #endregion Fields

        #region Constructors

        public AxesBase(AxesConfig config, Dictionary<string, ControllerBase> controllersDico,
            IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
            : base(globalStatusServer, logger)
        {
            _axesConfiguration = config;
            Name = config.Name;
            DeviceID = config.DeviceID;
            _globalStatusServiceCallback = globalStatusServer;
            // The logger level must be retrieved from the configuration
            Logger = logger;
            ReferentialManager = referentialManager;
            try
            {
                // Create all axes from config
                foreach (var axisConfig in config.AxisConfigs)
                {
                    Logger.Information($"Create axis '{axisConfig.Name}' with AxisID='{axisConfig.AxisID}' and ControllerID='{axisConfig.ControllerID}'.");

                    // Create axis
                    var newAxis = AxisFactory.CreateAxis(axisConfig, logger);

                    if (_axes.Exists(axe => axe.AxisID == newAxis.AxisID))
                        throw new Exception($"Axis with ID {newAxis.AxisID} already exists. Double Axis ID found in configuration");

                    // Search its controller
                    bool axisControllerFound = controllersDico.TryGetValue(newAxis.AxisConfiguration.ControllerID, out var controller);
                    if (!axisControllerFound) throw new Exception($"Controller with ID '{newAxis.AxisConfiguration.ControllerID}' not found.");
                    Logger.Information($"Controller with ID '{controller.DeviceID}' found.");

                    if (controller is AxesControllerBase axisController)
                    {
                        // Add the axis to the controller's axes list
                        axisController.AxesList.Add(newAxis);
                        Logger.Information($"Association between axis '{newAxis.Name}' with axisID='{newAxis.AxisID}'and controller with controllerID='{controller.DeviceID}' succeeded.");

                        // Add the controller to the axis' controllers list, if not already present
                        if (!AxesControllers.Any(c => c.DeviceID == axisController.DeviceID))
                            AxesControllers.Add(axisController);

                        // Finally add axis to axes
                        _axes.Add(newAxis);
                    }
                    else
                    {
                        throw new Exception($"Wrong type of controller with ID '{controller.DeviceID}'. It should be AxesControllerBase.");
                    }
                }
            }
            catch (Exception ex)
            {
                string msgErr = "Axes creation failed. Check configuration !!!";
                Logger.Information(msgErr);
                throw new Exception(msgErr, ex);
            }
        }

        #endregion Constructors

        #region Properties

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }
        public AxesConfig AxesConfiguration { get => _axesConfiguration; set => _axesConfiguration = value; }
        public new string Name { get => _axesConfiguration.Name; set => _axesConfiguration.Name = value; }
        public new string DeviceID { get => _axesConfiguration.DeviceID; set => _axesConfiguration.DeviceID = value; }
        public override DeviceFamily Family => DeviceFamily.Axes;
        public List<IAxis> Axes { get => _axes; }
        public List<IAxesController> AxesControllers { get => _axesControllers; set => _axesControllers = value; }

        #endregion Properties

        #region Public methods

        public virtual void InitAxesController(List<Message> initErrors)
        {
            Logger.Information(FormatMessage(" Axes controller initialization started"));

            initErrors.Clear();

            foreach (var axesController in AxesControllers)
            {
                if (axesController.ControllerConfiguration.IsEnabled)
                {
                    axesController.InitControllerAxes(initErrors);
                    axesController.WaitMotionEnd(3000);
                }
            }
        }

        public virtual void Init(List<Message> initErrors)
        {
            Logger.Information(FormatMessage(" Axes initialization started"));

            initErrors.Clear();

            // Initialize all controllers used by axes
            foreach (var axesController in AxesControllers)
            {
                if (axesController.ControllerConfiguration.IsEnabled)
                {
                    if (!axesController.IsAxesPositionChangedEventSet) axesController.AxesPositionChangedEvent += NotifyPositionUpdated;
                    if (!axesController.IsAxesStateChangedEventSet) axesController.AxesStateChangedEvent += NotifyStateUpdated;
                    if (!axesController.IsAxesErrorEventSet) axesController.AxesErrorEvent += NotifyErrorUpdated;
                    if (!axesController.IsAxesEndMoveEventSet) axesController.AxesEndMoveEvent += NotifyEndMove;

                    axesController.InitializationAllAxes(initErrors);
                }

                if (!ArePredifinedPositionsValid(axesController))
                    initErrors.Add(new Message(MessageLevel.Warning, $"Wrong predifined positions!", "Axes"));
            }
            Initialized = true;
        }

        public bool ArePredifinedPositionsValid(IAxesController axeController)
        {
            return axeController.AxesList.All(axis => axis.ArePredifinedPositionsConfiguredValid());
        }

        public abstract PositionBase GetPos();

        public abstract void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom);

        public bool IsAtPosition(PositionBase position)
        {
            // Check if Axis.CurrPos already up to date
            foreach (var axesController in AxesControllers)
            {
                var axesList = axesController.GetAxesControlledFromList(_axes);
                if (axesList.Count > 0)
                    axesController.RefreshCurrentPos(axesList);
            }

            return CheckIfPositionReached(position);
        }

        public bool CheckIfPositionReached(PositionBase position)
        {
            var curentPosition = GetPos();                               
            var tolerance = 0.01.Millimeters();
            if (!position.Near(curentPosition, tolerance))
            {
                Logger.Verbose($"Position check failed. Expected position: {position.ToString()}, Current position: {curentPosition.ToString()}, Tolerance: {tolerance}");
                return false;
            }
            else
            {                
                Logger.Debug($"Position check succeeded. Position {position.ToString()} is within tolerance: {tolerance} from {curentPosition.ToString()}");
            }
            return true;
        }
      
        public AxesState GetState()
        {
            bool allAxisEnabled = true;
            bool oneAxisIsMoving = false;
            bool isLanded = false;
            var axesWithLandingList = new List<IAxis>();

            CheckAxesInitialization();

            // Refresh all axes
            foreach (var axesController in AxesControllers)
            {
                var axesList = axesController.GetAxesControlledFromList(_axes);
                if (axesList.Count > 0)
                    axesList.ForEach(a => axesController.RefreshAxisState(a));
            }

            // Update common axes states
            foreach (var axis in _axes)
            {
                allAxisEnabled &= axis.Enabled;
                oneAxisIsMoving |= axis.Moving;
                if (axis.IsLandingUsed)
                    axesWithLandingList.Add(axis);
            }

            if (axesWithLandingList.Count > 0)
            {
                foreach (var axesController in AxesControllers)
                {
                    var axesList = axesController.GetAxesControlledFromList(axesWithLandingList);
                    if (axesList.Count > 0)
                    {
                        if (axesController.IsLanded())
                            isLanded = true;
                    }
                }
            }

            return new AxesState(allAxisEnabled, oneAxisIsMoving, isLanded);
        }

        public virtual void LinearMotion(PositionBase position, AxisSpeed speed)
        {
            if (position is OneAxisPosition pos)
            {
                double axisPos = pos.Position;
                string axisID = pos.AxisID;
                Logger.Information(FormatMessage($"LinearMotion Speed: {speed} {pos.AxisID}: {axisPos}"));

                CheckAxesInitialization();
                var axesList_OneAxis = new List<IAxis>();
                var oneAxis = GetAxis(axisID);

                axesList_OneAxis.Add(oneAxis);

                foreach (var axesController in AxesControllers)
                {
                    var axesList = axesController.GetAxesControlledFromList(axesList_OneAxis);
                    foreach (var axis in axesList)
                    {
                        axesController.LinearMotionSingleAxis(axis, speed, axisPos);
                        axesList_OneAxis.RemoveAll(a1 => axesList.Exists(a2 => a2.AxisID == a1.AxisID));
                    }
                }
                if (axesList_OneAxis.Count != 0)
                {
                    string axesListText = GetAxesListText(axesList_OneAxis);
                    string errorMsg = FormatMessage("Function LinearMotion called - No controller found to move this axes list [" + axesListText + "]");
                    throw new Exception(errorMsg);
                }
            }
            else
            {
                string errMsg = "LinearMotion - " + NOT_A_VALID_POSITION;
                Logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }

        public void StopAllMoves()
        {
            Logger.Information(FormatMessage("Stop all moves"));

            CheckAxesInitialization();

            foreach (var axesController in AxesControllers)
            {
                axesController.StopAxesMotion();
            }
        }

        public virtual void MoveIncremental(IncrementalMoveBase movement, AxisSpeed speed)
        {
            if (movement is OneAxisMove move)
            {
                string axisID = move.AxisID;
                double moveStep = move.Distance;

                if (double.IsNaN(moveStep))
                    throw new Exception("Move value is invalid");

                Logger.Information(FormatMessage($"MoveIncremental Speed: {speed} xStepMillimeters: {moveStep}"));

                if (moveStep == 0)
                    return;

                CheckAxesInitialization();

                var axesList_OneAxis = new List<IAxis>();
                var oneAxis = GetAxis(axisID);
                axesList_OneAxis.Add(oneAxis);

                foreach (var axesController in AxesControllers)
                {
                    var axesList = axesController.GetAxesControlledFromList(axesList_OneAxis);
                    foreach (var axis in axesList)
                    {
                        if (!double.IsNaN(moveStep))
                            axesController.MoveIncremental(axis, speed, moveStep);
                    }
                    axesList_OneAxis.RemoveAll(a1 => axesList.Exists(a2 => a2.AxisID == a1.AxisID));
                }
                if (axesList_OneAxis.Count != 0)
                {
                    string axesListText = GetAxesListText(axesList_OneAxis);
                    string errorMsg = FormatMessage("Function MoveIncremental called - No controller found to move this axes list [" + axesListText + "]");
                    throw new Exception(errorMsg);
                }
            }
            else
            {
                string errMsg = "MoveIncremental - " + NOT_A_VALID_MOVE;
                Logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }

        public void GotoHomePos(AxisSpeed speed)
        {
            Logger.Information(FormatMessage("GotoHomePos"));

            CheckAxesInitialization();

            var axesList_HomePos = GetAllAxes_HomePos();

            foreach (var axesController in AxesControllers)
            {
                var axesList = axesController.GetAxesControlledFromList(axesList_HomePos);
                var speedList = new List<AxisSpeed>();
                axesList.ForEach(a => speedList.Add(speed));
                axesController.GotoHomePos(axesList, speedList);
                axesList_HomePos.RemoveAll(a1 => axesList.Exists(a2 => a2.AxisID == a1.AxisID));
            }
            if (axesList_HomePos.Count != 0)
            {
                string axisListText = GetAxesListText(axesList_HomePos);
                string errorMsg = FormatMessage("Function GotoHomePos called - No controller found to move this axes list [" + axisListText + "]");
                throw new Exception(errorMsg);
            }
        }

        public void StopLanding()
        {
            Logger.Information(FormatMessage("StopLanding"));

            CheckAxesInitialization();

            AxesControllers.ForEach(axeController =>
            {
                foreach (var axis in axeController.AxesList)
                {
                    if (axis.IsLandingUsed)
                    {
                        axeController.StopLanding();
                        break; // do no check others axes
                    }
                }
            });
        }

        public void Land()
        {
            Logger.Information(FormatMessage("Land"));
            CheckAxesInitialization();

            AxesControllers.ForEach(axeController =>
            {
                foreach (var axis in axeController.AxesList)
                {
                    if (axis.IsLandingUsed)
                    {
                        axeController.Land();
                        break;  // do no check others axes
                    }
                }
            });
        }

        public void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            CheckAxesInitialization();

            var startTime = DateTime.Now;
            var taskList = new List<Task>();

            AxesControllers.ForEach(axesController =>
            {
                var WaitTimeout_AxesController = Task.Run(() =>
                {
                    Thread.CurrentThread.Name = "WaitTimeout[" + axesController.DeviceID + "]#" + Task.CurrentId.ToString();
                    axesController.WaitMotionEnd(timeout, waitStabilization);
                });
                taskList.Add(WaitTimeout_AxesController);
            });

            if (!Task.WaitAll(taskList.ToArray(), timeout))
            {
                string errorMsg = FormatMessage("Function WaitMotionEnd called - Motion not complete in time");
                throw new Exception(errorMsg);
            }
        }

        // TODO: factorize with TMapAxes.GotoPosition()
        public virtual void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            var positionInMotorReferential = ConvertTo(position, ReferentialTag.Motor);
            if (positionInMotorReferential is OneAxisPosition pos)
            {
                double axisPos = pos.Position;
                string axisID = pos.AxisID;

                if (double.IsNaN(axisPos))
                    throw new Exception("GotoPosition - Position value is invalid");

                Logger.Information(FormatMessage($"GotoPosition - Speed: {speed} {axisID}: {axisPos}"));

                CheckAxesInitialization();

                var targetAxisList = new List<IAxis>() { };
                var targetAxisCoords = new List<double>() { };
                var targetAxisSpeeds = new List<AxisSpeed>() { };

                var oneAxis = GetAxis(axisID);

                targetAxisList.Add(oneAxis);
                targetAxisCoords.Add(axisPos);
                targetAxisSpeeds.Add(speed);

                SetPosAxisToAllControllers(targetAxisList, targetAxisCoords, targetAxisSpeeds);
            }
            else
            {
                string errMsg = "GotoPosition - " + NOT_A_VALID_POSITION;
                Logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }

        #endregion Public methods

        protected PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return ReferentialManager.ConvertTo(positionToConvert, referentialTo);
        }

        #region Private methods
        protected void CheckAxesInitialization()
        {
            if (!Initialized)
            {
                string errorMsg = FormatMessage("The Axes controller is not initialized. Initialization process may be running, but is not finished");
                Logger.Warning(errorMsg);
                throw (new Exception(errorMsg));
            }

            if (AxesControllers == null)
            {
                string errorMsg = FormatMessage("The axes _axesControllerList is null");
                Logger.Warning(errorMsg);
                throw (new Exception(errorMsg));
            }
        }

        protected string FormatMessage(string message)
        {
            return $"{DeviceID} {message}";
        }

        protected void SetPosAxisToAllControllers(List<IAxis> axes, List<double> positions, List<AxisSpeed> speeds)
        {
            foreach (var axesController in AxesControllers)
            {
                var selectedAxesList = axesController.GetAxesControlledFromList(axes);
                if (selectedAxesList.Count > 0)
                {
                    var selectedPositions = new List<double>();
                    var selectedSpeeds = new List<AxisSpeed>();

                    foreach (var axis in selectedAxesList)
                    {
                        int idx = axes.FindIndex(a => a.AxisID == axis.AxisID);
                        selectedPositions.Add(positions[idx]);
                        selectedSpeeds.Add(speeds[idx]);

                        axes.RemoveAt(idx);
                        positions.RemoveAt(idx);
                        speeds.RemoveAt(idx);
                    }
                    axesController.SetPosAxis(selectedPositions, selectedAxesList, selectedSpeeds);
                }
            }

            if (axes.Count != 0)
            {
                string axisListText = GetAxesListText(axes);
                string errorMsg = FormatMessage("Function GotoPosition called - No controller found to move this axes list [" + axisListText + "]");
                throw new Exception(errorMsg);
            }
        }

        protected void SetPosAxisWithSpeedAndAccelToAllControllers(List<IAxis> axesList, List<double> cordsList, List<double> speedList, List<double> accelList)
        {
            foreach (var axesController in AxesControllers)
            {
                var selectedAxesList = axesController.GetAxesControlledFromList(axesList);
                if (selectedAxesList.Count > 0)
                {
                    var selectedCordsList = new List<double>();
                    var selectedSpeedList = new List<double>();
                    var selectedAccelsList = new List<double>();

                    foreach (var axis in selectedAxesList)
                    {
                        int idx = axesList.FindIndex(a => a.AxisID == axis.AxisID);
                        selectedCordsList.Add(cordsList[idx]);
                        selectedSpeedList.Add(speedList[idx]);
                        selectedAccelsList.Add(accelList[idx]);

                        axesList.RemoveAt(idx);
                        cordsList.RemoveAt(idx);
                        speedList.RemoveAt(idx);
                        accelList.RemoveAt(idx);
                    }
                    axesController.SetPosAxisWithSpeedAndAccel(selectedCordsList, selectedAxesList, selectedSpeedList, selectedAccelsList);
                }
            }

            if (axesList.Count != 0)
            {
                string axisListText = GetAxesListText(axesList);
                string errorMsg = FormatMessage("Function SetPosAxisWithSpeedAndAccelToAllControllers called - No controller found to move this axes list [" + axisListText + "]");
                throw new Exception(errorMsg);
            }
        }

        protected string GetAxesListText(List<IAxis> axesList)
        {
            var axisIDs = axesList.Select(axe => axe.AxisID);
            return string.Join(",", axisIDs);
        }

        public virtual void NotifyPositionUpdated(PositionBase axesPosition)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.PositionChanged(axesPosition);
            AxesPositionChangedMessage axesPositionMsg = new AxesPositionChangedMessage() { Position = axesPosition };
            Messenger.Send<AxesPositionChangedMessage>(axesPositionMsg);
        }

        // FIXME: we should aggregate AxesState received from all controllers, before sending it
        // (not sending received AxesState as it is).
        private void NotifyStateUpdated(AxesState axesState)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.StateChanged(axesState);
        }

        private void NotifyErrorUpdated(Message message)
        {
            _globalStatusServiceCallback.SetGlobalStatus(new GlobalStatus(message));
        }

        private void NotifyEndMove(bool targetReached)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.EndMove(targetReached);
        }

        protected List<IAxis> GetAllAxes_ManualLoad()
        {
            return _axes.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionManualLoad.Millimeters)).ToList();
        }

        protected List<IAxis> GetAllAxes_ParkPos()
        {
            return _axes.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionPark.Millimeters)).ToList();
        }

        protected List<IAxis> GetAllAxes_HomePos()
        {
            return _axes.Where(axis => !double.IsNaN(axis.AxisConfiguration.PositionHome.Millimeters)).ToList();
        }

        /// <summary>
        /// Returns the axis with the given ID or throws an exception if not found.
        /// </summary>
        /// <param name="axisID"></param>
        protected IAxis GetAxis(string axisID)
        {
            var axis = Axes.FirstOrDefault(a => a.AxisID == axisID);
            if (axis is null) throw new Exception($"Axis '{axisID}' is missing from AxesList.");
            return axis;
        }

        protected void AddMoveToListsIfValid(AxisMove axisMove, string axisID, ref List<IAxis> targetAxisList, ref List<double> targetAxisCoords, ref List<double> targetAxisSpeeds, ref List<double> targetAxisAccels)
        {
            if ((axisMove != null) && !double.IsNaN(axisMove.Position))
            {
                var axis = GetAxis(axisID);

                targetAxisList.Add(axis);
                targetAxisCoords.Add(axisMove.Position);
                targetAxisSpeeds.Add(axisMove.Speed);
                targetAxisAccels.Add(axisMove.Accel);
            }
        }

        protected void AddPositionToListsIfValid(OneAxisPosition axisPosition, AxisSpeed axisSpeed, ref List<IAxis> targetAxisList, ref List<double> targetAxisPositions, ref List<AxisSpeed> targetAxisSpeeds)
        {
            if ((axisPosition != null) && !double.IsNaN(axisPosition.Position))
            {
                var axis = GetAxis(axisPosition.AxisID);

                targetAxisList.Add(axis);
                targetAxisPositions.Add(axisPosition.Position);
                targetAxisSpeeds.Add(axisSpeed);
            }
        }

        public void Move(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public void Home(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void StopAllMotion()
        {
            throw new NotImplementedException();
        }

        public PositionBase GetPosition()
        {
            throw new NotImplementedException();
        }

        public abstract TimestampedPosition GetAxisPosWithTimestamp(IAxis axis);

        public void ResetAxis()
        {
            var errors = new List<Message>();
            try
            {
                InitAxesController(errors);
                Init(errors);

                Logger.Information("Successfully reset axes");
                _globalStatusServiceCallback?.SetGlobalState(PMGlobalStates.Busy);
            }
            catch (Exception e)
            {
                Logger.Error("Axis reset failed. Proceed with the hardware reset : " + e.Message);
                _globalStatusServiceCallback?.SetGlobalState(PMGlobalStates.Error);
                throw;
            }
        }

        public void AcknowledgeResetAxis()
        {
            _globalStatusServiceCallback?.SetGlobalState(PMGlobalStates.Error);
        }

        public void InitZTopFocus()
        {
            Logger.Information("Initialize ZTop focus (UOH)");
            foreach (var axesController in AxesControllers)
            {
                axesController.InitZTopFocus();
            }
            Logger.Information("Initialize ZTop focus (UOH) is completed");
        }

        public void InitZBottomFocus()
        {
            Logger.Information("Initialize ZBottom focus (LOH)");
            foreach (var axesController in AxesControllers)
            {
                axesController.InitZBottomFocus();
            }
            Logger.Information("Initialize ZBottom focus (LOH) is completed");
        }

        #endregion Private methods
    }
}
