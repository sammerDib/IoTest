using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    /// <summary>
    /// A Substrate Location may hold a substrate.
    /// Substrate Location is classified into Equipment Substrate Location and Carrier Substrate Location.
    /// </summary>
    public abstract class SubstrateLocation : MaterialLocation
    {
        /// <inheritdoc />
        public SubstrateLocation(string name) : base(name)
        {
        }

        public Substrate Substrate => Material as Substrate;
    }
}
