using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    internal static class TestConstants
    {
        public static MeasureType MeasureType = MeasureType.TSV;

        public static TSVResultSettings MeasureResSettings = new TSVResultSettings()
        {
            WidthTarget = 10.Micrometers(),
            WidthTolerance = new LengthTolerance(2, LengthToleranceUnit.Micrometer),
            DepthTarget = 5.Micrometers(),
            DepthTolerance = new LengthTolerance(2, LengthToleranceUnit.Micrometer),
            LengthTarget = 5.Micrometers(),
            LengthTolerance = new LengthTolerance(2, LengthToleranceUnit.Micrometer)
        };

        public static TSVPointData MeasurePointData = new TSVPointData()
        {
            State = MeasureState.Success,
            LengthState = MeasureState.Success,
            DepthState = MeasureState.Success,
            WidthState = MeasureState.Success,
            Length = 5.Micrometers(),
            Depth = 10.Micrometers(),
            Width = 5.Micrometers(),
            Timestamp = DateTime.Now,
            QualityScore = 1.0
        };

        public static double AutofocusZTopSimulatedResult = 13; // This value came from AFLiseFlowDummy class
        public static double PatternRecSimulatedShiftX = 0.015; // This value came from PatternRecFlowDummy class
        public static double PatternRecSimulatedShiftY = 0.012; // This value came from PatternRecFlowDummy class

        public static double NearEpsilon = 0.001;

        public static int DieCol = 1;
        public static int DieRow = 2;
        public static DieIndex DieIndex = new DieIndex(DieCol, DieRow);

        public static MeasurePoint MeasurePoint = new MeasurePoint(1, new PointPosition(0.0, 1.0, 2.0, 3.0));
        public static MeasurePoint MeasurePointWithPatternRec = new MeasurePoint(MeasurePoint.Id, MeasurePoint.Position, SimulatedData.ValidPatternRecognitionDataWithContext());

        public static XYZTopZBottomPosition MeasurePositionWafer = MeasurePoint.Position.ToXYZTopZBottomPosition(new WaferReferential());
        public static MeasureContext DefaultContextWafer = new MeasureContext(MeasurePoint, null, new Interface.Recipe.ResultFoldersPath());
        public static MeasureContext DefaultContextWaferWithPatternRec = new MeasureContext(MeasurePointWithPatternRec, null, new Interface.Recipe.ResultFoldersPath());

        public static XYZTopZBottomPosition MeasurePositionDie = MeasurePoint.Position.ToXYZTopZBottomPosition(new DieReferential(DieCol, DieRow));
        public static MeasureContext DefaultContextDie = new MeasureContext(MeasurePoint, DieIndex, new Interface.Recipe.ResultFoldersPath());
        public static MeasureContext DefaultContextDieWithPatternRec = new MeasureContext(MeasurePointWithPatternRec, DieIndex, new Interface.Recipe.ResultFoldersPath());
    }

    // Needed because MeasureToolsBase is abstract
    internal class TestableMeasureTools : MeasureToolsBase
    {
    }

    internal class TestableMeasureSettings : MeasureSettingsBase
    {
        public TestableMeasureSettings()
        {
            NbOfRepeat = 1;
        }

        public override MeasureType MeasureType => TestConstants.MeasureType;
    }

    internal class TestableMeasureSettingsStartAtMeasurePoint : TestableMeasureSettings
    {
        public override bool MeasureStartAtMeasurePoint => true;
    }

    internal class TestableMeasureSettingsWithAutofocus : TestableMeasureSettings, IAutoFocusMeasureSettings
    {
        public TestableMeasureSettingsWithAutofocus()
        {
            AutoFocusSettings = SimulatedData.ValidAFSettings();
        }

        public AutoFocusSettings AutoFocusSettings { get; set; }
    }

    internal class TestableMeasureSettingsWithAutofocusStartAtMeasurePoint : TestableMeasureSettingsWithAutofocus
    {
        public override bool MeasureStartAtMeasurePoint => true;
    }

    /// <summary>
    /// Base class to test measure.
    /// </summary>
    /// <typeparam name="TMeasureSettings">Adapted in every unit test for each use case
    /// (a measure that starts or not at the measure point and/or a measure that performs or not
    /// an autofocus)</typeparam>
    internal class TestableMeasureBase<TMeasureSettings> : MeasureBase<TMeasureSettings, MeasurePointResult>
        where TMeasureSettings : TestableMeasureSettings
    {
        public TestableMeasureBase() : base(ClassLocator.Default.GetInstance<ILogger<TestableMeasureBase<TMeasureSettings>>>())
        {
        }

        public override MeasureType MeasureType => TestConstants.MeasureType;

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var metroResult = new UnitySC.Shared.Format.Metro.TSV.TSVResult();
            metroResult.Settings = TestConstants.MeasureResSettings;
            return metroResult;
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(TMeasureSettings measureSettings, Exception Ex)
        {
            return null;
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(TMeasureSettings measureSettings)
        {
            return new TestableMeasureTools();
        }

        protected override MeasurePointDataResultBase Process(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            return TestConstants.MeasurePointData;
        }
    }

    public class ParameterizedTestCaseData
    {
        public MeasureContext Context { get; }
        public XYZTopZBottomPosition MeasurePointPosition { get; }
        public string DataName { get; }

        private ParameterizedTestCaseData(MeasureContext context, XYZTopZBottomPosition measurePointPosition, string dataName)
        {
            Context = context;
            MeasurePointPosition = measurePointPosition;
            DataName = dataName;
        }

        public static IEnumerable<object[]> DataContextWithoutPatternRec
        {
            get
            {
                return new[] {
                    new object[] { new ParameterizedTestCaseData(TestConstants.DefaultContextWafer, TestConstants.MeasurePositionWafer, "Wafer data without pattern rec")},
                    new object[] { new ParameterizedTestCaseData(TestConstants.DefaultContextDie, TestConstants.MeasurePositionDie, "Die data without pattern rec")}
                };
            }
        }

        public static IEnumerable<object[]> DataContextWithPatternRec
        {
            get
            {
                return new[] {
                    new object[] { new ParameterizedTestCaseData(TestConstants.DefaultContextWaferWithPatternRec, TestConstants.MeasurePositionWafer, "Wafer data with pattern rec")},
                    new object[] { new ParameterizedTestCaseData(TestConstants.DefaultContextDieWithPatternRec, TestConstants.MeasurePositionDie, "Die data with pattern rec")}
                };
            }
        }

        public static string GetDynamicNameOfTestCaseWithData(MethodInfo _, object[] data)
        {
            return $"Test with {(data[0] as ParameterizedTestCaseData).DataName}";
        }
    }

    [TestClass]
    public class MeasureBaseTest : TestWithMockedHardware<MeasureBaseTest>, ITestWithAxes, ITestWithProbeLise
    {
        // Simulate flows
        protected override bool FlowsAreSimulated => true;

        protected Mock<IContextManager> ContextManager { get; set; }

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

        #region Sequence properties

        protected int NbExpectedAxesGoto { get; set; }

        protected int NbExpectedAxesMoveIncremental { get; set; }

        protected int NbExpectedContextApplications { get; set; }

        // Moq currently has bugs with MockSequence, so we manually handle sequences order by callbacks and these counters
        protected int CurrentSetupSequenceId { get; set; }

        protected int CurrentCallSequenceId { get; set; }

        #endregion Sequence properties

        protected override void SpecializeRegister()
        {
            ContextManager = new Mock<IContextManager>(MockBehavior.Strict);
            ClassLocator.Default.Register(() => ContextManager.Object, true);
        }

        protected override void PreGenericSetup()
        {
            //// Use a strict mock
            SimulatedAxes = new Mock<IAxes>(MockBehavior.Strict);
        }

        protected override void PostGenericSetup()
        {
            NbExpectedAxesGoto = 0;
            NbExpectedAxesMoveIncremental = 0;
            NbExpectedContextApplications = 0;
            CurrentSetupSequenceId = 0;
            CurrentCallSequenceId = 0;
        }

        protected static bool XYPositionsAreEqual(XYPosition left, XYPosition right)
        {
            return left.Referential == right.Referential &&
                   left.X.IsNearOrBothNaN(right.X, TestConstants.NearEpsilon) &&
                   left.Y.IsNearOrBothNaN(right.Y, TestConstants.NearEpsilon);
        }

        protected static bool XYZTopZBottomPositionsAreEqual(XYZTopZBottomPosition left, XYZTopZBottomPosition right)
        {
            return XYPositionsAreEqual(left, right) &&
               left.ZTop.IsNearOrBothNaN(right.ZTop, TestConstants.NearEpsilon) &&
               left.ZBottom.IsNearOrBothNaN(right.ZBottom, TestConstants.NearEpsilon);
        }

        protected static bool XYZTopZBottomMovesAreEqual(XYZTopZBottomMove left, XYZTopZBottomMove right)
        {
            return left.ZTop.IsNearOrBothNaN(right.ZTop, TestConstants.NearEpsilon) &&
                   left.ZBottom.IsNearOrBothNaN(right.ZBottom, TestConstants.NearEpsilon) &&
                   left.X.IsNearOrBothNaN(right.X, TestConstants.NearEpsilon) &&
                   left.Y.IsNearOrBothNaN(right.Y, TestConstants.NearEpsilon);
        }

        protected void SetupSequenceAddAxesDoNotMove()
        {
            // Empty func just to add readability to the tests.
        }

        protected void AssertCurrentCallSequenceIdIsExpected(int expectedSequenceId)
        {
            Assert.AreEqual(expectedSequenceId, CurrentCallSequenceId++);
        }

        protected void SetupSequenceAddAxesGoToPosition(XYZTopZBottomPosition position)
        {
            int copyForLambdaGoToPosition = CurrentSetupSequenceId++;
            SimulatedAxes.Setup(
                a => a.GotoPosition(
                    It.Is<XYZTopZBottomPosition>(argPos => XYZTopZBottomPositionsAreEqual(argPos, position)),
                    AxisSpeed.Normal))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambdaGoToPosition));

            // We don't manage WaitMotionEnd sequence order
            SimulatedAxes.Setup(a => a.WaitMotionEnd(It.IsAny<int>(),true));

            NbExpectedAxesGoto++;
        }

        protected void SetupSequenceAddAxesMoveIncremental(XYZTopZBottomMove position)
        {
            int copyForLambdaGoToPosition = CurrentSetupSequenceId++;
            SimulatedAxes.Setup(
                a => a.MoveIncremental(
                    It.Is<XYZTopZBottomMove>(argPos => XYZTopZBottomMovesAreEqual(argPos, position)),
                    AxisSpeed.Normal))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambdaGoToPosition));

            // We don't manage WaitMotionEnd sequence order
            SimulatedAxes.Setup(a => a.WaitMotionEnd(It.IsAny<int>(), true));

            NbExpectedAxesMoveIncremental++;
        }

        protected void SetupSequenceAddContextManagerAppliedOnAutofocusInitialContext()
        {
            ContextManager.Setup(_ => _.Apply(null));

            NbExpectedContextApplications++;
        }

        protected void SetupSequenceAddContextManagerAppliedOnAFLiseInitialContext()
        {
            int copyForLambda = CurrentSetupSequenceId++;
            ContextManager.Setup(
                _ => _.Apply(
                    It.Is<ContextsList>(
                        c => c.Contexts.OfType<ObjectiveContext>().Any()
                        && c.Contexts.OfType<XYPositionContext>().Any())))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambda));

            NbExpectedContextApplications++;
        }

        protected void SetupSequenceAddContextManagerAppliedOnPatternRecInitialContext()
        {
            int copyForLambda = CurrentSetupSequenceId++;
            ContextManager.Setup(
                _ => _.Apply(It.IsAny<TopImageAcquisitionContext>()))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambda));

            NbExpectedContextApplications++;
        }

        protected void SetupSequenceAddContextManagerAppliedOnXYZTopZBottomPosition(XYZTopZBottomPosition position)
        {
            int copyForLambda = CurrentSetupSequenceId++;
            ContextManager.Setup(
                _ => _.Apply(
                    It.Is<ContextsList>(
                        c => c.Contexts.OfType<XYZTopZBottomPositionContext>().Any()
                        && XYZTopZBottomPositionsAreEqual(c.Contexts.OfType<XYZTopZBottomPositionContext>().First().Position, position))))
                .Callback(() => AssertCurrentCallSequenceIdIsExpected(copyForLambda));
            NbExpectedContextApplications++;
        }

        protected void VerifySequenceAndNbExpectedCalls()
        {
            SimulatedAxes.VerifyAll();
            SimulatedAxes.Verify(a => a.GotoPosition(It.IsAny<PositionBase>(), It.IsAny<AxisSpeed>()), Times.Exactly(NbExpectedAxesGoto));
            SimulatedAxes.Verify(a => a.MoveIncremental(It.IsAny<XYZTopZBottomMove>(), It.IsAny<AxisSpeed>()), Times.Exactly(NbExpectedAxesMoveIncremental));
            SimulatedAxes.Verify(a => a.WaitMotionEnd(It.IsAny<int>(),true), Times.Exactly(NbExpectedAxesGoto + NbExpectedAxesMoveIncremental));
            ContextManager.VerifyAll();
            ContextManager.Verify(c => c.Apply(It.IsAny<ANAContextBase>()), Times.Exactly(NbExpectedContextApplications));
        }

        [TestMethod]
        public void Default_measure_doesnt_move_axes_to_measure_point()
        {
            // Given a measure that is set to not start at measure point (default implem of MeasureSettingsBase.MeasureStartAtMeasurePoint)
            var settingsDontStartAtMeasurePoint = new TestableMeasureSettings();
            var measure = new TestableMeasureBase<TestableMeasureSettings>();
            // (Then setup) axes are never asked to go to the measure point
            SetupSequenceAddAxesDoNotMove();

            // When executing the measure
            var res = measure.Execute(settingsDontStartAtMeasurePoint, TestConstants.DefaultContextWafer);

            // Then the axes are never asked to go to the measure point
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Measure_that_stop_with_cancellationToken_cancel()
        {
            // Given a measure that is set to not start at measure point (default implem of MeasureSettingsBase.MeasureStartAtMeasurePoint)
            var settingsDontStartAtMeasurePoint = new TestableMeasureSettings();
            var measure = new TestableMeasureBase<TestableMeasureSettings>();
            // (Then setup) axes are never asked to go to the measure point
            SetupSequenceAddAxesDoNotMove();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            source.Cancel();

            // When executing the measure
            var res = measure.Execute(settingsDontStartAtMeasurePoint, TestConstants.DefaultContextWafer, token);

            // Then the axes are never asked to go to the measure point
            Assert.AreEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        [DynamicData(nameof(ParameterizedTestCaseData.DataContextWithoutPatternRec), typeof(ParameterizedTestCaseData),
            DynamicDataDisplayName = nameof(ParameterizedTestCaseData.GetDynamicNameOfTestCaseWithData),
            DynamicDataDisplayNameDeclaringType = typeof(ParameterizedTestCaseData))]
        public void Measure_that_starts_at_point_moves_axes_to_measure_point(ParameterizedTestCaseData data)
        {
            // Given a measure that is set to start at measure point
            var settingsStartAtMeasurePoint = new TestableMeasureSettingsStartAtMeasurePoint();
            var measure = new TestableMeasureBase<TestableMeasureSettingsStartAtMeasurePoint>();

            // Given measure point has no pattern rec data
            Assert.IsNull(data.Context.MeasurePoint.PatternRec);

            // (Then setup) axes are only asked to go to the measure point
            SetupSequenceAddAxesGoToPosition(data.MeasurePointPosition);

            // When executing the measure
            var res = measure.Execute(settingsStartAtMeasurePoint, data.Context);

            // Then the axes are only asked to go to the measure point
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        public void Measure_with_null_autofocus_does_nothing()
        {
            // Given a measure that needs an autofocus, but with null autofocus
            var settingsWithAutofocus = new TestableMeasureSettingsWithAutofocus()
            {
                AutoFocusSettings = null
            };
            var measure = new TestableMeasureBase<TestableMeasureSettingsWithAutofocus>();
            // (Then setup) axes are never asked to go to the measure point
            SetupSequenceAddAxesDoNotMove();

            // When executing the measure
            var res = measure.Execute(settingsWithAutofocus, TestConstants.DefaultContextWafer);

            // Then the axes are never asked to go to the measure point
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        [DynamicData(nameof(ParameterizedTestCaseData.DataContextWithoutPatternRec), typeof(ParameterizedTestCaseData),
            DynamicDataDisplayName = nameof(ParameterizedTestCaseData.GetDynamicNameOfTestCaseWithData),
            DynamicDataDisplayNameDeclaringType = typeof(ParameterizedTestCaseData))]
        public void Measure_with_autofocus_performs_autofocus(ParameterizedTestCaseData data)
        {
            // Given a measure that needs an autofocus, but with null autofocus
            var settingsWithAutofocus = new TestableMeasureSettingsWithAutofocus();
            settingsWithAutofocus.AutoFocusSettings.ProbeId = LiseUpId;
            settingsWithAutofocus.AutoFocusSettings.Type = AutoFocusType.Lise;
            settingsWithAutofocus.AutoFocusSettings.LiseAutoFocusContext = new ObjectiveContext(ObjectiveUpId);
            var measure = new TestableMeasureBase<TestableMeasureSettingsWithAutofocus>();

            // (Then setup)
            // AutofocusFlow will apply on first empty context, and a second one for AFLiseFlow
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0));
            SetupSequenceAddContextManagerAppliedOnAFLiseInitialContext();
            // Leave this setup at the end, or the test will not pass !
            SetupSequenceAddContextManagerAppliedOnAutofocusInitialContext();

            // AFLiseFlow will move to focus position but, we use AFLiseFlowDummy for test.
            // This one don't move to focus position

            // When executing the measure
            var res = measure.Execute(settingsWithAutofocus, data.Context);

            // Then the axes are asked to go to the focus point only
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        [DynamicData(nameof(ParameterizedTestCaseData.DataContextWithPatternRec), typeof(ParameterizedTestCaseData),
            DynamicDataDisplayName = nameof(ParameterizedTestCaseData.GetDynamicNameOfTestCaseWithData),
            DynamicDataDisplayNameDeclaringType = typeof(ParameterizedTestCaseData))]
        public void Measure_that_starts_at_point_with_pattern_rec_moves_axes_to_pattern_rec_corrected_point(ParameterizedTestCaseData data)
        {
            // Given a measure that is set to start at measure point
            var settingsStartAtMeasurePoint = new TestableMeasureSettingsStartAtMeasurePoint();
            var measure = new TestableMeasureBase<TestableMeasureSettingsStartAtMeasurePoint>();

            // Given measure point has pattern rec data
            Assert.IsNotNull(data.Context.MeasurePoint.PatternRec);

            // (Then setup)
            // Measure start at point will call go to position
            // And the axes are asked to go to the shifted position
            var measurePosition = (XYZTopZBottomPosition)data.MeasurePointPosition.Clone();
            SetupSequenceAddAxesGoToPosition(measurePosition);

            // PatternRecFlow will apply a TopImageAcquisitionContext (Lights + Objectives), and then moveIncremental to reach shifted position
            SetupSequenceAddContextManagerAppliedOnPatternRecInitialContext();
            SetupSequenceAddAxesMoveIncremental(new XYZTopZBottomMove(TestConstants.PatternRecSimulatedShiftX, TestConstants.PatternRecSimulatedShiftY, 0, 0));

            // When executing the measure on a context with pattern rec
            var res = measure.Execute(settingsStartAtMeasurePoint, data.Context);

            // Then the pattern rec context is applied and the axes are asked to go to the shifted position
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }

        [TestMethod]
        [DynamicData(nameof(ParameterizedTestCaseData.DataContextWithPatternRec), typeof(ParameterizedTestCaseData),
            DynamicDataDisplayName = nameof(ParameterizedTestCaseData.GetDynamicNameOfTestCaseWithData),
            DynamicDataDisplayNameDeclaringType = typeof(ParameterizedTestCaseData))]
        public void Measure_that_autofocus_and_starts_at_point_with_pattern_rec_performs_autofocus_then_pattern_rec_then_move(ParameterizedTestCaseData data)
        {
            // Given a measure that is set to start at measure point
            var settingsStartAtMeasurePoint = new TestableMeasureSettingsWithAutofocusStartAtMeasurePoint();
            settingsStartAtMeasurePoint.AutoFocusSettings.ProbeId = LiseUpId;
            settingsStartAtMeasurePoint.AutoFocusSettings.Type = AutoFocusType.Lise;
            settingsStartAtMeasurePoint.AutoFocusSettings.LiseAutoFocusContext = new ObjectiveContext(ObjectiveUpId);

            var measure = new TestableMeasureBase<TestableMeasureSettingsWithAutofocusStartAtMeasurePoint>();

            // Given measure point has pattern rec data
            Assert.IsNotNull(data.Context.MeasurePoint.PatternRec);

            // (Then setup)
            // Measure start at point will call go to position
            // And the axes are asked to go to the shifted position
            var measurePosition = (XYZTopZBottomPosition)data.MeasurePointPosition.Clone();
            SetupSequenceAddAxesGoToPosition(measurePosition);

            // AutofocusFlow will apply on first empty context, and a second one for AFLiseFlow
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0));
            SetupSequenceAddContextManagerAppliedOnAFLiseInitialContext();

            // PatternRecFlow will apply a TopImageAcquisitionContext (Lights + Objectives), and then moveIncremental to reach shifted position
            SetupSequenceAddContextManagerAppliedOnPatternRecInitialContext();
            SetupSequenceAddAxesMoveIncremental(new XYZTopZBottomMove(TestConstants.PatternRecSimulatedShiftX, TestConstants.PatternRecSimulatedShiftY, 0, 0));

            // Leave this setup at the end, or the test will not pass !
            SetupSequenceAddContextManagerAppliedOnAutofocusInitialContext();

            // AFLiseFlow will move to focus position but, we use AFLiseFlowDummy for test.
            // This one don't move to focus position

            // When executing the measure on a context with pattern rec
            var res = measure.Execute(settingsStartAtMeasurePoint, data.Context);

            // Then the pattern rec context is applied and the axes are asked to go to the shifted position
            Assert.AreNotEqual(res.State, MeasureState.NotMeasured);
            VerifySequenceAndNbExpectedCalls();
        }
    }
}
