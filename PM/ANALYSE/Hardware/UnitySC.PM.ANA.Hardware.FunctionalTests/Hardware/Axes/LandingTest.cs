using System.Linq;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Controllers;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes
{
    public class LandingTest : FunctionalTest
    {
        private const int NbExecution = 100;
        private readonly string _cameraId = "1";

        public override void Run()
        {
            // Start and stop continuous grab before execute AF camera only to avoid multiple logs
            var camera = HardwareManager.Cameras.First(_ => _.Key == _cameraId).Value;
            try
            {
                camera.StartContinuousGrab();
                for (int i = 0; i < NbExecution; i++)
                {
                    Logger.Information("Test multiple landing  : " + i);
                    HardwareManager.Axes.Land();
                    Thread.Sleep(1_000);

                    var pos = HardwareManager.Axes.GetPos();
                    Logger.Information(pos.ToString());
                    Thread.Sleep(1_000);

                    HardwareManager.Axes.StopLanding();
                    foreach (var controller in HardwareManager.Controllers)
                    {
                        if (controller.Value is ACSController aCSController)
                        {
                            var pressures = aCSController.GetAirBearingPressuresValues();
                            foreach (var pressure in pressures)
                            {
                                Logger.Information($"{pressure.Key} value : {pressure.Value}");
                            }
                        }
                    }
                }
            }
            finally
            {
                camera.StopContinuousGrab();
                Logger.Information("Test multiple landing Done");
            }
        }
    }
}
