using Moq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestWithAxes
    {
        AnaHardwareManager HardwareManager { get; set; }
        Mock<IAxes> SimulatedAxes { get; set; }
    }

    public static class TestWithAxesHelper
    {
        public static void Setup(ITestWithAxes test)
        {
            // Handle manually setting up the axes mock (to have a strict mock for example)
            if (test.SimulatedAxes is null)
            {
                test.SimulatedAxes = new Mock<IAxes>();
                test.SimulatedAxes.Setup(_ => _.GetPos()).Returns(new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0));
            }
            test.HardwareManager.Axes = test.SimulatedAxes.Object;
        }
    }
}
