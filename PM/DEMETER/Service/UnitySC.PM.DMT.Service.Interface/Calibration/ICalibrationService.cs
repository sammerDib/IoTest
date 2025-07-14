using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface
{
    [ServiceContract]
    public interface ICalibrationService
    {
        [OperationContract]
        Response<string> GetCalibrationBaseFolder();

        [OperationContract]
        Response<string> GetCalibrationOutputsFolder();

        [OperationContract]
        Response<bool> HasPerspectiveCalibration(Side side);

        [OperationContract]
        Response<bool> HasUniformityCorrectionCalibration(Side side);

        [OperationContract]
        Response<string> GetPerspectiveCalibrationFullFilePath(Side side);

        [OperationContract]
        Response<VoidResult> ReloadPerspectiveCalibrationForSide(Side side);

        [OperationContract]
        Task<Response<float>> CalibrateCurvatureDynamicsAsync(Side side);

        /// <summary>
        ///     Gets the Exposure Time for the tool to tool exposure time matching algorithm calibration acquisition, in seconds.
        /// </summary>
        /// <param name="side">side to use, eihter FS or BS</param>
        /// <returns>double representing time in seconds</returns>
        [OperationContract]
        Response<double> GetDefaultBlackDeadPixelCalibrationExposureTimeMs(Side side);

        [OperationContract]
        Response<VoidResult> UpdateAndSaveDeadPixels(Side side);

        [OperationContract]
        Response<bool> DoesDeadPixelsCalibrationExist(Side side);

        [OperationContract]
        Response<ServiceImageWithDeadPixels> AcquireDeadPixelsImageForSideAndType(Side side, DeadPixelTypes deadPixelType, int pixelValueThreshold);

        [OperationContract]
        Response<double> GetExposureMatchingAcquisitionExposureTimeMs();
        
        [OperationContract]
        Response<ExposureMatchingInputs> GetExposureMatchingInputs(Side side);

        [OperationContract]
        Response<double> CalibrateExposure(Side side, ExposureMatchingInputs inputs = null);

        [OperationContract]
        Response<ExposureMatchingInputs> GetGoldenValues(Side side);

        [OperationContract]
        Task<Response<VoidResult>> CalibrateSystemAsync(Side side, List<int> periods, double exposureTimeMs);

        [OperationContract]
        Task<Response<VoidResult>> SaveSystemCalibrationBackupAsync(Side side);

        [OperationContract]
        Task<Response<VoidResult>> CalibrateCameraAsync(Side side);

        [OperationContract]
        Task<Response<VoidResult>> SaveCameraCalibrationBackupAsync(Side side);

        [OperationContract]
        Task<Response<VoidResult>> CalibrateSystemUniformityAsync(Side side);

        [OperationContract]
        Task<Response<ServiceImage>> AcquireBrightFieldImageAsync(Side side, double exposureTimeMs);

        [OperationContract]
        Response<VoidResult> RemoveBrightFieldImage(Side side);

        [OperationContract]
        Response<VoidResult> ClearCameraCalibrationImages();

        [OperationContract]
        Task<Response<ServiceImage>> AcquireCameraCalibrationImageAsync(Side side, string imageName,
            double exposureTimeMs);

        [OperationContract]
        Response<VoidResult> RemoveCameraCalibrationImage(Side side, string imageName);

        [OperationContract]
        Response<List<string>> GetCameraCalibrationImageNames();

        [OperationContract]
        Response<bool> DoesGlobalTopoCameraCalibrationExist(Side side);

        [OperationContract]
        Response<bool> DoesGlobalTopoSystemCalibrationExist(Side side);

        [OperationContract]
        Response<VoidResult> SetHighAngleDarkFieldMaskForSide(Side side, ServiceImage image);

        [OperationContract]
        Response<int> GetAlignmentVerticalLineThicknessInPixels();

        [OperationContract]
        Response<bool> IsHighAngleDarkFieldMaskAvailableForSide(Side side);
    }
}
