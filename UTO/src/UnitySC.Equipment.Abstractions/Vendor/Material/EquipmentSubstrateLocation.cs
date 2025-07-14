using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    /// <summary>
    /// An Equipment Substrate Location is the location which can hold a substrate on the equipment resource.
    /// </summary>
    public class EquipmentSubstrateLocation : SubstrateLocation
    {
        /// <inheritdoc />
        public EquipmentSubstrateLocation(string name) : base(name)
        {
        }
        public Device Owner => Container as Device;
    }
}
