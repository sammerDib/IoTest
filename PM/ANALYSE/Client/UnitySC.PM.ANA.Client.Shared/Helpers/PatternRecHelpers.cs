using System;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Context;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Shared.Helpers
{
    public static class PatternRecHelpers
    {
        public static PatternRecognitionDataWithContext CreatePatternRectWithContext(Rect roiRect, bool isCenteredRoi)
        {
            var newPatternRecImage = new PatternRecognitionDataWithContext();

            // Acquire the image
            var serviceImage = ServiceLocator.CamerasSupervisor.GetCameraImage(ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID)?.Result;
            if (serviceImage == null)
            {
                throw new Exception("Impossible to get Camera image");
            }

            newPatternRecImage.PatternReference = serviceImage.ToExternalImage();
            newPatternRecImage.RegionOfInterest = RoiHelpers.GetRegionOfInterest(roiRect, isCenteredRoi);
            newPatternRecImage.Gamma = 0.3;
            newPatternRecImage.CameraId = ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID;

            if ( ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.ModulePosition == ModulePositions.Up)
                newPatternRecImage.Context = ClassLocator.Default.GetInstance<ContextSupervisor>().GetTopImageAcquisitionContext()?.Result;
           else
                newPatternRecImage.Context = ClassLocator.Default.GetInstance<ContextSupervisor>().GetBottomImageAcquisitionContext()?.Result;
            return newPatternRecImage;
        }

        public static PositionWithPatternRec CreatePositionWithTopPatternRec(Rect roiRect, bool isCenteredROI)
        {
            var newPatternRecImage = new PositionWithPatternRec();

            newPatternRecImage.PatternRec = CreatePatternRecWithTopCamera(roiRect, isCenteredROI);

            newPatternRecImage.Position = ServiceLocator.AxesSupervisor.AxesVM.Position.ToAxesPosition() as XYZTopZBottomPosition;
            newPatternRecImage.Context = ClassLocator.Default.GetInstance<ContextSupervisor>().GetTopImageAcquisitionContext()?.Result;
            return newPatternRecImage;
        }

        public static PositionWithPatternRec CreatePositionWithBottomPatternRec(Rect roiRect, bool isCenteredROI)
        {
            var newPatternRecImage = new PositionWithPatternRec();

            newPatternRecImage.PatternRec = CreatePatternRecWithBottomCamera(roiRect, isCenteredROI);

            newPatternRecImage.Position = ServiceLocator.AxesSupervisor.AxesVM.Position.ToAxesPosition() as XYZTopZBottomPosition;
            newPatternRecImage.Context = ClassLocator.Default.GetInstance<ContextSupervisor>().GetBottomImageAcquisitionContext()?.Result;
            return newPatternRecImage;
        }
        public static PatternRecognitionData CreatePatternRecWithMainCamera(Rect roiRect, bool isCenteredROI)
        {
            var cameraId = ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID;

            return CreatePatternRec(roiRect, isCenteredROI, cameraId);
        }

        public static PatternRecognitionData CreatePatternRecWithTopCamera(Rect roiRect, bool isCenteredROI)
        {
            var cameraId = ServiceLocator.CamerasSupervisor.TopCamera.Configuration.DeviceID;

            return CreatePatternRec(roiRect, isCenteredROI, cameraId);
        }

        public static PatternRecognitionData CreatePatternRecWithBottomCamera(Rect roiRect, bool isCenteredROI)
        {
            var cameraId = ServiceLocator.CamerasSupervisor.BottomCamera.Configuration.DeviceID;

            return CreatePatternRec(roiRect, isCenteredROI, cameraId);
        }

        private static PatternRecognitionData CreatePatternRec(Rect roiRect, bool isCenteredROI, string cameraId)
        {
            var newPatternRec = new PatternRecognitionData();

            // Acquire the image
            var serviceImage = ServiceLocator.CamerasSupervisor.GetCameraImage(cameraId)?.Result;
            if (serviceImage == null)
            {
                throw new Exception("Impossible to get Camera image");
            }

            newPatternRec.PatternReference = serviceImage.ToExternalImage();
            newPatternRec.RegionOfInterest = RoiHelpers.GetRegionOfInterest(roiRect, isCenteredROI);         
            newPatternRec.Gamma = 0.3;
            newPatternRec.CameraId = cameraId;

            return newPatternRec;
        }
    }
}
