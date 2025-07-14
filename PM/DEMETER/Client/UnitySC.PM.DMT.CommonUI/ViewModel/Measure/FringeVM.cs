using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.DMT.Service.Interface.Fringe;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class FringeVM : ObservableRecipient
    {
        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private FringeType _fringeType;

        public FringeType FringeType
        {
            get => _fringeType; set { if (_fringeType != value) { _fringeType = value; OnPropertyChanged(); } }
        }

        private int _period;

        public int Period
        {
            get => _period; set { if (_period != value) { _period = value; OnPropertyChanged(); } }
        }

        private List<int> _periods;

        public List<int> Periods
        {
            get => _periods; set { if (_periods != value) { _periods = value; OnPropertyChanged(); Period = _periods[0]; } }
        }

        private int _nbImagesPerDirection;

        public int NbImagesPerDirection
        {
            get => _nbImagesPerDirection; set { if (_nbImagesPerDirection != value) { _nbImagesPerDirection = value; OnPropertyChanged(); } }
        }

        private int _slopePrecision;

        public int SlopePrecision
        {
            get => _slopePrecision; set { if (_slopePrecision != value) { _slopePrecision = value; OnPropertyChanged(); } }
        }

        private int _phaseShiftPrecision;

        public int PhaseShiftPrecision
        {
            get => _phaseShiftPrecision; set { if (_phaseShiftPrecision != value) { _phaseShiftPrecision = value; OnPropertyChanged(); } }
        }

        private List<string> _imageFilenames;

        public List<string> ImageFilenames
        {
            get => _imageFilenames; set { if (_imageFilenames != value) { _imageFilenames = value; OnPropertyChanged(); } }
        }
    }
}
