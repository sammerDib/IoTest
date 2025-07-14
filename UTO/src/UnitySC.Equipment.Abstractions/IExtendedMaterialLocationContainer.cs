using System.Collections.Generic;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions
{
    public interface IExtendedMaterialLocationContainer : IMaterialLocationContainer
    {
        SampleDimension GetMaterialDimension(byte slot = 1);

        /// <summary>
        /// Determines whether the location container is in condition for material transfer (i.e. pick/place).
        /// </summary>
        /// <param name="effector">End-effector that will perform the transfer.</param>
        /// <param name="errorMessages">
        /// Messages indicating why this location container is not ready for transfer.
        /// Empty list when ready (<see langword="true"/> is returned).</param>
        /// <param name="armMaterial">
        /// Material that will be transferred by robot's arm.
        /// i.e.: Useful for location container to check if this kind of material is accepted (before a place);
        /// Can be <see langword="null"/> in case of pick (location container already know what material is on).</param>
        /// <param name="slot">Slot involved in the transfer.</param>
        /// <returns><see langword="true"/> when transfer can be done; Otherwise <see langword="false"/>.</returns>
        bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Agileo.EquipmentModeling.Material armMaterial = null,
            byte slot = 1);
    }
}
