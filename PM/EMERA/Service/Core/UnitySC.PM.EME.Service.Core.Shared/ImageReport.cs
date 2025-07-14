using System;
using System.IO;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Image;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public static class ImageReport
    {
        private static ILogger Logger => ClassLocator.Default.GetInstance<ILogger>();
        private static string LogHeader => "[ImageReport]";

        public static void SaveImage(ServiceImage img, string filename)
        {
            try
            {
                img.SaveToFile(filename);
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
            }
        }

        public static void SaveImage(ICameraImage image, string filename)
        {
            try
            {
                var imageBitmapSource = image.ConvertToWpfBitmapSource();
                var encoder = new PngBitmapEncoder();
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Frames.Add(BitmapFrame.Create(imageBitmapSource));
                    encoder.Save(stream);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
            }
        }
    }
}
