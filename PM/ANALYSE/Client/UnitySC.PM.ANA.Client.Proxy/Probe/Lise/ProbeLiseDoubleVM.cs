using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeLiseDoubleVM : ProbeLiseBaseVM
    {
        #region Constructors

        public ProbeLiseDoubleVM(IProbeService probeSupervisor, string probeId) : base(probeSupervisor, probeId)
        {
        }

        #endregion Constructors

        #region Events

        public delegate void ProbeUsedUpdatedHandler(bool isProbeUp);

        public event ProbeUsedUpdatedHandler ProbeUsedUpdated;

        #endregion Events

        #region Properties

        private bool _isAcquisitionForProbeUp = true;

        public bool IsAcquisitionForProbeUp
        {
            get
            {
                return _isAcquisitionForProbeUp;
            }
            set
            {
                if (_isAcquisitionForProbeUp == value)
                {
                    return;
                }

                _isAcquisitionForProbeUp = value;
                ProbeUsedUpdated?.Invoke(_isAcquisitionForProbeUp);
                OnPropertyChanged();
            }
        }

        private ProbeInputParametersLiseDoubleVM _inputParametersLiseDouble = null;

        public ProbeInputParametersLiseDoubleVM InputParametersLiseDouble
        {
            get
            {
                if (_inputParametersLiseDouble is null)
                {
                    _inputParametersLiseDouble = new ProbeInputParametersLiseDoubleVM();
                    _inputParametersLiseDouble.ProbeSample.Name = "Sample Name";
                    _inputParametersLiseDouble.ProbeSample.Info = "Sample Info";
                    // We subscribe to the propertyChanged on the InputParametersLiseVM
                    _inputParametersLiseDouble.PropertyChanged += InputParametersLiseVM_PropertyChanged;
                }
                return _inputParametersLiseDouble;
            }

            set
            {
                if (_inputParametersLiseDouble == value)
                {
                    return;
                }

                // We unsubscribe to the propertyChanged on the previous InputParametersLiseVM
                if (_inputParametersLiseDouble != null)
                    _inputParametersLiseDouble.PropertyChanged -= InputParametersLiseVM_PropertyChanged;

                _inputParametersLiseDouble = value;

                // We subscribe to the propertyChanged on the new InputParametersLiseVM
                _inputParametersLiseDouble.PropertyChanged += InputParametersLiseVM_PropertyChanged;

                OnPropertyChanged();
            }
        }

        private bool _isCalibrated = false;

        public bool IsCalibrated
        {
            get
            {
                return _isCalibrated;
            }

            set
            {
                if (_isCalibrated == value)
                {
                    return;
                }

                _isCalibrated = value;
                OnPropertyChanged();
            }
        }

        private ProbeDualLiseCalibResult _calibrationResult;

        public ProbeDualLiseCalibResult CalibrationResult
        {
            get
            {
                return _calibrationResult;
            }

            set
            {
                if (_calibrationResult == value)
                {
                    return;
                }

                _calibrationResult = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _calibrate;

        public AutoRelayCommand Calibrate
        {
            get
            {
                return _calibrate
                    ?? (_calibrate = new AutoRelayCommand(
                    () =>
                    {
                        var inputParametersLise = GetInputParametersForAcquisition() as DualLiseInputParams;
                        DualLiseCalibParams probeInputCalibParameters = new DualLiseCalibParams();

                        // TO DO
                        // note de rti : ça fait pas mal de parametre de calibration en dur , est ce toujours d'actu ?
                        probeInputCalibParameters.CalibrationMode = 3;
                        probeInputCalibParameters.BottomLiseAirgapThreshold = 0.7;
                        probeInputCalibParameters.TopLiseAirgapThreshold = 0.6;
                        probeInputCalibParameters.NbRepeatCalib = 16;
                        probeInputCalibParameters.ProbeCalibrationReference = new OpticalReferenceDefinition() { RefThickness = 750.46.Micrometers(), RefTolerance = 5.Micrometers(), RefRefrIndex = (float)1.4621 };

                        var axesService = ClassLocator.Default.GetInstance<IAxesService>();
                        var axesPosition = (XYZTopZBottomPosition)axesService.GetCurrentPosition()?.Result;
                        if (axesPosition != null)
                        {
                            probeInputCalibParameters.ZTopUsedForCalib = axesPosition.ZTop;
                            probeInputCalibParameters.ZBottomUsedForCalib = axesPosition.ZBottom;
                        }

                        if (inputParametersLise != null)
                        {
                            IsCalibrated = false;
                            StartCalibration(probeInputCalibParameters, inputParametersLise);
                            SettingsChanged = false;
                        }
                    },
                    () => true
                    ));
            }
        }

        #endregion RelayCommands

        #region Methods

        public override Sample Sample => (Sample)InputParametersLiseDouble.ProbeSample;

        public override void SetCalibrationResult(ProbeCalibResultsBase probeCalibrationResults)
        {
            CalibrationResult = probeCalibrationResults as ProbeDualLiseCalibResult;
            if (probeCalibrationResults.Success)
                IsCalibrated = true;
            else
                IsCalibrated = false;
            State = new DeviceState(DeviceStatus.Ready);

            UpdateAllCanExecutes();
        }

        public override ILiseInputParams GetInputParametersForAcquisition()
        {
            DualLiseInputParams inputParametersLise;
            Mapper mapper = ClassLocator.Default.GetInstance<Mapper>();
            inputParametersLise = mapper.AutoMap.Map<DualLiseInputParams>(_inputParametersLiseDouble);
            var a = mapper.AutoMap.Map<List<ProbeSampleLayer>>(_inputParametersLiseDouble.ProbeSample.Layers);
            inputParametersLise.ProbeSample.Layers = a.ToList();

            var configLiseDouble = Configuration as ProbeConfigurationLiseDoubleVM;
            inputParametersLise.CurrentProbeAcquisition = IsAcquisitionForProbeUp ? configLiseDouble.ProbeUp.DeviceID : configLiseDouble.ProbeDown.DeviceID;
            inputParametersLise.CurrentProbeModule = IsAcquisitionForProbeUp ? configLiseDouble.ProbeUp.ModulePosition : configLiseDouble.ProbeDown.ModulePosition;

            return inputParametersLise;
        }

        public override bool CheckInputParametersValidity()
        {
            if (_inputParametersLiseDouble is null)
                return false;

            if (_inputParametersLiseDouble.ProbeSample.Layers.Count() == 0)
                return false;

            int nbUNknownLayers = 0;

            foreach (var layer in _inputParametersLiseDouble.ProbeSample.Layers)
            {
                if (layer.RefractionIndex == 0)
                    nbUNknownLayers++;

                if ((layer.RefractionIndex != 0) && ((layer.Tolerance.Value == 0) || (layer.Thickness.Nanometers == 0)))
                    return false;
            }

            // We must have at least one layer with a thickness of 0
            if (nbUNknownLayers != 1)
                return false;

            return true;
        }

        #endregion Methods
    }
}
