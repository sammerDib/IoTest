using System;
using System.Windows;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.View
{
    /// <summary>
    /// Interaction logic for XYCalibrationResultDisplay.xaml
    /// </summary>
    public partial class XYCalibrationResultDisplay : Window
    {
        public XYCalibrationResultDisplay()
        {
            InitializeComponent();
            Closing += XYCalibrationResultDisplay_Closing;     
            
        }

        private void XYCalibrationResultDisplay_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= XYCalibrationResultDisplay_Closing;
            if (DataContext is IDisposable toDispose)
            {
                toDispose.Dispose();
            }
        }
    }
}
