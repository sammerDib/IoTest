using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Material
{
    public class WaferLocation : EquipmentSubstrateLocation
    {
        /// <inheritdoc />
        public WaferLocation(string name) : base(name)
        {
        }

        public Wafer Wafer => Material as Wafer;
    }
}
