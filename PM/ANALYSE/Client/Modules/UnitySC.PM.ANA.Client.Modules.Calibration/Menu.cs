using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.Calibration
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Calibration";

        public string Description => "Use to calibrate the tool";

        public string Group => "Settings";

        public string ImageResourceKey => "GearGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new CalibrationsVM();
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
                    _userControl = new CalibrationsView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public int Priority => 240;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.HardwareManagement };

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
