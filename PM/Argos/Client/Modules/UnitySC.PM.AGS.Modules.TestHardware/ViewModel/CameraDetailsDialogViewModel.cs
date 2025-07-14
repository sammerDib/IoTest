using GalaSoft.MvvmLight;

using MvvmDialogs;

namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class CameraDetailsDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields

        //private CamerasSupervisor _camerasSupervisor;
        private bool? _dialogResult;

        #endregion Fields
 
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => Set(nameof(DialogResult), ref _dialogResult, value);
        }
    }
}
