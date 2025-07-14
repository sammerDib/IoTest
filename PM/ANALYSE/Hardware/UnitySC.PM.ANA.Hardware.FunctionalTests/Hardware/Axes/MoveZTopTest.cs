using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Phytron;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes
{
    public class MoveZTopTest : FunctionalTest
    {
        public override void Run()
        {
            var harwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            harwareManager.Controllers.TryGetValue("MCCMotionController-Top", out var controller);
            var ZTopAxis = harwareManager.Axes.Axes.Find(axis => axis.AxisID == "ZTop");

            const double expectedPosition_mm = 5;
            const double precision_mm = 0.01;

            var coordinates = new List<double> { expectedPosition_mm };
            var axes = new List<IAxis> { ZTopAxis };
            var accell = new List<AxisSpeed> { AxisSpeed.Normal };

            var mccController = (controller as MCCController);

            double currentPos_mm = mccController.GetCurrentPosition().Position;
            if (!currentPos_mm.Near(0, precision_mm))
            {
                throw new Exception($"Ztop position should be near 0 mm (+/- {precision_mm} mm) but is at position {currentPos_mm} mm.");
            }

            mccController.SetPosAxis(coordinates, axes, accell);
            mccController.WaitMotionEnd(2_000);

            currentPos_mm = mccController.GetCurrentPosition().Position;
            if (!currentPos_mm.Near(expectedPosition_mm, precision_mm))
            {
                throw new Exception($"Ztop position should be near {expectedPosition_mm} mm (+/- {precision_mm} mm) but is at position {currentPos_mm} mm.");
            }

            Logger.Information("[MoveZTopTest] TEST SUCCESSFUL !");
        }
    }
}
