using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;



namespace UnitySC.PM.DMT.Modules.Settings.View
{
    /// <summary>
    /// Interaction logic for CameraFocusView.xaml
    /// </summary>
    public partial class CameraFocusView : UserControl
    {
        public CameraFocusView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //// Load a tif for test

            //Stream imageStreamSource = new FileStream(@"c:\temp\Img00_Reflectivity.tiff", FileMode.Open, FileAccess.Read, FileShare.Read);

            //BitmapDecoder decoder;
            //decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            //BitmapSource img = decoder.Frames[0];

            //int bpp;
            ////Type = UspImageType.Greyscale;
            //bpp = 8;

            //long size = img.PixelHeight * img.PixelWidth * (bpp / 5);
            //byte[] Data = new byte[size];
            //img.CopyPixels(Data, img.PixelWidth * (bpp / 8), 0);
            //int DataWidth = img.PixelWidth;
            //int DataHeight = img.PixelHeight;

            //var pSubImages = new SubImageProperties[5];

            ////var a=FocusQualityWrapper.GetFocusQuality(ref pSubImages);

            //Stopwatch chrono = new Stopwatch();
            //chrono.Start();

            //var result = FocusQualityWrapper.FocusQuality(Data, (uint)DataHeight, (uint)DataWidth, 500.0 / 100.0 /*pattern size µm / camera resolution pix*/, pSubImages);

            //chrono.Stop();

            //MessageBox.Show(chrono.ElapsedMilliseconds.ToString());
        }
    }
}
