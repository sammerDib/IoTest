using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestChuck : IUSPChuck, IChuckClamp
    {
    }
}
