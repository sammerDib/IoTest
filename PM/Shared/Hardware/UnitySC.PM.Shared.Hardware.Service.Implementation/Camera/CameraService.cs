using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using static UnitySC.PM.Shared.Hardware.Camera.CameraBase;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CameraService : BaseService, ICameraService
    {
        protected HardwareManager HardwareManager;

        protected IMessenger Messenger;

        private readonly ICameraManager _cameraManager;

        public static readonly TimeSpan DefaultNextImageTimeout = TimeSpan.FromMilliseconds(100);
        protected object _lock = new object();

        private class Client
        {
            public ICameraServiceCallback ServiceCallback;
            public Int32Rect AcquisitionRoi;
            public double Scale;
        }

        /// <summary>
        ///  Subscribed client callbacks 1
        /// </summary>
        private Dictionary<ICameraServiceCallback, Client> _clients = new Dictionary<ICameraServiceCallback, Client>();

        public CameraService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            HardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
        }

        public override void Init()
        {
            base.Init();

            Messenger = ClassLocator.Default.GetInstance<IMessenger>();
            Messenger.Register<CameraMessage>(this, (r, m) => ImageGrabbed(m.Camera, m.Image));
        }

        public void ImageGrabbed(CameraBase camera, ICameraImage procimage)
        {
            if (procimage == null)
                return;

            Int32Rect roi;
            double scale;
            lock (_lock)
            {
                if (_clients.Count() == 0)
                    return;

                Client aClient = _clients.Values.First();
                roi = aClient.AcquisitionRoi;
                scale = aClient.Scale;
            }

            ServiceImage svcimg = procimage.ToServiceImage(roi, scale);
            InvokeCallback(x => x.ImageGrabbedCallback(camera.DeviceID, svcimg));
        }

        private void InvokeCallback(Action<ICameraServiceCallback> reportOnClient)
        {
            lock (_lock)
            {
                List<Client> clientsToRemove = new List<Client>();

                // Execute callback for each subscribed client
                foreach (Client client in _clients.Values)
                {
                    if (((ICommunicationObject)(client.ServiceCallback)).State == CommunicationState.Opened)
                        reportOnClient(client.ServiceCallback);
                    else
                        clientsToRemove.Add(client);
                }

                // Remove disconnected client callbacks
                foreach (Client c in clientsToRemove)
                    _clients.Remove(c.ServiceCallback);
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Messenger.Unregister<CameraMessage>(this);
            _cameraManager.Shutdown();
        }

        Response<VoidResult> ICameraService.Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    ICameraServiceCallback serviceCallback = OperationContext.Current.GetCallbackChannel<ICameraServiceCallback>();
                    _clients[serviceCallback] = new Client() { ServiceCallback = serviceCallback, AcquisitionRoi = acquisitionRoi, Scale = scale };
                }
            });
        }

        Response<VoidResult> ICameraService.Unsubscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    ICameraServiceCallback serviceCallback = OperationContext.Current.GetCallbackChannel<ICameraServiceCallback>();
                    _clients.Remove(serviceCallback);
                }
            });
        }

        Response<VoidResult> ICameraService.SetCameraExposureTime(string cameraId, double exposureTimeMs)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                HardwareManager.Cameras[cameraId].SetExposureTimeMs(exposureTimeMs);
            });
        }

        public virtual Response<ServiceImage> GetCameraImage(string cameraId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                lock (_lock)
                {
                    CameraBase camera = HardwareManager.Cameras[cameraId];
                    var procimg = _cameraManager.GetLastCameraImage(camera);
                    return procimg != null ? procimg.ToServiceImage() : null;
                }
            });
        }

        Response<CameraInfo> ICameraService.GetCameraInfo(string cameraId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (!HardwareManager.Cameras.ContainsKey(cameraId))
                    return null;

                CameraBase cam = HardwareManager.Cameras[cameraId];
                return new CameraInfo()
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
                    DeadPixelsFile = cam.Config.DeadPixelsFile,
                };
            });
        }

        Response<VoidResult> ICameraService.StartAcquisition(string cameraId)
        {
            return InvokeVoidResponse<object>(() =>
            {
                HardwareManager.Cameras[cameraId].SetTriggerMode(TriggerMode.Off);
                HardwareManager.Cameras[cameraId].StartContinuousGrab();
                return null;
            });
        }

        Response<VoidResult> ICameraService.StopAcquisition(string cameraId)
        {
            return InvokeVoidResponse<object>(() =>
            {
                HardwareManager.Cameras[cameraId].StopContinuousGrab();
                return null;
            });
        }

        public Response<double> GetCameraFrameRate(string cameraId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                double camFrameRate = HardwareManager.Cameras[cameraId].GetFrameRate();
                return camFrameRate;
            });
        }
    }
}
