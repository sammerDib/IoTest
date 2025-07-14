using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools;
using System.Linq;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeLiseVM : ProbeLiseBaseVM
    {
        #region Constructors

        public ProbeLiseVM(IProbeService probeSupervisor, string probeID) : base(probeSupervisor, probeID)
        {
        }

        #endregion Constructors

        #region Properties

        private ProbeInputParametersLiseVM _inputParametersLise = null;

        public ProbeInputParametersLiseVM InputParametersLise
        {
            get
            {
                if (_inputParametersLise is null)
                {
                    _inputParametersLise = new ProbeInputParametersLiseVM();
                    _inputParametersLise.ProbeSample.Name = "Sample Name";
                    _inputParametersLise.ProbeSample.Info = "Sample Info";
                    // We subscribe to the propertyChanged on the InputParametersLiseVM
                    _inputParametersLise.PropertyChanged += InputParametersLiseVM_PropertyChanged;
                }
                return _inputParametersLise;
            }

            set
            {
                if (_inputParametersLise == value)
                {
                    return;
                }

                // We unsubscribe to the propertyChanged on the previous InputParametersLiseVM
                if (_inputParametersLise != null)
                    _inputParametersLise.PropertyChanged -= InputParametersLiseVM_PropertyChanged;

                _inputParametersLise = value;

                // We subscribe to the propertyChanged on the new InputParametersLiseVM
                _inputParametersLise.PropertyChanged += InputParametersLiseVM_PropertyChanged;

                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        public override Sample Sample => (Sample)InputParametersLise.ProbeSample;

        public override void SetCalibrationResult(ProbeCalibResultsBase probeCalibrationResults)
        {
            return;
        }

        public override ILiseInputParams GetInputParametersForAcquisition()
        {
            SingleLiseInputParams inputParametersLise;
            Mapper mapper = ClassLocator.Default.GetInstance<Mapper>();
            inputParametersLise = mapper.AutoMap.Map<SingleLiseInputParams>(_inputParametersLise);
            var a = mapper.AutoMap.Map<List<ProbeSampleLayer>>(_inputParametersLise.ProbeSample.Layers);
            inputParametersLise.ProbeSample.Layers = a.ToList();

            return inputParametersLise;
        }

        public override bool CheckInputParametersValidity()
        {
            if (_inputParametersLise is null)
                return false;

            if (_inputParametersLise.ProbeSample.Layers.Count() == 0)
                return false;
            foreach (var layer in _inputParametersLise.ProbeSample.Layers)
            {
                if ((layer.Thickness.Nanometers == 0) || (layer.Tolerance.Value == 0) || (layer.RefractionIndex == 0))
                    return false;
            }
            return true;
        }

        #endregion Methods
    }
}
