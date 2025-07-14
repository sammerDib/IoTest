using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using UnitySC.PM.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Hardware.Service.Interface.Screen
{
    public class ScreenCorrections
    {
        public ScreenCorrectionData CalibrationCorrection { get; private set; }
        public Bitmap CorrectedImage { get; private set; }

        //======================================================================
        /// <summary>
        /// Load "ScreenCorrections data" from file, or
        /// throw Exception: example -
        ///  file not found,
        ///  file not serialisable,
        ///  file image data invalid (size or data format).
        /// </summary>
        /// <exception cref="Exception">When file is not found, or cannot be deserialized</exception>
        /// <param name="filename"></param>
        //======================================================================
        public void Load(string filename)
        {
            try
            {
                string filePath = Path.Combine(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath, filename);
                var deserializer = new XmlSerializer(typeof(ScreenCorrectionData));
                using (TextReader reader = new StreamReader(filePath))
                {
                    CalibrationCorrection = (ScreenCorrectionData)deserializer.Deserialize(reader);
                }

                int width = CalibrationCorrection.ScreenSize.Width;
                int height = CalibrationCorrection.ScreenSize.Height;

                //var settings = Service.Services.Instance.FileService.CalibrationSettings;
                //if (settings.ScreenSizePix.Width != width || settings.ScreenSizePix.Height != height)
                //    throw new InvalidOperationException("Correction screen size and settings screen size are different !!");

                byte[] byteArray = Convert.FromBase64String(CalibrationCorrection.CorrectedImage);
                var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                var bmpData = bmp.LockBits(
                                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(byteArray, 0, bmpData.Scan0, byteArray.Length);
                bmp.UnlockBits(bmpData);
                CorrectedImage = bmp;
            }
            catch (FileNotFoundException ex)
            {
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Warning($"Screen Calibration : {filename} file is missing.");
                throw ex;
            }
            catch (Exception ex)
            {
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Failed to load the Screen Calibration : {filename}. " + ex.Message);
                throw ex; //EC//20240208 Fix: rethrow so caller knows "ScreenCorrections data" not valid
            }
        }
    }
}
