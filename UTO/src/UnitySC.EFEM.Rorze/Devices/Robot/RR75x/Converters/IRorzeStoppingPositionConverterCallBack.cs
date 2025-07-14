using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Converters
{
    public interface IRorzeStoppingPositionConverterCallBack : IStoppingPositionConverterCallBack
    {
        string GetInnerLocation(TransferLocation transferLocation);
    }
}
