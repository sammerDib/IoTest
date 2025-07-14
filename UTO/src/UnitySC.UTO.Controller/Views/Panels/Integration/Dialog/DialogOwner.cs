using System;
using System.ComponentModel;
using System.Windows;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.UTO.Controller.Views.Panels.Integration.Dialog
{
    public class DialogOwner : ObservableRecipient, IDialogOwnerService
    {
        private readonly DialogOwnerService _unityDialogOwnerService;

        public DialogOwner()
        {
            _unityDialogOwnerService = new DialogOwnerService(this, null, null, new UnityDialogFactory());
        }

        public static void Register()
        {
            ClassLocator.Default.Register<DialogOwner>(true);
            ClassLocator.Default.Register<IDialogOwnerService>(() => ClassLocator.Default.GetInstance<DialogOwner>());
        }

        #region Implementation of IDialogService

        public void Show<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel) where T : Window
        {
            _unityDialogOwnerService.Show<T>(ownerViewModel, viewModel);
        }

        public void ShowCustom<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel)
            where T : IWindow
        {
            _unityDialogOwnerService.ShowCustom<T>(ownerViewModel, viewModel);
        }

        public void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel)
        {
            var currentPanel = GUI.Common.App.Instance.UserInterface.Navigation.SelectedBusinessPanel;
            if (currentPanel != null && viewModel is GenericMvvmDialogViewModel genericMvvmDialogViewModel)
            {
                currentPanel.Popups.Show(new Popup(new InvariantText(genericMvvmDialogViewModel.Title))
                {
                    IsFullScreen = true,
                    Content = viewModel,
                    Commands =
                    {
                        new PopupCommand("Close", new DelegateCommand(() => genericMvvmDialogViewModel.OnClosed()))
                    }
                });
            }
            else
            {
                _unityDialogOwnerService.Show(ownerViewModel, viewModel);
            }
        }

        public bool? ShowDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
            where T : Window
        {
            return _unityDialogOwnerService.ShowDialog<T>(ownerViewModel, viewModel);
        }

        public bool? ShowCustomDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
            where T : IWindow
        {
            return _unityDialogOwnerService.ShowCustomDialog<T>(ownerViewModel, viewModel);
        }

        public bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
        {
            return _unityDialogOwnerService.ShowDialog(ownerViewModel, viewModel);
        }

        public bool Activate(INotifyPropertyChanged viewModel)
        {
            return _unityDialogOwnerService.Activate(viewModel);
        }

        public bool Close(INotifyPropertyChanged viewModel)
        {
            return _unityDialogOwnerService.Close(viewModel);
        }

        public MessageBoxResult ShowMessageBox(
            INotifyPropertyChanged ownerViewModel,
            string messageBoxText,
            string caption = "",
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return _unityDialogOwnerService.ShowMessageBox(ownerViewModel, messageBoxText, caption, button, icon, defaultResult);
        }

        public MessageBoxResult ShowMessageBox(INotifyPropertyChanged ownerViewModel, MessageBoxSettings settings)
        {
            return _unityDialogOwnerService.ShowMessageBox(ownerViewModel, settings);
        }

        public bool? ShowOpenFileDialog(INotifyPropertyChanged ownerViewModel, OpenFileDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowOpenFileDialog(ownerViewModel, settings);
        }

        public bool? ShowSaveFileDialog(INotifyPropertyChanged ownerViewModel, SaveFileDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowSaveFileDialog(ownerViewModel, settings);
        }

        public bool? ShowFolderBrowserDialog(
            INotifyPropertyChanged ownerViewModel,
            FolderBrowserDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowFolderBrowserDialog(ownerViewModel, settings);
        }

        #endregion

        #region Implementation of IDialogOwnerService

        public void Show<T>(INotifyPropertyChanged viewModel)
            where T : Window
        {
            _unityDialogOwnerService.Show<T>(viewModel);
        }

        public void Show(INotifyPropertyChanged viewModel)
        {
            _unityDialogOwnerService.Show(viewModel);
        }

        public void ShowCustom<T>(INotifyPropertyChanged viewModel)
            where T : IWindow
        {
            _unityDialogOwnerService.ShowCustom<T>(viewModel);
        }

        public bool? ShowCustomDialog<T>(IModalDialogViewModel viewModel)
            where T : IWindow
        {
            return _unityDialogOwnerService.ShowCustomDialog<T>(viewModel);
        }

        public bool? ShowDialog<T>(IModalDialogViewModel viewModel)
            where T : Window
        {
            return _unityDialogOwnerService.ShowDialog<T>(viewModel);
        }

        public bool? ShowDialog(IModalDialogViewModel viewModel)
        {
            return _unityDialogOwnerService.ShowDialog(viewModel);
        }

        public bool? ShowFolderBrowserDialog(FolderBrowserDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowFolderBrowserDialog(settings);
        }

        public MessageBoxResult ShowMessageBox(
            string messageBoxText,
            string caption = "",
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return _unityDialogOwnerService.ShowMessageBox(messageBoxText, caption, button, icon, defaultResult);
        }

        public MessageBoxResult ShowMessageBox(MessageBoxSettings settings)
        {
            return _unityDialogOwnerService.ShowMessageBox(settings);
        }

        public bool? ShowOpenFileDialog(OpenFileDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowOpenFileDialog(settings);
        }

        public bool? ShowSaveFileDialog(SaveFileDialogSettings settings)
        {
            return _unityDialogOwnerService.ShowSaveFileDialog(settings);
        }

        public void ShowException(Exception ex, string message)
        {
            _unityDialogOwnerService.ShowException(ex, message);
        }

        #endregion
    }
}
