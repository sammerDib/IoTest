using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    public class AutoFocusCameraFlow : FlowComponent<AutoFocusCameraInput, AutoFocusCameraResult, AutoFocusCameraConfiguration>
    {
        private const int DefaultAxesMoveTimeoutMs = 20_000;
        private const int MaxAxesReactionTimeMs = 1_000;

        private readonly IEmeraCamera _camera;
        private readonly MatroxCameraInfo _cameraInfo;
        private readonly FocusedImageTracker _focusedImageTracker;
        private readonly EmeHardwareManager _hardwareManager;

        private readonly ImageOperators _imageOperatorsLib;
        private readonly PhotoLumAxes _motionAxes;
        private readonly MotorizedAxisConfig _zAxisConfig;
        private readonly PositionTracker _zPositionTracker;

        private ScanRangeType _rangeType;

        private ScanRangeWithStep _scanRange;
        private double _zSpeedMmps;

        public AutoFocusCameraFlow(AutoFocusCameraInput input, IEmeraCamera camera) : base(input, "AutoFocusCameraFlow")
        {
            _camera = camera;
            _cameraInfo = camera.GetMatroxCameraInfo();
            _hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();

            var motionAxes = _hardwareManager.MotionAxes as PhotoLumAxes;
            if (motionAxes != null)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            var zAxisConfig =
                _motionAxes.AxesConfiguration.AxisConfigs.FirstOrDefault(axisConfig =>
                    axisConfig.MovingDirection == MovingDirection.Z);
            if (zAxisConfig is MotorizedAxisConfig motorizedAxisConfig)
            {
                _zAxisConfig = motorizedAxisConfig;
            }
            else
            {
                throw new Exception("AxisConfig for Z Axis should be a MotorizedAxisConfig.");
            }

            if (Input != null)
            {
                _rangeType = Input.RangeType;
            }

            bool saveImages = Configuration.IsAnyReportEnabled();
            _imageOperatorsLib = new ImageOperators();
            var emeraCamera = _hardwareManager.Cameras.FirstOrDefault().Value;
            _focusedImageTracker = new FocusedImageTracker(emeraCamera, saveImages, null, _imageOperatorsLib);
            _zPositionTracker = new PositionTracker(GetZPosition, Configuration.PositionTrackingPeriod_ms);
        }

        protected override void Process()
        {
            var stopWatchAutoFocusCamera = new Stopwatch();
            stopWatchAutoFocusCamera.Start();

            try
            {
                PrepareHardwareSettings();

                ExecuteAutofocus();

                // If range type was not small, run autofocus one more time
                // with a small range type to refine focus position
                if (Input.RangeType != ScanRangeType.Small)
                {
                    _rangeType = ScanRangeType.Small;
                    ExecuteAutofocus();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} An error occured: {ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
                throw;
            }
            finally
            {
                // Restore image resolution and AOI
                var fullAOI = new Rect(0, 0, _cameraInfo.MaxWidth,_cameraInfo.MaxHeight); 
                _camera.StopAcquisition();
                _camera.SetColorMode(Interface.Camera.ColorMode.Mono16);
                _camera.SetAOI(fullAOI);
                _camera.StartAcquisition();

                _focusedImageTracker.StopTracking();
                _zPositionTracker.StopTracking();
                _focusedImageTracker.Dispose();
                _zPositionTracker.Dispose();
            }

            stopWatchAutoFocusCamera.Stop();
            Logger.Information(
                $"{LogHeader} Autofocus was successful at sensor distance {Result.SensorDistance} in {stopWatchAutoFocusCamera.Elapsed}");
        }

        private void PrepareHardwareSettings()
        {
            int nbLines = Configuration.CameraNbLinesAOI;
            int topLeftY = _cameraInfo.Height / 2 - nbLines / 2;
            var aoi = new Rect(0, topLeftY, _cameraInfo.Width, nbLines);
            _camera.StopAcquisition();
            _camera.SetColorMode(Interface.Camera.ColorMode.Mono8);
            _camera.SetAOI(aoi);
            _camera.StartAcquisition();
        }

        private void ExecuteAutofocus()
        {
            double currentPositionZ = _motionAxes.GetPosition().ToXYZPosition().Z;

            if (_rangeType == ScanRangeType.Configured)
            {
                _scanRange = Input.ScanRangeConfigured;
            }
            else
            {
                Length positionMinWithMargin = _zAxisConfig.PositionMin + 0.01.Millimeters();
                Length positionMaxWithMargin = _zAxisConfig.PositionMax - 0.01.Millimeters();
                _scanRange = GetScanRange(_rangeType, Configuration, currentPositionZ, positionMinWithMargin,
                    positionMaxWithMargin);
            }

            var zFocusPosition = FindFocusWithAxis();
            WriteReportIfNeeded(zFocusPosition);

            GoToZPosition(zFocusPosition);
            Result.SensorDistance = _hardwareManager.DistanceSensor.GetDistanceSensorHeight();
        }

        /// <summary>
        ///     Phase 1: approximate Z focus position with the current camera settings (framerate and exposure time)
        ///     -- step 1. Calculate the axis motion speed from the current camera framerate value (in fps) and the scanRange.Step
        ///     --         (distance in mm between two consecutive images). Retrieve the initial and target positions.
        ///     -- step 2. Place the camera at its initial position (scanRange.ZMin).
        ///     -- step 3. Move (at speed value found in step 1) to the target position (scanRange.ZMax) while tracking images
        ///     --         and Z positions during the camera motion. Every image's focus value is computed in a separated
        ///     --         thread as soon as it is receive by the camera message handler.
        ///     -- step 4. Deduce the Z focus position from the image with the maximum focus value (linear interpollation from
        ///     --         Z positions tracked during step 3). = ZFocusPosition
        /// </summary>
        private Length FindFocusWithAxis()
        {
            // STEP 1
            CheckCancellation();
            _zSpeedMmps = GetZSpeed();
            var initialPosition = _scanRange.Min.Millimeters();
            var targetPosition = _scanRange.Max.Millimeters();
            double scanRange = Math.Abs(targetPosition.Millimeters - initialPosition.Millimeters);
            double expectedScanDurationMs = TimeSpan.FromSeconds(scanRange / _zSpeedMmps).TotalMilliseconds;
            int scanTimeout = (int)expectedScanDurationMs + MaxAxesReactionTimeMs;

            // STEP 2
            CheckCancellation();
            GoToZPosition(initialPosition);

            // STEP 3 Start tracking images and Z positions and move to the target position
            CheckCancellation();
            var stopWatchScan = new Stopwatch();
            stopWatchScan.Start();
            GoToZPositionAndTrackImages(targetPosition, scanTimeout);
            stopWatchScan.Stop();

            Logger.Debug(
                $"{LogHeader} Scan duration (expected | real) = {expectedScanDurationMs} ms | {stopWatchScan.ElapsedMilliseconds} ms.");
            Logger.Debug(
                $"{LogHeader} Scan speed (expected | real) = {_zSpeedMmps} mm/s | {scanRange / (stopWatchScan.ElapsedMilliseconds / 1000)} mm/s.");

            // Wait until all images have an associated focus value
            CheckCancellation();
            _focusedImageTracker.WaitFocusValuesComputation(true);

            int expectedImagesCount = (int)Math.Floor(scanRange / _scanRange.Step) + 1;
            int realImagesCount = _focusedImageTracker.TrackedImagesCount;
            Logger.Debug($"{LogHeader} Images count (expected | real) = {expectedImagesCount} | {realImagesCount}.");

            // STEP 4
            CheckCancellation();
            var imageWithMaxFocusValue = _focusedImageTracker.GetImageWithMaxFocusValue();

            var focusImage = imageWithMaxFocusValue.Key;

            long maxFocusValueTime = focusImage.Timestamp.ToUniversalTime().Ticks;
            var focusPosition = _zPositionTracker.GetPositionAtTime(maxFocusValueTime);

            return focusPosition;
        }

        private void WriteReportIfNeeded(Length focusPosition)
        {
            try
            {
                if (!Configuration.IsAnyReportEnabled())
                {
                    return;
                }

                var focusValuesAtPosition = new List<Tuple<double, double>>();
                var imageWithMaxFocusValue = _focusedImageTracker.GetImageWithMaxFocusValue();
                var focusImage = imageWithMaxFocusValue.Key;

                foreach (var imgWithFocusValue in _focusedImageTracker.ImagesWithFocusValues)
                {
                    var img = imgWithFocusValue.Key;
                    double focusValue = imgWithFocusValue.Value;
                    long acquisitionTimestamp = img.Timestamp.ToUniversalTime().Ticks;
                    var acquisitionPosition = _zPositionTracker.GetPositionAtTime(acquisitionTimestamp);
                    focusValuesAtPosition.Add(new Tuple<double, double>(acquisitionPosition.Millimeters, focusValue));
                }

                var orderedFocusValueAtPosition = focusValuesAtPosition.OrderBy(x => x.Item1).ToList();

                string csvFilname = $"focus_function_of_position_with_{_zAxisConfig.AxisID}_csharp.csv";
                SignalReport.WriteSignalInCSVFormat("Axe position (millimeters)", "Focus value",
                    orderedFocusValueAtPosition, Path.Combine(ReportFolder, csvFilname));

                string imageFocusFilename =
                    $"0_autofocusCamera_image{_zAxisConfig.AxisID}_at_focus_position_{focusPosition}_scan_range_type_{_rangeType}_csharp.png";
                if (focusImage is USPImageMil mil)
                {
                    ImageReport.SaveImage(mil, Path.Combine(ReportFolder, imageFocusFilename));
                }
                else
                {
                    ImageReport.SaveImage(focusImage as USPImage, Path.Combine(ReportFolder, imageFocusFilename));
                }

                _focusedImageTracker.SaveImages(ReportFolder, _zPositionTracker);
            }
            catch (Exception)
            {
                Logger.Debug($"Failed to generate the report for the AutoFocusCameraFlow");
            }
        }

        private double GetZSpeed()
        {
            var cameraInfo = _camera.GetMatroxCameraInfo();
            double cameraFramerate = cameraInfo.MaxFrameRate * Configuration.CameraFramerateLimiter;
            var zStep = _scanRange.Step.Millimeters();
            return Math.Min(cameraFramerate * zStep.Millimeters, _zAxisConfig.SpeedMaxScan);
        }

        /// <summary>
        ///     Orders the Z Axis to move at the given position. Default speed and acceleration values are retrieve from
        ///     the axis configuration (respectively SpeedMaxScan and AccelNormal).
        /// </summary>
        private void GoToZPosition(Length targetPosition)
        {
            double speed = _zAxisConfig.SpeedNormal;

            Logger.Debug(
                $"{LogHeader} Move Z axis to position {targetPosition.Millimeters} with speed = {speed} mm/s.");

            string axisIDz = _motionAxes.AxesConfiguration.AxisConfigs
                .FirstOrDefault(a => a.MovingDirection == MovingDirection.Z)?.AxisID;
            _motionAxes.Move(new PMAxisMove(axisIDz, targetPosition, new Speed(speed)));
            _motionAxes.WaitMotionEnd(DefaultAxesMoveTimeoutMs, false);
            Logger.Debug($"{LogHeader} Movemement Done.");
        }

        /// <summary>
        ///     Same function as GoToZPosition, but starts images and positions tracking just before ask for move. Default
        ///     speed and acceleration values are retrieve from the axis configuration (respectively SpeedMaxScan and AccelNormal).
        /// </summary>
        private void GoToZPositionAndTrackImages(Length targetPosition, int timeout)
        {
            double speed = _zSpeedMmps;

            Logger.Debug(
                $"{LogHeader} Move Z axis to position {targetPosition.Millimeters} with speed = {speed} mm/s.");

            _zPositionTracker.Reset();
            _focusedImageTracker.Reset();

            _zPositionTracker.StartTracking();
            _focusedImageTracker.StartTracking();

            string axisIDz = _motionAxes.AxesConfiguration.AxisConfigs
                .FirstOrDefault(a => a.MovingDirection == MovingDirection.Z)?.AxisID;
            _motionAxes.Move(new PMAxisMove(axisIDz, targetPosition, new Speed(speed)));
            _motionAxes.WaitMotionEnd(timeout, false);

            _focusedImageTracker.StopTracking();
            _zPositionTracker.StopTracking();
        }

        public ScanRangeWithStep GetScanRange(ScanRangeType rangeType, AutoFocusCameraConfiguration flowConfiguration,
            double scanCenter, Length axisMinLimit, Length axisMaxLimit)
        {
            switch (rangeType)
            {
                case ScanRangeType.AllAxisRange:
                    return new ScanRangeWithStep(axisMinLimit.Millimeters, axisMaxLimit.Millimeters, flowConfiguration.MaxZStep.Millimeters);

                case ScanRangeType.Small:
                case ScanRangeType.Medium:
                case ScanRangeType.Large:
                    return GetDynamicRange(rangeType, flowConfiguration, scanCenter, axisMinLimit, axisMaxLimit);

                default:
                    throw new ArgumentOutOfRangeException(nameof(rangeType), $"Invalid scan range type: {rangeType}");
            }
        }

        private ScanRangeWithStep GetDynamicRange(ScanRangeType rangeType, AutoFocusCameraConfiguration flowConfiguration,
            double zScanCenter, Length axisMinLimit, Length axisMaxLimit)
        {
            double range = flowConfiguration.AutoFocusScanRange.Millimeters;

            var rangeCoefficients = new Dictionary<ScanRangeType, double>
            {
                { ScanRangeType.Small, flowConfiguration.SmallRangeCoeff },
                { ScanRangeType.Medium, flowConfiguration.MediumRangeCoeff },
                { ScanRangeType.Large, flowConfiguration.LargeRangeCoeff }
            };

            if (rangeCoefficients.TryGetValue(rangeType, out double coeff))
            {
                range *= coeff;
            }
            else
            {
                throw new Exception("Invalid ScanRangeType");
            }
            
            double min = Math.Max(zScanCenter - range / 2.0, axisMinLimit.Millimeters);
            double max = Math.Min(zScanCenter + range / 2.0, axisMaxLimit.Millimeters);
            
            double stepSize = rangeType == ScanRangeType.Small
                ? flowConfiguration.MinZStep.Millimeters
                : flowConfiguration.MaxZStep.Millimeters;

            return new ScanRangeWithStep(min, max, stepSize);
        }

        private TimestampedPosition GetZPosition()
        {
            var pos = _motionAxes.GetPosition().ToXYZPosition();
            var posLength = new Length(pos.Z, LengthUnit.Millimeter);
            //TODO : GET A MORE ACCURATE TIMESTAMP USING THE CONTROLLER LIKE IN NSTAXES WITH THE STOPWATCH & ADDTICK & CONTROLLER REFRESHTIMESTAMP AT THE BEGINNING OF THE FLOW
            var timestampedPosition = new TimestampedPosition(posLength, DateTime.UtcNow);
            return timestampedPosition;
        }
    }
}
