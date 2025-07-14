using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Aerotech.Ensemble;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.CNC
{
    public class CNCMotionDummyController : MotionControllerBase, IMotion
    {
        private readonly CNCMotionControllerConfig _cncControllerConfig;
        private Dictionary<string, Length> _positions;
        private XYZPosition _currentPosition;
        private Length _currentRotation;

        //In milimeters
        private Length _currentFilterPos;

        private XYZPosition _destinationPosition;
        private Task _moveTask;
        private readonly Stack<CancellationTokenSource> _cancellationTokenSources = new Stack<CancellationTokenSource>();

        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);

        private const double IncrementalMovePrecision = 0.001;
        private const double IncrementalMoveStepSize = 0.1;

        public CNCMotionDummyController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(controllerConfig, globalStatusServer, logger)
        {
            _cncControllerConfig = controllerConfig as CNCMotionControllerConfig;
            _currentPosition = new XYZPosition(new MotorReferential(), 0.0, 0.0, 0.0);
            _currentRotation = 0.Millimeters();
            _destinationPosition = new XYZPosition(new MotorReferential(), 0, 0, 0);

            if (_cncControllerConfig == null)
                throw (new Exception("Bad controller configuration type. Controller creation failed !"));
            _positions = new Dictionary<string, Length>();
        }
        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init CNCMotionController as dummy");
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

        public override void Disconnect(string deviceID)
        {
        }

        public override PositionBase GetPosition()
        {
            throw new NotImplementedException();
        }

        public override void HomeAllAxes()
        {
            foreach (var item in _positions.ToArray())
            {
                _positions[item.Key] = new Length(0.0, LengthUnit.Millimeter);
            }
        }


        public override void InitializeAllAxes(List<Message> initErrors)
        {
            try
            {
                foreach (var axis in AxisList)
                {
                    if (axis is AerotechAxis cncAxix)
                    {
                        cncAxix.Enabled = true;
                        cncAxix.Initialized = true;
                        cncAxix.Moving = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error("InitializationAllAxes - AerotechMotionDummyController: " + Ex.Message);
                throw;
            }
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                switch (move.AxisId)
                {
                    case "X":
                        _destinationPosition.X = move.Position.Millimeters;
                        break;

                    case "Y":
                        _destinationPosition.Y = move.Position.Millimeters;
                        break;

                    case "Z":
                        _destinationPosition.Z = move.Position.Millimeters;
                        break;

                    case "Rotation":
                        _currentRotation = move.Position;
                        _positions[move.AxisId] = move.Position;
                        ChangeAxisPosition(move.AxisId, move.Position.Millimeters);
                        break;

                    case "Filter":
                        _currentFilterPos = move.Position;
                        _positions[move.AxisId] = move.Position;
                        ChangeAxisPosition(move.AxisId, move.Position.Millimeters);
                        break;

                    default:
                        return;
                }
            }
            _cancellationTokenSources.Push(new CancellationTokenSource());
            var cancellationToken = _cancellationTokenSources.Peek().Token;
            _moveTask = Task.Run(() => PerformMoveAsync(moves, cancellationToken), cancellationToken);
        }

        private async Task PerformMoveAsync(PMAxisMove[] moves, CancellationToken cancellationToken)
        {
            try
            {
                _synchro.Reset();

                RaiseStateChangedEvent(new AxesState(allAxisEnabled: false, oneAxisIsMoving: true));
                while (!HasReachedDestination())
                {
                    // Check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _synchro.Set();
                        return;
                    }
                    foreach (var move in moves)
                    {
                        switch (move.AxisId)
                        {
                            case "Z":
                                _currentPosition.Z += CalculateStep();
                                ChangeAxisPosition("Z", _currentPosition.Z);
                                break;

                            default:
                                break;
                        }
                    }
                    // Check if the current position is equal to the destination with appropriate precision
                    if (HasReachedDestination())
                    {
                        Logger?.Information($"Destination reached. Z={_currentPosition.Z}");
                        break; // Exit the loop
                    }
                    //Enables movement to be staggered to allow potential cancel
                    await Task.Delay(10);
                }
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

                case "Rotation":
                    RaisePositionChangedEvent(new RotationPosition(new MotorReferential(), position));
                    break;

                default:
                    break;
            }
        }

        public override void RefreshAxisState(IAxis axis)
        {
            axis.Moving = false;
            axis.Enabled = true;
        }

        public override void RefreshCurrentPos(List<IAxis> axes)
        {
            foreach (var axis in axes)
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

                    case "Rotation":
                        axis.CurrentPos = _currentRotation;
                        break;

                    case "Filter":
                        axis.CurrentPos = _currentFilterPos;
                        break;

                    default:
                        break;
                }
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                if (_positions.ContainsKey(move.AxisId))
                {
                    _positions[move.AxisId] += move.Position;
                }
                else
                {
                    _positions.Add(move.AxisId, move.Position);
                }
            }
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void StopAllMotion()
        {
            try
            {
                CancelAll();
                _moveTask?.Wait(); // Wait for the task to complete
                RaiseStateChangedEvent(new AxesState(allAxisEnabled: true, oneAxisIsMoving: false));
            }
            catch (Exception ex)
            {
                Logger?.Error($"StopAllMotion - CNCException: {ex.Message} - {ex.StackTrace}");
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

        private bool HasReachedDestination()
        {
            return Math.Abs(_currentPosition.Z - _destinationPosition.Z) <= IncrementalMovePrecision;
        }

        private double CalculateStep()
        {
            // Calculate the difference between the destination and current position
            double difference = _destinationPosition.Z - _currentPosition.Z;
            if (difference > IncrementalMoveStepSize)
            {
                return IncrementalMoveStepSize;
            }
            else if (difference < -IncrementalMoveStepSize)
            {
                return -IncrementalMoveStepSize;
            }
            else
            {
                return difference;
            }
        }

        public void CancelAll()
        {
            while (_cancellationTokenSources.Count > 0)
            {
                var cts = _cancellationTokenSources.Pop();
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
        }
    }
}
