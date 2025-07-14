using System;
using System.Windows;

using MvvmDialogs.FrameworkDialogs.MessageBox;

using Ookii.Dialogs.Wpf;

namespace UnitySC.Shared.UI.Dialog.FrameworkDialogs.MessageBox
{
    /// <summary>
    /// Custom message box using Ooki dialogs
    /// </summary>
    internal sealed class CustomMessageBox : IMessageBox
    {
        private readonly MessageBoxSettings _settings;
        private readonly TaskDialog _messageBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageBox"/> class.
        /// </summary>
        /// <param name="settings">The settings for the folder browser dialog.</param>
        public CustomMessageBox(MessageBoxSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _messageBox = new TaskDialog
            {
                Content = settings.MessageBoxText
            };
            SetUpTitle();
            SetUpButtons();
            SetUpIcon();
        }

        /// <summary>
        /// Opens a message box with specified owner.
        /// </summary>
        /// <param name="owner">
        /// Handle to the window that owns the dialog.
        /// </param>
        /// <returns>
        /// A <see cref="MessageBoxResult"/> value that specifies which message box button is
        /// clicked by the user.
        /// </returns>
        public MessageBoxResult Show(Window owner)
        {
            TaskDialogButton result;
            if (owner == null)
                result = _messageBox.ShowDialog();
            else
                result = _messageBox.ShowDialog(owner);

            return ToMessageBoxResult(result);
        }

        private void SetUpTitle()
        {
            _messageBox.WindowTitle = string.IsNullOrEmpty(_settings.Caption) ?
                " " :
                _settings.Caption;
        }

        private void SetUpButtons()
        {
            switch (_settings.Button)
            {
                case MessageBoxButton.OKCancel:
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Ok) { Default = MessageBoxResult.OK == _settings.DefaultResult });
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Cancel) { Default = MessageBoxResult.Cancel == _settings.DefaultResult });
                    break;

                case MessageBoxButton.YesNo:
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Yes) { Default = MessageBoxResult.Yes == _settings.DefaultResult });
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.No) { Default = MessageBoxResult.No == _settings.DefaultResult });
                    break;

                case MessageBoxButton.YesNoCancel:
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Yes) { Default = MessageBoxResult.Yes == _settings.DefaultResult });
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.No) { Default = MessageBoxResult.No == _settings.DefaultResult });
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Cancel) { Default = MessageBoxResult.Cancel == _settings.DefaultResult });
                    break;

                default:
                    _messageBox.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    break;
            }
        }

        private void SetUpIcon()
        {
            switch (_settings.Icon)
            {
                case MessageBoxImage.Error:
                    _messageBox.MainIcon = TaskDialogIcon.Error;
                    break;

                case MessageBoxImage.Information:
                    _messageBox.MainIcon = TaskDialogIcon.Information;
                    break;

                case MessageBoxImage.Warning:
                    _messageBox.MainIcon = TaskDialogIcon.Warning;
                    break;

                default:
                    _messageBox.MainIcon = TaskDialogIcon.Custom;
                    break;
            }
        }

        private static MessageBoxResult ToMessageBoxResult(TaskDialogButton button)
        {
            switch (button.ButtonType)
            {
                case ButtonType.Cancel:
                    return MessageBoxResult.Cancel;

                case ButtonType.No:
                    return MessageBoxResult.No;

                case ButtonType.Ok:
                    return MessageBoxResult.OK;

                case ButtonType.Yes:
                    return MessageBoxResult.Yes;

                default:
                    return MessageBoxResult.None;
            }
        }
    }
}