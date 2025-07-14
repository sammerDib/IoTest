using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.AutofocusV2
{
    internal struct CameraSettings
    {
        public Size ImageResolution { get; set; }
        public double FrameRate { get; set; }
        public double ExposureTime_ms { get; set; }
    }

    public class AFV2CameraFlow : FlowComponent<AFCameraInput, AFCameraResult, AFCameraConfiguration>
    {
        private const int DefaultAxesMoveTimeout_ms = 20_000;
        private const int MaxAxesReactionTime_ms = 1_000;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly IAxes _axes;
        private readonly MotorizedAxisConfig _zTopAxisConfig;
        private IAxesController _piezoController;
        private IAxis _piezoAxis;

        private readonly ICameraManager _cameraManager;
        private readonly CameraBase _camera;
        private ObjectiveConfig _currentObjectiveConfig;
        private CameraSettings _initialCameraSettings;

        private readonly ImageOperators _imageOperatorsLib;
        private readonly FocusedImageTracker _focusedImageTracker;
        private PositionTracker _zTopPositionTracker;
        private PositionTracker _piezoPositionTracker;

        private ScanRangeWithStep _scanRange;
        private ScanRangeType _rangeType;
        private double _zTopSpeed_mmps;
        private double _autoFocusLightFactor = 1.0;
        private Dictionary<string, double> _intensityStoredActiveLightsDictionary = new Dictionary<string, double>();

        private bool _currentObjectiveHasPiezo => !(_piezoController is null);

        public AFV2CameraFlow(AFCameraInput input, FocusedImageTracker imageTracker = null, PositionTracker zTopPositionTracker = null, PositionTracker piezoPositionTracker = null, ImageOperators imageOperatorsLib = null) : base(input, "AFV2CameraFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            FdcProvider = ClassLocator.Default.GetInstance<AFCameraFlowFDCProvider>();

            _axes = _hardwareManager.Axes;
            var zTopAxisConfig = _axes.AxesConfiguration.AxisConfigs.First(axisConfig => axisConfig.MovingDirection == MovingDirection.ZTop);
            if (zTopAxisConfig is MotorizedAxisConfig motorizedAxisConfig)
            {
                _zTopAxisConfig = motorizedAxisConfig;
            }
            else
            {
                throw new Exception($"AxisConfig for ZTop Axe should be a MotorizedAxisConfig.");
            }

            _camera = _hardwareManager.Cameras[Input.CameraId];
            _rangeType = Input.RangeType;

            bool saveImages = Configuration.IsAnyReportEnabled();
            _imageOperatorsLib = imageOperatorsLib ?? new ImageOperators();
            _focusedImageTracker = imageTracker ?? new FocusedImageTracker(_camera, saveImages, null, _imageOperatorsLib);
            _zTopPositionTracker = zTopPositionTracker ?? new PositionTracker(GetZTopPosition, Configuration.PositionTrackingPeriod_ms);
            _piezoPositionTracker = piezoPositionTracker ?? new PositionTracker(GetPiezoPosition, Configuration.PositionTrackingPeriod_ms);
        }

        protected override void Process()
        {
            Stopwatch stopwatchAFCamera = new Stopwatch();
            stopwatchAFCamera.Start();

            try
            {
                InitialSetup();

                //TODO : improvmnt idea, avoid xtra movement in case of not small scan by passing a double? fo Zfocus and only mov at final range
                ExecutAutofocus();

                // If range type was not small, run autofocus one more time
                // with a small range type to refine focus position
                if (Input.RangeType != ScanRangeType.Small)
                {
                    _rangeType = ScanRangeType.Small;
                    ExecutAutofocus();
                }
                Result.QualityScore = ComputeFocusQualityAtCurrentPosition();
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} An error occured: {ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
                throw;
            }
            finally
            {
                //Restore main Light intensity
                var currentLights = _hardwareManager.Lights;
                foreach (var item in _intensityStoredActiveLightsDictionary)
                {
                    currentLights[item.Key].SetIntensity(item.Value);
                }
                // Restore image resolution and AOI
                _camera.SetImageResolution(_initialCameraSettings.ImageResolution);
                _camera.SetAOI(Rect.Empty);

                _camera.SetFrameRate(_initialCameraSettings.FrameRate);
                _camera.SetExposureTimeMs(_initialCameraSettings.ExposureTime_ms);

                if (_currentObjectiveHasPiezo)
                {
                    GoToPiezoPosition(_piezoAxis.AxisConfiguration.PositionHome);
                }

                _focusedImageTracker.StopTracking();
                _zTopPositionTracker.StopTracking();
                _focusedImageTracker.Dispose();
                _zTopPositionTracker.Dispose();
            }

            stopwatchAFCamera.Stop();
            Logger.Information($"{LogHeader} Autofocus was successful at Z position {Result.ZPosition} mm in {stopwatchAFCamera.Elapsed}");
        }

        private void InitialSetup()
        {
            string selelectorId = _camera.Config.ObjectivesSelectorID;
            _currentObjectiveConfig = _hardwareManager.ObjectivesSelectors[selelectorId].GetObjectiveInUse();
            _piezoController = _axes?.AxesControllers?.Find(controller => controller.AxesList.Exists(axis => axis.AxisID == _currentObjectiveConfig.PiezoAxisID));
            _piezoAxis = _piezoController?.AxesList.First(axis => axis.AxisID == _currentObjectiveConfig.PiezoAxisID);

            SaveAndPrepareHardwareSettings();
        }

        private void SaveAndPrepareHardwareSettings()
        {
            _intensityStoredActiveLightsDictionary = _hardwareManager.Lights.ToDictionary(x => x.Key, x => x.Value.GetIntensity());
            _initialCameraSettings = new CameraSettings()
            {
                ImageResolution = _camera.ImageResolution,
                FrameRate = _camera.GetFrameRate(),
                ExposureTime_ms = _camera.GetExposureTimeMs(),
            };

            var aoi = Input.Aoi;
            if (aoi == new Rect(0, 0, 0, 0))
            {
                int nbLines = Configuration.CameraNbLinesAOI;
                int topLeftY = (_camera.Height / 2) - (nbLines / 2);
                aoi = new Rect(0, topLeftY, _camera.Width, nbLines);
            }
            _camera.SetAOI(aoi);

            foreach (var controller in _axes?.AxesControllers)
            {
                controller.ResetTimestamp();
            }

            if (_currentObjectiveHasPiezo)
            {
                GoToPiezoPosition(_piezoAxis.AxisConfiguration.PositionHome);
            }
        }

        private void ExecutAutofocus()
        {
            Length zFocusPositionmm;
            //TODO : improvmnt idea, avoid xtra movement in case of not small scan by passing a double? fo Zfocus and only mov at final range
            _scanRange = GetScanRange();

            if (_currentObjectiveHasPiezo && _rangeType == ScanRangeType.Small)
            {
                var zTopAxisFocus = FindFocusWithAxis();
                WriteReportIfNeeded(zTopAxisFocus, _zTopPositionTracker, _zTopAxisConfig);

                GoToZTopPosition(zTopAxisFocus);

                var piezoFocus = FindFocusWithPiezo();
                WriteReportIfNeeded(piezoFocus, _piezoPositionTracker, _piezoAxis.AxisConfiguration);

                zFocusPositionmm = ComputeZFocusPosition(zTopAxisFocus, piezoFocus);
            }
            else
            {
                var zTopAxisFocus = FindFocusWithAxis();
                WriteReportIfNeeded(zTopAxisFocus, _zTopPositionTracker, _zTopAxisConfig);

                zFocusPositionmm = zTopAxisFocus;
            }

            //TODO : improvmnt idea, avoid xtra movement in case of not small scan by passing a double? fo Zfocus and only mov at final range
            GoToZTopPosition(zFocusPositionmm, _zTopAxisConfig.SpeedNormal);
            Result.ZPosition = zFocusPositionmm.Millimeters;
        }

        private Length ComputeZFocusPosition(Length zTopAxisFocus, Length piezoFocus)
        {
            var zTopPiezoCorrection = piezoFocus - _piezoAxis.AxisConfiguration.PositionHome;
            var zTopAxisFocusWithCorrection = zTopAxisFocus + (zTopAxisFocus.Millimeters > 0 ? zTopPiezoCorrection : -zTopPiezoCorrection);
            return zTopAxisFocusWithCorrection;
        }

        /// <summary>
        /// Phase 1: approximate ZTop focus position with the current camera settings (framerate and exposure time)
        ///
        /// Initial assumption(s):
        /// -- a. The piezo is at home position (usually midle range position 50 µm)
        ///
        /// -- step 1. Calculate the axis motion speed from the current camera framerate value (in fps) and the scanRange.Step
        /// --         (distance in mm between two consecutive images). Retrieve the initial and target positions.
        /// -- step 2. Place the camera at its initial position (scanRange.ZMin).
        /// -- step 3. Move (at speed value found in step 1) to the target position (scanRange.ZMax) while tracking images
        /// --         and ZTop positions during the camera motion. Every image's focus value is computed in a separated
        /// --         thread as soon as it is receive by the camera message handler.
        /// -- step 4. Deduce the ZTop focus position from the image with the maximum focus value (linear interpollation from
        /// --         ZTop positions tracked during step 3). = ZTopFocusPosition
        /// </summary>
        private Length FindFocusWithAxis()
        {
            // STEP 1
            CheckCancellation();
            _zTopSpeed_mmps = GetZTopSpeed();
            SetupCamera(_zTopSpeed_mmps);
            SetupLight();
            double acceleration = 500;
            var initialPosition = _scanRange.Min.Millimeters();
            var targetPosition = _scanRange.Max.Millimeters();
            var scanRange = Math.Abs(targetPosition.Millimeters - initialPosition.Millimeters);
            double expectedScanDuration_ms = TimeSpan.FromSeconds(scanRange / _zTopSpeed_mmps).TotalMilliseconds;
            int scanTimeout = (int)expectedScanDuration_ms + MaxAxesReactionTime_ms;

            // STEP 2
            CheckCancellation();
            GoToZTopPosition(initialPosition);

            // STEP 3 Start tracking images and ZTop positions and move to the target position
            CheckCancellation();
            var stopWatchScan = new Stopwatch();
            stopWatchScan.Start();
            GoToZTopPositionAndTrackImages(targetPosition, _zTopSpeed_mmps, acceleration, scanTimeout);
            stopWatchScan.Stop();

            Logger.Debug($"{LogHeader} Scan duration (expected | real) = {expectedScanDuration_ms} ms | {stopWatchScan.ElapsedMilliseconds} ms.");
            Logger.Debug($"{LogHeader} Scan speed (expected | real) = {_zTopSpeed_mmps} mm/s | {scanRange / (stopWatchScan.ElapsedMilliseconds / 1000)} mm/s.");

            // Wait until all images have an associated focus value
            CheckCancellation();
            _focusedImageTracker.WaitFocusValuesComputation(logDuration: true);

            int expectedImagesCount = (int)Math.Floor(scanRange / _scanRange.Step) + 1;
            int realImagesCount = _focusedImageTracker.TrackedImagesCount;
            Logger.Debug($"{LogHeader} Images count (expected | real) = {expectedImagesCount} | {realImagesCount}.");

            // STEP 4
            CheckCancellation();
            var imageWithMaxFocusValue = _focusedImageTracker.GetImageWithMaxFocusValue();
            var focusImage = imageWithMaxFocusValue.Key;

            long maxFocusValueTime = focusImage.Timestamp.ToUniversalTime().Ticks;
            var focusPosition = _zTopPositionTracker.GetPositionAtTime(maxFocusValueTime);

            return focusPosition;
        }

        private void SetupLight()
        {
            _autoFocusLightFactor = _initialCameraSettings.ExposureTime_ms / _camera.GetExposureTimeMs();
            var currentLights = _hardwareManager.Lights;
            foreach (var item in _intensityStoredActiveLightsDictionary)
            {
                var lightIntensityForAutofocus = Math.Round(Math.Min(100, _autoFocusLightFactor * item.Value));
                HardwareUtils.SetLightIntensity(_hardwareManager, item.Key,lightIntensityForAutofocus );
            }
        }

        /// <summary>
        /// Phase 2: refine the exact ZTop focus position using a piezo motion
        ///
        /// Initial assumption(s):
        /// -- a. The camera is at ZTop focus position found in phase 1, step 4
        /// -- b. The piezo is at midle range position (50 µm) of its nominal range [0 µm, 100 µm]
        ///
        /// -- step 1. Calculate the maximum piezo motion speed from the current camera exposure time value (in ms) and the
        /// --         scanRange.Step so that the distance travelled during the exposure time is equals to 30 nm. For more
        /// --         information on this spec, see https://unitysc.sharepoint.com/:w:/s/Athos409/EdshYZYoHFFOs3OXONAtFk0B-miWXd8TnPKgra99u1e_XQ?e=xAhjRK
        /// -- step 2. Place the camera at its initial position (piezo position = 0 µm).
        /// -- step 3. Move (at speed value found in step 1) to the target position (piezo position = 100 µm) while
        /// --         tracking images and ZTop positions during the camera motion. Every image's focus value is
        /// --         computed in a separated thread as soon as it is receive by the camera message handler.
        /// -- step 4. Deduce the ZTop focus position from the image with the maximum focus value (linear interpollation from
        /// --         ZTop positions tracked during step 3). = PiezoFocusPosition
        /// </summary>
        private Length FindFocusWithPiezo()
        {
            //STEP 1
            CheckCancellation();
            double speed_mmps = GetPiezoSpeed();
            var initialPosition = _piezoAxis.AxisConfiguration.PositionMin;
            var targetPosition = _piezoAxis.AxisConfiguration.PositionMax;
            var scanRange = Math.Abs(targetPosition.Millimeters - initialPosition.Millimeters);
            double expectedScanDuration_ms = TimeSpan.FromSeconds(scanRange / speed_mmps).TotalMilliseconds;
            int scanTimeout = (int)expectedScanDuration_ms + MaxAxesReactionTime_ms;

            // STEP 2
            CheckCancellation();
            GoToPiezoPosition(initialPosition);

            // STEP 3 Start tracking images and piezo positions and move to the target position
            CheckCancellation();
            var stopWatchScan = new Stopwatch();
            stopWatchScan.Start();
            GoToPiezoPositionAndTrackImages(targetPosition, speed_mmps, scanTimeout);
            stopWatchScan.Stop();

            Logger.Debug($"{LogHeader} Scan duration (expected | real) = {expectedScanDuration_ms} ms | {stopWatchScan.ElapsedMilliseconds} ms.");
            Logger.Debug($"{LogHeader} Scan speed (expected | real) = {speed_mmps} mm/s | {scanRange / (stopWatchScan.ElapsedMilliseconds / 1000)} mm/s.");

            // Wait until all images have an associated focus value
            CheckCancellation();
            _focusedImageTracker.WaitFocusValuesComputation(logDuration: true);

            int cameraFramerate = (int)_camera.GetFrameRate();
            int expectedImagesCount = (cameraFramerate * (int)(expectedScanDuration_ms / 1000)) + 1;
            int realImagesCount = _focusedImageTracker.TrackedImagesCount;
            Logger.Debug($"{LogHeader} Images count (expected | real) = {expectedImagesCount} | {realImagesCount}.");

            // STEP 4
            CheckCancellation();
            var imageWithMaxFocusValue = _focusedImageTracker.GetImageWithMaxFocusValue();
            var focusImage = imageWithMaxFocusValue.Key;

            long maxFocusValueTime = focusImage.Timestamp.ToUniversalTime().Ticks;
            var focusPosition = _piezoPositionTracker.GetPositionAtTime(maxFocusValueTime);
            return focusPosition;
        }

        private void WriteReportIfNeeded(Length focusPosition, PositionTracker positionTracker, AxisConfig axis)
        {
            try
            {
                if (Configuration.IsAnyReportEnabled())
                {
                    var focusValuesAtPosition = new List<Tuple<double, double>>();
                    var imageWithMaxFocusValue = _focusedImageTracker.GetImageWithMaxFocusValue();
                    var focusImage = imageWithMaxFocusValue.Key;

                    foreach (var imgWithFocusValue in _focusedImageTracker.ImagesWithFocusValues)
                    {
                        var img = imgWithFocusValue.Key;
                        var focusValue = imgWithFocusValue.Value;
                        var acquisitionTimestamp = img.Timestamp.ToUniversalTime().Ticks;
                        var acquisitionPosition = positionTracker.GetPositionAtTime(acquisitionTimestamp);
                        focusValuesAtPosition.Add(new Tuple<double, double>(acquisitionPosition.Millimeters, focusValue));
                    }
                    var orderedFocusValueAtPosition = focusValuesAtPosition.OrderBy(x => x.Item1).ToList();
                    var folderName = Path.Combine(ReportFolder, _rangeType.ToString());
                    if (axis is PiezoAxisConfig)
                    {
                        folderName = Path.Combine(ReportFolder, "piezo");
                    }
                    Directory.CreateDirectory(folderName);

                    var csvFilname = $"focus_function_of_position_with_{axis.AxisID}_csharp.csv";
                    SignalReport.WriteSignalInCSVFormat("Axe position (millimeters)", "Focus value", orderedFocusValueAtPosition, Path.Combine(folderName, csvFilname));
                    string imageFocusFilename = $"0_autofocusCameraV2_image{axis.AxisID}_at_focus_position_{focusPosition}_scan_range_type_{_rangeType}_csharp.png";
                    ImageReport.SaveImage(focusImage, Path.Combine(folderName, imageFocusFilename));
                    SaveImagesOnDisk(folderName);
                }
            }
            catch (Exception)
            {
                Logger.Debug($"Failed to generate the report for the AFV2CameraFlow");
            }
        }

        private void SetupCamera(double zTopSpeed)
        {
            // Set camera framerate, it will set the exposure time at its max value for this framerate
            var cameraFramerate = zTopSpeed / _scanRange.Step;
            _camera.SetFrameRate(cameraFramerate);

            // Set exposure time, by default, use the exposure time setted by SetFramerate
            if (Input.CameraImageExposureTime != 0)
            {
                _camera.SetExposureTimeMs(Input.CameraImageExposureTime);
            }

            Logger.Debug($"{LogHeader} Camera framerate is set to {_camera.GetFrameRate()}");
            Logger.Debug($"{LogHeader} Camera exposure time is set to {_camera.GetExposureTimeMs()}");
        }

        private double GetZTopSpeed()
        {
            double maxSpeed = _zTopAxisConfig.SpeedMaxScan;

            // Check if the tool is in maintenance or run mode. For maintenance mode, Speed is limited
            // and the function CheckServiceSpeed will set maxSpeed to this limited speed.
            var axesController = _axes.AxesControllers.FirstOrDefault(c => c.DeviceID == _zTopAxisConfig.ControllerID);
            var zTopAxis = _axes.Axes.First(_ => _.AxisID == _zTopAxisConfig.AxisID);
            axesController.CheckServiceSpeed(zTopAxis, ref maxSpeed);

            double cameraFramerate = Input.CameraFramerate != 0 ? Input.CameraFramerate : (_camera.MaxFrameRate * Configuration.CameraFramerateLimiter);

            var zTopStep = _scanRange.Step.Millimeters();
            double zTopStepmmps = cameraFramerate * zTopStep.Millimeters;
            if (zTopStepmmps > maxSpeed)
            {
                zTopStepmmps = maxSpeed;
            }
            return zTopStepmmps;
        }

        /// <summary>
        /// This method returns the maximum speed (in mm/s) for which the distance travelled during the current camera
        /// exposure time is less or equals to Configuration.PiezoStep.
        /// </summary>
        private double GetPiezoSpeed()
        {
            double exposureTime_s = _camera.GetExposureTimeMs() / 1000;
            return Configuration.PiezoStep.Millimeters / exposureTime_s;
        }

        /// <summary>
        /// Orders the ZTop Axis to move at the given position. Default speed and acceleration values are retrieve from
        /// the axis configuration (respectively SpeedMaxScan and AccelNormal).
        /// </summary>
        private void GoToZTopPosition(Length targetPosition, double speed = 0, double acceleration = 0, int timeout = DefaultAxesMoveTimeout_ms)
        {
            speed = speed == 0 ? _zTopAxisConfig.SpeedMaxScan : speed;
            acceleration = acceleration == 0 ? _zTopAxisConfig.AccelNormal : acceleration;

            var zTopMove = new AxisMove(targetPosition.Millimeters, speed, acceleration);

            Logger.Debug($"{LogHeader} Move ZTop axis to position {targetPosition.Millimeters} with speed = {speed} mm/s.");
            Logger.Verbose($"{LogHeader} Distance traveled during camera exposure time = {GetDistanceDuringCameraExposureTime(speed)}");

            _axes.GotoPointCustomSpeedAccel(null, null, zTopMove, null);
            _axes.WaitMotionEnd(timeout, false);
            Logger.Debug($"{LogHeader} Movemement Done.");
        }

        /// <summary>
        /// Same function as GoToZTopPosition, but starts images and positions tracking just before ask for move. Default
        /// speed and acceleration values are retrieve from the axis configuration (respectively SpeedMaxScan and AccelNormal).
        /// </summary>
        private void GoToZTopPositionAndTrackImages(Length targetPosition, double speed = 0, double acceleration = 0, int timeout = DefaultAxesMoveTimeout_ms)
        {
            speed = speed == 0 ? _zTopAxisConfig.SpeedMaxScan : speed;
            acceleration = acceleration == 0 ? _zTopAxisConfig.AccelNormal : acceleration;

            var zTopMove = new AxisMove(targetPosition.Millimeters, speed, acceleration);

            Logger.Debug($"{LogHeader} Move ZTop axis to position {targetPosition.Millimeters} with speed = {speed} mm/s and Track images.");
            Logger.Verbose($"{LogHeader} Distance traveled during camera exposure time = {GetDistanceDuringCameraExposureTime(speed)}");

            _zTopPositionTracker.Reset();
            _focusedImageTracker.Reset();

            _zTopPositionTracker.StartTracking();
            _focusedImageTracker.StartTracking();

            try
            {
                _axes.GotoPointCustomSpeedAccel(null, null, zTopMove, null);
                _axes.WaitMotionEnd(timeout, false);
            }
            catch(Exception ex)
            {
                Logger.Verbose($"{LogHeader} ### Timeout waitmotionend not reached in GotoZTrackImg...");
                // TODO : Why GotoPointCustomSpeedAccel sometimes reach position but exceeds the timeout????
                // TODO : Find why ACSController waitMotionEnd sometimes never return before timeout
            }

            _focusedImageTracker.StopTracking();
            _zTopPositionTracker.StopTracking();
            Logger.Debug($"{LogHeader} Tracking Done.");
        }

        /// <summary>
        /// Orders the piezo axis to move at the given position. Default speed is retrieve from the axis configuration (SpeedNormal)
        /// </summary>
        private void GoToPiezoPosition(Length targetPosition, double speed = 0, int timeout = DefaultAxesMoveTimeout_ms)
        {
            speed = speed == 0 ? _piezoAxis.AxisConfiguration.SpeedNormal : speed;

            Logger.Debug($"{LogHeader} Move Piezo axis to position {targetPosition} with speed = {speed} mm/s.");
            Logger.Debug($"{LogHeader} Distance traveled during camera exposure time = {GetDistanceDuringCameraExposureTime(speed)}");

            // FIXME: Hack here. Use _axes.GotoPosition() to move the piezo -> not yet implemented
            _piezoController.SetPosAxisWithSpeedAndAccel(
                new List<double> { targetPosition.Millimeters },
                new List<IAxis> { _piezoAxis },
                new List<double> { speed },
                new List<double>()
            );
            _piezoController.WaitMotionEnd(timeout, false);
        }

        /// <summary>
        /// Same function as GoToPiezoPosition, but starts images and positions tracking just before ask for move.
        /// Default speed is retrieve from the axis configuration (respectively SpeedNormal and AccelNormal).
        /// </summary>
        private void GoToPiezoPositionAndTrackImages(Length targetPosition, double speed = 0, int timeout = DefaultAxesMoveTimeout_ms)
        {
            speed = speed == 0 ? _piezoAxis.AxisConfiguration.SpeedNormal : speed;

            Logger.Debug($"{LogHeader} Move Piezo axis to position {targetPosition} with speed = {speed} mm/s.");
            Logger.Debug($"{LogHeader} Distance traveled during camera exposure time = {GetDistanceDuringCameraExposureTime(speed)}");

            // Reset trackers
            _piezoPositionTracker.Reset();
            _focusedImageTracker.Reset();

            _piezoPositionTracker.StartTracking();
            _focusedImageTracker.StartTracking();

            // FIXME: Hack here. Use _axes.GotoPosition() to move the piezo -> not yet implemented
            _piezoController.SetPosAxisWithSpeedAndAccel(
                new List<double> { targetPosition.Millimeters },
                new List<IAxis> { _piezoAxis },
                new List<double> { speed },
                new List<double>()
            );
            _piezoController.WaitMotionEnd(timeout, false);

            // Stop tracking
            _focusedImageTracker.StopTracking();
            _piezoPositionTracker.StopTracking();
        }

        private Length GetDistanceDuringCameraExposureTime(double speed_mmps)
        {
            double exposureTime_ms = _camera.GetExposureTimeMs();
            return (speed_mmps * exposureTime_ms / 1000).Millimeters();
        }

        private void SaveImagesOnDisk(string path = null)
        {
            _focusedImageTracker.SaveImages(path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "IMAGES_FRANGES"), _zTopPositionTracker);
        }

        private ScanRangeWithStep GetScanRange()
        {
            var objectiveCalibration = ClassLocator.Default.GetInstance<CalibrationManager>().GetObjectiveCalibration(_currentObjectiveConfig.DeviceID);
            double range;
            if (objectiveCalibration?.AutoFocus == null)
            {
                range = (Configuration.AutoFocusScanRange.Millimeters);
            }
            else
            {
                var zMin = objectiveCalibration.AutoFocus.ZFocusPosition.Millimeters - (Configuration.AutoFocusScanRange.Millimeters / 2.0);
                var zMax = objectiveCalibration.AutoFocus.ZFocusPosition.Millimeters + (Configuration.AutoFocusScanRange.Millimeters / 2.0);
                range = zMax - zMin;
            }

            switch (_rangeType)
            {
                case ScanRangeType.Configured:
                    if (Input.ScanRangeConfigured == null)
                    {
                        throw new Exception("If no preconfigured scan range type is used, this scan range must be configured.");
                    }
                    Logger.Debug($"{LogHeader} Use a configured scan range : [{Input.ScanRangeConfigured.Min}, {Input.ScanRangeConfigured.Max}].");
                    return Input.ScanRangeConfigured;

                case ScanRangeType.AllAxisRange:
                    var zAxisConfig = GetCurrentZAxisConfig();
                    Logger.Debug($"{LogHeader} Use a 'Large' preconfigured scan range.");
                    return new ScanRangeWithStep(zAxisConfig.PositionMin.Millimeters, zAxisConfig.PositionMax.Millimeters, Configuration.MaxZStep.Millimeters); // TODO add 0.08 in config

                case ScanRangeType.Small:
                    range *= Configuration.SmallRangeCoeff;
                    Logger.Debug($"{LogHeader} Use a 'Small' preconfigured scan range.");
                    break;

                case ScanRangeType.Medium:
                    range *= Configuration.MediumRangeCoeff;
                    Logger.Debug($"{LogHeader} Use a 'Medium' preconfigured scan range.");
                    break;

                case ScanRangeType.Large:
                    range *= Configuration.LargeRangeCoeff;
                    Logger.Debug($"{LogHeader} Use a 'Large' preconfigured scan range.");
                    break;
            }

            double min, max;
            if (Input.UseCurrentZPosition || objectiveCalibration?.AutoFocus?.ZFocusPosition == null)
            {
                double zScanCenter = ((XYZTopZBottomPosition)_hardwareManager.Axes.GetPos()).ZTop;
                max = zScanCenter + (range / 2.0);
                min = zScanCenter - (range / 2.0);
            }
            else
            {
                var zScanCenter = objectiveCalibration.AutoFocus.ZFocusPosition.Millimeters;

                // Optical reference elevation is set to 0 for bottom probe
                if (!(objectiveCalibration?.OpticalReferenceElevationFromStandardWafer is null))
                    zScanCenter -= objectiveCalibration.OpticalReferenceElevationFromStandardWafer.Millimeters;

                max = zScanCenter + (range / 2.0);
                min = zScanCenter - (range / 2.0);
            }

            // min and max should not exceed axis min and max positions
            min = min < _zTopAxisConfig.PositionMin.Millimeters ? _zTopAxisConfig.PositionMin.Millimeters : min;
            max = max > _zTopAxisConfig.PositionMax.Millimeters ? _zTopAxisConfig.PositionMax.Millimeters : max;

            double stepSize;
            if (_rangeType == ScanRangeType.Small)
            {
                stepSize = (_currentObjectiveConfig.DepthOfField * Configuration.FactorBetweenDepthOfFieldAndStepSize).Millimeters;
                stepSize = Math.Max(stepSize, Configuration.MinZStep.Millimeters);
            }
            else
            {
                stepSize = Configuration.MaxZStep.Millimeters;
            }

            Logger.Debug($"{LogHeader} Use scan range of [{min},{max}] with step of {stepSize}");
            return new ScanRangeWithStep(min, max, stepSize);
        }

        private TimestampedPosition GetZTopPosition()
        {
            var ZTopAxis = _axes.Axes.Find(axis => axis.AxisID == "ZTop");
            var timestampedPosition = _axes.GetAxisPosWithTimestamp(ZTopAxis);
            if (timestampedPosition == null)
            {
                throw new Exception("Error in ZTop position retrieval");
            }

            return timestampedPosition;
        }

        private TimestampedPosition GetPiezoPosition()
        {
            var timestampedPosition = _axes.GetAxisPosWithTimestamp(_piezoAxis);
            if (timestampedPosition == null)
            {
                throw new Exception("Error in Piezo position retrieval");
            }

            return timestampedPosition;
        }

        private AxisConfig GetCurrentZAxisConfig()
        {
            var cameraPosition = _hardwareManager.Cameras[Input.CameraId].Config.ModulePosition;
            var axisConfigs = _hardwareManager.Axes.AxesConfiguration.AxisConfigs;
            AxisConfig zAxisConfig;

            if (cameraPosition == ModulePositions.Down)
            {
                zAxisConfig = axisConfigs.First(axis => axis.AxisID == "ZBottom");
            }
            else
            {
                zAxisConfig = axisConfigs.First(axis => axis.AxisID == "ZTop");
            }
            return zAxisConfig;
        }

        private double ComputeFocusQualityAtCurrentPosition()
        {
            var measureFocusValues = MultipleMeasureFocus(Configuration.MeasureNbToQualityScore);
            var meanFocus = measureFocusValues.Count > 0 ? measureFocusValues.Average() : 0;
            var stdDevFocus = measureFocusValues.Count > 1 ? measureFocusValues.StandardDeviation() : 0;

            if (meanFocus <= 0.1)
            {
                return 0;
            }

            var currentPosition = _axes.GetPos().ToXYZTopZBottomPosition().ZTop.Millimeters();
            var currentObjective = _hardwareManager.GetObjectiveInUseByCamera(Input.CameraId);

            var maxPosition = _zTopPositionTracker.PositionsOverTime.Values.Max();
            var minPosition = _zTopPositionTracker.PositionsOverTime.Values.Min();

            var largeRange = currentObjective.DepthOfField;
            var farPosition1 = (currentPosition + largeRange) < maxPosition ? currentPosition + largeRange : maxPosition;
            var farPosition2 = (currentPosition - largeRange) > minPosition ? currentPosition - largeRange : minPosition;
            var farPositionTime1 = _zTopPositionTracker.PositionsOverTime.OrderBy(x => x.Value).FirstOrDefault(x => x.Value.Millimeters >= farPosition1.Millimeters).Key;
            var farPositionTime2 = _zTopPositionTracker.PositionsOverTime.OrderByDescending(x => x.Value).FirstOrDefault(x => x.Value.Millimeters <= farPosition2.Millimeters).Key;
            var farFocusValue1 = _focusedImageTracker.ImagesWithFocusValues.OrderBy(x => x.Key.Timestamp).FirstOrDefault(image => ((DateTimeOffset)image.Key.Timestamp).ToUnixTimeMilliseconds() >= farPositionTime1).Value;
            var farFocusValue2 = _focusedImageTracker.ImagesWithFocusValues.OrderByDescending(x => x.Key.Timestamp).FirstOrDefault(image => ((DateTimeOffset)image.Key.Timestamp).ToUnixTimeMilliseconds() <= farPositionTime2).Value;

            var preciseRange = currentObjective.DepthOfField / 100;
            var precisePosition1 = (currentPosition + preciseRange) < maxPosition ? currentPosition + preciseRange : maxPosition;
            var precisePosition2 = (currentPosition - preciseRange) > minPosition ? currentPosition - preciseRange : minPosition;
            var precisePositionTime1 = _zTopPositionTracker.PositionsOverTime.OrderBy(x => x.Value).FirstOrDefault(x => x.Value.Millimeters >= precisePosition1.Millimeters).Key;
            var precisePositionTime2 = _zTopPositionTracker.PositionsOverTime.OrderByDescending(x => x.Value).FirstOrDefault(x => x.Value.Millimeters <= precisePosition2.Millimeters).Key;
            var preciseFocusValue1 = _focusedImageTracker.ImagesWithFocusValues.OrderBy(x => x.Key.Timestamp).FirstOrDefault(image => ((DateTimeOffset)image.Key.Timestamp).ToUnixTimeMilliseconds() >= precisePositionTime1).Value;
            var preciseFocusValue2 = _focusedImageTracker.ImagesWithFocusValues.OrderByDescending(x => x.Key.Timestamp).FirstOrDefault(image => ((DateTimeOffset)image.Key.Timestamp).ToUnixTimeMilliseconds() <= precisePositionTime2).Value;

            var minMeasureFocus = meanFocus - stdDevFocus;

            var focusPositionIsGoodRelativeToUpperLimitOfLargeRange = (minMeasureFocus > farFocusValue1) ? 1.0 : 0.0;
            var focusPositionIsGoodRelativeToLowerLimitOfLargeRange = (minMeasureFocus > farFocusValue2) ? 1.0 : 0.0;
            var focusPositionQualityInLargeRange = 0.5 * focusPositionIsGoodRelativeToUpperLimitOfLargeRange + 0.5 * focusPositionIsGoodRelativeToLowerLimitOfLargeRange;

            var focusPositionIsGoodRelativeToUpperLimitOfPreciseRange = (minMeasureFocus > farFocusValue1) ? 1.0 : 0.0;
            var focusPositionIsGoodRelativeToLowerLimitOfPreciseRange = (minMeasureFocus > farFocusValue2) ? 1.0 : 0.0;
            var focusPositionQualityInPreciseRange = 0.5 * focusPositionIsGoodRelativeToUpperLimitOfPreciseRange + 0.5 * focusPositionIsGoodRelativeToLowerLimitOfPreciseRange;

            var focusMeasureQuality = (stdDevFocus) > currentObjective.DepthOfField.Millimeters ? 0 : 1 - stdDevFocus;

            var quality = 0.5 * focusMeasureQuality + 0.4 * focusPositionQualityInLargeRange + 0.1 * focusPositionQualityInPreciseRange;
            return quality;
        }

        private double MeasureFocus()
        {
            try
            {
                var img = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);
                return _imageOperatorsLib.ComputeFocusMeasure(img);
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Autofocus quality score computation for one image failure : {ex.Message}");
                return double.NaN;
            }
        }

        private List<double> MultipleMeasureFocus(int measuresNumber)
        {
            // Handle camera continuous grab here or HardwareUtils.AcquireCameraImage
            // will Start and stop continuous grab for each acquisition
            var isAcquiringFromOutside = _camera.IsAcquiring;
            if (!isAcquiringFromOutside)
            {
                _camera.StartContinuousGrab();
            }
            var focusValues = new List<double>();
            for (int i = 0; i < measuresNumber; i++)
            {
                var focusValue = MeasureFocus();
                if (!double.IsNaN(focusValue))
                {
                    focusValues.Add(focusValue);
                }
            }
            if (!isAcquiringFromOutside)
            {
                _camera.StopContinuousGrab();
            }
            return focusValues;
        }
    }
}
