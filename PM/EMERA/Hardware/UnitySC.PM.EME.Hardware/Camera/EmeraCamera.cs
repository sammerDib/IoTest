using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;

namespace UnitySC.PM.EME.Hardware.Camera
{
    public class EmeraCamera : IEmeraCamera
    {
        private readonly HardwareManager _hardwareManager;
        private readonly ICameraManager _cameraManager;
        private readonly IMessenger _messenger;
        private readonly object _lock = new object();

        private bool _grab = false;
        private Task _grabTask;

        public CameraBase HardwareCamera => _hardwareManager.Cameras.First().Value;

        public EmeraCamera(HardwareManager hardwareManager, ICameraManager cameraManager, IMessenger messenger)
        {
            _hardwareManager = hardwareManager;
            _cameraManager = cameraManager;
            _messenger = messenger;
        }

        private ICameraImage SingleCameraImage()
        {
            if (!_grab)
            {
                HardwareCamera.SoftwareTrigger();
                HardwareCamera.WaitForSoftwareTriggerGrabbed();
            }
            return _cameraManager.GetLastCameraImage(HardwareCamera);
        }

        public ServiceImage GetCameraImage()
        {
            lock (_lock)
            {
                return _cameraManager.GetLastCameraImage(HardwareCamera).ToServiceImage();
            }
        }

        public ServiceImage GetScaledCameraImage(Int32Rect roi, double scale)
        {
            lock (_lock)
            {
                return _cameraManager.GetLastCameraImage(HardwareCamera).ToServiceImage(roi, scale);
            }
        }

        public ServiceImage SingleAcquisition()
        {
            return SingleCameraImage().ToServiceImage();
        }

        public ServiceImage SingleScaledAcquisition(Int32Rect roi, double scale = 1.0)
        {
            return SingleCameraImage().ToServiceImage(roi, scale);
        }

        public bool StartAcquisition()
        {
            _grab = true;
            HardwareCamera.SoftwareTrigger();
            HardwareCamera.WaitForSoftwareTriggerGrabbed();
            _grabTask = Task.Run(() =>
            {
                while (_grab)
                {
                    HardwareCamera.SoftwareTrigger();
                    HardwareCamera.WaitForSoftwareTriggerGrabbed();
                    var cameraImage = _cameraManager.GetLastCameraImage(HardwareCamera);
                    _messenger.Send(new CameraMessage() { Camera = HardwareCamera, Image = cameraImage });
                }
            });
            return true;
        }

        public void StopAcquisition()
        {
            _grab = false;
            _grabTask?.Wait();
        }

        public bool IsAcquiring()
        {
            return HardwareCamera.IsAcquiring;
        }

        public CameraInfo GetCameraInfo()
        {
            return new CameraInfo()
            {
                Model = HardwareCamera.Model,
                SerialNumber = HardwareCamera.SerialNumber,
                Version = HardwareCamera.Version,
                Width = HardwareCamera.Width,
                Height = HardwareCamera.Height,
                MinExposureTimeMs = HardwareCamera.MinExposureTimeMs,
                MaxExposureTimeMs = HardwareCamera.MaxExposureTimeMs,
                MinGain = HardwareCamera.MinGain,
                MaxGain = HardwareCamera.MaxGain,
                ColorModes = HardwareCamera.ColorModes,
                MinFrameRate = HardwareCamera.MinFrameRate,
                MaxFrameRate = HardwareCamera.MaxFrameRate,
                DeadPixelsFile = HardwareCamera.Config.DeadPixelsFile,
            };
        }

        public MatroxCameraInfo GetMatroxCameraInfo()
        {
            if (HardwareCamera is MatroxCameraBase matCam)
            {
                return new MatroxCameraInfo()
                {
                    Model = matCam.Model,
                    SerialNumber = matCam.SerialNumber,
                    Version = matCam.Version,
                    Width = matCam.Width,
                    Height = matCam.Height,
                    MaxWidth = matCam.MaxWidth,
                    MaxHeight = matCam.MaxHeight,
                    MinWidth = matCam.MinWidth,
                    MinHeight = matCam.MinHeight,
                    WidthIncrement = matCam.WidthIncrement,
                    HeightIncrement = matCam.HeightIncrement,
                    MinExposureTimeMs = matCam.MinExposureTimeMs,
                    MaxExposureTimeMs = matCam.MaxExposureTimeMs,
                    MinGain = matCam.MinGain,
                    MaxGain = matCam.MaxGain,
                    ColorModes = matCam.ColorModes,
                    MinFrameRate = matCam.MinFrameRate,
                    MaxFrameRate = matCam.MaxFrameRate,
                    DeadPixelsFile = matCam.Config.DeadPixelsFile
                };
            }
            else if (HardwareCamera is CameraBase cam)
            {
                return new MatroxCameraInfo()
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
                    DeadPixelsFile = cam.Config.DeadPixelsFile
                };
            }
            else
            {
                return null;
            }
        }

        public void SetCameraExposureTime(double exposureTime)
        {
            HardwareCamera.SetExposureTimeMs(exposureTime);
        }

        public double GetCameraExposureTime()
        {
            return HardwareCamera.GetExposureTimeMs();
        }

        public void SetImageResolution(Size imageResolution)
        {
            HardwareCamera.SetImageResolution(imageResolution);
        }

        public void SetFrameRate(double framerate)
        {
            HardwareCamera.SetFrameRate(framerate);
        }

        public void SetAOI(Rect aoi)
        {
            HardwareCamera.SetAOI(aoi);
        }

        public double GetCameraGain()
        {
            return HardwareCamera.GetGain();
        }

        public void SetCameraGain(double gain)
        {
            HardwareCamera.SetGain(gain);
        }

        public double GetFrameRate()
        {
            return HardwareCamera.GetFrameRate();
        }

        public ColorMode GetColorMode()
        {
            if (HardwareCamera.Config.IsSimulated) return ColorMode.Mono16;
            string colorMode = HardwareCamera.GetColorMode();
            return (ColorMode)Enum.Parse(typeof(ColorMode), colorMode);
        }

        public List<ColorMode> GetColorModes()
        {
            var colorModes = HardwareCamera.GetColorModes();
            return colorModes.ConvertAll(c => (ColorMode)Enum.Parse(typeof(ColorMode), c));
        }

        public void SetColorMode(ColorMode colorMode)
        {
            HardwareCamera.SetColorMode(colorMode.ToString());
        }

        public string GetCameraId()
        {
            return HardwareCamera.DeviceID;
        }

        public double GetCameraFrameRate()
        {
            return HardwareCamera.GetFrameRate();
        }
    }
}
