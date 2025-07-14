using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public static class HardwareUtils
    {
        private const int DefaultAxesMoveTimeoutMs = 20000;
        private static object s_cameraAcquisitionLock = new object();

        public static ServiceImage AcquireCameraImage(AnaHardwareManager hardwareManager, ICameraManager cameraManager, string cameraId)
        {
            var camera = GetCameraFromId(hardwareManager, cameraId);

            var image = AcquireCameraImage(camera, cameraManager);
            return image.ToServiceImage();
        }

        public static ICameraImage AcquireCameraImage(CameraBase camera, ICameraManager cameraManager)
        {
            lock (s_cameraAcquisitionLock)
            {
                var isAcquiringFromOutside = camera.IsAcquiring;
                if (!isAcquiringFromOutside)
                {
                    camera.StartContinuousGrab();
                }
                try
                {
                    return cameraManager.GetNextCameraImage(camera);
                }
                catch (Exception ex)
                {
                    var logger = ClassLocator.Default.GetInstance<ILogger>();
                    logger.Error(ex, $"Failed to acquire camera image from camera {camera.Name}");
                    throw;
                }
                finally
                {
                    if (!isAcquiringFromOutside)
                    {
                        camera.StopContinuousGrab();
                    }
                }
            }
        }

        public static ServiceImageWithPosition AcquireCameraImageAtXYPosition(AnaHardwareManager hardwareManager, ICameraManager cameraManager, string cameraId, XYPosition position)
        {
            var camera = GetCameraFromId(hardwareManager, cameraId);

            return AcquireCameraImageAtXYPosition(hardwareManager, cameraManager, camera, position);
        }

        public static ServiceImageWithPosition AcquireCameraImageAtXYPosition(AnaHardwareManager hardwareManager, ICameraManager cameraManager, CameraBase camera, XYPosition position)
        {
            var axesPosition = GetAxesPosition(hardwareManager.Axes);
            var moveDestination = new XYZTopZBottomPosition(
                position.Referential,
                position.X,
                position.Y,
                axesPosition.ZTop,
                axesPosition.ZBottom);

            ServiceImageWithPosition imageWithPosition;
            lock (s_cameraAcquisitionLock)
            {
                MoveAxesTo(hardwareManager.Axes, moveDestination);

                imageWithPosition = new ServiceImageWithPosition(AcquireCameraImage(camera, cameraManager).ToServiceImage(), position);
            }
            return imageWithPosition;
        }

        /// <summary>
        /// Acquire nbColumns * nbRows images around a XYPosition.
        /// </summary>
        /// <param name="hardwareManager">Hardware manager</param>
        /// <param name="cameraManager">Camera manager to acquire image</param>
        /// <param name="centroidPosition">Center XY positon around which we will acquire images</param>
        /// <param name="nbColumns">Number of images to acquire horizontally</param>
        /// <param name="nbRows">Number of images to acquire vertically</param>
        /// <param name="overlapRate">Overlap rate between images. Default = 0.1</param>
        /// <returns>List of acquired images</returns>
        public static List<ServiceImageWithPosition> AcquireNCameraImagesAroundXYPosition(AnaHardwareManager hardwareManager, ICameraManager cameraManager, string cameraId, XYPosition centroidPosition, int nbColumns = 2, int nbRows = 2, double overlapRate = 0.1)
        {
            var camera = GetCameraFromId(hardwareManager, cameraId);

            return AcquireNCameraImagesAroundXYPosition(hardwareManager, cameraManager, camera, centroidPosition, nbColumns, nbRows, overlapRate);
        }

        /// <summary>
        /// Acquire nbColumns * nbRows images around given XYPosition.
        /// </summary>
        /// <param name="hardwareManager">Hardware manager</param>
        /// <param name="cameraManager">Camera manager to acquire image</param>
        /// <param name="centroidPosition">Center XY positon around which we will acquire images</param>
        /// <param name="nbColumns">Number of images to acquire horizontally</param>
        /// <param name="nbRows">Number of images to acquire vertically</param>
        /// <param name="overlapRate">Overlap rate between images. Default = 0.1</param>
        /// <returns>List of acquired images</returns>
        public static List<ServiceImageWithPosition> AcquireNCameraImagesAroundXYPosition(AnaHardwareManager hardwareManager, ICameraManager cameraManager, CameraBase camera, XYPosition centroidPosition, int nbColumns = 2, int nbRows = 2, double overlapRate = 0.1)
        {
            var imagesWithPositions = new List<ServiceImageWithPosition>();

            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var objectiveCalibration = GetObjectiveParametersUsedByCamera(hardwareManager, calibrationManager, camera.DeviceID);

            double cameraWidthInMillimiters = camera.Width * objectiveCalibration.Image.PixelSizeX.Millimeters;
            double cameraHeightInMillimiters = camera.Height * objectiveCalibration.Image.PixelSizeY.Millimeters;

            bool isEvenCol = nbColumns % 2 == 0;
            bool isEvenRow = nbRows % 2 == 0;

            if (nbColumns == 0) nbColumns = 1;
            if (nbRows == 0) nbRows = 1;

            if (nbColumns < 0) nbColumns = -nbColumns;
            if (nbRows < 0) nbRows = -nbRows;

            double startPosX = centroidPosition.X - ((0.5 - (overlapRate / 2)) * cameraWidthInMillimiters * Convert.ToDouble(isEvenCol)) - ((1 - overlapRate) * cameraWidthInMillimiters * (Math.Ceiling((nbColumns / 2.0)) - 1));
            double startPosY = centroidPosition.Y - ((0.5 - (overlapRate / 2)) * cameraHeightInMillimiters * Convert.ToDouble(isEvenRow)) - ((1 - overlapRate) * cameraHeightInMillimiters * (Math.Ceiling((nbRows / 2.0)) - 1));

            var isAcquiringFromOutside = camera.IsAcquiring;
            if (!isAcquiringFromOutside)
            {
                camera.StartContinuousGrab();
            }
            try
            {
                for (int i = 0; i < nbColumns; i++)
                {
                    double posX = startPosX + i * (1 - overlapRate) * cameraWidthInMillimiters;
                    for (int j = 0; j < nbRows; j++)
                    {
                        double posY = startPosY + j * (1 - overlapRate) * cameraHeightInMillimiters;

                        var imageCentroidPosition = new XYPosition(
                        centroidPosition.Referential,
                        posX,
                        posY
                        );
                        imagesWithPositions.Add(AcquireCameraImageAtXYPosition(hardwareManager, cameraManager, camera, imageCentroidPosition));
                    }
                }
            }
            finally
            {
                if (!isAcquiringFromOutside)
                {
                    camera.StopContinuousGrab();
                }
            }

            return imagesWithPositions;
        }

        public static Length GetCameraFieldOfViewWidth(AnaHardwareManager hardwareManager, string cameraId)
        {
            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var objectiveCalibration = GetObjectiveParametersUsedByCamera(hardwareManager, calibrationManager, cameraId);
            var camera = GetCameraFromId(hardwareManager, cameraId);

            return (camera.Width * objectiveCalibration.Image.PixelSizeX.Millimeters).Millimeters();
        }

        public static Length GetCameraFieldOfViewHeight(AnaHardwareManager hardwareManager, string cameraId)
        {
            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var camera = GetCameraFromId(hardwareManager, cameraId);
            var objectiveCalibration = GetObjectiveParametersUsedByCamera(hardwareManager, calibrationManager, camera.DeviceID);

            return (camera.Height * objectiveCalibration.Image.PixelSizeY.Millimeters).Millimeters();
        }

        public static CameraBase GetCameraFromId(AnaHardwareManager hardwareManager, string cameraId)
        {
            CameraBase camera;
            if (!hardwareManager.Cameras.TryGetValue(cameraId, out camera))
            {
                throw new InvalidOperationException($"Provided camera ID ({cameraId}) cannot be found in hardware manager.");
            }
            return camera;
        }

        public static void MoveAxesTo(IAxes axes, PositionBase position, AxisSpeed speed = AxisSpeed.Normal)
        {
            lock (s_cameraAcquisitionLock)
            {
                axes.GotoPosition(position, speed);
                axes.WaitMotionEnd(DefaultAxesMoveTimeoutMs);
            }
        }

        public static void MoveAxesToOpticalReference(AnaHardwareManager hardwareManager, OpticalReferenceDefinition opticalRef, AxisSpeed speed = AxisSpeed.Normal)
        {
            lock (s_cameraAcquisitionLock)
            {
                var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
                var opticalRefObjectiveConfig = hardwareManager.GetObjectiveConfigs().Find(_ => _.DeviceID == opticalRef.PositionObjectiveID);
                var opticalRefObjectiveCalibration = calibrationManager.GetObjectiveCalibration(opticalRefObjectiveConfig.DeviceID);
                var opticalRefOjbectiveSelector = hardwareManager.ObjectivesSelectors.First(_ => _.Value.Config.Objectives.Contains(opticalRefObjectiveConfig)).Value;

                var currentObjectiveConfig = opticalRefOjbectiveSelector.GetObjectiveInUse();
                var objectiveCalibration = calibrationManager.GetObjectiveCalibration(currentObjectiveConfig.DeviceID);

                // Optical reference are stored in StageReferential
                var position = new XYZTopZBottomPosition(new StageReferential(), opticalRef.PositionX.Millimeters, opticalRef.PositionY.Millimeters, opticalRef.PositionZ.Millimeters, opticalRef.PositionZLower.Millimeters);

                if (currentObjectiveConfig.DeviceID != opticalRefObjectiveConfig.DeviceID)
                {
                    if (opticalRefOjbectiveSelector.Position == ModulePositions.Up)
                    {
                        Length previousZOffset = opticalRefObjectiveCalibration.ZOffsetWithMainObjective;
                        position.ZTop += (previousZOffset - objectiveCalibration.ZOffsetWithMainObjective).Millimeters;
                    }
                    else if (opticalRefOjbectiveSelector.Position == ModulePositions.Down)
                    {
                        Length previousZOffset = opticalRefObjectiveCalibration.ZOffsetWithMainObjective;
                        position.ZBottom += (previousZOffset - objectiveCalibration.ZOffsetWithMainObjective).Millimeters;
                    }
                }

                hardwareManager.Axes.GotoPosition(position, speed);
                hardwareManager.Axes.WaitMotionEnd(DefaultAxesMoveTimeoutMs);
            }
        }        

        public static void MoveIncremental(IAxes axes, XYZTopZBottomMove position, AxisSpeed speed = AxisSpeed.Normal)
        {
            lock (s_cameraAcquisitionLock)
            {
                axes.MoveIncremental(position, speed);
                axes.WaitMotionEnd(DefaultAxesMoveTimeoutMs);
            }
        }

        public static XYZTopZBottomPosition GetAxesPosition(IAxes axes)
        {
            return (XYZTopZBottomPosition)axes.GetPos();
        }

        public static bool SetNewObjective(string objectiveSelectorID, string newObjectiveToUseID, bool applyObjectiveOffset, ILogger logger, IReferentialManager referentialManager = null, AnaHardwareManager hardwareManager = null, CalibrationManager calibrationManager = null)
        {
            if (hardwareManager == null)
                hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            if (calibrationManager == null)
                calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            if (referentialManager == null)
                referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            var objectivesSelector = hardwareManager.ObjectivesSelectors[objectiveSelectorID];

            ObjectiveConfig newObjectiveToUse = null;
            foreach (var objective in objectivesSelector.Config.Objectives)
            {
                if (objective.DeviceID == newObjectiveToUseID)
                {
                    newObjectiveToUse = objective;
                    break;
                }
            }

            if (newObjectiveToUse == null)
            {
                string message = $"[All probes]ObjectiveSelectorSetPos - Objective corresponding objective to objective ID {newObjectiveToUseID} not found";
                logger?.Error(message);
                throw (new Exception(message));
            }

            if (applyObjectiveOffset)
            {
                var previousObjective = objectivesSelector.GetObjectiveInUse();

                var referentialSettings = referentialManager.GetSettings(ReferentialTag.Stage) as StageReferentialSettings;

                var applyProbeOffset = referentialSettings == null ? false : referentialSettings.EnableProbeSpotOffset;

                var newIncrementalPos = calibrationManager.GetXYZTopZBottomObjectiveOffset(previousObjective.DeviceID, newObjectiveToUseID, applyProbeOffset);
                hardwareManager.Axes.MoveIncremental(newIncrementalPos, AxisSpeed.Normal);
            }
            objectivesSelector.SetObjective(newObjectiveToUse);

            return true;
        }

        public static bool SetLightIntensity(AnaHardwareManager hardwareManager, string lightId, double intensity, int timeoutMs = 2000)
        {
            LightBase light;
            if (!hardwareManager.Lights.TryGetValue(lightId, out light))
            {
                throw new Exception($"Provided light ID ({lightId}) cannot be found in hardware manager.");
            }

            var currentIntensity = light.GetIntensity();
            light.SetIntensity(intensity);

            //TODO Add something like "WaitMotionEnd" for light
            int elapsed = 0;
            int loopSleep_ms = 10;             // todo (later) : wait sleep loop ms in config
            double intensityTolerance = 0.05;  // todo (later) : tolerance in config 
            while (Math.Abs(currentIntensity - intensity) > intensityTolerance && (elapsed < timeoutMs))
            {
                currentIntensity = light.GetIntensity();

                Thread.Sleep(loopSleep_ms);
                elapsed += timeoutMs / loopSleep_ms;
            }

            if ((elapsed >= timeoutMs) && Math.Abs(currentIntensity - intensity) > intensityTolerance)
                return false; 

            return true;
        }

        public static bool TurnOffAllLights(AnaHardwareManager hardwareManager, int timeoutMs = 2000)
        {
            var currentLights = hardwareManager.Lights;
            foreach (var light in currentLights)
            {
                var setLightResult=SetLightIntensity(hardwareManager, light.Key, 0.0, timeoutMs);
                if (!setLightResult)
                    return false;
            }

            return true;
        }

        public static T GetProbeFromID<T>(AnaHardwareManager hardwareManager, string probeID)
            where T : IProbe
        {
            T probe = default;
            if (hardwareManager.Probes.ContainsKey(probeID))
                probe = (T)(hardwareManager.Probes[probeID]);

            if (probe == null)
                throw new Exception($"could not found Probe ({probeID}) as {typeof(T)}.");

            return probe;
        }

        public static void ResetProbeCalibrationManager(AnaHardwareManager hardwareManager, string probeID)
        {
            IProbe probe = null;
            if (hardwareManager.Probes.ContainsKey(probeID))
                probe = hardwareManager.Probes[probeID];

            if (probe != null)
            {
                probe.CalibrationManager?.ResetCalibrations();
            }

        }

        public static IProbeLise GetProbeLiseFromID(AnaHardwareManager hardwareManager, string probeID)
        {
            IProbeLise probe = null;
            if (hardwareManager.Probes.ContainsKey(probeID))
                probe = hardwareManager.Probes[probeID] as IProbeLise;

            return probe;
        }

        public static ProbeLiseConfig GetProbeLiseConfigFromID(AnaHardwareManager hardwareManager, string probeID)
        {
            var probeLise = GetProbeLiseFromID(hardwareManager, probeID);
            var probeLiseConfig = probeLise.Configuration as ProbeLiseConfig;

            return probeLiseConfig;
        }

        public static ObjectiveCalibration GetObjectiveParametersUsedByCamera(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager, string cameraId)
        {
            var currentObjective = hardwareManager.GetObjectiveInUseByCamera(cameraId);
            var objectiveCalibration = GetObjectiveParameters(calibrationManager, currentObjective.DeviceID);
            return objectiveCalibration;
        }

        public static ObjectiveCalibration GetObjectiveParametersUsedByProbe(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager, string probeId)
        {
            var currentObjective = hardwareManager.GetObjectiveInUseByProbe(probeId);
            var objectiveCalibration = calibrationManager.GetObjectiveCalibration(currentObjective.DeviceID);
            return objectiveCalibration;
        }

        public static ObjectiveCalibration GetObjectiveParameters(CalibrationManager calibrationManager, string objectiveId)
        {
            var objectiveCalibration = calibrationManager.GetObjectiveCalibration(objectiveId);

            if (objectiveCalibration?.Image is null)
            {
                throw new Exception($"Calibration is missing for {objectiveId}. ");
            }

            if (objectiveCalibration.Image.PixelSizeX is null)
            {
                throw new Exception($"The pixel size X is missing for objective '{objectiveId}'.");
            }

            if (objectiveCalibration.Image.PixelSizeY is null)
            {
                throw new Exception($"The pixel size Y is missing for objective '{objectiveId}'.");
            }

            if (objectiveCalibration.Image.PixelSizeX.Micrometers != objectiveCalibration.Image.PixelSizeY.Micrometers)
            {
                throw new Exception($"The X and Y pixel sizes must be the same for the objective '{objectiveId}'.");
            }

            return objectiveCalibration;
        }

        public static AutofocusParameters GetAutofocusParameters(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager, string probeId)
        {
            var currentObjective = hardwareManager.GetObjectiveInUseByProbe(probeId);

            var objectiveCalibration = calibrationManager.GetObjectiveCalibration(currentObjective.DeviceID);
            if (objectiveCalibration?.AutoFocus is null)
            {
                throw new Exception($"Autofocus parameters are missing for device {probeId}.");
            }

            return objectiveCalibration?.AutoFocus;
        }

        public static LiseAutofocusParameters GetAutofocusLiseParameters(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager, string probeId)
        {
            var currentObjective = hardwareManager.GetObjectiveInUseByProbe(probeId);

            var objectiveCalibration = calibrationManager.GetObjectiveCalibration(currentObjective.DeviceID);
            if (objectiveCalibration?.AutoFocus?.Lise is null)
            {
                throw new Exception($"Lise autofocus parameters are missing for device {probeId}.");
            }

            return objectiveCalibration?.AutoFocus?.Lise;
        }

        public static bool ShouldEnsureUnclampedAirGapMeasure(AnaHardwareManager hardwareManager)
        {
            // On non-open chucks, the clamp may interfere with the air gap measures, so we should
            // ensure the wafer is unclamped when doing it.
            bool IsThereAWaferClamped = hardwareManager.Chuck.GetState().WaferClampStates.Any(w => w.Value == true);
            return !hardwareManager.Chuck.Configuration.IsOpenChuck && IsThereAWaferClamped;
        }

        public static bool ManageWaferUnclampingBeforeAirGapMeasure(AnaHardwareManager hardwareManager, Length waferDiameter, XYZTopZBottomPosition position, int releaseWaferTimeoutMilliseconds)
        {
            bool shouldEnsureUnclampedAirGapMeasure = HardwareUtils.ShouldEnsureUnclampedAirGapMeasure(hardwareManager);
            if (shouldEnsureUnclampedAirGapMeasure)
            {
                // Manually go to the position before releasing the wafer.
                // Here, we can't use context to go to position beacause we want to release wafer BEFORE execute the flow (which apply context).
                if (position != null)
                {
                    hardwareManager.Axes.GotoPosition(position, AxisSpeed.Normal);
                    hardwareManager.Axes.WaitMotionEnd(20000);
                }
                hardwareManager.ClampHandler.ReleaseWafer(waferDiameter);
                // Wait for the wafer to go back to its unconstrained shape.
                Thread.Sleep(releaseWaferTimeoutMilliseconds);
            }

            return shouldEnsureUnclampedAirGapMeasure;
        }
    }
}
