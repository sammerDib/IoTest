using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using Path = System.IO.Path;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CalibrationService : BaseService, ICalibrationService
    {
        private readonly string _calibrationFilePath;
        private const string CameraCalibrationFilename = "CameraCalibrationData.xml";
        private readonly CalibrationManager _calibrationManager;

        public CalibrationService(ILogger logger, IEMEServiceConfigurationManager configurationManager, CalibrationManager calibrationManager) : base(logger,
            ExceptionType.HardwareException)
        {
            _calibrationManager = calibrationManager;
            _calibrationFilePath =
                Path.Combine(configurationManager.CalibrationFolderPath, CameraCalibrationFilename);
        }

        public Response<CameraCalibrationData> GetCameraCalibrationData()
        {
            if (!File.Exists(_calibrationFilePath))
            {
                return new Response<CameraCalibrationData>
                {
                    Exception = new ExceptionService($"{_calibrationFilePath} is not found.",
                        ExceptionType.CalibrationException)
                };
            }

            using (TextReader reader = new StreamReader(_calibrationFilePath))
            {
                var calibration = CameraCalibrationData.ReadFrom(reader);
                return new Response<CameraCalibrationData> { Result = calibration };
            }
        }

        public Response<string> GetCalibrationPath()
        {
            return new Response<string> { Result = _calibrationFilePath };
        }

        public Response<IEnumerable<ICalibrationData>> GetCalibrations()
        {
            return InvokeDataResponse(() =>
            {                
                try
                {
                    return _calibrationManager.Calibrations;
                }
                catch (Exception ex)
                {
                    _logger.Error("Calibration cannot GetCalibrations from configuration", ex);
                    return null;
                }
            });
        }
        
        public Response<WaferReferentialSettings> GetWaferReferentialSettings(Length waferDiameter)
        {
            return InvokeDataResponse(() => _calibrationManager.GetWaferReferentialSettings(waferDiameter));
        }

        public Response<List<Filter>> GetFilters()
        {
            return InvokeDataResponse(() =>
            {               
                try
                {
                    return _calibrationManager.GetFilters();
                }
                catch (Exception ex)
                {
                    _logger.Error("Calibration cannot GetFilters from configuration", ex);
                    return null;
                }
            });
        }

        public Response<VoidResult> SaveCalibration(ICalibrationData calibrationData)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _calibrationManager.UpdateCalibration(calibrationData);
            });            
        }

        public Response<int> GetNeededCalibrationCount()
        {
            return InvokeDataResponse(() =>
            {
                try
                {
                    return _calibrationManager.GetCalibrationTypes().Count;
                }
                catch (Exception ex)
                {
                    _logger.Error("Calibration cannot GetCalibrationTypes from configuration", ex);
                    return 0;
                }
            });
        }
    }
}
