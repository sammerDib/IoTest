using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    [ValueConversion(typeof(Color), typeof(string))]
    internal class ColorToHexConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(ColorToHexConverter),
                new PropertyMetadata(true, ShowAlphaChangedCallback));

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public event EventHandler OnShowAlphaChange;

        public void RaiseShowAlphaChange() => OnShowAlphaChange?.Invoke(this, EventArgs.Empty);

        private static void ShowAlphaChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorToHexConverter self)
            {
                self.RaiseShowAlphaChange();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color color)
            {
                return DependencyProperty.UnsetValue;
            }

            return !ShowAlpha ? ConvertNoAlpha(color) : color.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text)
            {
                return DependencyProperty.UnsetValue;
            }

            if (!ShowAlpha)
            {
                return ConvertBackNoAlpha(text);
            }

            text = Regex.Replace(text.ToUpperInvariant(), @"[^0-9A-F]", "");
            var final = new StringBuilder();

            switch (text.Length)
            {
                //short hex with no alpha
                case 3:
                    final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
                    break;

                //short hex with alpha
                case 4:
                    final.Append("#").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]).Append(text[3]).Append(text[3]);
                    break;

                //hex with no alpha
                case 6:
                    final.Append("#FF").Append(text);
                    break;
                default:
                    final.Append("#").Append(text);
                    break;
            }

            try
            {
                return ColorConverter.ConvertFromString(final.ToString());
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertNoAlpha(Color color)
        {
            return "#" + color.ToString().Substring(3, 6);
        }

        public object ConvertBackNoAlpha(string text)
        {
            text = Regex.Replace(text.ToUpperInvariant(), @"[^0-9A-F]", "");
            var final = new StringBuilder();

            switch (text.Length)
            {
                //short hex
                case 3:
                    final.Append("#FF").Append(text[0]).Append(text[0]).Append(text[1]).Append(text[1]).Append(text[2]).Append(text[2]);
                    break;
                case 4:
                case > 6:
                    return DependencyProperty.UnsetValue;
                //regular hex
                default:
                    final.Append("#").Append(text);
                    break;
            }

            try
            {
                return ColorConverter.ConvertFromString(final.ToString());
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
