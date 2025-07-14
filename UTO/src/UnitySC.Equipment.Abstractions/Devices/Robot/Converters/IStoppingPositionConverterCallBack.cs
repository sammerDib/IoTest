using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Converters
{
    public interface IStoppingPositionConverterCallBack
    {
        SampleDimension GetSampleDimension(TransferLocation transferLocation, RobotArm arm);
    }
}
