using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class SampleLayer : ObservableObject
    {
        public SampleLayer()
        { }

        public SampleLayer(Length thickness, LengthTolerance tolerance, double refractionIndex, double type)
        {
            Thickness = thickness;
            Tolerance = tolerance;
            RefractionIndex = refractionIndex;
            Type = type;
        }

        private Length _thickness;

        public Length Thickness
        {
            get
            {
                return _thickness;
            }

            set
            {
                if (_thickness == value)
                {
                    return;
                }

                _thickness = value;
                ResetMeasure();
                OnPropertyChanged();
            }
        }

        // Tolerance on the thickness
        private LengthTolerance _tolerance;

        public LengthTolerance Tolerance
        {
            get
            {
                return _tolerance;
            }
            set
            {
                if (_tolerance == value)
                {
                    return;
                }
                _tolerance = value;
                ResetMeasure();
                OnPropertyChanged();
            }
        }

        // Refraction index of the layer
        private double _refractionIndex;

        public double RefractionIndex
        {
            get
            {
                return _refractionIndex;
            }

            set
            {
                if (_refractionIndex == value)
                {
                    return;
                }

                _refractionIndex = value;
                ResetMeasure();
                OnPropertyChanged();
            }
        }

        private void ResetMeasure()
        {
            MeasuredThickness = null;
            MeasuredQuality = null;
        }

        private double? _measuredThickness = null;

        public double? MeasuredThickness
        {
            get
            {
                return _measuredThickness;
            }
            set
            {
                _measuredThickness = value;
                OnPropertyChanged();
            }
        }

        private double? _measuredQuality = null;

        public double? MeasuredQuality
        {
            get
            {
                return _measuredQuality;
            }
            set
            {
                _measuredQuality = value;
                OnPropertyChanged();
            }
        }

        public double Type { get; set; }

        //public double Gain;

        //public double QualityThreshold;
    }
}
