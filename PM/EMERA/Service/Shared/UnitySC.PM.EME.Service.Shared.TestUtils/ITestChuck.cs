using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.Configuration;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestChuck : IUSPChuck, ISubstrateSlotChuckConfig<SubstSlotWithPositionConfig>,  IChuckMaterialPresence, IChuckClamp, IChuckLoadingPosition
    {
    }
}
