using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Camera
{
    [ServiceContract(CallbackContract = typeof(ICameraServiceCallbackEx))]
    public interface ICameraServiceEx
    {
        [OperationContract]
        Response<double> GetCameraExposureTime();

        [OperationContract]
        Response<VoidResult> SetAOI(Rect aoi);

        [OperationContract]
        Response<double> GetCameraGain();

        [OperationContract]
        Response<ServiceImage> GetScaledCameraImage(Int32Rect roi, double scale);

        [OperationContract]
        Response<MatroxCameraInfo> GetMatroxCameraInfo();

        [OperationContract]
        Response<VoidResult> SetCameraGain(double gain);

        [OperationContract]
        Response<double> GetFrameRate();

        [OperationContract]
        Response<ColorMode> GetColorMode();

        [OperationContract]
        Response<List<ColorMode>> GetColorModes();

        [OperationContract]
        Response<bool> IsAcquiring();

        [OperationContract]
        Response<VoidResult> SetColorMode(ColorMode colorMode);

        [OperationContract]
        Response<string> GetCameraID();

        [OperationContract]
        Response<ServiceImage> SingleAcquisition();

        [OperationContract]
        Response<ServiceImage> SingleScaledAcquisition(Int32Rect roi, double scaleValue);

        [OperationContract]
        Response<VoidResult> SetStreamedImageDimension(Int32Rect roi, double scale);

        [OperationContract]
        Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale);

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<VoidResult> SetCameraExposureTime(double exposureTime);

        [OperationContract]
        Response<ServiceImage> GetCameraImage();

        [OperationContract]
        Response<CameraInfo> GetCameraInfo();

        [OperationContract]
        Response<bool> StartAcquisition();

        [OperationContract]
        Response<VoidResult> StopAcquisition();

        [OperationContract]
        Response<double> GetCameraFrameRate();
    }
}
