using System;

using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;

using CarrierType = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Helpers
{
    /// <summary>
    ///     Provide all static behavior to convert a RE201 element into a more generic LoadPort element
    /// </summary>
    public static class Converters
    {
        /// <summary>
        ///     Convert a <see cref="RE201SlotState" /> into a <see cref="SlotState" />.
        /// </summary>
        /// <param name="slotState">The specific slot state.</param>
        /// <returns>The generic slot state.</returns>
        public static SlotState ToAbstractionSlotState(RE201SlotState slotState)
        {
            switch (slotState)
            {
                case RE201SlotState.WaferDoesNotExist:
                    return SlotState.NoWafer;

                case RE201SlotState.WaferExists:
                    return SlotState.HasWafer;

                case RE201SlotState.ThicknessAbnormal_ThickWafer:
                    return SlotState.Thick;

                case RE201SlotState.ThicknessAbnormal_ThinWafer:
                    return SlotState.Thin;

                case RE201SlotState.FrontBow:
                    return SlotState.FrontBow;

                case RE201SlotState.CrossedWafer:
                    return SlotState.CrossWafer;

                case RE201SlotState.SeveralWafersInSameSlot:
                    return SlotState.DoubleWafer;

                default:
                    return SlotState.NoWafer;
            }
        }

        /// <summary>
        ///     Convert a <see cref="LoadPortLightRoleType" /> into a <see cref="LoadPortIndicators" />.
        /// </summary>
        public static LoadPortIndicators ToRe201Light(LoadPortLightRoleType light)
        {
            switch (light)
            {
                case LoadPortLightRoleType.LoadReady:
                    return LoadPortIndicators.Load;

                case LoadPortLightRoleType.UnloadReady:
                    return LoadPortIndicators.Unload;

                default:
                    throw new ArgumentOutOfRangeException(nameof(light), light, null);
            }
        }

        public static string ToCarrierTypeCmdParam(CarrierType carrierType)
        {
            switch (carrierType)
            {
                case CarrierType.Auto:
                    return "AUTO";

                case CarrierType.NotIdentified:
                    return "----";

                default:
                    return "AUTO";
            }
        }

        public static string ToSetLpLightCmdParam(LoadPortIndicators lpIndicator)
        {
            switch (lpIndicator)
            {
                case LoadPortIndicators.Green:
                    return "16";

                case LoadPortIndicators.Red:
                    return "17";

                case LoadPortIndicators.Load:
                    return "18";

                case LoadPortIndicators.Unload:
                    return "19";

                case LoadPortIndicators.Access:
                    return "20";

                default:
                    return string.Empty;
            }
        }

        public static string ToLightStateCmdParam(LightState mode)
        {
            switch (mode)
            {
                case LightState.Undetermined:
                    return "0";

                case LightState.Off:
                    return "0";

                case LightState.On:
                    return "1";

                case LightState.Flashing:
                    return "1000";

                case LightState.FlashingSlow:
                    return "2000";

                case LightState.FlashingFast:
                    return "500";

                default:
                    return "0";
            }
        }
    }
}
