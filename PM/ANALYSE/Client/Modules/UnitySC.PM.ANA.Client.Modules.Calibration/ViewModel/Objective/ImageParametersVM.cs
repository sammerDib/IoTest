using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class ImageParametersVM : ObservableObject
    {
        private LengthVM _pixelSize;

        public LengthVM PixelSize
        {
            get => _pixelSize; set { if (_pixelSize != value) { _pixelSize = value; OnPropertyChanged(); } }
        }

        private LengthVM _xOffset;

        public LengthVM XOffset
        {
            get => _xOffset; set { if (_xOffset != value) { _xOffset = value; OnPropertyChanged(); } }
        }

        private LengthVM _yOffset;

        public LengthVM YOffset
        {
            get => _yOffset; set { if (_yOffset != value) { _yOffset = value; OnPropertyChanged(); } }
        }

        private XYPosition _centricitiesRefPosition;

        public XYPosition CentricitiesRefPosition
        {
            get => _centricitiesRefPosition; set { if (_centricitiesRefPosition != value) { _centricitiesRefPosition = value; OnPropertyChanged(); } }
        }
    }
}
