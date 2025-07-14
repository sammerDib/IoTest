using System;
using System.Collections.ObjectModel;

namespace UnitySC.Equipment.Abstractions.Material
{
    public class SlotMapEventArgs : EventArgs
    {
        public SlotMapEventArgs(ReadOnlyCollection<SlotState> slotMap)
        {
            SlotMap = slotMap;
        }

        public ReadOnlyCollection<SlotState> SlotMap { get; }
    }
}
