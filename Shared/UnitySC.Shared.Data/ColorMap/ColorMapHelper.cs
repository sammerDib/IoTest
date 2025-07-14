using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace UnitySC.Shared.Data.ColorMap
{
    public static class ColorMapHelper
    {
        private static ReadOnlyCollection<ColorMap> s_colorMaps;

        public static ReadOnlyCollection<ColorMap> ColorMaps => s_colorMaps ?? (s_colorMaps = BuildAll().AsReadOnly());

        public static ColorMap ThumbnailColorMap { private get; set; }

        private static List<ColorMap> BuildAll()
        {
            var colorMaps = new List<ColorMap>();
            var assembly = Assembly.GetExecutingAssembly();
            var rm = new ResourceManager(assembly.GetName().Name + ".g", assembly);
            try
            {
                var list = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true).Cast<DictionaryEntry>();

                foreach (var item in list)
                {
                    if (item.Key.ToString().Contains(nameof(ColorMap).ToLower()))
                    {
                        string name = item.Key.ToString().Split('/').Last().Split('.').First();

                        var bitmap = Image.FromStream((Stream)item.Value) as Bitmap;
                        if (bitmap == null) continue;

                        var colors = GetColorsFromBitmap(bitmap);
                        var bitmapImage = ImageFromColors(colors);

                        colorMaps.Add(new ColorMap(name, colors, bitmapImage));
                    }
                }

                colorMaps.Sort((a, b) => a.Name.CompareTo(b.Name)); // ordre alphabetic
            }
            finally
            {
                rm.ReleaseAllResources();
            }

            return colorMaps;
        }

        private static Bitmap ImageFromColors(Color[] colors)
        {
            var bitmap = new Bitmap(colors.Length, 1);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            int widthInBytes = bitmapData.Width * bytesPerPixel;
            for (int x = 0; x < widthInBytes; x += bytesPerPixel)
            {
                var color = colors[x / 4];
                pixels[x] = color.B;
                pixels[x + 1] = color.G;
                pixels[x + 2] = color.R;
                pixels[x + 3] = color.A;
            }

            // Copy modified bytes back
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        /// <summary>
        /// Initialize color-map use to generate image.
        /// </summary>
        private static Color[] GetColorsFromBitmap(Bitmap bitMap)
        {
            int nbPaletteColors = bitMap.Width;
            var outColorMapRef = new Color[nbPaletteColors];

            var bitmapData = bitMap.LockBits(new Rectangle(0, 0, bitMap.Width, bitMap.Height), ImageLockMode.WriteOnly, bitMap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bitMap.PixelFormat) / 8;

            int byteCount = bitmapData.Stride * bitMap.Height;
            byte[] pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            Parallel.For(0, nbPaletteColors, i =>
            {
                int a = 255;
                int r = 0;
                int g = 0;
                int b = 0;

                switch (bytesPerPixel)
                {
                    // For 32 bpp set Red, Green, Blue and Alpha
                    case 4:
                        b = pixels[i * bytesPerPixel];
                        g = pixels[i * bytesPerPixel + 1];
                        r = pixels[i * bytesPerPixel + 2];
                        a = pixels[i * bytesPerPixel + 3];
                        break;
                    // For 24 bpp set Red, Green and Blue
                    case 3:
                        b = pixels[i * bytesPerPixel];
                        g = pixels[i * bytesPerPixel + 1];
                        r = pixels[i * bytesPerPixel + 2];
                        break;
                    // For 8 bpp set color value (Red, Green and Blue values are the same)
                    case 1:
                        b = g = r = pixels[i * bytesPerPixel];
                        break;
                }

                outColorMapRef[i] = Color.FromArgb(a, r, g, b);
            });

            //Unlock the bitmaps
            bitMap.UnlockBits(bitmapData);

            bitMap.Dispose();
            return outColorMapRef;
        }

        /// <summary>
        /// Determines the color index to use from the definition of an affine function.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="a">Slope</param>
        /// <param name="b">Ordinate at the origin</param>
        /// <param name="colorsLength">Number of colors</param>
        /// <returns>Color index</returns>
        public static int GetColorIndexFromValue(float value, float a, float b, int colorsLength)
        {
            if (value == 0) return -1;
            float fVal = value * a + b;
            int colorIndex = (int)Math.Round(fVal);
            if (colorIndex < 0) colorIndex = 0;
            else if (colorIndex >= colorsLength)
                colorIndex = colorsLength - 1;

            return colorIndex;
        }

        public static ColorMap GetThumbnailColorMap()
        {
            return ThumbnailColorMap ?? ColorMaps.First();
        }
    }
}
