using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes.Piezo
{
    public class PiezoSpeedTest : FunctionalTest
    {
        private readonly PIE709Controller _piezoController;
        private static readonly Stopwatch s_stopwatch = new Stopwatch();

        public PiezoSpeedTest()
        {
            const string piezoControllerID = "PI-E709-1";
            _piezoController = (PIE709Controller)HardwareManager.Axes.AxesControllers.First(controller => controller.DeviceID == piezoControllerID);
        }

        public override void Run()
        {
            Logger.Debug("**** Start piezo speed Test ****");

            var deltasPerSpeed = new Dictionary<int, List<double>>(); // key = speed, value = delta times (expected vs. real) in ms
            var speeds = new List<int>() { 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };

            const int sampleCount = 50;
            for (int sample = 1; sample <= sampleCount; sample++)
            {
                Logger.Debug($"Sample {sample} proceeding...");
                foreach (int speed in speeds)
                {
                    var initialPosition = 5.Micrometers();
                    var finalPosition = 55.Micrometers();
                    var motionDistance = finalPosition - initialPosition;
                    var positionPrecision = 1.Micrometers();
                    double durationPrecision_ms = 100;

                    _piezoController.SetSpeed(2000);
                    _piezoController.MoveTo(initialPosition); // initial position
                    _piezoController.WaitMotionEnd(60_000);

                    var piezoCurrentPosition = _piezoController.GetCurrentPosition();
                    if (!piezoCurrentPosition.Near(initialPosition, positionPrecision))
                    {
                        throw new Exception($"Piezo is not at its initial position {initialPosition}.");
                    }

                    s_stopwatch.Restart();
                    _piezoController.MoveTo(finalPosition, speed);
                    _piezoController.WaitMotionEnd(60_000);
                    s_stopwatch.Stop();

                    piezoCurrentPosition = _piezoController.GetCurrentPosition();
                    if (!piezoCurrentPosition.Near(finalPosition, positionPrecision))
                    {
                        throw new Exception($"Piezo is not at its final position {finalPosition}.");
                    }

                    double expectedDuration_ms = motionDistance.Micrometers * 1000 / speed;
                    long motionDuration_ms = s_stopwatch.ElapsedMilliseconds;
                    double delta_ms = Math.Abs(expectedDuration_ms - motionDuration_ms);

                    if (!delta_ms.Near(0, durationPrecision_ms))
                    {
                        throw new Exception($"Motion duration ({motionDuration_ms} ms) is too far from expected ({expectedDuration_ms} ms).");
                    }

                    Logger.Debug($"[{sample}|{speed}] Motion duration (expected|real) = ({expectedDuration_ms}|{motionDuration_ms}) ms ---> delta = {delta_ms} ms");

                    deltasPerSpeed[speed] = deltasPerSpeed.TryGetValue(speed, out var _) ? deltasPerSpeed[speed] : new List<double>();
                    deltasPerSpeed[speed].Add(delta_ms);
                }
            }

            Logger.Debug($"Average values based on {sampleCount} samples:");
            Logger.Debug($"Speed deltas_expected_vs_real(ms) std-dev");
            foreach (int speed in speeds)
            {
                double average = deltasPerSpeed[speed].Average();
                double stdDev = deltasPerSpeed[speed].StandardDeviation();
                Logger.Debug($"{speed} {average} {stdDev}");
            }

            Logger.Debug("**** Stop piezo speed Test ****");
        }
    }
}
