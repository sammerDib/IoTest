using Moq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared.Hardware.Light;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestWithLight
    {
        string LightId { get; set; }

        Mock<LightBase> SimulatedLight { get; set; }

        AnaHardwareManager HardwareManager { get; set; }
    }

    public static class TestWithLightHelper
    {
        public static void Setup(ITestWithLight test)
        {
            test.LightId = "lightId";
            test.SimulatedLight = new Mock<LightBase>();
            test.HardwareManager.Lights[test.LightId] = test.SimulatedLight.Object;
        }
    }
}
