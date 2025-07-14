using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.AutofocusV2
{
    public class FocusedImageTracker : USPCameraImageTracker
    {
        private readonly ILogger<FocusedImageTracker> _logger;
        private readonly ImageOperators _imageOperatorsLib;

        /// <summary>
        /// Dictionnary listing the images and their associated focus values.
        /// </summary>
        public virtual ConcurrentDictionary<USPImage, double> ImagesWithFocusValues { get; set; }

        private List<USPImage> _chronologicalImages => (from image in ImagesWithFocusValues.Keys orderby image.Timestamp ascending select image).ToList();

        private readonly object _computeFocusLock = new object();
        private int _imagesCount;
        private bool _saveImages;
        private List<Task> _analyseImageTasks = new List<Task>();

        public FocusedImageTracker(CameraBase camera, bool saveImages = false, ConcurrentDictionary<USPImage, double> imagesWithFocusValues = null, ImageOperators imageOperatorsLib = null) : base(camera)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<FocusedImageTracker>>();
            _saveImages = saveImages;

            _imageOperatorsLib = imageOperatorsLib != null ? imageOperatorsLib : new ImageOperators();

            ImagesWithFocusValues = imagesWithFocusValues ?? new ConcurrentDictionary<USPImage, double>(Environment.ProcessorCount, 2 ^ 11);
        }

        public override void StartTracking()
        {
            _imagesCount = 0;
            base.StartTracking();
        }

        public KeyValuePair<USPImage, double> GetImageWithMaxFocusValue()
        {
            var imageWithMaxValue = (from imageWithFocusValue in ImagesWithFocusValues orderby imageWithFocusValue.Value descending select imageWithFocusValue).First();
            _logger.Debug($"MaxFocusValue found on image ${imageWithMaxValue.Key} with focusValue={imageWithMaxValue.Value}");
            return imageWithMaxValue;
        }

        public List<double> FocusValues => ImagesWithFocusValues.Values.ToList() ?? new List<double>();
        public int TrackedImagesCount => ImagesWithFocusValues.Count;

        public List<KeyValuePair<USPImage, double>> TimeOrderedImages => (from image in ImagesWithFocusValues orderby image.Key.Timestamp ascending select image).ToList();

        public string PrintTimedFocusValues()
        {
            var result = new StringBuilder();
            foreach (var image in TimeOrderedImages)
            {
                result.AppendLine($"{image.Key.Timestamp.ToUniversalTime().Ticks} {image.Value}");
            }
            return result.ToString();
        }

        public virtual void WaitFocusValuesComputation(bool logDuration = false)
        {
            Task.WaitAll(_analyseImageTasks.Cast<Task>().ToArray());

            var startingTime_ms = DateTime.Now;
            SpinWait.SpinUntil(() => ImagesWithFocusValues.Count >= _imagesCount);
            if (logDuration) _logger.Debug($"WaitFocusValuesComputation total duration = {(DateTime.Now - startingTime_ms).TotalMilliseconds} ms.");
        }

        private double GetFocusValue(USPImage image)
        {
            lock (_computeFocusLock)
            {
                return _imageOperatorsLib.ComputeFocusMeasure(image);
            }
        }

        private void AnalyzeImageTask(CameraMessage message)
        {
            double focusValue = GetFocusValue((USPImage)message.Image);

            bool imageSuccessfullyAdded;
            if (_saveImages)
            {
                imageSuccessfullyAdded = ImagesWithFocusValues.TryAdd((USPImage)message.Image, focusValue);
            }
            else
            {
                var emptyImage = new USPImage() { Timestamp = message.Image.Timestamp };
                imageSuccessfullyAdded = ImagesWithFocusValues.TryAdd(emptyImage, focusValue);
            }

            if (!imageSuccessfullyAdded)
            {
                _logger.Error("Unable to add image.");
            }
            Interlocked.Increment(ref _imagesCount);
        }

        protected override void CameraMessageHandler(CameraMessage message)
        {
            _analyseImageTasks.Add(Task.Run(() => AnalyzeImageTask(message)));
        }

        public override void Dispose()
        {
            ImagesWithFocusValues.Clear();
            _analyseImageTasks.Clear();
        }

        public override void Reset()
        {
            ImagesWithFocusValues.Clear();
            _analyseImageTasks.Clear();
        }

        public void SaveImages(string directoryPath, PositionTracker positionTracker)
        {
            int imageNumber = 1;
            foreach (ICameraImage image in _chronologicalImages)
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
