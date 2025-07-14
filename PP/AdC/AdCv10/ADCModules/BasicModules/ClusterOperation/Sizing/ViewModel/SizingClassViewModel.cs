using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Sizing
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class SizingClassViewModel : ObservableRecipient
    {
        private SizingClass _sizingClass;
        private SizingParameter _parameter;

        public SizingClassViewModel(SizingClass sizingClass, SizingParameter parameter)
        {
            _sizingClass = sizingClass;
            _parameter = parameter;
        }

        public string DefectLabel { get { return _sizingClass.DefectLabel; } }

        public eSizingType Measure
        {
            get { return _sizingClass.Measure; }
            set
            {
                if (value == _sizingClass.Measure)
                    return;
                _sizingClass.Measure = value;
                OnPropertyChanged();
                _parameter.ReportChange();
            }
        }

        public double TuningMultiplier
        {
            get { return _sizingClass.TuningMultiplier; }
            set
            {
                if (value == _sizingClass.TuningMultiplier)
                    return;
                _sizingClass.TuningMultiplier = value;
                OnPropertyChanged();
                _parameter.ReportChange();
            }
        }

        public double TuningOffset
        {
            get { return _sizingClass.TuningOffset; }
            set
            {
                if (value == _sizingClass.TuningOffset)
                    return;
                _sizingClass.TuningOffset = value;
                OnPropertyChanged();
                _parameter.ReportChange();
            }
        }
    }
}
