using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment
{
    //TODO Reactive this line to display the menu
    //public class ProbeAlignmentMenu : IMenuItem
    public class ProbeAlignmentMenu
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Probe Alignment";
        public string Description => "Align probes";
        public string Group => "Settings";
        public string ImageResourceKey => "ChamberGeometry";

        public IMenuContentViewModel ViewModel
        {
            get => _viewModel ?? (_viewModel = new ProbeAlignmentVM());
        }

        public UserControl UserControl
        {
            get => _userControl ?? (_userControl = new ProbeAlignmentView { DataContext = ViewModel });
        }


        public int Priority => 260;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode> { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights> { UserRights.Configuration };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }

        public bool CanClose()
        {
            return ViewModel.CanClose();
        }
    }
}
