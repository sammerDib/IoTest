using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Controls
{
    /// <summary>
    /// Represent an Item with Slot, Id, and color
    /// </summary>
    public class ItemUi
    {
        /// <summary>
        /// Slot number in carrier
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// Item Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Color of Item to display in Carrier and Map table
        /// </summary>
        public DisplayColor Color { get; set; }
    }
}
