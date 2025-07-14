using UnitsNet;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Simulation
{
    public class EK9000SimulationData : SimulationData
    {
        #region Constructor

        public EK9000SimulationData(EK9000 ioModule)
            : base(ioModule)
        {

        }

        #endregion

        #region Properties

        #region Inputs

        private bool _emoStatus;
        public bool I_EMO_Status
        {
            get => _emoStatus;
            set
            {
                _emoStatus = value;
                OnPropertyChanged(nameof(I_EMO_Status));
            }
        }

        private bool _ffuAlarm;
        public bool I_FFU_Alarm
        {
            get => _ffuAlarm;
            set
            {
                _ffuAlarm = value;
                OnPropertyChanged(nameof(I_FFU_Alarm));
            }
        }

        private bool _vacuumPressureSensor;
        public bool I_VacuumPressureSensor
        {
            get => _vacuumPressureSensor;
            set
            {
                _vacuumPressureSensor = value;
                OnPropertyChanged(nameof(I_VacuumPressureSensor));
            }
        }

        private bool _cdaPressureSensor;
        public bool I_CDA_PressureSensor
        {
            get => _cdaPressureSensor;
            set
            {
                _cdaPressureSensor = value;
                OnPropertyChanged(nameof(I_CDA_PressureSensor));
            }
        }

        private bool _serviceLightLed;
        public bool I_ServiceLightLed
        {
            get => _serviceLightLed;
            set
            {
                _serviceLightLed = value;
                OnPropertyChanged(nameof(I_ServiceLightLed));
            }
        }

        private bool _airFlowPressureSensorIonizer;
        public bool I_AirFlowPressureSensorIonizer
        {
            get => _airFlowPressureSensorIonizer;
            set
            {
                _airFlowPressureSensorIonizer = value;
                OnPropertyChanged(nameof(I_AirFlowPressureSensorIonizer));
            }
        }

        private bool _ionizer1Status;
        public bool I_Ionizer1Status
        {
            get => _ionizer1Status;
            set
            {
                _ionizer1Status = value;
                OnPropertyChanged(nameof(I_Ionizer1Status));
            }
        }

        private bool _rV201Interlock;
        public bool I_RV201Interlock
        {
            get => _rV201Interlock;
            set
            {
                _rV201Interlock = value;
                OnPropertyChanged(nameof(I_RV201Interlock));
            }
        }

        private bool _maintenanceSwitch;
        public bool I_MaintenanceSwitch
        {
            get => _maintenanceSwitch;
            set
            {
                _maintenanceSwitch = value;
                OnPropertyChanged(nameof(I_MaintenanceSwitch));
            }
        }

        private bool _robotDriverPower;
        public bool I_RobotDriverPower
        {
            get => _robotDriverPower;
            set
            {
                _robotDriverPower = value;
                OnPropertyChanged(nameof(I_RobotDriverPower));
            }
        }

        private bool _efemDoorStatus;
        public bool I_EFEM_DoorStatus
        {
            get => _efemDoorStatus;
            set
            {
                _efemDoorStatus = value;
                OnPropertyChanged(nameof(I_EFEM_DoorStatus));
            }
        }

        private bool _tpMode;
        public bool I_TPMode
        {
            get => _tpMode;
            set
            {
                _tpMode = value;
                OnPropertyChanged(nameof(I_TPMode));
            }
        }

        private bool _ocrWaferReaderLimitSensor1;
        public bool I_OCRWaferReaderLimitSensor1
        {
            get => _ocrWaferReaderLimitSensor1;
            set
            {
                _ocrWaferReaderLimitSensor1 = value;
                OnPropertyChanged(nameof(I_OCRWaferReaderLimitSensor1));
            }
        }

        private bool _ocrWaferReaderLimitSensor2;
        public bool I_OCRWaferReaderLimitSensor2
        {
            get => _ocrWaferReaderLimitSensor2;
            set
            {
                _ocrWaferReaderLimitSensor2 = value;
                OnPropertyChanged(nameof(I_OCRWaferReaderLimitSensor2));
            }
        }

        private Pressure _differentialAirPressureSensor;
        public Pressure I_DifferentialAirPressureSensor
        {
            get => _differentialAirPressureSensor;
            set
            {
                _differentialAirPressureSensor = value;
                OnPropertyChanged(nameof(I_DifferentialAirPressureSensor));
            }
        }

        private bool _pm1DoorOpened;
        public bool I_PM1_DoorOpened
        {
            get => _pm1DoorOpened;
            set
            {
                _pm1DoorOpened = value;
                OnPropertyChanged(nameof(I_PM1_DoorOpened));
            }
        }

        private bool _pm1ReadyToLoadUnload;
        public bool I_PM1_ReadyToLoadUnload
        {
            get => _pm1ReadyToLoadUnload;
            set
            {
                _pm1ReadyToLoadUnload = value;
                OnPropertyChanged(nameof(I_PM1_ReadyToLoadUnload));
            }
        }

        private bool _pm2DoorOpened;
        public bool I_PM2_DoorOpened
        {
            get => _pm2DoorOpened;
            set
            {
                _pm2DoorOpened = value;
                OnPropertyChanged(nameof(I_PM2_DoorOpened));
            }
        }

        private bool _pm2ReadyToLoadUnload;
        public bool I_PM2_ReadyToLoadUnload
        {
            get => _pm2ReadyToLoadUnload;
            set
            {
                _pm2ReadyToLoadUnload = value;
                OnPropertyChanged(nameof(I_PM2_ReadyToLoadUnload));
            }
        }

        #endregion

        #region Outputs

        private bool _signalTowerLightningRed;

        public bool O_SignalTower_LightningRed
        {
            get => _signalTowerLightningRed;
            set
            {
                _signalTowerLightningRed = value;
                OnPropertyChanged(nameof(O_SignalTower_LightningRed));
            }
        }

        private bool _signalTowerLightningYellow;
        public bool O_SignalTower_LightningYellow
        {
            get => _signalTowerLightningYellow;
            set
            {
                _signalTowerLightningYellow = value;
                OnPropertyChanged(nameof(O_SignalTower_LightningYellow));
            }
        }

        private bool _signalTowerLightningGreen;
        public bool O_SignalTower_LightningGreen
        {
            get => _signalTowerLightningGreen;
            set
            {
                _signalTowerLightningGreen = value;
                OnPropertyChanged(nameof(O_SignalTower_LightningGreen));
            }
        }

        private bool _signalTowerLightningBlue;
        public bool O_SignalTower_LightningBlue
        {
            get => _signalTowerLightningBlue;
            set
            {
                _signalTowerLightningBlue = value;
                OnPropertyChanged(nameof(O_SignalTower_LightningBlue));
            }
        }

        private bool _signalTowerBuzzer1;
        public bool O_SignalTower_Buzzer1
        {
            get => _signalTowerBuzzer1;
            set
            {
                _signalTowerBuzzer1 = value;
                OnPropertyChanged(nameof(O_SignalTower_Buzzer1));
            }
        }

        private bool _ocrWaferReaderValve1;
        public bool O_OCRWaferReaderValve1
        {
            get => _ocrWaferReaderValve1;
            set
            {
                _ocrWaferReaderValve1 = value;
                OnPropertyChanged(nameof(O_OCRWaferReaderValve1));
            }
        }

        private bool _ocrWaferReaderValve2;
        public bool O_OCRWaferReaderValve2
        {
            get => _ocrWaferReaderValve2;
            set
            {
                _ocrWaferReaderValve2 = value;
                OnPropertyChanged(nameof(O_OCRWaferReaderValve2));
            }
        }

        private RotationalSpeed _ffuSpeed;
        public RotationalSpeed O_FFU_Speed
        {
            get => _ffuSpeed;
            set
            {
                _ffuSpeed = value;
                OnPropertyChanged(nameof(O_FFU_Speed));
            }
        }

        private bool _robotArmNotExtendedPm1;
        public bool O_RobotArmNotExtended_PM1
        {
            get => _robotArmNotExtendedPm1;
            set
            {
                _robotArmNotExtendedPm1 = value;
                OnPropertyChanged(nameof(O_RobotArmNotExtended_PM1));
            }
        }

        private bool _robotArmNotExtendedPm2;
        public bool O_RobotArmNotExtended_PM2
        {
            get => _robotArmNotExtendedPm2;
            set
            {
                _robotArmNotExtendedPm2 = value;
                OnPropertyChanged(nameof(O_RobotArmNotExtended_PM2));
            }
        }

        #endregion Outputs

        #endregion
    }
}
