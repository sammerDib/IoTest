using System;
using System.Threading;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Simulation;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000
{
    public partial class EK9000 : ISimDevice
    {
        #region Properties

        protected internal EK9000SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new EK9000SimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected virtual void InternalSimulateSetDigitalOutput(
            DigitalOutputs output,
            bool value,
            Tempomat tempomat)
        {
            switch (output)
            {
                case DigitalOutputs.O_SignalTower_LightningRed:
                    SimulationData.O_SignalTower_LightningRed = value;
                    break;
                case DigitalOutputs.O_SignalTower_LightningYellow:
                    SimulationData.O_SignalTower_LightningYellow = value;
                    break;
                case DigitalOutputs.O_SignalTower_LightningGreen:
                    SimulationData.O_SignalTower_LightningGreen = value;
                    break;
                case DigitalOutputs.O_SignalTower_LightningBlue:
                    SimulationData.O_SignalTower_LightningBlue = value;
                    break;
                case DigitalOutputs.O_SignalTower_Buzzer1:
                    SimulationData.O_SignalTower_Buzzer1 = value;
                    break;
                case DigitalOutputs.O_OCRWaferReaderValve1:
                    SimulationData.O_OCRWaferReaderValve1 = value;
                    break;
                case DigitalOutputs.O_OCRWaferReaderValve2:
                    SimulationData.O_OCRWaferReaderValve2 = value;
                    break;
                case DigitalOutputs.O_RobotArmNotExtended_PM1:
                    SimulationData.O_RobotArmNotExtended_PM1 = value;
                    break;
                case DigitalOutputs.O_RobotArmNotExtended_PM2:
                    SimulationData.O_RobotArmNotExtended_PM2 = value;
                    break;
            }
        }

        protected virtual void InternalSimulateSetAnalogOutput(
            AnalogOutputs output,
            double value,
            Tempomat tempomat)
        {
            switch (output)
            {
                case AnalogOutputs.O_FFU_Speed:
                    SimulationData.O_FFU_Speed = RotationalSpeed.FromRevolutionsPerMinute(value);
                    break;
            }
        }

        protected virtual void InternalSimulateSetFfuSpeed(
            RotationalSpeed setPoint,
            Tempomat tempomat)
        {
            InternalSimulateSetAnalogOutput(AnalogOutputs.O_FFU_Speed, setPoint.Value, tempomat);
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            //Do nothing in case of EK9000
        }

        protected virtual void InternalSimulateSetLightColor(
            LightColors color,
            LightState mode,
            Tempomat tempomat)
        {
            switch (color)
            {
                case LightColors.Red:
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningRed,
                        mode == LightState.On,
                        tempomat);
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingRed,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow,
                        tempomat);
                    break;
                case LightColors.Blue:
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningBlue,
                        mode == LightState.On,
                        tempomat);
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingBlue,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow,
                        tempomat);
                    break;
                case LightColors.Yellow:
                case LightColors.Orange:
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningYellow,
                        mode == LightState.On,
                        tempomat);
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingYellow,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow,
                        tempomat);
                    break;
                case LightColors.Green:
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningGreen,
                        mode == LightState.On,
                        tempomat);
                    InternalSimulateSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingGreen,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow,
                        tempomat);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        protected virtual void InternalSimulateSetBuzzerState(BuzzerState state, Tempomat tempomat)
        {
            InternalSimulateSetDigitalOutput(
                DigitalOutputs.O_SignalTower_Buzzer1,
                state == BuzzerState.Fast || state == BuzzerState.On,
                tempomat);
            InternalSimulateSetDigitalOutput(
                DigitalOutputs.O_SignalTower_Buzzer2,
                state == BuzzerState.Slow,
                tempomat);
        }

        protected virtual void InternalSimulateSetReaderPosition(
            SampleDimension dimension,
            Tempomat tempomat)
        {
            if (dimension == SampleDimension.S200mm)
            {
                InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve1, false, tempomat);
                InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve2, true, tempomat);
            }
            else
            {
                InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve1, true, tempomat);
                InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve2, true, tempomat);
            }

            I_OCRTablePositionReach = false;
            Thread.Sleep(20);
            InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferTableDrive, true, tempomat);
            I_OCRTablePositionReach = true;
            Thread.Sleep(20);
            InternalSimulateSetDigitalOutput(DigitalOutputs.O_OCRWaferTableDrive, false, tempomat);
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new EK9000SimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
        }

        private void DisposeSimulatedMode()
        {
            if (SimulationData != null)
            {
                SimulationData.PropertyChanged -= SimulationData_PropertyChanged;
                SimulationData = null;
            }
        }

        #endregion

        #region Event Handlers

        private void SimulationData_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SimulationData.I_AirFlowPressureSensorIonizer):
                    I_AirFlowPressureSensorIonizer = SimulationData.I_AirFlowPressureSensorIonizer;
                    break;
                case nameof(SimulationData.I_CDA_PressureSensor):
                    I_CDA_PressureSensor = SimulationData.I_CDA_PressureSensor;
                    break;
                case nameof(SimulationData.I_DifferentialAirPressureSensor):
                    I_DifferentialAirPressureSensor =
                        SimulationData.I_DifferentialAirPressureSensor;
                    break;
                case nameof(SimulationData.I_EFEM_DoorStatus):
                    I_EFEM_DoorStatus = SimulationData.I_EFEM_DoorStatus;
                    if (I_EFEM_DoorStatus)
                    {
                        SetAlarm(_safetyDoorOpenAlarmKey);
                    }

                    break;
                case nameof(SimulationData.I_EMO_Status):
                    I_EMO_Status = SimulationData.I_EMO_Status;
                    break;
                case nameof(SimulationData.I_FFU_Alarm):
                    I_FFU_Alarm = SimulationData.I_FFU_Alarm;
                    break;
                case nameof(SimulationData.I_Ionizer1Status):
                    I_Ionizer1Status = SimulationData.I_Ionizer1Status;
                    break;
                case nameof(SimulationData.I_MaintenanceSwitch):
                    I_MaintenanceSwitch = SimulationData.I_MaintenanceSwitch;
                    break;
                case nameof(SimulationData.I_OCRWaferReaderLimitSensor1):
                    I_OCRWaferReaderLimitSensor1 = SimulationData.I_OCRWaferReaderLimitSensor1;
                    break;
                case nameof(SimulationData.I_OCRWaferReaderLimitSensor2):
                    I_OCRWaferReaderLimitSensor2 = SimulationData.I_OCRWaferReaderLimitSensor2;
                    break;
                case nameof(SimulationData.I_PM1_DoorOpened):
                    I_PM1_DoorOpened = SimulationData.I_PM1_DoorOpened;
                    break;
                case nameof(SimulationData.I_PM1_ReadyToLoadUnload):
                    I_PM1_ReadyToLoadUnload = SimulationData.I_PM1_ReadyToLoadUnload;
                    break;
                case nameof(SimulationData.I_PM2_DoorOpened):
                    I_PM2_DoorOpened = SimulationData.I_PM2_DoorOpened;
                    break;
                case nameof(SimulationData.I_PM2_ReadyToLoadUnload):
                    I_PM2_ReadyToLoadUnload = SimulationData.I_PM2_ReadyToLoadUnload;
                    break;
                case nameof(SimulationData.I_RV201Interlock):
                    I_RV201Interlock = SimulationData.I_RV201Interlock;
                    break;
                case nameof(SimulationData.I_RobotDriverPower):
                    I_RobotDriverPower = SimulationData.I_RobotDriverPower;
                    break;
                case nameof(SimulationData.I_ServiceLightLed):
                    I_ServiceLightLed = SimulationData.I_ServiceLightLed;
                    break;
                case nameof(SimulationData.I_TPMode):
                    I_TPMode = SimulationData.I_TPMode;
                    break;
                case nameof(SimulationData.I_VacuumPressureSensor):
                    I_VacuumPressureSensor = SimulationData.I_VacuumPressureSensor;
                    break;
                case nameof(SimulationData.O_FFU_Speed):
                    O_FFU_Speed = SimulationData.O_FFU_Speed;
                    break;
                case nameof(SimulationData.O_OCRWaferReaderValve1):
                    O_OCRWaferReaderValve1 = SimulationData.O_OCRWaferReaderValve1;
                    break;
                case nameof(SimulationData.O_OCRWaferReaderValve2):
                    O_OCRWaferReaderValve2 = SimulationData.O_OCRWaferReaderValve2;
                    break;
                case nameof(SimulationData.O_RobotArmNotExtended_PM1):
                    O_RobotArmNotExtended_PM1 = SimulationData.O_RobotArmNotExtended_PM1;
                    break;
                case nameof(SimulationData.O_RobotArmNotExtended_PM2):
                    O_RobotArmNotExtended_PM2 = SimulationData.O_RobotArmNotExtended_PM2;
                    break;
                case nameof(SimulationData.O_SignalTower_Buzzer1):
                    O_SignalTower_Buzzer1 = SimulationData.O_SignalTower_Buzzer1;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningBlue):
                    O_SignalTower_LightningBlue = SimulationData.O_SignalTower_LightningBlue;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningGreen):
                    O_SignalTower_LightningGreen = SimulationData.O_SignalTower_LightningGreen;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningRed):
                    O_SignalTower_LightningRed = SimulationData.O_SignalTower_LightningRed;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningYellow):
                    O_SignalTower_LightningYellow = SimulationData.O_SignalTower_LightningYellow;
                    break;
            }
        }

        #endregion
    }
}
