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

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Phytron
{
    // TODO: make this controller handle as many axis as necessary
    /// <summary>
    /// Warning: this controller can only handle 1 axis: ZTop or ZBottom
    /// </summary>
    public class MCCController : AxesControllerBase, IDisposable

    {
        private readonly HashSet<MovingDirection> _allowedMovingDirections = new HashSet<MovingDirection>
        {
            MovingDirection.ZTop, MovingDirection.ZBottom
        };

        // FIXME: is it the correct value?
        private readonly Length _distancePerMotorRotation = 0.00315.Millimeters();

        // TODO: harmonize name with other controllers
        /// <summary>
        /// Minimum distance travelled between two pooling iterations (<see cref="_pollingTask"/>), from which we
        /// consider an axis is in motion.
        /// </summary>
        public static readonly Length InMotionDistanceThreshold = 0.01.Millimeters();

        private List<PhytronAxis> PhytronAxes => AxesList.Cast<PhytronAxis>().ToList();

        /// <summary>
        /// Task which do polling to notify when position or status changed.
        /// Polling delay is defined by <see cref="_config#PollingDelay"/>.
        /// </summary>
        private Task _pollingTask;

        private CancellationTokenSource _pollingCancellationToken;

        /// <summary>
        /// True if at least one axis is moving.
        /// </summary>
        private bool _isMoving;

        private readonly MCCControllerConfig _config;
        private readonly SerialPortCommunication _communication;
        private readonly MCCRequestBuilder _requestBuilder;

        private readonly int _motorResolution;

        public MCCController(MCCControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(config, globalStatusServer, logger)
        {
            _config = config;
            _motorResolution = 1;
            _communication = new SerialPortCommunication(_config.PortName, _config.BaudRate)
            {
                ConsistentResponsePattern = new Regex($"^{NPC.STX}.*{NPC.ETX}$"),
                ErrorResponsePattern = new Regex($"^{NPC.STX}{NPC.NAK}{NPC.ETX}\r\n$"),
            };
            _requestBuilder = new MCCRequestBuilder(config.Address);
        }

        private PhytronAxis GetAxis(MovingDirection movingDirection)
        {
            return AxesList.Find(axis => axis.AxisConfiguration.MovingDirection == movingDirection) as PhytronAxis ?? throw new Exception($"Axis with direction {movingDirection} not found.");
        }

        #region LifeCycle

        public override void Init(List<Message> initErrors = null)
        {
            Connect();
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            ValidateAxesList();
            // TODO: make sure all parameters have correct value (even if default values)
            RefreshCurrentPos(AxesList);
            foreach (var axis in PhytronAxes)
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

            RaiseStateChangedEvent(new AxesState(true, false));
        }

        private void ValidateAxesList()
        {
            var actualMovingDirections = AxesList.Select(axis => axis.AxisConfiguration.MovingDirection).ToList();

            if (actualMovingDirections.Count != 1)
            {
                throw new Exception(
                    $"MCC controller can only handle 1 axis. {actualMovingDirections.Count} axes configured"
                );
            }

            var movingDirection = actualMovingDirections.First();
            if (!_allowedMovingDirections.Contains(movingDirection))
            {
                throw new Exception(
                    $"MCC controller does not support {movingDirection} axis. Only {_allowedMovingDirections} axes are supported"
                );
            }
        }

        // TODO: Make it async
        /// <summary>
        /// Set the motor's position 0 at middle range of each axis.
        /// </summary>
        public void Calibrate()
        {
            var calibrationTasks = new List<Task>();
            foreach (var axis in PhytronAxes)
            {
                calibrationTasks.Add(CalibrateAsync(axis));
            }

            Task.WaitAll(calibrationTasks.ToArray());
        }

        // TODO: add documentation
        private async Task CalibrateAsync(PhytronAxis axis)
        {
            // TODO Set parameter for search origin
            // TODO Search origin
            var (speed, acceleration) = GetSpeedAndAcceleration(axis, AxisSpeed.Normal);
            await MoveToAsync(axis, 0.Millimeters(), speed, acceleration);
        }

        public override bool ResetController()
        {
            Disconnect();
            return true;
        }

        public void Dispose()
        {
            _communication.Dispose();
        }

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            bool axisTypeNotSupported = axesList.Any(axis => !(axis is PhytronAxis));
            if (axisTypeNotSupported) throw new Exception("MCC controller handles only axes of type PhytronAxis.");

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

        #endregion Connexion

        #region Motion

        public OneAxisPosition GetCurrentPosition()
        {
            RefreshCurrentPos(AxesList);
            return BuildPosition(PhytronAxes.First().CurrentPos);
        }

        private OneAxisPosition BuildPosition(Length position)
        {
            var axis = PhytronAxes.First();
            switch (axis.MovingDirection)
            {
                case MovingDirection.ZTop:
                    return new ZTopPosition(new MotorReferential(), position.Millimeters);

                case MovingDirection.ZBottom:
                    return new ZBottomPosition(new MotorReferential(), position.Millimeters);

                default:
                    throw new Exception(
                        $"Moving direction {axis.MovingDirection} not supported. Only {MovingDirection.ZTop} and {MovingDirection.ZBottom} are supported"
                    );
            }
        }

        /// <returns>Current position of a given direction, in motor referential.</returns>
        public Length GetCurrentPosition(PhytronAxis axis)
        {
            string response = _communication.Query(_requestBuilder.GetCurrentPosition(axis));
            double position_mm = ConvertStepsToMm(int.Parse(response)) + axis.Config.PositionZero.Millimeters;
            return position_mm.Millimeters();
        }

        public override void StopAxesMotion()
        {
            foreach (var axis in PhytronAxes)
            {
                _communication.Command(_requestBuilder.StopMotor(axis));
            }
        }

        public override void RefreshCurrentPos(List<IAxis> axes)
        {
            foreach (var axis in axes)
            {
                axis.CurrentPos = GetCurrentPosition((PhytronAxis)axis);
            }
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            var position = GetCurrentPosition((PhytronAxis)axis).Millimeters;
            var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
            return new TimestampedPosition(position.Millimeters(), highResolutionDateTime);
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speedEnum, double step)
        {
            Task.Run(async () =>
                {
                    var phytronAxis = (PhytronAxis)axis;
                    var currentPosition = GetCurrentPosition(phytronAxis);
                    (var speed, var acceleration) = GetSpeedAndAcceleration(phytronAxis, speedEnum);
                    await MoveToAsync(phytronAxis, currentPosition + step.Millimeters(), speed, acceleration);
                }
            );
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> coordsList, List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            for (int i = 0; i < coordsList.Count; i++)
            {
                var coordinate = coordsList[i].Millimeters();
                var axis = (PhytronAxis)axisList[i];
                var speed = speedsList[i].MillimetersPerSecond();
                var acceleration = accelsList[i].MillimetersPerSecondSquared();
                var _ = MoveToAsync(axis, coordinate, speed, acceleration); //Disabled warning CS4014. More information -> https://docs.microsoft.com/fr-fr/dotnet/csharp/language-reference/compiler-messages/cs4014
            }
        }

        public override void SetPosAxis(List<double> coordsList, List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            for (int i = 0; i < coordsList.Count; i++)
            {
                var coordinate = coordsList[i].Millimeters();
                var axis = (PhytronAxis)axisList[i];
                (var speed, var acceleration) = GetSpeedAndAcceleration(axis, speedsList[i]);
                MoveToAsync(axis, coordinate, speed, acceleration).ConfigureAwait(false);
            }
        }

        public async Task MoveToAsync(PhytronAxis axis, Length position, Speed speed, Acceleration acceleration)
        {
            // FIXME: notify caller when validPosition != position (since motion cannot be performed as requested)
            var validPosition = GetPositionWithinRange(axis, position);
            int position_steps = ConvertMmToSteps((validPosition - axis.Config.PositionZero).Millimeters);
            int speed_stepsPerSecond = ConvertMmToSteps(speed.MillimetersPerSecond);
            int acceleration_stepsPerSecondSquared = ConvertMmToSteps(acceleration.MillimetersPerSecondSquared);

            _communication.Command(_requestBuilder.SetSpeed(axis, speed_stepsPerSecond));
            _communication.Command(_requestBuilder.SetAcceleration(axis, acceleration_stepsPerSecondSquared));
            _communication.Command(_requestBuilder.SetPosition(axis, position_steps));

            await WaitMotionEndAsync(axis, 20_000); // FIXME: use custom timeout values from Config
            // TODO: move RaiseMotionEndEvent() in caller, as it should be called once after motion in ALL axes are done.
            RaiseMotionEndEvent(true); // TODO: send false when target position not reached
        }

        public (Speed, Acceleration) GetSpeedAndAcceleration(PhytronAxis axis, AxisSpeed speed)
        {
            switch (speed)
            {
                case AxisSpeed.Slow:
                    return (axis.Config.SpeedSlow.MillimetersPerSecond(), axis.Config.AccelSlow.MillimetersPerSecondSquared());

                case AxisSpeed.Normal:
                    return (axis.Config.SpeedNormal.MillimetersPerSecond(), axis.Config.AccelNormal.MillimetersPerSecondSquared());

                case AxisSpeed.Fast:
                    return (axis.Config.SpeedFast.MillimetersPerSecond(), axis.Config.AccelFast.MillimetersPerSecondSquared());

                default:
                    throw new Exception($"Unknown speed '{speed}'");
            }
        }

        /// <summary>
        /// Returns a position within the axis position range, bounded by [axis.PositionMin, axis.PositionMax].
        /// </summary>
        private Length GetPositionWithinRange(PhytronAxis axis, Length targetedPosition)
        {
            var newPosition = Length.Max(axis.PositionMin, Length.Min(axis.PositionMax, targetedPosition));
            if (newPosition != targetedPosition)
            {
                // TODO: replace this by an exception to facilitate problem solving by not hiding this
                Logger.Warning($"[{axis.MovingDirection}] Destination ({targetedPosition}) out of limits [{axis.PositionMin}, {axis.PositionMax}]: replaced by {newPosition}");
            }
            return newPosition;
        }

        private double ConvertStepsToMm(int steps)
        {
            return (((double)steps / (double)_motorResolution) * (double)_distancePerMotorRotation.Millimeters);
        }

        private int ConvertMmToSteps(double distance_mm)
        {
            return (int)Math.Floor(distance_mm * _motorResolution / _distancePerMotorRotation.Millimeters);
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            // TODO: to implement
            throw new NotImplementedException();
        }

        public override void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            // TODO: to implement
            throw new NotImplementedException();
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            var tasks = new List<Task>();
            foreach (var axis in PhytronAxes)
            {
                tasks.Add(WaitMotionEndAsync(axis, timeout_ms));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public async Task WaitMotionEndAsync(PhytronAxis axis, int timeout_ms)
        {
            // TODO: make this task cancellable
            await Task.Run(() =>
                {
                    // Wait until the motor is 'in position'
                    bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() =>
                        {
                            var motorStatus = GetMotorStatus();
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

        public MotorStatus GetMotorStatus()
        {
            string motorStatus = _communication.Query(_requestBuilder.GetMotorStatus());

            return Convert.MotorStatusFromString(motorStatus);
        }

        private void NotifyIfNecessary()
        {
            // TODO Iterate over all axes
            var axis = PhytronAxes.First();
            var lastNotifiedPosition = axis.LastNotifiedPosition;

            RefreshCurrentPos(AxesList);

            var currentPosition = axis.CurrentPos;

            bool positionChanged = !lastNotifiedPosition.Near(currentPosition, axis.DistanceThresholdForNotification);
            if (positionChanged)
            {
                Logger.Debug($"Motor {axis.AxisID} is moving. Current position = {currentPosition}");
                RaisePositionChangedEvent(BuildPosition(currentPosition));

                axis.LastNotifiedPosition = currentPosition;
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
            // TODO: to implement
            throw new NotImplementedException();
        }

        public override void EnableAxis(List<IAxis> axisList)
        {
            // TODO: to implement
            throw new NotImplementedException();
        }

        public override void DisableAxis(List<IAxis> axisList)
        {
            // TODO: to implement
            throw new NotImplementedException();
        }

        public override void RefreshAxisState(IAxis axis)
        {
            var motorStatus = GetMotorStatus();
            axis.Moving = motorStatus == MotorStatus.Moving;
            axis.Enabled = true; // TODO: fetch value from hardware
        }

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            // TODO: to implement
            throw new NotImplementedException();
        }

        #endregion Motion

        #region Irrelevant methods

        public override bool IsLanded()
        {
            throw new NotImplementedException();
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coordsList)
        {
            throw new NotImplementedException();
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axisList, AxisSpeed axisSpeed, List<double> coordsList)
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
