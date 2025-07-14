using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1.Simulation
{
    public class Dio1SimulationData : SimulationData
    {
        #region Constructor

        public Dio1SimulationData(Dio1 dio1)
            : base(dio1)
        {

        }

        #endregion

        #region Properties

        #region Inputs

        private bool _pressureSensorVac;
        public bool I_PressureSensor_VAC
        {
            get => _pressureSensorVac;
            set
                => SetAndRaiseIfChanged(
                    ref _pressureSensorVac,
                    value,
                    nameof(I_PressureSensor_VAC));
        }

        private bool _pressureSensorAir;
        public bool I_PressureSensor_AIR
        {
            get => _pressureSensorAir;
            set
                => SetAndRaiseIfChanged(
                    ref _pressureSensorAir,
                    value,
                    nameof(I_PressureSensor_AIR));
        }

        private bool _ledPushButton;
        public bool I_Led_PushButton
        {
            get => _ledPushButton;
            set
                => SetAndRaiseIfChanged(
                    ref _ledPushButton,
                    value,
                    nameof(I_Led_PushButton));
        }

        private bool _pressureSensorIonAir;
        public bool I_PressureSensor_ION_AIR
        {
            get => _pressureSensorIonAir;
            set
                => SetAndRaiseIfChanged(
                    ref _pressureSensorIonAir,
                    value,
                    nameof(I_PressureSensor_ION_AIR));
        }

        private bool _ionizer1Alarm;
        public bool I_Ionizer1Alarm
        {
            get => _ionizer1Alarm;
            set
                => SetAndRaiseIfChanged(
                    ref _ionizer1Alarm,
                    value,
                    nameof(I_Ionizer1Alarm));
        }

        private bool _rv201Interlock;
        public bool I_RV201Interlock
        {
            get => _rv201Interlock;
            set
                => SetAndRaiseIfChanged(
                    ref _rv201Interlock,
                    value,
                    nameof(I_RV201Interlock));
        }

        private bool _maintenanceSwitch;
        public bool I_MaintenanceSwitch
        {
            get => _maintenanceSwitch;
            set
                => SetAndRaiseIfChanged(
                    ref _maintenanceSwitch,
                    value,
                    nameof(I_MaintenanceSwitch));
        }

        private bool _driverPower;
        public bool I_DriverPower
        {
            get => _driverPower;
            set
                => SetAndRaiseIfChanged(
                    ref _driverPower,
                    value,
                    nameof(I_DriverPower));
        }

        private bool _doorStatus;
        public bool I_DoorStatus
        {
            get => _doorStatus;
            set
                => SetAndRaiseIfChanged(
                    ref _doorStatus,
                    value,
                    nameof(I_DoorStatus));
        }

        private bool _tpMode;
        public bool I_TPMode
        {
            get => _tpMode;
            set
                => SetAndRaiseIfChanged(
                    ref _tpMode,
                    value,
                    nameof(I_TPMode));
        }

        private bool _ocrWaferReaderLimitSensor1;
        public bool I_OCRWaferReaderLimitSensor1
        {
            get => _ocrWaferReaderLimitSensor1;
            set
                => SetAndRaiseIfChanged(
                    ref _ocrWaferReaderLimitSensor1,
                    value,
                    nameof(I_OCRWaferReaderLimitSensor1));
        }

        private bool _ocrWaferReaderLimitSensor2;
        public bool I_OCRWaferReaderLimitSensor2
        {
            get => _ocrWaferReaderLimitSensor2;
            set
                => SetAndRaiseIfChanged(
                    ref _ocrWaferReaderLimitSensor2,
                    value,
                    nameof(I_OCRWaferReaderLimitSensor2));
        }

        private bool _lightCurtain;
        public bool I_LightCurtain
        {
            get => _lightCurtain;
            set => SetAndRaiseIfChanged(ref _lightCurtain, value);
        }

        #endregion Inputs

        #region Outputs

        private bool _signalTowerLightningRed;
        public bool O_SignalTower_LightningRed
        {
            get => _signalTowerLightningRed;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningRed,
                    value,
                    nameof(O_SignalTower_LightningRed));
        }

        private bool _signalTowerLightningYellow;
        public bool O_SignalTower_LightningYellow
        {
            get => _signalTowerLightningYellow;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningYellow,
                    value,
                    nameof(O_SignalTower_LightningYellow));
        }

        private bool _signalTowerLightningGreen;
        public bool O_SignalTower_LightningGreen
        {
            get => _signalTowerLightningGreen;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningGreen,
                    value,
                    nameof(O_SignalTower_LightningGreen));
        }

        private bool _signalTowerLightningBlue;
        public bool O_SignalTower_LightningBlue
        {
            get => _signalTowerLightningBlue;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerLightningBlue,
                    value,
                    nameof(O_SignalTower_LightningBlue));
        }

        private bool _signalTowerBlinkingRed;
        public bool O_SignalTower_BlinkingRed
        {
            get => _signalTowerBlinkingRed;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingRed,
                    value,
                    nameof(O_SignalTower_BlinkingRed));
        }

        private bool _signalTowerBlinkingYellow;
        public bool O_SignalTower_BlinkingYellow
        {
            get => _signalTowerBlinkingYellow;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingYellow,
                    value,
                    nameof(O_SignalTower_BlinkingYellow));
        }

        private bool _signalTowerBlinkingGreen;
        public bool O_SignalTower_BlinkingGreen
        {
            get => _signalTowerBlinkingGreen;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingGreen,
                    value,
                    nameof(O_SignalTower_BlinkingGreen));
        }

        private bool _signalTowerBlinkingBlue;
        public bool O_SignalTower_BlinkingBlue
        {
            get => _signalTowerBlinkingBlue;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBlinkingBlue,
                    value,
                    nameof(O_SignalTower_BlinkingBlue));
        }

        private bool _signalTowerBuzzer1;
        public bool O_SignalTower_Buzzer1
        {
            get => _signalTowerBuzzer1;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBuzzer1,
                    value,
                    nameof(O_SignalTower_Buzzer1));
        }

        private bool _signalTowerBuzzer2;
        public bool O_SignalTower_Buzzer2
        {
            get => _signalTowerBuzzer2;
            set
                => SetAndRaiseIfChanged(
                    ref _signalTowerBuzzer2,
                    value,
                    nameof(O_SignalTower_Buzzer2));
        }

        private bool _ocrWaferReaderValve1;
        public bool O_OCRWaferReaderValve1
        {
            get => _ocrWaferReaderValve1;
            set
                => SetAndRaiseIfChanged(
                    ref _ocrWaferReaderValve1,
                    value,
                    nameof(O_OCRWaferReaderValve1));
        }

        private bool _ocrWaferReaderValve2;
        public bool O_OCRWaferReaderValve2
        {
            get => _ocrWaferReaderValve2;
            set
                => SetAndRaiseIfChanged(
                    ref _ocrWaferReaderValve2,
                    value,
                    nameof(O_OCRWaferReaderValve2));
        }

        #endregion Outputs

        #endregion
    }
}
