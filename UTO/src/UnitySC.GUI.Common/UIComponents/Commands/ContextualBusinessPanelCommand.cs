using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.UIComponents.Commands
{
    public class ContextualBusinessPanelCommand : BusinessPanelCommand
    {
        #region Constructors

        public ContextualBusinessPanelCommand(
            string id,
            ContextualDelegateCommand command,
            IIcon icon = null)
            : base(id, command, icon)
        {
        }

        #endregion

        #region Open Can Execute Popup Command

        private SafeDelegateCommandAsync _openCanExecuteCommand;

        public ICommand OpenCanExecutePopupCommand
            => _openCanExecuteCommand ??= new SafeDelegateCommandAsync(
                OpenCanExecutePopupCommandExecute,
                () => true);

        private Task OpenCanExecutePopupCommandExecute()
        {
            return Task.Run(
                () =>
                {
                    var popup = new Agileo.GUI.Services.Popups.Popup(
                        PopupButtons.OK,
                        new InvariantText("Pre-conditions"),
                        new InvariantText(GetCanExecuteText()));
                    App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(popup);
                });
        }

        #endregion

        #region Protected Methods

        protected string GetCanExecuteText()
        {
            return (IntegratorCommand as ContextualDelegateCommand)?.GetPreconditions();
        }

        #endregion
    }
}
