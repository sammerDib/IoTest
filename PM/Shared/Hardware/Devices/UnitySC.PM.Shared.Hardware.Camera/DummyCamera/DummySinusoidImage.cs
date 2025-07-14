using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummySinusoidImage
    {
        private const double BaseWaveLength = 200.0;
        private const int LUT_SIZE = 4096;
        private static readonly double[] s_sinLUT = BuildSinLUT(LUT_SIZE);

        public BitmapSource Src { get; }

        private readonly double _cosA;
        private readonly double _sinA;

        public DummySinusoidImage(int width, int height, byte color, PixelFormat pixelFormat)
        {
            const int dpi = 96;

            var angle = new Angle(20, AngleUnit.Degree);
            _cosA = Math.Cos(angle.Radians);
            _sinA = Math.Sin(angle.Radians);

            try
            {
                double timeRatio = color / 255.0;
                double maxValue = (pixelFormat == PixelFormats.Gray8) ? 255.0 : 65535.0;

                if (pixelFormat == PixelFormats.Gray8)
                {
                    byte[] pixels = GeneratePixels<byte>(width, height, timeRatio, maxValue, (val) => (byte)val);
                    Src = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Gray8, null, pixels, width);
                }
                else
                {
                    short[] pixels = GeneratePixels<short>(width, height, timeRatio, maxValue, (val) => (short)val);
                    Src = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Gray16, null, pixels, width * 2);
                }

                Src.Freeze();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating image: " + e.Message);
            }
        }

        private T[] GeneratePixels<T>(int width, int height, double timeRatio, double maximumPixelValue, Func<double, T> convert)
        {
            var pixels = new T[width * height];

            double phase = 0.5 * BaseWaveLength * timeRatio;
            double amplitude = 0.25 + 1.5 * (timeRatio > 0.5 ? 1.0 - timeRatio : timeRatio);
            double invBaseWaveLength = 1.0 / BaseWaveLength;
            double lutScale = LUT_SIZE / (2.0 * Math.PI);
            T colorval = convert(timeRatio * 255.0);
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = colorval;
                }
            }
            for (int y = 20; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double proj = x * _cosA + y * _sinA;
                    double angle = (2.0 * Math.PI * proj * invBaseWaveLength + phase);
                    angle %= 2.0 * Math.PI;
                    if (angle < 0) angle += 2.0 * Math.PI;

                    int index = (int)(angle * lutScale) % LUT_SIZE;
                    double sinVal = s_sinLUT[index];

                    double value = amplitude * sinVal;
                    value = 0.5 * (value + 1.0) * maximumPixelValue;

                    pixels[y * width + x] = convert(value);
                }
            }

            return pixels;
        }

        private static double[] BuildSinLUT(int size)
        {
            double[] lut = new double[size];
            for (int i = 0; i < size; i++)
            {
                lut[i] = Math.Sin(2.0 * Math.PI * i / size);
            }
            return lut;
        }
    }
}
