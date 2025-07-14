using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Simulation;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem
{
    public partial class Dio1MediumSizeEfem : ISimDevice
    {
        #region Properties

        protected internal Dio1MediumSizeEfemSimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new Dio1MediumSizeEfemSimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands
        protected override void InternalSimulateSetOutputSignal(SignalData signalData, Tempomat tempomat)
        {
            if (signalData is not Dio1MediumSizeEfemSignalData dio1SignalData)
            {
                return;
            }

            SimulationData.O_RobotArmNotExtended_PM1 = dio1SignalData.RobotArmNotExtended_PM1 ?? SimulationData.O_RobotArmNotExtended_PM1;
            SimulationData.O_RobotArmNotExtended_PM2 = dio1SignalData.RobotArmNotExtended_PM2 ?? SimulationData.O_RobotArmNotExtended_PM2;
            SimulationData.O_RobotArmNotExtended_PM3 = dio1SignalData.RobotArmNotExtended_PM3 ?? SimulationData.O_RobotArmNotExtended_PM3;
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
            SimulationData.O_SignalTower_PowerSupply = dio1SignalData.SignalTowerPowerSupply ?? SimulationData.O_SignalTower_PowerSupply;
        }

        protected virtual void InternalSimulateSetLightColor(
            LightColors color,
            LightState mode,
            Tempomat tempomat)
        {
            // Update only the needed bits (outputs are null by default and a mask is used to only set non-null outputs)
            var outputSignal = new Dio1MediumSizeEfemSignalData() { SignalTowerPowerSupply = true };
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
            var outputSignal = new Dio1MediumSizeEfemSignalData
            {
                SignalTowerPowerSupply = true,
                SignalTower_Buzzer1 = state == BuzzerState.Slow,
                SignalTower_Buzzer2 = state == BuzzerState.Fast
            };
            InternalSimulateSetOutputSignal(outputSignal, tempomat);
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new Dio1MediumSizeEfemSimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
            SimulationData.I_PressureSensor_VAC = true;
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
                case nameof(SimulationData.I_MaintenanceSwitch):
                    I_MaintenanceSwitch = SimulationData.I_MaintenanceSwitch;
                    break;
                case nameof(SimulationData.I_PressureSensor_VAC):
                    I_PressureSensor_VAC = SimulationData.I_PressureSensor_VAC;
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
                case nameof(SimulationData.I_LightCurtain):
                    I_LightCurtain = SimulationData.I_LightCurtain;
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
                case nameof(SimulationData.I_PM3_DoorOpened):
                    I_PM3_DoorOpened = SimulationData.I_PM3_DoorOpened;
                    break;
                case nameof(SimulationData.I_PM3_ReadyToLoadUnload):
                    I_PM3_ReadyToLoadUnload = SimulationData.I_PM3_ReadyToLoadUnload;
                    break;
                case nameof(SimulationData.O_RobotArmNotExtended_PM1):
                    O_RobotArmNotExtended_PM1 = SimulationData.O_RobotArmNotExtended_PM1;
                    break;
                case nameof(SimulationData.O_RobotArmNotExtended_PM2):
                    O_RobotArmNotExtended_PM2 = SimulationData.O_RobotArmNotExtended_PM2;
                    break;
                case nameof(SimulationData.O_RobotArmNotExtended_PM3):
                    O_RobotArmNotExtended_PM3 = SimulationData.O_RobotArmNotExtended_PM3;
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
                case nameof(SimulationData.O_SignalTower_PowerSupply):
                    O_SignalTower_PowerSupply = SimulationData.O_SignalTower_PowerSupply;
                    break;
            }
        }

        #endregion
    }
}
