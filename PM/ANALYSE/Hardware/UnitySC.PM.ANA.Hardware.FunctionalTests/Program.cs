using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware.FunctionalTests.Flows.AFCameraV2;
using UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes;
using UnitySC.PM.ANA.Service.Host;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests
{
    public static class Program
    {
        public static void Main()
        {
            var toolId = "4MET2229";
            //Bootstrapper.Register(new string[] { "-c", "4MET2229", "-sh", "-sf" }); // To run in simulated mode
            Bootstrapper.Register(new string[] { "-c", toolId }); // To run on the tool
            InitHardwareManager(true); // use false to not initialize the HardwareManager

            var tests = new List<IFunctionalTest>()
            {
                // Add your test(s) below
                //new ACSTest(),
                //new SpectrometerTest(),²
                //new PSIFlowTest(),
                //new LandingTest(),
                new AFV2CameraTest(toolId),
            };

            // Run tests
            tests.ForEach(test => test.Run());
        }

        private static void InitHardwareManager(bool initialize)
        {
            if (initialize)
            {
                var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
                hardwareManager.Init();
            }
        }
    }
}
