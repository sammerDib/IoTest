using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestExternalProcessing
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public Menu()
        {
        }

        public string Name => "Test External Processing";

        public string Description => "Use to test external processing one by one";

        public string Group => "Test";

        public string ImageResourceKey => "RenderingGeometry";

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new TestEPVM();
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
                    _userControl = new TestEPView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public int Priority => 240;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UnitySC.Shared.Data.UserRights> RequiredRights => new List<UserRights>() { UserRights.ManualOperation };

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
