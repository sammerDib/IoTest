using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Communication;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance
{
    // TODO: Check axes config are valid (i.e. accelerations are set (not 0)...)
    // TODO: Reliance hardware controller accepts speeds in a fixed range. Let's check given speeds are within this range.
    // TODO: GetAxis() seems often called in method which take MovingDirection parameter. Why not
    // providing Axis as parameter, since public methods accept Axis as parameter, not MovingDirection?
    // TODO: make this controller handle as many axis as necessary
    /// <summary>
    /// Warning: This controller can only handle 2 axes: X and Y
    /// </summary>

    public class RCMController : AxesControllerBase, IDisposable
    {
        // TODO: axisMove this in config?
        private const int DefaultWaitMotionEnd_ms = 300_000;

        public static readonly Length DistancePerMotorRotation = 4.Millimeters(); // FIXME: is it the correct value?
        private int _motorResolution { get; set; }
        private int _speedFactor { get; set; }
        private Length DistanceThreshold => DistancePerMotorRotation / _motorResolution;

        /// <summary>
        /// Factor to be applied to the motor revolution speed.
        /// </summary>
        private const SpeedUnit DefaultSpeedUnit = SpeedUnit.Hundred;

        /// <summary>
        /// Hardware controller cannot handle speed lower than this value (in pulse per second).
        /// </summary>
        private const int MinimalSpeedPps = 1;

        /// <summary>
        /// Hardware controller cannot handle acceleration lower than this value (in pulse per
        /// second squared).
        /// </summary>
        private const int MinimalAccelerationPpss = 1;

        /// <summary>
        /// Multiplicative factor of acceleration used by hardware controller.
        /// </summary>
        private const int AccelerationUnit = 1_000;

        private readonly HashSet<string> _expectedAxesIds = new HashSet<string> { "X", "Y" };
        private List<RelianceAxis> RelianceAxes => AxesList.Cast<RelianceAxis>().ToList();

        /// <summary>
        /// Task which do polling to notify when position or status changed. Polling delay is
        /// defined by <see cref="_config#PollingDelay"/>.
        /// </summary>
        private Task _pollingTask;

        private CancellationTokenSource _pollingCancellationToken;

        /// <summary>
        /// True if at least one axis is moving.
        /// </summary>
        private bool _isMoving;

        private readonly RCMControllerConfig _config;
        private readonly SerialPortCommunication _communication;

        public RCMController(RCMControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger, ISerialPort serialPort = null) 
            : base(config,globalStatusServer,logger)
        {
            _config = config;
            _communication = new SerialPortCommunication(_config.PortName, _config.BaudRate, serialPort)
            {
                ConsistentResponsePattern = new Regex(@".*\r\n$"),
                IgnoredResponsePattern = new Regex("error : C?CW Limit!!"),
            };
        }

        private RelianceAxis GetAxis(MovingDirection movingDirection)
        {
            return RelianceAxes.Find(axis => axis.MovingDirection == movingDirection) ??
                   throw new Exception($"Axis with direction {movingDirection} not found.");
        }

        #region LifeCycle

        public override void Init(List<Message> initErrors = null)
        {
            Connect();
        }

        public override void InitializationAllAxes(List<Message> initErrors = null)
        {
            ValidateAxesList();
            ResetController();
            SetMotorResolutionAndSpeedUnit(_config.MotorResolution, DefaultSpeedUnit);
            // TODO: make sure all parameters have correct value (even if default values)
            RefreshCurrentPos(AxesList);
            foreach (var axis in RelianceAxes)
            {
                axis.LastNotifiedPosition = axis.CurrentPos;
                RefreshAxisState(axis);
            }

            _pollingCancellationToken = new CancellationTokenSource();
            _pollingTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        _pollingCancellationToken.Token.ThrowIfCancellationRequested();
                        NotifyIfNecessary();
                        await Task.Delay(_config.PollingDelay, _pollingCancellationToken.Token);
                    }
                },
                _pollingCancellationToken.Token
            );

            RaiseStateChangedEvent(new AxesState(AreAllAxesEnabled(), false));
        }

        private void ValidateAxesList()
        {
            var actualAxesIds = AxesList.Select(axis => axis.AxisID).ToList();

            if (actualAxesIds.Count != _expectedAxesIds.Count)
            {
                throw new Exception(
                    $"RCM controller handles only a set of {_expectedAxesIds.Count} axes: {_expectedAxesIds}. {actualAxesIds.Count} axes configured"
                );
            }

            foreach (string expectedAxisId in _expectedAxesIds.Where(expectedAxisId => !actualAxesIds.Contains(expectedAxisId)))
            {
                throw new Exception(
                    $"Axis {expectedAxisId} is not configured in RCM controller, which is mandatory. Configured axes are {actualAxesIds}"
                );
            }

            foreach (var unsupportedAxis in AxesList.Where(axis => !(axis is RelianceAxis)))
            {
                throw new Exception($"Unsupported axis of type {unsupportedAxis.GetType()}. RCM controller handles only axes of type RelianceAxis.");
            }
        }

        // TODO: Make it async
        /// <summary>
        /// Set the motor's position 0 at middle range of each axis.
        /// </summary>
        public void Calibrate()
        {
            var calibrationTasks = new List<Task>();
            foreach (var axis in RelianceAxes)
            {
                calibrationTasks.Add(CalibrateAsync(axis));
            }

            Task.WaitAll(calibrationTasks.ToArray());
        }

        // FIXME: update documentation
        /// <summary> Calibration steps are:
        /// 1. Perform an origin search method (command '|.<mororID>')
        /// => we are in position 0
        /// 2. Move quickly to position 300mm. 2 hypotheses are made:
        /// - motion range of axis is > 300mm
        /// - maximum motion range is far enough from 300mm not to hit the hardware bound
        /// 3. Move slowly to position 400mm > this will trigger an error (CCW Limit!) but motor
        /// status (command '?99.<mororID>') should be OK (Ux.<mororID>=8). Hypothesis:
        /// - maximum motion range of axis is < 400mm
        /// 4. Record the current position as the upper bound (hardware limit)
        /// 5. Compute the range center and axisMove to this position
        /// 6. Set the current position as the origin with command '|2.<mororID>' </summary>
        private async Task CalibrateAsync(RelianceAxis axis)
        {
            // TODO: Set K42-48 parameters
            var movingDirection = axis.MovingDirection;
            Logger.Debug($"[{movingDirection}] Calibration starting");

            await PerformSearchOriginAsync(axis);
            Logger.Debug($"[{movingDirection}] Search origin finished");

            // Move to center
            await MoveToAsync(new RCMAxisMove(axis, 0.Millimeters()));
            Logger.Debug($"[{movingDirection}] Moved to center");
        }

        public override bool ResetController()
        {
            foreach (var axis in RelianceAxes)
            {
                // TODO: find another way to get speed and acceleration in pulses per second, than using RCMAxisMove
                // which is dedicated to a move
                var axisMove = new RCMAxisMove(axis, 0.Millimeters(), AxisSpeed.Slow);
                int normalSpeedAsPulsesPerSecond = axisMove.GetSpeedAsPulsesPerSecond(
                    _motorResolution,
                    DistancePerMotorRotation
                );
                int normalAccelerationAsPulsesPerSecond =
                    axisMove.GetAccelerationAsPulsesPerSecondSquared(_motorResolution, DistancePerMotorRotation);
                foreach (string cmd in new List<string>
                         {
                             $"K42.{axis.Config.RelianceAxisID}={normalSpeedAsPulsesPerSecond}", // Search origin speed
                             $"K43.{axis.Config.RelianceAxisID}={normalAccelerationAsPulsesPerSecond}", // Search origin acceleration
                             $"K45.{axis.Config.RelianceAxisID}={axis.Config.MotorDirection}", // Search origin direction
                             $"K46.{axis.Config.RelianceAxisID}=2", // Search origin method
                             $"K47.{axis.Config.RelianceAxisID}=100", // Search origin stopper voltage
                             $"K27.{axis.Config.RelianceAxisID}=6900", // Input Function at Quick Response Logical High (6: CW Limit/Origin Switch 9:  CCW Limit/Origin Switch)
                             $"K21.{axis.Config.RelianceAxisID}=0", // Semi / Full Closed Loop Operation (full closed loop)
                             $"K55.{axis.Config.RelianceAxisID}=5", // In position tolerance
                             $"K56.{axis.Config.RelianceAxisID}=3000", // Position Error Overflow Alarm
                             $"K37.{axis.Config.RelianceAxisID}=10", // Resolution and Speed Unit
                             $"K51.{axis.Config.RelianceAxisID}=500", // Creeping speed (warning: not in documentation)
                             $"K60.{axis.Config.RelianceAxisID}=80", // Pushmode Current Limit
                             $"K23.{axis.Config.RelianceAxisID}=8", // Event Status (disable Echo)
                             "(" // Enable
                         })
                {
                    _communication.Command(new SerialPortCommand<string> { Message = $"{cmd}\r\n" });
                }

                var status = GetMotorStatus(axis);
                if (status != MotorStatus.InPosition)
                {
                    Logger.Warning(
                        $"[{axis.MovingDirection}] Motor is not ready to receive commands after reset: status is {status} ({MotorStatus.InPosition} expected)"
                    );
                }
            }

            return true;
        }

        public void Dispose()
        {
            Disconnect();
            _communication.Dispose();
        }

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            bool axisTypeNotSupported = axesList.Any(axis => !(axis is RelianceAxis));
            if (axisTypeNotSupported) throw new Exception("RCM controller handles only axes of type RelianceAxis.");

            return true;
        }

        #endregion LifeCycle

        #region Connexion

        public override void Connect()
        {
            _communication.Connect();
        }

        public override void Connect(string _)
        {
            Connect();
        }

        public override void Disconnect()
        {
            _pollingCancellationToken?.Cancel();
            try
            {
                _pollingTask?.Wait();
            }
            catch (AggregateException e)
            {
                if (!(e.InnerException is TaskCanceledException))
                {
                    throw;
                }
            }
            _communication.Disconnect();
        }

        public override void Disconnect(string _)
        {
            Disconnect();
        }

        public override void EnableAxis(List<IAxis> axisList)
        {
            foreach (var axis in axisList)
            {
                var relianceAxis = (RelianceAxis)axis;
                string relianceAxisId = relianceAxis.Config.RelianceAxisID;
                _communication.Command(new SerialPortCommand<string>() { Message = $"(.{relianceAxisId}\r\n^\r\n", });
            }
        }

        #endregion Connexion

        #region Internal properties

        private void SetMotorResolutionAndSpeedUnit(int motorResolution, SpeedUnit speedUnit)
        {
            if (!MotorResolution.ValidValues.Contains(motorResolution))
                throw new Exception(
                    "MotorResolution value not supported. Please refer to the documentation to see available values."
                );

            // pulses pour 1 revolution
            _motorResolution = motorResolution; // FIXME: value should not be = to 0
            _speedFactor = (int)speedUnit;

            int motorResolutionParameterValue = MotorResolution.GetParameterValue(_motorResolution, speedUnit);
            foreach (var axis in RelianceAxes)
            {
                string relianceAxisID = axis.Config.RelianceAxisID;
                _communication.Command(new SerialPortCommand<string>()
                {
                    Message = $"K37.{relianceAxisID}={motorResolutionParameterValue}\r\n"
                }
                );
            }
        }

        #endregion Internal properties

        #region Motion

        /// <returns>Current position of a given direction, in motor referential.</returns>
        public Length GetCurrentPosition(RelianceAxis axis)
        {
            string relianceAxisID = axis.Config.RelianceAxisID;

            // returned position is expressed in 'pulses'.
            string response = _communication.Query(new SerialPortQuery<string>()
            {
                Message = $"?96.{relianceAxisID}\r\n",
                ResponsePattern = new Regex($@"Px\.{relianceAxisID}=(-?\d+)")
            });

            return ConvertPulsesToMm(int.Parse(response), axis).Millimeters();
        }

        public override void StopAxesMotion()
        {
            _communication.Command(new SerialPortCommand<string> { Message = "*\r\n*1\r\n" });
            ResetController();
        }

        public override void RefreshCurrentPos(List<IAxis> axes)
        {
            foreach (var axis in axes)
            {
                axis.CurrentPos = GetCurrentPosition((RelianceAxis)axis);
            }
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            var position = GetCurrentPosition((RelianceAxis)axis).Millimeters;
            var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
            return new TimestampedPosition(position.Millimeters(), highResolutionDateTime);
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double step)
        {
            var relianceAxis = (RelianceAxis)axis;
            var currentPosition = GetCurrentPosition(relianceAxis);
            var requestedPosition = currentPosition + step.Millimeters();
            var move = new RCMAxisMove(relianceAxis, requestedPosition, speed);
            //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
            _ = MoveToAsync(move);
        }

        private async Task MoveToAsync(RCMXYMove move)
        {
            // TODO: regroups these commands in single command to hardware controller
            SendCommand(move.X);
            SendCommand(move.Y);

            await WaitMotionEndAsync();

            RaiseMotionEndEvent(true); // TODO: send false when target position not reached
        }

        public async Task MoveToAsync(RCMAxisMove axisMove, TimeSpan timeout = default)
        {
            SendCommand(axisMove);
            await WaitMotionEndAsync(axisMove.Axis);
            RaiseMotionEndEvent(true); // TODO: send false when target position not reached
        }

        private void SendCommand(RCMAxisMove move)
        {
            if (move.Distance < DistanceThreshold)
            {
                Logger.Information($"{move.Axis.MovingDirection} movement skipped: distance ({move.Distance}) under threshold ({DistanceThreshold})");
                return;
            }

            // FIXME What if currently moving?
            int position_pulses = move.GetDestinationAsPulses(_motorResolution, DistancePerMotorRotation);

            double speed_pps = move.GetSpeedAsPulsesPerSecond(_motorResolution, DistancePerMotorRotation);
            double rotationalSpeed = Math.Max(Math.Floor(speed_pps / _speedFactor), MinimalSpeedPps);

            double acceleration_ppss = move.GetAccelerationAsPulsesPerSecondSquared(_motorResolution, DistancePerMotorRotation);
            double rotationalAcceleration = Math.Max(Math.Floor(acceleration_ppss / AccelerationUnit), MinimalAccelerationPpss);

            string relianceAxisID = move.RelianceAxisId;

            _communication.Command(new SerialPortCommand<string>()
            {
                Message =
                        $"P.{relianceAxisID}={position_pulses}\r\nS.{relianceAxisID}={rotationalSpeed}\r\nA.{relianceAxisID}={rotationalAcceleration}\r\n^.{relianceAxisID}\r\n",
            }
            );
        }

        public override void SetPosAxisWithSpeedAndAccel(
            List<double> coordsList,
            List<IAxis> axisList,
            List<double> speedsList,
            List<double> accelsList
        )
        {
            var axes = new AxesSet(axisList);
            if (axes.Empty)
            {
                return;
            }

            if (!axes.ContainsAnyOf(MovingDirection.X, MovingDirection.Y))
            {
                return;
            }

            if (axes.ContainsAll(MovingDirection.X, MovingDirection.Y))
            {
                var xyMove = new RCMXYMove(coordsList, axisList, speedsList, accelsList);
                //"_ =" to disable warning CS4014
                _ = MoveToAsync(xyMove);
            }
            else if (axes.Contains(MovingDirection.X))
            {
                int xIndex = axes.IndexOf(MovingDirection.X);
                //"_ =" to disable warning CS4014
                _ = MoveToAsync(new RCMAxisMove(axisList[xIndex] as RelianceAxis,
                        coordsList[xIndex].Millimeters(),
                        speedsList[xIndex].MillimetersPerSecond(),
                        accelsList[xIndex].MillimetersPerSecondSquared()
                    )
                );
            }
            else if (axes.Contains(MovingDirection.Y))
            {
                int yIndex = axes.IndexOf(MovingDirection.Y);
                //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
                _ = MoveToAsync(new RCMAxisMove(axisList[yIndex] as RelianceAxis,
                        coordsList[yIndex].Millimeters(),
                        speedsList[yIndex].MillimetersPerSecond(),
                        accelsList[yIndex].MillimetersPerSecondSquared()
                    )
                );
            }
        }

        public override void SetPosAxis(List<double> coordsList, List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            var axes = new AxesSet(axisList);
            if (axes.Empty)
            {
                return;
            }

            if (!axes.ContainsAnyOf(MovingDirection.X, MovingDirection.Y))
            {
                return;
            }

            if (axes.ContainsAll(MovingDirection.X, MovingDirection.Y))
            {
                var xyMove = new RCMXYMove(coordsList, axisList, speedsList);
                // TODO: regroups these commands in single command to hardware controller
                //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
                _ = MoveToAsync(xyMove);
            }
            else if (axes.Contains(MovingDirection.X))
            {
                int xIndex = axes.IndexOf(MovingDirection.X);
                //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
                _ = MoveToAsync(new RCMAxisMove(axisList[xIndex] as RelianceAxis,
                        coordsList[xIndex].Millimeters(),
                        speedsList[xIndex]
                    )
                );
            }
            else if (axes.Contains(MovingDirection.Y))
            {
                int yIndex = axes.IndexOf(MovingDirection.Y);
                //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
                _ = MoveToAsync(new RCMAxisMove(axisList[yIndex] as RelianceAxis,
                        coordsList[yIndex].Millimeters(),
                        speedsList[yIndex]
                    )
                );
            }
        }

        // FIXME: extract and set parameters values (K42, K43, K45, K46, K47, K48)
        public async Task PerformSearchOriginAsync(RelianceAxis axis, TimeSpan timeout = default)
        {
            string relianceAxisID = axis.Config.RelianceAxisID;

            _communication.Command(new SerialPortCommand<string>()
            {
                Message = $"K42.{relianceAxisID}=800\r\n", // search origin speed
            }
            );
            _communication.Command(new SerialPortCommand<string>()
            {
                Message =
                        $"K45.{relianceAxisID}=0\r\n", // search origin direction (0 = clockwise, 1 = counter clockwise, 2 = clockwise + reverse coordinate, 3 = counter clockwise + reverse coordinate)
            }
            );
            _communication.Command(new SerialPortCommand<string>() { Message = $"|.{relianceAxisID}\r\n", });

            int timeout_ms = (int)(timeout == default
                ? TimeSpan.FromMinutes(2).TotalMilliseconds
                : timeout.TotalMilliseconds);
            await WaitMotionEndAsync(axis, timeout_ms);
        }

        private double ConvertPulsesToMm(int distance_pulses, RelianceAxis axis)
        {
            return ((((double)distance_pulses / (double)_motorResolution) * (double)DistancePerMotorRotation.Millimeters) * axis.Config.MotorDirection) + axis.Config.PositionZero.Millimeters;
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            throw new NotImplementedException();
        }

        public override void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            throw new NotImplementedException();
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            var tasks = new List<Task>();
            foreach (var axis in RelianceAxes)
            {
                tasks.Add(WaitMotionEndAsync(axis, timeout_ms));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public async Task WaitMotionEndAsync()
        {
            var tasks = new List<Task>();
            foreach (var axis in RelianceAxes)
            {
                tasks.Add(WaitMotionEndAsync(axis));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public async Task WaitMotionEndAsync(RelianceAxis axis, int timeout_ms = DefaultWaitMotionEnd_ms)
        {
            // TODO: make this task cancellable
            await Task.Run(() =>
                {
                    // Wait until the motor is 'in position'
                    bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() =>
                        {
                            var motorStatus = GetMotorStatus(axis);
                            bool isStopped = motorStatus == MotorStatus.InPosition;
                            if (!isStopped)
                            {
                                Logger.Debug(
                                    $"Motor {axis.MovingDirection} is moving. Current position = {GetCurrentPosition(axis)}, status = {motorStatus}"
                                );
                            }
                            Thread.Sleep(_config.PollingDelay);
                            return isStopped;
                        },
                        timeout_ms
                    );

                    if (!hasStoppedWithinTimeout)
                    {
                        throw new TimeoutException(
                            $"WaitMotionEnd on motor {axis.MovingDirection} exits on timeout ({timeout_ms} ms)."
                        );
                    }
                }
            );
        }

        public MotorStatus GetMotorStatus(RelianceAxis axis)
        {
            string relianceAxisID = axis.Config.RelianceAxisID;

            string status = _communication.Query(new SerialPortQuery<string>()
            {
                Message = $"?99.{relianceAxisID}\r\n",
                ResponsePattern = new Regex($@"Ux\.{relianceAxisID}=(\d+)"),
            }
            );

            return ResponseAsEnum<MotorStatus>(int.Parse(status));
        }

        private void NotifyIfNecessary()
        {
            var xAxis = GetAxis(MovingDirection.X);
            var yAxis = GetAxis(MovingDirection.Y);

            var xLastNotifiedPosition = xAxis.LastNotifiedPosition;
            var yLastNotifiedPosition = yAxis.LastNotifiedPosition;

            RefreshCurrentPos(AxesList);

            var xCurrentPosition = xAxis.CurrentPos;
            var yCurrentPosition = yAxis.CurrentPos;

            bool xPositionChanged = !xLastNotifiedPosition.Near(xCurrentPosition, xAxis.DistanceThresholdForNotification);
            bool yPositionChanged = !yLastNotifiedPosition.Near(yCurrentPosition, yAxis.DistanceThresholdForNotification);

            bool positionChanged = xPositionChanged || yPositionChanged;

            if (positionChanged)
            {
                var newPosition = new XYPosition(new MotorReferential(), xCurrentPosition.Millimeters, yCurrentPosition.Millimeters);
                RaisePositionChangedEvent(newPosition);

                xAxis.LastNotifiedPosition = xCurrentPosition;
                yAxis.LastNotifiedPosition = yCurrentPosition;
            }

            NotifyAxisState(positionChanged);
        }

        private void NotifyAxisState(bool anAxisIsMoving)
        {
            if (_isMoving != anAxisIsMoving)
            {
                RaiseStateChangedEvent(new AxesState(AreAllAxesEnabled(), anAxisIsMoving));
                _isMoving = anAxisIsMoving;
            }
        }

        private bool AreAllAxesEnabled()
        {
            // TODO: to be implemented
            return true;
        }

        public override void CheckControllerCommunication()
        {
            // TODO: to be implemented
            throw new NotImplementedException();
        }

        public override void DisableAxis(List<IAxis> axisList)
        {
            // TODO: set IAxis.Enabled, and prevent any further motion if false
            throw new NotImplementedException();
        }

        public override void RefreshAxisState(IAxis axis)
        {
            var relianceAxis = (RelianceAxis)axis;
            var motorStatus = GetMotorStatus(relianceAxis);
            axis.Moving = motorStatus == MotorStatus.Running;
            axis.Enabled = true; // TODO: fetch value from hardware
        }

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            // TODO: check speed is within range given by config
            throw new NotImplementedException();
        }

        #endregion Motion

        #region Utils

        // FIXME: those methods could be factorized in the ControllerBase abstract class ?

        private int[] valuesOf<T>() => (int[])Enum.GetValues(typeof(T));

        private bool isNotAValidValue(double value, params double[] allowedValues) => !allowedValues.Contains(value);

        private T ResponseAsEnum<T>(int responseValue)
        {
            if (isNotAValidValue(responseValue, valuesOf<T>().Select(Convert.ToDouble).ToArray()))
            {
                string errorMessage = "Error: incorrect or unknown response value.";
                throw new Exception(errorMessage);
            }

            return (T)Enum.ToObject(typeof(T), responseValue);
        }

        #endregion Utils

        #region Irrelevant methods

        public override bool IsLanded()
        {
            throw new NotImplementedException();
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coordsList)
        {
            throw new NotImplementedException();
        }

        public override void LinearMotionMultipleAxis(
            List<IAxis> axisList,
            AxisSpeed axisSpeed,
            List<double> coordsList
        )
        {
            throw new NotImplementedException();
        }

        public override void StopLanding()
        {
            throw new NotImplementedException();
        }

        public override void Land()
        {
            throw new NotImplementedException();
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
            throw new NotImplementedException();
        }

        public override void InitZTopFocus()
        {
            throw new NotImplementedException();
        }

        public override void InitZBottomFocus()
        {
            throw new NotImplementedException();
        }

        #endregion Irrelevant methods
    }
}
