using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnitySC.PM.DMT.CommonUI.View.ExposureSettings
{
    /// <summary>
    /// Interaction logic for ExposureView.xaml
    /// </summary>
    public partial class ExposureView : UserControl
    {
        public ExposureView()
        {
            InitializeComponent();
        }



        public bool IsManualTuneNeeded
        {
            get { return (bool)GetValue(IsManualTuneNeededProperty); }
            set { SetValue(IsManualTuneNeededProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsManualTuneNeeded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsManualTuneNeededProperty =
            DependencyProperty.Register("IsManualTuneNeeded", typeof(bool), typeof(ExposureView), new PropertyMetadata(false));





    }
}
