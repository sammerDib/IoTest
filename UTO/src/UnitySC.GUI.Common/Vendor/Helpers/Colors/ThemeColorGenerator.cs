using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.Helpers.Colors
{
    /// <summary>
    /// Class allowing to generate <see cref="Color"/> from the red, green and blue configured in the current theme of the application.
    /// </summary>
    public static class ThemeColorGenerator
    {
        private static float _brightR;
        private static float _brightG;
        private static float _brightB;

        private static float _saturationR;
        private static float _saturationG;
        private static float _saturationB;

        private static readonly Random Random = new();

        public static  bool IsInitialized { get; private set; }

        public static void InitializeWithAppColors()
        {
            if (IsInitialized) return;

            var redColor = UIComponents.XamlResources.Shared.Brushes.SeverityErrorBrush.Color;
            var greenColor = UIComponents.XamlResources.Shared.Brushes.SeveritySuccessBrush.Color;
            var blueColor = UIComponents.XamlResources.Shared.Brushes.SeverityInformationBrush.Color;
            Initialize(redColor, greenColor, blueColor);
        }
        
        public static void Initialize(Color red, Color blue, Color green)
        {
            if (IsInitialized) return;

            _brightR = GetBrightness(red);
            _brightG = GetBrightness(green);
            _brightB = GetBrightness(blue);

            _saturationR = GetSaturation(red);
            _saturationG = GetSaturation(green);
            _saturationB = GetSaturation(blue);

            IsInitialized = true;
        }

        public static IEnumerable<Color> Generate(int count, bool shuffle = false)
        {
            var colors = new List<Color>();

            var firstColor = 0;

            if (shuffle)
            {
                firstColor = Random.Next(360);
            }

            var hueRange = 360D / count;

            for (var i = 0; i < count; i++)
            {
                var hue = (i * hueRange + firstColor) % 360;
                colors.Add(GetColor(hue));
            }

            if (shuffle)
            {
                Shuffle(colors);
            }

            return colors;
        }

        private static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static Color GetColor(double hue)
        {
            var saturation = GetSaturationFromHue(hue);
            var brightness = GetBrightnessFromHue(hue);
            return HslToColor(hue, saturation, brightness);
        }

        #region Color Analysis

        private static float GetBrightness(Color color)
        {
            var num1 = color.R / (float)byte.MaxValue;
            var num2 = color.G / (float)byte.MaxValue;
            var num3 = color.B / (float)byte.MaxValue;
            var num4 = num1;
            var num5 = num1;

            if (num2 > (double)num4)
                num4 = num2;

            if (num3 > (double)num4)
                num4 = num3;

            if (num2 < (double)num5)
                num5 = num2;

            if (num3 < (double)num5)
                num5 = num3;

            return (float)((num4 + (double)num5) / 2.0);
        }

        private static float GetSaturation(Color color)
        {
            var num1 = color.R / (float)byte.MaxValue;
            var num2 = color.G / (float)byte.MaxValue;
            var num3 = color.B / (float)byte.MaxValue;
            var num4 = 0.0f;
            var num5 = num1;
            var num6 = num1;
            if (num2 > (double)num5)
                num5 = num2;
            if (num3 > (double)num5)
                num5 = num3;
            if (num2 < (double)num6)
                num6 = num2;
            if (num3 < (double)num6)
                num6 = num3;
            if (num5 != (double)num6)
                num4 = (num5 + (double)num6) / 2.0 > 0.5 ? (float)((num5 - (double)num6) / (2.0 - num5 - num6)) : (float)((num5 - (double)num6) / (num5 + (double)num6));
            return num4;
        }

        #endregion

        #region Color Generation

        private static double GetBrightnessFromHue(double hue)
        {
            double a, b;
            if (hue < 120)
            {
                a = (_brightG - _brightR) / 120;
                b = _brightR - a * 0;
            }
            else if (hue < 240)
            {
                a = (_brightB - _brightG) / 120;
                b = _brightG - a * 120;
            }
            else
            {
                a = (_brightR - _brightB) / 120;
                b = _brightB - a * 240;
            }
            return a * hue + b;
        }

        private static double GetSaturationFromHue(double hue)
        {
            double a, b;
            if (hue < 120)
            {
                a = (_saturationG - _saturationR) / 120;
                b = _saturationR - a * 0;
            }
            else if (hue < 240)
            {
                a = (_saturationB - _saturationG) / 120;
                b = _saturationG - a * 120;
            }
            else
            {
                a = (_saturationR - _saturationB) / 120;
                b = _saturationB - a * 240;
            }
            return a * hue + b;
        }

        private static double Tolerance => 0.000000000000001;

        /// <summary>
        /// Converts HSL to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// https://stackoverflow.com/questions/4087581/creating-a-c-sharp-color-from-hsl-values
        /// </summary>
        /// <param name="hue">Hue, must be in [0, 360].</param>
        /// <param name="saturation">Saturation, must be in [0, 1].</param>
        /// <param name="luminance">Luminance, must be in [0, 1].</param>
        private static Color HslToColor(double hue, double saturation, double luminance)
        {
            hue = Math.Max(0D, Math.Min(360D, hue));
            saturation = Math.Max(0D, Math.Min(1D, saturation));
            luminance = Math.Max(0D, Math.Min(1D, luminance));

            // achromatic argb (gray scale)
            if (Math.Abs(saturation) < Tolerance)
            {
                return Color.FromArgb(
                        255,
                        Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{luminance * 255D:0.00}")))),
                        Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{luminance * 255D:0.00}")))),
                        Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{luminance * 255D:0.00}")))));
            }

            var q = luminance < .5D
                    ? luminance * (1D + saturation)
                    : (luminance + saturation) - (luminance * saturation);
            var p = (2D * luminance) - q;

            var hk = hue / 360D;
            var T = new double[3];
            T[0] = hk + (1D / 3D); // Tr
            T[1] = hk; // Tb
            T[2] = hk - (1D / 3D); // Tg

            for (var i = 0; i < 3; i++)
            {
                if (T[i] < 0D)
                    T[i] += 1D;
                if (T[i] > 1D)
                    T[i] -= 1D;

                if ((T[i] * 6D) < 1D)
                    T[i] = p + ((q - p) * 6D * T[i]);
                else if ((T[i] * 2D) < 1)
                    T[i] = q;
                else if ((T[i] * 3D) < 2)
                    T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
                else
                    T[i] = p;
            }

            return Color.FromArgb(
                    255,
                    Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{T[0] * 255D:0.00}")))),
                    Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{T[1] * 255D:0.00}")))),
                    Math.Max((byte)0, Math.Min((byte)255, Convert.ToByte(double.Parse($"{T[2] * 255D:0.00}")))));
        }

        #endregion
    }
}

