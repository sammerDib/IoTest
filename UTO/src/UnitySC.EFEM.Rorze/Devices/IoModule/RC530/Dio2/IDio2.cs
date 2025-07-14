using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2
{
    /// <summary>
    /// Define all statuses needed by DIO2.
    /// </summary>
    [Device]
    public interface IDio2 : IGenericRC5xx
    {
        #region Statuses

        [Status]
        ErrorCode ErrorDescription { get; }

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

        #region Outputs

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM1 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM2 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM3 { get; }

        #endregion Outputs

        #endregion Statuses
    }
}
