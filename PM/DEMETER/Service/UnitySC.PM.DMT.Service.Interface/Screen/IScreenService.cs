using System.ServiceModel;
using System.Windows.Media;

using UnitySC.PM.Shared.Data.Image;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Hardware.Service.Interface.Screen
{
    [ServiceContract(CallbackContract = typeof(IScreenServiceCallback))]
    public interface IScreenService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<VoidResult> SetScreenColor(string screenId, Color color);

        [OperationContract]
        Response<VoidResult> DisplayImage(string screenId, ServiceImage image);

        [OperationContract]
        Response<ScreenInfo> GetScreenInfo(string screenId);
    }
}
