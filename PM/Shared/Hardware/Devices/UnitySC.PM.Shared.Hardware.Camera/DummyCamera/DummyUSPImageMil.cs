using System.Drawing;
using System.Windows.Media;

using UnitySC.Shared.Image;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummyUSPImageMil : USPImageMil
    {
        public DummyUSPImageMil(int width, int height, byte color)
        {
            Src = new DummyImage(width, height, color, PixelFormats.Gray8).Src;
        }

        public DummyUSPImageMil(string imgPath) : base(imgPath)
        {
        }

        public DummyUSPImageMil(Bitmap bitmap) : base(bitmap)
        {
        }
    }
}
