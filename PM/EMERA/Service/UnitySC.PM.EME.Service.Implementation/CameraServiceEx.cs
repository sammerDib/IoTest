using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

using UnitySCSharedAlgosOpenCVWrapper;

using ImageType = UnitySCSharedAlgosOpenCVWrapper.ImageType;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CameraServiceEx : DuplexServiceBase<ICameraServiceCallbackEx>, ICameraServiceEx
    {
        private readonly IEmeraCamera _camera;

        private Int32Rect _imageRoi = Int32Rect.Empty;
        private double _imageResolutionScale = 1.0;

        public CameraServiceEx(IEmeraCamera camera, IMessenger messenger, ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _camera = camera;
            messenger.Register<CameraMessage>(this, (_, m) => ImageGrabbed(m.Image));
        }

        private void ImageGrabbed(ICameraImage image)
        {
            InvokeCallback(i =>
            {
                var resizedImage = Resize(image?.ToServiceImage());
                i.ImageGrabbedCallback(resizedImage);
            });
        }

        private ServiceImageWithStatistics Resize(ServiceImage serviceImage)
        {
            if (serviceImage == null)
                return null;
            if (_imageResolutionScale == 1.0 && _imageRoi == Int32Rect.Empty)
            {
                return new ServiceImageWithStatistics()
                {
                    Data = serviceImage.Data,
                    DataWidth = serviceImage.DataWidth,
                    DataHeight = serviceImage.DataHeight,
                    OriginalWidth = serviceImage.DataWidth,
                    OriginalHeight = serviceImage.DataHeight,
                    Type = serviceImage.Type,
                    Scale = _imageResolutionScale,
                    AcquisitionRoi = _imageRoi
                };
            }

            var imageType = GetOpenCvImageType(serviceImage.Type);
            var openCvImage = new ImageData(serviceImage.Data, serviceImage.DataWidth, serviceImage.DataHeight, imageType);
            var croppingRoi = new RegionOfInterest() { X = _imageRoi.X, Y = _imageRoi.Y, Width = _imageRoi.Width, Height = _imageRoi.Height };
            var resizedOpenCVImage = ImageOperators.Resize(openCvImage, croppingRoi, _imageResolutionScale);

            return new ServiceImageWithStatistics()
            {
                Data = resizedOpenCVImage.ByteArray,
                DataWidth = resizedOpenCVImage.Width,
                DataHeight = resizedOpenCVImage.Height,
                OriginalWidth = serviceImage.DataWidth,
                OriginalHeight = serviceImage.DataHeight,
                Type = serviceImage.Type,
                Scale = _imageResolutionScale,
                AcquisitionRoi = _imageRoi
            };
        }

        private static ImageType GetOpenCvImageType(ServiceImage.ImageType type)
        {
            switch (type)
            {
                case ServiceImage.ImageType.Greyscale:
                    return ImageType.GRAYSCALE_Unsigned8bits;
                case ServiceImage.ImageType.Greyscale16Bit:
                    return ImageType.GRAYSCALE_Unsigned16bits;
                case ServiceImage.ImageType.RGB:
                    return ImageType.RGB_Unsigned8bits;
                case ServiceImage.ImageType._3DA:
                    return ImageType.GRAYSCALE_Float32bits;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Response<VoidResult> SetStreamedImageDimension(Int32Rect roi, double scale)
        {
            return InvokeVoidResponse(_ =>
            {
                _imageRoi = roi;
                _imageResolutionScale = scale;
            });
        }

        public Response<ServiceImage> GetCameraImage()
        {
            return InvokeDataResponse(_ => _camera.GetCameraImage());
        }

        public Response<ServiceImage> GetScaledCameraImage(Int32Rect roi, double scale)
        {
            return InvokeDataResponse(_ => _camera.GetScaledCameraImage(roi, scale));
        }

        public Response<CameraInfo> GetCameraInfo()
        {
            return InvokeDataResponse(_ => _camera.GetCameraInfo());
        }

        Response<MatroxCameraInfo> ICameraServiceEx.GetMatroxCameraInfo()
        {
            return InvokeDataResponse(_ => _camera.GetMatroxCameraInfo());
        }

        public Response<VoidResult> SetCameraExposureTime(double exposureTime)
        {
            return InvokeVoidResponse(_ => _camera.SetCameraExposureTime(exposureTime));
        }

        public Response<double> GetCameraExposureTime()
        {
            return InvokeDataResponse(_ => _camera.GetCameraExposureTime());
        }

        public Response<bool> StartAcquisition()
        {
            return InvokeDataResponse(_ => _camera.StartAcquisition());
        }

        public Response<VoidResult> StopAcquisition()
        {
            return InvokeVoidResponse(_ => _camera.StopAcquisition());
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            _imageRoi = acquisitionRoi;
            _imageResolutionScale = scale;
            return InvokeVoidResponse(_ => base.Subscribe());
        }

        public new Response<VoidResult> Unsubscribe()
        {
            return InvokeVoidResponse(_ => base.Unsubscribe());
        }

        public Response<VoidResult> SetAOI(Rect aoi)
        {
            return InvokeVoidResponse(_ => _camera.SetAOI(aoi));
        }

        public Response<double> GetCameraGain()
        {
            return InvokeDataResponse(_ => _camera.GetCameraGain());
        }

        public Response<VoidResult> SetCameraGain(double gain)
        {
            return InvokeVoidResponse(_ => _camera.SetCameraGain(gain));
        }

        public Response<double> GetFrameRate()
        {
            return InvokeDataResponse(_ => _camera.GetFrameRate());
        }

        public Response<ColorMode> GetColorMode()
        {
            return InvokeDataResponse(_ => _camera.GetColorMode());
        }

        public Response<List<ColorMode>> GetColorModes()
        {
            return InvokeDataResponse(_ => _camera.GetColorModes());
        }

        public Response<VoidResult> SetColorMode(ColorMode colorMode)
        {
            return InvokeVoidResponse(_ => _camera.SetColorMode(colorMode));
        }

        public Response<string> GetCameraID()
        {
            return InvokeDataResponse(_ => _camera.GetCameraId());
        }

        public Response<ServiceImage> SingleAcquisition()
        {
            return InvokeDataResponse(_ => _camera.SingleAcquisition());
        }

        public Response<ServiceImage> SingleScaledAcquisition(Int32Rect roi, double scaleValue)
        {
            return InvokeDataResponse(_ => _camera.SingleScaledAcquisition(roi, scaleValue));
        }

        public Response<bool> IsAcquiring()
        {
            return InvokeDataResponse(_ => _camera.IsAcquiring());
        }

        public Response<double> GetCameraFrameRate()
        {
            return InvokeDataResponse(_ => _camera.GetCameraFrameRate());
        }
    }
}
