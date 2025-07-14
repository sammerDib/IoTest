using System;
using System.Windows.Media;

namespace UnitySC.Shared.DieCutUpUI.Common
{
    public class HslColor
    {
        public double Hue;
        public double Lightness;
        public double Saturation;

        public HslColor(double hue, double saturation, double lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }

        public HslColor(int index, double saturation, double lightness)
        {
            Hue = IndexColor(index);
            Saturation = saturation;
            Lightness = lightness;
        }

        // Hand-made algorithm to iterate through all the color hues in a beautiful way
        // The idea is to segment all the hues into slices of 6 hues each, which have
        // a 60° difference between each.
        // Eg:
        // slice1: 0°, 60°, 120°, 180°, 240°, 300° (which are btw the primary and secondary colors)
        // slice2: 30°, 90°, 150°, 210°, 270°, 330°
        // slice3: 15°, 75°, 135°, 195°, 255°, 315°
        public double IndexColor(int index)
        {
            // Here, we'll call the reference hue shift the value of the first hue of each slice. As such,
            // each time we take a new reference hue shift between 0 and 60°, we have a new slice.
            // The idea if the code here is to maximize the distance of each successive reference hue shift 

            int refHueShiftIndex = index / 6;
            double hueOffset;

            // special cases: we visually prefer to have a shift of 30 before having one of 0

            if (refHueShiftIndex == 0)
            {
                hueOffset = 30;
            }
            else if (refHueShiftIndex == 1)
            {
                hueOffset = 0;
            }
            else
            {
                // The goal here is to know the current offset. To know it, we'll think in terms of
                // granularity modulus 60°: first of all we want 0°, then 30°, then all multiples of 15°,
                // then all multiples of 7.5°, and so on.
                //
                // To find the granularity, we can first realize that the log2 will increase to the next
                // integer value when there are at the previous integer value.
                //
                // Eg: log2(2) = 1, log2(4) = 2, log2(8) = 3, we realize that we have
                // 4 numbers between 4 and 8 (allowing us to reach the integer value 3)
                // and 2 numbers between 2 and 4 (allowing us to reach the integer value 2).
                // 
                // Then, we can realize that th same happens to each granularity: each time we change the
                // granularity, we divide it by 2, so there are 2 times more elements to iterate through
                // before going to the next granularity.
                // 
                // Eg: at granularity 15, we need 2 elements (15° and 45°),
                // and at granularity 7.5, we need 4 elements (7.5°, 22.5°, 37.5° and 52.5°),
                // 
                // As such, we can use the floor of the log2 to get the index of the granularity.

                int granularityIndex = (int)Math.Log(refHueShiftIndex, 2);

                // Here, to decide on the value 15°, we simply checked for the first few refHueShiftIndex what
                // granularity we want.

                double hueGranularity = 15.0 / granularityIndex;

                int indexInGranularity = refHueShiftIndex - (int)Math.Pow(2, granularityIndex);

                // To have the current value, we start with an offset of the granularity, and then
                // iterate with twice the granularity (otherwise we'll collide with the previous granularity)

                hueOffset = 60.0 - hueGranularity - hueGranularity * 2.0 * indexInGranularity;

                // Some colors hurt the eyes as is (eg: primary yellow, primary cyan), giving a small offset of 10
                // allows to avoid such colors on the first indexes

                hueOffset -= 10.0;
            }

            // To even more spread out the colors, we'll start at either the 1st (at +0°), 3rd (at +120°)
            // or 5th (at +240°) color of the slice.

            double resultingHue = hueOffset + 120 * (2 * refHueShiftIndex % 3);

            // And finally, the sequence the writer liked is by doing successively +180° and -60° to the color.
            // Eg: 0°, 180°, 120°, 300°, 240°, 60°

            resultingHue += 180 * (index % 2) + 120 * (index % 6 / 2);
            resultingHue %= 360;

            return resultingHue / 360;
        }

        public Color ToColor()
        {
            double r;
            double g;
            double b;
            if (Saturation == 0.0)
            {
                r = Lightness;
                g = Lightness;
                b = Lightness;
            }
            else
            {
                double q = Lightness < 0.5
                    ? Lightness * (1.0 + Saturation)
                    : Lightness + Saturation - Lightness * Saturation;
                double p = 2.0 * Lightness - q;
                r = HueToRgb(p, q, Hue + 1.0 / 3.0);
                g = HueToRgb(p, q, Hue);
                b = HueToRgb(p, q, Hue - 1.0 / 3.0);
            }

            return Color.FromRgb(
                (byte)Math.Round(r * 255),
                (byte)Math.Round(g * 255),
                (byte)Math.Round(b * 255)
            );
        }

        private double HueToRgb(double p, double q, double t)
        {
            if (t < 0.0) t += 1;
            if (t > 1.0) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
            return p;
        }
    }
}
