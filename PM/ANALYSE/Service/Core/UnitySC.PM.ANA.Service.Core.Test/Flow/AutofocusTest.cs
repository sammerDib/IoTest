using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;
using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class AutofocusTest : TestWithMockedHardware<AutofocusTest>, ITestWithAxes, ITestWithCamera, ITestWithProbeLise
    {
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

        #endregion Interfaces properties

        private Mock<IContextManager> ContextManager { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        protected override void SpecializeRegister()
        {
            ContextManager = new Mock<IContextManager>();
            ClassLocator.Default.Register(() => ContextManager.Object, true);
        }

        [TestMethod]
        public void Autofocus_flow_with_lise_nominal_case()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return the reference unsaturated peak and the target unsaturated peak
                var signalWithRefPeakAndOneInterestPeak = CreateLiseSignalFromPeakPositions(new List<int>() { RefPeakArbitraryPosition, FirstPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength);
                signalWithRefPeakAndOneInterestPeak.ProbeID = probeId;
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signalWithRefPeakAndOneInterestPeak, this);

                var afLiseContext = new TopObjectiveContext(ObjectiveUpId);

                var afLiseSettings = new AutoFocusSettings()
                {
                    Type = AutoFocusType.Lise,
                    ProbeId = probeId,
                    LiseGain = 1.45,
                    LiseScanRange = new ScanRange(-50, 50),
                    LiseAutoFocusContext = afLiseContext
                };
                var afInput = new AutofocusInput(afLiseSettings);

                // When : Try to autofocus

                var autoFocus = new AutofocusFlow(afInput);
                var result = autoFocus.Execute();

                // Then : Autofocus succeeded
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
            // Then context applier called : one time for each Autofocus flow (afLiseFlow)
            var nbContextApplierCall = SimpleProbesLise.Count;
            ContextManager.Verify(_ => _.Apply(It.Is<ANAContextBase>(c => (c != null))), Times.Exactly(nbContextApplierCall));
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestAutofocusReport";
            Directory.CreateDirectory(directoryPath);

            var afCameraContext = new TopImageAcquisitionContext()
            {
                TopObjectiveContext = new TopObjectiveContext(ObjectiveUpId)
            };
            var afLiseContext = new TopObjectiveContext(ObjectiveUpId);

            var afSettings = new AutoFocusSettings()
            {
                Type = AutoFocusType.LiseAndCamera,
                CameraId = CameraUpId,
                CameraScanRange = ScanRangeType.Configured,
                CameraScanRangeConfigured = new ScanRangeWithStep(11, 15, 1),
                LiseGain = 1.45,
                ProbeId = LiseUpId,
                LiseScanRange = new ScanRange(-50, 50),
                LiseAutoFocusContext = afLiseContext,
                ImageAutoFocusContext = afCameraContext
            };
            var afInput = new AutofocusInput(afSettings);
            var flow = new AutofocusFlow(afInput);
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
            string directoryPath = "TestAutofocusReport";
            Directory.CreateDirectory(directoryPath);

            var afCameraContext = new TopImageAcquisitionContext()
            {
                TopObjectiveContext = new TopObjectiveContext(ObjectiveUpId)
            };
            var afLiseContext = new TopObjectiveContext(ObjectiveUpId);

            var afSettings = new AutoFocusSettings()
            {
                Type = AutoFocusType.LiseAndCamera,
                CameraId = CameraUpId,
                CameraScanRange = ScanRangeType.Configured,
                CameraScanRangeConfigured = new ScanRangeWithStep(11, 15, 1),
                LiseGain = 1.45,
                ProbeId = LiseUpId,
                LiseScanRange = new ScanRange(-50, 50),
                LiseAutoFocusContext = afLiseContext,
                ImageAutoFocusContext = afCameraContext
            };
            var afInput = new AutofocusInput(afSettings);
            var flow = new AutofocusFlow(afInput);
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
    }
}
