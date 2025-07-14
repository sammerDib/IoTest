using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1.Simulation;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1
{
    public partial class Dio1 : ISimDevice
    {
        #region Properties

        protected internal Dio1SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new Dio1SimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected override void InternalSimulateSetOutputSignal(SignalData signalData, Tempomat tempomat)
        {
            if (signalData is not Dio1SignalData dio1SignalData)
            {
                return;
            }

            SimulationData.O_SignalTower_LightningRed = dio1SignalData.SignalTower_LightningRed ?? SimulationData.O_SignalTower_LightningRed;
            SimulationData.O_SignalTower_LightningYellow = dio1SignalData.SignalTower_LightningYellow ?? SimulationData.O_SignalTower_LightningYellow;
            SimulationData.O_SignalTower_LightningGreen = dio1SignalData.SignalTower_LightningGreen ?? SimulationData.O_SignalTower_LightningGreen;
            SimulationData.O_SignalTower_LightningBlue = dio1SignalData.SignalTower_LightningBlue ?? SimulationData.O_SignalTower_LightningBlue;
            SimulationData.O_SignalTower_BlinkingRed = dio1SignalData.SignalTower_BlinkingRed ?? SimulationData.O_SignalTower_BlinkingRed;
            SimulationData.O_SignalTower_BlinkingYellow = dio1SignalData.SignalTower_BlinkingYellow ?? SimulationData.O_SignalTower_BlinkingYellow;
            SimulationData.O_SignalTower_BlinkingGreen = dio1SignalData.SignalTower_BlinkingGreen ?? SimulationData.O_SignalTower_BlinkingGreen;
            SimulationData.O_SignalTower_BlinkingBlue = dio1SignalData.SignalTower_BlinkingBlue ?? SimulationData.O_SignalTower_BlinkingBlue;
            SimulationData.O_SignalTower_Buzzer1 = dio1SignalData.SignalTower_Buzzer1 ?? SimulationData.O_SignalTower_Buzzer1;
            SimulationData.O_SignalTower_Buzzer2 = dio1SignalData.SignalTower_Buzzer2 ?? SimulationData.O_SignalTower_Buzzer2;
            SimulationData.O_OCRWaferReaderValve1 = dio1SignalData.OCRWaferReaderValve1 ?? SimulationData.O_OCRWaferReaderValve1;
            SimulationData.O_OCRWaferReaderValve2 = dio1SignalData.OCRWaferReaderValve2 ?? SimulationData.O_OCRWaferReaderValve2;
        }

        protected virtual void InternalSimulateSetLightColor(
            LightColors color,
            LightState mode,
            Tempomat tempomat)
        {
            // Update only the needed bits (outputs are null by default and a mask is used to only set non-null outputs)
            var outputSignal = new Dio1SignalData();
            switch (color)
            {
                case LightColors.Red:
                    outputSignal.SignalTower_LightningRed = mode == LightState.On;
                    outputSignal.SignalTower_BlinkingRed = mode == LightState.Flashing
                                                           || mode == LightState.FlashingFast
                                                           || mode == LightState.FlashingSlow;
                    break;
                case LightColors.Blue:
                    outputSignal.SignalTower_LightningBlue = mode == LightState.On;
                    outputSignal.SignalTower_BlinkingBlue = mode == LightState.Flashing
                                                            || mode == LightState.FlashingFast
                                                            || mode == LightState.FlashingSlow;
                    break;
                case LightColors.Yellow:
                case LightColors.Orange:
                    outputSignal.SignalTower_LightningYellow = mode == LightState.On;
                    outputSignal.SignalTower_BlinkingYellow = mode == LightState.Flashing
                                                              || mode == LightState.FlashingFast
                                                              || mode == LightState.FlashingSlow;
                    break;
                case LightColors.Green:
                    outputSignal.SignalTower_LightningGreen = mode == LightState.On;
                    outputSignal.SignalTower_BlinkingGreen = mode == LightState.Flashing
                                                             || mode == LightState.FlashingFast
                                                             || mode == LightState.FlashingSlow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            InternalSimulateSetOutputSignal(outputSignal, tempomat);
        }

        protected virtual void InternalSimulateSetBuzzerState(BuzzerState state, Tempomat tempomat)
        {
            // Update only the needed bits (outputs are null by default and a mask is used to only set non-null outputs)
            var outputSignal = new Dio1SignalData
            {
                SignalTower_Buzzer1 = state == BuzzerState.Slow,
                SignalTower_Buzzer2 = state == BuzzerState.Fast
            };
            InternalSimulateSetOutputSignal(outputSignal, tempomat);
        }

        protected virtual void InternalSimulateSetReaderPosition(
            SampleDimension dimension,
            Tempomat tempomat)
        {
            var outputSignal = new Dio1SignalData();
            if (dimension == SampleDimension.S200mm)
            {
                outputSignal.OCRWaferReaderValve1 = false;
                outputSignal.OCRWaferReaderValve2 = true;
            }
            else
            {
                outputSignal.OCRWaferReaderValve1 = true;
                outputSignal.OCRWaferReaderValve2 = false;
            }

            InternalSimulateSetOutputSignal(outputSignal, tempomat);
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new Dio1SimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
            SimulationData.I_PressureSensor_VAC = true;
            SimulationData.I_PressureSensor_AIR = true;
            SimulationData.I_DoorStatus = true;
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
                case nameof(SimulationData.I_PressureSensor_VAC):
                    I_PressureSensor_VAC = SimulationData.I_PressureSensor_VAC;
                    break;
                case nameof(SimulationData.I_PressureSensor_AIR):
                    I_PressureSensor_AIR = SimulationData.I_PressureSensor_AIR;
                    break;
                case nameof(SimulationData.I_Led_PushButton):
                    I_Led_PushButton = SimulationData.I_Led_PushButton;
                    break;
                case nameof(SimulationData.I_PressureSensor_ION_AIR):
                    I_PressureSensor_ION_AIR = SimulationData.I_PressureSensor_ION_AIR;
                    break;
                case nameof(SimulationData.I_Ionizer1Alarm):
                    I_Ionizer1Alarm = SimulationData.I_Ionizer1Alarm;
                    break;
                case nameof(SimulationData.I_RV201Interlock):
                    I_RV201Interlock = SimulationData.I_RV201Interlock;
                    break;
                case nameof(SimulationData.I_MaintenanceSwitch):
                    I_MaintenanceSwitch = SimulationData.I_MaintenanceSwitch;
                    break;
                case nameof(SimulationData.I_DriverPower):
                    I_DriverPower = SimulationData.I_DriverPower;
                    break;
                case nameof(SimulationData.I_DoorStatus):
                    I_DoorStatus = SimulationData.I_DoorStatus;
                    break;
                case nameof(SimulationData.I_TPMode):
                    I_TPMode = SimulationData.I_TPMode;
                    break;
                case nameof(SimulationData.I_OCRWaferReaderLimitSensor1):
                    I_OCRWaferReaderLimitSensor1 = SimulationData.I_OCRWaferReaderLimitSensor1;
                    break;
                case nameof(SimulationData.I_OCRWaferReaderLimitSensor2):
                    I_OCRWaferReaderLimitSensor2 = SimulationData.I_OCRWaferReaderLimitSensor2;
                    break;
                case nameof(SimulationData.I_LightCurtain):
                    I_LightCurtain = SimulationData.I_LightCurtain;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningRed):
                    O_SignalTower_LightningRed = SimulationData.O_SignalTower_LightningRed;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningYellow):
                    O_SignalTower_LightningYellow = SimulationData.O_SignalTower_LightningYellow;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningGreen):
                    O_SignalTower_LightningGreen = SimulationData.O_SignalTower_LightningGreen;
                    break;
                case nameof(SimulationData.O_SignalTower_LightningBlue):
                    O_SignalTower_LightningBlue = SimulationData.O_SignalTower_LightningBlue;
                    break;
                case nameof(SimulationData.O_SignalTower_BlinkingRed):
                    O_SignalTower_BlinkingRed = SimulationData.O_SignalTower_BlinkingRed;
                    break;
                case nameof(SimulationData.O_SignalTower_BlinkingYellow):
                    O_SignalTower_BlinkingYellow = SimulationData.O_SignalTower_BlinkingYellow;
                    break;
                case nameof(SimulationData.O_SignalTower_BlinkingGreen):
                    O_SignalTower_BlinkingGreen = SimulationData.O_SignalTower_BlinkingGreen;
                    break;
                case nameof(SimulationData.O_SignalTower_BlinkingBlue):
                    O_SignalTower_BlinkingBlue = SimulationData.O_SignalTower_BlinkingBlue;
                    break;
                case nameof(SimulationData.O_SignalTower_Buzzer1):
                    O_SignalTower_Buzzer1 = SimulationData.O_SignalTower_Buzzer1;
                    break;
                case nameof(SimulationData.O_SignalTower_Buzzer2):
                    O_SignalTower_Buzzer2 = SimulationData.O_SignalTower_Buzzer2;
                    break;
                case nameof(SimulationData.O_OCRWaferReaderValve1):
                    O_OCRWaferReaderValve1 = SimulationData.O_OCRWaferReaderValve1;
                    break;
                case nameof(SimulationData.O_OCRWaferReaderValve2):
                    O_OCRWaferReaderValve2 = SimulationData.O_OCRWaferReaderValve2;
                    break;
            }
        }

        #endregion
    }
}
