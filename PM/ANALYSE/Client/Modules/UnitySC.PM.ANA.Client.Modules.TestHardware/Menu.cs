using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Test hardware";

        public string Description => "Use to test hardware one by one";

        public string Group => "Test";

        public string ImageResourceKey => "HardwareGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new TestHardwareViewModel();
                }
                return _viewModel;
            }
        }

        public System.Windows.Controls.UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new TestHardwareView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public int Priority => 220;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.HardwareManagement};

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
