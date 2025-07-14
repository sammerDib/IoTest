using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tracking;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class PositionTracker : ITracker
    {
        private readonly ILogger<PositionTracker> _logger;

        private readonly Func<TimestampedPosition> _getPosition;

        private Timer _timer;

        /// <summary>
        /// Dictionnary saving Z positions over time. Keys are long time values expressed in milliseconds since the UTC
        /// unix epoch. Values are positions.
        /// </summary>
        public virtual ConcurrentDictionary<long, Length> PositionsOverTime { get; set; }

        public PositionTracker()
        {
            // Empty constructor for tests purposes
        }

        /// <summary>
        /// This class handles position tracking over time.
        /// </summary>
        /// <param name="getPosition">The function to call in order to retrieve the position.</param>
        /// <param name="trackingPeriod_ms">
        /// Time step in milliseconds used for position tracking. Default value is 10 ms.
        /// </param>
        /// <param name="positionsOverTime"></param>
        public PositionTracker(Func<TimestampedPosition> getPosition, int trackingPeriod_ms = 10, ConcurrentDictionary<long, Length> positionsOverTime = null)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<PositionTracker>>();
            _getPosition = getPosition;

            PositionsOverTime = positionsOverTime ?? new ConcurrentDictionary<long, Length>(Environment.ProcessorCount, 2 ^ 10);

            _timer = new Timer(trackingPeriod_ms);
            _timer.Elapsed += TrackPosition;
        }

        private void TrackPosition(object source, ElapsedEventArgs e)
        {
            var posAndTime = _getPosition.Invoke();
            var zPos = posAndTime.Position;
            var ticks = posAndTime.Timestamp.ToUniversalTime().Ticks;
            PositionsOverTime.TryAdd(ticks, zPos);
        }

        public virtual void Dispose()
        {
            Reset();
            _timer.Elapsed -= TrackPosition;
            _timer.Dispose();
        }

        public void Reset()
        {
            PositionsOverTime.Clear();
        }

        public virtual void StartTracking()
        {
            _timer.Start();
        }

        public virtual void StopTracking()
        {
            _timer.Stop();
        }

        public List<KeyValuePair<long, Length>> TimeOrderedPositions => (from timedPosition in PositionsOverTime orderby timedPosition.Key ascending select timedPosition).ToList();

        /// <summary>
        /// Neighbours: interpollation is based on the nearest timed records of TimeOrderedPositions.
        /// Boundaries: interpollation is based on the earliest tracked position (first record after StartTracking()
        /// call) and the latest tracked position (last record after StopTracking() call).
        /// </summary>
        public enum Interpollation
        {
            Boundaries,
            Neighbours,
        }

        /// <summary>
        /// Calculates and returns the interpollated position at the given time. Linear movement is assumed between two
        /// consecutive timesteps.
        /// </summary>
        /// <param name="ticks">Time in number of ticks.</param>
        /// <param name="interpollation"></param>
        public Length GetPositionAtTime(long ticks, Interpollation interpollation = Interpollation.Neighbours)
        {
            var timeOrderedPositions = TimeOrderedPositions;
            long earliestTrackedTime = timeOrderedPositions.First().Key;
            long latestTrackedTime = timeOrderedPositions.Last().Key;

            // Out of bounds and bounary cases
            if (ticks <= earliestTrackedTime)
            {
                _logger?.Debug($"Time value {ticks} is earlier or equals to the earliest tracked position. Earliest tracked position value returned.");
                return timeOrderedPositions.First().Value;
            }

            if (ticks >= latestTrackedTime)
            {
                _logger?.Debug($"Time value {ticks} is later or equals to the lastest tracked position. Latest tracked position value returned.");
                return timeOrderedPositions.Last().Value;
            }

            // Within range cases, need to interpolate
            switch (interpollation)
            {
                case Interpollation.Neighbours:
                    {
                        int indexOfNextTimestamp = timeOrderedPositions.FindIndex((keyValuePair) => keyValuePair.Key > ticks);
                        var prev = timeOrderedPositions[indexOfNextTimestamp - 1];
                        var next = timeOrderedPositions[indexOfNextTimestamp];

                        double a = (next.Value.Millimeters - prev.Value.Millimeters) / (next.Key - prev.Key);
                        double b = next.Value.Millimeters - (a * next.Key);

                        var interpolatedPosition = ((a * ticks) + b).Millimeters();
                        return interpolatedPosition;
                    }
                case Interpollation.Boundaries:
                    {
                        double earliestTrackedPositio_mm = timeOrderedPositions.First().Value.Millimeters;
                        double latestTrackedPosition_mm = timeOrderedPositions.Last().Value.Millimeters;

                        double a = (latestTrackedPosition_mm - earliestTrackedPositio_mm) / (latestTrackedTime - earliestTrackedTime);
                        double b = latestTrackedPosition_mm - (a * latestTrackedTime);

                        var interpolatedPosition = ((a * ticks) + b).Millimeters();
                        return interpolatedPosition;
                    }
                default: throw new Exception($"Unsupported interpollation value: {interpollation}.");
            }
        }
    }
}
