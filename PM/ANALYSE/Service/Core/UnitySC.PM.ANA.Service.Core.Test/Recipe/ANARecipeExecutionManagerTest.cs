using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Recipe
{
    [TestClass]
    public class ANARecipeExecutionManagerTest : TestWithMockedHardware<ANARecipeExecutionManagerTest>, ITestWithAxes, ITestWithProbeLise, ITestWithCamera, ITestWithChuck, ITestWithLight
    {
        #region InterfaceProperties

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
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public Mock<LightBase> SimulatedLight { get; set; }
        public string LightId { get; set; }

        #endregion InterfaceProperties

        #region Sequence properties

        protected int NbExpectedSetLightIntensity { get; set; }

        protected int NbExpectedAxesGoto { get; set; }

        // Moq currently has bugs with MockSequence, so we manually handle sequences order by callbacks and these counters
        protected int CurrentSetupSequenceId { get; set; }

        protected int CurrentCallSequenceId { get; set; }

        #endregion Sequence properties

        protected override bool FlowsAreSimulated => true;

        private TestConstants _testConstants;
        private IReferentialManager _referentialManager;
        private ANARecipeExecutionManager _recipeExecutionManager;

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   AuthorizedMeasures = new List<MeasureType>
                   {
                        MeasureType.Thickness
                   },
                   Measures = new List<MeasureConfigurationBase>()
                   {
                       new MeasureThicknessConfiguration(),
                   }
               });
        }

        [TestMethod]
        public void Execute_recipe_without_measure_point()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = false, RunBwa = false, RunMarkAlignment = false };
            var recipe = CreateRecipe(alignmentParameters);

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.IsNull(_referentialManager.GetSettings(ReferentialTag.Wafer));
            Assert.IsTrue(result.Count == 0);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = false, RunBwa = false, RunMarkAlignment = false };
            var recipe = CreateRecipe(alignmentParameters);
            recipe.Points = _testConstants.MeasurePoints;

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            var zTopFocus = recipe.Alignment.AutoFocusLise.ZTopFocus.Millimeters;
            SetupSequenceMoveZTop(zTopFocus);

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_with_autofocus_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = true, RunBwa = false, RunMarkAlignment = false };
            var recipe = CreateRecipe(alignmentParameters);
            var zTopFocus = 13;// 13 came from AFLiseFlowDummy
            recipe.Points = _testConstants.MeasurePoints;

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            SetupSequenceAutofocus(recipe.Alignment.AutoFocusLise);
            SetupSequenceMoveZTop(zTopFocus);

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_with_bwa_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = false, RunBwa = true, RunMarkAlignment = false };
            var recipe = CreateRecipe(alignmentParameters);
            recipe.Points = _testConstants.MeasurePoints;

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            var zTopFocus = recipe.Alignment.AutoFocusLise.ZTopFocus.Millimeters;
            SetupSequenceMoveZTop(zTopFocus);
            SetupSequenceBWA();

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then

            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_with_mark_alignment_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = false, RunBwa = false, RunMarkAlignment = true };
            var recipe = CreateRecipe(alignmentParameters);
            recipe.Points = _testConstants.MeasurePoints;

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            var zTopFocus = recipe.Alignment.AutoFocusLise.ZTopFocus.Millimeters;
            SetupSequenceMoveZTop(zTopFocus);
            SetupSequenceMarkAlignment();

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_with_autofocus_bwa_mark_alignment_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = true, RunBwa = true, RunMarkAlignment = true };
            var recipe = CreateRecipe(alignmentParameters);
            var zTopFocus = 13;// 13 came from AFLiseFlowDummy result
            recipe.Points = _testConstants.MeasurePoints;

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            SetupSequenceAutofocus(recipe.Alignment.AutoFocusLise);
            SetupSequenceMoveZTop(zTopFocus); // 13 came from AFLiseFlowDummy result
            SetupSequenceBWA();
            SetupSequenceMarkAlignment();

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Execute_recipe_with_wafer_map_without_measure()
        {
            // Given
            var alignmentParameters = new AlignmentParameters() { RunAutoFocus = false, RunBwa = false, RunMarkAlignment = false };
            var recipe = CreateRecipe(alignmentParameters);
            recipe.Points = _testConstants.MeasurePoints;
            recipe.WaferMap = _testConstants.WaferMap;
            recipe.Dies = new List<DieIndex> { new DieIndex(1, 1) };

            SetupSequenceSetLightIntensity(recipe.Alignment.AutoLight.LightIntensity);
            var zTopFocus = recipe.Alignment.AutoFocusLise.ZTopFocus.Millimeters;
            SetupSequenceMoveZTop(zTopFocus);

            // When
            var result = _recipeExecutionManager.Execute(recipe);

            // Then
            Assert.AreEqual(0, result.Count);
            CheckWaferReferentialSettings(alignmentParameters);
            CheckDieReferentialSettings(recipe.WaferMap?.WaferMapData);
            VerifySequenceAndNbExpectedCalls();
        }

        protected override void PostGenericSetup()
        {
            // Set up the referential manager to return given pos
            var logger = ClassLocator.Default.GetInstance<ILogger>();
            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();

            SimulatedAxes = new Mock<IAxes>();
            HardwareManager.Axes = SimulatedAxes.Object;

            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            _referentialManager = new AnaReferentialManager();
            _recipeExecutionManager = new ANARecipeExecutionManager(logger, HardwareManager, _referentialManager, pmConfiguration);
            _testConstants = new TestConstants(ObjectiveUpId, CameraUpId);
        }

        private void SetupSequenceBWA()
        {
            // Nothing to setup
        }

        private void SetupSequenceMarkAlignment()
        {
            // Nothing to setup
        }

        private void SetupSequenceMoveZTop(double zTopFocus)
        {
            var focusPosition = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zTopFocus, double.NaN);
            SetupSequenceAddAxesGoToPosition(focusPosition);
        }

        private void SetupSequenceAutofocus(AutoFocusLiseParameters afParameters)
        {
            var zTopFocusParam = afParameters.ZTopFocus.Millimeters;
            var centerPosition = new XYZTopZBottomPosition(new StageReferential(), 0, 0, zTopFocusParam, double.NaN);
            SetupSequenceAddAxesGoToPosition(centerPosition);
        }

        private void SetupSequenceAddAxesGoToPosition(XYZTopZBottomPosition position)
        {
            int sequenceNumberExepected = CurrentSetupSequenceId++;
            SimulatedAxes.Setup(
                a => a.GotoPosition(position, AxisSpeed.Normal))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(sequenceNumberExepected));

            // We don't manage WaitMotionEnd sequence order
            SimulatedAxes.Setup(a => a.WaitMotionEnd(It.IsAny<int>(), true));

            NbExpectedAxesGoto++;
        }

        private void SetupSequenceSetLightIntensity(double intensity)
        {
            SimulatedLight.Setup(_ => _.IsMainLight).Returns(true);

            int sequenceNumberExepected = CurrentSetupSequenceId++;
            SimulatedLight.Setup(
                a => a.SetIntensity(It.Is<double>(arg => arg == intensity)))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(sequenceNumberExepected));

            NbExpectedSetLightIntensity++;
        }

        private void VerifySequenceAndNbExpectedCalls()
        {
            SimulatedLight.VerifyAll();
            SimulatedLight.Verify(l => l.SetIntensity(It.IsAny<double>()), Times.Exactly(NbExpectedSetLightIntensity));

            SimulatedAxes.VerifyAll();
            SimulatedAxes.Verify(a => a.GotoPosition(It.IsAny<PositionBase>(), It.IsAny<AxisSpeed>()), Times.Exactly(NbExpectedAxesGoto));
            SimulatedAxes.Verify(a => a.WaitMotionEnd(It.IsAny<int>(), true), Times.Exactly(NbExpectedAxesGoto));
        }

        private void AssertCurrentCallSequenceIdIsExpected(int expectedSequenceId)
        {
            Assert.AreEqual(expectedSequenceId, CurrentCallSequenceId++);
        }

        private void CheckWaferReferentialSettings(AlignmentParameters alignmentParameters)
        {
            double expectedZTopFocus;
            double expectedShiftX = 0;
            double expectedShiftY = 0;
            double expectedAngle = 0;

            if (alignmentParameters.RunAutoFocus)
            {
                expectedZTopFocus = 13; // These values came from AFLiseFlowDummy
            }
            else
            {
                expectedZTopFocus = _testConstants.RecipeAlignment.AutoFocusLise.ZTopFocus.Millimeters;
            }

            if (alignmentParameters.RunBwa)
            {
                expectedShiftX = 10.0 + ( 0.585 * 1000.0); // These values came from BareWaferAlignmentFlowDummy
                expectedShiftY = 10.0 + (-2.023 * 1000.0);
                expectedAngle = 2.0;
            }

            if (alignmentParameters.RunMarkAlignment)
            {
                expectedShiftX += 3; // These values came from AlignmentMarksFlowDummy
                expectedShiftY += 2;
                expectedAngle += 0.5;
            }

            var waferRef = _referentialManager.GetSettings(ReferentialTag.Wafer) as WaferReferentialSettings;
            Assert.IsNotNull(_referentialManager.GetSettings(ReferentialTag.Wafer));
            Assert.AreEqual(expectedZTopFocus, waferRef.ZTopFocus.Millimeters, 0.000001);
            Assert.AreEqual(expectedShiftX, waferRef.ShiftX.Micrometers, 0.000001);
            Assert.AreEqual(expectedShiftY, waferRef.ShiftY.Micrometers, 0.000001);
            Assert.AreEqual(expectedAngle, waferRef.WaferAngle.Value, 0.000001);
        }

        private void CheckDieReferentialSettings(WaferMapResult waferMapExpected)
        {
            var DieRef = _referentialManager.GetSettings(ReferentialTag.Die) as DieReferentialSettings;
            if (waferMapExpected == null)
            {
                Assert.IsNull(_referentialManager.GetSettings(ReferentialTag.Die));
            }
            else
            {
                Assert.IsNotNull(_referentialManager.GetSettings(ReferentialTag.Die));
                Assert.AreEqual(waferMapExpected.RotationAngle, DieRef.DieGridAngle);
                Assert.AreEqual(waferMapExpected.DieDimensions, DieRef.DieGridDimensions);
                Assert.AreEqual(waferMapExpected.DieGridTopLeft, DieRef.DieGridTopLeft);
                Assert.AreEqual(waferMapExpected.DiesPresence, DieRef.PresenceGrid);
            }
        }

        private ANARecipe CreateRecipe(AlignmentParameters alignmentParameters)
        {
            var recipe = new ANARecipe();
            recipe.Alignment = _testConstants.RecipeAlignment;
            recipe.AlignmentMarks = _testConstants.AlignmentMarks;
            recipe.Step = _testConstants.Step;
            recipe.Points = new List<MeasurePoint>();
            recipe.Measures = new List<MeasureSettingsBase>();
            recipe.Execution = new ExecutionSettings()
            {
                Alignment = alignmentParameters,
                Strategy = MeasurementStrategy.PerPoint,
            };
            return recipe;
        }
    }

    internal class TestConstants
    {
        public Step Step;
        public List<MeasureSettingsBase> Measures;
        public List<MeasurePoint> MeasurePoints;
        public AlignmentSettings RecipeAlignment;
        public AlignmentMarksSettings AlignmentMarks;
        public WaferMapSettings WaferMap;

        public TestConstants(string objectiveUpId, string cameraUpId)
        {
            Step = new Step();
            Step.Product = new Product();
            Step.Product.WaferCategory = new WaferCategory()
            {
                DimentionalCharacteristic = new WaferDimensionalCharacteristic
                {
                    WaferShape = WaferShape.Notch,
                    Diameter = 300.Millimeters(),
                    Category = "1.15",
                    DiameterTolerance = null,
                    Flats = null,
                    Notch = new NotchDimentionalCharacteristic
                    {
                        Depth = 1.Millimeters(),
                        Angle = 0.Degrees(),
                        DepthPositiveTolerance = 0.25.Millimeters(),
                        AngleNegativeTolerance = 1.Degrees(),
                        AnglePositiveTolerance = 5.Degrees()
                    },
                    SampleWidth = null,
                    SampleHeight = null
                }
            };

            Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings
                {
                    Name = "mon tsv",
                    IsActive = true,
                    MeasurePoints = new List<int> { 0, 1 },
                    NbOfRepeat = 1,
                    Strategy = TSVAcquisitionStrategy.Standard,
                    Precision = TSVMeasurePrecision.Fast,
                    DepthTarget = 50.Micrometers(),
                    DepthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                    LengthTarget = 5.Micrometers(),
                    LengthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                    WidthTarget = 6.Micrometers(),
                    WidthTolerance = new LengthTolerance(2, LengthToleranceUnit.Micrometer),
                    CameraId = "1",
                    Probe = new ProbeSettings(){ProbeId = "ProbeLiseUp" },
                    EllipseDetectionTolerance = 1.Micrometers(),
                    Shape = UnitySC.Shared.Format.Metro.TSV.TSVShape.Elipse,
                    ROI = null
                }
            };

            MeasurePoints = new List<MeasurePoint>()
            {
                new MeasurePoint
                {
                    PatternRec = null,
                    Id = 0,
                    Position = new PointPosition { X = 2.776, Y = -138.323, ZTop = 0.780, ZBottom = 0 }
                },
                new MeasurePoint
                {
                    PatternRec = null,
                    Id = 1,
                    Position = new PointPosition { X = 2.822, Y = -138.322, ZTop = 0.780, ZBottom = 0 }
                }
            };

            RecipeAlignment = new AlignmentSettings()
            {
                AutoFocusLise = new AutoFocusLiseParameters
                {
                    ZIsDefinedByUser = false,
                    ZTopFocus = 13.3.Millimeters(), // expected
                    LiseParametersAreDefinedByUser = false,
                    LiseGain = 1.8,
                    ZMin = 12.Millimeters(),
                    ZMax = 15.Millimeters(),
                    LiseObjectiveContext = new ObjectiveContext(objectiveUpId)
                },
                AutoLight = new AutoLightParameters()
                {
                    LightIntensity = 0.5,
                },
                BareWaferAlignment = new BareWaferAlignmentParameters()
                {
                    CustomImagePositions = new List<BareWaferAlignmentImagePosition>(),
                    ObjectiveContext = new ObjectiveContext(objectiveUpId)
                },
            };

            var site1Img = new PositionWithPatternRec()
            {
                Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, -250),
                Context = new TopImageAcquisitionContext(),
                PatternRec = new PatternRecognitionData()
                {
                    CameraId = cameraUpId,
                    Gamma = 0.35,
                    PatternReference = new UnitySC.Shared.Data.ExternalFile.ExternalImage(),
                    RegionOfInterest = new RegionOfInterest(0.Millimeters(), 0.Millimeters(), 100.Micrometers(), 100.Micrometers()),
                }
            };

            var site2Img = new PositionWithPatternRec()
            {
                Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 250),
                Context = new TopImageAcquisitionContext(),
                PatternRec = new PatternRecognitionData()
                {
                    CameraId = cameraUpId,
                    Gamma = 0.35,
                    PatternReference = new UnitySC.Shared.Data.ExternalFile.ExternalImage(),
                    RegionOfInterest = new RegionOfInterest(0.Millimeters(), 0.Millimeters(), 100.Micrometers(), 100.Micrometers())
                }
            };

            AlignmentMarks = new AlignmentMarksSettings()
            {
                AlignmentMarksSite1 = new List<PositionWithPatternRec>() { site1Img },
                AlignmentMarksSite2 = new List<PositionWithPatternRec>() { site2Img },
                ObjectiveContext = new ObjectiveContext(objectiveUpId),
            };

            WaferMap = new WaferMapSettings()
            {
                WaferMapData = new WaferMapResult()
                {
                    RotationAngle = 0.3.Degrees(),
                    DieGridTopLeft = new XYPosition(new WaferReferential(), -150, 150),
                    DieDimensions = new DieDimensionalCharacteristic(
                                       dieWidth: 14.5.Millimeters(),
                                       dieHeight: 14.5.Millimeters(),
                                       streetWidth: 0.5.Millimeters(),
                                       streetHeight: 0.5.Millimeters(),
                                       dieAngle: 0.Degrees()),
                    DiesPresence = new Matrix<bool>(20, 20),
                    DieReference = new DieIndex(0, 0)
                },
            };
        }
    }
}
