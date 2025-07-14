using System.Windows.Media;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Hardware.ClientProxy.Screen
{
    public class ScreenImageChangedMessage
    {
        public string ScreenId;
        public ServiceImage ServiceImage;
        public Color Color;

        /// <summary>
        /// L'image au format WPF.
        /// Le but est que la conversion soit faite une seule fois même si plusieurs client souscrivent au message.
        /// </summary>
        public System.Windows.Media.Imaging.BitmapSource ImageSource
        {
            get
            {
                if (_imageSource == null)
                    _imageSource = ServiceImage.ConvertToWpfBitmapSource();
                return _imageSource;
            }
        }

        private System.Windows.Media.Imaging.BitmapSource _imageSource;
    }

    public class BacklightChangedMessage
    {
        public Side Side { get; set; }
        public short Backlight { get; set; }
    }

    public class BrightnessChangedMessage
    {
        public Side Side { get; set; }
        public short Brightness { get; set; }
    }

    public class ContrastChangedMessage
    {
        public Side Side { get; set; }
        public short Contrast { get; set; }
    }

    public class SharpnessChangedMessage
    {
        public Side Side { get; set; }
        public int Sharpness { get; set; }
    }

    public class TemperatureChangedMessage
    {
        public Side Side { get; set; }
        public double Temperature { get; set; }
    }

    public class FanChangedMessage
    {
        public Side Side { get; set; }
        public int Fan { get; set; }
    }

    public class FanAutoChangedMessage
    {
        public Side Side { get; set; }
        public bool FanAuto { get; set; }
    }

    public class PowerStateChangedMessage
    {
        public Side Side { get; set; }
        public bool PowerState { get; set; }
    }
}
