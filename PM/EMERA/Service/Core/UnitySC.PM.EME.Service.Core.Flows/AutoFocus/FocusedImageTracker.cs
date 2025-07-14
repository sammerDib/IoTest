using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    public class FocusedImageTracker : USPCameraImageTracker
    {
        private readonly object _computeFocusLock = new object();
        private readonly ImageOperators _imageOperatorsLib;
        private readonly ILogger<FocusedImageTracker> _logger;
        private readonly bool _saveImages;
        private int _imagesCount;

        public FocusedImageTracker(CameraBase camera, bool saveImages = false,
            ConcurrentDictionary<ICameraImage, double> imagesWithFocusValues = null,
            ImageOperators imageOperatorsLib = null) : base(camera)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<FocusedImageTracker>>();
            _saveImages = saveImages;
            _imageOperatorsLib = imageOperatorsLib ?? new ImageOperators();

            ImagesWithFocusValues = imagesWithFocusValues ?? new ConcurrentDictionary<ICameraImage, double>();
        }

        public virtual ConcurrentDictionary<ICameraImage, double> ImagesWithFocusValues { get; set; }

        private List<ICameraImage> ChronologicalImages =>
            (from image in ImagesWithFocusValues.Keys orderby image.Timestamp select image).ToList();

        public int TrackedImagesCount => ImagesWithFocusValues.Count;

        public override void StartTracking()
        {
            _imagesCount = 0;
            base.StartTracking();
        }

        public KeyValuePair<ICameraImage, double> GetImageWithMaxFocusValue()
        {
            var imageWithMaxValue = (from imageWithFocusValue in ImagesWithFocusValues
                orderby imageWithFocusValue.Value descending
                select imageWithFocusValue).First();
            _logger.Debug(
                $"MaxFocusValue found on image ${imageWithMaxValue.Key} with focusValue={imageWithMaxValue.Value}");
            return imageWithMaxValue;
        }

        public void WaitFocusValuesComputation(bool logDuration = false)
        {
            var startingTimeMs = DateTime.Now;
            SpinWait.SpinUntil(() => ImagesWithFocusValues.Count >= _imagesCount);
            if (logDuration)
                _logger.Debug(
                    $"WaitFocusValuesComputation total duration = {(DateTime.Now - startingTimeMs).TotalMilliseconds} ms.");
        }

        private double GetFocusValue(ICameraImage image)
        {
            lock (_computeFocusLock)
            {
                if (image is USPImageMil mil)
                {
                    return _imageOperatorsLib.ComputeFocusMeasure(mil);
                }

                return _imageOperatorsLib.ComputeFocusMeasure(image as USPImage);
            }
        }

        private void AnalyzeImageTask(CameraMessage message)
        {
            double focusValue = GetFocusValue(message.Image);

            bool imageSuccessfullyAdded;
            if (_saveImages)
            {
                imageSuccessfullyAdded = ImagesWithFocusValues.TryAdd(message.Image, focusValue);
            }
            else
            {
                if (message.Image is USPImageMil)
                {
                    var emptyImage = new USPImageMil() { Timestamp = DateTime.UtcNow };
                    imageSuccessfullyAdded = ImagesWithFocusValues.TryAdd(emptyImage, focusValue);
                }
                else
                {
                    var emptyImage = new USPImage { Timestamp = message.Image.Timestamp };
                    imageSuccessfullyAdded = ImagesWithFocusValues.TryAdd(emptyImage, focusValue);
                }
            }

            if (!imageSuccessfullyAdded)
            {
                _logger.Error("Unable to add image.");
            }
        }

        protected override void CameraMessageHandler(CameraMessage message)
        {
            _imagesCount++;
            Task.Run(() => AnalyzeImageTask(message));
        }

        public override void Dispose()
        {
            ImagesWithFocusValues.Clear();
        }

        public override void Reset()
        {
            ImagesWithFocusValues.Clear();
        }

        public void SaveImages(string directoryPath, PositionTracker positionTracker)
        {
            int imageNumber = 1;
            foreach (var image in ChronologicalImages)
            {
                Directory.CreateDirectory(directoryPath);

                var imagePosition = positionTracker.GetPositionAtTime(image.Timestamp.ToUniversalTime().Ticks);

                string imageName = $"img-{imageNumber}-pos -{Math.Round(imagePosition.Millimeters, 3)}.png";
                string imagePath = Path.Combine(directoryPath, imageName);

                ImageReport.SaveImage(image.ToServiceImage(), imagePath);
                imageNumber++;
            }
        }
    }
}
