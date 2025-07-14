using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.ClientProxy.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Shared.UI.Proxy
{
    /// <summary>
    /// Proxy to supervise hardware
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CameraSupervisor : ICameraServiceCallback
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly DuplexServiceInvoker<IDMTCameraService> _cameraService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CameraSupervisor(ILogger<CameraSupervisor> logger, ILogger<IDMTCameraService> cameraServiceLogger,
            IMessenger messenger, IDialogOwnerService dialogService)
        {
            var instanceContext = new InstanceContext(this);
            _cameraService = new DuplexServiceInvoker<IDMTCameraService>(instanceContext, "DEMETERCameraService",
                cameraServiceLogger, messenger);
            _logger = logger;
            _messenger = messenger;
        }

        public void Subscribe()
        {
            _cameraService.Invoke(s => s.Subscribe(Int32Rect.Empty, 1 / 16.0));
        }

        public void Unsubscribe()
        {
            _cameraService.Invoke(s => s.Unsubscribe());
        }

        void ICameraServiceCallback.ImageGrabbedCallback(string side, ServiceImage image)
        {
            _logger.Debug("Image grabbed");
            _messenger.Send(new ImageGrabbedMessage() { ServiceImage = image });
        }

        public List<Side> GetCameraSides()
        {
            return _cameraService.Invoke(s => s.GetCamerasSides());
        }

        // exposureTime in seconds
        public void SetExposureTime(Side side, double exposureTimeMs, string screenId = null, int period = 0)
        {
            _cameraService.Invoke(s => s.SetExposureTimeForSide(side, exposureTimeMs, screenId, period));
        }

        public void SetGain(Side side, double gain)
        {
            _cameraService.Invoke(s => s.SetGain(side, gain));
        }

        public double GetGain(Side side)
        {
            return _cameraService.Invoke(s => s.GetGain(side));
        }

        public ServiceImage GetCameraImage(Side side)
        {
            return _cameraService.Invoke(s => s.GetCameraImageBySide(side));
        }

        public ServiceImageWithStatistics GetCalibratedImageWithStatistics(Side side, Int32Rect acquisitionRoi, double scale, ROI statisticRoi)
        {
            return _cameraService.Invoke(s => s.GetCalibratedImageWithStatistics(side, acquisitionRoi, scale, statisticRoi));
        }

        public ServiceImageWithStatistics GetRawImageWithStatistics(Side side, Int32Rect acquisitionRoi, double scale, ROI statisticRoi)
        {
            return _cameraService.Invoke(s => s.GetRawImageWithStatistics(side, acquisitionRoi, scale, statisticRoi));
        }

        public double GetCameraFrameRate(Side side)
        {
            return _cameraService.Invoke(s => s.GetCameraFrameRateBySide(side));
        }

        public ServiceImageWithFocus GetImageWithFocus(Side side, double scale, int waferSize, int patternSize)
        {
            return _cameraService.Invoke(s => s.GetImageWithFocus(side, scale, waferSize, patternSize));
        }

        public CameraInfo GetCameraInfo(Side side)
        {
            return _cameraService.Invoke(s => s.GetCameraInfoBySide(side));
        }

        public System.Drawing.Size GetCalibratedImageSize(Side side)
        {
            return _cameraService.Invoke(s => s.GetCalibratedImageSize(side));
        }

        public Rect CalibratedImageToMicrons(Side side, Rect pixelRect)
        {
            return _cameraService.Invoke(s => s.CalibratedImageToMicrons(side, pixelRect));
        }

        public Rect MicronsToCalibratedImage(Side side, Rect micronRect)
        {
            return _cameraService.Invoke(s => s.MicronsToCalibratedImage(side, micronRect));
        }

        public OpticalMountShape GetOpticalMountShape(Side side)
        {
            return _cameraService.Invoke(s => s.GetOpticalMountShape(side));
        }

        public void StartContinuousAcquisition(Side side)
        {
            _cameraService.Invoke(s => s.StartContinuousAcquisition(side));
        }

        public void StopContinuousAcquisition(Side side)
        {
            _cameraService.Invoke(s => s.StopContinuousAcquisition(side));
        }
    }
}
