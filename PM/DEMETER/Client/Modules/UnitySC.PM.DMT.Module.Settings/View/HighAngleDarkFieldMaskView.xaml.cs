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

namespace UnitySC.PM.DMT.Modules.Settings.View
{
    /// <summary>
    /// Interaction logic for HighAngleDarkfieldMaskView.xaml
    /// </summary>
    public partial class HighAngleDarkFieldMaskView : UserControl
    {
        public HighAngleDarkFieldMaskView()
        {
            InitializeComponent();
        }

        private void SetInitialZoom()
        {
            //drawingZoombox.MinScale = 0;
            //drawingZoombox.FitToBounds();
            //drawingZoombox.MinScale = drawingZoombox.Scale;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialZoom();
        }

  
    }
}
