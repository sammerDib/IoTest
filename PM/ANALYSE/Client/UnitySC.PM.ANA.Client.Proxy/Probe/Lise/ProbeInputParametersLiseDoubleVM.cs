using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeInputParametersLiseDoubleVM : ObservableObject
    {
        private double _gainUp = 1.8;
        private double _gainDown = 1.8;
        private double _qualityThresholdUp = 1;
        private double _qualityThresholdDown = 1;
        private double _detectionThresholdUp = 1;
        private double _detectionThresholdDown = 1;
        private IProbeSample _probeSample;

        public IProbeSample ProbeSample
        {
            get
            {
                if (_probeSample == null)
                    _probeSample = new Sample();
                return _probeSample;
            }
            set
            {
                if (_probeSample == value)
                {
                    return;
                }

                _probeSample = value;

                var layersUp = new List<ProbeSampleLayer>();
                var layersDown = new List<ProbeSampleLayer>();
                bool isLayerUp = true;
                foreach (var layer in _probeSample.Layers)
                {
                    if (layer.RefractionIndex == 0)
                    {
                        isLayerUp = false;
                    }
                    if (isLayerUp)
                    {
                        layersUp.Add(layer);
                    }
                    else
                    {
                        layersDown.Add(layer);
                    }
                }

                ProbeUpParams.ProbeSample = new ProbeSample(layersUp, _probeSample.Name, _probeSample.Info);
                ProbeDownParams.ProbeSample = new ProbeSample(layersDown, _probeSample.Name, _probeSample.Info);
                OnPropertyChanged();
            }
        }

        private int _nbMeasuresAverage = 16;

        public int NbMeasuresAverage
        {
            get
            {
                return _nbMeasuresAverage;
            }

            set
            {
                if (_nbMeasuresAverage == value)
                {
                    return;
                }
                _nbMeasuresAverage = value;
                _probeUpParams.NbMeasuresAverage = value;
                _probeDownParams.NbMeasuresAverage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProbeUpParams));
                OnPropertyChanged(nameof(ProbeDownParams));
            }
        }

        private string _currentProbeAcquisition;

        public string CurrentProbeAcquisition
        {
            get
            {
                return _currentProbeAcquisition;
            }
            set
            {
                _currentProbeAcquisition = value;
                OnPropertyChanged();
            }
        }

        private ModulePositions _currentProbeModule;

        public ModulePositions CurrentProbeModule
        {
            get
            {
                return _currentProbeModule;
            }
            set
            {
                _currentProbeModule = value;
                OnPropertyChanged();
            }
        }

        private ProbeInputParametersLiseVM _probeUpParams;

        public ProbeInputParametersLiseVM ProbeUpParams
        {
            get
            {
                var layersUp = new List<ProbeSampleLayer>();
                bool isLayerUp = true;
                foreach (var layer in _probeSample.Layers)
                {
                    if (layer.RefractionIndex == 0)
                    {
                        isLayerUp = false;
                    }
                    if (isLayerUp)
                    {
                        layersUp.Add(layer);
                    }
                }
                var probeSample = new ProbeSample(layersUp, _probeSample.Name, _probeSample.Info);

                if (_probeUpParams == null)
                {
                    _probeUpParams = new ProbeInputParametersLiseVM(probeSample, _gainUp, _qualityThresholdUp, _detectionThresholdUp, _nbMeasuresAverage);
                }
                else
                {
                    _probeUpParams.ProbeSample = probeSample;
                }

                return _probeUpParams;
            }
            set
            {
                if (_probeUpParams == value)
                {
                    return;
                }

                _probeUpParams = value;
                OnPropertyChanged();
            }
        }

        private ProbeInputParametersLiseVM _probeDownParams;

        public ProbeInputParametersLiseVM ProbeDownParams
        {
            get
            {
                var layersDown = new List<ProbeSampleLayer>();
                bool isLayerUp = true;
                foreach (var layer in _probeSample.Layers)
                {
                    if (!isLayerUp)
                    {
                        layersDown.Add(layer);
                    }
                    if (layer.RefractionIndex == 0)
                    {
                        isLayerUp = false;
                    }
                }
                var probeSample = new ProbeSample(layersDown, _probeSample.Name, _probeSample.Info);

                if (_probeDownParams == null)
                {
                    _probeDownParams = new ProbeInputParametersLiseVM(probeSample, _gainDown, _qualityThresholdDown, _detectionThresholdDown, _nbMeasuresAverage);
                }
                else
                {
                    _probeDownParams.ProbeSample = probeSample;
                }

                return _probeDownParams;
            }
            set
            {
                if (_probeDownParams == value)
                {
                    return;
                }

                _probeDownParams = value;
                OnPropertyChanged();
            }
        }
    }
}
