using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.ProcessModule
{
    [Device(IsAbstract = true)]
    public interface IProcessModule : IUnityCommunicatingDevice, IExtendedMaterialLocationContainer
    {
        [Status]
        bool IsOutOfService { get; }

        [Status]
        bool IsDoorOpen { get; }

        [Status]
        bool IsReadyToLoadUnload { get; }

        [Status(Documentation = "Wafer dimension")]
        SampleDimension WaferDimension { get; }

        [Status(Documentation = "Simplified wafer ID")]
        string SimplifiedWaferId { get; }

        [Status(Documentation = "Wafer status")]
        WaferStatus WaferStatus { get; }

        [Status(Documentation = "Wafer presence")]
        WaferPresence WaferPresence { get; }

        [Status(Documentation = "Substrate detection error")]
        bool SubstrateDetectionError { get; }

        #region Public

        public void CheckSubstrateDetectionError(bool reset = false);

        #endregion
    }
}
