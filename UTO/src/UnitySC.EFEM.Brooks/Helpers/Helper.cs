using BAI.Systems.Common;
using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.Systems.Modules.EFEM;

using UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem;
using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot;

namespace UnitySC.EFEM.Brooks.Helpers
{
    public static class Helper
    {
        public static string GetBrooksLocation(BrooksRobot robot, TransferLocation location, byte slot)
        {
            var efem = robot.GetTopDeviceContainer().AllOfType<BrooksEfem>().Single();

            switch (location)
            {
                case TransferLocation.LoadPort1:
                    return $"{efem.Configuration.BrooksLoadPort1LocationName}.Slot{slot}";
                case TransferLocation.LoadPort2:
                    return $"{efem.Configuration.BrooksLoadPort2LocationName}.Slot{slot}";
                case TransferLocation.LoadPort3:
                    return $"{efem.Configuration.BrooksLoadPort3LocationName}.Slot{slot}";
                case TransferLocation.LoadPort4:
                    return $"{efem.Configuration.BrooksLoadPort4LocationName}.Slot{slot}";
                case TransferLocation.PreAlignerA:
                    return efem.TryGetDevice<BrooksAligner>().Configuration.BrooksChuckName;
                case TransferLocation.ProcessModuleA:
                    return $"{efem.Configuration.BrooksProcessModuleALocationName}.Slot{slot}";
                case TransferLocation.ProcessModuleB:
                    return $"{efem.Configuration.BrooksProcessModuleBLocationName}.Slot{slot}";
                case TransferLocation.ProcessModuleC:
                    return $"{efem.Configuration.BrooksProcessModuleCLocationName}.Slot{slot}";
                default:
                    throw new InvalidOperationException("Unknown transfer location");
            }
        }

        public static WaferPresence ConvertPresenceStateToWaferPresence(WaferPresenceState waferPresenceState)
        {
            switch (waferPresenceState)
            {
                case WaferPresenceState.Absent:
                    return WaferPresence.Absent;
                case WaferPresenceState.Present:
                    return WaferPresence.Present;
                case WaferPresenceState.Error:
                case WaferPresenceState.Unknown:
                case WaferPresenceState.Unavailable:
                case WaferPresenceState.Double:
                case WaferPresenceState.Cross:
                case WaferPresenceState.Tilt:
                case WaferPresenceState.Shift:
                case WaferPresenceState.TiltUp:
                case WaferPresenceState.TiltDown:
                case WaferPresenceState.WrongSize:
                default:
                    return WaferPresence.Unknown;
            }
        }

        public static SampleDimension ConvertDiameterInSampleDimension(double diameter)
        {
            switch (diameter)
            {
                case 100:
                    return SampleDimension.S100mm;
                case 150:
                    return SampleDimension.S150mm;
                case 200:
                    return SampleDimension.S200mm;
                case 300:
                    return SampleDimension.S300mm;
                case 450:
                    return SampleDimension.S450mm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(diameter));
            }
        }

        public static WaferAlignFeature ConvertMaterialTypeToWaferAlignFeature(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.SiliconWithNotch:
                    return WaferAlignFeature.Notch;
                case MaterialType.Unknown:
                case MaterialType.Frame:
                case MaterialType.SiliconWithoutNotch:
                case MaterialType.Glass:
                    return WaferAlignFeature.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(materialType), materialType, null);
            }
        }

        public static EfemProxy GetEfemProxy(Device device)
        {
            var efem = device.GetTopDeviceContainer().AllOfType<BrooksEfem>().Single();

            return efem == null ? null : new EfemProxy(efem.Configuration.BrooksEfemName, efem.Configuration.BrooksClientName);
        }
    }
}
