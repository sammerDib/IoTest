using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

using Size = System.Drawing.Size;

namespace UnitySC.PM.DMT.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(ICameraServiceCallback))]
    public interface IDMTCameraService : ICameraService
    {
        // Retrieves the list of the cameras id
        [OperationContract]
        Response<List<Side>> GetCamerasSides();

        [OperationContract]
        Response<Size> GetCalibratedImageSize(Side side);

        /// <summary>
        ///    Acquire an image from the camera and apply perspective calibration
        /// </summary>
        [OperationContract]
        Response<ServiceImage> GetCalibratedImage(Side side);

        [OperationContract]
        Response<ServiceImage> GetCameraImageBySide(Side side);

        [OperationContract]
        Response<VoidResult> SetStatisticRoi(Side side, ROI statisticRoi);

        [OperationContract]
        Response<ServiceImageWithStatistics> GetCalibratedImageWithStatistics(Side side, Int32Rect acquisitionRoi,
            double scale, ROI statisticRoi);

        [OperationContract]
        Response<ServiceImageWithStatistics> GetRawImageWithStatistics(Side side, Int32Rect acquisitionRoi,
            double scale, ROI statisticRoi);

        [OperationContract]
        Response<ServiceImageWithFocus> GetImageWithFocus(Side side, double scale, int waferSize, int patternSize);

        [OperationContract]
        Response<Rect> CalibratedImageToMicrons(Side side, Rect pixelRect);

        [OperationContract]
        Response<Rect> MicronsToCalibratedImage(Side side, Rect micronRect);

        [OperationContract]
        Response<VoidResult> SetExposureTimeForSide(Side side, double exposureTimeMs, string screenId = null, int period = 0);

        [OperationContract]
        Response<double> GetGain(Side side);

        [OperationContract]
        Response<VoidResult> SetGain(Side side, double gain);

        [OperationContract]
        Response<VoidResult> TrigAndWait(Side side);

        [OperationContract]
        Response<CameraInfo> GetCameraInfoBySide(Side side);

        [OperationContract]
        Response<OpticalMountShape> GetOpticalMountShape(Side side);

        [OperationContract]
        Response<double> GetCameraFrameRateBySide(Side side);

        [OperationContract]
        Response<VoidResult> StartContinuousAcquisition(Side side);


        [OperationContract]
        Response<VoidResult> StopContinuousAcquisition(Side side);
    }
}
