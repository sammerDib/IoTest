using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Communication;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.CNC
{
    public class CNCMotionController : MotionControllerBase, IMotion
    {
        private readonly CNCMotionControllerConfig _cncControllerConfig;
        private readonly SerialPortCommunication _communication;
        private XYZPosition _currentPosition;
        private double _destinationZ;
        private double _currentRotation;
        private double _destinationRotation;
        private Stack<CancellationTokenSource> _cancellationTokenSources = new Stack<CancellationTokenSource>();
        private Task _moveTask;

        private CNCAxis _axisSelected { get; set; }

        private const double IncrementalMovePrecision = 0.01;
        private const double IncrementalMoveStepSize = 0.1;
        private const double IncrementalMoveRotationStepSize = 10;
        

        public CNCMotionController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(controllerConfig, globalStatusServer, logger)
        {
            ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            ClassLocator.Default.GetInstance<ILogger>();
            _currentPosition = new XYZPosition(new MotorReferential(), 0.0, 0.0, 0.0);
            _destinationZ = 0.0;
            _currentRotation = 0.0;
            _destinationRotation = 0.0;

            if (controllerConfig is CNCMotionControllerConfig)
            {
                _cncControllerConfig = (CNCMotionControllerConfig)controllerConfig;
            }
            else
                throw (new Exception(FormatMessage("Bad controller configuration type. Controller creation failed !")));

            _communication = new SerialPortCommunication(_cncControllerConfig.SerialCom.Port, _cncControllerConfig.SerialCom.BaudRate)
            {
                ConsistentResponsePattern = new Regex(@".*\r\n$"),
                IgnoredResponsePattern = new Regex("error : C?CW Limit!!"),
            };
        }

        public override void Init(List<Message> initErrors)
        {
            Connect();
        }

        public override bool ResetController()
        {
            throw new NotImplementedException();
        }

        public override void Connect()
        {
            _communication.Connect();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
            _communication.Disconnect();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override void CheckControllerIsConnected()
        {
        }

        public override void StopAllMotion()
        {
            try
            {
                Logger?.Information($"StopAllMotion");

                CancelAll();
                _moveTask?.Wait();
            }
            catch (Exception ex)
            {
                Logger?.Error($"StopAllMotion - CNCException : {ex.Message} - {ex.StackTrace}");
            }
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            try
            {
                // Wait until the controller stops moving
                bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() =>
                {
                    Thread.Sleep(100);

                    var motorStatus = IsItMoving();
                    bool isStopped = motorStatus.Contains("Idle");
                    if (!isStopped)
                    {
                        var pos = GetCurrentPosition(_axisSelected);
                        Logger.Debug($"Motor {_axisSelected.MovingDirection} is moving. Current position = {pos.Value}");
                        ChangeAxisPosition(_axisSelected.AxisID, pos.Value);
                    }
                    return isStopped;
                }, timeout_ms);

                if (!hasStoppedWithinTimeout)
                {
                    throw new TimeoutException($"CNC controller WaitMotionEnd exits on timeout ({timeout_ms} ms).");
                }
            }
            finally
            {
                if (_axisSelected != null)
                {
                    var pos = GetCurrentPosition(_axisSelected);
                    Logger.Debug($"Motor {_axisSelected.MovingDirection} is moving. Current position = {pos.Value}");
                    ChangeAxisPosition(_axisSelected.AxisID, pos.Value);
                }
            }
        }

        public string IsItMoving()
        {
            return SendCommand(@"?", @"([^<]+)");
        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            throw new NotImplementedException();
        }

        public override void HomeAllAxes()
        {
        }

        public override PositionBase GetPosition()
        {
            throw new NotImplementedException();
        }

        public Length GetCurrentPosition(CNCAxis axis)
        {
            string response = "";
            if (axis.AxisID == "Z")
            {
                response = SendCommand(@"?", @"WPos:-?\d+\.\d+,\d+\.\d+,(-?\d+\.\d+)");
            }
            else if (axis.AxisID == "Rotation")
            {
                response = SendCommand(@"?", @"WPos:(-?\d+\.\d+)");
            }

            axis.CurrentPos = Convert.ToDouble(response).Millimeters();
            return axis.CurrentPos;
        }

        private void ChangeAxisPosition(string axisId, double position)
        {
            switch (axisId)
            {
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
            var motorStatus = IsItMoving();
            bool isStopped = motorStatus.Contains("Idle");
            axis.Moving = !isStopped;
            axis.Enabled = true; // TODO: fetch value from hardware
        }

        public override void RefreshCurrentPos(List<IAxis> axes)
        {
            foreach (var axis in axes)
            {
                axis.CurrentPos = GetCurrentPosition((CNCAxis)axis);
            }
        }

        private void AbsoluteMoveAxis(string axisId, double position)
        {
            string msg = $"G0 {axisId} {position}\r\n";
            _communication.Command(new SerialPortCommand<string>() { Message = msg });
        }



        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                switch (move.AxisId)
                {
                    case "Z":
                        _axisSelected = (CNCAxis)GetAxis(move.AxisId);
                        _destinationZ = move.Position.Millimeters;
                        break;
                    case "Rotation":                        
                        _axisSelected = (CNCAxis)GetAxis(move.AxisId);
                        _destinationRotation = move.Position.Millimeters;                        
                        break;
                }
            }
            _cancellationTokenSources.Push(new CancellationTokenSource());
            CancellationToken cancellationToken = _cancellationTokenSources.Peek().Token;
            _moveTask = Task.Run(() => PerformIncrementalMoveAsync(moves, cancellationToken), cancellationToken);
        }


        private async Task PerformIncrementalMoveAsync(PMAxisMove[] moves, CancellationToken cancellationToken)
        {
            try
            {
                while (!HasReachedDestination())
                {
                    // Check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    foreach (var move in moves)
                    {
                        switch (move.AxisId)
                        {
                            case "Z":
                                _currentPosition.Z += CalculatePositionStep(_destinationZ, _currentPosition.Z, IncrementalMoveStepSize);
                                AbsoluteMoveAxis("Z", _currentPosition.Z);
                                break;
                            case "Rotation":                                
                                _currentRotation += CalculatePositionStep(_destinationRotation, _currentRotation, IncrementalMoveRotationStepSize);
                                string axisId = "X";
                                AbsoluteMoveAxis(axisId, _currentRotation);                                
                                break;
                            default:
                                break;
                        }
                    }
                    // Check if the current position is equal to the destination with appropriate precision
                    if (HasReachedDestination())
                    {
                        Logger?.Information($"Destination reached. Z={_currentPosition.Z}");
                        Logger?.Information($"Destination reached. Rotation={_currentRotation}");
                        break; // Exit the loop
                    }
                    //Enables movement to be staggered to allow potential cancel
                    await Task.Delay(50, cancellationToken);

                }
            }
            catch (TaskCanceledException)
            {
                Logger?.Information("Task canceled. Exiting PerformMove.");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Exception in PerformMove: {ex}");
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
        }

        private string SendCommand(string msg, string pattern)
        {
            string response = _communication.Query(new SerialPortQuery<string>()
            {
                Message = msg,
                ResponsePattern = new Regex(pattern)
            });

            return response;
        }

        private string FormatMessage(string message)
        {
            return ($"[{DeviceID}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }
        private bool HasReachedDestination()
        {          
            return Math.Abs(_currentPosition.Z - _destinationZ) <= IncrementalMovePrecision
             && Math.Abs(_currentRotation - _destinationRotation) <= IncrementalMovePrecision;
        }

        private double CalculatePositionStep(double destination, double currentPosition, double step)
        {
            // Calculate the difference between the destination and current position                    
            double difference = destination - currentPosition;
            return (difference > IncrementalMoveStepSize) ? step : (difference < -step) ? -step : (difference); ;
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
