using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Popup
{
    public class StringWrapper : Notifier
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => SetAndRaiseIfChanged(ref _value, value);
        }

        public StringWrapper(string value)
        {
            _value = value;
        }
    }

    public class CustomPopupContent
    {
    }

    public enum PopupDisplayOwner
    {
        Application,
        BusinessPanel,
        Custom
    }

    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class PopupPanel : BusinessPanel
    {
        #region Properties

        public PopupDisplayer LocalPopups { get; } = new();

        private MessageLevel _popupLevel;

        public MessageLevel PopupLevel
        {
            get => _popupLevel;
            private set => SetAndRaiseIfChanged(ref _popupLevel, value);
        }

        private bool _popupIsFullScreen;

        public bool PopupIsFullScreen
        {
            get => _popupIsFullScreen;
            set => SetAndRaiseIfChanged(ref _popupIsFullScreen, value);
        }

        private string _title = "Save your work?";

        public string Title
        {
            get => _title;
            set => SetAndRaiseIfChanged(ref _title, value);
        }

        private bool _enableMessage = true;

        public bool EnableMessage
        {
            get => _enableMessage;
            set => SetAndRaiseIfChanged(ref _enableMessage, value);
        }

        private string _message = "Lorem ipsum, nuntius semper magis narrans cum bene indutus est.";

        public string Message
        {
            get => _message;
            set => SetAndRaiseIfChanged(ref _message, value);
        }

        private bool _enableCustomContent = true;

        public bool EnableCustomContent
        {
            get => _enableCustomContent;
            set => SetAndRaiseIfChanged(ref _enableCustomContent, value);
        }

        public ObservableCollection<StringWrapper> PopupCommands { get; } = new()
        {
            new StringWrapper("Cancel"), new StringWrapper("Do not save"), new StringWrapper("Save")
        };

        #endregion

        static PopupPanel()
        {
            DataTemplateGenerator.Create(typeof(CustomPopupContent), typeof(CustomPopupContentView));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTree.DataTreePanel" /> class.
        /// </summary>
        public PopupPanel()
            : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTree.DataTreePanel" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public PopupPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
        }

        #region Private Methods

        private void ShowPopup(PopupDisplayOwner owner)
        {
            var popup = new Agileo.GUI.Services.Popups.Popup(new InvariantText(Title))
            {
                IsFullScreen = PopupIsFullScreen
            };

            popup.Message = EnableMessage ? new InvariantText(Message) : null;
            popup.Content = EnableCustomContent ? new CustomPopupContent() : null;
            popup.SeverityLevel = PopupLevel;

            foreach (var command in PopupCommands)
            {
                popup.Commands.Add(new PopupCommand(command.Value));
            }

            if (popup.Commands.Count == 0)
            {
                popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            }

            switch (owner)
            {
                case PopupDisplayOwner.Application:
                    App.Instance.UserInterface.Popups.Show(popup);
                    break;
                case PopupDisplayOwner.BusinessPanel:
                    Popups.Show(popup);
                    break;
                case PopupDisplayOwner.Custom:
                    LocalPopups.Show(popup);
                    break;
            }
        }

        #endregion

        #region Commands

        private ICommand _setPopupLevelCommand;

        public ICommand SetPopupLevelCommand
            => _setPopupLevelCommand ??= new DelegateCommand<object>(SetPopupLevelCommandExecute);

        private void SetPopupLevelCommandExecute(object arg)
        {
            if (arg is MessageLevel level)
            {
                PopupLevel = level;
            }
        }

        private ICommand _showOnApplicationCommand;

        public ICommand ShowOnApplicationCommand
            => _showOnApplicationCommand ??= new DelegateCommand(ShowOnApplicationCommandExecute);

        private void ShowOnApplicationCommandExecute() => ShowPopup(PopupDisplayOwner.Application);

        private ICommand _showOnBusinessPanelCommand;

        public ICommand ShowOnBusinessPanelCommand
            => _showOnBusinessPanelCommand ??= new DelegateCommand(ShowOnBusinessPanelCommandExecute);

        private void ShowOnBusinessPanelCommandExecute() => ShowPopup(PopupDisplayOwner.BusinessPanel);

        private ICommand _showOnCustomAreaCommand;

        public ICommand ShowOnCustomAreaCommand
            => _showOnCustomAreaCommand ??= new DelegateCommand(ShowOnCustomAreaCommandExecute);

        private void ShowOnCustomAreaCommandExecute() => ShowPopup(PopupDisplayOwner.Custom);

        private ICommand _addCommandCommand;

        public ICommand AddCommandCommand => _addCommandCommand ??= new DelegateCommand(AddCommandCommandExecute);

        private void AddCommandCommandExecute()
            => PopupCommands.Add(new StringWrapper($"Command {PopupCommands.Count + 1}"));

        private ICommand _removeCommandCommand;

        public ICommand RemoveCommandCommand
            => _removeCommandCommand ??= new DelegateCommand<StringWrapper>(RemoveCommandCommandExecute);

        private void RemoveCommandCommandExecute(StringWrapper arg) => PopupCommands.Remove(arg);

        #endregion
    }
}
