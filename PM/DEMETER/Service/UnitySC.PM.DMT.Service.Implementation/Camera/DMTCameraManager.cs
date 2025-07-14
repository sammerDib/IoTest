using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper.Internal;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Extensions;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Implementation.Wrapper;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

using UnitySCSharedAlgosOpenCVWrapper;

using Size = System.Drawing.Size;

namespace UnitySC.PM.DMT.Service.Implementation.Camera
{
    public class DMTCameraManager : USPMilCameraManager, IDMTInternalCameraMethods
    {
        private const double CameraDeadPixelThresholdPercentage = 0.001;

        private readonly object _lock = new object();
        private readonly DMTHardwareManager _hardwareManager;
        private readonly AlgorithmManager _algorithmManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly FringeManager _fringeManager;
        private readonly ILogger<DMTCameraManager> _logger;
        private readonly Dictionary<Side, MaskRoi> _maskCache = new Dictionary<Side, MaskRoi>();
        private readonly Dictionary<Side, bool> _isCameraGrabbingBySide;
        private readonly IDMTServiceConfigurationManager _serviceConfigurationManager;
        private bool _areFlowsSimulated;
        private Dictionary<Side, Task> _grabbingTasksBySide;

        public DMTCameraManager(
            DMTHardwareManager hardwareManager, AlgorithmManager algorithmManager, CalibrationManager calibrationManager, IDMTServiceConfigurationManager serviceConfigurationManager, FringeManager fringeManager, ILogger<DMTCameraManager> logger)
        {
            _hardwareManager = hardwareManager;
            _algorithmManager = algorithmManager;
            _calibrationManager = calibrationManager;
            _logger = logger;
            _isCameraGrabbingBySide = new Dictionary<Side, bool>
            {
                { Side.Front, false },
                { Side.Back, false },
            };
            _grabbingTasksBySide = new Dictionary<Side, Task>
            {
                { Side.Front, null },
                { Side.Back, null }
            };
            _serviceConfigurationManager = serviceConfigurationManager;
            _areFlowsSimulated = _serviceConfigurationManager.FlowsAreSimulated;
            _fringeManager = fringeManager;
        }

        public USPImageMil CreateMask(Side side, ROI roi, bool ignorePerspectiveCalibration = false)
        {
            var camera = _hardwareManager.CamerasBySide[side];
            var sizeX = camera.Width;
            var sizeY = camera.Height;
            var procimg = new USPImageMil(sizeX, sizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
            ComputeMask(procimg, side, roi, ignorePerspectiveCalibration);
            return procimg;
        }

        public void Dispose()
        {
            foreach (var maskImage in _maskCache.Values.Select(mCache => mCache.Mask))
            {
                maskImage.Dispose();
            }
        }

        public USPImageMil GrabNextImage(CameraBase camera)
        {
            var grabbedImage = GetNextCameraImageUspImageMil(camera, true);
            return grabbedImage;
        }

        public void StartContinuousAcquisition(Side side)
        {
            if (!_isCameraGrabbingBySide[side] || (!(_grabbingTasksBySide[side] is null) && _grabbingTasksBySide[side].Status != TaskStatus.Running))
            {
                _isCameraGrabbingBySide[side] = true;
                // This is necessary to avoid a race condition
                TrigAndWait(side);
                _grabbingTasksBySide[side] = Task.Run(() =>
                {
                    while (_isCameraGrabbingBySide[side])
                    {
                        TrigAndWait(side);
                    }
                });
            }
        }

        public void StopContinuousAcquisition(Side side)
        {
            _isCameraGrabbingBySide[side] = false;
        }

        public double SetExposureTime(CameraBase camera, double exposureTimeMs, ScreenBase screen = null, int period = 0)
        {
            camera.SetExposureTimeMs(exposureTimeMs);
            return exposureTimeMs;
        }

        public List<Side> GetCamerasSides()
        {
            return _hardwareManager.CamerasBySide.Keys.ToList();
        }

        public CameraInfo GetCameraInfoBySide(Side side)
        {
            if (!_hardwareManager.CamerasBySide.TryGetValue(side, out var cam))
            {
                return null;
            }

            return new CameraInfo
            {
                Model = cam.Model,
                SerialNumber = cam.SerialNumber,
                Version = cam.Version,
                Width = cam.Width,
                Height = cam.Height,
                MinExposureTimeMs = cam.MinExposureTimeMs,
                MaxExposureTimeMs = cam.MaxExposureTimeMs,
                MinGain = cam.MinGain,
                MaxGain = cam.MaxGain,
                ColorModes = cam.ColorModes,
                MinFrameRate = cam.MinFrameRate,
                MaxFrameRate = cam.MaxFrameRate,
            };
        }

        public Size GetCalibratedImageSize(Side side)
        {
            var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
            if (!(calibration is null))
            {
                return new Size(calibration.OutputSizeX, calibration.OutputSizeY);
            }

            var camera = _hardwareManager.CamerasBySide[side];
            return new Size(camera.Width, camera.Height);
        }

        public ServiceImage GetCameraImageBySide(Side side)
        {
            lock (_lock)
            {
                var camera = _hardwareManager.CamerasBySide[side];

                var serviceImage = GrabNextImage(camera)?.ToServiceImage();
                if (serviceImage == null)
                {
                    return null;
                }
                serviceImage.IsSaturated = CheckSaturatedPixels(serviceImage);
                return serviceImage;
            }
        }

        public ServiceImage GetCalibratedImage(Side side)
        {
            lock (_lock)
            {
                var camera = _hardwareManager.CamerasBySide[side];
                var procImg = GrabNextImage(camera);
                ServiceImage svcImg;
                var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
                if (calibration is null)
                {
                    svcImg = procImg.ToServiceImage();
                }
                else
                {
                    using (var calibratedProcImage = calibration.Transform(procImg))
                    {
                        svcImg = calibratedProcImage.ToServiceImage();
                    }
                }

                svcImg.IsSaturated = CheckSaturatedPixels(svcImg);
                return svcImg;
            }
        }

        public ServiceImageWithFocus GetImageWithFocus(Side side, double scale, int waferSize, int patternSize)
        {
            lock (_lock)
            {
                var camera = _hardwareManager.CamerasBySide[side];
                ServiceImageWithFocus resultImage = null;
                var svcImg = GrabNextImage(camera)?.ToServiceImage();
                if (svcImg == null)
                {
                    return null;
                }

                var mountShape = _hardwareManager.OpticalMounts.GetOrDefault(side);
                int subImagesNumber;
                switch (mountShape)
                {
                    case OpticalMountShape.Cross:
                    case OpticalMountShape.SquarePlusCenter:
                        subImagesNumber = 5;
                        break;

                    default:
                        subImagesNumber = 3;
                        break;
                }

                var subImages = new SubImageProperties[subImagesNumber];
                var chrono = new Stopwatch();
                chrono.Start();

                int result = FocusExposureCalibrationWrapper.FocusQuality(svcImg.Data, (uint)svcImg.DataWidth,
                                                                          (uint)svcImg.DataHeight, waferSize,
                                                                          patternSize,
                                                                          mountShape, ref subImages);

                chrono.Stop();
                if (result != 0)
                {
                    _logger.Error($"The focus quality calculation failed, result = {result}");
                }
                else
                {
                    _logger.Information($"Focus computation time : {chrono.ElapsedMilliseconds}");
                }

                resultImage = new ServiceImageWithFocus(svcImg, subImages);

                return resultImage;
            }
        }

        public double GetCameraFrameRate(Side side)
        {
            return _hardwareManager.CamerasBySide[side].GetFrameRate();
        }

        public Rect CalibratedImageToMicrons(Side side, Rect pixelRect)
        {
            return _calibrationManager.GetPerspectiveCalibrationForSide(side)?.CalibratedImageToMicrons(pixelRect) ?? pixelRect;
        }

        public Rect MicronsToCalibratedImage(Side side, Rect micronRect)
        {
            return _calibrationManager.GetPerspectiveCalibrationForSide(side)?.MicronsToCalibratedImage(micronRect) ?? micronRect;
        }

        public void SetExposureTimeForSide(Side side, double exposureTimeMs, string screenId, int period)
        {
            var camera = _hardwareManager.CamerasBySide[side];
            ScreenBase screen = null;
            bool screenFound = !(screenId is null) && _hardwareManager.Screens.TryGetValue(screenId, out screen);
            if (!screenFound)
            {
                _hardwareManager.ScreensBySide.GetOrDefault(side);
            }

            SetExposureTime(camera, exposureTimeMs, screen, period);
        }

        public double GetGain(Side side)
        {
            return _hardwareManager.CamerasBySide[side].GetGain();
        }

        public void SetGain(Side side, double gain)
        {
            _hardwareManager.CamerasBySide[side].SetGain(gain);
        }

        public void TrigAndWait(Side side)
        {
            var chrono = new Stopwatch();
            chrono.Start();
            var camera = _hardwareManager.CamerasBySide[side];
            camera.SoftwareTrigger();
            camera.WaitForSoftwareTriggerGrabbed();
            chrono.Stop();
            _logger.Debug($"TrigAndWait : {chrono.ElapsedMilliseconds} ms");
        }

        public OpticalMountShape GetOpticalMountShape(Side side)
        {
            return _hardwareManager.OpticalMounts.GetOrDefault(side);
        }

        public ServiceImageWithDeadPixels GetImageWithDeadPixels(
            Side side, DeadPixelTypes deadPixelType, int pixelValueThreshold)
        {
            var camera = _hardwareManager.CamerasBySide[side];
            var applyDeadPixelCorrection = false;
            var serviceImage = GetNextCameraImageUspImageMil(camera, applyDeadPixelCorrection).ToServiceImage();
            return GetServiceImageWithDeadPixelsFromServiceImage(serviceImage, deadPixelType, pixelValueThreshold,
                                                                    camera.Height * camera.Width);
        }

        public void SetStatisticRoi(Side side, ROI statisticRoi)
        {
            MaskRoi cache;
            if (!_maskCache.TryGetValue(side, out cache))
            {
                var camera = _hardwareManager.CamerasBySide[side];
                cache = new MaskRoi(camera);
                _maskCache[side] = cache;
            }

            bool isCacheMaskValid = cache.Roi.RoiType == statisticRoi.RoiType && cache.Roi.Rect == statisticRoi.Rect &&
                    cache.Roi.EdgeExclusion == statisticRoi.EdgeExclusion;
            if (!isCacheMaskValid)
            {
                cache.Roi = statisticRoi;
                ComputeMask(cache.Mask, side, cache.Roi, false);
            }
        }

        public ServiceImageWithStatistics GetImageWithStatistics(
            Side side, Int32Rect acquisitionRoi, double scale, ROI statisticRoi, bool useCalibration = true)
        {
            SetStatisticRoi(side, statisticRoi);
            var mask = _maskCache[side].Mask;

            var camera = _hardwareManager.CamerasBySide[side];
            lock (_lock)
            {
                var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
                var pixelRect = calibration?.MicronsToCamera(statisticRoi.Rect) ?? statisticRoi.Rect;
                pixelRect.Width = Math.Max(pixelRect.Width, 1);
                pixelRect.Height = Math.Max(pixelRect.Height, 1);

                var procimg = GrabNextImage(camera);
                ServiceImageWithStatistics svcImg;
                if (useCalibration && !(calibration is null))
                {
                    using (var calibratedProcimage = calibration.Transform(procimg))
                    {
                        svcImg = (ServiceImageWithStatistics)calibratedProcimage.ToServiceImage(acquisitionRoi, scale);
                    }
                }
                else
                {
                    svcImg = (ServiceImageWithStatistics)procimg.ToServiceImage(acquisitionRoi, scale);
                }

                procimg.ComputeStatisticsAndProfile(svcImg, mask, pixelRect);
                svcImg.ImageId = LastImagesIds[camera];
                return svcImg;
            }
        }

        private bool CheckSaturatedPixels(USPImageMil image)
        {
            return CheckSaturatedPixels(image.ToServiceImage());
        }

        private bool CheckSaturatedPixels(ServiceImage image)
        {
            var nbPixelsSaturated = ComputeNbSaturatedPixels(image);
            var nbPixelsInImage = image.DataHeight * image.DataWidth;
            var saturationThresholdPercent = _algorithmManager.Config.Image.SaturationThresholdPercent;
            var percentageSaturatedPixels = nbPixelsSaturated * 100 / nbPixelsInImage;
            if (percentageSaturatedPixels <= saturationThresholdPercent)
            {
                return false;
            }

            _logger.Warning($"The image is saturated : the percentage of saturated pixels is {percentageSaturatedPixels}% which is over the threshold {saturationThresholdPercent}%");
            return true;
        }

        private void ComputeMask(USPImageMil mask, Side side, ROI roi, bool ignorePerspectiveCalibration = true)
        {
            var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
            mask.ComputeMask(roi, calibration, ignorePerspectiveCalibration);
        }

        private ServiceImageWithDeadPixels GetServiceImageWithDeadPixelsFromServiceImage(
            ServiceImage serviceImage, DeadPixelTypes deadPixelsType,
            int pixelValueThreshold, int numberCameraPixels)
        {
            var returnedServiceImage = new ServiceImageWithDeadPixels
            {
                Image = serviceImage,
                MaximumDeadPixelThreshold = (int)(CameraDeadPixelThresholdPercentage * numberCameraPixels),
                DeadPixelType = deadPixelsType,
                DeadPixels = new List<DeadPixel>()
            };
            var imageData = new ImageData(returnedServiceImage.Image.Data, returnedServiceImage.Image.DataWidth,
                                          returnedServiceImage.Image.DataHeight, ImageType.GRAYSCALE_Unsigned8bits);
            var thresholdType = deadPixelsType == DeadPixelTypes.WhitePixel ? ThresholdType.AboveOrEqualThreshold : ThresholdType.BelowOrEqualThreshold;
            var points = ImageOperators.FindPixelCoordinatesByThresholding(imageData, pixelValueThreshold, thresholdType);
            int resultCount = points.Length;
            if (resultCount > 0)
            {
                returnedServiceImage.NumberOfDeadPixelsFound = points.Length;
                if (resultCount > returnedServiceImage.MaximumDeadPixelThreshold)
                {
                    returnedServiceImage.CalibrationStatus = DeadPixelsCalibrationStatus.Failure;
                }
                else
                {
                    returnedServiceImage.DeadPixels = points.AsParallel()
                                                            .Select(point => new DeadPixel
                                                            {
                                                                X = point.X,
                                                                Y = point.Y,
                                                                Type = deadPixelsType
                                                            })
                                                            .ToList();
                    returnedServiceImage.CalibrationStatus = DeadPixelsCalibrationStatus.Success;
                }
            }

            return returnedServiceImage;
        }

        private double ComputeNbSaturatedPixels(ServiceImage image)
        {
            var imageDataType = ImageType.GRAYSCALE_Unsigned8bits;
            switch (image.Type)
            {
                case ServiceImage.ImageType.Greyscale:
                    imageDataType = ImageType.GRAYSCALE_Unsigned8bits;
                    break;

                case ServiceImage.ImageType.Greyscale16Bit:
                    imageDataType = ImageType.GRAYSCALE_Unsigned16bits;
                    break;

                case ServiceImage.ImageType.RGB:
                    imageDataType = ImageType.RGB_Unsigned8bits;
                    break;

                case ServiceImage.ImageType._3DA:
                    imageDataType = ImageType.GRAYSCALE_Float32bits;
                    break;
            }

            var imageData = new ImageData(image.Data, image.DataHeight, image.DataWidth, imageDataType);
            float[] histogram = ImageOperators.CalculateHistogram(imageData, 256);
            return histogram[255];
        }

        private void ApplyDeadPixelsCorrection(Side side, USPImageMil procimg)
        {
            DeadPixelsManager currentDeadPixelManager = _calibrationManager.GetDeadPixelsManagerBySide(side);
            if (currentDeadPixelManager is null)
            {
                _logger.Warning($"Dead pixel correction file for side {side} is missing");
                return;
            }

            procimg.ApplyDeadPixelsCorrection(currentDeadPixelManager.BlackDeadPixels, currentDeadPixelManager.WhiteDeadPixels);
        }

        private USPImageMil GetNextCameraImageUspImageMil(CameraBase camera, bool applyDeadPixelCorrection)
        {
            var cameraSide = _hardwareManager.CamerasBySide.FirstOrDefault(sideCameraPair => sideCameraPair.Value == camera).Key;
            if (!_isCameraGrabbingBySide[cameraSide])
            {
                TrigAndWait(cameraSide);
            }
            else
            {
                // To ensure we get another image
                Task.Delay(TimeSpan.FromMilliseconds(camera.GetExposureTimeMs())).Wait();
            }
            var cameraImage = GetLastCameraImage(camera);
            var uspImageMil = CameraImageToUspImageMil(cameraImage);
            if (applyDeadPixelCorrection)
            {
                ApplyDeadPixelsCorrection(cameraSide, uspImageMil);
            }
            return uspImageMil;
        }

        private USPImageMil CameraImageToUspImageMil(ICameraImage cameraImage)
        {
            if (cameraImage is null)
            {
                return null;
            }
            return cameraImage as USPImageMil ?? new USPImageMil(cameraImage.ToServiceImage());
        }

        private class MaskRoi
        {
            public ROI Roi;
            public readonly USPImageMil Mask;

            public MaskRoi(CameraBase camera)
            {
                Roi = new ROI();
                Roi.Rect = Rect.Empty;

                var sizeX = camera.Width;
                var sizeY = camera.Height;
                Mask = new USPImageMil(sizeX, sizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
            }
        }
    }
}
