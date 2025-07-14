using System.Windows;
using System.Windows.Controls;



namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View
{
    /// <summary>
    /// Interaction logic for ProbeLiseView.xaml
    /// </summary>
    public partial class ProbeLiseDoubleView : UserControl
    {
        public ProbeLiseDoubleView()
        {
            InitializeComponent();
            this.Loaded += ProbeLiseDoubleView_Loaded;
        }

        private void ProbeLiseDoubleView_Loaded(object sender, RoutedEventArgs e)
        {

            // We must do it in the code behind because if we do it in the xaml it doesn't work. It is a ligtning chart bug.
            //RawAcquisitionChart.ColorTheme = ColorTheme.LightGray;
        }

  
    }
}
