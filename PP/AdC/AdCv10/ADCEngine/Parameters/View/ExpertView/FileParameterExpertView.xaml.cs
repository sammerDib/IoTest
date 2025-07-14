using System.Windows;
using System.Windows.Controls;

namespace ADCEngine.View
{
    /// <summary>
    /// Interaction logic for FileParameterExpertView.xaml
    /// </summary>
    public partial class FileParameterExpertView : UserControl
    {

        private FileParameter ViewModel { get { return (FileParameter)DataContext; } }

        public FileParameterExpertView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }
    }
}
