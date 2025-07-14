using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Calibration
{
    public class CalibrationManager : ICalibrationManager
    {
        private readonly ILogger _logger;
        private readonly string _calibrationFolderPath;
        public IEnumerable<ICalibrationData> Calibrations => _calibrations;
        private readonly List<ICalibrationData> _calibrations = new List<ICalibrationData>();
        public CalibrationManager(string calibrationFolderPath, bool initWithPersistentCalibrations = true)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<CalibrationManager>>();
            _calibrationFolderPath = calibrationFolderPath;
            if (initWithPersistentCalibrations)
                InitWithPersistentCalibrations();
        }

        public List<Type> GetCalibrationTypes()
        {
            // Get calibration types with XmlInclude defined in ICalibrationData
            var xmlIncludes = (XmlIncludeAttribute[])Attribute.GetCustomAttributes(typeof(ICalibrationData), typeof(XmlIncludeAttribute));
            var types = xmlIncludes.Select(x => x.Type).ToList();
            return types;
        }
        
        public WaferReferentialSettings GetWaferReferentialSettings(Length waferDiameter)
        {
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            var waferReferential = Calibrations.OfType<WaferReferentialCalibrationData>().FirstOrDefault();
            var waferConfiguration = waferReferential?.WaferConfigurations?.Find(config => config?.WaferDiameter == waferDiameter)?.WaferReferentialSettings;
            if (waferConfiguration != null)
            {
                return waferConfiguration;
            }

            var emeChuckConfig = hardwareManager?.Chuck?.Configuration as EMEChuckConfig;

            if (emeChuckConfig == null)
            {
                throw new Exception("The chuck config should be of type EMEChuckConfig");
            }

            var waferSlotConfiguration = emeChuckConfig.SubstrateSlotConfigs?.Find(config => config.Diameter == waferDiameter);

            if (waferSlotConfiguration != null)
            {
                var positionSensor = waferSlotConfiguration.PositionSensor as XYPosition;
                if (positionSensor != null)
                    return new WaferReferentialSettings
                    {
                        ShiftX = positionSensor.X.Millimeters(), ShiftY = positionSensor.Y.Millimeters()
                    };
            }

            _logger.Error($"No {typeof(WaferReferentialCalibrationData)} calibration or default found");
            return null;
        }

        public List<Filter> GetFilters()
        {
            var filterCalibrationData = Calibrations.OfType<FilterData>().FirstOrDefault();
            if (filterCalibrationData == null || filterCalibrationData.Filters.IsNullOrEmpty())
            {
                throw new Exception("No calibration filter data found");
            }

            return filterCalibrationData.Filters;
        }

        private void InitWithPersistentCalibrations()
        {
            foreach (var calibrationType in GetCalibrationTypes())
            {
                string calibrationFile = Path.Combine(_calibrationFolderPath, calibrationType.Name + ".xml");
                if (File.Exists(calibrationFile))
                {
                    var deserializer = new XmlSerializer(calibrationType);
                    ICalibrationData calibration;
                    using (TextReader reader = new StreamReader(calibrationFile))
                    {
                        calibration = (ICalibrationData)deserializer.Deserialize(reader);
                    }
                    _calibrations.Add(calibration);
                }
                else
                {
                    _logger.Information($"Calibration file {calibrationFile} is missing");
                }
            }
        }
        
        public void UpdateCalibration(ICalibrationData calibrationData)
        {
            calibrationData.CreationDate = DateTime.Now;
            MakeCalibrationPersistent(calibrationData);

            // Update calibrations
            var oldCalibration = _calibrations.FirstOrDefault(calibration => calibration.GetType() == calibrationData.GetType());
            if (oldCalibration != null)
                _calibrations.Remove(oldCalibration);
            _calibrations.Add(calibrationData);
        }

        private void MakeCalibrationPersistent(ICalibrationData calibrationData)
        {
            string calibrationFile = Path.Combine(_calibrationFolderPath, calibrationData.GetType().Name + ".xml");

            if (File.Exists(calibrationFile))
                File.Delete(calibrationFile);

            calibrationData.Serialize(calibrationFile);
        }

        public DistortionData GetDistortion()
        {
            var distortionCalibrationData = Calibrations.OfType<DistortionCalibrationData>().FirstOrDefault();
            if (distortionCalibrationData?.DistortionData == null)
                throw new Exception("No calibration distortion data found");

            return distortionCalibrationData.DistortionData;
        }
        
        public DistanceSensorCalibrationData GetDistanceSensorCalibrationData()
        {
            var distortionCalibrationData = Calibrations.OfType<DistanceSensorCalibrationData>().First();
            if (distortionCalibrationData == null)
                throw new Exception("No calibration DistanceSensor data found");

            return distortionCalibrationData;
        }
        public AxisOrthogonalityCalibrationData GetAxisOrthogonalityCalibrationData()
        {
            var axisOrthogonalityCalibrationData = Calibrations.OfType<AxisOrthogonalityCalibrationData>().First();
            if (axisOrthogonalityCalibrationData == null)
                throw new Exception("No axis orthogonality calibration data found");

            return axisOrthogonalityCalibrationData;
        }
    }
}
