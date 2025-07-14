using UnitySC.PM.ANA.Client.Modules.TestHardware.View.Dialog;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class CamerasViewModel : TabViewModelBase
    {
        private CamerasSupervisor _camerasSupervisor;
        private IDialogOwnerService _dialogService;

        private AutoRelayCommand _showDetailsCommand;


        public CamerasViewModel()
        {
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        public CamerasSupervisor CamerasSupervisor
        {
            get
            {
                if (_camerasSupervisor == null)
                {
                    _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
                }

                return _camerasSupervisor;
            }
        }

        #region RelayCommand
        public AutoRelayCommand ShowDetailsCommand
        {
            get
            {
                return _showDetailsCommand ?? (_showDetailsCommand = new AutoRelayCommand(
                    () =>
                    {
                        var dialogViewModel = new CameraDetailsDialogViewModel();
                        _dialogService.ShowCustomDialog<CameraDetailsDialog>(dialogViewModel);
                    }));
            }
        }
        #endregion
    }
}
