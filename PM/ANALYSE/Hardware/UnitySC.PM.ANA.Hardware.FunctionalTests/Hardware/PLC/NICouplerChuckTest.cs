using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware
{
    public class NICouplerChuckTest : FunctionalTest
    {
        public override void Run()
        {
            Logger.Information("\n******************\n[NICouplerChuckTest] START\n******************\n");
            /*
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic
            {
                Diameter = 200.Millimeters()
            };
            */
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic
            {
                Diameter = 300.Millimeters()
            };
            /*
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic
            {
                Diameter = 300.Millimeters()
                IsFilmFrame = true,
            };
            */

            Logger.Information($"Start test for wafer {wafer.Diameter} mm");
            ClampWafer(wafer);
            Logger.Information($"Stop test for wafer {wafer.Diameter} mm");

            Logger.Information("[NICouplerChuckTest] END !");
        }

        public void ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            Logger.Information("[Clamp wafer test] Start !");
            HardwareManager.ClampHandler.ClampWafer(wafer.Diameter);

            var state = HardwareManager.Chuck.GetState();
            if (!state.WaferClampStates[wafer.Diameter])
            {
                throw new System.Exception("Test clamp Wafer: wafer is not clamped");
            }

            HardwareManager.ClampHandler.ReleaseWafer(wafer.Diameter);
            var stateAfterRelease = HardwareManager.Chuck.GetState();
            if (stateAfterRelease.WaferClampStates[wafer.Diameter])
            {
                throw new System.Exception("Test clamp Wafer: wafer is not release");
            }
            Logger.Information("[Clamp wafer test] Stop with succes !");
        }
    }
}
