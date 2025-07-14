using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface.Screen
{
    [ServiceContract(CallbackContract = typeof(IScreenServiceCallback))]
    public interface IDMTScreenService
    {
        [OperationContract]
        Response<VoidResult> SetScreenColorOnSide(Side side, Color color, bool? applyEllipseMask);

        [OperationContract]
        Response<VoidResult> DisplayImageOnSide(Side side, ServiceImage image, bool? applyEllipseMask);

        [OperationContract]
        Response<VoidResult> DisplayCrossOnSide(Side side, Color backgroundColor, Color crossColor, int thickness, double centerX, double centerY);

        [OperationContract]
        Response<VoidResult> DisplayFringeOnSide(Side side, Measure.Fringe fringe, int imageIndex, Color color);

        [OperationContract]
        Response<VoidResult> DisplayHighAngleDarkFielMaskdOnSide(Side side, Color color);

        [OperationContract]
        Response<List<Measure.Fringe>> GetAvailableFringes();

        [OperationContract]
        Response<List<Color>> GetAvailableColors();

        [OperationContract]
        Response<ScreenInfo> GetScreenInfoForSide(Side side);

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent(Side side);

        [OperationContract]
        Task<Response<VoidResult>> SwitchScreenOnOffAsync(Side screenSide, bool on);

        [OperationContract]
        Task<Response<VoidResult>> RestoreParametersAsync(Side screenSide);

        [OperationContract]
        Task<Response<VoidResult>> TurnFanAutoOnAsync(Side screenSide, bool isFanAuto);

        [OperationContract]
        Task<Response<VoidResult>> SetBrightnessAsync(Side screenSide, short brightness);

        [OperationContract]
        Task<Response<VoidResult>> SetBacklightAsync(Side screenSide, short backlight);

        [OperationContract]
        Task<Response<VoidResult>> SetContrastAsync(Side screenSide, short contrast);

        [OperationContract]
        Task<Response<VoidResult>> SetSharpnessAsync(Side screenSide, int sharpness);

        [OperationContract]
        Task<Response<VoidResult>> SetFanSpeedAsync(Side screenSide, int fanSpeed);

        [OperationContract]
        Response<short> GetBacklight(Side screenSide);

        [OperationContract]
        Response<short> GetBrightness(Side side);

        [OperationContract]
        Response<short> GetContrast(Side screenSide);

        [OperationContract]
        Response<double> GetTemperature(Side screenSide);

        [OperationContract]
        Response<int> GetFanRPM(Side screenSide);

        [OperationContract]
        Response<Dictionary<string, short>> GetDefaultScreenValues(Side screenSide);

        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeFromChanges();

        [OperationContract]
        Task<Response<VoidResult>> SetScreenColorAsync(string screenId, Color color);

        [OperationContract]
        Task<Response<VoidResult>> DisplayImageAsync(string screenId, ServiceImage image);

        [OperationContract]
        Response<ScreenInfo> GetScreenInfo(string screenId);
    }
}
