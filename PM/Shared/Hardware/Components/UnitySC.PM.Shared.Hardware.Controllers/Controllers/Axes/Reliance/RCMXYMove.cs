using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance
{
    /// <summary>
    /// This class represents a move on X and Y axes.
    /// </summary>
    public class RCMXYMove
    {
        public readonly RCMAxisMove X;
        public readonly RCMAxisMove Y;

        /// <param name="coordinates">In millimeters in motor referential</param>
        /// <param name="axes"></param>
        /// <param name="speeds">in millimeters per second</param>
        public RCMXYMove(
            List<double> coordinates,
            List<IAxis> axes,
            List<double> speeds,
            List<double> accelerations
        )
        {
            Validate(coordinates, axes, speeds, accelerations);

            int xIndex = axes.FindIndex(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.X);
            X = new RCMAxisMove(
                axes[xIndex] as RelianceAxis,
                coordinates[xIndex].Millimeters(),
                speeds[xIndex].MillimetersPerSecond(),
                accelerations[xIndex].MillimetersPerSecondSquared()
            );
            int yIndex = axes.FindIndex(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.Y);
            Y = new RCMAxisMove(
                axes[yIndex] as RelianceAxis,
                coordinates[yIndex].Millimeters(),
                speeds[yIndex].MillimetersPerSecond(),
                accelerations[yIndex].MillimetersPerSecondSquared()
            );

            AdaptSpeedForStraightPath();
        }

        public RCMXYMove(List<double> coordinates, List<IAxis> axes, List<AxisSpeed> speeds)
        {
            Validate(coordinates, axes, speeds);

            int xIndex = axes.FindIndex(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.X);
            X = new RCMAxisMove(axes[xIndex] as RelianceAxis,
                coordinates[xIndex].Millimeters(),
                speeds[xIndex]
            );
            int yIndex = axes.FindIndex(axis => axis.AxisConfiguration.MovingDirection == MovingDirection.Y);
            Y = new RCMAxisMove(axes[yIndex] as RelianceAxis,
                coordinates[yIndex].Millimeters(),
                speeds[yIndex]
            );

            AdaptSpeedForStraightPath();
        }

        private void Validate<T>(List<double> coordinates, List<IAxis> axes, List<T> speeds, List<double> accelerations = null)
        {
            CheckListCount(coordinates, 2, "coordinates");
            CheckListCount(axes, 2, "axes");
            CheckListCount(speeds, 2, "speeds");

            if (!(accelerations is null))
            {
                CheckListCount(accelerations, 2, "accelerations");
            }

            CheckListContainsAxis(axes, MovingDirection.X);
            CheckListContainsAxis(axes, MovingDirection.Y);
        }

        private static void CheckListCount<T>(List<T> list, int expectedCount, string content)
        {
            if (list.Count != expectedCount)
            {
                throw new Exception($"{expectedCount} {content} expected, {list.Count} founded");
            }
        }

        private void CheckListContainsAxis(List<IAxis> axes, MovingDirection movingDirection)
        {
            if (!axes.Exists(axis => axis.AxisConfiguration.MovingDirection == movingDirection))
            {
                throw new Exception($"cannot find axis {movingDirection}");
            }
        }

        /// <summary>
        /// Adapt X or Y speed to make sure move is straight path.
        /// </summary>
        private void AdaptSpeedForStraightPath()
        {
            if (X.Duration < Y.Duration)
            {
                X.Speed = (X.Distance.Millimeters / Y.Duration.TotalSeconds).MillimetersPerSecond();
            }
            else
            {
                Y.Speed = (Y.Distance.Millimeters / X.Duration.TotalSeconds).MillimetersPerSecond();
            }
        }
    }
}
