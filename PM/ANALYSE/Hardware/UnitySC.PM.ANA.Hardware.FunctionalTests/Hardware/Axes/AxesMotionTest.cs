using System;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes
{
    public class AxesMotionTest : FunctionalTest
    {
        public override void Run()
        {
            // GetPos
            Console.WriteLine("Test_GetPos - Starting");
            var position = HardwareManager.Axes.GetPos();
            Console.WriteLine($"Position = {position}");

            // GotoPosition
            Console.WriteLine("Test_GotoPosition - Starting");
            HardwareManager.Axes.GotoPosition(new XYPosition(new StageReferential(), double.NaN, 0.0), AxisSpeed.Normal);

            // GotoPointCustomSpeedAccel
            Console.WriteLine("Test_GotoPointCustomSpeedAccel - Starting");
            HardwareManager.Axes.GotoPointCustomSpeedAccel(null, new AxisMove(0.0, 200, 20), null, null);

            // MoveIncremental
            Console.WriteLine("Test_MoveIncremental - Starting");
            HardwareManager.Axes.MoveIncremental(new XYZTopZBottomMove(50.0, double.NaN, double.NaN, double.NaN), AxisSpeed.Normal);
            HardwareManager.Axes.MoveIncremental(new XYZTopZBottomMove(double.NaN, 50.0, double.NaN, double.NaN), AxisSpeed.Normal);

            // LinearMotion
            Console.WriteLine("Test_LinearMotion - Starting");
            var position1 = new XYZTopZBottomPosition(new MotorReferential(), 10.0, 115.0, 12.0, 0.0);
            HardwareManager.Axes.LinearMotion(position1, AxisSpeed.Slow);
            var position2 = new XYZTopZBottomPosition(new MotorReferential(), 100.0, 15.0, 8.0, 0.0);
            HardwareManager.Axes.LinearMotion(position2, AxisSpeed.Slow);
        }
    }
}
