using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

using Workstation.ServiceModel.Ua;

namespace UnitySC.PM.DMT.Shared.UI.Proxy
{
    public class CalibrationSupervisor
    {
        private readonly ServiceInvoker<ICalibrationService> _calibrationService;
        private readonly InstanceContext _instanceContext;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;

        public CalibrationSupervisor(ILogger<CalibrationSupervisor> logger, ILogger<ICalibrationService> serviceLogger,
            IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // Acquisition service
            _calibrationService =
                new ServiceInvoker<ICalibrationService>("DEMETERCalibrationService", serviceLogger, _messenger);

            _logger = logger;
            _messenger = messenger;
        }

        public string GetCalibrationBaseFolder()
        {
            string calibrationBaseFolder = _calibrationService.Invoke(x => x.GetCalibrationBaseFolder());
            return calibrationBaseFolder;
        }

        public string GetCalibrationOutputsFolder()
        {
            return _calibrationService.Invoke(s => s.GetCalibrationOutputsFolder());
        }

        public bool HasPerspectiveCalibration(Side side)
        {
            return _calibrationService.Invoke(s => s.HasPerspectiveCalibration(side));
        }

        public bool HasUniformityCorrectionCalibration(Side side)
        {
            return _calibrationService.Invoke(s => s.HasUniformityCorrectionCalibration(side));
        }

        public string GetPerspectiveCalibrationFullFilePath(Side side)
        {
            return _calibrationService.Invoke(s => s.GetPerspectiveCalibrationFullFilePath(side));
        }

        public void ReloadPerspectiveCalibrationForSide(Side side)
        {
            _calibrationService.Invoke(s => s.ReloadPerspectiveCalibrationForSide(side));
        }

        public async Task<float> CalibrateCurvatureDynamicsAsync(Side side)
        {
            return await _calibrationService.InvokeAsync(s => s.CalibrateCurvatureDynamicsAsync(side));
        }

        public double GetDefaultBlackDeadPixelCalibrationExposureTime(Side side)
        {
            return _calibrationService.Invoke(s => s.GetDefaultBlackDeadPixelCalibrationExposureTimeMs(side));
        }

        public ServiceImageWithDeadPixels AcquireDeadPixelsImageForSideAndType(Side side, DeadPixelTypes types, int pixelThresholdValue)
        {
            return _calibrationService.Invoke(s => s.AcquireDeadPixelsImageForSideAndType(side, types, pixelThresholdValue));
        }

        public void UpdateAndSaveDeadPixels(Side side)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.UpdateAndSaveDeadPixels(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public bool DoesDeadPixelsCalibrationExist(Side side)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.DoesDeadPixelsCalibrationExist(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public double GetExposureMatchingAcquistionExposureTimeMs()
        {
            return _calibrationService.Invoke(s => s.GetExposureMatchingAcquisitionExposureTimeMs());
        }

        public ExposureMatchingInputs GetExposureMatchingInputs(Side side)
        {
            return _calibrationService.Invoke(s => s.GetExposureMatchingInputs(side));
        }

        public double CalibrateExposure(Side side, ExposureMatchingInputs inputs = null)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.CalibrateExposure(side, inputs));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public ExposureMatchingInputs GetGoldenValues(Side side)
        {
            return _calibrationService.TryInvokeAndGetMessages(s => s.GetGoldenValues(side))?.Result;
        }

        public async Task CalibrateSystemAsync(Side side, List<int> periods, double exposureTimeMs)
        {
            var result =
                await _calibrationService.InvokeAndGetMessagesAsync(s =>
                    s.CalibrateSystemAsync(side, periods, exposureTimeMs));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public async Task SaveSystemCalibrationBackupAsync(Side side)
        {
            var result =
                await _calibrationService.InvokeAndGetMessagesAsync(s => s.SaveSystemCalibrationBackupAsync(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public async Task CalibrateCameraAsync(Side side)
        {
            var result = await _calibrationService.InvokeAndGetMessagesAsync(s => s.CalibrateCameraAsync(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public async Task SaveCameraCalibrationBackupAsync(Side side)
        {
            var result =
                await _calibrationService.InvokeAndGetMessagesAsync(s => s.SaveCameraCalibrationBackupAsync(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public async Task CalibrateSystemUniformityAsync(Side side)
        {
            var result =
                await _calibrationService.InvokeAndGetMessagesAsync(s => s.CalibrateSystemUniformityAsync(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public List<string> GetCameraCalibrationImageNames()
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.GetCameraCalibrationImageNames());
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public async Task<ServiceImage> AcquireBrightFieldImageAsync(Side side, double exposureTimeMs)
        {
            var result =
                await _calibrationService.InvokeAndGetMessagesAsync(s =>
                    s.AcquireBrightFieldImageAsync(side, exposureTimeMs));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public void RemoveBrightFieldImage(Side side)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.RemoveBrightFieldImage(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public async Task<ServiceImage> AcquireCameraCalibrationImageAsync(Side side, string imageName,
            double exposureTimeMs)
        {
            var result = await _calibrationService.InvokeAndGetMessagesAsync(s =>
                s.AcquireCameraCalibrationImageAsync(side, imageName, exposureTimeMs));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public void RemoveCameraCalibrationImage(Side side, string imageName)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.RemoveCameraCalibrationImage(side, imageName));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public void ClearCameraCalibrationImages()
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.ClearCameraCalibrationImages());
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }
        }

        public bool DoesGlobalTopoCameraCalibrationExist(Side side)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.DoesGlobalTopoCameraCalibrationExist(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public bool DoesGlobalTopoSystemCalibrationExist(Side side)
        {
            var result = _calibrationService.InvokeAndGetMessages(s => s.DoesGlobalTopoSystemCalibrationExist(side));
            if (result.Exception != null)
            {
                throw new Exception(result.Exception.Message);
            }

            return result.Result;
        }

        public bool SetHighAngleDarkFieldMaskForSide(Side side, ServiceImage image)
        {
            var response = _calibrationService.InvokeAndGetMessages(s => s.SetHighAngleDarkFieldMaskForSide(side, image));
            return response.Exception == null;
        }

        public int GetAlignmentVerticalLineThicknessInPixels()
        {
            return _calibrationService.Invoke(s => s.GetAlignmentVerticalLineThicknessInPixels());
        }

        public bool IsHighAngleDarkFieldMaskAvailableForSide(Side side)
        {
            return _calibrationService.Invoke(s => s.IsHighAngleDarkFieldMaskAvailableForSide(side));
        }
    }
}
