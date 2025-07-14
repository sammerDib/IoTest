using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.ANA.Service.Interface.Calibration;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.View
{
    /// <summary>
    /// Interaction logic for XYCalibrationSettingsView.xaml
    /// </summary>
    public partial class XYCalibrationSettingsView : UserControl
    {
        public XYCalibrationSettingsView()
        {
            InitializeComponent();
        }
    }

    public class XYCalibDirectionToImageConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if( (value == null) || !(value is XYCalibDirection))
                    return null;

            XYCalibDirection xyCalibDirection = (XYCalibDirection)value;
            object correspondingGeometry=null;
            switch (xyCalibDirection)
            {
                case XYCalibDirection.TopBottomThenLeftRight:
                    correspondingGeometry= Application.Current.FindResource("ScanDirectionTBLR");
                    break;
                default:
                case XYCalibDirection.LeftRightThenTopBottom:
                    correspondingGeometry = Application.Current.FindResource("ScanDirectionLRTB");
                    break;
                case XYCalibDirection.BottomTopThenRightLeft:
                    correspondingGeometry = Application.Current.FindResource("ScanDirectionBTRL");
                    break;
                case XYCalibDirection.RightLeftThenBottomTop:
                    correspondingGeometry = Application.Current.FindResource("ScanDirectionRLBT");
                    break;
            }

            return correspondingGeometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
