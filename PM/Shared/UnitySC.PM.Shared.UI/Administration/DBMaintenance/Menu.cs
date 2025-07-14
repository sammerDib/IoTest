using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.UI.Administration.DBMaintenance
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Database maintenance";

        public string Description => "Use to maintain the database";

        public string Group => "Administration";

        public string ImageResourceKey => "DatabaseGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new DBMaintenanceVM();
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
                    _userControl = new DBMaintenanceView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public int Priority => 230;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance, ApplicationMode.Production, ApplicationMode.WaferLess };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.Configuration };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }

        public bool CanClose()
        {
            return _viewModel?.CanClose() ?? true;
        }
    }
}
