using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Recipes.Management
{
    public class RecipesManagementMenu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "Recipes Manager";

        public string Description => "Process module recipes manager";

        public string Group => "Recipe";

        public string ImageResourceKey => "FileGeometry";
        public int Priority => 110;

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ClassLocator.Default.GetInstance<RecipesManagementViewModel>();
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
                    _userControl = new RecipesManagementView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance, ApplicationMode.Production, ApplicationMode.WaferLess };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            ((RecipesManagementViewModel)ViewModel).Mode = newMode;
        }

        public bool CanClose()
        {
            if (_viewModel == null)
                return true;
            return _viewModel.CanClose();
        }

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.RecipeReadonly };
    }
}
