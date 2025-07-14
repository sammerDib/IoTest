using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Calibration

{
    /// <summary>
    /// How to update the calibration
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Only update the calibration for the current manager, but it won't be kept when the manager is deleted.
        /// </summary>
        Local,

        /// <summary>
        /// Make the calibration persistent by saving it, so that it can be restored when the manager is created.
        /// </summary>
        Persistent
    }

    public class CalibrationManager
    {
        private ILogger _logger;
        private string _calibrationFolderPath;
        private CalibrationManagerFDCProvider _fdcProvider;

        public CalibrationManager(string calibrationFolderPath, bool initWithPersistentCalibrations = true)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<CalibrationManager>>();
            _calibrationFolderPath = calibrationFolderPath;
            if (initWithPersistentCalibrations)
                InitWithPersistentCalibrations();

            _fdcProvider = ClassLocator.Default.GetInstance<CalibrationManagerFDCProvider>();
            _fdcProvider.CreateFDC(_calibrations);
        }

        public IEnumerable<ICalibrationData> Calibrations => _calibrations;

        private List<ICalibrationData> _calibrations = new List<ICalibrationData>();
        private const string CalibrationFolderName = "Calibration";

        private void InitWithPersistentCalibrations()
        {
            foreach (var calibrationType in GetCalibrationTypes())
            {
                ICalibrationData calibration;
                string calibrationFile = Path.Combine(_calibrationFolderPath, calibrationType.Name + ".xml");
                if (File.Exists(calibrationFile))
                {
                    XmlSerializer deserializer = new XmlSerializer(calibrationType);
                    using (TextReader reader = new StreamReader(calibrationFile))
                    {
                        calibration = (ICalibrationData)deserializer.Deserialize(reader);
                    }

                    if (calibration is XYCalibrationData)
                    {
                        _logger.Debug($"XY Calibration file {calibrationFile} is pre-computing...");
                        if (false == XYCalibrationCalcul.PreCompute(calibration as XYCalibrationData))
                            _logger.Error($"Calibration file {calibrationFile} could not be pre-computed");
                        else
                            _logger.Debug($"XY Calibration file {calibrationFile} pre-compute Done");
                    }
                    _calibrations.Add(calibration);
                }
                else
                {
                    _logger.Information($"Calibration file {calibrationFile} is missing");
                }
            }
        }

        public void UpdateCalibration(ICalibrationData calibrationData, UpdateMode updateMode = UpdateMode.Local)
        {
            calibrationData.CreationDate = DateTime.Now;
            if (string.IsNullOrEmpty(calibrationData.User))
                throw new InvalidOperationException("User is missing in calibration result");

            if (updateMode == UpdateMode.Persistent)
                MakeCalibrationPeristent(calibrationData);

            // Update calibrations
            ICalibrationData oldCalibration = _calibrations.FirstOrDefault(calibration => calibration.GetType() == calibrationData.GetType());
            if (oldCalibration != null)
                _calibrations.Remove(oldCalibration);
            _calibrations.Add(calibrationData);

            if (updateMode == UpdateMode.Persistent)
                _fdcProvider.CreateFDC(_calibrations);
        }

        private void MakeCalibrationPeristent(ICalibrationData calibrationData)
        {
            string calibrationFile = Path.Combine(_calibrationFolderPath, calibrationData.GetType().Name + ".xml");

            if (File.Exists(calibrationFile))
                File.Delete(calibrationFile);

            XML.Serialize(calibrationData, calibrationFile);
        }

        private List<Type> GetCalibrationTypes()
        {
            // Get calibration types with XmlInclude defined in ICalibrationData
            XmlIncludeAttribute[] xmlIncludes = (XmlIncludeAttribute[])Attribute.GetCustomAttributes(typeof(ICalibrationData), typeof(XmlIncludeAttribute));
            var types = xmlIncludes.Select(x => x.Type).ToList();
            return types;
        }

        public ObjectiveCalibration GetObjectiveCalibration(string objectiveID)
        {
            return Calibrations.OfType<ObjectivesCalibrationData>().FirstOrDefault()?.Calibrations.FirstOrDefault(x => x.DeviceId == objectiveID);
        }

        public LiseHFObjectiveIntegrationTimeCalibration GetLiseHFObjectiveIntegrationTime(string objectiveID)
        {
            return Calibrations.OfType<LiseHFCalibrationData>().FirstOrDefault()?.IntegrationTimes.FirstOrDefault(x => x.ObjectiveDeviceId == objectiveID);
        }

        public IProbeSpotCalibration GetLiseHFObjectiveSpotPosition(string objectiveID)
        {
            return Calibrations.OfType<LiseHFCalibrationData>().FirstOrDefault()?.SpotPositions.FirstOrDefault(x => x.ObjectiveDeviceId == objectiveID);
        }

        public double GetLiseHFObjectiveSpotPositionExposurTimeMs(string objectiveID)
        {
            var spotLiseHFCalibata = Calibrations.OfType<LiseHFCalibrationData>().FirstOrDefault()?.SpotPositions.FirstOrDefault(x => x.ObjectiveDeviceId == objectiveID);
            return spotLiseHFCalibata?.CamExposureTime_ms ?? 0.0;
        }

        public XYCalibrationData GetXYCalibrationData()
        {
            var xyCalibration = Calibrations.OfType<XYCalibrationData>().FirstOrDefault();
            if (xyCalibration != null && !xyCalibration.IsInterpReady)
            {
                _logger.Debug($"xy Calibration pre-computing ...");
                if (false == XYCalibrationCalcul.PreCompute(xyCalibration))
                    _logger.Error($"xy Calibration could not be pre-computed");
                else
                    _logger.Debug($"xy Calibration pre-compute Done");
            }
            return xyCalibration;
        }

        public TCalibrationData PopCalibrationData<TCalibrationData>()
        {
            TCalibrationData calibration = _calibrations.OfType<TCalibrationData>().FirstOrDefault();
            _calibrations.RemoveAll(c => c is TCalibrationData);
            return calibration;
        }

        public XYZTopZBottomMove GetXYZTopZBottomObjectiveOffset(string previousObjectiveId, string newObjectiveId, bool applyProbeOffset)
        {
            if (previousObjectiveId != newObjectiveId)
            {
                var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
                var previousObjectiveSelector = hardwareManager.GetObjectiveSelectorOfObjective(previousObjectiveId);
                var newObjectiveSelector = hardwareManager.GetObjectiveSelectorOfObjective(newObjectiveId);

                if (previousObjectiveSelector == newObjectiveSelector)
                {
                    var previousCalibration = GetObjectiveCalibration(previousObjectiveId);
                    var previousProbeSpotPositions = GetLiseHFObjectiveSpotPosition(previousObjectiveId);
                    var newCalibration = GetObjectiveCalibration(newObjectiveId);
                    var newProbeSpotPositions = GetLiseHFObjectiveSpotPosition(newObjectiveId);
                    if (!(newCalibration is null) && !(previousCalibration is null))
                    {
                        var objectiveOffset = new XYZTopZBottomMove(0, 0, 0, 0);

                        // Previous offset
                        objectiveOffset.X -= previousCalibration?.Image?.XOffset?.Millimeters ?? 0.0;
                        objectiveOffset.Y -= previousCalibration?.Image?.YOffset?.Millimeters ?? 0.0;

                        if (applyProbeOffset && !(previousProbeSpotPositions is null))
                        {
                            objectiveOffset.X -= previousProbeSpotPositions?.XOffset?.Millimeters ?? 0.0;
                            objectiveOffset.Y -= previousProbeSpotPositions?.YOffset?.Millimeters ?? 0.0;
                        }

                        // New offset
                        objectiveOffset.X += newCalibration?.Image?.XOffset?.Millimeters ?? 0.0;
                        objectiveOffset.Y += newCalibration?.Image?.YOffset?.Millimeters ?? 0.0;

                        if (applyProbeOffset && !(newProbeSpotPositions is null))
                        {
                            objectiveOffset.X += newProbeSpotPositions?.XOffset?.Millimeters ?? 0.0;
                            objectiveOffset.Y += newProbeSpotPositions?.YOffset?.Millimeters ?? 0.0;
                        }

                        Length previousZOffset = previousCalibration.ZOffsetWithMainObjective;
                        if (newObjectiveSelector.Position == ModulePositions.Up)
                        {
                            objectiveOffset.ZTop = (previousZOffset - newCalibration.ZOffsetWithMainObjective).Millimeters;
                        }
                        else if (newObjectiveSelector.Position == ModulePositions.Down)
                        {
                            objectiveOffset.ZBottom = (previousZOffset - newCalibration.ZOffsetWithMainObjective).Millimeters;
                        }

                        return objectiveOffset;
                    }
                }
            }
            return new XYZTopZBottomMove(0.0, 0.0, 0.0, 0.0);
        }

        public void WriteLiseHFCalibCSV(string filename, string content)
        {
            try
            {
                string BaseCSV = Path.Combine(_calibrationFolderPath, "LiseHF_CalibIT");
                if (!Directory.Exists(BaseCSV))
                {
                    Directory.CreateDirectory(BaseCSV);
                }
                string filePath = Path.Combine(BaseCSV, filename + ".csv");
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unable to save LiseHF CSV file - {ex.Message}");
            }
        }
    }
}
