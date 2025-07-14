using System;
using System.Linq;
using System.Text;
using System.Windows;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Service.DMTCalTransform.Ole;
using UnitySC.Shared.Image;
using UnitySC.Shared.LibMIL;

namespace UnitySC.PM.DMT.Service.DMTCalTransform
{
    public class DMTTransform : IDisposable
    {
        public MIL_ID MilSystem { get; private set; }
        public MIL_ID MilCalib { get; private set; }

        public int InputSizeX { get; private set; }
        public int InputSizeY { get; private set; }
        public int OutputSizeX { get; private set; }
        public int OutputSizeY { get; private set; }

        public double PixelSize { get; private set; }

        public double OffsetX_um { get; private set; }
        public double OffsetY_um { get; private set; }

        public double TopMargin_um { get; private set; }
        public double BottomMargin_um { get; private set; }
        public double RightMargin_um { get; private set; }
        public double LeftMargin_um { get; private set; }

        public double WaferRadius_um { get; private set; }

        public double Version {get; private set; }

        public DMTTransform(MIL_ID milSystem, string filename)
        {
            MilSystem = milSystem;

            LoadCalibrationFile(filename);
        }

        private DMTTransform()
        {
        }

        ~DMTTransform()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (MilCalib != MIL.M_NULL)
            {
                MIL.McalFree(MilCalib);
                MilCalib = MIL.M_NULL;
            }
        }

        protected void LoadCalibrationFile(string filename)
        {
            // On nettoye la précédente si il ya lieu
            if (MilCalib != MIL.M_NULL)
            {
                MIL.McalFree(MilCalib);
                MilCalib = MIL.M_NULL;
            }

            PixelSize = double.NaN;
            OffsetX_um = double.NaN;
            OffsetY_um = double.NaN;
            WaferRadius_um = double.NaN;
            Version = 0.0;
            try
            {
                using (OleStorage storage = OleStorage.CreateInstance(filename))
                {
                    // [ROOT]
                    // ->Version
                    // ->Date
                    // ->Settings
                    // ->MilCalibObj

                    int nLength = 0;
                    byte[] strbuff = null;

                    /*********************** Version  ********************************/
                    using (OleStream oStream = storage.OpenStream("Version"))
                    {
                        nLength = oStream.ReadInt();
                        strbuff = oStream.ReadBuffer(nLength);
                        string sCurrentVersion = ASCIIEncoding.ASCII.GetString(strbuff);
                        Version = double.Parse(sCurrentVersion, System.Globalization.CultureInfo.InvariantCulture);
                        oStream.Close();
                    }

                    /*********************** Date  ********************************/
                    using (OleStream oStream = storage.OpenStream("Date"))
                    {
                        nLength = oStream.ReadInt();
                        strbuff = oStream.ReadBuffer(nLength);
                        string sDate = ASCIIEncoding.ASCII.GetString(strbuff);
                        oStream.Close();
                    }

                    /*********************** Settings  ********************************/
                    using (OleStream oStream = storage.OpenStream("Settings"))
                    {
                        InputSizeX = oStream.ReadInt();
                        InputSizeY = oStream.ReadInt();
                        OutputSizeX = oStream.ReadInt();
                        OutputSizeY = oStream.ReadInt();

                        // Not Used in transformed
                        PixelSize = oStream.ReadDouble();
                        TopMargin_um = oStream.ReadDouble();
                        BottomMargin_um = oStream.ReadDouble();
                        RightMargin_um = oStream.ReadDouble();
                        LeftMargin_um = oStream.ReadDouble();

                        if (Version >= 2.0)
                        {
                            double waferDiameter_um = oStream.ReadDouble();
                            WaferRadius_um = 0.5 * waferDiameter_um;
                        }

                        oStream.Close();
                    }

                    /*********************** MilCalibObj  ********************************/
                    using (OleStream oStream = storage.OpenStream("MilCalibObj"))
                    {
                        // get calibration object size
                        int nCalibObjSize = oStream.ReadInt(); // in bytes

                        // get buffer stream
                        byte[] bufcal = oStream.ReadBuffer(nCalibObjSize);
                        MIL_ID milCalib = MIL.M_NULL;
                        MIL.McalStream(bufcal, MilSystem, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milCalib, MIL.M_NULL);
                        MilCalib = milCalib;
                        if (MilCalib == MIL.M_NULL)
                            throw new Exception("Calibration File Load Error : Could not restore MIL Calibration object");
                        oStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string smsg = "Calibration File Load Error -" + ex.Message;
                throw new Exception("Failed to load calibration from file " + filename, ex);
            }

            if (Version >= 2.0)
            {
                OffsetX_um = -LeftMargin_um - WaferRadius_um;
                OffsetY_um = -(WaferRadius_um + TopMargin_um); //i.e la meme chose que  -(2.0 * WaferRadius_um + TopMargin_um) + WaferRadius_um;
            }
            else
            {
                double offsetX_um = 0.0;
                double offsetY_um = 0.0;
                //depracated since MIL10 bit need to used older psd transform files so make warning silent here
#pragma warning disable CS0612 // MIL.M_TRANSFORM_IMAGE_WORLD_POS_{} is obsolete
                MIL.McalInquire(MilCalib, MIL.M_TRANSFORM_IMAGE_WORLD_POS_X, ref offsetX_um);
                MIL.McalInquire(MilCalib, MIL.M_TRANSFORM_IMAGE_WORLD_POS_Y, ref offsetY_um);
#pragma warning restore CS0612 
                OffsetX_um = offsetX_um;
                OffsetY_um = offsetY_um;
                WaferRadius_um = (OffsetX_um - OffsetY_um) / 2;
            }           
        }

        public DMTTransform Clone()
        {
            if (MilSystem == 0)
            {
                return new DMTTransform();
            }
            MIL_INT bufferSize = 0;
            var calibId = MilCalib;
            MIL.McalStream(MIL.M_NULL, MIL.M_NULL, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref calibId, ref bufferSize);
            var calibBuffer = new byte[bufferSize];
            MIL.McalStream(calibBuffer, 0, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref calibId, MIL.M_NULL);
            MIL_ID newCalibId = MIL.M_NULL;
            MIL.McalStream(calibBuffer, MilSystem, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref newCalibId, MIL.M_NULL);
            if (newCalibId == MIL.M_NULL)
            {
                throw new Exception("Couldn't copy the calibration to a new id");
            }
            var newInstance = (DMTTransform)MemberwiseClone();
            newInstance.MilCalib = newCalibId;
            return newInstance;
        }

        /// <summary>
        /// Transform une image selon la calibration
        /// </summary>
        public MilImage Transform(MilImage milImage)
        {
            MilImage milCalibrated = new MilImage();
            milCalibrated.Alloc2d(milImage.OwnerSystem, OutputSizeX, OutputSizeY, milImage.Type, MIL.M_IMAGE + MIL.M_PROC);
            Transform(milImage, milCalibrated);
            return milCalibrated;
        }

        public USPImageMil Transform(USPImageMil procimage)
        {
            MilImage milImage = procimage.GetMilImage();

            MilImage milCalibrated = Transform(milImage);
            USPImageMil calibratedProcimage = new USPImageMil();
            calibratedProcimage.SetMilImage(milCalibrated);
            return calibratedProcimage;
        }

        /// <summary>
        /// this method expects input and output images to be already allocated
        /// </summary>
        public void Transform(MilImage milImage, MilImage milCalibrated)
        {
            // Sanity Check
            //.............
            if ((milImage.SizeX != InputSizeX) || (milImage.SizeY != InputSizeY))
                throw new ArgumentNullException("milImage", String.Format("Transform Exception : Input Image wrong size (expected: {0}x{1})(actual: {2}x{3})", InputSizeX, InputSizeY, milImage.SizeX, milImage.SizeY));
            if ((milCalibrated.SizeX != OutputSizeX) || (milCalibrated.SizeY != OutputSizeY))
                throw new ArgumentNullException("milCalibrated", String.Format("Transform Exception : Input Image wrong size (expected: {0}x{1})(actual: {2}x{3})", OutputSizeX, OutputSizeX, milCalibrated.SizeX, milCalibrated.SizeY));

            // Redresse l'image
            //.................

            if (Version >= 2.0)
            {
                // for MILX sp6 adaptation
                // world wafer origin is in middle of wafer
                MIL.McalUniform(milCalibrated,
                          OffsetX_um, // match old deprecated McalControl with M_TRANSFORM_IMAGE_WORLD_POS_X
                          OffsetY_um, // match old deprecated McalControl with M_TRANSFORM_IMAGE_WORLD_POS_Y1
                          PixelSize,  // match old deprecated McalControl with M_TRANSFORM_IMAGE_PIXEL_SIZE_X
                          PixelSize,  // match old deprecated McalControl with M_TRANSFORM_IMAGE_PIXEL_SIZE_Y
                          0.0, MIL.M_DEFAULT);
                MIL.McalTransformImage(milImage, milCalibrated, MilCalib, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_FULL_CORRECTION, MIL.M_WARP_IMAGE + MIL.M_USE_DESTINATION_CALIBRATION);
            }
            else
            {
                MIL.McalTransformImage(milImage, milCalibrated, MilCalib, MIL.M_BILINEAR + MIL.M_OVERSCAN_CLEAR, MIL.M_FULL_CORRECTION, MIL.M_WARP_IMAGE);
            }
          
            // Calibration object is dissociated from redressed image to avoid conflicts with future processes on this image.
            MIL.McalAssociate(MIL.M_NULL, milCalibrated, MIL.M_DEFAULT);
        }

        //=================================================================

        #region Changement de repère

        //=================================================================


        /// <summary>
        /// Transforme des coordonnées Wafer (µm, centre du wafer, Y vers le haut) en coordonnées dans l'image redressée (pixels).
        /// </summary>
        public Point MicronsToCalibratedImage(Point mp)
        {
            // En µm, repère en haut à gauche de l'image
            double x = (mp.X + LeftMargin_um + WaferRadius_um) / PixelSize;
            double y = (mp.Y + TopMargin_um + WaferRadius_um) / PixelSize;
            return new Point(x, y);
        }

        /// <summary>
        /// Transforme des coordonnées Wafer (µm, centre du wafer, Y vers le haut) en coordonnées dans l'image redressée (pixels).
        /// </summary>
        public Rect MicronsToCalibratedImage(Rect micronRect)
        {
            var topLeft = MicronsToCalibratedImage(micronRect.TopLeft);
            var bottomRight = MicronsToCalibratedImage(micronRect.BottomRight);

            return new Rect(topLeft, bottomRight);
        }

        /// <summary>
        /// Transforme des coordonnées dans l'image redressée (pixels) en coordonnées Wafer (µm, centre du wafer, Y vers le haut).
        /// </summary>
        public Point CalibratedImageToMicrons(Point ip)
        {

            // En µm, repère au centre du wafer
            double xwafer = ip.X * PixelSize - LeftMargin_um - WaferRadius_um;
            double ywafer = ip.Y * PixelSize - TopMargin_um - WaferRadius_um;

            return new Point(xwafer, ywafer);
        }

        public Point CameraToMicrons(Point ip)
        {
            double x = double.NaN, y = double.NaN;
            MIL.McalTransformCoordinate(MilCalib, MIL.M_PIXEL_TO_WORLD, ip.X, ip.Y, ref x, ref y);
            //double xwafer = x - LeftMargin_um - WaferRadius_um;
            //double ywafer = TopMargin_um + WaferRadius_um - y;

            Point wp = new Point();
            wp.X = x - WaferRadius_um;
            wp.Y = -WaferRadius_um - y;

            //var pxelRect=MicronsToCamera(wp);

            return wp;

            //return new Point(xwafer, ywafer);

            //var topLeft = CalibratedImageToMicrons(new Point(0, 0));
            //var topLeftCamera=MicronsToCamera(topLeft);

            //var lefPix = ip.X - topLeftCamera.X;
            //var topPix = ip.Y - topLeftCamera.Y;

            //// En µm, repère en haut à gauche de l'image (World)
            //double xworld = lefPix * PixelSize;
            //double yworld = topPix * PixelSize;

            //// En µm, repère au centre du wafer
            //double xwafer = xworld - LeftMargin_um - WaferRadius_um;
            //double ywafer = TopMargin_um + WaferRadius_um - yworld;

            //return new Point(xwafer, ywafer);
        }

        /// <summary>
        /// Transforme des coordonnées dans l'image redressée (pixels) en coordonnées Wafer (µm, centre du wafer, Y vers le haut).
        /// </summary>
        public Rect CalibratedImageToMicrons(Rect pixelRect)
        {
            var topLeft = CalibratedImageToMicrons(pixelRect.TopLeft);
            var bottomRight = CalibratedImageToMicrons(pixelRect.BottomRight);
            return new Rect(topLeft, bottomRight);
        }

        /// <summary>
        /// Transforme des coordonnées Wafer (µm, centre du wafer, Y vers le haut) en coordonnées pixel caméra (image avant correction).
        /// </summary>
        public Point MicronsToCamera(Point mp)
        {

            double x = double.NaN, y = double.NaN;
            MIL.McalTransformCoordinate(MilCalib, MIL.M_WORLD_TO_PIXEL, mp.X, mp.Y, ref x, ref y);

            return new Point(x, y);
        }

        /// <summary>
        /// Transforme des coordonnées Wafer (µm, centre du wafer, Y vers le haut) en coordonnées pixel caméra (image avant correction).
        /// </summary>
        public Rect MicronsToCamera(Rect micronRect)
        {
            // à cause de la rotation, il ne faut pas se contenter de deux coins

            Point[] corners = new Point[4];
            corners[0] = MicronsToCamera(micronRect.TopLeft);
            corners[1] = MicronsToCamera(micronRect.TopRight);
            corners[2] = MicronsToCamera(micronRect.BottomLeft);
            corners[3] = MicronsToCamera(micronRect.BottomRight);

            double x = corners.Min(c => c.X);
            double y = corners.Min(c => c.Y);
            Point topleft = new Point(x, y);

            double x2 = corners.Max(c => c.X);
            double y2 = corners.Max(c => c.Y);
            Point bottomright = new Point(x2, y2);

            Rect pixelRect = new Rect(topleft, bottomright);
            return pixelRect;
        }

        #endregion Changement de repère
    }
}
