using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class AutofocusParametersVM : ObservableObject
    {
        private LengthVM _zMin;

        public LengthVM ZMin
        {
            get => _zMin; set { if (_zMin != value) { _zMin = value; OnPropertyChanged(); } }
        }

        private LengthVM _zMax;

        public LengthVM ZMax
        {
            get => _zMax; set { if (_zMax != value) { _zMax = value; OnPropertyChanged(); } }
        }

        private LengthVM _zFocusPosition;

        public LengthVM ZFocusPosition
        {
            get => _zFocusPosition; set { if (_zFocusPosition != value) { _zFocusPosition = value; OnPropertyChanged(); } }
        }

        private LiseAutofocusParametersVM _liseAutofocusParameters;

        public LiseAutofocusParametersVM Lise
        {
            get => _liseAutofocusParameters; set { if (_liseAutofocusParameters != value) { _liseAutofocusParameters = value; OnPropertyChanged(); } }
        }
    }
}
