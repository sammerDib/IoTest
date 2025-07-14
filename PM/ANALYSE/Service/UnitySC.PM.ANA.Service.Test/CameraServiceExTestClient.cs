using System;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Test
{
    /// <summary>
    /// Implementation of a WCF client for the ICameraServiceEx server testing
    ///
    /// Override needed methods in tests
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CameraServiceExTestClient : ICameraServiceEx, ICameraServiceCallback
    {
        protected DuplexServiceInvoker<ICameraServiceEx> _remoteService;
        private readonly ILogger<ICameraServiceEx> _logger;

        public CameraServiceExTestClient()
        {
            var instanceContext = new InstanceContext(this);
            _logger = new SerilogLogger<ICameraServiceEx>();
            var messenger = new WeakReferenceMessenger();
            _remoteService = new DuplexServiceInvoker<ICameraServiceEx>(
                instanceContext,
                "CameraService",
                _logger,
                messenger);
        }

        public Response<bool> SetSettings(string cameraId, ICameraInputParams inputParameters)
        {
            throw new NotImplementedException();
        }

        public Response<ICameraInputParams> GetSettings(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Response<ServiceImage> GetSingleGrabImage(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Response<double> GetLightIntensity(string name)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SetLightIntensity(string lightID, double intensity)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _remoteService.TryInvokeAndGetMessages(s => s.Subscribe(acquisitionRoi, scale));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Camera subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> Unsubscribe()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _remoteService.TryInvokeAndGetMessages(s => s.Unsubscribe());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Camera unsubscribe error");
            }

            return resp;
        }

        public Response<VoidResult> SetCameraExposureTime(string cameraId, double exposureTimeMs)
        {
            throw new NotImplementedException();
        }

        public Response<ServiceImage> GetCameraImage(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Response<CameraInfo> GetCameraInfo(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartAcquisition(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StopAcquisition(string cameraId)
        {
            throw new NotImplementedException();
        }
        public Response<double> GetCameraFrameRate(string cameraId)
        {
            throw new NotImplementedException();
        }

        public void ImageGrabbedCallback(string cameraId, ServiceImage image)
        {
            throw new NotImplementedException();
        }       
    }
}
