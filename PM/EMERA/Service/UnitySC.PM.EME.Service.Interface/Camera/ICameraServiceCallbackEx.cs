using System.ServiceModel;

using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Service.Interface.Camera
{
    [ServiceContract]
    public interface ICameraServiceCallbackEx
    {
        [OperationContract(IsOneWay = true)]
        void ImageGrabbedCallback(ServiceImageWithStatistics image);
    }
}
