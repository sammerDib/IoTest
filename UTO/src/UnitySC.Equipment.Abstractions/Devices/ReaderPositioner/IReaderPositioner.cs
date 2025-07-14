using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.ReaderPositioner
{
    [Device(IsAbstract = true)]
    public interface IReaderPositioner : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status]
        SampleDimension CurrentPosition { get; }

        #endregion

        #region Commands

        [Command(Documentation = "Set the position of the reader.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void SetPosition(SampleDimension dimension);

        #endregion
    }
}
