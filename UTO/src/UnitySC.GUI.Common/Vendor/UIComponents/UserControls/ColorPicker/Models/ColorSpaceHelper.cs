using System;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models
{
    internal static class ColorSpaceHelper
    {
        /// <summary>
        /// Converts RGB to HSV, returns -1 for undefined channels
        /// </summary>
        /// <param name="r">Red channel</param>
        /// <param name="g">Green channel</param>
        /// <param name="b">Blue channel</param>
        /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Value (0-1)</returns>
        public static Tuple<double, double, double> RgbToHsv(double r, double g, double b)
        {
            double h, s;

            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var v = max;
            var delta = max - min;

            if (max != 0)
            {
                s = delta / max;
            }
            else
            {
                //pure black
                s = -1;
                h = -1;
                return new Tuple<double, double, double>(h, s, v);
            }
            if (r.Equals(max))
            {
                // between yellow & magenta
                h = (g - b) / delta;
            }
            else if (g.Equals(max))
            {
                // between cyan & yellow
                h = 2 + (b - r) / delta;
            }
            else
            {
                // between magenta & cyan
                h = 4 + (r - g) / delta;
            }

            h *= 60;
            if (h < 0)
            {
                h += 360;
            }
            if (double.IsNaN(h))
            {
                //delta == 0, case of pure gray
                h = -1;
            }

            return new Tuple<double, double, double>(h, s, v);
        }

        /// <summary>
        /// Converts RGB to HSL, returns -1 for undefined channels
        /// </summary>
        /// <param name="r">Red channel</param>
        /// <param name="b">Blue channel</param>
        /// <param name="g">Green channel</param>
        /// <returns>Values in order: Hue (0-360 or -1), Saturation (0-1 or -1), Lightness (0-1)</returns>
        public static Tuple<double, double, double> RgbToHsl(double r, double g, double b)
        {
            double h, s, l;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            l = (max + min) / 2;

            if (max == 0)
            {
                //pure black
                return new Tuple<double, double, double>(-1, -1, 0);
            }

            if (delta == 0)
            {
                //gray
                return new Tuple<double, double, double>(-1, 0, l);
            }

            //magic
            s = l <= 0.5 ? delta / (max + min) : delta / (2 - max - min);

            if (r.Equals(max))
            {
                h = (g - b) / 6 / delta;
            }
            else if (g.Equals(max))
            {
                h = 1.0f / 3 + (b - r) / 6 / delta;
            }
            else
            {
                h = 2.0f / 3 + (r - g) / 6 / delta;
            }

            if (h < 0)
            {
                h++;
            }

            if (h > 1)
            {
                h--;
            }

            h *= 360;

            return new Tuple<double, double, double>(h, s, l);
        }

        /// <summary>
        /// Converts HSV to RGB
        /// </summary>
        /// <param name="h">Hue, 0-360</param>
        /// <param name="s">Saturation, 0-1</param>
        /// <param name="v">Value, 0-1</param>
        /// <returns>Values (0-1) in order: R, G, B</returns>
        public static Tuple<double, double, double> HsvToRgb(double h, double s, double v)
        {
            if (s == 0)
            {
                // achromatic (grey)
                return new Tuple<double, double, double>(v, v, v);
            }

            if (h >= 360.0)
            {
                h = 0;
            }

            h /= 60;
            int i = (int)h;
            double f = h - i;
            double p = v * (1 - s);
            double q = v * (1 - s * f);
            double t = v * (1 - s * (1 - f));

            return i switch
            {
                0 => new Tuple<double, double, double>(v, t, p),
                1 => new Tuple<double, double, double>(q, v, p),
                2 => new Tuple<double, double, double>(p, v, t),
                3 => new Tuple<double, double, double>(p, q, v),
                4 => new Tuple<double, double, double>(t, p, v),
                _ => new Tuple<double, double, double>(v, p, q)
            };
        }

        /// <summary>
        /// Converts HSV to HSL
        /// </summary>
        /// <param name="h">Hue, 0-360</param>
        /// <param name="s">Saturation, 0-1</param>
        /// <param name="v">Value, 0-1</param>
        /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Lightness (0-1)</returns>
        public static Tuple<double, double, double> HsvToHsl(double h, double s, double v)
        {
            double hslL = v * (1 - s / 2);
            double hslS;
            if (hslL.Equals(0) || hslL.Equals(1))
            {
                hslS = -1;
            }
            else
            {
                hslS = (v - hslL) / Math.Min(hslL, 1 - hslL);
            }

            return new Tuple<double, double, double>(h, hslS, hslL);
        }

        /// <summary>
        /// Converts HSL to RGB
        /// </summary>
        /// <param name="h">Hue, 0-360</param>
        /// <param name="s">Saturation, 0-1</param>
        /// <param name="l">Lightness, 0-1</param>
        /// <returns>Values (0-1) in order: R, G, B</returns>
        public static Tuple<double, double, double> HslToRgb(double h, double s, double l)
        {
            int hueCircleSegment = (int)(h / 60);

            if (h.Equals(360))
            {
                hueCircleSegment = 5;
            }

            double circleSegmentFraction = (h - (60 * hueCircleSegment)) / 60;

            double maxRgb = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double minRgb = 2 * l - maxRgb;
            double delta = maxRgb - minRgb;

            return hueCircleSegment switch
            {
                // red-yellow
                0 => new Tuple<double, double, double>(maxRgb, delta * circleSegmentFraction + minRgb, minRgb),
                // yellow-green
                1 => new Tuple<double, double, double>(delta * (1 - circleSegmentFraction) + minRgb, maxRgb, minRgb),
                // green-cyan
                2 => new Tuple<double, double, double>(minRgb, maxRgb, delta * circleSegmentFraction + minRgb),
                // cyan-blue
                3 => new Tuple<double, double, double>(minRgb, delta * (1 - circleSegmentFraction) + minRgb, maxRgb),
                // blue-purple
                4 => new Tuple<double, double, double>(delta * circleSegmentFraction + minRgb, minRgb, maxRgb),
                // purple-red
                _ => new Tuple<double, double, double>(maxRgb, minRgb, delta * (1 - circleSegmentFraction) + minRgb)
            };
        }

        /// <summary>
        /// Converts HSL to HSV
        /// </summary>
        /// <param name="h">Hue, 0-360</param>
        /// <param name="s">Saturation, 0-1</param>
        /// <param name="l">Lightness, 0-1</param>
        /// <returns>Values in order: Hue (same), Saturation (0-1 or -1), Value (0-1)</returns>
        public static Tuple<double, double, double> HslToHsv(double h, double s, double l)
        {
            double hsvV = l + s * Math.Min(l, 1 - l);
            double hsvS;

            if (hsvV == 0)
            {
                hsvS = -1;
            }
            else
            {
                hsvS = 2 * (1 - l / hsvV);
            }

            return new Tuple<double, double, double>(h, hsvS, hsvV);
        }
    }
}
