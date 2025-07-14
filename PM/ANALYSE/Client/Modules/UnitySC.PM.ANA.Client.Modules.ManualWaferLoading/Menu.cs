using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.ManualWaferLoading
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Manual wafer loading";
        public string Description => "Load the wafer manually";
        public string Group => "Tools";
        public string ImageResourceKey => "ManualGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new ManualWaferLoadingVM();
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
                    _userControl = new ManualWaferLoadingView
                    {
                        DataContext = ViewModel
                    };
                }
                return _userControl;
            }
        }

        public int Priority => 520;

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
