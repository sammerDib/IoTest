using System;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance
{
    /// <summary>
    /// This class represents a move along an axis.
    /// </summary>
    public class RCMAxisMove
    {
        private ILogger<RCMAxisMove> _logger;
        public readonly RelianceAxis Axis;
        public string RelianceAxisId => Axis.Config.RelianceAxisID;
        public Length Origin => Axis.CurrentPos;

        /// <summary>
        /// Coordinate in motor referential.
        /// </summary>
        public Length Destination { get; }

        public Length Distance => Math.Abs((Destination - Origin).Millimeters).Millimeters();
        public TimeSpan Duration => TimeSpan.FromSeconds(Distance.Millimeters / Speed.MillimetersPerSecond);

        public Speed Speed { get; set; }
        public Acceleration Acceleration { get; }

        public RCMAxisMove(RelianceAxis axis, Length destination, Speed speed, Acceleration acceleration)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<RCMAxisMove>>();
            Axis = axis;
            Destination = Length.Max(Axis.PositionMin, Length.Min(Axis.PositionMax, destination));
            if (Destination != destination)
            {
                // TODO: replace this by an exception to facilitate problem solving by not hiding this
                _logger.Warning($"[{Axis.MovingDirection}] Destination ({destination}) out of limits [{Axis.PositionMin}, {Axis.PositionMax}]: replaced by {Destination}");
            }
            Speed = speed;
            Acceleration = acceleration;
        }

        public RCMAxisMove(RelianceAxis axis, Length destination, AxisSpeed speed = AxisSpeed.Normal) : this(axis, destination, ConvertSpeed(axis, speed), GetAcceleration(axis, speed))
        {
        }

        private static Speed ConvertSpeed(RelianceAxis axis, AxisSpeed speed)
        {
            switch (speed)
            {
                case AxisSpeed.Slow:
                    return axis.Config.SpeedSlow.MillimetersPerSecond();

                case AxisSpeed.Normal:
                    return axis.Config.SpeedNormal.MillimetersPerSecond();

                case AxisSpeed.Fast:
                    return axis.Config.SpeedFast.MillimetersPerSecond();

                case AxisSpeed.Measure:
                    return axis.Config.SpeedMeasure.MillimetersPerSecond();

                default: throw new NotImplementedException($"Unknown speed {speed}");
            }
        }

        private static Acceleration GetAcceleration(RelianceAxis axis, AxisSpeed speed)
        {
            switch (speed)
            {
                case AxisSpeed.Slow:
                    return axis.Config.AccelSlow.MillimetersPerSecondSquared();

                case AxisSpeed.Normal:
                    return axis.Config.AccelNormal.MillimetersPerSecondSquared();

                case AxisSpeed.Fast:
                    return axis.Config.AccelFast.MillimetersPerSecondSquared();

                case AxisSpeed.Measure:
                    return axis.Config.AccelMeasure.MillimetersPerSecondSquared();

                default: throw new NotImplementedException($"Unknown speed {speed}");
            }
        }

        public int GetDestinationAsPulses(int motorResolution, Length distancePerMotorRotation)
        {
            return (int)Math.Floor(
                (Destination.Millimeters - Axis.Config.PositionZero.Millimeters) *
                Axis.Config.MotorDirection *
                motorResolution /
                distancePerMotorRotation.Millimeters
            );
        }

        public int GetSpeedAsPulsesPerSecond(int motorResolution, Length distancePerMotorRotation)
        {
            return (int)Math.Floor(Speed.MillimetersPerSecond * motorResolution / distancePerMotorRotation.Millimeters);
        }

        public int GetAccelerationAsPulsesPerSecondSquared(int motorResolution, Length distancePerMotorRotation)
        {
            return (int)Math.Floor(
                Acceleration.MillimetersPerSecondSquared *
                motorResolution /
                distancePerMotorRotation.Millimeters
            );
        }
    }
}
