using System.ServiceModel;
using System.Windows;

using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Camera
{
    [ServiceContract(CallbackContract = typeof(ICameraServiceCallback))]
    public interface ICameraService
    {
        [OperationContract]
        Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale);

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<VoidResult> SetCameraExposureTime(string cameraId, double exposureTimeMs);

        [OperationContract]
        Response<ServiceImage> GetCameraImage(string cameraId);

        [OperationContract]
        Response<CameraInfo> GetCameraInfo(string cameraId);

        [OperationContract]
        Response<VoidResult> StartAcquisition(string cameraId);

        [OperationContract]
        Response<VoidResult> StopAcquisition(string cameraId);

        [OperationContract]
        Response<double> GetCameraFrameRate(string cameraId);
    }
}
