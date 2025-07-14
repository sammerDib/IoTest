using System.ServiceModel;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Camera
{
    [ServiceContract(CallbackContract = typeof(ICameraServiceCallback))]
    [ServiceKnownType(typeof(CameraInputParams))]
    public interface ICameraServiceEx : ICameraService
    {
        [OperationContract]
        Response<bool> SetSettings(string cameraId, ICameraInputParams inputParameters);

        [OperationContract]
        Response<ICameraInputParams> GetSettings(string cameraId);

        [OperationContract]
        Response<ServiceImage> GetSingleGrabImage(string cameraId);
    }
}
