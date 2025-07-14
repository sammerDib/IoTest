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

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Parallax
{
    public class ParallaxMotionDummyController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private double _position;
        private RotationPosition _rotPosition;

        private enum Std900Cmds
        { MoveAbsPosition, RaisePropertiesChanged }

        private enum EFeedbackMsgStd900
        {
            State = 1,
            PositionMsg = 10,
            IdMsg
        }

        public ParallaxMotionDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _position = 0.0;
            _rotPosition = new RotationPosition(new MotorReferential(), _position);
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init ParallaxMotionController as dummy");
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
            throw new NotImplementedException();
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public override void HomeAllAxes()
        {
        }

        public override PositionBase GetPosition()
        {
            return _rotPosition;
        }

        public override void RefreshAxisState(IAxis axis)
        {
            throw new NotImplementedException();
        }

        public void Move(params PMAxisMove[] moves)
        {
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void CustomCommand(string custom)
        {
            throw new NotImplementedException();
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {
            throw new NotImplementedException();
        }
    }
}
