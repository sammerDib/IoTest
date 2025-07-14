using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class AerotechMotionDummyController : MotionControllerBase, IMotion, IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private Task _moveTask;

        private readonly List<string> _axisIds;
        private readonly XYZPosition _currentPosition;

        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);

        public AerotechMotionDummyController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            _currentPosition = new XYZPosition(new MotorReferential(), 0.0, 0.0, 0.0);

            if (controllerConfig is AerotechControllerConfig aerotechControllerConfig)
            {
                _axisIds = aerotechControllerConfig.AerotechAxisIDLinks.Select(axis => axis.AxisID).ToList();
            }
            
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init AerotechMotionController as dummy");
        }

        public override void CheckControllerIsConnected()
        {
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

        public override void Disconnect(string deviceId)
        {
        }

        public void Dispose()
        {

        }

        public override PositionBase GetPosition()
        {
            return _currentPosition;
        }

        public override void HomeAllAxes()
        {
            try
            {
                var homeMoves = new PMAxisMove[]
                {
                    new PMAxisMove("X", new Length(0.0, LengthUnit.Millimeter)),
                    new PMAxisMove("Y", new Length(0.0, LengthUnit.Millimeter)),
                    new PMAxisMove("Z", new Length(0.0, LengthUnit.Millimeter))
                };

                Move(homeMoves);

                RaiseStateChangedEvent(new AxesState(allAxisEnabled: true, oneAxisIsMoving: false));
                Logger?.Information("All axes homed successfully.");
            }
            catch (Exception ex)
            {
                Logger?.Error($"HomeAllAxes - AerotechMotionDummyController: {ex.Message}");
                throw;
            }
        }


        public override void InitializeAllAxes(List<Message> initErrors)
        {
            foreach (var axis in AxisList.OfType<AerotechAxis>())
            {
                axis.Enabled = true;
                axis.Initialized = true;
                axis.Moving = false;
            }
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIds.Any(x => x == axis.AxisID);
        }

        public override void RefreshAxisState(IAxis axis)
        {

        }

        public override bool ResetController()
        {
            return true;
        }

        public override void StopAllMotion()
        {
            try
            {
                _cancellationTokenSource.Cancel(); // Stop the movement task
                _moveTask?.Wait(); // Wait for the task to complete
                RaiseStateChangedEvent(new AxesState(allAxisEnabled: true, oneAxisIsMoving: false));
            }
            catch (Exception ex)
            {
                Logger?.Error($"StopAllMotion - AerotechException: {ex.Message} - {ex.StackTrace}");
                throw;
            }
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            if (_moveTask == null || _moveTask.IsCompleted)
            {
                return;
            }
            _synchro.WaitOne(timeout);
        }

        public void Move(params PMAxisMove[] moves)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _moveTask = Task.Run(() => PerformMove(moves, cancellationToken), cancellationToken);
        }
        private async Task PerformMove(PMAxisMove[] moves, CancellationToken cancellationToken)
        {
            _synchro.Reset();

            try
            {
                RaiseStateChangedEvent(new AxesState(allAxisEnabled: false, oneAxisIsMoving: true));

                var destination = GetDestination(moves);
                while (!IsDestinationReached(destination))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Wait for 10 ms to simulate hardware movement
                    await Task.Delay(10, cancellationToken);
                    
                    foreach (var move in moves)
                    {
                        double step = 0.5;
                        switch (move.AxisId)
                        {
                            case "X":
                                _currentPosition.X += DeltaPosition(_currentPosition.X, destination.X, step);
                                ChangeAxisPosition("X", _currentPosition.X);
                                break;
                            case "Y":
                                _currentPosition.Y += DeltaPosition(_currentPosition.Y, destination.Y, step);
                                ChangeAxisPosition("Y", _currentPosition.Y);
                                break;
                            case "Z":
                                _currentPosition.Z += DeltaPosition(_currentPosition.Z, destination.Z, step);
                                ChangeAxisPosition("Y", _currentPosition.Y);
                                break;
                            default:
                                return;
                        }
                    }
                }

                var stringBuilder = new StringBuilder("Destination reached for simulated aerotech controller: ");
                if (_axisIds.Contains("X"))
                    stringBuilder.Append($"X={_currentPosition.X} ");
                if (_axisIds.Contains("Y"))
                    stringBuilder.Append($"Y={_currentPosition.Y} ");
                if (_axisIds.Contains("Z"))
                    stringBuilder.Append($"Z={_currentPosition.Z} ");
                Logger?.Information($"{stringBuilder}");
                RaiseStateChangedEvent(new AxesState(allAxisEnabled: true, oneAxisIsMoving: false));
            }
            catch (TaskCanceledException)
            {
                Logger?.Information("Task canceled. Exiting PerformMove.");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Exception in PerformMove: {ex}");
            }

            _synchro.Set();
        }

        private XYZPosition GetDestination(PMAxisMove[] moves)
        {
            double destinationX = moves.FirstOrDefault(move => move.AxisId == "X")?.Position.Millimeters ??
                                  _currentPosition.X;
            double destinationY = moves.FirstOrDefault(move => move.AxisId == "Y")?.Position.Millimeters ??
                                  _currentPosition.Y;
            double destinationZ = moves.FirstOrDefault(move => move.AxisId == "Z")?.Position.Millimeters ??
                                  _currentPosition.Z;
            var destination = new XYZPosition(new MotorReferential(), destinationX, destinationY, destinationZ);
            return destination;
        }

        private bool IsDestinationReached(XYZPosition destination)
        {
            return Math.Abs(_currentPosition.X - destination.X) <= 0.01
                   && Math.Abs(_currentPosition.Y - destination.Y) <= 0.01
                   && Math.Abs(_currentPosition.Z - destination.Z) <= 0.01;
        }

        private double DeltaPosition(double currentPosition, double destination, double step)
        {
            if (destination - currentPosition > step)
            {
                return step;
            }

            if (destination - currentPosition < -step)
            {
                return -step;
            }

            return destination - currentPosition;
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
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
                        return;

                }
            }
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
            }
        }
        public override void RefreshCurrentPos(List<IAxis> axesList)
        {
            foreach (var axis in axesList.OfType<AerotechAxis>())
            {
                switch (axis.AxisID)
                {
                    case "X":
                        axis.CurrentPos = _currentPosition.X.Millimeters();
                        break;
                    case "Y":
                        axis.CurrentPos = _currentPosition.Y.Millimeters();
                        break;
                    case "Z":
                        axis.CurrentPos = _currentPosition.Z.Millimeters();
                        break;
                }
            }
        }
    }
}
