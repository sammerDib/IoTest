using System;
using System.Collections.Concurrent;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tracking;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    public class USPCameraImageTracker : ITracker
    {
        /// <summary>
        /// ConcurrentBag intented to be populated with images as soon as the StartTracking() method has been called.
        /// </summary>
        public virtual ConcurrentBag<USPImage> Images { get; set; }

        /// <summary>
        /// Camera used for continuous image acquisition.
        /// </summary>
        private readonly CameraBase _camera;

        /// <summary>
        /// Camera state before tracking starts
        /// </summary>
        private bool _cameraIsAcquiringFromOutside { get; set; } = false;

        private bool _trackingStarted { get; set; } = false;

        /// <summary>
        /// Messerger used to listen incoming camera messages containing images.
        /// </summary>
        private IMessenger _messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public USPCameraImageTracker(CameraBase camera, ConcurrentBag<USPImage> images = null)
        {
            _camera = camera;
            Images = images ?? new ConcurrentBag<USPImage>();
        }

        public virtual void StartTracking()
        {
            // Start listening for incomming messages from the camera
            _cameraIsAcquiringFromOutside = _camera.IsAcquiring;
            if (!_cameraIsAcquiringFromOutside)
            {
                _camera.StartContinuousGrab();
            }
            _messenger.Register<CameraMessage>(this, (r, m) => CameraMessageHandler(m));

            _trackingStarted = true;
        }

        public virtual void StartTrackingUntil(int imageCount, int timeout_ms = 5000)
        {
            Reset();
            StartTracking();

            bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() => imageCount <= Images.Count, timeout_ms);
            if (!hasStoppedWithinTimeout)
            {
                StopTracking();
                throw new TimeoutException($"CameraImageTracker exits on timeout ({timeout_ms} ms).");
            }

            StopTracking();
        }

        public virtual void StopTracking()
        {
            _messenger.Unregister<CameraMessage>(this);
            _messenger.Cleanup();

            if (!_cameraIsAcquiringFromOutside && _trackingStarted)
            {
                _camera.StopContinuousGrab();
            }
            _trackingStarted = false;
        }

        protected virtual void CameraMessageHandler(CameraMessage message)
        {
            Images.Add((USPImage)message.Image);
        }

        public virtual void Reset()
        {
            Dispose();
            Images = new ConcurrentBag<USPImage>();
        }

        public virtual void Dispose()
        {
            Images = null;
        }
    }
}
