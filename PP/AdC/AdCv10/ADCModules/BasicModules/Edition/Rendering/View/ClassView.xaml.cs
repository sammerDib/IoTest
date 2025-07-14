using System.Windows;
using System.Windows.Controls;

namespace BasicModules.Edition.Rendering
{
    /// <summary>
    /// Interaction logic for GenericClassView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class ClassView : UserControl
    {
        public ClassView()
        {
            InitializeComponent();
        }

        private void ClassGotFocus(object sender, RoutedEventArgs e)
        {
            listViewClass.SelectedItem = (sender as Grid).DataContext;
        }

    }
}
