using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Referentials.Interface.Positions;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.AGS.Hardware.Axes.Axes
{
    public class EdgeAxes : ArgosAxesBase
    {
        private IGlobalStatusServer _globalStatusServiceCallback;
        private ILogger _logger;

        private readonly IReferentialManager _referentialManager;

        private List<IAxis> _axes = new List<IAxis>();

        //CONTROLLERS
        private readonly IPositionSynchronizedOutput _psoController;

        private readonly Dictionary<string, IMotion> _axisIdToIMotion = new Dictionary<string, IMotion>();
        private List<MotionControllerBase> _motionControllers = new List<MotionControllerBase>();

        public List<MotionControllerBase> MotionControllers
        {
            get => _motionControllers;
            set => _motionControllers = value;
        }

        public EdgeAxes(AxesConfig config, Dictionary<string, MotionControllerBase> controllers,
            IGlobalStatusServer globalStatusServer, HardwareLogger logger, IReferentialManager referentialManager)
            : base(config, globalStatusServer, logger)
        {
            _globalStatusServiceCallback = globalStatusServer;
            _logger = logger;
            _referentialManager = referentialManager;
            try
            {
                // TODO ADD REQUIREMENTS IF NEEDED
                // Create all axes from config
                foreach (var axisConfig in AxesConfiguration.AxisConfigs)
                {
                    _logger.Information(
                        $"Create axis '{axisConfig.Name}' with AxisID='{axisConfig.AxisID}' and ControllerID='{axisConfig.ControllerID}'.");
                    // Create axis
                    var newAxis = AxisFactory.CreateAxis(axisConfig, _logger);
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

                    //check controller manages required axes
                    if (!controller.IsAxisManaged(newAxis))
                    {
                        throw new Exception(
                            $"Axis {newAxis.AxisID} tried to link to controller {controller.DeviceID} but the controller does not manage that axis, check controller configuration");
                    }

                    _axisIdToIMotion.Add(newAxis.AxisID, controller as IMotion);
                    if (newAxis.AxisID == "T")
                    {
                        _psoController = controller as IPositionSynchronizedOutput;
                    }

                    // Add the axis to the controller's axes list
                    controller.AxisList.Add(newAxis);
                    _logger.Information(
                        $"Association between axis '{newAxis.Name}' with axisID='{newAxis.AxisID}'and controller with controllerID='{controller.DeviceID}' succeeded.");

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
                _logger.Information(msgErr);
                throw new Exception(msgErr, ex);
            }
        }

        public override void Init(List<Message> initErrors)
        {
            _logger.Information(FormatMessage(" Axes initialization started"));

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
                    initErrors.Add(new Message(MessageLevel.Warning, $"Wrong predifined positions!", "Axes"));
            }
        }

        public override void StopAllMotion()
        {
            foreach (var motionController in _motionControllers)
            {
                motionController.StopAllMotion();
            }
        }

        public override void GoToLoadUnload(AxisSpeed aSpeed)
        {
            foreach (var axisToInterface in _axisIdToIMotion)
            {
                var conf = GetAxisConfig(axisToInterface.Key);
                var speed = ConvertAxisSpeed(aSpeed, conf);
                var pos = new Length(conf.PositionManualLoad, LengthUnit.Millimeter);
                var move = new PMAxisMove(axisToInterface.Key, pos, speed);
                axisToInterface.Value.Move(move);
            }
        }

        public override void GoToHomePosition(AxisSpeed aSpeed)
        {
            foreach (var axisToInterface in _axisIdToIMotion)
            {
                var conf = GetAxisConfig(axisToInterface.Key);
                var speed = ConvertAxisSpeed(aSpeed, conf);
                var pos = new Length(conf.PositionHome, LengthUnit.Millimeter);
                var move = new PMAxisMove(axisToInterface.Key, pos, speed);
                axisToInterface.Value.Move(move);
            }
        }

        public override PositionBase GetPositon()
        {

            return new XTPosition(new MotorReferential(), 0, 0);

            // TODO GVA: A mettre à jour lors du refacto sur les axes/motionAxes
            /*var itemList = new List<PositionBase>();
            foreach (var controller in _motionControllers)
            {
                itemList.AddRange(controller.GetPosition().GetPositionItems());
            }            

            AxisPositionsList position = new AxisPositionsList();
            foreach (var item in itemList)
            {
                position.Add(item);
            }
            return position;*/
        }

        public override void Home(AxisSpeed speed)
        {
            foreach (var motionController in _motionControllers)
            {
                motionController.HomeAllAxes();
            }
        }

        protected string FormatMessage(string message)
        {
            return $"ArgosPMAxes - {message}";
        }

        public bool ArePredifinedPositionsValid(MotionControllerBase motionController)
        {
            return motionController.AxisList.All(axis => axis.ArePredifinedPositionsConfiguredValid());
        }

        public virtual void NotifyPositionUpdated(PositionBase axesPosition)
        {
            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.PositionChanged(axesPosition);
        }

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

        public AxisConfig GetAxisConfig(string axisId)
        {
            return _axes.Find(axis => axis.AxisID == axisId).AxisConfiguration;
        }

        public override void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _axisIdToIMotion[move.AxisId].Move(new[] { move });
            }
        }

        public override void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _axisIdToIMotion[move.AxisId].RelativeMove(new[] { move });
            }
        }

        public override void WaitMotionEnd(int timeout_ms)
        {
            foreach (var motionController in _motionControllers)
            {
                motionController.WaitMotionEnd(timeout_ms);
            }
        }

        public override double GetNearestPSOPixelSize(double pixelSizeMicrometers, double waferRadiusMillimeters)
        {
            return _psoController.GetNearestPSOPixelSize(pixelSizeMicrometers, waferRadiusMillimeters);
        }

        public override void SetPSOInFixedWindowMode(double beginPosition, double endPosition,
            double psoAngularInterval)
        {
            _psoController.SetPSOInFixedWindowMode(beginPosition, endPosition, psoAngularInterval);
        }

        public override void DisablePSO()
        {
            _psoController.DisablePSO();
        }

        public override DeviceFamily Family { get => DeviceFamily.Axes; }
    }
}
