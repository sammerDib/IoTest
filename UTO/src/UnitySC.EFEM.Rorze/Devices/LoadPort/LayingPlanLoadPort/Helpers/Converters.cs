using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort.Helpers
{
    /// <summary>
    /// Provide all static behavior to convert a RR75x element into a more generic LoadPort element
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Convert a <see cref="RR75xSlotState" /> into a <see cref="SlotState" />.
        /// </summary>
        /// <param name="slotState">The specific slot state.</param>
        /// <returns>The generic slot state.</returns>
        public static SlotState ToAbstractionSlotState(RR75xSlotState slotState)
        {
            switch (slotState)
            {
                case RR75xSlotState.WaferDoesNotExist:
                    return SlotState.NoWafer;

                case RR75xSlotState.WaferExists:
                    return SlotState.HasWafer;

                case RR75xSlotState.ThicknessAbnormal_ThickWafer:
                    return SlotState.Thick;

                case RR75xSlotState.ThicknessAbnormal_ThinWafer:
                    return SlotState.Thin;

                case RR75xSlotState.FrontBow:
                    return SlotState.FrontBow;

                case RR75xSlotState.CrossedWafer:
                    return SlotState.CrossWafer;

                case RR75xSlotState.SeveralWafersInSameSlot:
                    return SlotState.DoubleWafer;

                default:
                    return SlotState.NoWafer;
            }
        }
    }
}
