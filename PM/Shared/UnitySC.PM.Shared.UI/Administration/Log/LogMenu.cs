using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.UI.Administration.Log
{
    public class LogMenu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "Log";

        public string Description => "Database log";

        public string Group => "Administration";

        public string ImageResourceKey => "LogGeometry";

        public int Priority => 410;

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ClassLocator.Default.GetInstance<LogViewModel>();
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
                    _userControl = new LogView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance, ApplicationMode.Production };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }

        public bool CanClose()
        {
            return true;
        }

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.Log };
    }
}
