using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;

namespace UnitySC.PM.DMT.Service.Implementation.Extensions
{
    public static class USPImageMilExt
    {
        // Generate an image with a cross
        public static void CreateCrossImage(this USPImageMil procimg, int imageWidth, int imageHeight,
            Color backgroundColor, Color crossColor, int thickness, double centerX, double centerY)
        {
            if (procimg.IsMilSimulated)
            {
                return;
            }

            MilImage milImage = procimg.GetMilImage();
            milImage.AllocColor(Mil.Instance.HostSystem, 3, imageWidth, imageHeight, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
            milImage.Clear(MIL.M_RGB888(backgroundColor.R, backgroundColor.G, backgroundColor.B));
            milImage.PutHorizontalLine((int)Math.Round(centerY - thickness / 2), crossColor, thickness);
            milImage.PutVerticalLine((int)Math.Round(centerX - thickness / 2), crossColor, thickness);
        }

        public static void ComputeMask(this USPImageMil procimg, ROI roi, DMTTransform calib, bool ignorePerspectiveCalibration = false)
        {
            if (procimg.IsMilSimulated)
            {
                return;
            }

            using (var milGC = new MilGraphicsContext())
            {
                var milMask = procimg.GetMilImage();
                milMask.Clear();
                milGC.Alloc(Mil.Instance.HostSystem);
                milGC.Image = milMask;
                milGC.Color = 255;
                if (calib is null || ignorePerspectiveCalibration)
                {
                    MIL.MgraControl(milGC, MIL.M_INPUT_UNITS, MIL.M_PIXEL);
                }
                else
                {
                    MIL.MgraControl(milGC, MIL.M_GRAPHIC_SOURCE_CALIBRATION, calib.MilCalib);
                    MIL.MgraControl(milGC, MIL.M_INPUT_UNITS, MIL.M_WORLD);
                    MIL.MgraControl(milGC, MIL.M_GRAPHIC_CONVERSION_MODE, MIL.M_RESHAPE_FOLLOWING_DISTORTION);
                }

                switch (roi.RoiType)
                {
                    case RoiType.WholeWafer:
                        DrawWholeWaferMask(roi, calib, milGC);
                        break;

                    case RoiType.Rectangular:
                        DrawRectangularMask(roi, milGC);
                        break;

                    default:
                        throw new ApplicationException("unknown RoiType: " + roi.RoiType);
                }
            }
        }

        private static void DrawRectangularMask(ROI roi, MilGraphicsContext milGC)
        {

            milGC.RectFill((int)roi.Rect.X, (int)roi.Rect.Y, (int)(roi.Rect.X + roi.Rect.Width),
                (int)(roi.Rect.Y + roi.Rect.Height));
        }

        private static void DrawWholeWaferMask(ROI roi, DMTTransform calib, MilGraphicsContext milGC)
        {
            if (calib == null)
            {
                throw new Exception("Perspective calibration is missing, cannot apply whole wafer mask");
            }
            double radius = calib.WaferRadius_um - roi.EdgeExclusion;
            milGC.ArcFill(0, 0, radius, radius);
        }

        public static double ComputeNbSaturatedPixels(this USPImageMil procimg)
        {
            if (procimg.IsMilSimulated)
            {
                return 0;
            }

            using (MilImageResult milStat = new MilImageResult())
            {
                milStat.AllocResult(procimg.GetMilImage().OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(procimg.GetMilImage(), MilTo.StatList(MIL.M_STAT_NUMBER), MIL.M_EQUAL, 255, 0);
                var nbSaturated = milStat.GetResult(MIL.M_STAT_NUMBER);
                return nbSaturated;
            }
        }

        public static void ApplyDeadPixelsCorrection(this USPImageMil procimg, List<DeadPixel> blackDeadPixels, List<DeadPixel> whiteDeadPixels)
        {
            foreach (var DP in blackDeadPixels)
            {
                if (DP.X < procimg.Width && DP.Y < procimg.Height && DP.X >= 0 && DP.Y >= 0)
                {
                    EightbitsBufferDeadPixelCorrector(procimg, DP);
                }
            }

            foreach (var DP in whiteDeadPixels)
            {
                if (DP.X < procimg.Width && DP.Y < procimg.Height && DP.X >= 0 && DP.Y >= 0)
                {
                    EightbitsBufferDeadPixelCorrector(procimg, DP);
                }
            }
        }

        private static void EightbitsBufferDeadPixelCorrector(USPImageMil image, DeadPixel DP)
        {
            var milImageId = image.GetMilImage().MilId;
            if (DP.X > image.Width - 2 || DP.Y > image.Height - 2 || DP.X < 1 || DP.Y < 1)
            {
                return;
            }

            byte[] data = new byte[9];
            MIL.MbufGet2d(milImageId, DP.X - 1, DP.Y - 1, 3, 3, data);
            float accumulatedValue = 0;
            foreach (byte B in data)
            {
                accumulatedValue += B;
            }

            accumulatedValue = (accumulatedValue - data[(data.Length - 1) / 2]) / (data.Length - 1);
            data[(data.Length - 1) / 2] = (byte)accumulatedValue;
            MIL.MbufPut2d(milImageId, DP.X - 1, DP.Y - 1, 3, 3, data);
        }

        public static ServiceImageWithStatistics ComputeStatisticsAndProfile(this USPImageMil procimg, ServiceImageWithStatistics svcImg, USPImageMil mask, Rect pixelRect)
        {
            if (procimg.IsMilSimulated)
            {
                svcImg.Min = 5;
                svcImg.Max = 250;
                svcImg.Mean = 100;
                svcImg.StandardDeviation = 10;
                svcImg.Histogram = new long[255];
                svcImg.Profile = new long[255];

                for (int i = 0; i < 255; i++)
                {
                    svcImg.Histogram[i] = i;
                    svcImg.Profile[i] = i;
                }
                return svcImg;
            }

            // Apply mask
            procimg.GetMilImage().SetRegion(mask.GetMilImage(), MIL.M_DEFAULT, MIL.M_RASTERIZE);

            procimg.ComputeStatistics(svcImg);
            procimg.ComputeProfile(svcImg, pixelRect);

            // Remove mask
            procimg.GetMilImage().SetRegion(MIL.M_NULL, MIL.M_DEFAULT, MIL.M_DELETE);

            return svcImg;
        }

        private static void ComputeProfile(this USPImageMil procimg, ServiceImageWithStatistics svcimg, Rect r)
        {
            if (svcimg.Type != ServiceImage.ImageType.Greyscale)
            {
                throw new ApplicationException("Profile can be computed only on greyscale images");
            }

            var milCamImage = procimg.GetMilImage();

            using (var milChild = new MilImage())
            {
                using (var milResult = new MilImageResult())
                {
                    try
                    {
                        milChild.Child2d(milCamImage, (int)r.X, (int)r.Y, (int)r.Width,
                                         (int)r
                                             .Height); // NB: l'image child hérite de la région (MbufSetRegion) du parent

                        milResult.AllocResult(milChild.OwnerSystem, milChild.SizeX, MIL.M_PROJ_LIST);
                        milResult.Projection(milChild, 0, MIL.M_MEAN);
                        milResult.GetResult(MIL.M_VALUE, out svcimg.Profile);
                    }
                    catch (Exception)
                    {
                        // An exception can be raised when the roi is over a zone that doesn't contain data. It happens when the not calibrated image is not large enough
                    }
                }
            }
        }

        private static void ComputeStatistics(this USPImageMil procimg, ServiceImageWithStatistics svcimg)
        {
            if (svcimg.Type != ServiceImage.ImageType.Greyscale)
            {
                throw new ApplicationException("Statistics can be computed only on greyscale images");
            }

            var milImage = procimg.GetMilImage();

            using (var milStat = new MilImageResult())
            {
                milStat.AllocResult(milImage.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(milImage, MIL.M_STAT_MIN, MIL.M_STAT_MAX, MIL.M_STAT_MEAN, MIL.M_STAT_STANDARD_DEVIATION);
                svcimg.Min = milStat.GetResult(MIL.M_STAT_MIN);
                svcimg.Max = milStat.GetResult(MIL.M_STAT_MAX);
                svcimg.Mean = milStat.GetResult(MIL.M_STAT_MEAN);
                svcimg.StandardDeviation = milStat.GetResult(MIL.M_STAT_STANDARD_DEVIATION);

                svcimg.Histogram = MilImageResult.Histogram(milImage, 256);
            }
        }

        public static USPImageMil CreateThumbnail(this USPImageMil procimg)
        {
            // le cas des floating images are not thumbnailmling here using color map to match final display.. to improve later
            var milImage = procimg.GetMilImage();
            int height = 256; // Note de rti ; thumbnail use @ 256 within viewer
            int width = milImage.SizeX * height / milImage.SizeY;
            var thumb = new USPImageMil();
            var thumbMilImg = thumb.GetMilImage();
            thumbMilImg.Alloc2d(width, height, milImage.Type, milImage.Attribute);
            MilImage.Resize(milImage, thumbMilImg, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);

            return thumb;
        }
    }
}
