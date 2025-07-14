using System.Windows.Input;


using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.BusinessPanelCommand
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class BusinessPanelCommandPanel : BusinessPanel
    {
        /// <summary>
        /// Initializes the <see cref="BusinessPanelCommandPanel"/> class.
        /// </summary>
        static BusinessPanelCommandPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(BusinessPanelCommandResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:Agileo.SDK.UI.Templates.Business_Panel.BlankPanel" /> class.
        /// </summary>
        public BusinessPanelCommandPanel() : this("Commands", IconFactory.PathGeometryFromRessourceKey("RadioButtonIcon"))
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Agileo.GUI.Tester.UI.Panels.Testers.BusinessPanelCommand.BusinessPanelCommandTester" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public BusinessPanelCommandPanel(string id, IIcon icon = null) : base(id, icon)
        {
            // BusinessPanelCommand
            var command = new DelegateCommand(() => ShowCommandExecuted("Command"), () => BusinessPanelCommandCanExecute);
            var businessPanelCommand = new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND), command, PathIcon.Agileo);
            Commands.Add(businessPanelCommand);

            // BusinessPanelToggleCommand
            var commandA = new DelegateCommand(() => ShowCommandExecuted("Command A"), () => ToggleA);
            var commandB = new DelegateCommand(() => ShowCommandExecuted("Command B"), () => ToggleB);
            ToggleCommand = new BusinessPanelToggleCommand(nameof(BusinessPanelCommandResources.CMD_TOGGLE),
                new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND_A), commandA, PathIcon.Agileo),
                new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND_B), commandB, PathIcon.Agileo));
            Commands.Add(ToggleCommand);

            // BusinessPanelCheckToggleCommand
            var checkCommand = new DelegateCommand(() => ShowCommandExecuted("Checked Command"), () => CheckCanExecute);
            var uncheckCommand = new DelegateCommand(() => ShowCommandExecuted("Unchecked Command"), () => UncheckCanExecute);
            CheckCommand = new BusinessPanelCheckToggleCommand(nameof(BusinessPanelCommandResources.CMD_CHECK), checkCommand, uncheckCommand, PathIcon.Agileo);
            Commands.Add(CheckCommand);

            // BusinessPanelCommandGroup
            var group = new BusinessPanelCommandGroup("Group", PathIcon.Agileo);
            var commmand1 = new DelegateCommand(() => ShowCommandExecuted("Command 1"), () => Command1CanExecute);
            var commmand2 = new DelegateCommand(() => ShowCommandExecuted("Command 2"), () => Command2CanExecute);
            var commmand3 = new DelegateCommand(() => ShowCommandExecuted("Command 3"), () => Command3CanExecute);
            var subCommand1 = new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND_1), commmand1, PathIcon.Agileo);
            var subCommand2 = new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND_2), commmand2, PathIcon.Agileo);
            var subCommand3 = new Agileo.GUI.Components.Commands.BusinessPanelCommand(nameof(BusinessPanelCommandResources.CMD_COMMAND_3), commmand3, PathIcon.Agileo);
            group.Commands.Add(subCommand1);
            group.Commands.Add(subCommand2);
            group.Commands.Add(subCommand3);
            Commands.Add(group);
        }

        public BusinessPanelToggleCommand ToggleCommand { get; }
        
        public BusinessPanelCheckToggleCommand CheckCommand { get; }

        private void ShowCommandExecuted(string commandName)
        {
            var interaction = new Agileo.GUI.Services.Popups.Popup(new InvariantText($"{commandName} executed"));
            interaction.Commands.Add(new PopupCommand("Ok"));
            Popups.Show(interaction);
        }

        #region Properties

        private bool _businessPanelCommandCanExecute;

        public bool BusinessPanelCommandCanExecute
        {
            get { return _businessPanelCommandCanExecute; }
            set { SetAndRaiseIfChanged(ref _businessPanelCommandCanExecute, value); }
        }

        private bool _toggleA;

        public bool ToggleA
        {
            get { return _toggleA; }
            set { SetAndRaiseIfChanged(ref _toggleA, value); }
        }

        private bool _toggleB;

        public bool ToggleB
        {
            get { return _toggleB; }
            set { SetAndRaiseIfChanged(ref _toggleB, value); }
        }

        private bool _checkCanExecute;

        public bool CheckCanExecute
        {
            get { return _checkCanExecute; }
            set { SetAndRaiseIfChanged(ref _checkCanExecute, value); }
        }

        private bool _uncheckCanExecute;

        public bool UncheckCanExecute
        {
            get { return _uncheckCanExecute; }
            set { SetAndRaiseIfChanged(ref _uncheckCanExecute, value); }
        }

        private bool _command1CanExecute;

        public bool Command1CanExecute
        {
            get { return _command1CanExecute; }
            set { SetAndRaiseIfChanged(ref _command1CanExecute, value); }
        }

        private bool _command2CanExecute;

        public bool Command2CanExecute
        {
            get { return _command2CanExecute; }
            set { SetAndRaiseIfChanged(ref _command2CanExecute, value); }
        }

        private bool _command3CanExecute;

        public bool Command3CanExecute
        {
            get { return _command3CanExecute; }
            set { SetAndRaiseIfChanged(ref _command3CanExecute, value); }
        }

        #endregion

        private ICommand _setToogleCommandToDisplay;

        public ICommand SetToogleCommandToDisplay => _setToogleCommandToDisplay ??= new DelegateCommand<object>(SetToogleCommandToDisplayExecute, _ => true);

        private void SetToogleCommandToDisplayExecute(object value)
        {
            if (value is bool boolValue)
            {
                ToggleCommand.IsChecked = boolValue;
            }
        }

        private ICommand _setCheckStateCommand;

        public ICommand SetCheckStateCommand => _setCheckStateCommand ??= new DelegateCommand<object>(SetCheckStateCommandExecute, _ => true);
        
        private void SetCheckStateCommandExecute(object value)
        {
            if (value is bool boolValue)
            {
                CheckCommand.IsChecked = boolValue;
            }
        }
    }
}
