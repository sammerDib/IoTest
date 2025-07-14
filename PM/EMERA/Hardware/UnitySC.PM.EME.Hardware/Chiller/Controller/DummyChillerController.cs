using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Chiller.Controller
{
    public class DummyChillerController : IChillerController
    {
        private double _temperature;
        private double _fanSpeed;
        private int _maxCompressionSpeed;
        private ConstFanSpeedMode _constantFanSpeedMode;
        private ChillerMode _chillerMode;
        private static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        public void SetTemperature(double value)
        {
            _temperature = value;
            Messenger.Send(new ChillerTemperatureChangedMessage(value));
        }

        public void SetFanSpeed(double value)
        {
            _fanSpeed = value;
            Messenger.Send(new ChillerFanSpeedChangedMessage(value));
        }

        public void SetMaxCompressionSpeed(int value)
        {
            _maxCompressionSpeed = value;
            Messenger.Send(new ChillerMaxCompressionSpeedChangedMessage(value));
        }

        public void SetConstantFanSpeedMode(ConstFanSpeedMode value)
        {
            _constantFanSpeedMode = value;
            Messenger.Send(new ChillerConstantFanSpeedModeChangedMessage(value));
        }

        public void Reset()
        {
            
        }

        public void SetChillerMode(ChillerMode value)
        {
            _chillerMode = value;
            Messenger.Send(new ChillerModeChangedMessage(value));
        }
    }
}
