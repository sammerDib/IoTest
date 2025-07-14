using System.Collections.Generic;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.EquipmentController.Simulator.Driver.EventArgs
{
    /// <summary>
    /// Contain all information about mapping event.
    /// </summary>
    internal class MappingPatternEventArgs: System.EventArgs
    {
        internal MappingPatternEventArgs(Constants.Port port, List<SlotState> slotStates)
        {
            Port       = port;
            SlotStates = slotStates;
        }

        internal Constants.Port Port { get; }

        /// <summary>
        /// The list of <see cref="SlotStates"/> ordered from the carrier bottom to the carrier top.
        /// </summary>
        internal List<SlotState> SlotStates { get; }
    }
}
