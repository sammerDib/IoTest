using System;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions
{
    public static class MaterialLocationHelper
    {
        /// <summary>
        /// Retrieve SubstrateLocation using provided parameters
        /// </summary>
        /// <param name="equipment">The equipment instance.</param>
        /// <param name="materialLocationContainer">The instance of device holding the SubstrateLocation.</param>
        /// <param name="locationIndex">The index of SubstrateLocation to retrieve.</param>
        /// <returns>Instance of required SubstrateLocation if found.</returns>
        public static SubstrateLocation RetrieveSubstrateLocation(Agileo.EquipmentModeling.Equipment equipment, IMaterialLocationContainer materialLocationContainer, byte locationIndex = 0)
        {
            MaterialLocation materialLocation;
            if (materialLocationContainer == null)
            {
                throw new ArgumentNullException(nameof(materialLocationContainer), "MaterialLocationContainer cannot be null.");
            }

            if (materialLocationContainer is LoadPort loadPort)
            {
                if (loadPort.Carrier == null)
                {
                    throw new ArgumentException($"No carrier found on '{materialLocationContainer.Name}'.");
                }
                if (locationIndex == 0 || locationIndex > loadPort.Carrier.MaterialLocations.Count)
                {
                    throw new ArgumentException($"Location index does not match carrier capacity '{loadPort.Carrier.Capacity}'.");
                }

                // Get slot in carrier
                materialLocation = loadPort.Carrier.MaterialLocations[locationIndex - 1];
            }
            else if (locationIndex < 1)
            {
                // Get single location
                materialLocation = materialLocationContainer.MaterialLocations.SingleOrDefault();
            }
            else
            {
                if (locationIndex > materialLocationContainer.MaterialLocations.Count)
                {
                    throw new ArgumentException($"Slot '{locationIndex}' is greater than number of locations in device '{materialLocationContainer.Name}'.");
                }

                // Get location corresponding to index
                materialLocation = materialLocationContainer.MaterialLocations[locationIndex - 1];
            }

            if (materialLocation is not SubstrateLocation substrateLocation)
            {
                throw new ArgumentException($"No substrate location retrieved for device '{materialLocationContainer}' slot '{locationIndex}'.");
            }

            return substrateLocation;
        }
    }
}
