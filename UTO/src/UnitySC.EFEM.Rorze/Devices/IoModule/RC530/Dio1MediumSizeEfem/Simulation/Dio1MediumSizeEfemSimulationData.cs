using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Simulation
{
    public class Dio1MediumSizeEfemSimulationData : SimulationData
    {
        #region Constructor

        public Dio1MediumSizeEfemSimulationData(Dio1MediumSizeEfem dio1)
            : base(dio1)
        {

        }

        #endregion

        #region Properties

        #region Inputs

        private bool _maintenanceSwitch;
        public bool I_MaintenanceSwitch
        {
            get => _maintenanceSwitch;
            set
                => SetAndRaiseIfChanged(
                    ref _maintenanceSwitch,
                    value);
        }

        private bool _pressureSensorVac;
        public bool I_PressureSensor_VAC
        {
            get => _pressureSensorVac;
            set
                => SetAndRaiseIfChanged(
                    ref _pressureSensorVac,
                    value);
        }

        private bool _ledPushButton;
        public bool I_Led_PushButton
        {
            get => _ledPushButton;
            set
                => SetAndRaiseIfChanged(
                    ref _ledPushButton,
                    value);
        }

        private bool _pressureSensorIonAir;
        public bool I_PressureSensor_ION_AIR
        {
            get => _pressureSensorIonAir;
            set
                => SetAndRaiseIfChanged(
                    ref _pressureSensorIonAir,
                    value);
        }

        private bool _ionizer1Alarm;
        public bool I_Ionizer1Alarm
        {
            get => _ionizer1Alarm;
            set
                => SetAndRaiseIfChanged(
                    ref _ionizer1Alarm,
                    value);
        }

        private bool _lightCurtain;
        public bool I_LightCurtain
        {
            get => _lightCurtain;
            set => SetAndRaiseIfChanged(ref _lightCurtain, value);
        }


        private bool _pm1_DoorOpened;
        public bool I_PM1_DoorOpened
        {
            get => _pm1_DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm1_DoorOpened, value);
        }

        private bool _pm1_ReadyToLoadUnload;
        public bool I_PM1_ReadyToLoadUnload
        {
            get => _pm1_ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm1_ReadyToLoadUnload, value);
        }

        private bool _pm2_DoorOpened;
        public bool I_PM2_DoorOpened
        {
            get => _pm2_DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm2_DoorOpened, value);
        }

        private bool _pm2_ReadyToLoadUnload;
        public bool I_PM2_ReadyToLoadUnload
        {
            get => _pm2_ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm2_ReadyToLoadUnload, value);
        }

        private bool _pm3_DoorOpened;
        public bool I_PM3_DoorOpened
        {
            get => _pm3_DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm3_DoorOpened, value);
        }

        private bool _pm3_ReadyToLoadUnload;
        public bool I_PM3_ReadyToLoadUnload
        {
            get => _pm3_ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm3_ReadyToLoadUnload, value);
        }
        #endregion Inputs

        #region Outputs

        private bool _robotArmNotExtended_PM1;
        public bool O_RobotArmNotExtended_PM1
        {
            get => _robotArmNotExtended_PM1;
            set
                => SetAndRaiseIfChanged(
                    ref _robotArmNotExtended_PM1,
                    value);
        }

        private bool _robotArmNotExtended_PM2;
        public bool O_RobotArmNotExtended_PM2
        {
            get => _robotArmNotExtended_PM2;
            set
                => SetAndRaiseIfChanged(
                    ref _robotArmNotExtended_PM2,
                    value);
        }

        private bool _robotArmNotExtended_PM3;
        public bool O_RobotArmNotExtended_PM3
        {
            get => _robotArmNotExtended_PM3;
            set
                => SetAndRaiseIfChanged(
                    ref _robotArmNotExtended_PM3,
                    value);
        }

        private bool _signalTowerLightningRed;
        public bool O_SignalTower_LightningRed
        {
            get => _signalTowerLightningRed;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningRed,
                    value);
        }

        private bool _signalTowerLightningYellow;
        public bool O_SignalTower_LightningYellow
        {
            get => _signalTowerLightningYellow;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningYellow,
                    value);
        }

        private bool _signalTowerLightningGreen;
        public bool O_SignalTower_LightningGreen
        {
            get => _signalTowerLightningGreen;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningGreen,
                    value);
        }

        private bool _signalTowerLightningBlue;
        public bool O_SignalTower_LightningBlue
        {
            get => _signalTowerLightningBlue;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningBlue,
                    value);
        }

        private bool _signalTowerBlinkingRed;
        public bool O_SignalTower_BlinkingRed
        {
            get => _signalTowerBlinkingRed;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingRed,
                    value);
        }

        private bool _signalTowerBlinkingYellow;
        public bool O_SignalTower_BlinkingYellow
        {
            get => _signalTowerBlinkingYellow;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingYellow,
                    value);
        }

        private bool _signalTowerBlinkingGreen;
        public bool O_SignalTower_BlinkingGreen
        {
            get => _signalTowerBlinkingGreen;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingGreen,
                    value);
        }

        private bool _signalTowerBlinkingBlue;
        public bool O_SignalTower_BlinkingBlue
        {
            get => _signalTowerBlinkingBlue;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingBlue,
                    value);
        }

        private bool _signalTowerBuzzer1;
        public bool O_SignalTower_Buzzer1
        {
            get => _signalTowerBuzzer1;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBuzzer1,
                    value);
        }

        private bool _signalTowerBuzzer2;
        public bool O_SignalTower_Buzzer2
        {
            get => _signalTowerBuzzer2;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBuzzer2,
                    value);
        }

        private bool _signalTowerPowerSupply;
        public bool O_SignalTower_PowerSupply
        {
            get => _signalTowerPowerSupply;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerPowerSupply,
                    value);
        }
        #endregion Outputs

        #endregion
    }
}
