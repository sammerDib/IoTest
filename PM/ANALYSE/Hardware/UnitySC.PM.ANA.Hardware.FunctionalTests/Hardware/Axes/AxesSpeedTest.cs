using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Extensions;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes
{
    // Warning: in case of error, make sure requested positions are within ranges defined in hardware configuration.
    public class AxesSpeedTest : FunctionalTest
    {
        /// <summary>
        /// Maximum delay of any motion, in milliseconds.
        /// </summary>
        private const int MaxMotionTimeoutMs = 30_000;

        /// <summary>
        /// Precision of motion delays (between real and expected one).
        /// Motions are not instantaneous, and there are acceleration and deceleration ramps.
        /// </summary>
        private readonly TimeSpan _motionDelayPrecision = 1.Seconds();

        private Acceleration DefaultXYAcceleration = 1000.MillimetersPerSecondSquared();
        private static readonly TimeSpan s_currentPositionPollingDelay = TimeSpan.FromMilliseconds(100);

        private IAxes _axes => ClassLocator.Default.GetInstance<AnaHardwareManager>().Axes;

        public override void Run()
        {
            var zTopConfig = _axes.Axes.Select(axis => axis.AxisConfiguration)
                .ToList()
                .Find(config => config.MovingDirection == MovingDirection.ZTop) as MotorizedAxisConfig;
            new List<Action>
            {
                TestSpeed_XY_WhenDiagonalMovement,
                TestSpeed_XY_WhenDiagonalMovement_withAsymmetricSpeed,
                TestSpeed_XY_WhenDiagonalMovement_withAsymetricRange,
                TestSpeed_XY_InStraightLine,
                () => TestSpeed_ZTop(zTopConfig.SpeedSlow.MillimetersPerSecond(), zTopConfig.AccelSlow.MillimetersPerSecondSquared()),
                () => TestSpeed_ZTop(zTopConfig.SpeedNormal.MillimetersPerSecond(), zTopConfig.AccelNormal.MillimetersPerSecondSquared()),
                () => TestSpeed_ZTop(zTopConfig.SpeedFast.MillimetersPerSecond(), zTopConfig.AccelFast.MillimetersPerSecondSquared())
            }.ForEach(test =>
                {
                    test.Invoke();
                    Console.WriteLine($"{test.Method.Name} is successful");
                }
            );

            Console.WriteLine("Tests finished successfully!");
        }

        // Diagonal Movement, X and Y with same range and speed
        private void TestSpeed_XY_WhenDiagonalMovement()
        {
            // Given
            _axes.GotoPosition(new XYPosition(new StageReferential(), -150, -150), AxisSpeed.Normal);
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            const int eachAxisRangeInMillimeters = 300;
            var speed = 25.MillimetersPerSecond();

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _axes.GotoPointCustomSpeedAccel(
                new AxisMove(150, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                new AxisMove(150, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                null,
                null
            );
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            stopwatch.Stop();

            // Then
            var expectedDuration = TimeSpan.FromSeconds(eachAxisRangeInMillimeters / speed.MillimetersPerSecond);
            stopwatch.Elapsed.Should().BeCloseTo(expectedDuration, _motionDelayPrecision);
        }

        // Diagonal Movement, X and Y with same range, but asymmetric speed
        private void TestSpeed_XY_WhenDiagonalMovement_withAsymmetricSpeed()
        {
            // Given
            _axes.GotoPosition(new XYPosition(new StageReferential(), 150, 150), AxisSpeed.Normal);
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);

            const int eachAxisRangeInMillimeters = 300;
            var xSpeed = 25.MillimetersPerSecond();
            var ySpeed = 50.MillimetersPerSecond();

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _axes.GotoPointCustomSpeedAccel(
                new AxisMove(-150, xSpeed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                new AxisMove(-150, ySpeed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                null,
                null
            );
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            stopwatch.Stop();

            // Then
            var expectedDuration = TimeSpan.FromSeconds(
                eachAxisRangeInMillimeters /
                Math.Min(
                    xSpeed.MillimetersPerSecond,
                    ySpeed.MillimetersPerSecond
                )
            );
            stopwatch.Elapsed.Should().BeCloseTo(expectedDuration, _motionDelayPrecision);
        }

        // Diagonal Movement, X and Y with same speed, but not same range
        private void TestSpeed_XY_WhenDiagonalMovement_withAsymetricRange()
        {
            // Given
            _axes.GotoPosition(new XYPosition(new StageReferential(), 150, 150), AxisSpeed.Normal);
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            const int xRangeInMillimeters = 300;
            const int yRangeInMillimeters = 150;
            var speed = 25.MillimetersPerSecond();

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _axes.GotoPointCustomSpeedAccel(
                new AxisMove(-150, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                new AxisMove(0, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                null,
                null
            );
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            stopwatch.Stop();

            // Then X is used to compute expected duration, since its range is bigger (but same speed)
            var expectedDuration = TimeSpan.FromSeconds(
                Math.Max(xRangeInMillimeters, yRangeInMillimeters) / speed.MillimetersPerSecond
            );
            stopwatch.Elapsed.Should().BeCloseTo(expectedDuration, _motionDelayPrecision);
        }

        // Diagonal Movement is a straight line
        private void TestSpeed_XY_InStraightLine()
        {
            // Given
            _axes.GotoPosition(new XYPosition(new StageReferential(), 0, 0), AxisSpeed.Normal);
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            var speed = 25.MillimetersPerSecond();

            // When moving X+150mm and Y+75mm
            _axes.GotoPointCustomSpeedAccel(
                new AxisMove(150, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                new AxisMove(75, speed.MillimetersPerSecond, DefaultXYAcceleration.MillimetersPerSecondSquared),
                null,
                null
            );

            // Then X=2*Y at any time while on the move
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    var currentPosition = (AnaPosition)_axes.GetPos();
                    currentPosition.X
                        .Should()
                        .BeApproximately(2 * currentPosition.Y, 5);
                    await Task.Delay(s_currentPositionPollingDelay, cancellationTokenSource.Token);
                },
                cancellationTokenSource.Token
            );

            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            cancellationTokenSource.Cancel();
        }

        // ZTop
        private void TestSpeed_ZTop(Speed speed, Acceleration acceleration)
        {
            // Given
            var startPosition = 50.Millimeters();
            var endPosition = -10.Millimeters();
            var travelledDistance = Math.Abs((endPosition - startPosition).Millimeters).Millimeters();

            _axes.GotoPosition(new ZTopPosition(new StageReferential(), startPosition.Millimeters), AxisSpeed.Normal);
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _axes.GotoPointCustomSpeedAccel(
                null,
                null,
                new AxisMove(
                    endPosition.Millimeters,
                    speed.MillimetersPerSecond,
                    acceleration.MillimetersPerSecondSquared
                ),
                null
            );
            _axes.WaitMotionEnd(MaxMotionTimeoutMs);
            stopwatch.Stop();

            // Then
            var expectedDuration = TimeSpan.FromSeconds(travelledDistance.Millimeters / speed.MillimetersPerSecond);
            stopwatch.Elapsed.Should().BeCloseTo(expectedDuration, _motionDelayPrecision);
            Logger.Information($"Expected duration: {expectedDuration} (precision: {_motionDelayPrecision})");
            Logger.Information($"Actual duration: {stopwatch.Elapsed}");
        }
    }
}
