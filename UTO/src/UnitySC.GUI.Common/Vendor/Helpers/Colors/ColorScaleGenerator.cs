using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.Helpers.Colors
{
    /// <summary>
    /// Provide services to generate a list of color.
    /// </summary>
    public static class ColorScaleGenerator
    {
        private static int CubicRoot(int n)
        {
            var res = Math.Pow(n, 1.0 / 3.0);
            return (int)Math.Ceiling(res);
        }

        /// <summary>
        /// Generate at least <paramref name="number"/> colors by maximizing distance between each pair of colors.
        /// First, the generator will cut every color component (RED, GREEN, and BLUE), and then it will merge each component ladder rung with all other component ladder rungs.
        /// </summary>
        /// <param name="number">The number of colors to generate.</param>
        /// <returns>A list of generated colors.</returns>
        public static List<Color> GenerateColors(int number)
        {
            var l = new List<Color>();
            var nBis = (int)Math.Pow(number, 1.0 / 3.0) + 1;
            var n = 255 / ((int)Math.Pow(number, 1.0 / 3.0) + 1);

            for (var i = 0; i < nBis; ++i)
            {
                var r = (byte)(n * i % 255);
                for (var j = 0; j < nBis; ++j)
                {
                    var g = (byte)(n * j % 255);
                    for (var k = 0; k < nBis; ++k)
                    {
                        var b = (byte)(n * k % 255);
                        l.Add(Color.FromRgb(r, g, b));
                    }
                }
            }

            return l;
        }

        /// <summary>
        /// Generate at least <paramref name="minNumber"/> colors. They will be chosen to be the more contrasted accorded to the given background.
        /// The number of generated colors will be a power of three.
        /// First, the generator will cut every color component (RED, GREEN, and BLUE), and then it will merge each component ladder rung with all other component ladder rungs.
        /// </summary>
        /// <param name="minNumber">The minimum number of colors to be generated.</param>
        /// <param name="bkR">The red component of the background.</param>
        /// <param name="bkG">The green component of the background.</param>
        /// <param name="bkB">The blue component of the background.</param>
        /// <returns></returns>
        public static List<Color> GenerateColorsOnBackground(int minNumber, byte bkR, byte bkG, byte bkB)
        {
            var chosenColors = new List<Color>(minNumber);

            // We will remove up to 8 (= 2^3) colors possibilities to avoid the background color
            var nInScale = CubicRoot(minNumber + 8);
            var step = Math.Floor(byte.MaxValue / (nInScale - 1.0));

            var scale = new List<byte>(nInScale);
            for (var i = 0; i < nInScale; ++i)
                scale.Add(Convert.ToByte(step * i));

            for (var i = 0; i < nInScale; ++i)
            {
                for (var j = 0; j < nInScale; ++j)
                {
                    for (var k = 0; k < nInScale; ++k)
                    {
                        var isCeilR = bkR >= scale[i] && i < nInScale - 1 && bkR <= scale[i + 1];
                        var isCeilG = bkG >= scale[j] && j < nInScale - 1 && bkG <= scale[j + 1];
                        var isCeilB = bkB >= scale[k] && k < nInScale - 1 && bkB <= scale[k + 1];
                        var isFloorR = bkR <= scale[i] && i > 0 && bkR <= scale[i - 1];
                        var isFloorG = bkG <= scale[j] && j > 0 && bkG <= scale[j - 1];
                        var isFloorB = bkB <= scale[k] && k > 0 && bkB <= scale[k - 1];

                        if ((isFloorR || isCeilR) && (isFloorG || isCeilG) && (isFloorB || isCeilB)) continue;
                        var colorToKeep = new Color { A = byte.MaxValue, R = scale[i], G = scale[j], B = scale[k] };
                        chosenColors.Add(colorToKeep);
                    }
                }
            }

            return chosenColors;
        }
    }
}
