using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummyImage
    {
        public BitmapSource Src { get; }

        public DummyImage(int width, int height, byte color, PixelFormat pixelFormat)
        {
            const int dpi = 96;
            try
            {
                if (pixelFormat == PixelFormats.Gray8)
                {
                    byte[] pixels = GeneratePixelFor8BitsFormat(width, height, color);
                    Src = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Gray8, null, pixels, width);
                }
                else
                {
                    short[] pixels = GeneratePixelFor16BitsFormat(width, height, color);
                    Src = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Gray16, null, pixels, width * 2);
                }
                
                Src.Freeze();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating image: " + e.Message);
            }
        }

        private static byte[] GeneratePixelFor8BitsFormat(int width, int height, byte color)
        {
            byte[] pixels = new byte[height * width];
            for (int index = 0; index < pixels.Length; ++index)
            {
                pixels[index] = color;
            }

            return pixels;
        }
        
        private static short[] GeneratePixelFor16BitsFormat(int width, int height, byte color)
        {
            short[] pixels = new short[height * width];
            for (int index = 0; index < pixels.Length; ++index)
            {
                pixels[index] = (short)((double)color / 255 * 65535);
            }

            return pixels;
        }
    }
}
