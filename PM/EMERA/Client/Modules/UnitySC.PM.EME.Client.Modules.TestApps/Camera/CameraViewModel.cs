using System;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Shared.Image;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;


namespace UnitySC.PM.EME.Client.Modules.TestApps.Camera
{
    public class CameraViewModel : ObservableRecipient
    {
        private readonly CameraBench _camera;

        public CameraViewModel(CameraBench camera, IMessenger messenger)
        {
            messenger?.Register<ServiceImageWithStatistics>(this, UpdateImage);
            _camera = camera;

            double cameraResolution = _camera.Width * _camera.Height;
            double initialScaleResolution = Math.Min(1.0, Math.Sqrt(TargetResolution / cameraResolution));
            _camera.SetStreamedImageDimension(Int32Rect.Empty, initialScaleResolution);
        }
        private void UpdateImage(object recipient, ServiceImageWithStatistics image)
        {
            if (NormalizationMin == 0 && NormalizationMax == _camera.MaxPixelValue) 
            {
                Image = image;
            }
            else
            {
                Image = NormalizeImage(image, NormalizationMin, NormalizationMax);
            }
        }

        private ServiceImageWithStatistics _image;

        public ServiceImageWithStatistics Image
        {
            get => _image;
            set
            {
                SetProperty(ref _image, value);
                OnPropertyChanged(nameof(ScaleTextValue));
                OnPropertyChanged(nameof(DistanceValueText));
                OnPropertyChanged(nameof(IntensityPoints));
                OnPropertyChanged(nameof(FullImageWidth));
                OnPropertyChanged(nameof(FullImageHeight));
                OnPropertyChanged(nameof(ImageCropArea));
            }
        }

        public int FullImageWidth => (Image?.OriginalWidth) ?? 0;
        public int FullImageHeight => (Image?.OriginalHeight) ?? 0;
        public Int32Rect ImageCropArea
        {
            get
            {
                if (Image == null)
                {
                    return Int32Rect.Empty;
                }
                if (Image.AcquisitionRoi == Int32Rect.Empty)
                {
                    return new Int32Rect { X = 0, Y = 0, Width = Image.OriginalWidth, Height = Image.OriginalHeight };
                }
                return Image.AcquisitionRoi;
            }
        }

        private double _zoom;

        public double Zoom
        {
            get => _zoom;
            set
            {
                SetProperty(ref _zoom, value);
                OnPropertyChanged(nameof(ScaleTextValue));
                OnPropertyChanged(nameof(DistanceValueText));
                OnPropertyChanged(nameof(IntensityPoints));
            }
        }

        private Point _imageReferentialOrigin;

        public Point ImageReferentialOrigin
        {
            get => _imageReferentialOrigin;
            set
            {
                SetProperty(ref _imageReferentialOrigin, value);
                OnPropertyChanged(nameof(IntensityPoints));
            }
        }
        public int TargetResolution { get; set; } = 1920 * 1080;

        private Int32Rect _imagePortion;

        public Int32Rect ImagePortion
        {
            get => _imagePortion;
            set
            {
                if (Image == null || _imagePortion == value)
                {
                    return;
                }

                double currentResolution = value.Width * value.Height;
                double scaleResolution = Math.Min(1.0, Math.Sqrt(TargetResolution / currentResolution));
                _camera.SetStreamedImageDimension(value, scaleResolution);

                SetProperty(ref _imagePortion, value);
            }
        }

        public double ScaleLengthInPixel => 200;

        public string ScaleTextValue
        {
            get
            {
                if (_image == null)
                    return "";

                double scaleLengthInMicrometers = _camera.PixelSize.Micrometers * ScaleLengthInPixel / Zoom;
                return FormatAsLengthText(scaleLengthInMicrometers);
            }
        }

        public SeriesPoint[] IntensityPoints
        {
            get
            {
                if (Image == null || !IsRulerActivated)
                    return new SeriesPoint[] { };

                return GetIntensityPoints(Image);
            }
        }

        private SeriesPoint[] GetIntensityPoints(ServiceImageWithStatistics image)
        {
            var imageData = image.ToOpenCvImage();
            var startPixel = GetPixelPositionOnImage(RulerStartPoint);
            var endPixel = GetPixelPositionOnImage(RulerEndPoint);
            var profile = UnitySCSharedAlgosOpenCVWrapper.ImageOperators.ExtractIntensityProfile(imageData, startPixel, endPixel);
            return profile.Select(point => new SeriesPoint(point.X, point.Y)).ToArray();
        }

        public int Maximum => Image?.Type == ServiceImage.ImageType.Greyscale16Bit ? 65536 : 255;

        private Point2i GetPixelPositionOnImage(Point pixel)
        {
            if (Zoom == 0)
            {
                return new Point2i(0, 0);
            }
            int x = (int)(((pixel.X - ImageReferentialOrigin.X) / Zoom - ImageCropArea.X) * Image.Scale);
            int y = (int)(((pixel.Y - ImageReferentialOrigin.Y) / Zoom - ImageCropArea.Y) * Image.Scale);
            return new Point2i(x, y);
        }

        public int NormalizationMax
        {
            get => _camera.NormalizationMax;
        }

        public int NormalizationMin
        {
            get => _camera.NormalizationMin;
        }

        private bool _isRulerActivated;

        public bool IsRulerActivated
        {
            get => _isRulerActivated;
            set
            {
                SetProperty(ref _isRulerActivated, value);
                OnPropertyChanged(nameof(IntensityPoints));
                OnPropertyChanged(nameof(Maximum));
            }
        }

        private Point _rulerStartPoint;

        public Point RulerStartPoint
        {
            get => _rulerStartPoint;
            set
            {
                SetProperty(ref _rulerStartPoint, value);
                OnPropertyChanged(nameof(DistanceValueText));
                OnPropertyChanged(nameof(IntensityPoints));
            }
        }

        private Point _rulerEndPoint;

        public Point RulerEndPoint
        {
            get => _rulerEndPoint;
            set
            {
                SetProperty(ref _rulerEndPoint, value);
                OnPropertyChanged(nameof(DistanceValueText));
                OnPropertyChanged(nameof(IntensityPoints));
            }
        }

        public string DistanceValueText
        {
            get
            {
                if (_image == null)
                {
                    return "";
                }

                double distanceInPixelOnCanvas = Math.Sqrt(Math.Pow(RulerStartPoint.X - RulerEndPoint.X, 2) +
                                                           Math.Pow(RulerStartPoint.Y - RulerEndPoint.Y, 2));
                double distanceInMicrometersOnImage = distanceInPixelOnCanvas * _camera.PixelSize.Micrometers / Zoom;

                return FormatAsLengthText(distanceInMicrometersOnImage);
            }
        }

        private string FormatAsLengthText(double distanceInMicrometers)
        {
            if (distanceInMicrometers >= 1000)
            {
                return $"{distanceInMicrometers / 1000:G3} mm";
            }

            if (distanceInMicrometers > 1)
            {
                return $"{distanceInMicrometers:G3} µm";
            }

            return $"{distanceInMicrometers * 1000:G3} nm";
        }

        private ServiceImageWithStatistics NormalizeImage(ServiceImageWithStatistics image, int min, int max)
        {
            var imgData = AlgorithmLibraryUtils.CreateImageData(image);
            var normalizedImgData = UnitySCSharedAlgosOpenCVWrapper.ImageOperators.SaturatedNormalization(imgData, min, max);
            var serviceImage = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(normalizedImgData);
            return new ServiceImageWithStatistics()
            {
                Data = serviceImage.Data,
                Type = serviceImage.Type,
                DataHeight = serviceImage.DataHeight,
                DataWidth = serviceImage.DataWidth,
                OriginalWidth = image.OriginalWidth,
                OriginalHeight = image.OriginalHeight,
                Scale = image.Scale,
                ImageId = image.ImageId,
                AcquisitionRoi = image.AcquisitionRoi,
                StatisticRoi = image.StatisticRoi,
                Min = image.Min,
                Max = image.Max,
                Mean = image.Mean,
                StandardDeviation = image.StandardDeviation,
                Histogram = image.Histogram,
                Profile = image.Profile
            };
        }
    }
}
