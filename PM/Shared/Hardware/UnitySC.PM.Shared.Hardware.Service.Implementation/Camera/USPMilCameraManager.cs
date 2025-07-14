using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation.Camera
{
    public class USPMilCameraManager : ICameraManager
    {
        public Dictionary<CameraBase, ICameraImage> LastImages { get; set; } = new Dictionary<CameraBase, ICameraImage>();
        public Dictionary<CameraBase, long> LastImagesIds { get; set; } = new Dictionary<CameraBase, long>();

        protected object Lock = new object();

        private IMessenger _messenger;

        private DateTime _minNextImageTime;
        private TaskCompletionSource<ICameraImage> _nextImageTaskCompletion;

        public USPMilCameraManager()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _messenger.Register<CameraMessage>(this, (r, m) => ImageGrabbed(m.Camera, m.Image));
        }

        public void ImageGrabbed(CameraBase camera, ICameraImage procimage)
        {
            if (procimage == null)
                return;

            if (!LastImagesIds.ContainsKey(camera))
                LastImagesIds[camera] = 0;

            lock (Lock)
            {
                LastImages.TryGetValue(camera, out ICameraImage previmg);
                LastImages[camera] = procimage;
                LastImagesIds[camera]++;
            }
        }

        public virtual ICameraImage GetLastCameraImage(CameraBase camera)
        {
            LastImages.TryGetValue(camera, out ICameraImage procimg);
            return procimg;
        }

        public ICameraImage GetNextCameraImage(CameraBase camera)
        {
            if (!camera.IsAcquiring)
            {
                throw new Exception("Camera is not acquiring");
            }

            // To be sure that the camera image acquisition didn't start before the function call,
            // we must wait at least the exposure time before returning the new image.
            DateTime functionCallStart = DateTime.Now;
            double exposureTimeMs = camera.GetExposureTimeMs();
            _minNextImageTime = functionCallStart.AddMilliseconds(exposureTimeMs);
            _nextImageTaskCompletion = new TaskCompletionSource<ICameraImage>();

            try
            {
                _messenger.Unregister<CameraMessage>(this);
                _messenger.Register<CameraMessage>(this, (r, m) => { if (m.Camera == camera) OnNextImage(m); ImageGrabbed(m.Camera, m.Image); });

                var timeout = CameraService.DefaultNextImageTimeout;
                if (camera.GetFrameRate() != 0)
                {
                    var delayBetweenFramesInMs = 1000 / camera.GetFrameRate();
                    var timeoutMs = 10 * delayBetweenFramesInMs;
                    timeoutMs = Math.Max(timeoutMs, 250);
                    timeout = TimeSpan.FromMilliseconds(timeoutMs);
                }

                var task = _nextImageTaskCompletion.Task;
                if (!task.Wait(timeout))
                {
                    throw new Exception($"Timeout {timeout} reached");
                }
                var image = task.Result;
                return image;
            }
            finally
            {
                _messenger.Unregister<CameraMessage>(this);
                _messenger.Register<CameraMessage>(this, (r, m) => ImageGrabbed(m.Camera, m.Image));
            }
        }

        public void OnNextImage(CameraMessage m)
        {
            if (DateTime.Now < _minNextImageTime)
                return;
            _nextImageTaskCompletion.TrySetResult(m.Image);
        }

        public void Shutdown()
        {
            foreach (USPImageMil img in LastImages.Values)
                img.Dispose();

            _messenger.Unregister<CameraMessage>(this);
        }
    }
}
