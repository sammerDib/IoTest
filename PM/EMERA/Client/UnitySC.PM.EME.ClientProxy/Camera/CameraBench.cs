using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Proxy.Camera
{
    public class CameraBench : ObservableObject
    {
        private readonly ICameraServiceEx _cameraSupervisor;
        private readonly IMessenger _messenger;

        private bool _isStreaming;

        public bool IsStreaming
        {
            get => _isStreaming;
            private set => SetProperty(ref _isStreaming, value);
        }

        public string CameraId { get; }
        public double MaxExposureTime { get; }
        public double MinExposureTime { get; }
        public double MaxGain { get; }
        public double MinGain { get; }
        public int Width { get; }
        public int Height { get; }

        private int _maxPixelValue;

        public int MaxPixelValue
        {
            get => _maxPixelValue;
            set => SetProperty(ref _maxPixelValue, value);
        }

        private int _normalizationMax;

        public int NormalizationMax
        {
            get => _normalizationMax;
            set => SetProperty(ref _normalizationMax, value);
        }

        private int _normalizationMin = 0;

        public int NormalizationMin
        {
            get => _normalizationMin;
            set => SetProperty(ref _normalizationMin, value);
        }

        public Length PixelSize { get; } = 2.0.Micrometers();

        public CameraBench(ICameraServiceEx cameraSupervisor, ICalibrationService calibrationSupervisor,
            IMessenger messenger)
        {
            _cameraSupervisor = cameraSupervisor;
            _messenger = messenger;

            CameraId = _cameraSupervisor.GetCameraID().Result;

            var cameraInfo = GetMatroxCameraInfo();
            MinExposureTime = cameraInfo.MinExposureTimeMs;
            MaxExposureTime = cameraInfo.MaxExposureTimeMs;
            MinGain = cameraInfo.MinGain;
            MaxGain = cameraInfo.MaxGain;
            Width = cameraInfo.Width;
            Height = cameraInfo.Height;

            int depth = (int)GetColorMode();
            MaxPixelValue = (int)Math.Pow(2, depth) - 1;
            NormalizationMax = MaxPixelValue;

            var calibrations = calibrationSupervisor?.GetCalibrations().Result;
            var calibration = calibrations.OfType<CameraCalibrationData>().FirstOrDefault();
            PixelSize = calibration != null ? calibration.PixelSize : PixelSize;

            _cameraSupervisor.SetCameraExposureTime(100);
            _cameraSupervisor.Subscribe(Int32Rect.Empty, 1.0);
        }

        public void SetAOI(Rect aoi)
        {
            _cameraSupervisor.SetAOI(aoi);
        }

        public async Task SetExposureTime(double exposureTime)
        {
            if (exposureTime < MinExposureTime || exposureTime > MaxExposureTime)
            {
                _messenger?.Send(new Message(MessageLevel.Information,
                    $"Set exposure time ({exposureTime}) is outside of allowed range ({MinExposureTime},{MaxExposureTime})."));
                return;
            }

            await Task.Run(() => _cameraSupervisor.SetCameraExposureTime(exposureTime));
        }

        public double GetExposureTime()
        {
            return _cameraSupervisor.GetCameraExposureTime().Result;
        }

        public async Task SetGain(double gain)
        {
            if (gain < MinGain || gain > MaxGain)
            {
                _messenger?.Send(new Message(MessageLevel.Information,
                    $"Set gain ({gain}) is outside of allowed range ({MinGain},{MaxGain})."));
                return;
            }

            await Task.Run(() => _cameraSupervisor.SetCameraGain(gain));
        }

        public double GetGain()
        {
            return _cameraSupervisor.GetCameraGain().Result;
        }

        public double GetFrameRate()
        {
            return _cameraSupervisor.GetFrameRate().Result;
        }

        public CameraInfo GetCameraInfo()
        {
            return _cameraSupervisor.GetCameraInfo().Result;
        }

        public MatroxCameraInfo GetMatroxCameraInfo()
        {
            return _cameraSupervisor.GetMatroxCameraInfo().Result;
        }

        public ColorMode GetColorMode()
        {
            return _cameraSupervisor.GetColorMode().Result;
        }

        public async Task<ServiceImage> GetScaledCameraImageAsync(Int32Rect roi, double scale)
        {
            var response = await Task.Run(() => _cameraSupervisor.GetScaledCameraImage(roi, scale));
            return response.Result;
        }

        public async Task<ServiceImage> SingleAcquisitionAsync()
        {
            var response = await Task.Run(() => _cameraSupervisor.SingleAcquisition());
            return response.Result;
        }

        public async Task SaveImageAsync(ServiceImage image, string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            await Task.Run(() => image.SaveToFile(filePath));
        }

        public async Task StartStreamingAsync()
        {
            bool imageAcquired = await Task.Run(() => _cameraSupervisor.StartAcquisition().Result);
            if (imageAcquired) 
            {
                IsStreaming = true;
            }
        }

        public async Task StopStreamingAsync()
        {
            await Task.Run(() => _cameraSupervisor.StopAcquisition());
            IsStreaming = false;
        }

        public async Task SetColorModeAsync(ColorMode colorMode)
        {
            await Task.Run(() => _cameraSupervisor.SetColorMode(colorMode));
        }

        public void SetStreamedImageDimension(Int32Rect croppingArea, double scale)
        {
            Task.Run(() => _cameraSupervisor.SetStreamedImageDimension(croppingArea, scale));
        }

        public List<ColorMode> GetColorModes()
        {
            return _cameraSupervisor.GetColorModes().Result;
        }

        public void Unsubscribe()
        {
            Task.Run(() => _cameraSupervisor.Unsubscribe());
        }
    }
}
