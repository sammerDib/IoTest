using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Hardware.GlobalStatus
{
    // Uncomment : IMenuItem to display in UI
    public class GlobalStatus //: IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "Goblal status";

        public string Description => "Global status of hardware";

        public string Group => "Hardware";

        public string ImageResourceKey => "HardwareGeometry";

        public int Priority => 210;

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ClassLocator.Default.GetInstance<GlobalStatusViewModel>();
                }
                return _viewModel;
            }
        }

        public UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new GlobalStatusView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public bool CanClose()
        {
            return true;
        }

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance, ApplicationMode.Production };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.Status };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }
    }
}
