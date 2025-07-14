using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UnitySC.Shared.Image
{
    public interface ICameraImage
    {
        int Width { get; }

        int Height { get; }

        DateTime Timestamp { get; set; }

        ImageFormat Format { get; }

        void Load(string filename);

        void Save(string filename);

        ServiceImage ToServiceImage();

        ServiceImage ToServiceImage(Int32Rect roi, double scale = 1);

        BitmapSource ConvertToWpfBitmapSource();
    }
}
