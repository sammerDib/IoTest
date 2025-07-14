using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Hardware.Chiller
{
    public class ChillerTemperatureChangedMessage
    {
        public ChillerTemperatureChangedMessage(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
    
    public class ChillerFanSpeedChangedMessage
    {
        public ChillerFanSpeedChangedMessage(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
    
    public class ChillerMaxCompressionSpeedChangedMessage
    {
        public ChillerMaxCompressionSpeedChangedMessage(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
    
    public class ChillerConstantFanSpeedModeChangedMessage
    {
        public ChillerConstantFanSpeedModeChangedMessage(ConstFanSpeedMode value)
        {
            Value = value;
        }

        public ConstFanSpeedMode Value { get; }
    }
    
    public class ChillerModeChangedMessage
    {
        public ChillerModeChangedMessage(ChillerMode value)
        {
            Value = value;
        }

        public ChillerMode Value { get; }
    }
}
