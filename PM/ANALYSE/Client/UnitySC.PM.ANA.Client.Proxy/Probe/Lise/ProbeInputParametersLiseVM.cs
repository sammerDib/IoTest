using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeInputParametersLiseVM : ObservableObject, ISingleLiseInputParams
    {
        public ProbeInputParametersLiseVM()
        {

        }

        public ProbeInputParametersLiseVM(ProbeSample probeSample, double gain, double qualityThreshold, double detectionThreshold, int nbMeasuresAverage)
        {
            ProbeSample = probeSample;
            Gain = gain;
            QualityThreshold = qualityThreshold;
            DetectionThreshold = detectionThreshold;
            NbMeasuresAverage = nbMeasuresAverage;
        }

        private string _liseInputParameter1;

        public string LiseInputParameter1
        {
            get { return _liseInputParameter1; }
            set
            {
                _liseInputParameter1 = value;
                OnPropertyChanged();
            }
        }

        private double _gain = 1.8;

        public double Gain
        {
            get
            {
                return _gain;
            }

            set
            {
                if (_gain == value)
                {
                    return;
                }

                _gain = value;
                OnPropertyChanged();
            }
        }

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
                OnPropertyChanged();
            }
        }

        private double _qualityThreshold = 1;

        public double QualityThreshold
        {
            get
            {
                return _qualityThreshold;
            }

            set
            {
                if (_qualityThreshold == value)
                {
                    return;
                }

                _qualityThreshold = value;
                OnPropertyChanged();
            }
        }

        private double _detectionThreshold = 1;

        public double DetectionThreshold
        {
            get
            {
                return _detectionThreshold;
            }

            set
            {
                if (_detectionThreshold == value)
                {
                    return;
                }

                _detectionThreshold = value;
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
                OnPropertyChanged();
            }
        }
    }
}
