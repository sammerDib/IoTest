using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.TSV;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class TSVDepthTest : TestWithMockedHardware<TSVDepthTest>, ITestWithCamera, ITestWithProbeLise, ITestWithAxes
    {
        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public Mock<IAxes> SimulatedAxes { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
        }

        [TestMethod]
        public void TSV_depth_flow_nominal_case()
        {
            // Given
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, AirRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var input = new TSVDepthInput(
                approximateDepth: 750.Micrometers(),
                approximateWidth: 5.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId= LiseUpId }
            );

            var flow = new TSVDepthFlow(input);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(750, result.Depth.Micrometers, 5);
        }

        [TestMethod]
        public void TSV_depth_flow_needs_valid_signal()
        {
            // Given
            var signal = CreateFakeLiseSignalCorrespondingToOneTSVOf750MicrometersDepth();
            signal.RawValues.Clear();
            signal.ReferencePeaks.Clear();
            signal.SelectedPeaks.Clear();
            signal.DiscardedPeaks.Clear();
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var input = new TSVDepthInput(
                approximateDepth: 2.Micrometers(),
                approximateWidth: 1.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = "LiseUpId" }
            );

            var flow = new TSVDepthFlow(input);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void TSV_depth_flow_needs_two_peaks_to_compute_depth()
        {
            // Given
            var signal = CreateFakeLiseSignalCorrespondingToOneTSVOf750MicrometersDepth();
            signal.SelectedPeaks.RemoveAt(1);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var input = new TSVDepthInput(
                approximateDepth: 2.Micrometers(),
                approximateWidth: 1.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = "LiseUpId" }
            );

            var flow = new TSVDepthFlow(input);

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Positions_along_archimedean_spiral_are_correct()
        {
            // Given
            XYPosition initialPosition = new XYPosition(new StageReferential(), 250, 250);
            double spiralRingsNumber = 1.75;
            double distBetweenSpiralRings = 5;

            // When
            var points = TSVDepthFlow.CalculatePositionsAlongArchimedeanSpiral(initialPosition, 0.5.Micrometers(), spiralRingsNumber, distBetweenSpiralRings);

            // Then
            Assert.AreEqual(initialPosition.X, points[0].X);
            Assert.AreEqual(initialPosition.Y, points[0].Y);
            Assert.AreEqual(initialPosition.Referential, points[0].Referential);

            double expectedFinalPositionX = initialPosition.X + spiralRingsNumber * distBetweenSpiralRings;
            double expectedFinalPositionY = initialPosition.Y;
            Assert.AreEqual(expectedFinalPositionX, points[points.Count - 1].X, 1.0);
            Assert.AreEqual(expectedFinalPositionY, points[points.Count - 1].Y, 1.0);
            Assert.AreEqual(initialPosition.Referential, points[points.Count - 1].Referential);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestTSVDepthReport";
            Directory.CreateDirectory(directoryPath);

            var input = new TSVDepthInput(
                approximateDepth: 750.Micrometers(),
                approximateWidth: 5.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = "LiseUpId" }
            );

            var flow = new TSVDepthFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"input.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_result_is_working()
        {
            //Given
            string directoryPath = "TestTSVDepthReport";
            Directory.CreateDirectory(directoryPath);

            var input = new TSVDepthInput(
                approximateDepth: 750.Micrometers(),
                approximateWidth: 5.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = "LiseUpId" }
            );

            var flow = new TSVDepthFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var status = (flow.Result == null) ? "null" : ((flow.Result.Status == null) ? "ukn" : flow.Result.Status.State.ToString());
            var filename = Path.Combine(flow.ReportFolder, $"result_{status}.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_signals_is_working()
        {
            // Given
            int nbValues = 30000;
            Length micrometersToGeometricRatio = 1.Millimeters();
            int referencePeakPosition = 1000;
            int firstPeakPosition = 15000;
            int secondPeakPosition = firstPeakPosition + 750;
            var signal = CreateLiseSignalFromPeakPositions(new List<int> { referencePeakPosition, firstPeakPosition, secondPeakPosition }, (float)micrometersToGeometricRatio.Millimeters, nbValues);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var input = new TSVDepthInput(
                approximateDepth: 750.Micrometers(),
                approximateWidth: 5.Micrometers(),
                depthTolerance: 1.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = LiseUpId }
            );

            string directoryPath = "TestTSVDepthReport";
            Directory.CreateDirectory(directoryPath);

            var flow = new TSVDepthFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            // When
            flow.Execute();

            // Then
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            FileInfo[] files = directory.GetFiles();

            var reportSignalFiles = 0;
            string separator = CSVStringBuilder.GetCSVSeparator();
            var cSep = new char[] { separator[0] };
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains("liseRawSignal_at_position"))
                {
                    reportSignalFiles++;

                    using (var reader = new StreamReader(Path.Combine(flow.ReportFolder, file.Name)))
                    {
                        List<string> index = new List<string>();
                        List<string> rawSignal = new List<string>();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(cSep);

                            index.Add(values[0]);
                            rawSignal.Add(values[1]);
                        }

                        Assert.AreEqual(nbValues, rawSignal.Count);
                        Assert.AreEqual(nbValues, index.Count);
                    }
                }
            }

            Assert.IsTrue(reportSignalFiles > 0);

            Directory.Delete(directoryPath, true);
        }
    }
}
