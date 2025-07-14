using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class LiseAutofocusParametersVM : ObservableObject
    {
        private double _minGain;

        public double MinGain
        {
            get => _minGain; set { if (_minGain != value) { _minGain = value; OnPropertyChanged(); } }
        }

        private double _maxGain;

        public double MaxGain
        {
            get => _maxGain; set { if (_maxGain != value) { _maxGain = value; OnPropertyChanged(); } }
        }

        private LengthVM _zStartPosition;

        public LengthVM ZStartPosition
        {
            get => _zStartPosition; set { if (_zStartPosition != value) { _zStartPosition = value; OnPropertyChanged(); } }
        }

        private LengthVM _airGap;

        public LengthVM AirGap
        {
            get => _airGap; set { if (_airGap != value) { _airGap = value; OnPropertyChanged(); } }
        }
    }
}
