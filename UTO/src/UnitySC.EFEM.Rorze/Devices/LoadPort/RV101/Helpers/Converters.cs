using System;

using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;

using CarrierType = UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Helpers
{
    /// <summary>
    /// Provide all static behavior to convert a RV101 element into a more generic LoadPort element
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Convert a <see cref="RV101SlotState"/> into a <see cref="SlotState"/>.
        /// </summary>
        /// <param name="slotState">The specific slot state.</param>
        /// <returns>The generic slot state.</returns>
        public static SlotState ToAbstractionSlotState(RV101SlotState slotState)
        {
            switch (slotState)
            {
                case RV101SlotState.WaferDoesNotExist:
                    return SlotState.NoWafer;

                case RV101SlotState.WaferExists:
                    return SlotState.HasWafer;

                case RV101SlotState.ThicknessAbnormal_ThickWafer:
                    return SlotState.Thick;

                case RV101SlotState.ThicknessAbnormal_ThinWafer:
                    return SlotState.Thin;

                case RV101SlotState.FrontBow:
                    return SlotState.FrontBow;

                case RV101SlotState.CrossedWafer:
                    return SlotState.CrossWafer;

                case RV101SlotState.SeveralWafersInSameSlot:
                    return SlotState.DoubleWafer;

                default:
                    return SlotState.NoWafer;
            }
        }

        /// <summary>
        /// Convert a <see cref="LoadPortLightRoleType"/> into a <see cref="LoadPortIndicators"/>.
        /// </summary>
        public static LoadPortIndicators ToRv101Light(LoadPortLightRoleType light)
        {
            switch (light)
            {
                case LoadPortLightRoleType.LoadReady:
                    return LoadPortIndicators.Load;

                case LoadPortLightRoleType.UnloadReady:
                    return LoadPortIndicators.Unload;

                case LoadPortLightRoleType.HandOffButton:
                    return LoadPortIndicators.Latch;

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

                case CarrierType.FOUP:
                    return "FOUP";

                case CarrierType.FOSB:
                    return "FOSB";

                case CarrierType.OpenCassette:
                    return "OC25";

                case CarrierType.Foup1Slot:
                    return "FP01";

                case CarrierType.CarrierOnAdapter:
                    return "OC13";

                case CarrierType.Special:
                    return "ELSE";

                case CarrierType.NotIdentified:
                    return "FP13";

                default:
                    return "AUTO";
            }
        }

        public static string ToSetLpLightCmdParam(LoadPortIndicators lpIndicator)
        {
            switch (lpIndicator)
            {
                case LoadPortIndicators.Load:
                    return "42";

                case LoadPortIndicators.Unload:
                    return "43";

                case LoadPortIndicators.Latch:
                    return "46";

                case LoadPortIndicators.Error:
                    return "47";

                case LoadPortIndicators.Busy:
                    return "50";

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
