using System.ServiceModel;

using UnitySC.Shared.Image;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Camera
{
    [ServiceContract]
    public interface ICameraServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ImageGrabbedCallback(string cameraId, ServiceImage image);
    }
}
