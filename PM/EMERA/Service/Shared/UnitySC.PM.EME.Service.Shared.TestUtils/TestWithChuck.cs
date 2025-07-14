using System.Collections.Generic;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestWithChuck
    {
        EmeHardwareManager HardwareManager { get; set; }
        Mock<ITestChuck> SimulatedChuck { get; set; }
    }

    public static class TestWithChuckHelper
    {
        public static void Setup(ITestWithChuck test)
        {
            if (test.SimulatedChuck is null)
            {
                test.SimulatedChuck = new Mock<ITestChuck>();
                var config = new EMEChuckConfig();
                config.SubstrateSlotConfigs = new List<SubstSlotWithPositionConfig>();
                config.SubstrateSlotConfigs.Add(new SubstSlotWithPositionConfig() { Diameter = 200.Millimeters(), PositionSensor = new XYPosition(new MotorReferential(), 28.0, 0.0), PositionPark = new XYZPosition(new MotorReferential(), 10.0, 11.0, 12.0), PositionManualLoad = new XYZPosition(new MotorReferential(), 10.3, 11.3, 12.3) });
                config.SubstrateSlotConfigs.Add(new SubstSlotWithPositionConfig() { Diameter = 150.Millimeters(), PositionSensor = new XYPosition(new MotorReferential(), 28.0, 0.0), PositionPark = new XYZPosition(new MotorReferential(), 10.1, 11.1, 12.1), PositionManualLoad = new XYZPosition(new MotorReferential(), 10.4, 11.4, 12.4) });
                config.SubstrateSlotConfigs.Add(new SubstSlotWithPositionConfig() { Diameter = 100.Millimeters(), PositionSensor = new XYPosition(new MotorReferential(), 1.0, 1.0),  PositionPark = new XYZPosition(new MotorReferential(), 10.2, 11.2, 12.2), PositionManualLoad = new XYZPosition(new MotorReferential(), 10.5, 11.5, 12.5) });

                test.SimulatedChuck.SetupGet(_ => _.Configuration).Returns(config);
                test.HardwareManager.Chuck = test.SimulatedChuck.Object;
            }
        }
    }
}
