using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Console.Test
{
    public class LiseAlgosTest
    {
        private const double REF750_xPos = -209.2;
        private const double REF750_yPos = 102.7758;

        public static void Main(string[] args)
        {
            Bootstrapper.Register();

            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            hardwareManager.Init();

            LISEDownSignalFunctionOfZBottomAnalysis();
            // LISEDownSignalFunctionOfGainAnalysis();
            // HardwareTest();
            // LISETopSignalFunctionOfZTopAnalysis();
            // LISETopSignalFunctionOfGainAnalysis();
            // LiseCalibration();
            // AutofocusLise();
            //LiseSimpleDoMeasureOnRef750UM(nbMeasuresWanted: 10);
            //LiseDoubleCalibrationOnRef750UM();
            //LiseDoubleDoMeasureOnRef750UM();
        }

        protected static void HardwareTest()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var chuckConfig = hardwareManager?.Chuck?.Configuration;
            var slotConfig = chuckConfig.GetSubstrateSlotConfigs().Find(config => config.Diameter == 300.Millimeters());

            var liseUp = hardwareManager.Probes["ProbeLiseUp"] as IProbeLise;
            var parameter = CreateProbeInputParametersLise();
            liseUp.StartContinuousAcquisition(parameter);
            var axes = hardwareManager.Axes;
            axes.WaitMotionEnd(20000);
            axes.GotoHomePos(AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);
            axes.GotoPosition(slotConfig.PositionPark, AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);
            ProbeLiseSignal probeRawSignal = null;
            Thread.Sleep(10000);
            probeRawSignal = liseUp.LastRawSignal as ProbeLiseSignal;
            liseUp.StopContinuousAcquisition();
        }

        protected static void LISETopSignalFunctionOfZTopAnalysis()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            var axes = hardwareManager.Axes;
            ProbeLiseSignal probeRawSignal = null;
            int peakOfInterestNb = 1;

            // Peak position as a function of Z-top position
            List<Point> functionZTop = new List<Point>();
            for (double ZTopPos = 15.0; ZTopPos >= 10.0; ZTopPos -= 0.5)
            {
                axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, ZTopPos, -15), AxisSpeed.Slow);
                axes.WaitMotionEnd(20000);

                var liseUp = hardwareManager.Probes["ProbeLiseUp"] as IProbeLise;
                var parameter = CreateProbeInputParametersLise(1.8, 16);
                probeRawSignal = liseUp.DoSingleAcquisition(parameter) as ProbeLiseSignal;
                List<ProbePoint> pts = probeRawSignal.SelectedPeaks;
                if (pts.Any())
                {
                    ProbePoint peakOfInterest = pts.ElementAt(peakOfInterestNb);
                    functionZTop.Add(new Point { X = ZTopPos, Y = peakOfInterest.X });
                }
                // Raw signal
                WriteCSV(probeRawSignal.RawValues, "RawSignal_ZTop" + ZTopPos + ".csv");
            }

            WriteCSV(functionZTop, "PeakPositionFunctionOfZTop.csv");
        }

        protected static void LISEDownSignalFunctionOfZBottomAnalysis()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            var axes = hardwareManager.Axes;
            ProbeLiseSignal probeRawSignal = null;
            int peakOfInterestNb = 1;

            // Peak position as a function of Z-top position
            List<Point> functionZTop = new List<Point>();
            for (double ZBottomPos = 1; ZBottomPos >= -10.0; ZBottomPos -= 0.5)
            {
                axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, 12, ZBottomPos), AxisSpeed.Slow);
                axes.WaitMotionEnd(20000);

                var liseDown = hardwareManager.Probes["ProbeLiseDown"] as IProbeLise;
                var parameter = CreateProbeInputParametersLise(1.8, 16);
                probeRawSignal = liseDown.DoSingleAcquisition(parameter) as ProbeLiseSignal;
                List<ProbePoint> pts = probeRawSignal.SelectedPeaks;
                if (pts.Any())
                {
                    ProbePoint peakOfInterest = pts.ElementAt(peakOfInterestNb);
                    functionZTop.Add(new Point { X = ZBottomPos, Y = peakOfInterest.X });
                }
                // Raw signal
                WriteCSV(probeRawSignal.RawValues, "RawSignal_ZBottom" + ZBottomPos + ".csv");
            }

            WriteCSV(functionZTop, "PeakPositionFunctionOfZBottom.csv");
        }

        protected static void LISETopSignalFunctionOfGainAnalysis()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            var axes = hardwareManager.Axes;
            axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, 15, -15), AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);

            int peakOfInterestNb = 1;
            ProbeLiseSignal probeRawSignal = null;

            // Peak amplitude as a function of gain
            List<Point> functionGain = new List<Point>();
            for (double currentGain = 0.0; currentGain <= 3.0; currentGain += 0.1)
            {
                var liseUp = hardwareManager.Probes["ProbeLiseUp"] as IProbeLise;
                var parameter = CreateProbeInputParametersLise(currentGain, 16);
                probeRawSignal = liseUp.DoSingleAcquisition(parameter) as ProbeLiseSignal;
                List<ProbePoint> pts = probeRawSignal.SelectedPeaks;
                if (pts.Any())
                {
                    ProbePoint peakOfInterest = pts.ElementAt(peakOfInterestNb);
                    functionGain.Add(new Point { X = currentGain, Y = peakOfInterest.Y });
                }
                // Raw signal
                WriteCSV(probeRawSignal.RawValues, "LiseTopRawSignal_Gain" + currentGain + ".csv");
            }

            WriteCSV(functionGain, "LiseTopPeakAmplitudeFunctionOfGain.csv");
        }

        protected static void LISEDownSignalFunctionOfGainAnalysis()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            var axes = hardwareManager.Axes;
            axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, 15, -0.2), AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);

            int peakOfInterestNb = 1;
            ProbeLiseSignal probeRawSignal = null;

            // Peak amplitude as a function of gain
            List<Point> functionGain = new List<Point>();
            for (double currentGain = 0.0; currentGain <= 3.0; currentGain += 0.1)
            {
                var liseDown = hardwareManager.Probes["ProbeLiseDown"] as IProbeLise;
                var parameter = CreateProbeInputParametersLise(currentGain, 16);
                probeRawSignal = liseDown.DoSingleAcquisition(parameter) as ProbeLiseSignal;
                List<ProbePoint> pts = probeRawSignal.SelectedPeaks;
                if (pts.Any())
                {
                    ProbePoint peakOfInterest = pts.ElementAt(peakOfInterestNb);
                    functionGain.Add(new Point { X = currentGain, Y = peakOfInterest.Y });
                }
                // Raw signal
                WriteCSV(probeRawSignal.RawValues, "LiseDownRawSignal_Gain" + currentGain + ".csv");
            }

            WriteCSV(functionGain, "LiseDownPeakAmplitudeFunctionOfGain.csv");
        }

        protected static void LiseCalibration()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var liseCalib = new LiseCalibration();

            ProbeSample sample = CreateProbeSample();

            var axes = hardwareManager.Axes;
            axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, 15, -15), AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);

            LiseAutofocusCalibration calibration = new LiseAutofocusCalibration { MinGain = 0, MaxGain = 5.5, ZPosition = -1 };
            bool preCalibrationSucceeded =
                liseCalib.Calibration(sample, ref calibration, "ProbeLiseUp", 0, 15, AxisSpeed.Slow);
        }

        protected static void AutofocusLise()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var liseCalib = new LiseCalibration();

            ProbeSample sample = CreateProbeSample();

            var axes = hardwareManager.Axes;
            axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, double.NaN, double.NaN), AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);

            LiseAutofocusCalibration calibration = new LiseAutofocusCalibration { MinGain = 0, MaxGain = 5.5, ZPosition = -1 };
            bool preCalibrationSucceeded =
                liseCalib.Calibration(sample, ref calibration, "ProbeLiseUp", 0, 15, AxisSpeed.Slow);
            var afLiseInput = new AFLiseInput("ProbeLiseUp");
            var afLise = new AFLiseFlow(afLiseInput);
            var result = afLise.Execute();
        }

        private static void LiseDoubleCalibrationOnRef750UM()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var axes = hardwareManager.Axes;
            double zTop = 13.099;
            double zBottom = 0.2365;
            axes.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), REF750_xPos, REF750_yPos, zTop, zBottom), AxisSpeed.Slow);
            axes.WaitMotionEnd(20000);

            var inputCalib = new DualLiseCalibParams();
            inputCalib.ProbeCalibrationReference = new OpticalReferenceDefinition() { RefThickness = 750.46.Micrometers(), RefTolerance = 5.Micrometers(), RefRefrIndex = (float)1.4621 };
            inputCalib.TopLiseAirgapThreshold = 0.6;
            inputCalib.BottomLiseAirgapThreshold = 0.7;
            inputCalib.CalibrationMode = 3;
            inputCalib.NbRepeatCalib = 16;
            inputCalib.ZTopUsedForCalib = zTop;
            inputCalib.ZBottomUsedForCalib = zBottom;

            DualLiseInputParams inputParams = CreateProbeInputParametersLiseDouble();
            inputParams.ProbeSample.Layers[0].RefractionIndex = 0.0;
            LiseDoubleTuning calibration = new LiseDoubleTuning("ProbeLiseDouble");
            var result = calibration.Calibration(inputCalib, inputParams);
            Thread.Sleep(5000);
        }

        private static ProbeSample CreateProbeSample()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");
            return sample;
        }

        private static SingleLiseInputParams CreateProbeInputParametersLise()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter

            double gain = 1.8;
            int nbMeasures = 16;

            var probeInputParams = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);

            return probeInputParams;
        }

        private static DualLiseInputParams CreateProbeInputParametersLiseDouble()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 0.0);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter

            double gain = 1.8;
            int nbMeasures = 16;
            var probeUpParams = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeDownParams = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);

            var probeInputParamsDouble = new DualLiseInputParams(sample, probeUpParams, probeDownParams);

            return probeInputParamsDouble;
        }

        private static SingleLiseInputParams CreateProbeInputParametersLise(double gain, int nbMeasureAverage)
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter

            var probeInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasureAverage);

            return probeInputParameters;
        }

        private struct Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        private static void WriteCSV(List<Point> points, string filePath)
        {
            var csv = new StringBuilder();

            foreach (Point p in points)
            {
                var newLine = string.Format("{0},{1}", p.X, p.Y);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(filePath, csv.ToString());
        }

        private static void WriteCSV(List<double> values, string filePath)
        {
            var csv = new StringBuilder();
            int i = 0;
            foreach (double p in values)
            {
                var newLine = string.Format("{0},{1}", i, p);
                csv.AppendLine(newLine);
                i++;
            }
            File.WriteAllText(filePath, csv.ToString());
        }

        private static void WriteResultInFile(List<LiseResult> results, string filePath)
        {
            int measureIndex = 1;
            if (results.Count <= 0)
            {
                System.IO.File.AppendAllText(filePath, "Results list is empty!\n");
                return;
            }
            foreach (var result in results)
            {
                if (result.LayersThickness.Count <= 0)
                {
                    System.IO.File.AppendAllText(filePath, "no layer detected\n");
                    continue;
                }
                foreach (var layer in result.LayersThickness)
                {
                    System.IO.File.AppendAllText(filePath, $"{layer.Thickness.Nanometers.ToString("#.##")}\n");
                    measureIndex++;
                }
            }
        }
    }
}
