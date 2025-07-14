using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem;

namespace UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem
{
    [Device]
    public interface IBrooksEfem : IEfem
    {
        #region Inputs

        [Status(Category = "Inputs")]
        bool I_PM1_DoorOpened { get; }

        [Status(Category = "Inputs")]
        bool I_PM1_ReadyToLoadUnload { get; }

        [Status(Category = "Inputs")]
        bool I_PM2_DoorOpened { get; }

        [Status(Category = "Inputs")]
        bool I_PM2_ReadyToLoadUnload { get; }

        [Status(Category = "Inputs")]
        bool I_PM3_DoorOpened { get; }

        [Status(Category = "Inputs")]
        bool I_PM3_ReadyToLoadUnload { get; }

        #endregion Inputs
    }
}
