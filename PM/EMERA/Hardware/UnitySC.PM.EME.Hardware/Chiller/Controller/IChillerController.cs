using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Hardware.Chiller.Controller
{
    public interface IChillerController
    {
        void SetTemperature(double value);
        void SetFanSpeed(double value);
        void SetMaxCompressionSpeed(int value);
        void SetConstantFanSpeedMode(ConstFanSpeedMode value);
        void Reset();
        void SetChillerMode(ChillerMode value);
    }
}
