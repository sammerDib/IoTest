using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class StableFramerateTest : FunctionalTest
    {
        private readonly IDSCameraBase _cameraTop;

        public StableFramerateTest()
        {
            HardwareManager.Cameras.TryGetValue("1", out var camera);
            if (!(camera is IDSCameraBase))
            {
                throw new Exception($"Camera type is not valid. Expected 'IDSCameraBase' but get '{camera.GetType()}'.");
            }

            _cameraTop = (IDSCameraBase)camera;
        }

        public override void Run()
        {
            Logger.Information("\n******************\n[StableFramerateTest] START\n******************\n");
            Logger.Information($"{_cameraTop.GetSettings()}");

            // Start aquisition
            Logger.Information("Start images acquisition");
            _cameraTop.StartContinuousGrab();

            var records = new Dictionary<double, List<double>> // <targetedTramerate, realFramerateRecords>
            {
                [10] = new List<double>(),
                [16] = new List<double>(),
                [22] = new List<double>(),
                [26] = new List<double>(),
                [32] = new List<double>(),
                [36] = new List<double>(),
                [42] = new List<double>(),
                [46] = new List<double>(),
                [52] = new List<double>(),
            };

            foreach (double framerate in records.Keys)
            {
                _cameraTop.SetFrameRate(framerate);
                Logger.Information($"Start acquisition for framerate {framerate} fps. Wait 2s...");
                Thread.Sleep(2_000);

                const int valuesCount = 30;
                for (int i = 1; i <= valuesCount; i++)
                {
                    double realFramerate = _cameraTop.GetRealFramerate();
                    Logger.Information($"\t{i:00} record(s) => expected = {framerate} fps | real = {realFramerate} fps.");
                    records[framerate].Add(realFramerate);
                    Thread.Sleep(1_000);
                }
            }

            // Assertions on stability
            foreach (double framerate in records.Keys)
            {
                double mean = records[framerate].Average();
                double sd = records[framerate].StandardDeviation();
                double min = records[framerate].Min();
                double max = records[framerate].Max();

                bool meanOK = mean.Near(framerate, 2);
                bool sdOK = sd.Near(0, 2);

                if (meanOK && sdOK)
                {
                    Logger.Information($"Framerate {framerate} fps => stable: average value = {mean}, standard deviation = {sd}, min = {min}, max = {max}");
                }
                else
                {
                    Logger.Error($"Framerate {framerate} fps => not stable: average value = {mean} ({(meanOK ? "OK" : "KO")}), standard deviation = {sd} ({(sdOK ? "OK" : "KO")})");
                }
            }

            Logger.Information("[StableFramerateTest] END !");
        }
    }
}
