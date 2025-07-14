using System;
using System.Windows;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Core
{
    internal class HsvColorSlider : PreviewColorSlider
    {
        public static readonly DependencyProperty SliderHsvTypeProperty =
            DependencyProperty.Register(nameof(SliderHsvType), typeof(string), typeof(HsvColorSlider),
                new PropertyMetadata(""));

        public string SliderHsvType
        {
            get => (string)GetValue(SliderHsvTypeProperty);
            set => SetValue(SliderHsvTypeProperty, value);
        }

        protected override void GenerateBackground()
        {
            if (SliderHsvType == "H")
            {
                var colorStart = GetColorForSelectedArgb(0);
                var colorEnd = GetColorForSelectedArgb(360);
                LeftCapColor.Color = colorStart;
                RightCapColor.Color = colorEnd;
                BackgroundGradient = new GradientStopCollection
                {
                    new(colorStart, 0),
                    new(GetColorForSelectedArgb(60), 1/6.0),
                    new(GetColorForSelectedArgb(120), 2/6.0),
                    new(GetColorForSelectedArgb(180), 0.5),
                    new(GetColorForSelectedArgb(240), 4/6.0),
                    new(GetColorForSelectedArgb(300), 5/6.0),
                    new(colorEnd, 1)
                };
            }
            else
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
        }

        private Color GetColorForSelectedArgb(int value)
        {
            double r, g, b;
            switch (SliderHsvType)
            {
                case "H":
                    (r, g, b) = ColorSpaceHelper.HsvToRgb(value, 1, 1);
                    return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                case "S":
                    (r, g, b) = ColorSpaceHelper.HsvToRgb(CurrentColorState.HsvH, value / 255.0, 1);
                    return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                case "V":
                    (r, g, b) = ColorSpaceHelper.HsvToRgb(CurrentColorState.HsvH, 1, value / 255.0);
                    return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                default:
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(CurrentColorState.RgbR * 255), (byte)(CurrentColorState.RgbG * 255), (byte)(CurrentColorState.RgbB * 255));
            }
        }
    }
}
