using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeCameraSupervisor : ICameraServiceEx
    {
        private readonly IMessenger _messenger;

        private int _imageWidth = 160;
        private int _imageHeight = 90;
        private bool _isStreaming;
        private Int32Rect _croppingRoi = Int32Rect.Empty;
        private double _resolutionScale = 1.0;

        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);

        public double Gain { get; set; }
        public double ExposureTime { get; set; }

        public FakeCameraSupervisor(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public Response<VoidResult> SetAOI(Rect aoi)
        {
            return new Response<VoidResult>();
        }

        public Response<ServiceImage> GetCameraImage()
        {
            var image = new ServiceImage() { Data = new byte[] { 1 }, DataHeight = 1, DataWidth = 1 };
            return new Response<ServiceImage>() { Result = image };
        }
        public Response<ServiceImage> GetScaledCameraImage(Int32Rect roi, double scale)
        {
            throw new NotImplementedException();
        }

        public Response<CameraInfo> GetCameraInfo()
        {
            var cameraInfo = new CameraInfo() { MinExposureTimeMs = 0.0, MaxExposureTimeMs = 7.0, MinGain = 0.0, MaxGain = 10.0, Width = 900, Height = 700 };
            return new Response<CameraInfo>() { Result = cameraInfo };
        }
        public Response<MatroxCameraInfo> GetMatroxCameraInfo()
        {
            var cameraInfo = new MatroxCameraInfo() { MinExposureTimeMs = 0.0, MaxExposureTimeMs = 7.0, MinGain = 0.0, MaxGain = 10.0, Width = 900, Height = 700 };
            return new Response<MatroxCameraInfo>() { Result = cameraInfo };
        }

        public Response<VoidResult> SetCameraExposureTime(double exposureTime)
        {
            ExposureTime = exposureTime;
            return new Response<VoidResult>();
        }

        public Response<double> GetCameraExposureTime()
        {
            return new Response<double>() { Result = ExposureTime };
        }

        public Response<bool> StartAcquisition()
        {
            _isStreaming = true;
            Task.Run(() => SimulateStreaming());
            return new Response<bool>() { Result = true};
        }

        public Response<VoidResult> StopAcquisition()
        {
            _isStreaming = false;
            return new Response<VoidResult>();
        }

        private void SimulateStreaming()
        {
            while (_isStreaming)
            {
                _synchro.Reset();
                Thread.Sleep(10);
                int width = _croppingRoi != Int32Rect.Empty ? (int)(_croppingRoi.Width * _resolutionScale) : _imageWidth;
                int height = _croppingRoi != Int32Rect.Empty ? (int)(_croppingRoi.Height * _resolutionScale) : _imageHeight;
                var image = new ServiceImageWithStatistics()
                {
                    Data = Enumerable.Repeat((byte)0, width * height).ToArray(),
                    DataWidth = width,
                    DataHeight = height,
                    OriginalWidth = _imageWidth,
                    OriginalHeight = _imageHeight,
                    Scale = _resolutionScale,
                    AcquisitionRoi = _croppingRoi
                };
                _messenger.Send(image);
                _synchro.Set();
            }
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            return new Response<VoidResult>();
        }

        public Response<VoidResult> Unsubscribe()
        {
            return new Response<VoidResult>();
        }

        public Response<double> GetCameraGain()
        {
            return new Response<double>() { Result = Gain };
        }

        public Response<string> GetCameraID()
        {
            return new Response<string>() { Result = "1" };
        }

        public Response<VoidResult> SetCameraGain(double gain)
        {
            Gain = gain;
            return new Response<VoidResult>();
        }

        public Response<ServiceImage> SingleAcquisition()
        {
            var dummyImage = new ServiceImage() { Data = new byte[] { 1 }, DataHeight = 1, DataWidth = 1 };
            return new Response<ServiceImage>() { Result = dummyImage };
        }

        public Response<ServiceImage> SingleScaledAcquisition(Int32Rect roi, double scaleValue)
        {
            var dummyImage = new ServiceImage() { Data = new byte[] { 1 }, DataHeight = 1, DataWidth = 1 };
            return new Response<ServiceImage>() { Result = dummyImage };
        }

        public Response<double> GetFrameRate()
        {
            throw new NotImplementedException();
        }

        public Response<bool> IsAcquiring
            ()
        {
            return new Response<bool>() { Result = true };
        }

        public Response<ColorMode> GetColorMode()
        {
            return new Response<ColorMode>() { Result = ColorMode.Mono16 };
        }

        public Response<VoidResult> SetStreamedImageDimension(Int32Rect roi, double scale)
        {
            _croppingRoi = roi;
            _resolutionScale = scale;
            return new Response<VoidResult>();
        }

        public Response<double> GetCameraFrameRate()
        {
            throw new NotImplementedException();
        }

        public void WaitForOneAcquisition()
        {
            _synchro.WaitOne(500);
        }

        public Response<VoidResult> SetColorMode(ColorMode colorMode)
        {
            return new Response<VoidResult>();
        }

        public Response<List<ColorMode>> GetColorModes()
        {
            return new Response<List<ColorMode>>();
        }
    }
}
