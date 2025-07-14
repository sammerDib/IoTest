using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitySC.Shared.ResultUI.Common.Helpers
{
    public static class ScreenshotHelper
    {
        public static Bitmap Take(FrameworkElement element)
        {
            var presentationSource = PresentationSource.FromVisual(element);

            if (presentationSource?.CompositionTarget == null) return null;

            var m = presentationSource.CompositionTarget.TransformToDevice;
            
            double dpiX = m.M11 * 96.0;
            double dpiY = m.M22 * 96.0;
            
            var target = new RenderTargetBitmap(
                (int)element.ActualWidth,
                (int)element.ActualHeight,
                dpiX, dpiY, PixelFormats.Pbgra32);
            target.Render(element);

            var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(target);
            encoder.Frames.Add(outputFrame);
            encoder.Save(stream);

            return new Bitmap(stream);
        }
    }
}
