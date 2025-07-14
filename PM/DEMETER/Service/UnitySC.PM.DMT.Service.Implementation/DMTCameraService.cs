using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

using Size = System.Drawing.Size;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTCameraService : CameraService, IDMTCameraService
    {
        private readonly DMTCameraManager _cameraManager;

        //=================================================================

        #region Construction

        //=================================================================
        public DMTCameraService(
            DMTCameraManager cameraManager, DMTHardwareManager hardwareManager, ILogger logger) : base(logger)
        {
            _cameraManager = cameraManager;
            HardwareManager = hardwareManager;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _cameraManager.Dispose();
        }

        #endregion Construction

        //=================================================================

        #region Implémentation de IDMTCameraService

        //=================================================================

        public Response<List<Side>> GetCamerasSides()
        {
            return InvokeDataResponse(messageContainer => _cameraManager.GetCamerasSides());
        }

        public Response<CameraInfo> GetCameraInfoBySide(Side side)
        {
            return InvokeDataResponse(messageContainer => _cameraManager.GetCameraInfoBySide(side));
        }

        public Response<Size> GetCalibratedImageSize(Side side)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetCalibratedImageSize(side));
        }

        public Response<ServiceImage> GetCameraImageBySide(Side side)
        {
            return InvokeDataResponse(messageContainer => _cameraManager.GetCameraImageBySide(side));
        }

        public Response<ServiceImage> GetCalibratedImage(Side side)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetCalibratedImage(side));
        }

        public Response<ServiceImageWithFocus> GetImageWithFocus(
            Side side, double scale, int waferSize, int patternSize)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetImageWithFocus(side, scale, waferSize, patternSize));
        }

        public Response<VoidResult> SetStatisticRoi(Side side, ROI statisticRoi)
        {
            return InvokeVoidResponse(messageContainer => _cameraManager.SetStatisticRoi(side, statisticRoi));
        }

        public Response<ServiceImageWithStatistics> GetRawImageWithStatistics(
            Side side, Int32Rect acquisitionRoi, double scale, ROI statisticRoi)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetImageWithStatistics(side, acquisitionRoi, scale,
                                                                                statisticRoi, false));
        }

        public Response<ServiceImageWithStatistics> GetCalibratedImageWithStatistics(
            Side side, Int32Rect acquisitionRoi, double scale, ROI statisticRoi)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetImageWithStatistics(side, acquisitionRoi, scale,
                                                                                statisticRoi));
        }

        public Response<Rect> CalibratedImageToMicrons(Side side, Rect pixelRect)
        {
            return InvokeDataResponse(messagesContainer => _cameraManager.CalibratedImageToMicrons(side, pixelRect));
        }

        public Response<Rect> MicronsToCalibratedImage(Side side, Rect micronRect)
        {
            return InvokeDataResponse(messagesContainer => _cameraManager.MicronsToCalibratedImage(side, micronRect));
        }

        // exposureTimeMs in milliseconds
        public Response<VoidResult> SetExposureTimeForSide(
            Side side, double exposureTimeMs, string screenId = null, int period = 0)
        {
            return InvokeVoidResponse(messagesContainer =>
                                          _cameraManager.SetExposureTimeForSide(side, exposureTimeMs, screenId, period));
        }

        public Response<double> GetGain(Side side)
        {
            return InvokeDataResponse(messagesContainer => _cameraManager.GetGain(side));
        }

        public Response<VoidResult> SetGain(Side side, double gain)
        {
            return InvokeVoidResponse(messagesContainer =>
                                          _cameraManager.SetGain(side, gain));
        }

        public Response<double> GetCameraFrameRateBySide(Side side)
        {
            return InvokeDataResponse(messageContainer =>
                                          _cameraManager.GetCameraFrameRate(side));
        }

        public Response<VoidResult> TrigAndWait(Side side)
        {
            return InvokeVoidResponse(messagesContainer =>
                                          _cameraManager.TrigAndWait(side));
        }

        public Response<OpticalMountShape> GetOpticalMountShape(Side side)
        {
            return InvokeDataResponse(messagesContainer =>
                                          _cameraManager.GetOpticalMountShape(side));
        }

        public Response<VoidResult> StartContinuousAcquisition(Side side)
        {
            return InvokeVoidResponse(messageContainer => _cameraManager.StartContinuousAcquisition(side));
        }

        public Response<VoidResult> StopContinuousAcquisition(Side side)
        {
            return InvokeVoidResponse(messageContainer => _cameraManager.StopContinuousAcquisition(side));
        }

        #endregion Implémentation de IDMTCameraService
    }
}
