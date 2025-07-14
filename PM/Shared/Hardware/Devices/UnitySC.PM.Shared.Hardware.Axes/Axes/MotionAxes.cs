using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class MotionAxes : MotionAxesBase
    {
        private IGlobalStatusServer _globalStatusServiceCallback;

        private List<IAxis> _axes = new List<IAxis>();

        private readonly Dictionary<string, IMotion> _axisIdToIMotion = new Dictionary<string, IMotion>();
        private List<MotionControllerBase> _motionControllers = new List<MotionControllerBase>();
        protected bool Initialized = false;

        public List<MotionControllerBase> MotionControllers
        {
            get => _motionControllers;
            set => _motionControllers = value;
        }

        public List<IAxis> Axes { get => _axes; }

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
        public MotionAxes(AxesConfig config, Dictionary<string, MotionControllerBase> controllers, IGlobalStatusServer globalStatusServer,
            ILogger logger, IReferentialManager referentialManager) : base(config, globalStatusServer, logger, referentialManager)
        {
            _globalStatusServiceCallback = globalStatusServer;
            try
            {
                foreach (var axisConfig in AxesConfiguration.AxisConfigs)
                {
                    this.Logger.Information($"Create axis '{axisConfig.Name}' with AxisID='{axisConfig.AxisID}' and ControllerID='{axisConfig.ControllerID}'.");
                    var newAxis = AxisFactory.CreateAxis(axisConfig, this.Logger);
                    string axId = newAxis.AxisID;
                    if (_axes.Exists(axe => axe.AxisID == axId))
                    {
                        throw new Exception($"Axis with ID {axId} already exists. Double Axis ID found in configuration");
                    }

                    bool axisControllerFound = controllers.TryGetValue(newAxis.AxisConfiguration.ControllerID, out var controller);
                    if (!axisControllerFound)
                    {
                        throw new Exception($"Controller with ID '{newAxis.AxisConfiguration.ControllerID}' not found.");
                    }

                    _axisIdToIMotion.Add(newAxis.AxisID, controller as IMotion);

                    // Add the axis to the controller's axes list
                    controller.AxisList.Add(newAxis);
                    this.Logger.Information($"Association between axis '{newAxis.Name}' with axisID='{newAxis.AxisID}' " +
                                        $"and controller with controllerID='{controller.DeviceID}' succeeded.");

                    // Add the controller to the axis' controllers list, if not already present
                    if (_motionControllers.All(c => c.DeviceID != controller.DeviceID))
                    {
                        _motionControllers.Add(controller);
                    }

                    // Finally add axis to axes
                    _axes.Add(newAxis);
                }
            }
            catch (Exception ex)
            {
                string msgErr = "Axes creation failed. Check configuration !!!";
                this.Logger.Information(msgErr);
                throw new Exception(msgErr, ex);
            }
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information($"MotionAxes initialization started");

            initErrors.Clear();

            // Initialize all controllers used by axes
            foreach (var motionController in MotionControllers)
            {
                if (motionController.ControllerConfiguration.IsEnabled)
                {
                    motionController.InitializeAllAxes(initErrors);

                    if (!motionController.IsAxesPositionChangedEventSet) motionController.AxesPositionChangedEvent += NotifyPositionUpdated;
                    if (!motionController.IsAxesStateChangedEventSet) motionController.AxesStateChangedEvent += NotifyStateUpdated;
                    if (!motionController.IsAxesErrorEventSet) motionController.AxesErrorEvent += NotifyErrorUpdated;
                    if (!motionController.IsAxesEndMoveEventSet) motionController.AxesEndMoveEvent += NotifyEndMove;
                }

                if (!ArePredifinedPositionsValid(motionController))
                    initErrors.Add(new Message(MessageLevel.Warning, $"Wrong predefined positions!", "Axes"));
            }
            Initialized = true;
        }

        public override void StopAllMotion()
        {
            foreach (var motionController in _motionControllers)
            {
                motionController.StopAllMotion();
            }
        }

        public override PositionBase GetPosition()
        {
            var axes = new List<IAxis>(2)
            {
                GetAxis("Linear"),
            };
            try
            {
                axes.Add(GetAxis("Rotation"));
            }
            catch
            {
            }

            axes.ForEach(axis =>
            {
                if (axis != null && _axisIdToIMotion.TryGetValue(axis.AxisID, out var controller))
                {
                    if (controller is MotionControllerBase)
                    {
                        axis.CurrentPos = ((MotionControllerBase)controller).GetPosition().ToOnePosition().Millimeters();
                    }
                    else
                    {
                        axis.CurrentPos = null;
                    }
                }
            });

            return new XTPosition(new MotorReferential(), axes[0].CurrentPos.Millimeters, axes.Count() == 2 ? axes[1].CurrentPos.Millimeters : 0.0);
        }
        public override AxesState GetCurrentState()
        {
            bool allAxisEnabled = true;
            bool oneAxisIsMoving = false;          
            // Refresh all axes
            foreach (var motionController in _motionControllers)
            {
                var axesList = motionController.GetAxesControlledFromList(_axes);
                axesList?.ForEach(a => motionController.RefreshAxisState(a));
            }
            // Update common axes states
            foreach (var axis in _axes)
            {
                allAxisEnabled &= axis.Enabled;
                oneAxisIsMoving |= axis.Moving;                
            }
            return new AxesState(allAxisEnabled, oneAxisIsMoving, false);
        }

        public override void Home(AxisSpeed speed = 0)
        {
            foreach (var motionController in _motionControllers)
            {
                motionController.HomeAllAxes();
            }
        }

        public bool ArePredifinedPositionsValid(MotionControllerBase motionController)
        {
            return motionController.AxisList.All(axis => axis.ArePredifinedPositionsConfiguredValid());
        }

        public virtual void NotifyPositionUpdated(PositionBase axesPosition)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            axesServiceCallback.PositionChanged(axesPosition);
            AxesPositionChangedMessage axesPositionMsg = new AxesPositionChangedMessage() { Position = axesPosition };
            Messenger.Send<AxesPositionChangedMessage>(axesPositionMsg);

        }

        private void NotifyStateUpdated(AxesState axesState)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            axesServiceCallback.StateChanged(axesState);
        }

        private void NotifyErrorUpdated(Message message)
        {
            _globalStatusServiceCallback.SetGlobalStatus(new GlobalStatus(message));
        }

        private void NotifyEndMove(bool targetReached)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            axesServiceCallback.EndMove(targetReached);
        }

        public AxisConfig GetAxisConfig(string axisId)
        {
            return _axes.Find(axis => axis.AxisID == axisId).AxisConfiguration;
        }

        protected IAxis GetAxis(string axisID)
        {
            var axis = _axes.FirstOrDefault(a => a.AxisID == axisID);
            if (axis is null) throw new Exception($"Axis '{axisID}' is missing from AxesList.");
            return axis;
        }

        public override void Move(params PMAxisMove[] moves)
        {
            var movesGroupedByIMotion = moves.GroupBy(move => _axisIdToIMotion[move.AxisId]);
            foreach (var groupedMoves in movesGroupedByIMotion)
            {
                groupedMoves.Key.Move(groupedMoves.ToArray());
            }
        }

        public override void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _axisIdToIMotion[move.AxisId].RelativeMove(new[] { move });
            }
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            var endWatingTime = DateTime.Now.AddMilliseconds(timeout_ms);
            foreach (var motionController in _motionControllers)
            {
                var motionControllerTimeout = (int)(endWatingTime - DateTime.Now).TotalMilliseconds;
                if (motionControllerTimeout <= 0)
                    motionControllerTimeout = 1;
                motionController.WaitMotionEnd(motionControllerTimeout, waitStabilization);
            }
        }

        protected void CheckAxesInitialization()
        {
            if (!Initialized)
            {
                string errorMsg = "The Axes controller is not initialized. Initialization process may be running, but is not finished";
                Logger.Warning(errorMsg);
                throw (new Exception(errorMsg));
            }

            if (MotionControllers == null)
            {
                string errorMsg = "The axes _axesMotionControllerList is null";
                Logger.Warning(errorMsg);
                throw (new Exception(errorMsg));
            }
        }

        public override bool IsAtPosition(PositionBase position)
        {
            // Check if Axis.CurrPos already up to date
            foreach (var motionController in _motionControllers)
            {
                var axesList = motionController.GetAxesControlledFromList(_axes);
                foreach (var axis in axesList)
                {
                    if (axis.Moving) 
                        return false;
                }
                if (axesList.Count > 0)
                    motionController.RefreshCurrentPos(axesList);
                 
            }

            return CheckIfPositionReached(position);
        }

        public override bool CheckIfPositionReached(PositionBase position)
        {
            var curentPosition = GetPosition();
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
    }
}
