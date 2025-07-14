using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    /// <summary>
    /// A Carrier Location may hold a carrier.
    /// </summary>
    public class CarrierLocation : MaterialLocation
    {
        /// <inheritdoc />
        public CarrierLocation(string name) : base(name)
        {
        }

        public Carrier Carrier => Material as Carrier;
    }
}
