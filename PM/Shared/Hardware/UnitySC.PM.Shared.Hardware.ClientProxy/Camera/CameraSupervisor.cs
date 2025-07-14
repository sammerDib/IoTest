using System;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Camera
{
    public class CameraSupervisor : ICameraService, ICameraServiceCallback
    {
        private readonly ILogger _logger;
        private readonly DuplexServiceInvoker<ICameraService> _cameraService;
        private readonly IDialogOwnerService _dialogService;
        private readonly IMessenger _messenger;

        /// <summary>
        /// Constructor
        /// </summary>
        public CameraSupervisor(ILogger<CameraSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _cameraService = new DuplexServiceInvoker<ICameraService>(new InstanceContext(this), "CameraService", ClassLocator.Default.GetInstance<SerilogLogger<ICameraService>>(), messenger);
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        void ICameraServiceCallback.ImageGrabbedCallback(string cameraId, ServiceImage image)
        {
            _logger.Debug("Image grabbed");
            _messenger.Send(new ImageGrabbedMessage() { CameraId = cameraId, ServiceImage = image });
        }

        /// <summary>
        /// Subscribe to hardware changes
        /// </summary>
        public void Subscribe()
        {
            _cameraService.Invoke(s => s.Subscribe(Int32Rect.Empty, 1));
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.Subscribe(acquisitionRoi, scale));
        }

        /// <summary>
        /// Unsubscribe to hardware changes
        /// </summary>
        public void Unsubscribe()
        {
            try
            {
                _cameraService.Invoke(s => s.Unsubscribe());
            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex, "Unsubscribe error");
            }
        }

        Response<VoidResult> ICameraService.Unsubscribe()
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.Unsubscribe());
        }

        public void StartAcquisition(string cameraId)
        {
            _cameraService.Invoke(s => s.StartAcquisition(cameraId));
        }

        Response<VoidResult> ICameraService.StartAcquisition(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.StartAcquisition(cameraId));
        }

        public void StopAcquisition(string cameraId)
        {
            _cameraService.Invoke(s => s.StopAcquisition(cameraId));
        }

        Response<VoidResult> ICameraService.StopAcquisition(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.StopAcquisition(cameraId));
        }

        public void SetCameraExposureTime(string cameraId, double exposureTimeMs)
        {
            _cameraService.Invoke(s => s.SetCameraExposureTime(cameraId, exposureTimeMs));
        }

        Response<VoidResult> ICameraService.SetCameraExposureTime(string cameraId, double exposureTimeMs)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.SetCameraExposureTime(cameraId, exposureTimeMs));
        }

        public ServiceImage GetCameraImage(string cameraId)
        {
            return _cameraService.Invoke(s => s.GetCameraImage(cameraId));
        }

        Response<ServiceImage> ICameraService.GetCameraImage(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetCameraImage(cameraId));
        }

        public CameraInfo GetCameraInfo(string cameraId)
        {
            return _cameraService.Invoke(s => s.GetCameraInfo(cameraId));
        }

        Response<CameraInfo> ICameraService.GetCameraInfo(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetCameraInfo(cameraId));
        }

        public double GetCameraFrameRate(string cameraId)
        {
            return _cameraService.Invoke(s => s.GetCameraFrameRate(cameraId));
        }

        Response<double> ICameraService.GetCameraFrameRate(string cameraId)
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraFrameRate(cameraId));
        }
    }
}
