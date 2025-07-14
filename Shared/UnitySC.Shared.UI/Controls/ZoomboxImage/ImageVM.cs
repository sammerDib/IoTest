using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// VueModèle pour la liste des images de la ZoomboxWithImageList
    /// </summary>
    public class ImageVM
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Side { get; set; }
        public ImageType ImageType { get; set; }

        private ImageSource _image;

        public ImageSource Image
        {
            get
            {
                if (_image == null)
                {
                    BusyHourglass.SetBusyState();

                    try
                    {
                        _image = BitmapFrame.Create(new Uri(Path, UriKind.Absolute), BitmapCreateOptions.IgnoreImageCache | BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    }
                    catch (Exception)
                    {
                        var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                        dialogService.ShowMessageBox($"Unable to load the image : {Path}", "Load image", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    // Test of a diferent way to open the image but there is no difference
                    // Open a Stream and decode a TIFF image.
                    //Stream imageStreamSource = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //var decoder = new TiffBitmapDecoder( imageStreamSource, BitmapCreateOptions.PreservePixelFormat|BitmapCreateOptions.IgnoreColorProfile|BitmapCreateOptions.IgnoreImageCache , BitmapCacheOption.Default);
                    //BitmapSource bitmapSource = decoder.Frames[0];

                    //_image = decoder.Frames[0];
                }
                return _image;
            }
        }
    }
}
