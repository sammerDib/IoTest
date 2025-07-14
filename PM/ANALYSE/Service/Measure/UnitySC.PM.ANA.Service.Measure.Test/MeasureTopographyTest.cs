using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Topography;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureTopographyTest : TestWithMockedHardware<MeasureTopographyTest>, ITestWithAxes, ITestWithProbeLise, ITestWithCamera
    {
        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public Mock<LightBase> SimulatedLight { get; set; }
        public string LightId { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        #region Sequence properties

        protected int CurrentSetupSequenceId { get; set; }
        protected int CurrentCallSequenceId { get; set; }
        protected int NbExpectedLandAndStopland { get; set; }
        protected int NbExpectedAxesGotoPosition { get; set; }
        protected int NbExpectedContextApplications { get; set; }

        #endregion Sequence properties

        private Mock<IContextManager> ContextManager { get; set; }
        protected override bool FlowsAreSimulated => true;

        private readonly string _measureObjectiveId = TestConstants.Objective50XINT.DeviceID;
        private readonly string _measureAutofocusObjectiveId = TestConstants.Objective5XNIR.DeviceID;

        internal static class TestConstants
        {
            public static ObjectiveConfig Objective5XINT = new ObjectiveConfig() { ObjType = ObjectiveConfig.ObjectiveType.INT, DeviceID = "ID-5XINT01" };
            public static ObjectiveConfig Objective50XINT = new ObjectiveConfig() { ObjType = ObjectiveConfig.ObjectiveType.INT, DeviceID = "ID-50XINT01" };
            public static ObjectiveConfig Objective5XNIR = new ObjectiveConfig() { ObjType = ObjectiveConfig.ObjectiveType.NIR, DeviceID = "ID-5XNIR01" };

            public static List<ObjectiveConfig> ObjectivesConfigs = new List<ObjectiveConfig>()
            {
                Objective5XINT,
                Objective50XINT,
                Objective5XNIR,
            };

            public static Length ScanMargin = 1.Micrometers();
            public static Length HeightVariation = 2.Micrometers();
            public static Length StepSize = 50.Nanometers();
            public static int NbStep = 320;
            public static Length VSIMarginConstant = 5.Micrometers();
            public static string PiezoAxisID = "PiezoAxisID";
            public static XYZTopZBottomPosition MeasurePointPosition = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            public static string TopographyResultFolderPath = "TopographyResultFolderPath";
        }

        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory(TestConstants.TopographyResultFolderPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(TestConstants.TopographyResultFolderPath, true);
        }

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   Measures = new List<MeasureConfigurationBase>() {
                       new MeasureTopoConfiguration() {
                           VSIMarginConstant = TestConstants.VSIMarginConstant,
                           VSIStepSize = TestConstants.StepSize,
                       }
                   }
               });
            ContextManager = new Mock<IContextManager>(MockBehavior.Strict);
            ClassLocator.Default.Register(() => ContextManager.Object, true);
        }

        protected override void PostGenericSetup()
        {
            var topObjectiveSelector = HardwareManager.ObjectivesSelectors.First(_ => _.Value.Position == ModulePositions.Up);
            topObjectiveSelector.Value.Config.Objectives = TestConstants.ObjectivesConfigs;
            var measureObjCalib = CreateDummyObjectiveCalibration(_measureObjectiveId);
            var measureAFObjCalib = CreateDummyObjectiveCalibration(_measureAutofocusObjectiveId);
            var objCalibs = new List<ObjectiveCalibration>() { measureObjCalib, measureAFObjCalib };
            CreateNewObjectivesCalibrationsData(objCalibs);
        }

        private ObjectiveCalibration CreateDummyObjectiveCalibration(string objectiveID)
        {
            return new ObjectiveCalibration()
            {
                DeviceId = objectiveID,
                AutoFocus = null,
                Image = null,
                ZOffsetWithMainObjective = 0.Millimeters(),
                OpticalReferenceElevationFromStandardWafer = 0.Micrometers(),
            };
        }

        private static void CreateNewObjectivesCalibrationsData(List<ObjectiveCalibration> objCalibs)
        {
            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var objectiveCalibrationData = new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>(),
            };

            foreach (var objCalib in objCalibs)
            {
                objectiveCalibrationData.Calibrations.Add(objCalib);
            }

            calibrationManager.UpdateCalibration(objectiveCalibrationData);
        }

        private TopographySettings CreateTopographySettings()
        {
            return new TopographySettings()
            {
                NbOfRepeat = 1,
                CameraId = CameraUpId,
                ObjectiveId = _measureObjectiveId,
                SurfacesInFocus = SurfacesInFocus.Unknown,
                ScanMargin = TestConstants.ScanMargin,
                HeightVariation = TestConstants.HeightVariation,
                PostProcessingSettings = new PostProcessingSettings(),
            };
        }

        private TopographySettings CreateTopographySettingsWithAutofocus()
        {
            var topographySettings = CreateTopographySettings();
            topographySettings.AutoFocusSettings = new AutoFocusSettings()
            {
                Type = AutoFocusType.Camera,
                CameraId = CameraUpId,
                CameraScanRange = ScanRangeType.Configured,
                CameraScanRangeConfigured = new ScanRangeWithStep(11, 15, 1),
                ProbeId = LiseUpId,
                UseCurrentZPosition = true,
                ImageAutoFocusContext = CreateAFCameraContext(_measureAutofocusObjectiveId)
            };

            return topographySettings;
        }

        private TopImageAcquisitionContext CreateAFCameraContext(string objectiveId)
        {
            return new TopImageAcquisitionContext()
            {
                TopObjectiveContext = new TopObjectiveContext(objectiveId),
            };
        }

        private MeasureContext CreateMeasureContext()
        {
            //Given
            var resultFoldersPath = new Interface.Recipe.ResultFoldersPath();
            resultFoldersPath.RecipeFolderPath = TestConstants.TopographyResultFolderPath;
            resultFoldersPath.ExternalFileFolderName = "";

            return new MeasureContext(new MeasurePoint(0, TestConstants.MeasurePointPosition, false), null, resultFoldersPath);
        }

        private void setupAxisConfigs()
        {
            // Setup Axis config
            var axesConfig = new AxesConfig()
            {
                AxisConfigs = new List<AxisConfig>()
                {
                    new MotorizedAxisConfig()
                    {
                        MovingDirection = MovingDirection.ZBottom,
                        PositionMax = 2.9.Millimeters()
                    },
                    new MotorizedAxisConfig()
                    {
                        MovingDirection = MovingDirection.ZTop,
                        PositionMax = 19.9.Millimeters()
                    }
                }
            };
            SimulatedAxes.Setup(a => a.AxesConfiguration).Returns(axesConfig);
        }

        private void SetupPiezoAxis()
        {
            var piezoControllerConfig = new PiezoControllerConfig()
            {
                Name = TestConstants.PiezoAxisID,
                ControllerTypeName = "PiezoAxisConfig",
                DeviceID = _measureObjectiveId,
                PiezoAxisIDs = new List<string>() { TestConstants.PiezoAxisID }
            };
            var simulatedPiezoController = new Mock<PiezoController>(piezoControllerConfig) { CallBase = false };
            HardwareManager.Controllers.Add(_measureObjectiveId, simulatedPiezoController.Object);

            var piezoAxisConfig = new AxisConfig()
            {
                AxisID = TestConstants.PiezoAxisID,
                PositionMax = 100.Micrometers(),
                PositionMin = 0.Millimeters(),
                PositionHome = 50.Micrometers(),
            };
            var piezoAxis = new ACSAxis(piezoAxisConfig, null);
            simulatedPiezoController.Setup(_ => _.AxesList).Returns(new List<IAxis>() { piezoAxis });
        }

        private void SetupObjective()
        {
            var objectiveSelectorConfig = new SingleObjectiveSelectorConfig();
            var objectiveConfig = new ObjectiveConfig()
            {
                DepthOfField = 0.01.Millimeters(),
                DeviceID = _measureObjectiveId,
                PiezoAxisID = TestConstants.PiezoAxisID
            };
            objectiveSelectorConfig.Objectives = new List<ObjectiveConfig>() { objectiveConfig };
            objectiveSelectorConfig.DeviceID = _measureObjectiveId;
            var hardwareLogger = Mock.Of<IHardwareLogger>();
            var simulatedObjectiveSelector = new Mock<SingleObjectiveSelector>(objectiveSelectorConfig,hardwareLogger);
            HardwareManager.ObjectivesSelectors.Clear();
            HardwareManager.ObjectivesSelectors[_measureObjectiveId] = simulatedObjectiveSelector.Object;
        }

        #region Sequence functions

        private void SetupSequenceAxesGoToPositionInMeasureBase(XYZTopZBottomPosition position)
        {
            int sequenceNumberExepected = CurrentSetupSequenceId++;
            SimulatedAxes.Setup(
                a => a.GotoPosition(position, AxisSpeed.Normal))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(sequenceNumberExepected));

            // We don't manage WaitMotionEnd sequence order
            SimulatedAxes.Setup(a => a.WaitMotionEnd(It.IsAny<int>(), true));

            NbExpectedAxesGotoPosition++;
        }

        private void SetupSequenceAutofocus(XYPosition position)
        {
            int sequenceNumberExepected = CurrentSetupSequenceId++;
            SimulatedAxes.Setup(
                a => a.GotoPosition(position, AxisSpeed.Normal))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(sequenceNumberExepected));

            // We don't manage WaitMotionEnd sequence order
            SimulatedAxes.Setup(a => a.WaitMotionEnd(It.IsAny<int>(), true));

            NbExpectedAxesGotoPosition++;
        }

        protected void SetupContextManagerAppliedNullContext()
        {
            ContextManager.Setup(_ => _.Apply(null));

            NbExpectedContextApplications++;
        }

        protected void SetupSequenceForVSIFlowContextApplicationInMeasureTopography()
        {
            SetupContextManagerAppliedNullContext();
        }

        protected void SetupSequenceForAutofocusInMeasureBase(string objectiveId)
        {
            // First autofocus will call :
            // - AutofocusFlow with a null context
            // - AutofocusCameraFlow with context list that contains TopImageAcquisitionContext (objective + light) + position
            SetupContextManagerAppliedNullContext();

            int copyForLambda = CurrentSetupSequenceId++;
            ContextManager.Setup(
                _ => _.Apply(
                    It.Is<ContextsList>(
                        c => c.Contexts.OfType<TopImageAcquisitionContext>().Any()
                        && c.Contexts.OfType<XYPositionContext>().Any()
                        && c.Contexts.OfType<TopImageAcquisitionContext>().First().TopObjectiveContext.ObjectiveId == objectiveId)))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambda));

            NbExpectedContextApplications++;
        }

        protected void SetupSequenceForLandedAutofocusInMeasureTopography(string objectiveId)
        {
            // Second autofocus context is applied before autofocusCamera call. So we have :
            // - One new context creation using GetCurrent function
            // - One context application with a TopImageAcquisitionContext (objective + light)
            // - One AutofocusCamera with a null context
            ContextManager.Setup(_ => _.GetCurrent<TopImageAcquisitionContext>()).Returns(CreateAFCameraContext(objectiveId));

            int copyForLambda = CurrentSetupSequenceId++;
            ContextManager.Setup(
                _ => _.Apply(It.Is<TopImageAcquisitionContext>(c => c.TopObjectiveContext.ObjectiveId == objectiveId)))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambda));

            NbExpectedContextApplications++;

            SetupContextManagerAppliedNullContext();

            var pos = TestConstants.MeasurePointPosition.ToXYPosition();
            pos.Referential = new StageReferential();
            SetupSequenceAutofocus(pos);

            NbExpectedLandAndStopland++;
        }

        private void AssertCurrentCallSequenceIdIsExpected(int expectedSequenceId)
        {
            Assert.AreEqual(expectedSequenceId, CurrentCallSequenceId++);
        }

        protected void VerifySequenceAndNbExpectedCalls()
        {
            SimulatedAxes.VerifyAll();
            SimulatedAxes.Verify(a => a.Land(), Times.Exactly(NbExpectedLandAndStopland));
            SimulatedAxes.Verify(a => a.StopLanding(), Times.Exactly(NbExpectedLandAndStopland));
            SimulatedAxes.Verify(a => a.GotoPosition(It.IsAny<PositionBase>(), It.IsAny<AxisSpeed>()), Times.Exactly(NbExpectedAxesGotoPosition));

            ContextManager.VerifyAll();
            ContextManager.Verify(c => c.Apply(It.IsAny<ANAContextBase>()), Times.Exactly(NbExpectedContextApplications));
        }

        #endregion Sequence functions

        [TestMethod]
        public void MeasureTopography_GetMeasureTools_NominalCase()
        {
            // Given default settings
            var measureSettings = CreateTopographySettings();
            var measure = new MeasureTopography();

            // When getting measure tools
            var measureTools = measure.GetMeasureTools(measureSettings) as TopographyMeasureTools;

            //Then
            var nbExpectedCompatibleObjectives = 2;
            var expectedFirstObjectiveId = TestConstants.Objective5XINT.DeviceID;
            var expectedSecondObjectiveId = TestConstants.Objective50XINT.DeviceID;
            Assert.IsTrue(measureTools.CompatibleObjectives.Count == nbExpectedCompatibleObjectives);
            Assert.AreEqual(expectedFirstObjectiveId, measureTools.CompatibleObjectives[0]);
            Assert.AreEqual(expectedSecondObjectiveId, measureTools.CompatibleObjectives[1]);
        }

        [TestMethod]
        public void MeasureTopography_NominalCase()
        {
            //Given
            // We need to reset Simulated mock setup because it define the GetPos function which is never called
            // by measureTopography without autofocus
            SimulatedAxes.Reset();
            SetupPiezoAxis();
            SetupObjective();

            SetupSequenceAxesGoToPositionInMeasureBase(TestConstants.MeasurePointPosition);
            SetupSequenceForVSIFlowContextApplicationInMeasureTopography();

            var topographySettings = CreateTopographySettings();
            var measure = new MeasureTopography();

            //When measure is executed
            var measureResult = measure.Execute(topographySettings, CreateMeasureContext());

            //Then
            var topographyPointData = measureResult.Datas[0] as TopographyPointData;
            Assert.AreEqual(MeasureState.Success, topographyPointData.State);
            Assert.IsNotNull(topographyPointData);
            Assert.IsNotNull(topographyPointData.ResultImageFileName);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void MeasureTopography_with_autofocus()
        {
            //Given
            setupAxisConfigs();
            SetupPiezoAxis();
            SetupObjective();

            SetupSequenceAxesGoToPositionInMeasureBase(TestConstants.MeasurePointPosition);
            SetupSequenceForAutofocusInMeasureBase(_measureAutofocusObjectiveId);
            SetupSequenceForLandedAutofocusInMeasureTopography(_measureObjectiveId);
            SetupSequenceForVSIFlowContextApplicationInMeasureTopography();

            var topographySettings = CreateTopographySettingsWithAutofocus();
            var measure = new MeasureTopography();

            //When measure is executed
            var measureResult = measure.Execute(topographySettings, CreateMeasureContext());

            //Then
            var topographyPointData = measureResult.Datas[0] as TopographyPointData;
            Assert.AreEqual(MeasureState.Success, topographyPointData.State);
            Assert.IsNotNull(topographyPointData);
            Assert.IsNotNull(topographyPointData.ResultImageFileName);
            VerifySequenceAndNbExpectedCalls();
        }
    }
}
