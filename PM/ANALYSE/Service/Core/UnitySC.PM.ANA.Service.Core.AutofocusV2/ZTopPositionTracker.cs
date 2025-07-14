using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tracking;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.AutofocusV2
{
    public class ZTopPositionTracker : ITracker
    {
        private readonly ILogger<ZTopPositionTracker> _logger;

        /// <summary>
        /// Stage axis to be controled.
        /// </summary>
        private readonly IAxes _axes;

        private bool _tracking = false;

        /// <summary>
        /// Time step in milliseconds used for position tracking.
        /// </summary>
        public int TrackingPeriod_ms { get; set; } = 1;

        /// <summary>
        /// Dictionnary saving Z positions over time. Keys are long time values expressed in milliseconds since the UTC
        /// unix epoch. Values are Z positions within the scanRange.
        /// </summary>
        public virtual Dictionary<long, Length> ZTopPositionsOverTime { get; set; }

        public ZTopPositionTracker(IAxes axes, Dictionary<long, Length> zTopPositionsOverTime = null)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<ZTopPositionTracker>>();

            _axes = axes;
            ZTopPositionsOverTime = zTopPositionsOverTime ?? new Dictionary<long, Length>();
        }

        public virtual void StartTracking()
        {
            _tracking = true;
            Task.Run(() => TrackPosition());
        }

        private void TrackPosition()
        {
            while (_tracking)
            {
                ZTopPositionsOverTime.Add(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), GetCurrentZTopPosition());
                Thread.Sleep(TrackingPeriod_ms);
            }
        }

        public void StopTracking()
        {
            _tracking = false;
        }

        public void Reset()
        {
            ZTopPositionsOverTime.Clear();
        }

        public Length GetCurrentZTopPosition()
        {
            var currentPosition = _axes.GetPos();

            if (currentPosition is XYZTopZBottomPosition position)
            {
                return position.ZTop.Millimeters();
            }
            else
            {
                throw new Exception("Error: current position is not a XYZTopZBottomPosition");
            }
        }

        /// <summary>
        /// Calculates and returns the position value at the given time, assuming linear movement between two
        /// consecutive timesteps.
        /// </summary>
        /// <param name="unixTime_ms">Unix time in millisecond.</param>
        public Length FindPositionAtTime(long unixTime_ms)
        {
            var timeOrderedPositions = TimeOrderedPositions;
            long earliestTrackedTime = timeOrderedPositions.First().Key;
            long latestTrackedTime = timeOrderedPositions.Last().Key;

            // Out of bounds and bounary cases
            if (unixTime_ms <= earliestTrackedTime)
            {
                _logger.Debug($"Time value {unixTime_ms} is earlier or equals to the earliest tracked position. Earliest tracked position value returned.");
                return timeOrderedPositions.First().Value;
            }

            if (unixTime_ms >= latestTrackedTime)
            {
                _logger.Debug($"Time value {unixTime_ms} is later or equals to the lastest tracked position. Latest tracked position value returned.");
                return timeOrderedPositions.Last().Value;
            }

            // Within range cases, need to interpolate
            int indexOfNextTimestamp = timeOrderedPositions.FindIndex((keyValuePair) => keyValuePair.Key > unixTime_ms);
            var prev = timeOrderedPositions[indexOfNextTimestamp - 1];
            var next = timeOrderedPositions[indexOfNextTimestamp];

            double a = (next.Value.Millimeters - prev.Value.Millimeters) / (next.Key - prev.Key);
            double b = next.Value.Millimeters - (a * next.Key);

            var focusPosition = ((a * unixTime_ms) + b).Millimeters();
            return focusPosition;
        }

        /// <summary>
        /// Returns the ZTop position at the given time, using linear interpolation. Linear interpolation is based on
        /// the earliest tracked position (first record after StartTracking() call) and the latest tracked position
        /// (last record after StopTracking() call).
        /// </summary>
        public Length PositionInterpollationAtTime(long unixTime_ms)
        {
            var timeOrderedPositions = TimeOrderedPositions;
            long earliestTrackedTime = timeOrderedPositions.First().Key;
            long latestTrackedTime = timeOrderedPositions.Last().Key;

            // Out of bounds and bounary cases
            if (unixTime_ms <= earliestTrackedTime)
            {
                _logger.Debug($"Time value {unixTime_ms} is earlier or equals to the earliest tracked position. Earliest tracked position value returned.");
                return timeOrderedPositions.First().Value;
            }

            if (unixTime_ms >= latestTrackedTime)
            {
                _logger.Debug($"Time value {unixTime_ms} is later or equals to the lastest tracked position. Latest tracked position value returned.");
                return timeOrderedPositions.Last().Value;
            }

            // Within range cases, need to interpolate
            double earliestTrackedPositio_mm = timeOrderedPositions.First().Value.Millimeters;
            double latestTrackedPosition_mm = timeOrderedPositions.Last().Value.Millimeters;

            double a = (latestTrackedPosition_mm - earliestTrackedPositio_mm) / (latestTrackedTime - earliestTrackedTime);
            double b = latestTrackedPosition_mm - (a * latestTrackedTime);

            var interpolatedPosition = ((a * unixTime_ms) + b).Millimeters();
            return interpolatedPosition;
        }

        private List<KeyValuePair<long, Length>> TimeOrderedPositions => (from timedPosition in ZTopPositionsOverTime orderby timedPosition.Key ascending select timedPosition).ToList();

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine("Time ZTopPosition"); // Header
            TimeOrderedPositions.ForEach((KeyValuePair<long, Length> timedPosition) => result.AppendLine($"{timedPosition.Key} {timedPosition.Value}"));
            return result.ToString();
        }

        public void Dispose()
        {
            ZTopPositionsOverTime.Clear();
        }
    }
}
