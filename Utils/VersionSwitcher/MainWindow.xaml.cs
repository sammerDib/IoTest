using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.DependencyInjection;

using VersionSwitcher.View;
using VersionSwitcher.ViewModel;

namespace VersionSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LOGO_PATH = @"C:\UnitySC\UNLogoBigTrans.ico";
            
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();
            this.Icon = File.Exists(LOGO_PATH) ? BitmapFrame.Create(new Uri(@"C:\UnitySC\UNLogoBigTrans.ico",
                UriKind.RelativeOrAbsolute)) : null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            object selectedVM = ((MainWindowViewModel)DataContext).SelectedVM;
            if (selectedVM is VersionSelectionPage &&
                ((VersionSelectionViewModel)((VersionSelectionPage)selectedVM).DataContext).OnSwitchCommand.IsRunning)
            {
                e.Cancel = true;
                MessageBox.Show("You cannot quit this application while the switch is in progress.", "Exit forbidden", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                
        }
    }
}
