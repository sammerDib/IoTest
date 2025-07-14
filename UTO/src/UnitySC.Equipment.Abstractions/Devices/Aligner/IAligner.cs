using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Conditions;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner
{
    [Device(IsAbstract = true)]
    public interface IAligner : IUnityCommunicatingDevice, IExtendedMaterialLocationContainer
    {
        #region Status

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

        [Status(Documentation = "Clamp state")]
        bool IsClamped { get; }

        #endregion

        #region Commands

        [Command(Documentation = "Align material at the given target")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsWaferPresent))]
        void Align(
            [Unit(AngleUnit.Degree)] Angle target,
            AlignType alignType = AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);

        [Command(Documentation = "Centering material")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsWaferPresent))]
        void Centering();

        [Command(Documentation = "Prepare the aligner (unclamp, move pins...) for material deposit/removal.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void PrepareTransfer(EffectorType effector, SampleDimension dimension, MaterialType materialType);

        [Command(Documentation = "Secure material on spindle so aligner axes can move without loosing it.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void Clamp();

        [Command(Documentation =
            "Release material on spindle so it can be removed by hand. Caution: this is less safe than PrepareTransfer command.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void Unclamp();

        [Command(Documentation =
            "Move Z Axis command.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void MoveZAxis(bool isBottom);

        [Command(Documentation = "Send current date and time to hardware in order to synchronize logs.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetDateAndTime();

        #endregion Commands

        #region Public

        public void CheckSubstrateDetectionError(bool reset = false);

        #endregion
    }
}
