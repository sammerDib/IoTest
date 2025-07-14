using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class FlatFieldVM : SettingVM, ITabManager
    {
        public FlatFieldVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            Header = "Flat Field";
        }

        #region ITabManager implementation

        public void Display()
        {
        }

        public bool CanHide() => true;

        public void Hide()
        {
        }

        #endregion ITabManager implementation
    }
}
