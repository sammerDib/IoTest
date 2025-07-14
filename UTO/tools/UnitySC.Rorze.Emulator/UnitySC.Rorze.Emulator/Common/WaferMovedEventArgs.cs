using System;

namespace UnitySC.Rorze.Emulator.Common
{
    public class WaferMovedEventArgs : EventArgs
    {
        public int LocationId { get; }

        public int Slot { get; }

        public WaferMovedEventArgs(int locationId, int slot)
        {
            LocationId = locationId;
            Slot = slot;
        }
    }
}
