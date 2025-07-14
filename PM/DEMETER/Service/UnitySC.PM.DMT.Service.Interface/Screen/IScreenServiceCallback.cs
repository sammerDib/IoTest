using System.ServiceModel;
using System.Windows.Media;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Hardware.Service.Interface.Screen
{
    [ServiceContract]
    public interface IScreenServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ScreenImageChangedCallback(string screenId, ServiceImage image, Color color);

        [OperationContract(IsOneWay = true)]
        void OnBacklightChangedCallback(Side side, short value);

        [OperationContract(IsOneWay = true)]
        void OnBrightnessChangedCallback(Side side, short value);

        [OperationContract(IsOneWay = true)]
        void OnContrastChangedCallback(Side side, short value);

        [OperationContract(IsOneWay = true)]
        void OnSharpnessChangedCallback(Side side, int value);

        [OperationContract(IsOneWay = true)]
        void OnTemperatureChangedCallback(Side side, double value);

        [OperationContract(IsOneWay = true)]
        void OnFanChangedCallback(Side side, int value);

        [OperationContract(IsOneWay = true)]
        void OnFanAutoChangedCallback(Side side, bool value);

        [OperationContract(IsOneWay = true)]
        void OnPowerStateChangedCallback(Side side, bool value);
    }
}
