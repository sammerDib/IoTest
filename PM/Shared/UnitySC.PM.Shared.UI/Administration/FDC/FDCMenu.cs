using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Administration.FDC
{
    public class FDCMenu : IMenuItem
    {
        private FDCViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "FDC";

        public string Description => "FDCs management";

        public string Group => "Administration";

        public string ImageResourceKey => "FDCGeometry";

        public int Priority => 400;

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ClassLocator.Default.GetInstance<FDCViewModel>();
                    
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
                    _userControl = new FDCView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance, ApplicationMode.Production };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.Log };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
        }

        public bool CanClose()
        {
            return _viewModel?.CanClose() ?? true;
        }
    }
}
