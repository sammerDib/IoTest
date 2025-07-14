using System.Windows;
using System.Windows.Controls;


namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View
{
    /// <summary>
    /// Interaction logic for ProbeLiseView.xaml
    /// </summary>
    public partial class ProbeLiseView : UserControl
    {
        public ProbeLiseView()
        {
            InitializeComponent();
            Loaded += ProbeLiseView_Loaded;
        }

        private void ProbeLiseView_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
