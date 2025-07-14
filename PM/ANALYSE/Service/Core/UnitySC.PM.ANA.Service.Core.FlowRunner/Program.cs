using System;
using System.IO;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Autolight;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.FlowRunner
{
    public class Program
    {
        private const string HELP = "use -c N to run the auto-alignment N times";

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-h")
            {
                Console.WriteLine(HELP);
                Environment.Exit(0);
            }

            const int DEFAULT_RUN_COUNT = 5;
            int runs = DEFAULT_RUN_COUNT;

            if (args.Length == 2 && args[0] == "-c")
            {
                runs = int.Parse(args[1]);
            }
            Console.WriteLine("I will perform " + runs + " runs");

            Service.Host.Bootstrapper.Register();

            Mil.Instance.Allocate();

            var manager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            manager.Init();

            var x5Config = manager.ObjectivesSelectors["ObjectiveSelector01"].Config.Objectives[0];
            manager.ObjectivesSelectors["ObjectiveSelector01"].SetObjective(x5Config);

            StreamWriter logfile;
            logfile = File.AppendText("autoalignment.log");
            logfile.WriteLine("------------------------------------------------");
            logfile.WriteLine("[" + DateTime.Now + "] Auto-align data collect");
            logfile.WriteLine("Shift X;Shift Y; rotation; bwa confidence; light power; z position; autofocus score");

            // Reset axes position to its zero
            var zero = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0);
            var axesService = ClassLocator.Default.GetInstance<IAxesService>();
            axesService.GotoPosition(zero, AxisSpeed.Normal);
            axesService.WaitMotionEnd(1000);

            // initialization of RAM buffer for camera image
            if (!manager.Cameras.TryGetValue("1", out var camera))
            {
                throw new System.Exception("Cannot get camera");
            }

            if (camera is UI324xCpNir UI324xCpNirCamera)
            {
                UI324xCpNirCamera.Init();
            }

            var cameraService = ClassLocator.Default.GetInstance<ICameraServiceEx>();
            string cameraId = manager.Cameras["1"].Config.DeviceID;
            ((CameraServiceEx)cameraService).Init();
            cameraService.StartAcquisition(cameraId);
            Thread.Sleep(2000);

            for (int runsDone = 0; runsDone < runs; runsDone++)
            {
                try
                {
                    var trace = DoOneAutoAlignment(manager);
                    logfile.WriteLine(trace);
                    logfile.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured: " + e.Message);
                }
            }
            logfile.Close();
            cameraService.StopAcquisition(cameraId);
            manager.Shutdown();
            Mil.Instance.Free();
        }

        private static string DoOneAutoAlignment(AnaHardwareManager manager)
        {
            // 1. Autofocus LISE
            var afLiseInput = new AFLiseInput("ProbeLiseUp");
            var afLiseFlow = new AFLiseFlow(afLiseInput);
            var autofocusResult = afLiseFlow.Execute();

            // 2. Light calibration
            string cameraId = manager.Cameras["1"].Config.DeviceID;
            string lightId = manager.Lights["VIS_WHITE_LED"].DeviceID;

            var autoLightInput = new AutolightInput(cameraId, lightId, 30, new ScanRangeWithStep(1, 100, .1));
            var autoLightFlow = new AutolightFlow(autoLightInput);
            var autoLightResult = autoLightFlow.Execute();

            // 3. BWA
            var WaferDimensionalCharacteristic = new WaferDimensionalCharacteristic();
            WaferDimensionalCharacteristic.Diameter = 300.Millimeters();
            WaferDimensionalCharacteristic.WaferShape = UnitySC.Shared.Data.Enum.WaferShape.Notch;

            var bareWaferAlignmentInput = new BareWaferAlignmentInput(WaferDimensionalCharacteristic, "1");
            var bareWaferAlignmentFlow = new BareWaferAlignmentFlow(bareWaferAlignmentInput);
            var bareWaferAlignmentResult = bareWaferAlignmentFlow.Execute();

            // re-center
            var zero = new XYZTopZBottomPosition(new StageReferential(), 0, 0, double.NaN, double.NaN);
            var axesService = ClassLocator.Default.GetInstance<IAxesService>();
            axesService.GotoPosition(zero, AxisSpeed.Normal);
            axesService.WaitMotionEnd(2000);

            string trace;
            if (bareWaferAlignmentResult is BareWaferAlignmentResult result)
            {
                trace = result.ShiftX + ";" + result.ShiftY + ";" + result.Angle + ";" + result.Confidence + ";"  + result.ShiftStageX + ";" + result.ShiftStageY + ";";
            }
            else
            {
                trace = ";;;;;;";
            }

            if (autoLightResult.Status.State == FlowState.Success)
            {
                trace += autoLightResult.LightPower + ";";
            }
            else
            {
                trace += ";";
            }
            if (autofocusResult.Status.State == FlowState.Success)
            {
                trace += autofocusResult.ZPosition + ";" + autofocusResult.QualityScore;
            }
            else
            {
                trace += ";";
            }
            return trace;
        }
    }
}
