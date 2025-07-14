using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.UI.Converters
{
    public class BitmapToBitmapImageConverter : MarkupExtension, IValueConverter
    {
        public bool UseCache { get; set; }

        private readonly Dictionary<Bitmap, BitmapImage> _cacheImages = new Dictionary<Bitmap, BitmapImage>();

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Bitmap bitmap)) return null;

            if (_cacheImages.ContainsKey(bitmap))
            {
                return _cacheImages[bitmap];
            }

            var bitmapImage = ImageHelper.FromBitmap(bitmap);

            if (UseCache)
            {
                _cacheImages.Add(bitmap, bitmapImage);
            }

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IValueConverter

        #region Overrides of MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion Overrides of MarkupExtension
    }
}
