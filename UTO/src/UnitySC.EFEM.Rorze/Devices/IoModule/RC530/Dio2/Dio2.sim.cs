using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2
{
    public partial class Dio2 : ISimDevice
    {
        #region Properties

        protected internal Dio2SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new Dio2SimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected override void InternalSimulateSetOutputSignal(
            SignalData signalData,
            Tempomat tempomat)
        {
            if (signalData is not Dio2SignalData dio2SignalData)
            {
                return;
            }

            SimulationData.O_RobotArmNotExtended_PM1 =
                dio2SignalData.RobotArmNotExtended_PM1 != null
                && (bool)dio2SignalData.RobotArmNotExtended_PM1;
            SimulationData.O_RobotArmNotExtended_PM2 =
                dio2SignalData.RobotArmNotExtended_PM2 != null
                && (bool)dio2SignalData.RobotArmNotExtended_PM2;
            SimulationData.O_RobotArmNotExtended_PM3 =
                dio2SignalData.RobotArmNotExtended_PM3 != null
                && (bool)dio2SignalData.RobotArmNotExtended_PM3;
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new Dio2SimulationData(this);
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
            }
        }

        #endregion
    }
}
