using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.NavigateToPanelTester
{
    public class NavigateToPanel : BusinessPanel
    {
        public NavigateToPanel() : this("Navigate To Panel", IconFactory.PathGeometryFromRessourceKey("NavigationIcon"))
        {

        }

        public NavigateToPanel(string id, IIcon icon = null) : base(id, icon)
        {
        }

        private List<BusinessPanel> _panels;

        public List<BusinessPanel> Panels
        {
            get => _panels;
            set => SetAndRaiseIfChanged(ref _panels, value);
        }

        private BusinessPanel _selectedPanel;

        public BusinessPanel SelectedPanel
        {
            get => _selectedPanel;
            set => SetAndRaiseIfChanged(ref _selectedPanel, value);
        }

        #region NavigateToCommand

        private ICommand _navigateToCommand;

        public ICommand NavigateToCommand => _navigateToCommand ??= new DelegateCommand(NavigateToCommandExecute, () => SelectedPanel != null);

        private void NavigateToCommandExecute()
        {
            AgilControllerApplication.Current.UserInterface.Navigation.NavigateTo(SelectedPanel.NavigationAddress);
        }

        #endregion NavigateToCommand

        public override void OnSetup()
        {
            base.OnSetup();
            Panels = AgilControllerApplication.Current.UserInterface.BusinessPanels.ToList();
        }
    }
}
