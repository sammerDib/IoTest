using System.Security.Principal;
using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using VersionSwitcher.ViewModel;

namespace VersionSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //TODO : Change that !
        
        #if DEBUG
        private static bool isElevated => true;
        #else
        private static bool isElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        #endif

        public App()
        {
            //Admin Right check
            if (!isElevated)
            {
                MessageBox.Show("This program should only run with an elevated privileges.", "Elevated Privileges",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            var serviceProvider = new ServiceCollection();
            serviceProvider.AddSingleton<VersionSelectionViewModel>();
            serviceProvider.AddSingleton<PasswordPopupViewModel>();
            serviceProvider.AddSingleton<MainWindowViewModel>();
            Ioc.Default.ConfigureServices(serviceProvider.BuildServiceProvider());
        }
    }
}
