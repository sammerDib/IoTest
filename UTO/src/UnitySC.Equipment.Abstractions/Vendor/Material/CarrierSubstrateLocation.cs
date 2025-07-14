namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    /// <summary>
    /// A Carrier Substrate Location is the location which can hold a substrate in a carrier.
    /// </summary>
    public class CarrierSubstrateLocation : SubstrateLocation
    {
        /// <inheritdoc />
        public CarrierSubstrateLocation(string name) : base(name)
        {
        }

        public Carrier Owner => Container as Carrier;
    }
}
