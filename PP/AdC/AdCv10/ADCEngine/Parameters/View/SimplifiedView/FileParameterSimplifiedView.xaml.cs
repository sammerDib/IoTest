using System.Windows;
using System.Windows.Controls;

namespace ADCEngine.View
{
    /// <summary>
    /// Interaction logic for FileParameterSimplifiedView.xaml
    /// </summary>
    public partial class FileParameterSimplifiedView : UserControl
    {
        private FileParameter ViewModel { get { return (FileParameter)DataContext; } }

        public FileParameterSimplifiedView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }
    }
}
