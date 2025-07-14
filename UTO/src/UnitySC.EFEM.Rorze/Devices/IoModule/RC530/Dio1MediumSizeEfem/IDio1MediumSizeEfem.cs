using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

using ErrorCode = UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode;
using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem
{
    [Device]
    public interface IDio1MediumSizeEfem : IGenericRC5xx
    {
        #region Statuses

        [Status]
        ErrorCode ErrorDescription { get; }

        #region Inputs

        [Status(Category = "Inputs")]
        bool I_MaintenanceSwitch { get; }

        [Status(Category = "Inputs")]
        bool I_PressureSensor_VAC { get; }

        [Status(Category = "Inputs")]
        bool I_Led_PushButton { get; }

        [Status(Category = "Inputs")]
        bool I_PressureSensor_ION_AIR { get; }

        [Status(Category = "Inputs")]
        bool I_Ionizer1Alarm { get; }

        [Status(Category = "Inputs")]
        bool I_LightCurtain { get; }

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

        #endregion

        #region Outputs

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM1 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM2 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM3 { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_LightningRed { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_LightningYellow { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_LightningGreen { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_LightningBlue { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_BlinkingRed { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_BlinkingYellow { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_BlinkingGreen { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_BlinkingBlue { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_Buzzer1 { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_Buzzer2 { get; }

        [Status(Category = "Outputs")]
        bool O_SignalTower_PowerSupply { get; }

        #endregion

        #endregion

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetLightColor(LightColors color, LightState mode);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetBuzzerState(BuzzerState state);

        #endregion
    }
}
