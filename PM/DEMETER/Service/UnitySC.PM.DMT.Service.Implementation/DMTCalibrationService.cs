using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false,
                        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTCalibrationService : BaseService, ICalibrationService
    {
        private readonly IDMTServiceConfigurationManager _serviceConfigurationManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly DMTCameraManager _cameraManager;
        private readonly DMTHardwareManager _hardwareManager;
        private readonly AlgorithmManager _algorithmManager;

        public DMTCalibrationService(
            ILogger logger,
            IDMTServiceConfigurationManager serviceConfigurationManager,
            AlgorithmManager algorithmManager,
            CalibrationManager calibrationManager, DMTCameraManager cameraManager, DMTHardwareManager hardwareManager) : base(logger, ExceptionType.CalibrationException)
        {
            _algorithmManager = algorithmManager;
            _serviceConfigurationManager = serviceConfigurationManager;
            _calibrationManager = calibrationManager;
            _cameraManager = cameraManager;
            _hardwareManager = hardwareManager;
        }

        public Response<string> GetCalibrationBaseFolder()
        {
            return InvokeDataResponse(messagesContainer => _serviceConfigurationManager.CalibrationFolderPath);
        }

        public Response<string> GetCalibrationOutputsFolder()
        {
            return InvokeDataResponse(messageContainer => _serviceConfigurationManager.CalibrationOutputFolderPath);
        }

        Response<bool> ICalibrationService.HasPerspectiveCalibration(Side side)
        {
            return InvokeDataResponse(messagesContainer => !(_calibrationManager.GetPerspectiveCalibrationForSide(side) is null));
        }

        public Response<bool> HasUniformityCorrectionCalibration(Side side)
        {
            return InvokeDataResponse(messagesContainer => _calibrationManager.GetUniformityCorrectionCalibImageBySide(side) != null);
        }

        public Response<string> GetPerspectiveCalibrationFullFilePath(Side side)
        {
            return InvokeDataResponse(messagesContainer => _calibrationManager.GetPerspectiveCalibrationFullFilePathForSide(side));
        }

        public Response<VoidResult> ReloadPerspectiveCalibrationForSide(Side side)
        {
            return InvokeVoidResponse(messages => _calibrationManager.ReloadPerspectiveCalibrationForSide(side));
        }

        public async Task<Response<float>> CalibrateCurvatureDynamicsAsync(Side side)
        {
            return await InvokeDataResponseAsync(async messagesContainer =>
            {
                var acquisitionFlowTask = _calibrationManager.StartAcquireCurvatureDynamicsCalibrationImages(side, _cameraManager);
                return await _calibrationManager.CalibrateCurvatureDynamicsAsync(side, acquisitionFlowTask);
            });
        }

        public Response<double> GetDefaultBlackDeadPixelCalibrationExposureTimeMs(Side side)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var exposureInputs = _calibrationManager.GetCalibrationInputs().BlackDeadPixelExposureInputs;
                return side == Side.Front
                    ? exposureInputs.DefaultCalibrationExposureTimeMsFS
                    : exposureInputs.DefaultCalibrationExposureTimeMsBS;
            });
        }

        public Response<ServiceImageWithDeadPixels> AcquireDeadPixelsImageForSideAndType(Side side,
            DeadPixelTypes deadPixelType, int pixelValueThreshold)
        {
            return InvokeDataResponse(messages =>
            {
                var image = _cameraManager.GetImageWithDeadPixels(side, deadPixelType, pixelValueThreshold);
                _calibrationManager.SetDeadPixelsImageForSideAndType(side, image);
                return image;
            });
        }

        public Response<VoidResult> UpdateAndSaveDeadPixels(Side side)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _calibrationManager.UpdateAndSaveDeadPixels(side);
            });
        }

        public Response<bool> DoesDeadPixelsCalibrationExist(Side side)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _calibrationManager.DoesDeadPixelsCalibrationExist(side);
            });
        }

        public Response<double> GetExposureMatchingAcquisitionExposureTimeMs()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var exposureMatchingInputs = _calibrationManager.GetCalibrationInputs().ExposureMatchingInputs;
                return exposureMatchingInputs.AcquisitionExposureTimeMs;
            });
        }

        public Response<ExposureMatchingInputs> GetExposureMatchingInputs(Side side)
        {
            return InvokeDataResponse(messageContainer =>
            {
                var inputs = _calibrationManager.GetCalibrationInputs().ExposureMatchingInputs; 
                return new ExposureMatchingInputs
                {
                    GoldenValues = new List<ExposureMatchingGoldenValues> { inputs.GoldenValuesBySide[side] },
                    AcquisitionExposureTimeMs = inputs.AcquisitionExposureTimeMs

                };
            });
        }

        public Response<double> CalibrateExposure(Side side, ExposureMatchingInputs inputs = null)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _calibrationManager.CalibrateExposure(side, inputs);
            });
        }

        public Response<ExposureMatchingInputs> GetGoldenValues(Side side)
        {
            return InvokeDataResponse(_ => _calibrationManager.GetGoldenValues(side));
        }

        public async Task<Response<VoidResult>> CalibrateSystemAsync(Side side, List<int> periods, double exposureTimeMs)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                var waferDiameter = _hardwareManager.GetCurrentChuckMaximumDiameter();
                using (var image = await _calibrationManager.AcquireBrightFieldImageForCalibration(side, exposureTimeMs, _cameraManager))
                {
                    var acquisitionFlowTask = _calibrationManager.StartAcquirePhaseImagesForPeriods(side, periods, exposureTimeMs, _cameraManager);
                    _calibrationManager.CalibrateGlobalTopoSystem(side, periods, image, acquisitionFlowTask, waferDiameter);    
                }
            });
        }

        public async Task<Response<VoidResult>> SaveSystemCalibrationBackupAsync(Side side)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _calibrationManager.SaveGlobalTopoSystemCalibrationBackup(side));
            });
        }

        public async Task<Response<VoidResult>> CalibrateCameraAsync(Side side)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _calibrationManager.CalibrateGlobalTopoCamera(side));
            });
        }

        public async Task<Response<VoidResult>> SaveCameraCalibrationBackupAsync(Side side)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _calibrationManager.SaveGlobalTopoCameraCalibrationBackup(side));
            });
        }

        public async Task<Response<ServiceImage>> AcquireCameraCalibrationImageAsync(Side side, string imageName, double exposureTimeMs)
        {
            return await InvokeDataResponseAsync(async messagesContainer =>
            {
                var image = await _calibrationManager.AcquireBrightFieldImageForCalibration(side, exposureTimeMs, _cameraManager);
                return _calibrationManager.StoreGlobalTopoCameraCalibrationImage(side, image, imageName, exposureTimeMs);
            });
        }

        public Response<VoidResult> RemoveCameraCalibrationImage(Side side, string imageName)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _calibrationManager.RemoveGlobalTopoCameraCalibrationImage(side, imageName);
            });
        }

        public Response<VoidResult> ClearCameraCalibrationImages()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _calibrationManager.ClearGlobalTopoCameraCalibrationImages();
            });
        }

        public Response<List<string>> GetCameraCalibrationImageNames()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _calibrationManager.GetGlobalTopoCameraCalibrationImagesName().ToList();
            });
        }

        public Response<bool> DoesGlobalTopoCameraCalibrationExist(Side side)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _calibrationManager.DoesGlobalTopoCameraCalibrationExist(side);
            });
        }

        public Response<bool> DoesGlobalTopoSystemCalibrationExist(Side side)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _calibrationManager.DoesGlobalTopoSystemCalibrationExist(side);
            });
        }

        public Response<VoidResult> SetHighAngleDarkFieldMaskForSide(Side side, ServiceImage image)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                var procImage = new USPImageMil(image);
                _calibrationManager.SetHighAngleDarkFieldMaskForSide(side, procImage);
            });
        }

        public Response<bool> IsHighAngleDarkFieldMaskAvailableForSide(Side side)
        {
            return InvokeDataResponse(messagesContainer => !(_calibrationManager.GetHighAngleDarkFieldMaskForSide(side) is null));
        }

        public Response<int> GetAlignmentVerticalLineThicknessInPixels()
        {
            return InvokeDataResponse(messageContainer =>
            {
                return _calibrationManager.GetCalibrationInputs().AlignmentScreenVerticalLineThicknessInPixels;
            });
        }

        public async Task<Response<VoidResult>> CalibrateSystemUniformityAsync(Side side)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _calibrationManager.CalibrateSystemUniformityAsync(side));
            });
        }

        public async Task<Response<ServiceImage>> AcquireBrightFieldImageAsync(Side side, double exposureTimeMs)
        {
            return await InvokeDataResponseAsync(async messagesContainer =>
            {
                var image = await _calibrationManager.AcquireBrightFieldImageForCalibration(side, exposureTimeMs, _cameraManager);
                _calibrationManager.SetBrightFieldImageForSide(side, image);
                return image.ToServiceImage();
            });
        }

        public Response<VoidResult> RemoveBrightFieldImage(Side side)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _calibrationManager.RemoveBrightFieldImage(side);
            });
        }
    }
}
