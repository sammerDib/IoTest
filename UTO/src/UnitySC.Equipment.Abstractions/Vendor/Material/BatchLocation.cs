using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Vendor.Material
{
    /// <summary>
    /// A Batch Location may hold a group of substrates.
    /// </summary>
    class BatchLocation : MaterialLocation
    {
        private readonly ZeroToOneReference<Substrate> _substrates;

        /// <inheritdoc />
        public BatchLocation(string name) : base(name)
        {
            _substrates = ReferenceFactory.ZeroToOneReference<Substrate>(nameof(Substrate), this);
        }
    }
}
