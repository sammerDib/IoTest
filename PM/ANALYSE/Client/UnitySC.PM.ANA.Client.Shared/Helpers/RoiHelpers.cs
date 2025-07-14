using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Shared.Helpers
{
    public static class RoiHelpers
    {
        // roiRect in pixels
        static public CenteredRegionOfInterest GetCenteredRegionOfInterest(Size roiSize)
        {
            if (roiSize==Size.Empty) 
                return null;

            CenteredRegionOfInterest newRegionOfInterest = null;
            if ((roiSize.Width != ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width) || (roiSize.Height != ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height))
            {
                var pixelSizeX = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
                var pixelSizeY = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters;

                var roiWidth = (roiSize.Width * pixelSizeX).Millimeters();
                var roiHeight = (roiSize.Height * pixelSizeY).Millimeters();

                newRegionOfInterest = new CenteredRegionOfInterest(roiWidth, roiHeight);
            }
            return newRegionOfInterest;
        }
        // roiRect in pixels
        static public RegionOfInterest GetRegionOfInterest(Rect roiRect, bool isCenteredRoi)
        {
            RegionOfInterest newRegionOfInterest = null;
            if ((roiRect.Width != ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width) || (roiRect.Height != ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height))
            {
                var pixelSizeX = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
                var pixelSizeY = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters;

                var roiWidth = (roiRect.Width * pixelSizeX).Millimeters();
                var roiHeight = (roiRect.Height * pixelSizeY).Millimeters();

                Length roiX;
                Length roiY;
                if (isCenteredRoi)
                {
                    roiX = ((ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width - roiRect.Width) * pixelSizeX).Millimeters() / 2;
                    roiY = ((ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height - roiRect.Height) * pixelSizeX).Millimeters() / 2;
                }
                else
                {
                    roiX = (roiRect.Left * pixelSizeX).Millimeters();
                    roiY = (roiRect.Top * pixelSizeX).Millimeters();
                }

                newRegionOfInterest = new RegionOfInterest(roiX, roiY, roiWidth, roiHeight);
            }
            return newRegionOfInterest;
        }

        static public Size GetSizeInPixels(CenteredRegionOfInterest roi, string objectiveID)
        {
            Size roiSize = new Size();

            var pixelSizeX = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(objectiveID).Image.PixelSizeX.Millimeters;
            var pixelSizeY = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(objectiveID).Image.PixelSizeY.Millimeters;

            if ((pixelSizeX == 0) || (pixelSizeY == 0))
                return roiSize;

            roiSize.Width = roi.Width.Millimeters / pixelSizeX;
            roiSize.Height = roi.Height.Millimeters / pixelSizeY;

            return roiSize;
        }

        static public Rect GetRectInPixels(RegionOfInterest roi)
        {
            Rect roiRect = new Rect();

            var pixelSizeX = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
            var pixelSizeY = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters;

            if ((pixelSizeX == 0) || (pixelSizeY == 0))
                return roiRect;

            roiRect.X = roi.X.Millimeters / pixelSizeX;
            roiRect.Y = roi.Y.Millimeters / pixelSizeY;
            roiRect.Width = roi.Width.Millimeters / pixelSizeX;
            roiRect.Height = roi.Height.Millimeters / pixelSizeY;

            return roiRect;
        }
    }
}
