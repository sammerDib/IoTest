using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
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
    public class AutofocusLiseTest : TestWithMockedHardware<AutofocusLiseTest>, ITestWithAxes, ITestWithCamera, ITestWithProbeLise
    {
        private static readonly double s_micrometersToGeometricRatio = (double) (LiseTestUtils.MicrometertoGeometricRatio);

        private static readonly Length s_toFocuGapTop = 1.Millimeters();
        private static readonly Length s_toFocuGapBottom = 3.Millimeters();
        private static readonly Length s_initialZTopPosition = 13.Millimeters();
        private static readonly Length s_initialZBottomPosition = -5.Millimeters();
        private static readonly Length s_focusAirGapTop = (987.0).Micrometers();
        private static readonly Length s_focusAirGapBottom = (856.5).Micrometers();
        private static readonly Length s_airGapAtInitialZTopPosition = s_focusAirGapTop + s_toFocuGapTop;
        private static readonly Length s_airGapAtInitialZBottomPosition = s_focusAirGapBottom + s_toFocuGapBottom;

        private static readonly Length s_zTopFocusPosition = s_initialZTopPosition - s_toFocuGapTop;
        private static readonly Length s_zBottomFocusPosition = s_initialZBottomPosition + s_toFocuGapBottom;
        private static readonly Length s_airGapAtTopFocusPosition = s_focusAirGapTop;
        private static readonly Length s_airGapAtBottomFocusPosition = s_focusAirGapBottom;

        private List<double> _fakeTopRawSignal;
        private List<double> _fakeBottomRawSignal;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }
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
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            // Setup axes mock
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(new XYZTopZBottomPosition(new StageReferential(), 0, 0, s_initialZTopPosition.Millimeters, s_initialZBottomPosition.Millimeters));

            // Setup probe Lise with fake signal
            var liseTopSignal = CreateLiseSignalWithOnlyAirGap(s_airGapAtInitialZTopPosition);
            liseTopSignal.ProbeID = LiseUpId;
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, liseTopSignal, this);
            _fakeTopRawSignal = liseTopSignal.RawValues;

            var liseBottomSignal = CreateLiseSignalWithOnlyAirGap(s_airGapAtInitialZBottomPosition);
            liseBottomSignal.ProbeID = LiseBottomId;
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, liseBottomSignal, this);
            _fakeBottomRawSignal = liseBottomSignal.RawValues;

            // Setup Lise calibration
            var autofocusUpParams = new AutofocusParameters()
            {
                ZFocusPosition = s_zTopFocusPosition,            
                Lise = new LiseAutofocusParameters()
                {
                    AirGap = s_airGapAtTopFocusPosition,
                    ZStartPosition = s_zTopFocusPosition,
                    MinGain = 1.4,
                    MaxGain = 1.5
                }
            };

            var autofocusBottomParams = new AutofocusParameters()
            {
                ZFocusPosition = s_zBottomFocusPosition,                
                Lise = new LiseAutofocusParameters()
                {
                    AirGap = s_airGapAtBottomFocusPosition,
                    ZStartPosition = s_zBottomFocusPosition,
                    MinGain = 1.4,
                    MaxGain = 1.5
                }
            };

            var image = new ImageParameters()
            {
                PixelSizeX = 1.Millimeters(),
                PixelSizeY = 1.Millimeters(),
                XOffset = 0.Millimeters(),
                YOffset = 0.Millimeters()
            };

            ObjectiveCalibration objectiveUpCalibration = new ObjectiveCalibration()
            {
                DeviceId = ObjectiveUpId,
                AutoFocus = autofocusUpParams,
                OpticalReferenceElevationFromStandardWafer = 623.Micrometers(),
                Image = image
            };
            ObjectiveCalibration objectiveBottomCalibration = new ObjectiveCalibration()
            {
                DeviceId = ObjectiveBottomId,
                AutoFocus = autofocusBottomParams,
                Image = image
            };
            var objectiveCalibrationData = new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>() { objectiveUpCalibration, objectiveBottomCalibration }
            };

            CalibManager.UpdateCalibration(objectiveCalibrationData);
        }

        [TestMethod]
        public void Autofocus_lise_flow_nominal_case()
        {
            // Given : Signal with air gap corresponding at 1.55 mm is provided by probe Lise
            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);
            
            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded
            Assert.AreEqual(FlowState.Success, result.Status.State);
        }

        [TestMethod]
        public void Autofocus_lise_flow_with_top_probe_find_right_focus_position()
        {
            // Given : Signal analyzed with air gap corresponding to the position z top = 1mm and a targeted air gap corresponding to the focus position z top = 12mm
            // start Ztop = 13 --> 1.0 de diff avec le airgap focus

            Length zFocusTarget = (12.00).Millimeters();
            var gapDiff = s_focusAirGapTop + (s_initialZTopPosition - zFocusTarget);
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, gapDiff, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);
         
            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded at expected position
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(zFocusTarget.Millimeters, result.ZPosition,  1e-3);
        }

        [TestMethod]
        public void Autofocus_lise_flow_with_bottom_probe_find_right_focus_position()
        {
            // Given : Signal analyzed with air gap corresponding to the position z bottom = -5mm and a targeted air gap corresponding to the focus position z bottom = -2mm
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeBottomRawSignal.ToArray(), referencePeakPosition, s_airGapAtInitialZBottomPosition, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var afLiseInput = new AFLiseInput(LiseBottomId);
            var afLise = new AFLiseFlow(afLiseInput);
         
            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded at expected position
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(s_zBottomFocusPosition.Millimeters, result.ZPosition, 1e-3);
        }

        [TestMethod]
        public void Autofocus_lise_needs_valid_signal()
        {
            // Given : Signal analyzed is invalid
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, s_airGapAtInitialZTopPosition, s_micrometersToGeometricRatio);
            signal.RawValues.Clear();
            signal.ReferencePeaks.Clear();
            signal.SelectedPeaks.Clear();
            signal.DiscardedPeaks.Clear();
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);


            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus failed
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_lise_needs_signal_with_a_reference_peak()
        {
            // Given : Signal analyzed doesn't have reference peak
            
            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), 0, s_airGapAtInitialZTopPosition, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);


            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus failed
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_lise_needs_signal_with_an_air_gap_peak()
        {
            // Given : Signal analyzed doesn't have peak to define airgap with reference peak
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, null, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);


            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus failed
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_lise_flow_fails_when_focus_position_is_not_into_expected_range()
        {
            // Given : z top focus position = 12mm and expected range [0,5]
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, s_airGapAtInitialZTopPosition, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            afLiseInput.ZPosScanRange = new ScanRange(0, 5);
            var afLise = new AFLiseFlow(afLiseInput);


            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus failed
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_lise_flow_fails_when_focus_position_is_not_into_expected_range_because_of_optical_reference_elevation()
        {
            // Given : z top focus position = 12.41mm
            // start Ztop = 13 --> 0.59 de diff avec le airgap focus

            Length zFocusTarget = (12.41).Millimeters();
            var gapDiff = s_focusAirGapTop + (s_initialZTopPosition - zFocusTarget);
            int referencePeakPosition = 1000;
            
            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, gapDiff, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);
            
            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus failed
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_lise_flow_with_top_probe_find_right_focus_position_because_of_optical_reference_elevation()
        {
            // Given : z top focus position = 10.77mm 
            // start Ztop = 13 --> 2.23 de diff avec le airgap focus
            // 

            Length zFocusTarget = (10.77).Millimeters();
            var gapDiff = s_focusAirGapTop + (s_initialZTopPosition - zFocusTarget);     
            int referencePeakPosition = 1000;

            var signal = CreateFakeLiseSignal(_fakeTopRawSignal.ToArray(), referencePeakPosition, gapDiff, s_micrometersToGeometricRatio);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var afLiseInput = new AFLiseInput(LiseUpId);
            var afLise = new AFLiseFlow(afLiseInput);


            double zMin = s_initialZTopPosition.Millimeters - afLise.Configuration.AutoFocusScanRange.Millimeters / 2;
            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.IsTrue(zMin > 10.77);
            Assert.AreEqual(10.77, result.ZPosition, 1e-2);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestAFLiseReport1";
            Directory.CreateDirectory(directoryPath);

            var afLiseInput = new AFLiseInput(LiseUpId);

            var flow = new AFLiseFlow(afLiseInput);
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
            string directoryPath = "TestAFLiseReport2";
            Directory.CreateDirectory(directoryPath);

            var afLiseInput = new AFLiseInput(LiseUpId);

            var flow = new AFLiseFlow(afLiseInput);
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
        public void Report_of_intermediate_signal_is_working()
        {
            // Given : Signal analyzed with air gap corresponding to the position z top = 15mm and a targeted air gap corresponding to the focus position z top = 12mm
            int nbValues = 30000;
            int saturationValue = 7;
            int peakAmplitude = 8;
            int referencePeakPosition = 1000;
            int firstPeakPosition = (int)(referencePeakPosition + s_airGapAtInitialZTopPosition.Micrometers);
            var signal = CreateLiseSignalFromPeakPositions(new List<int> { referencePeakPosition, firstPeakPosition }, (float)s_micrometersToGeometricRatio, nbValues, null, saturationValue, peakAmplitude);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            string directoryPath = "TestAFLiseReport3";
            Directory.CreateDirectory(directoryPath);

             double gain = 2;
            var afLiseInput = new AFLiseInput(LiseUpId, gain);
            var afLise = new AFLiseFlow(afLiseInput);
            afLise.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;

            afLise.ReportFolder = directoryPath;

            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded at expected position
            string filename = Path.Combine(afLise.ReportFolder, $"autofocusLISE_signal_at_gain_{gain}_valid.csv");
            Assert.IsTrue(File.Exists(filename));

            using (var reader = new StreamReader(filename))
            {
                string separator = CSVStringBuilder.GetCSVSeparator();
                var cSep = new char[] { separator[0] };

                var line = reader.ReadLine();
                var values = line.Split(cSep);

                Assert.AreEqual("Raw signal", values[0]);
                Assert.AreEqual("Threshold", values[1]);
                Assert.AreEqual("Reference peak", values[2]);
                Assert.AreEqual("Peak of interest", values[3]);

                List<string> rawValue = new List<string>();
                List<string> threeshold = new List<string>();
                List<string> referencePeaks = new List<string>();
                List<string> selectedPeaks = new List<string>();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    values = line.Split(cSep);

                    rawValue.Add(values[0]);
                    threeshold.Add(values[1]);
                    referencePeaks.Add(values[2]);
                    selectedPeaks.Add(values[3]);
                }

                Assert.AreEqual(nbValues, rawValue.Count);
                Assert.AreEqual(nbValues, threeshold.Count);
                Assert.AreEqual(nbValues, referencePeaks.Count);
                Assert.AreEqual(nbValues, selectedPeaks.Count);

                Assert.AreEqual(peakAmplitude, Convert.ToDouble(rawValue[referencePeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(peakAmplitude, Convert.ToDouble(rawValue[firstPeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(peakAmplitude, Convert.ToDouble(referencePeaks[referencePeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(peakAmplitude, Convert.ToDouble(selectedPeaks[firstPeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
            }

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Report_of_signal_at_focus_is_working()
        {
            // Given : Signal analyzed with air gap corresponding to the position z top = 15mm and a targeted air gap corresponding to the focus position z top = 12mm
            int nbValues = 30000;
            int saturationValue = 7;
            int peakAmplitude = 8;
            int referencePeakPosition = 1000;
            int firstPeakPosition = (int)(referencePeakPosition + s_airGapAtInitialZTopPosition.Micrometers * s_micrometersToGeometricRatio);
            int focusPeakPosition = (int)(referencePeakPosition + s_airGapAtTopFocusPosition.Micrometers * s_micrometersToGeometricRatio);
            var signal = CreateLiseSignalFromPeakPositions(new List<int> { referencePeakPosition, firstPeakPosition }, (float)(1.0/s_micrometersToGeometricRatio), nbValues, null, saturationValue, peakAmplitude);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            string directoryPath = "TestAFLiseReport4";
            Directory.CreateDirectory(directoryPath);

            double gain = 2;
            var afLiseInput = new AFLiseInput(LiseUpId, gain);
            var afLise = new AFLiseFlow(afLiseInput);
            afLise.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;

            afLise.ReportFolder = directoryPath;

            // When : Try to run autofocus
            var result = afLise.Execute();

            // Then : Autofocus succeeded at expected position
             string prefix = $"autofocusLISE_signal_at_focus_position_";
             var reportfiles = Directory.GetFiles(afLise.ReportFolder)
                                    .Where(f => Path.GetFileName(f).StartsWith(prefix) &&
                                                Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                                    .ToList();

            Assert.IsTrue(reportfiles.Any());
            string filename = reportfiles.First();

            using (var reader = new StreamReader(filename))
            {
                string separator = CSVStringBuilder.GetCSVSeparator();
                var cSep = new char[] { separator[0] };

                var line = reader.ReadLine();
                var values = line.Split(cSep);

                Assert.AreEqual("Raw signal", values[0]);
                Assert.AreEqual("Focus position", values[1]);

                List<string> rawSignal = new List<string>();
                List<string> focusPosition = new List<string>();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    values = line.Split(cSep);

                    rawSignal.Add(values[0]);
                    focusPosition.Add(values[1]);
                }

                Assert.AreEqual(nbValues, rawSignal.Count);
                Assert.AreEqual(nbValues, focusPosition.Count);

                Assert.AreEqual(peakAmplitude, Convert.ToDouble(rawSignal[referencePeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(peakAmplitude, Convert.ToDouble(rawSignal[firstPeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(saturationValue, Convert.ToDouble(focusPosition[focusPeakPosition], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
            }

            Directory.Delete(directoryPath, true);
        }
    }
}
