using System;
using System.Diagnostics;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes.Piezo
{
    public class PiezoMotionByTriggerInSignalTest : FunctionalTest
    {
        private readonly PIE709Controller _piezoController;
        private static readonly Stopwatch s_stopwatch = new Stopwatch();

        public PiezoMotionByTriggerInSignalTest()
        {
            const string piezoControllerID = "PI-E709-1";
            _piezoController = (PIE709Controller)HardwareManager.Axes.AxesControllers.First(controller => controller.DeviceID == piezoControllerID);
        }

        public override void Run()
        {
            // Params
            var initialPosition = 100.Micrometers();
            var motionStep = 10.Nanometers();

            // Execute
            _piezoController.MoveTo(initialPosition);
            Console.WriteLine($"*** Initial position = {_piezoController.GetCurrentPosition()}");
            _piezoController.ConfigureTriggerIn(initialPosition, motionStep);
            Console.WriteLine($"*** Initial position = {_piezoController.GetCurrentPosition()}");

            s_stopwatch.Restart();
            while (true)
            {
                long realEnlapsedTime = s_stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"{realEnlapsedTime} {(int)_piezoController.GetTriggerInState()} {_piezoController.GetCurrentPosition().Micrometers}");
                //Thread.Sleep(1000); // 1 second
                //Console.WriteLine($"*** {iter++} - trigger IN state = {controller.TriggerInState}");
                //Thread.Sleep(50);
                if (realEnlapsedTime > 1000)
                {
                    break;
                }
            }
        }
    }
}
