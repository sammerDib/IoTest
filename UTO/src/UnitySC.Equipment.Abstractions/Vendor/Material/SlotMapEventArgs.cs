using System;
using System.Collections.ObjectModel;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
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
