using System;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public class TimestampedPosition
    {
        public Length Position { get; set; }
        public DateTime Timestamp { get; set; }

        public TimestampedPosition(Length position, DateTime timestamp)
        {
            Position = position;
            Timestamp = timestamp;
        }
    }
}
