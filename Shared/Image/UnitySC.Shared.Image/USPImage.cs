using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Data.Extensions;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.Shared.Image
{
    public class USPImage : ICameraImage
    {
        protected BitmapSource Src;

        public int Width
        { get { return Src.PixelWidth; } }

        public int Height
        { get { return Src.PixelHeight; } }

        public DateTime Timestamp { get; set; }

        public ImageFormat Format
        { get { return ImageFormat.GreyLevel; } }

        public string ImgPath { get; }

        public string FileName { get; private set; }

        public USPImage()
        { }

        public USPImage(string imgPath)
        {
            Load(imgPath);

            // USPImageMil() will be automatically called,
            // as well as DisposableObject(), which will increment
            // internal reference counter. As DummyUSPImageMil
            // does not need such management, we decrement the counter
            // here to avoid exception when runtime tries to Dispose
            // a DummyUSPImageMil with a ref counter > 0 (here, 1).
            //DelRef();
        }

        public USPImage(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
            new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            Src = BitmapSource.Create(
            bitmapData.Width, bitmapData.Height,
            bitmap.HorizontalResolution, bitmap.VerticalResolution,
            PixelFormats.Gray8, null,
            bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            // USPImageMil() will be automatically called,
            // as well as DisposableObject(), which will increment
            // internal reference counter. As DummyUSPImageMil
            // does not need such management, we decrement the counter
            // here to avoid exception when runtime tries to Dispose
            // a DummyUSPImageMil with a ref counter > 0 (here, 1).
            //DelRef();
        }

        public USPImage(ServiceImage img)
        {
            var bitmapSource = img.ConvertToWpfBitmapSource();
            Src = bitmapSource;
        }

        public USPImage(IntPtr pixels, int sizeX, int sizeY, int s32Pitch) : this(ConvertToByteArray(pixels, s32Pitch, sizeY), sizeX, sizeY, s32Pitch)
        {
            // USPImageMil() will be automatically called,
            // as well as DisposableObject(), which will increment
            // internal reference counter. As DummyUSPImageMil
            // does not need such management, we decrement the counter
            // here to avoid exception when runtime tries to Dispose
            // a DummyUSPImageMil with a ref counter > 0 (here, 1).
            //DelRef();
        }

        public USPImage(byte[] pixels, int sizeX, int sizeY, int s32Pitch)
        {
            int dpi = 96;
            Src = BitmapSource.Create(
                  sizeX,
                  sizeY,
                  dpi,
                  dpi,
                  PixelFormats.Gray8,
              null,
                  pixels,
                  s32Pitch);
            Src.Freeze();
        }

        public USPImage Clone()
        {
            return new USPImage(BitmapFromSource(Src));
        }

        public void Load(string filename)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filename);
            image.EndInit();
            Src = image;
            FileName = filename;
        }

        public ServiceImage ToServiceImage()
        {
            var serviceImage = new ServiceImage();
            serviceImage.Data = Src.ConvertToByteArray();
            serviceImage.DataWidth = Src.PixelWidth;
            serviceImage.DataHeight = Src.PixelHeight;
            serviceImage.Type = ServiceImage.GetTypeFromBitmapPixelFormat(Src.Format);
            return serviceImage;
        }

        public ServiceImage ToServiceImage(USPImage procimg)
        {
            var image = new ServiceImageWithStatistics();
            procimg.CopyTo(image);
            return image;
        }

        public ServiceImage ToServiceImage(Int32Rect roi, double scale = 1)
        {
            var target = new ServiceImageWithStatistics();
            target.Data = Src.ConvertToByteArray();
            target.OriginalWidth = Src.PixelWidth;
            target.OriginalHeight = Src.PixelHeight;
            target.DataWidth = Src.PixelWidth;
            target.DataHeight = Src.PixelHeight;
            target.AcquisitionRoi = roi;
            target.Scale = scale;
            target.Type = ServiceImage.GetTypeFromBitmapPixelFormat(Src.Format);

            return target;
        }

        public BitmapSource ConvertToWpfBitmapSource()
        {
            return Src;
        }

        private static byte[] ConvertToByteArray(IntPtr pixels, int sizeX, int sizeY)
        {
            byte[] pixelsArray = new byte[sizeX * sizeY];
            Marshal.Copy(pixels, pixelsArray, 0, sizeX * sizeY);
            return pixelsArray;
        }

        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        public void CopyTo(ServiceImageWithStatistics target)
        {
            target.Data = Src.ConvertToByteArray();
            target.OriginalWidth = Src.PixelWidth;
            target.OriginalHeight = Src.PixelHeight;
            target.DataWidth = Src.PixelWidth;
            target.DataHeight = Src.PixelHeight;
            target.AcquisitionRoi = Int32Rect.Empty;
            target.Scale = 1;
        }

        public ExternalImage ToExternalImage()
        {
            return ToServiceImage().ToExternalImage();
        }

        public void Save(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
