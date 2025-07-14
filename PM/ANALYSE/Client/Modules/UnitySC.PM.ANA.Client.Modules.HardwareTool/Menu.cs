using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.HardwareReset
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Hardware reset";

        public string Description => "Manage hardware";

        public string Group => "Tools";
        public string ImageResourceKey => "GearGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new HardwareResetVM();
                }
                return _viewModel;
            }
        }

        public int Priority => 510;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.HardwareManagement };

        public UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new HardwareResetView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }

        public bool CanClose()
        {
            return _viewModel.CanClose();
        }
    }
}
