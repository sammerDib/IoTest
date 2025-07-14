using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Camera
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CameraSupervisorEx : ICameraServiceEx, ICameraServiceCallbackEx
    {
        private readonly DuplexServiceInvoker<ICameraServiceEx> _cameraService;
        private readonly IMessenger _messenger;

        public CameraSupervisorEx(SerilogLogger<ICameraServiceEx> logger, IMessenger messenger)
        {
            var customAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _messenger = messenger;
            _cameraService = new DuplexServiceInvoker<ICameraServiceEx>(new InstanceContext(this), "EMERACameraService", logger, messenger, null, customAddress);
        }

        public void ImageGrabbedCallback(ServiceImageWithStatistics image)
        {
            _messenger.Send(image);
        }

        public Response<VoidResult> SetStreamedImageDimension(Int32Rect roi, double scale)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SetStreamedImageDimension(roi, scale));
        }

        public Response<ServiceImage> GetCameraImage()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraImage());
        }

        public Response<ServiceImage> GetScaledCameraImage(Int32Rect roi, double scale)
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetScaledCameraImage(roi, scale));
        }

        public Response<CameraInfo> GetCameraInfo()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraInfo());
        }

        public Response<MatroxCameraInfo> GetMatroxCameraInfo()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetMatroxCameraInfo());
        }

        public Response<VoidResult> SetCameraExposureTime(double exposureTime)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SetCameraExposureTime(exposureTime));
        }

        public Response<double> GetCameraExposureTime()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraExposureTime());
        }

        public Response<bool> StartAcquisition()
        {
            return _cameraService.InvokeAndGetMessages(s => s.StartAcquisition());
        }

        public Response<VoidResult> StopAcquisition()
        {
            return _cameraService.InvokeAndGetMessages(s => s.StopAcquisition());
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            return _cameraService.InvokeAndGetMessages(s => s.Subscribe(acquisitionRoi, scale));
        }

        public Response<VoidResult> Unsubscribe()
        {
            return _cameraService.InvokeAndGetMessages(s => s.Unsubscribe());
        }

        public Response<VoidResult> SetAOI(Rect aoi)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SetAOI(aoi));
        }

        public Response<double> GetCameraGain()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraGain());
        }

        public Response<VoidResult> SetCameraGain(double gain)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SetCameraGain(gain));
        }

        public Response<double> GetFrameRate()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetFrameRate());
        }

        public Response<ColorMode> GetColorMode()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetColorMode());
        }

        public Response<List<ColorMode>> GetColorModes()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetColorModes());
        }

        public Response<bool> IsAcquiring()
        {
            return _cameraService.InvokeAndGetMessages(s => s.IsAcquiring());
        }

        public Response<VoidResult> SetColorMode(ColorMode colorMode)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SetColorMode(colorMode));
        }

        public Response<string> GetCameraID()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraID());
        }

        public Response<ServiceImage> SingleAcquisition()
        {
            return _cameraService.InvokeAndGetMessages(s => s.SingleAcquisition());
        }

        public Response<ServiceImage> SingleScaledAcquisition(Int32Rect roi, double scaleValue)
        {
            return _cameraService.InvokeAndGetMessages(s => s.SingleScaledAcquisition(roi, scaleValue));
        }

        public Response<double> GetCameraFrameRate()
        {
            return _cameraService.InvokeAndGetMessages(s => s.GetCameraFrameRate());
        }
    }
}
