using System;
using System.Collections.Generic;
using System.Linq;

using Aerotech.Ensemble;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;


namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.Owis
{
    public class OwisMotionDummyController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ILogger _logger;
        private UnknownPosition _position;

        private enum OwisMotionCmds
        { JogBackwardPosition, JogForwardPosition, MoveAbsPosition, MoveRelPosition, HomePosition, RaisePropertiesChanged, CustomCommand }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;

        private enum EFeedbackMsgOwisAxes
        {
            State = 1,
            StateMsg,
            PositionMsg = 10,
            IdMsg,
            CustomMsg,
        }

        public OwisMotionDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;

            _stageServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            _position = new UnknownPosition(new MotorReferential(), 0.0);
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init OwiMotionController as dummy");
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void Connect()
        {
        }

        public override void Connect(string deviceId)
        {
        }

        public override void Disconnect()
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

        public override void CheckControllerIsConnected()
        {
        }

        public override void StopAllMotion()
        {

        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {

        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
            try
            {
                foreach (var axis in AxisList)
                {
                    if (axis is OwisAxis owisAxis)
                    {
                        owisAxis.Enabled = true;
                        owisAxis.Initialized = true;
                        owisAxis.Moving = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error("InitializationAllAxes - OwisMotionDummyController: " + Ex.Message);
                throw;
            }
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public override void HomeAllAxes()
        {
            _position.Position = 0.0;
        }

        public override PositionBase GetPosition()
        {
            return _position;
        }

        public override void RefreshAxisState(IAxis axis)
        {

        }

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _position.Position = move.Position.Millimeters;
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _position.Position += move.Position.Millimeters;
            }
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void CustomCommand(string custom)
        {
        }

        public override void RefreshCurrentPos(List<IAxis> axesList)
        {
            foreach (var axis in axesList)
            {
                switch (axis.AxisID)
                {
                    case "Linear":
                        axis.CurrentPos = _position.Position.Millimeters();
                        break;
                    case "Rotation":
                        axis.CurrentPos = _position.Position.Millimeters();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
