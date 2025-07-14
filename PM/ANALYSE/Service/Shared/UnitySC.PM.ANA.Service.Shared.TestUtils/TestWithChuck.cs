using Moq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestWithChuck
    {
        AnaHardwareManager HardwareManager { get; set; }
        Mock<ITestChuck> SimulatedChuck { get; set; }
    }

    public static class TestWithChuckHelper
    {

        public static double XChuckOffset_mm = 0.500;
        public static double YChuckOffset_mm = -0.250;

        public static void Setup(ITestWithChuck test)
        {
            if (test.SimulatedChuck is null)
            {
                test.SimulatedChuck = new Mock<ITestChuck>();
                var opticalRef = new OpticalReferenceDefinition();
                opticalRef.RefThickness = 750.Micrometers();
                opticalRef.RefTolerance = 5.Micrometers();
                //TODO Update
                opticalRef.RefRefrIndex = (float)1.43;
                opticalRef.PositionX = 131.78700256347656.Millimeters();
                opticalRef.PositionY = 114.99700164794922.Millimeters();
                opticalRef.PositionZ = 0.Millimeters();
                opticalRef.PositionZLower = 0.Millimeters();
                opticalRef.ReferenceName = "REF 750UM-sori";

                var chuckConfig = new ANAChuckConfig();
                chuckConfig.ReferencesList.Add(opticalRef);

                // Add subrate slot config
                var slotconfig300mm = new SubstrateSlotConfig() {
                    Name = "testSlot300mm",
                    Diameter = 300.Millimeters(),
                    IsPresenceSensorAvailable = true,
                    PositionPark = new XYZTopZBottomPosition(new StageReferential(), -218.2, -176.5, 15.0, -15.0),
                    PositionManualLoad = new XYZTopZBottomPosition(new StageReferential(), 0.0, 154.0, 15.0, 0.0),
                    PositionChuckCenter = new XYPosition(new StageReferential(), XChuckOffset_mm, YChuckOffset_mm),
                };
                var slotconfig200mm = new SubstrateSlotConfig()
                {
                    Name = "testSlot200mm",
                    Diameter = 100.Millimeters(),
                    IsPresenceSensorAvailable = false,
                    PositionPark = new XYZTopZBottomPosition(new StageReferential(), -218.2, -176.5, 15.0, -15.0),
                    PositionManualLoad = new XYZTopZBottomPosition(new StageReferential(), 0.0, 154.0, 15.0, 0.0),
                    PositionChuckCenter = new XYPosition(new StageReferential(), XChuckOffset_mm, YChuckOffset_mm),
                };
                chuckConfig.SubstrateSlotConfigs.Add(slotconfig300mm);
                chuckConfig.SubstrateSlotConfigs.Add(slotconfig200mm);

                test.SimulatedChuck.SetupGet(_ => _.Configuration).Returns(chuckConfig);
                //test.HardwareManager.Chuck.Configuration = chuckDef;
                //  test.SimulatedChuck.Object.Configuration = chuckDef;
                test.HardwareManager.Chuck = test.SimulatedChuck.Object;
            }
        }
    }
}
