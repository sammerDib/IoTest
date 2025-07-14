using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class CameraScreenSettingVM : ObservableRecipient
    {
        public CameraScreenSettingVM(Side waferSide)
        {
            WaferSide = waferSide;
        }

        public Side WaferSide { get; protected set; }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }
    }
}
