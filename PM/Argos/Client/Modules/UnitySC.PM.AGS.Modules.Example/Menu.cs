using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.AGS.Modules.Example
{
    public class Menu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;

        public string Name => "Example";

        public string Description => "Example description";

        public string Group => "Example Group";

        public string ImageResourceKey => "Help";

        public System.Windows.Controls.UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new ExampleView();
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
                    _viewModel = new ExampleVM();
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
