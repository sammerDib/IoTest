using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    public interface IChuckMaterialPresence
    {
        MaterialPresence CheckWaferPresence(Length size);
    }
}
