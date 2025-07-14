using System.Windows.Media.Imaging;

using UnitySC.Shared.Image;

namespace UnitySC.PM.ANA.Client.Proxy
{
    internal class ImageGrabbedMessage
    {
        /// <summary>
        /// L'image recue par WCF
        /// </summary>
        public ServiceImage ServiceImage;

        /// <summary>
        /// L'image au format WPF.
        /// Le but est que la conversion soit faite une seule fois même si plusieurs client souscrivent au message.
        /// </summary>
        public BitmapSource ImageSource
        {
            get
            {
                if (_imageSource == null)
                    _imageSource = ServiceImage?.ConvertToWpfBitmapSource();
                return _imageSource;
            }
        }
        private BitmapSource _imageSource;

    }
}
