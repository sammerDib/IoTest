using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using uEye.Defines;
using uEye.Types;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Configuration;

namespace UnitySC.PM.Shared.Hardware.Camera.IDSCamera
{
    public abstract class IDSCameraBase : USPCameraBase
    {
        protected new IDSCameraConfigBase Config;

        protected uEye.Camera Camera;

        private USPImage _lastImage;
        private bool _settingsHasChanged;

        private System.Timers.Timer _timer;
        private System.Timers.Timer _timerRefreshRealFramerate;

        public override double MinGain { get => 1; }
        public override double MaxGain { get => 4; }

        protected IDSCameraBase(IDSCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            Config = config;
        }

        #region LIFECYCLE

        public override void Init()
        {
            base.Init();
            uEye.Info.Camera.GetCameraList(out var cameraInfos);

            var cameraInfo = cameraInfos.Where(x => x.SerialNumber == Config.SerialNumber); // FIXME: breaking behaviour if SerialNumber is missing from Config
            if (!cameraInfo.Any())
                throw new ApplicationException($"The camera with serial number {Config.SerialNumber} is not present");

            Camera = new uEye.Camera();
            int deviceID = cameraInfo.First().DeviceID;

            ApiErrorHandler(Camera.Init(deviceID | (int)DeviceEnumeration.UseDeviceID));

            string filePath = Path.Combine(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().ConfigurationFolderPath, Config.ParameterFileRelativePath);
            LoadParametersFromFile(filePath);

            AllocateMemory(ImageResolution);

            SetColorMode(Config.ColorMode); // FIXME: breaking behaviour if ColorMode is missing from Config
            SetRopEffect(Config.RopEffect); // FIXME: breaking behaviour if RopEffect is missing from Config
            SetGain(Config.Gain);           // FIXME: breaking behaviour if Gain is missing from Config
            InitPixelClock(0.9);            // Limitate pixel clock at 90% to limit overheating and bandwith usage. // TODO : add in config
            SetCameraMode(Mode.Bright);

            Camera.EventFrame += delegate { onFrameEvent(); };

            _timerRefreshRealFramerate = new System.Timers.Timer(Config.RefreshFramerateInterval_ms) { Enabled = true };
            _timerRefreshRealFramerate.Elapsed += delegate { RefreshRealFramerate(); };

            Logger.Information($"{Name} initialized with success.\n{GetSettings()}");
        }

        private void LoadParametersFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"Camera parameter file not found at location '{filePath}'.");
            }

            ApiErrorHandler(Camera.Parameter.Load(filePath));
        }

        public virtual void Dispose()
        {
            WaitForMemorySequenceUnlock().Wait();

            ApiErrorHandler(Camera.Memory.GetList(out var idList));
            ApiErrorHandler(Camera.Memory.Free(idList));
            ApiErrorHandler(Camera.Memory.Sequence.ExitImageQueue());
            ApiErrorHandler(Camera.Memory.Sequence.Clear());

            ApiErrorHandler(Camera.Exit());
        }

        public override void Shutdown()
        {
            Invoke(() =>
            {
                if (IsAcquiring)
                    throw new ApplicationException("Camera is still acquiring");

                _lastImage = null;

                Dispose();

                State = new DeviceState(DeviceStatus.Unknown, "Camera has been shut down");
                Logger.Information($"{Name} shutdown, status : {State.StatusMessage}");
            });
        }

        #endregion LIFECYCLE

        #region MEMORY

        /// <summary>
        /// Allocates an image memory sequence (ring buffering) for a given image resolution. The size of the buffer can
        /// be specified in `Config.ImageMemoryBufferCount`.
        /// </summary>
        private void AllocateMemory(System.Windows.Size imageResolution)
        {
            ApiErrorHandler(Camera.Memory.Sequence.GetList(out int[] previousImageMemoryIDs));

            if (previousImageMemoryIDs.Any())
            {
                // Allocated memory is already set for that image resolution => early return
                if (ImageResolution.Width == imageResolution.Width && ImageResolution.Height == imageResolution.Height)
                {
                    return;
                }

                // A memory allocation with distinct image resolution exists => need to unallocate memory
                // Before, assert that all memory sequence are unlocked
                WaitForMemorySequenceUnlock().Wait();

                ApiErrorHandler(Camera.Memory.GetList(out var idList));
                ApiErrorHandler(Camera.Memory.Free(idList));
                ApiErrorHandler(Camera.Memory.Sequence.ExitImageQueue());
                ApiErrorHandler(Camera.Memory.Sequence.Clear());
            }

            // Memory allocation for with the requested image resolution
            var imageMemoryIDs = new List<int>();
            foreach (int _ in Enumerable.Range(1, Config.ImageMemoryBufferCount))
            {
                ApiErrorHandler(Camera.Memory.Allocate((int)imageResolution.Width, (int)imageResolution.Height, DefaultImageBpp, true, out int imageMemoryId));
                imageMemoryIDs.Add(imageMemoryId);
            }

            ApiErrorHandler(Camera.Memory.Sequence.Add(imageMemoryIDs.ToArray()));
            ApiErrorHandler(Camera.Memory.Sequence.InitImageQueue());
            ImageResolution = new System.Windows.Size(imageResolution.Width, imageResolution.Height);
        }

        private List<int> GetImageMemorySequence()
        {
            ApiErrorHandler(Camera.Memory.Sequence.GetList(out int[] ids));
            return ids.ToList();
        }

        #endregion MEMORY

        #region CAMERA PROPERTIES

        public override string Model
        {
            get
            {
                ApiErrorHandler(Camera.Information.GetSensorInfo(out var sensorInfo));
                return sensorInfo.SensorName;
            }
        }

        public override string SerialNumber
        {
            get
            {
                ApiErrorHandler(Camera.Information.GetCameraInfo(out var cameraInfo));
                return cameraInfo.SerialNumber;
            }
        }

        public override string Version
        {
            get
            {
                ApiErrorHandler(uEye.Info.System.GetApiVersion(out System.Version version));
                return version.ToString();
            }
        }

        #endregion CAMERA PROPERTIES

        #region CAMERA MODE

        public void SetCameraMode(Mode mode)
        {
            var cameraMode = Config.CameraModes.Find(m => m.Mode == mode);
            if (cameraMode.Mode == Mode.Unknown)
            {
                throw new Exception($"Camera mode {mode} not found.");
            }

            SetFrameRate(cameraMode.Framerate);
            SetExposureTimeMs(cameraMode.ExposureTimeMs);
        }

        #endregion CAMERA MODE

        #region AOI

        public override Rect GetAOI()
        {
            ApiErrorHandler(Camera.Size.AOI.Get(out var rectangle));
            var rectFromRectangle = new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            return rectFromRectangle;
        }

        /// <summary>
        /// Sets the image AOI (area of interest). If Rectangle.Empty is given, AOI is set to the current image
        /// resolution. Note that for IDS camera, the available sizes for AOI are multiple of 4 for width and multiple
        /// of 2 for height. If given AOI is not the right size, it will be automatically resize.
        /// For AOI positionning, note that coordinate (0, 0) is at bottom-left image corner.
        /// </summary>
        /// <param name="aoi"></param>
        public override void SetAOI(Rect aoi)
        {
            ApiErrorHandler(Camera.Size.AOI.Get(out var currentAOI));
            if (aoi == Rect.Empty)
            {
                aoi = new Rect(0, 0, ImageResolution.Width, ImageResolution.Height);
            }

            var rectFromRectangle = new Rect(currentAOI.X, currentAOI.Y, currentAOI.Width, currentAOI.Height);

            if (rectFromRectangle == aoi)
            {
                return;
            }

            bool isCurrentlyAquiring = IsAcquiring;
            if (isCurrentlyAquiring)
            {
                StopContinuousGrab();
            }

            // AOI repositioning at bottom-left image corner (keep current AOI width and height values)
            ApiErrorHandler(Camera.Size.AOI.Set(new Rectangle(0, 0, currentAOI.Width, currentAOI.Height)));

            // Control that AOI width and height values are valid
            var aoiSizeRange = GetAOISizeRange();

            var widthValidValues = RangeValues(aoiSizeRange.Width);
            if (!widthValidValues.Contains((int)aoi.Width))
            {
                ApiErrorHandler(Camera.Size.AOI.GetSizeRange(out int minWidth, out int minHeight, out int maxWidth, out int maxHeight, out int incWidth, out int incHeight));
                var widthModRes = aoi.Width % incWidth;
                aoi.Width -= widthModRes;
                aoi.Width = Math.Max(aoi.Width, minWidth);
            }

            var heightValidValues = RangeValues(aoiSizeRange.Height);
            if (!heightValidValues.Contains((int)aoi.Height))
            {
                ApiErrorHandler(Camera.Size.AOI.GetSizeRange(out int minWidth, out int minHeight, out int maxWidth, out int maxHeight, out int incWidth, out int incHeight));
                var heightModRes = aoi.Height % incHeight;
                aoi.Height -= heightModRes;
                aoi.Height = Math.Max(aoi.Height, minHeight);
            }

            var rectangleFromRect = new Rectangle((int)aoi.X, (int)aoi.Y, (int)aoi.Width, (int)aoi.Height);

            // Set AOI
            ApiErrorHandler(Camera.Size.AOI.Set(rectangleFromRect));

            // Reallocate image memory
            AllocateMemory(new System.Windows.Size((int)aoi.Width, (int)aoi.Height));

            // Restart image stream if camera was acquiring
            if (isCurrentlyAquiring)
            {
                StartContinuousGrab();
            }
            _settingsHasChanged = true;
        }

        private (Range<int> Width, Range<int> Height) GetAOISizeRange()
        {
            ApiErrorHandler(Camera.Size.AOI.GetSizeRange(out var rangeWidth, out var rangeHeight));
            return (rangeWidth, rangeHeight);
        }

        private List<int> RangeValues(Range<int> range)
        {
            int valuesCount = ((range.Maximum - range.Minimum) / range.Increment) + 1;
            return Enumerable
                    .Repeat(range.Minimum, valuesCount)
                    .Select((minimumValue, index) => minimumValue + (range.Increment * index))
                    .ToList();
        }

        #endregion AOI

        #region IMAGE PROPERTIES

        private const int DefaultImageBpp = 8; // FIXME: put me in Config

#pragma warning disable CS0809 // An obsolete member replaces a non-obsolete member

        [Obsolete("Deprecated, please use GetImageResolution().Height instead.")]
        public override int Height => (int)ImageResolution.Height;

#pragma warning disable CS0809 // An obsolete member replaces a non-obsolete member

        [Obsolete("Deprecated, please use GetImageResolution().Width instead.")]
        public override int Width => (int)ImageResolution.Width;

        public override void SetImageResolution(System.Windows.Size imageResolution)
        {
            bool isCurrentlyAquiring = IsAcquiring;

            StopContinuousGrab();
            AllocateMemory(imageResolution);

            if (isCurrentlyAquiring)
            {
                StartContinuousGrab();
            }
        }

        #endregion IMAGE PROPERTIES

        #region COLOR MODE

        public override string GetColorMode()
        {
            Camera.PixelFormat.Get(out var colorMode);
            return colorMode.ToString();
        }

        public override List<string> ColorModes => GetColorModes();

        public override List<string> GetColorModes()
        {
            var colorModesList = new List<string> { "Mono8" };
            return colorModesList;
        }

        public override void SetColorMode(string colorMode)
        {
            Enum.TryParse<ColorMode>(colorMode, out var mode);
            ApiErrorHandler(Camera.PixelFormat.Set(mode));
        }

        #endregion COLOR MODE

        #region EXPOSURE TIME

        private double _exposureTime_ms { get; set; }

        /// <summary>
        /// Returns the currently set exposure time (in ms).
        /// </summary>
        public override double GetExposureTimeMs()
        {
            ApiErrorHandler(Camera.Timing.Exposure.Get(out double exposureTimeMs));
            return exposureTimeMs;
        }

        /// <summary>
        /// Sets the exposure time (in ms).
        /// </summary>
        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            if (exposureTime_ms == 0)
            {
                Logger.Warning($"{Name} Set the camera exposure time to 0 is not allowed. Assignment skiped.");
                return;
            }

            if (exposureTime_ms == _exposureTime_ms)
            {
                return;
            }

            double minExposureTimeMs = MinExposureTimeMs;
            double maxExposureTimeMs = MaxExposureTimeMs;

            bool outOfMinBound = exposureTime_ms < minExposureTimeMs;
            bool outOfMaxBound = maxExposureTimeMs < exposureTime_ms;

            if (outOfMinBound || outOfMaxBound)
            {
                double actualExposureTime_ms = GetExposureTimeMs();
                Logger.Warning($"{Name} Exposure time ({exposureTime_ms} ms) out of bounds ([min, max] = [{minExposureTimeMs}, {maxExposureTimeMs}]). " +
                    $"Actual exposure time is {actualExposureTime_ms} ms.");
                return;
            }

            ApiErrorHandler(Camera.Timing.Exposure.Set(exposureTime_ms));
            _exposureTime_ms = exposureTime_ms;
            _settingsHasChanged = true;
        }

        public override double MinExposureTimeMs => GetExposureTimeRange().Minimum;
        public override double MaxExposureTimeMs => GetExposureTimeRange().Maximum;

        private Range<double> GetExposureTimeRange()
        {
            ApiErrorHandler(Camera.Timing.Exposure.GetRange(out var range));
            return range;
        }

        #endregion EXPOSURE TIME

        #region FRAMERATE

        private double _framerate { get; set; }

        public override double GetFrameRate()
        {
            ApiErrorHandler(Camera.Timing.Framerate.Get(out double frameRate));
            return frameRate;
        }

        /// <summary>
        /// Command the camera to run at given framerate.
        /// Note that the exposure time will be set to its maximum value for the given framerate.
        /// </summary>
        /// <param name="framerate"></param>
        public override void SetFrameRate(double framerate)
        {
            if (framerate == _framerate)
            {
                return;
            }

            var maxFrameRate = MaxFrameRate;
            framerate = framerate > maxFrameRate ? maxFrameRate : framerate;

            ApiErrorHandler(Camera.Timing.Framerate.Set(framerate));
            _framerate = framerate;

            ApiErrorHandler(Camera.Timing.Exposure.Get(out double newExposureTime));
            _exposureTime_ms = newExposureTime;

            _settingsHasChanged = true;
        }

        /// <summary>
        /// Command the camera to run at 90% of its maximum framerate according to the current exposure time.
        /// </summary>
        public void SetMaxFramerate()
        {
            double currentExposureTime = GetExposureTimeMs();
            double maxFramerate = Math.Floor(Math.Min(1_000 / currentExposureTime, MaxFrameRate));

            SetFrameRate(maxFramerate);
        }

        public new double MinFrameRate => GetFrameRateRange().Minimum;

        public override double MaxFrameRate
        {
            get
            {
                return GetFrameRateRange().Maximum - 1;
            }
        }

        private Range<double> GetFrameRateRange()
        {
            ApiErrorHandler(Camera.Timing.Framerate.GetFrameRateRange(out var range));
            return range;
        }

        private int _imageCountDiff;

        /// <summary>
        /// When camera is acquiring, returns the real framerate value (actualized every second).
        /// </summary>
        public override double GetRealFramerate()
        {
            if (!IsAcquiring)
            {
                Logger.Warning($"{Name} is not acquiring. Impossible to log the real framerate.");
            }

            return _imageCountDiff * 1_000 / Config.RefreshFramerateInterval_ms;
        }

        private void RefreshRealFramerate()
        {
            _imageCountDiff = _imageCount - _previousImageCount;
            _previousImageCount = _imageCount;
        }

        public void EnableLogFramerate(int period_ms = 1_000)
        {
            if (period_ms < Config.RefreshFramerateInterval_ms)
            {
                Logger.Warning($"{Name} The log period of the real camera framerate value should be greater that the RefreshFramerateInterval parameter set in the Config, witch has a value of {Config.RefreshFramerateInterval_ms} ms. This value will be considered as the log period.");
            }

            _timer = new System.Timers.Timer(Math.Max(period_ms, Config.RefreshFramerateInterval_ms)) { Enabled = true };
            _timer.Elapsed += LogFramerate;
            _timer.Start();
        }

        public void DisableLogFramerate()
        {
            _timer.Elapsed -= LogFramerate;
            _timer?.Stop();
            _timer?.Dispose();
        }

        private void LogFramerate(object sender, ElapsedEventArgs e)
        {
            Logger.Information($"{Name} running at {GetRealFramerate()} fps. (total images = {_imageCount})");
        }

        #endregion FRAMERATE

        #region GAIN

        public override double GetGain()
        {
            Camera.Gain.Hardware.Scaled.GetMaster(out int percent);
            double gain = (percent / 100 * (MaxGain - MinGain)) + MinGain;
            return gain;
        }

        public override void SetGain(double gain)
        {
            double percent = (gain - MinGain) / (MaxGain - MinGain); // Convert multiplier to percentage
            ApiErrorHandler(Camera.Gain.Hardware.Scaled.SetMaster((int)(percent * 100)));
            _settingsHasChanged = true;
        }

        #endregion GAIN

        #region PIXEL CLOCK

        private int _pixelClock { get; set; }

        private int GetPixelClock()
        {
            ApiErrorHandler(Camera.Timing.PixelClock.Get(out int pixelClock));
            return pixelClock;
        }

        private void SetPixelClock(int pixelClock)
        {
            if (pixelClock == _pixelClock)
            {
                return;
            }

            bool pixelClockValueNotSupported = !GetPixelClockSupportedValues().Contains(pixelClock);
            if (pixelClockValueNotSupported)
            {
                throw new Exception($"PixelClock value '{pixelClock}' is not supported.");
            }

            ApiErrorHandler(Camera.Timing.PixelClock.Set(pixelClock));
            _pixelClock = pixelClock;
            _settingsHasChanged = true;
        }

        public int GetMinPixelClock() => GetPixelClockRange().Minimum;

        public int GetMaxPixelClock()
        {
            int[] pixelClockSupportedValues = GetPixelClockSupportedValues();
            int penultimatePixelClockIndex = pixelClockSupportedValues.Length - 2;

            // return the penultimate value since the maximum value may generates delays or transmission errors
            return pixelClockSupportedValues[penultimatePixelClockIndex];
        }

        /// <summary>
        /// Returns the pixel clock range with minimum, maximum and increment values. /!\ When the camera has only
        /// discrete pixel clocks values, increment = 0. That means that the increment may vary between two consecutive
        /// pixel clock values. In such case, perfer using GetPixelClockSupportedValues().
        /// </summary>
        private Range<int> GetPixelClockRange()
        {
            ApiErrorHandler(Camera.Timing.PixelClock.GetRange(out var range));
            return range;
        }

        private int[] GetPixelClockSupportedValues()
        {
            ApiErrorHandler(Camera.Timing.PixelClock.GetList(out int[] supportedValues));
            return supportedValues;
        }

        private void InitPixelClock(double ratio)
        {
            var maxPixelClock = GetMaxPixelClock();
            var newPixelClock = (int)(GetMaxPixelClock() * ratio);

            if (newPixelClock >= maxPixelClock)
            {
                newPixelClock = maxPixelClock;
            }

            while (!GetPixelClockSupportedValues().Contains(newPixelClock) && newPixelClock < maxPixelClock)
            {
                newPixelClock++;
            }

            SetPixelClock(newPixelClock);
        }

        #endregion PIXEL CLOCK

        #region ROP EFFECT

        private void SetRopEffect(string ropEffect)
        {
            Enum.TryParse<RopEffectMode>(ropEffect, out var ropEffectMode);
            ApiErrorHandler(Camera.RopEffect.Set(ropEffectMode, true));
        }

        #endregion ROP EFFECT

        #region TRIGGER

        public override void SetTriggerMode(TriggerMode mode)
        {
            switch (mode)
            {
                case TriggerMode.Off:
                    Camera.Trigger.Set(uEye.Defines.TriggerMode.Off);
                    break;

                case TriggerMode.Software:
                    Camera.Trigger.Set(uEye.Defines.TriggerMode.Software);
                    break;

                case TriggerMode.Hardware:
                    throw new NotImplementedException();
                default:
                    throw new ApplicationException("unknown trigger mode: " + mode);
            }
        }

        public override void SoftwareTrigger()
        {
            Camera.Trigger.Get(out var triggerMode);
            if (triggerMode == uEye.Defines.TriggerMode.Software)
                Camera.Acquisition.Freeze();
        }

        #endregion TRIGGER

        #region IMAGE PROCESSING

        public override USPImage SingleGrab()
        {
            Logger.Debug($"{Name} SingleGrab");
            LogSettingsIfNeeded();

            if (IsAcquiring)
            {
                StopContinuousGrab();
            }

            Camera.Acquisition.Freeze();
            WaitForSoftwareTriggerGrabbed();

            var image = (USPImage)_lastImage.Clone();
            _lastImage = null;

            return image;
        }

        public override void StartContinuousGrab()
        {
            if (IsAcquiring)
            {
                Logger.Debug($"{Name} is already acquiring images.");
                return;
            }

            Logger.Debug($"{Name} starts acquiring images (StartContinousGrab)");
            LogSettingsIfNeeded();

            WaitForMemorySequenceUnlock().Wait();
            ApiErrorHandler(Camera.Acquisition.Capture(1)); // 1 means Wait until the camera succeeds to Start image acquisition
            Camera.Information.GetSensorInfo(out var sensorInfo);
            IsAcquiring = true;
            _timerRefreshRealFramerate.Start();
        }

        public override void StopContinuousGrab()
        {
            if (!IsAcquiring)
            {
                Logger.Debug($"{Name} is not acquiring images.");
                return;
            }

            Logger.Debug($"{Name} stops acquiring images (StopContinuousGrab)");

            IsAcquiring = false;
            ApiErrorHandler(Camera.Acquisition.Stop());

            _lastImage = null;
            _timerRefreshRealFramerate.Stop();
            _imageCountDiff = 0;

            // Call to Messenger.Cleanup is absolutly necessary. Otherwise, the messenger will slow down and cause frame losses
            Messenger.Cleanup();
        }

        public void RestartContinuousGrab()
        {
            if (IsAcquiring)
            {
                StopContinuousGrab();
            }

            StartContinuousGrab();
        }

        public override void WaitForSoftwareTriggerGrabbed()
        {
            const int timeout_ms = 1_000;
            bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() => !(_lastImage is null), timeout_ms);

            if (!hasStoppedWithinTimeout)
            {
                throw new TimeoutException($"Exits on timeout ({timeout_ms} ms).");
            }
        }

        private int _previousImageCount;
        private int _imageCount;

        private void onFrameEvent()
        {
            int seqID = -1;
            try
            {
                Interlocked.Increment(ref _imageCount); // used for real framerate value

                ApiErrorHandler(Camera.Memory.Sequence.GetLast(out seqID));
                ApiErrorHandler(Camera.Memory.Sequence.Lock(seqID));
                ApiErrorHandler(Camera.Memory.Sequence.ToMemoryID(seqID, out int imageMemoryId));
                ApiErrorHandler(Camera.Information.GetImageInfo(imageMemoryId, out var imageInfo));
                ApiErrorHandler(Camera.Memory.ToIntPtr(imageMemoryId, out var ptr));
                ApiErrorHandler(Camera.Memory.GetPitch(imageMemoryId, out int pitch));
                ApiErrorHandler(Camera.Size.AOI.Get(out var imageResolution));

                var currentImage = new USPImage(ptr, imageResolution.Width, imageResolution.Height, pitch)
                {
                    Timestamp = imageInfo.TimestampSystem
                };

                ApiErrorHandler(Camera.Memory.Sequence.Unlock(seqID));

                Messenger.Send(new CameraMessage() { Camera = this, Image = currentImage });

                _lastImage = currentImage;
            }
            catch (Exception ex)
            {
                // IsAcquiring false means that the camera acquisition is stop. If a running thread was processing a frameEvent
                // at the same time, an error could occur because the camera memory is no more accessible.
                // We don't want to lose time with a spinWait before call for a camera acquisition Stop, so we simply
                // catch this case here and do nothing.
                if (IsAcquiring)
                {
                    Logger.Error($"{Name} onFrameEvent error : {ex.Message}");
                    if (seqID != -1)
                    {
                        ApiErrorHandler(Camera.Memory.Sequence.GetLocked(seqID, out bool isLocked));
                        if (isLocked)
                        {
                            ApiErrorHandler(Camera.Memory.Sequence.Unlock(seqID));
                        }
                    }
                }
            }
        }

        private Task WaitForMemorySequenceUnlock()
        {
            var timeout_ms = 3_000;
            if (!SpinWait.SpinUntil(() => { return IsMemorySequenceUnlock(); }, timeout_ms))
            {
                throw new TimeoutException($"{Name} can't unlock memory sequence");
            }
            return Task.CompletedTask;
        }

        private bool IsMemorySequenceUnlock()
        {
            ApiErrorHandler(Camera.Memory.Sequence.GetList(out var idList));
            foreach (var seqId in idList)
            {
                ApiErrorHandler(Camera.Memory.Sequence.GetLocked(seqId, out bool isLocked));
                if (isLocked)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion IMAGE PROCESSING

        #region HELPERS

        protected void ApiErrorHandler(uEye.Defines.Status operationStatus)
        {
            if (operationStatus != uEye.Defines.Status.SUCCESS)
            {
                Camera.Information.GetLastError(out operationStatus, out string msg);
                throw new Exception($"An error occured for camera {Config.Name} S/N {Config.SerialNumber} status : {operationStatus} message : {msg}");
            }
        }

        public string GetSettings()
        {
            var state = new StringBuilder("\nCamera settings:\n");
            state.Append($"\tmodel = {Model}\n");
            state.Append($"\tserial number = {SerialNumber}\n");
            state.Append($"\tversion = {Version}\n");
            state.Append($"\tring buffering size = {GetImageMemorySequence().Count()}\n");
            state.Append($"\tcolor mode = {GetColorMode()}\n");
            state.Append($"\timage resolution = {ImageResolution}\n");
            state.Append($"\taoi = {GetAOI()}\n");
            state.Append($"\tpixelClock = {GetPixelClock()} (min = {GetMinPixelClock()}; max = {GetMaxPixelClock()})\n");
            state.Append($"\tframeRate = {GetFrameRate()} fps (min = {MinFrameRate} fps; max = {MaxFrameRate} fps)\n");
            state.Append($"\texposureTime = {GetExposureTimeMs()} (min = {MinExposureTimeMs} ms; max = {MaxExposureTimeMs} ms)\n");
            state.Append($"\tgain = {GetGain()}%\n");

            return state.ToString();
        }

        private void LogSettingsIfNeeded()
        {
            if (_settingsHasChanged)
            {
                Logger.Information($"{Name} with the following settings:\n{GetSettings()}");
                _settingsHasChanged = false;
            }
        }

        #endregion HELPERS
    }
}
