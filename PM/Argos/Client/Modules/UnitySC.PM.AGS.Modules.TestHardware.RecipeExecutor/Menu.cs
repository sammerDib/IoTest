using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;


namespace UnitySC.PM.AGS.Modules.TestHardware.RecipeExecutor
{
    public  class Menu: IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "Recipe Executor";

        public string Description => "Use to test the execution of recipes";

        public string Group => "Test";

        public string ImageResourceKey => "PlayGeometry";

        public System.Windows.Controls.UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new RecipeExecutorView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new RecipeExecutorViewModel();
                }
                return _viewModel;
            }
        }

        public int Priority => 240;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Production };

        public IEnumerable<UnitySC.Shared.Data.UserRights> RequiredRights => new List<UserRights>() { UserRights.RecipeReadonly };

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
