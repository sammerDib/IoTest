using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Edition.MicroscopeReview.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class MicroscopeReviewClassViewModel : ObservableRecipient
    {
        private MicroscopeReviewClass _reviewClassModel;
        private Action _classChanged;
        public MicroscopeReviewClassViewModel(MicroscopeReviewClass reviewClass, Action classChanged)
        {
            _reviewClassModel = reviewClass;
            StrategyTypes = Enum.GetValues(typeof(StrategyType)).Cast<StrategyType>();
            _classChanged = classChanged;
        }

        public bool UseNbSamples => _reviewClassModel.Strategy != StrategyType.All;

        public IEnumerable<StrategyType> StrategyTypes { get; private set; }

        public string DefectLabel
        {
            get => _reviewClassModel.DefectLabel;
            set
            {
                if (_reviewClassModel.DefectLabel != value)
                {
                    _reviewClassModel.DefectLabel = value;
                    OnPropertyChanged();
                    _classChanged.Invoke();
                }
            }
        }
        public bool UseReview
        {
            get => _reviewClassModel.UseReview;
            set
            {
                if (_reviewClassModel.UseReview != value)
                {
                    _reviewClassModel.UseReview = value;
                    OnPropertyChanged();
                    _classChanged.Invoke();
                }
            }
        }
        public int NbSamples
        {
            get => _reviewClassModel.NbSamples;
            set
            {
                if (_reviewClassModel.NbSamples != value)
                {
                    _reviewClassModel.NbSamples = value;
                    OnPropertyChanged();
                    _classChanged.Invoke();
                }
            }
        }

        public StrategyType Strategy
        {
            get => _reviewClassModel.Strategy;
            set
            {
                if (_reviewClassModel.Strategy != value)
                {
                    _reviewClassModel.Strategy = value;
                    OnPropertyChanged();
                    _classChanged.Invoke();
                    OnPropertyChanged(nameof(UseNbSamples));
                }
            }
        }
    }
}
