using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000
{
    [Device]
    public interface IEK9000 : IUnityCommunicatingDevice
    {

        #region Statuses

        #region Inputs

        [Status(Category = "Inputs")]
        bool I_EMO_Status { get; }

        [Status(Category = "Inputs")]
        bool I_FFU_Alarm { get; }

        [Status(Category = "Inputs")]
        bool I_VacuumPressureSensor { get; }

        [Status(Category = "Inputs")]
        bool I_CDA_PressureSensor { get; }

        [Status(Category = "Inputs")]
        bool I_ServiceLightLed { get; }

        [Status(Category = "Inputs")]
        bool I_AirFlowPressureSensorIonizer { get; }

        [Status(Category = "Inputs")]
        bool I_Ionizer1Status { get; }

        [Status(Category = "Inputs")]
        bool I_RV201Interlock { get; }

        [Status(Category = "Inputs")]
        bool I_MaintenanceSwitch { get; }

        [Status(Category = "Inputs")]
        bool I_RobotDriverPower { get; }

        [Status(Category = "Inputs")]
        bool I_EFEM_DoorStatus { get; }

        [Status(Category = "Inputs")]
        bool I_TPMode { get; }

        [Status(Category = "Inputs")]
        bool I_OCRTableAlarm { get; }

        [Status(Category = "Inputs")]
        bool I_OCRTablePositionReach { get; }

        [Status(Category = "Inputs")]
        bool I_OCRWaferReaderLimitSensor1 { get; }

        [Status(Category = "Inputs")]
        bool I_OCRWaferReaderLimitSensor2 { get; }

        [Status(Category = "Inputs", IsLoggingActivated = false)]
        Pressure I_DifferentialAirPressureSensor { get; }

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

        [Status(Category = "Outputs")]
        bool O_OCRTableDrive { get; }

        [Status(Category = "Outputs")]
        bool O_OCRTableReset { get; }

        [Status(Category = "Outputs")]
        bool O_OCRTableInitialization { get; }

        [Status(Category = "Outputs")]
        [Unit(RotationalSpeedUnit.RevolutionPerMinute)]
        RotationalSpeed O_FFU_Speed { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM1 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM2 { get; }

        [Status(Category = "Outputs")]
        bool O_RobotArmNotExtended_PM3 { get; }

        #endregion Outputs

        [Status]
        bool Alarm { get; }

        [Status]
        Pressure MeanPressure { get; }

        [Status]
        RotationalSpeed FanSpeed { get; }

        #endregion Statuses

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetDigitalOutput(DigitalOutputs output, bool value);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetAnalogOutput(AnalogOutputs output, double value);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetFfuSpeed(RotationalSpeed setPoint);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetDateAndTime();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetLightColor(LightColors color, LightState mode);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetBuzzerState(BuzzerState state);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetReaderPosition(SampleDimension dimension);

        #endregion Commands

    }
}
