using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    public class ArmHistoryItem
    {
        /// <summary>
        /// Indicates the <see cref="IRobot"/> command (pick, place...) that has been performed.
        /// </summary>
        public string Command { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether arm had a material on or not at the end of <see cref="Command"/>.
        /// </summary>
        public bool HasMaterial { get; set; }

        /// <summary>
        /// Indicates on which device the material has been picked/placed.
        /// </summary>
        public TransferLocation Location { get; set; } = TransferLocation.DummyPortA;

        /// <summary>
        /// Indicates on which slot of <see cref="Location"/> the material has been picked/placed.
        /// </summary>
        public byte Slot { get; set; }
    }
}
