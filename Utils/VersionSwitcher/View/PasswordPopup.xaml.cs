using System;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.DependencyInjection;

using VersionSwitcher.ViewModel;

namespace VersionSwitcher.View
{
    public partial class PasswordPopup : UserControl
    {
        public PasswordPopup()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<PasswordPopupViewModel>();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((PasswordPopupViewModel)DataContext).Password = ((PasswordBox)sender).SecurePassword;
        }

        public void SetFocus()
        {
            PasswordBox.Focus();
        }
    }
}
