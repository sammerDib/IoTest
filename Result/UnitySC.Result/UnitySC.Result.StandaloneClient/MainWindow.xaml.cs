using System;
using System.Windows;
using System.Windows.Input;

namespace UnitySC.Result.StandaloneClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BackgroundRect_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainWindowVM mainWindowVM)
            {
                mainWindowVM.CloseExpanderCommand.Execute(null);
            }
        }
    }
}
