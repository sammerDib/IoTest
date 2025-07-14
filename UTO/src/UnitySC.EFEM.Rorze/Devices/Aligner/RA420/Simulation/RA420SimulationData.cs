using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Simulation
{
    public class RA420SimulationData : SimulationData
    {
        #region Constructor

        public RA420SimulationData(RA420 aligner)
            : base(aligner)
        {

        }

        #endregion

        #region Properties

        #region Inputs

        private bool _exhaustFanRotating;
        public bool I_ExhaustFanRotating
        {
            get => _exhaustFanRotating;
            set
            {
                _exhaustFanRotating = value;
                OnPropertyChanged(nameof(I_ExhaustFanRotating));
            }
        }

        private bool _substrateDetectionSensor1;

        public bool I_SubstrateDetectionSensor1
        {
            get => _substrateDetectionSensor1;
            set
            {
                _substrateDetectionSensor1 = value;
                OnPropertyChanged(nameof(I_SubstrateDetectionSensor1));
            }
        }

        private bool _substrateDetectionSensor2;

        public bool I_SubstrateDetectionSensor2
        {
            get => _substrateDetectionSensor2;
            set
            {
                _substrateDetectionSensor2 = value;
                OnPropertyChanged(nameof(I_SubstrateDetectionSensor2));
            }
        }

        #endregion

        #region Outputs

        private bool _alignerReadyToOperate;

        public bool O_AlignerReadyToOperate
        {
            get => _alignerReadyToOperate;
            set
            {
                _alignerReadyToOperate = value;
                OnPropertyChanged(nameof(O_AlignerReadyToOperate));
            }
        }

        private bool _temporarilyStop;
        public bool O_TemporarilyStop
        {
            get => _temporarilyStop;
            set
            {
                _temporarilyStop = value;
                OnPropertyChanged(nameof(O_TemporarilyStop));
            }
        }

        private bool _significantError;
        public bool O_SignificantError
        {
            get => _significantError;
            set
            {
                _significantError = value;
                OnPropertyChanged(nameof(O_SignificantError));
            }
        }

        private bool _lightError;
        public bool O_LightError
        {
            get => _lightError;
            set
            {
                _lightError = value;
                OnPropertyChanged(nameof(O_LightError));
            }
        }

        private bool _substrateDetection;
        public bool O_SubstrateDetection
        {
            get => _substrateDetection;
            set
            {
                _substrateDetection = value;
                OnPropertyChanged(nameof(O_SubstrateDetection));
            }
        }

        private bool _alignmentComplete;
        public bool O_AlignmentComplete
        {
            get => _alignmentComplete;
            set
            {
                _alignmentComplete = value;
                OnPropertyChanged(nameof(O_AlignmentComplete));
            }
        }

        private bool _spindleSolenoidValveChuckingOFF;
        public bool O_SpindleSolenoidValveChuckingOFF
        {
            get => _spindleSolenoidValveChuckingOFF;
            set
            {
                _spindleSolenoidValveChuckingOFF = value;
                OnPropertyChanged(nameof(O_SpindleSolenoidValveChuckingOFF));
            }
        }

        private bool _spindleSolenoidValveChuckingON;
        public bool O_SpindleSolenoidValveChuckingON
        {
            get => _spindleSolenoidValveChuckingON;
            set
            {
                _spindleSolenoidValveChuckingON = value;
                OnPropertyChanged(nameof(O_SpindleSolenoidValveChuckingON));
            }
        }

        #endregion

        #endregion
    }
}
