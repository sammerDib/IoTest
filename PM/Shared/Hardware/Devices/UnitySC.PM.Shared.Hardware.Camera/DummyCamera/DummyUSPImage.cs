using System.Drawing;
using System.Windows.Media;

using UnitySC.Shared.Image;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummyUSPImage : USPImage
    {
        public DummyUSPImage(int width, int height, byte color, bool useSine = false, int depth = 8)
        {
            var pixelFormat = GetPixelFormat(depth);

            if (!useSine)
                Src = new DummyImage(width, height, color, pixelFormat).Src;
            else
                Src = new DummySinusoidImage(width, height, color, pixelFormat).Src;
        }

        private PixelFormat GetPixelFormat(int depth)
        {
            return depth == 16 ? PixelFormats.Gray16 : PixelFormats.Gray8;
        }

        public DummyUSPImage(string imgPath) : base(imgPath)
        {
        }

        public DummyUSPImage(Bitmap bitmap) : base(bitmap)
        {
        }

        public DummyUSPImage(ServiceImage img) : base(img)
        {
        }
    }
}
