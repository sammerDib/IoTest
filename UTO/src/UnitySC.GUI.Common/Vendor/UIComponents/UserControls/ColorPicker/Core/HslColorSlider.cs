using System;
using System.Windows;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Core
{
    internal class HslColorSlider : PreviewColorSlider
    {
        public static readonly DependencyProperty SliderHslTypeProperty = DependencyProperty.Register(
            nameof(SliderHslType), typeof(string), typeof(HslColorSlider), new PropertyMetadata(""));

        public string SliderHslType
        {
            get => (string)GetValue(SliderHslTypeProperty);
            set => SetValue(SliderHslTypeProperty, value);
        }

        protected override void GenerateBackground()
        {
            switch (SliderHslType)
            {
                case "H":
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
                        break;
                    }
                case "L":
                    {
                        var colorStart = GetColorForSelectedArgb(0);
                        var colorEnd = GetColorForSelectedArgb(255);
                        LeftCapColor.Color = colorStart;
                        RightCapColor.Color = colorEnd;
                        BackgroundGradient = new GradientStopCollection
                        {
                            new(colorStart, 0),
                            new(GetColorForSelectedArgb(128), 0.5),
                            new(colorEnd, 1)
                        };
                        break;
                    }
                default:
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
                        break;
                    }
            }
        }

        private Color GetColorForSelectedArgb(int value)
        {
            switch (SliderHslType)
            {
                case "H":
                    {
                        var (r, g, b) = ColorSpaceHelper.HslToRgb(value, 1, 0.5);
                        return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                    }
                case "S":
                    {
                        var (r, g, b) = ColorSpaceHelper.HslToRgb(CurrentColorState.HslH, value / 255.0, 0.5);
                        return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                    }
                case "L":
                    {
                        var (r, g, b) = ColorSpaceHelper.HslToRgb(CurrentColorState.HslH, 1, value / 255.0);
                        return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                    }
                default:
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(CurrentColorState.RgbR * 255), (byte)(CurrentColorState.RgbG * 255), (byte)(CurrentColorState.RgbB * 255));
            }
        }
    }
}
