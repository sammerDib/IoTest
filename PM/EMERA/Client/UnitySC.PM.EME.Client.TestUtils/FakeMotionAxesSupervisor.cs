using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeMotionAxesSupervisor : IEmeraMotionAxesService
    {
        private XYZPosition _currentPosition = new XYZPosition();
        private int _movementNumber = 0;
        private readonly IMessenger _messenger;
        private readonly List<AxisConfig> _axisConfigs = new List<AxisConfig>
        {
            new AxisConfig { AxisID = "X", MovingDirection = MovingDirection.X, PositionMin = -200.Millimeters(), PositionMax = 200.Millimeters() },
            new AxisConfig { AxisID = "Y", MovingDirection = MovingDirection.Y, PositionMin = -200.Millimeters(), PositionMax = 200.Millimeters() },
            new AxisConfig { AxisID = "Z", MovingDirection = MovingDirection.Z, PositionMin = -100.Millimeters(), PositionMax = 100.Millimeters() },
            new MotorizedAxisConfig
            {
                AxisID = "Rotation",
                MovingDirection = MovingDirection.Rotation,
                PositionMin = 0.Millimeters(),
                PositionMax = 6.Millimeters()
            },
        };

        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);

        public FakeMotionAxesSupervisor(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void SetPosition(XYZPosition initialPosition)
        {
            _currentPosition = initialPosition;
            _messenger.Send(_currentPosition);
        }

        public Response<AxesConfig> GetAxesConfiguration()
        {
            return new Response<AxesConfig> { Result = new PhotoLumAxesConfig() { AxisConfigs = _axisConfigs } };
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            return new Response<PositionBase>() { Result = _currentPosition };
        }

        public Response<AxesState> GetCurrentState()
        {
            var state = new AxesState(true, true);
            return new Response<AxesState>() { Result = state };
        }

        public Response<bool> GoToHome(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> GoToPosition(XYZPosition targetPosition, AxisSpeed speed)
        {
            _synchro.Reset();
            _currentPosition = targetPosition;
            _messenger.Send(_currentPosition);
            _synchro.Set();
            _movementNumber++;
            return new Response<VoidResult>();
        }

        public Response<bool> Move(params PMAxisMove[] moves)
        {
            _synchro.Reset();
            foreach (var move in moves)
                switch (move.AxisId)
                {
                    case "X":
                        _currentPosition.X = move.Position.Millimeters;
                        break;
                    case "Y":
                        _currentPosition.Y = move.Position.Millimeters;
                        break;
                    case "Z":
                        _currentPosition.Z = move.Position.Millimeters;
                        break;
                    default:
                        return new Response<bool>() { Result = false };

                }
            _messenger.Send(_currentPosition);
            _movementNumber++;
            _synchro.Set();
            return new Response<bool>() { Result = true };
        }

        public Response<bool> RelativeMove(params PMAxisMove[] moves)
        {
            _synchro.Reset();
            foreach (var move in moves)
                switch (move.AxisId)
                {
                    case "X":
                        _currentPosition.X += move.Position.Millimeters;
                        break;
                    case "Y":
                        _currentPosition.Y += move.Position.Millimeters;
                        break;
                    case "Z":
                        _currentPosition.Z += move.Position.Millimeters;
                        break;
                    default:
                        return new Response<bool>() { Result = false };

                }
            _messenger.Send(_currentPosition);
            _movementNumber++;
            _synchro.Set();
            return new Response<bool>() { Result = true };
        }

        public Response<bool> StopAllMotion()
        {
            return new Response<bool> { Result = true };
        }

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnsubscribeToAxesChanges()
        {
            throw new NotImplementedException();
        }

        public Response<bool> WaitMotionEnd(int timeout)
        {
            _synchro.WaitOne(timeout);
            return new Response<bool> { Result = true };
        }

        public void WaitForMovementNumber(int targetNumber, int timeout)
        {
            var stopWatch = Stopwatch.StartNew();
            while (_movementNumber < targetNumber && stopWatch.ElapsedMilliseconds < timeout)
                WaitMotionEnd(100);
            stopWatch.Stop();
        }

        public Response<bool> GoToEfemLoad(Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }
    }
}
