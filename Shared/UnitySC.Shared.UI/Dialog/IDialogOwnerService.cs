using System;
using System.ComponentModel;
using System.Windows;

using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

namespace UnitySC.Shared.UI.Dialog
{
    public interface IDialogOwnerService : IDialogService
    {
        //
        // Summary:
        //     Displays a non-modal dialog of specified type T.
        //
        //   viewModel:
        //     The view model of the new dialog.
        //
        // Type parameters:
        //   T:
        //     The type of the dialog to show.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        void Show<T>(INotifyPropertyChanged viewModel) where T : Window;

        //
        // Summary:
        //     Displays a non-modal dialog of a type that is determined by the dialog type locator.
        //
        //
        //   viewModel:
        //     The view model of the new dialog.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.

        void Show(INotifyPropertyChanged viewModel);

        //
        // Summary:
        //     Displays a non-modal custom dialog of specified type T.
        //
        // Parameters:
        //   viewModel:
        //     The view model of the new custom dialog.
        //
        // Type parameters:
        //   T:
        //     The type of the custom dialog to show.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.

        void ShowCustom<T>(INotifyPropertyChanged viewModel) where T : IWindow;

        //
        // Summary:
        //     Displays a custom modal dialog of specified type T.
        //
        // Parameters:
        //   viewModel:
        //     The view model of the new custom dialog.
        //
        // Type parameters:
        //   T:
        //     The type of the custom dialog to show.
        //
        // Returns:
        //     A nullable value of type System.Boolean that signifies how a window was closed
        //     by the user.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowCustomDialog<T>(IModalDialogViewModel viewModel) where T : IWindow;

        //
        // Summary:
        //     Displays a modal dialog of specified type T.
        //
        // Parameters:
        //   viewModel:
        //     The view model of the new dialog.
        //
        // Type parameters:
        //   T:
        //     The type of the dialog to show.
        //
        // Returns:
        //     A nullable value of type System.Boolean that signifies how a window was closed
        //     by the user.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowDialog<T>(IModalDialogViewModel viewModel) where T : Window;

        //
        // Summary:
        //     Displays a modal dialog of a type that is determined by the dialog type locator.
        //
        // Parameters:
        //   viewModel:
        //     The view model of the new dialog.
        //
        // Returns:
        //     A nullable value of type System.Boolean that signifies how a window was closed
        //     by the user.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowDialog(IModalDialogViewModel viewModel);

        //
        // Summary:
        //     Displays the System.Windows.Forms.FolderBrowserDialog.
        //
        // Parameters:
        //   settings:
        //     The settings for the folder browser dialog.
        //
        // Returns:
        //     If the user clicks the OK button of the dialog that is displayed, true is returned;
        //     otherwise false.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowFolderBrowserDialog(FolderBrowserDialogSettings settings);

        //
        // Summary:
        //     Displays a message box that has a message, title bar caption, button, and icon;
        //     and that accepts a default message box result and returns a result.
        //
        // Parameters:
        //   messageBoxText:
        //     A System.String that specifies the text to display.
        //
        //   caption:
        //     A System.String that specifies the title bar caption to display. Default value
        //     is an empty string.
        //
        //   button:
        //     A System.Windows.MessageBoxButton value that specifies which button or buttons
        //     to display. Default value is System.Windows.MessageBoxButton.OK.
        //
        //   icon:
        //     A System.Windows.MessageBoxImage value that specifies the icon to display. Default
        //     value is System.Windows.MessageBoxImage.None.
        //
        //   defaultResult:
        //     A System.Windows.MessageBoxResult value that specifies the default result of
        //     the message box. Default value is System.Windows.MessageBoxResult.None.
        //
        // Returns:
        //     A System.Windows.MessageBoxResult value that specifies which message box button
        //     is clicked by the user.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.

        MessageBoxResult ShowMessageBox(string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None);

        //
        // Summary:
        //     Displays a message box that has a message, title bar caption, button, and icon;
        //     and that accepts a default message box result and returns a result.
        //
        // Parameters:
        //   settings:
        //     The settings for the message box dialog.
        //
        // Returns:
        //     A System.Windows.MessageBoxResult value that specifies which message box button
        //     is clicked by the user.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.

        MessageBoxResult ShowMessageBox(MessageBoxSettings settings);

        //
        // Summary:
        //     Displays the System.Windows.Forms.OpenFileDialog.
        //
        // Parameters:
        //   settings:
        //     The settings for the open file dialog.
        //
        // Returns:
        //     If the user clicks the OK button of the dialog that is displayed, true is returned;
        //     otherwise false.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowOpenFileDialog(OpenFileDialogSettings settings);

        //
        // Summary:
        //     Displays the System.Windows.Forms.SaveFileDialog.
        //
        // Parameters:
        //   settings:
        //     The settings for the save file dialog.
        //
        // Returns:
        //     If the user clicks the OK button of the dialog that is displayed, true is returned;
        //     otherwise false.
        //
        // Exceptions:
        //   T:MvvmDialogs.ViewNotRegisteredException:
        //     No view is registered with specified owner view model as data context.
        bool? ShowSaveFileDialog(SaveFileDialogSettings settings);

        /// <summary>
        /// Display the Excpetion dialog
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message"> Message </param>
        void ShowException(Exception ex, string message);
    }
}