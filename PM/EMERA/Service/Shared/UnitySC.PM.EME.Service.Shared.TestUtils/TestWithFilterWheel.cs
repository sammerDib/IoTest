using System.Collections.Generic;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.CNC;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestWithFilterWheel
    {
        EmeHardwareManager HardwareManager { get; set; }
        Mock<FilterWheel> SimulatedFilterWheel { get; set; }
    }

    public static class TestWithFilterWheelHelper
    {
        public static void Setup(ITestWithFilterWheel test)
        {
            if (test.SimulatedFilterWheel is null)
            {
                var logger = new SerilogLogger<FilterWheel>();

                var axisConfig = new CNCAxisConfig
                {
                    MovingDirection = MovingDirection.Rotation,
                    ControllerID = "Rotation",
                    AxisID = "Rotation",
                    PositionMin = 0.Millimeters(),
                    PositionMax = 359.9999.Millimeters(),
                };

                var slots = new List<FilterSlot>
                {
                    new FilterSlot { Name = "Slot 1", Position = 0 },
                    new FilterSlot { Name = "Slot 2", Position = 20 },
                    new FilterSlot { Name = "Slot 3", Position = 40 },
                    new FilterSlot { Name = "Slot 4", Position = 80 }
                };

                var axesConfig = new FilterWheelConfig { AxisConfig = axisConfig, FilterSlots = slots };

                var controllerConfig = new CNCMotionControllerConfig { Name = "Rotation", DeviceID = "Rotation", IsEnabled = false };
                var cncDummyController = new CNCMotionDummyController(controllerConfig, null, logger);

                test.SimulatedFilterWheel = new Mock<FilterWheel>(axesConfig, cncDummyController, null, logger);
                test.SimulatedFilterWheel.Setup(_ => _.GetCurrentPosition()).Returns(0.0);
            }

            test.HardwareManager.Wheel = test.SimulatedFilterWheel.Object;
        }
    }
}
