using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

using ErrorCode = UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums.ErrorCode;
using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1
{
    /// <summary>Define all statuses needed by DIO1.</summary>
    [Device]
    public interface IDio1 : IGenericRC5xx
    {
        #region Statuses

        [Status]
        ErrorCode ErrorDescription { get; }

        #region Inputs

        [Status(Category = "Inputs")]
        bool I_PressureSensor_VAC { get; }

        [Status(Category = "Inputs")]
        bool I_PressureSensor_AIR { get; }

        [Status(Category = "Inputs")]
        bool I_Led_PushButton { get; }

        [Status(Category = "Inputs")]
        bool I_PressureSensor_ION_AIR { get; }

        [Status(Category = "Inputs")]
        bool I_Ionizer1Alarm { get; }

        [Status(Category = "Inputs")]
        bool I_RV201Interlock { get; }

        [Status(Category = "Inputs")]
        bool I_MaintenanceSwitch { get; }

        [Status(Category = "Inputs")]
        bool I_DriverPower { get; }

        [Status(Category = "Inputs")]
        bool I_DoorStatus { get; }

        [Status(Category = "Inputs")]
        bool I_TPMode { get; }

        [Status(Category = "Inputs")]
        bool I_OCRWaferReaderLimitSensor1 { get; }

        [Status(Category = "Inputs")]
        bool I_OCRWaferReaderLimitSensor2 { get; }

        [Status(Category = "Inputs")]
        bool I_LightCurtain { get; }

        #endregion Inputs

        #region Outputs

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
        bool O_OCRWaferReaderValve1 { get; }

        [Status(Category = "Outputs")]
        bool O_OCRWaferReaderValve2 { get; }

        #endregion Outputs

        #endregion Statuses

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetLightColor(LightColors color, LightState mode);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetBuzzerState(BuzzerState state);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetReaderPosition(SampleDimension dimension);

        #endregion
    }
}
