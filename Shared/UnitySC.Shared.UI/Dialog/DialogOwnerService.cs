using System;
using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using MvvmDialogs.DialogTypeLocators;
using MvvmDialogs.FrameworkDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.UI.Dialog.ExceptionDialogs;

namespace UnitySC.Shared.UI.Dialog
{
    public class DialogOwnerService : DialogService, IDialogOwnerService
    {
        private readonly ObservableObject _dialogOwner;

        public DialogOwnerService(ObservableRecipient dialogOwner) : base()
        {
            _dialogOwner = dialogOwner;
        }

        public DialogOwnerService(ObservableRecipient dialogOwner, IDialogFactory dialogFactory = null, IDialogTypeLocator dialogTypeLocator = null, IFrameworkDialogFactory frameworkDialogFactory = null) : base(dialogFactory, dialogTypeLocator, frameworkDialogFactory)
        {
            _dialogOwner = dialogOwner;
        }

        public void Show<T>(INotifyPropertyChanged viewModel) where T : Window
        {
            CheckOwner();
            Show<T>(_dialogOwner, viewModel);
        }

        public void Show(INotifyPropertyChanged viewModel)
        {
            CheckOwner();
            Show(_dialogOwner, viewModel);
        }

        public void ShowCustom<T>(INotifyPropertyChanged viewModel) where T : IWindow
        {
            CheckOwner();
            ShowCustom<T>(_dialogOwner, viewModel);
        }

        public bool? ShowCustomDialog<T>(IModalDialogViewModel viewModel) where T : IWindow
        {
            CheckOwner();
            return ShowCustomDialog<T>(_dialogOwner, viewModel);
        }

        public bool? ShowDialog<T>(IModalDialogViewModel viewModel) where T : Window
        {
            CheckOwner();
            return ShowDialog<T>(_dialogOwner, viewModel);
        }

        public bool? ShowDialog(IModalDialogViewModel viewModel)
        {
            CheckOwner();
            return ShowDialog(_dialogOwner, viewModel);
        }

        public void ShowException(Exception ex, string message)
        {
            CheckOwner();
            Application.Current.Dispatcher.Invoke(() =>
            {
                var exceptionVieModel = new ExceptionDialogViewModel(message, ex);
                ShowDialog(exceptionVieModel);
            });
        }

        public bool? ShowFolderBrowserDialog(FolderBrowserDialogSettings settings)
        {
            CheckOwner();
            return ShowFolderBrowserDialog(_dialogOwner, settings);
        }

        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            CheckOwner();
            return ShowMessageBox(_dialogOwner, messageBoxText, caption, button, icon, defaultResult);
        }

        public MessageBoxResult ShowMessageBox(MessageBoxSettings settings)
        {
            CheckOwner();
            return ShowMessageBox(_dialogOwner, settings);
        }

        public bool? ShowOpenFileDialog(OpenFileDialogSettings settings)
        {
            CheckOwner();
            return ShowOpenFileDialog(_dialogOwner, settings);
        }

        public bool? ShowSaveFileDialog(SaveFileDialogSettings settings)
        {
            CheckOwner();
            return ShowSaveFileDialog(_dialogOwner, settings);
        }

        private void CheckOwner()
        {
            if (_dialogOwner == null)
                throw new InvalidOperationException("The dialog owner is missing");
        }
    }
}