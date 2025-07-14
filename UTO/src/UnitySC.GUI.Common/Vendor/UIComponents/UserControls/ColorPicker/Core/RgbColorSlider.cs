using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Core
{
    internal class RgbColorSlider : PreviewColorSlider
    {
        public static readonly DependencyProperty SliderArgbTypeProperty =
            DependencyProperty.Register(nameof(SliderArgbType), typeof(string), typeof(RgbColorSlider),
                new PropertyMetadata(""));

        public string SliderArgbType
        {
            get => (string)GetValue(SliderArgbTypeProperty);
            set => SetValue(SliderArgbTypeProperty, value);
        }

        protected override void GenerateBackground()
        {
            var colorStart = GetColorForSelectedArgb(0);
            var colorEnd = GetColorForSelectedArgb(255);
            LeftCapColor.Color = colorStart;
            RightCapColor.Color = colorEnd;
            BackgroundGradient = new GradientStopCollection
            {
                new(colorStart, 0.0),
                new(colorEnd, 1)
            };
        }

        private Color GetColorForSelectedArgb(byte value)
        {
            var a = (byte)(CurrentColorState.A * 255);
            var r = (byte)(CurrentColorState.RgbR * 255);
            var g = (byte)(CurrentColorState.RgbG * 255);
            var b = (byte)(CurrentColorState.RgbB * 255);

            return SliderArgbType switch
            {
                "A" => Color.FromArgb(value, r, g, b),
                "R" => Color.FromArgb(255, value, g, b),
                "G" => Color.FromArgb(255, r, value, b),
                "B" => Color.FromArgb(255, r, g, value),
                _ => Color.FromArgb(a, r, g, b)
            };
        }
    }
}
