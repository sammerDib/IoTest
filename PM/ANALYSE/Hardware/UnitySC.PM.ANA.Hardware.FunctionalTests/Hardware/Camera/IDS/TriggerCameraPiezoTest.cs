using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class TriggerCameraPiezoTest : FunctionalTest
    {
        public override void Run()
        {
            Logger.Debug("******************* START TriggerCameraPiezoTest *******************");

            // Setup & Parameters
            const string cameraTopID = "1";
            const string piezoAxisID = "ZPiezo-2";
            const string controllerID = "PI-E709-2";

            var cameraTop = (UI324xCpNir)HardwareManager.Cameras[cameraTopID];
            var piezoController = (PIE709Controller)HardwareManager.Controllers[controllerID];

            var initialPiezoPosition = 5.Micrometers();
            var moveStep = 0.1.Micrometers();
            Logger.Debug($"initialPiezoPosition = {initialPiezoPosition} | moveStep = {moveStep}");

            // Start

            // move to initial position
            piezoController.MoveTo(initialPiezoPosition);
            piezoController.WaitMotionEnd(1000);

            var piezoPosition = GetPiezoPosition(HardwareManager, piezoAxisID);
            Logger.Debug($"Initial piezo position (expected | real) = {initialPiezoPosition} | {piezoPosition}");
            if (!piezoPosition.Micrometers.Near(initialPiezoPosition.Micrometers, 0.1)) throw new Exception($"Expected initial position ({initialPiezoPosition}) is too far from real position ({piezoPosition})");

            // configure the piezo so that one electrical pluse commands the piezo to move 'moveStep' µm forward
            piezoController.ConfigureTriggerIn(initialPiezoPosition, moveStep);

            PrintPiezoControllerTriggerConfiguration(piezoController); // Comment this line if you don't want the controller trigger IN Config to be logged

            // send a trigger signal with the camera
            var start = DateTime.Now;

            const int stepCount = 10;
            var finalPosition = (initialPiezoPosition.Micrometers + (moveStep.Micrometers * stepCount)).Micrometers();
            if (finalPosition > piezoController.GetPositionMax()) throw new Exception("Maximum range exceeded");

            PrintPiezoTriggerInStatus(); // Comment this line if you don't want trigger IN status to be logged

            for (int step = 1; step <= stepCount; step++)
            {
                Logger.Debug($">> Step number {step}");
                cameraTop.TriggerOutEmitSignal();
                piezoController.WaitMotionEnd(1000);
            }

            Logger.Debug($"Motion duration {(DateTime.Now - start).TotalMilliseconds} ms");

            // actualize piezo position
            piezoPosition = GetPiezoPosition(HardwareManager, piezoAxisID);

            Logger.Debug($"Final piezo position (expected | real) = {finalPosition} | {piezoPosition}");
            if (!piezoPosition.Micrometers.Near(finalPosition.Micrometers, 0.1)) throw new Exception($"Expected final position ({finalPosition}) is too far from real position ({piezoPosition})");

            Logger.Debug("******************* END TriggerCameraPiezoTest *******************");
        }

        private Length GetPiezoPosition(AnaHardwareManager anaHardwareManager, string piezoAxisID)
        {
            var axes = anaHardwareManager.Axes;
            var axesPosition = axes.GetPos();

            if (axesPosition is AnaPosition anaPosition)
            {
                return (anaPosition.GetPiezoPosition(piezoAxisID).Position).Micrometers();
            }
            else
            {
                throw new Exception("Position is not an AnaPosition");
            }
        }

        private void PrintPiezoTriggerInStatus()
        {
            const string controllerID = "PI-E709-2";
            var piezoController = (PIE709Controller)HardwareManager.Controllers[controllerID];

            const int logIntervall = 100; // milliseconds
            const int logDuration = 20; // seconds

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            piezoController.StartLogTriggerInSignal(logIntervall);
            SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds > (logDuration * 1000));
            piezoController.StopLogTriggerInSignal();
        }

        private void PrintPiezoControllerTriggerConfiguration(PIE709Controller piezoController)
        {
            Logger.Debug($@"{new StringBuilder()
                .AppendLine("######## piezo controller internal configuration")
                .AppendLine($"  GetServoMode = {piezoController.GetServoMode()}")
                .AppendLine($"  GetTriggerInType = {piezoController.GetTriggerInType()}")
                .AppendLine($"  GetTriggerInPolarity = {piezoController.GetTriggerInPolarity()}")
                .AppendLine($"  GetTriggerInUsage = {piezoController.GetTriggerInUsage()}")
                .AppendLine($"  GetTriggerInState = {piezoController.GetTriggerInState()}")
                .AppendLine($"  GetWaveGeneratorStartingMode = {piezoController.GetWaveGeneratorStartingMode()}")
                .AppendLine($"  GetGeneratorWaveTableId = {piezoController.GetGeneratorWaveTableId()}")
                .AppendLine($"  IsGeneratorRunning = {piezoController.IsGeneratorRunning()}")
                .AppendLine($"  GetCommandMode = {piezoController.GetCommandMode()}")
                .AppendLine($"  GetWaveGeneratorOffset = {piezoController.GetWaveGeneratorOffset()}")
                .AppendLine($"  GetCyclesCount = {piezoController.GetCyclesCount()}")
                .AppendLine($"  GetTableRate = {piezoController.GetTableRate()}")
                .Append("########")}");
        }
    }
}
