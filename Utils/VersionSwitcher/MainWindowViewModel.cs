using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using VersionSwitcher.View;
using VersionSwitcher.ViewModel;

namespace VersionSwitcher
{
    public class MainWindowViewModel : ObservableObject
    {
        private object _selectedVM;

        public object SelectedVM
        {
            get => _selectedVM;
            set => SetProperty(ref _selectedVM, value);
        }
        
        public MainWindowViewModel()
        {
            _selectedVM = new PasswordPopup();
            ((PasswordPopupViewModel)((PasswordPopup)_selectedVM).DataContext).OnPasswordIsValid += PasswordCheckHandler;
            ((PasswordPopup)_selectedVM).SetFocus();
        }

        private void PasswordCheckHandler(object sender, PasswordCheckEventArgs args)
        {
            bool isPasswordValid = args.IsPasswordValid;
            HandlePasswordSubmit(isPasswordValid);
        }

        private void HandlePasswordSubmit(bool isPasswordValid)
        {
            if (isPasswordValid)
            {
                SelectedVM = new VersionSelectionPage();
            }
            else
            {
                MessageBox.Show("Invalid password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
