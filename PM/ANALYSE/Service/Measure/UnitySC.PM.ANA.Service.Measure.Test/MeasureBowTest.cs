using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Bow;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureBowTest : TestWithMockedHardware<MeasureBowTest>, ITestWithAxes, ITestWithProbeLise, ITestWithChuck, ITestWithCamera
    {
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
        public double ThicknessThresholdInTheAir { get; set; }

        private const double Precision = 1e-3;

        private BowSettings CreateBowSettings()
        {
            var newBowSettings = new BowSettings()
            {
                ProbeSettings = new SingleLiseSettings()
                {
                    ProbeId = LiseUpId,
                    LiseGain = 1.8,
                    ProbeObjectiveContext = new Interface.Context.ObjectiveContext()
                    {
                        ObjectiveId = ObjectiveUpId
                    }
                },
                NbOfRepeat = 1,

                BowMin = -3.Millimeters(),
                BowMax = 3.Millimeters(),

                SubMeasurePoints = new List<int>(),
                IsMeasureWithSubMeasurePoints = true,

                WaferCharacteristic = new WaferDimensionalCharacteristic()
                {
                    Diameter = 300.Millimeters(),
                }
            };

            return newBowSettings;
        }

        private BowTotalPointData PrepareAndExecuteMeasureBow()
        {
            var bowSettings = CreateBowSettings();

            //When measure is executed
            var measure = new MeasureBow();
            var subMeasuresResults = new List<MeasurePointResult>();
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(1, 0d, 0d)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(2, 0d, 143.650)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(3, -124.405, -71.825)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(4, 124.405, -71.825)));
            var measureResult = measure.ComputeMeasureFromSubMeasures(bowSettings, CreateMeasureContext(1, 0d, 0d), subMeasuresResults);
            var bowPointData1 = subMeasuresResults[0].Datas[0] as BowPointData;
            var bowPointData2 = subMeasuresResults[1].Datas[0] as BowPointData;
            var bowPointData3 = subMeasuresResults[2].Datas[0] as BowPointData;
            var bowTotalPointData = measureResult.Datas[0] as BowTotalPointData;
            Assert.IsNotNull(bowPointData1);
            Assert.IsNotNull(bowPointData2);
            Assert.IsNotNull(bowPointData3);

            return bowTotalPointData;
        }

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   Measures = new List<MeasureConfigurationBase>() {
                       new MeasureBowConfiguration() {
                           MaxReferencePlaneAngle = 0.5.Degrees(),
                           DefaultReferencePlanePointsAngularPositions = new List<Angle> ()
                           {
                               0.Degrees(),
                               90.Degrees(),
                               180.Degrees(),
                               270.Degrees()
                           },
                           DefaultReferencePlanePointsDistanceFromWaferEdge = 7.Millimeters(),
                           ReferencePlanePointsRotationWhenDefaultUnreachable = 1.Degrees(),
                       }
                   }
               });
        }

        protected override void PostGenericSetup()
        {
            // By default we have an open chuck (may be specialized later in individual tests)
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
        }

        private MeasureContext CreateMeasureContext(int siteId, double x, double y)
        {
            var newMeasurePoint = new MeasurePoint(siteId, x, y, false);
            newMeasurePoint.IsSubMeasurePoint = true;
            return new MeasureContext(newMeasurePoint, null, null);
        }

        private void AssertXYPositionIs(double expectedX, double expectedY, XYPosition position)
        {
            Assert.AreEqual(expectedX, position.X, Precision);
            Assert.AreEqual(expectedY, position.Y, Precision);
        }

        private void AssertXYPositionIsWithRotation(double expectedX, double expectedY, Angle rotation, XYPosition position)
        {
            var center = new XYPosition(new WaferReferential(), 0, 0);
            var rotatedPosition = new XYPosition(new WaferReferential(), expectedX, expectedY);
            MathTools.ApplyAntiClockwiseRotation(rotation, rotatedPosition, center);
            AssertXYPositionIs(rotatedPosition.X, rotatedPosition.Y, position);
        }

        [TestMethod]
        public void MeasureBow_GetMeasureTools_NominalCase()
        {
            // Given default settings
            var measureSettings = CreateBowSettings();
            // Given default config (see SpecializeRegister)

            // When geting measure tools
            var measure = new MeasureBow();
            var measureTools = measure.GetMeasureTools(measureSettings) as BowMeasureTools;

            // Then default reference points are as expected
            Assert.AreEqual(4, measureTools.DefaultReferencePlanePositions.Count);
            AssertXYPositionIs(143, 0, measureTools.DefaultReferencePlanePositions[0]);
            AssertXYPositionIs(0, 143, measureTools.DefaultReferencePlanePositions[1]);
            AssertXYPositionIs(-143, 0, measureTools.DefaultReferencePlanePositions[2]);
            AssertXYPositionIs(0, -143, measureTools.DefaultReferencePlanePositions[3]);
        }

        [TestMethod]
        public void MeasureBow_GetMeasureTools_UnreachableDefaultPinsAreAvoided()
        {
            // Given settings that refer to unreachable default point (under pins)
            var measureSettings = CreateBowSettings();
            measureSettings.ProbeSettings.ProbeId = LiseBottomId;
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given default config (see SpecializeRegister)

            // When geting measure tools
            var measure = new MeasureBow();
            var measureTools = measure.GetMeasureTools(measureSettings) as BowMeasureTools;

            // Then default reference points are rotated as expected
            Assert.AreEqual(4, measureTools.DefaultReferencePlanePositions.Count);
            AssertXYPositionIsWithRotation(143, 0, 1.Degrees(), measureTools.DefaultReferencePlanePositions[0]);
            AssertXYPositionIsWithRotation(0, 143, 1.Degrees(), measureTools.DefaultReferencePlanePositions[1]);
            AssertXYPositionIsWithRotation(-143, 0, 1.Degrees(), measureTools.DefaultReferencePlanePositions[2]);
            AssertXYPositionIsWithRotation(0, -143, 1.Degrees(), measureTools.DefaultReferencePlanePositions[3]);
        }

        [TestMethod]
        public void MeasureBow_NominalCase()
        {
            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
            // Given air gaps on measure point and reference plane points
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.Success, bowTotalPointData.State);
            Assert.AreEqual(1.0, bowTotalPointData.Bow.Millimeters, Precision);
        }

        [TestMethod]
        public void MeasureBow_ResultOutOfTolerance()
        {
            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
            // Given air gaps on measure point and reference plane points
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(1.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(6.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(6.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(6.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.Error, bowTotalPointData.State);
            Assert.AreEqual(5.0, bowTotalPointData.Bow.Millimeters, Precision);
        }

        [TestMethod]
        public void MeasureBow_PlaneAngleTooBigReturnsErrorStatus()
        {
            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
            // Given air gaps on measure point and reference plane points that create a steep plane
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(1.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(5.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(9.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            var bowSettings = CreateBowSettings();

            //When measure is executed
            var measure = new MeasureBow();
            var subMeasuresResults = new List<MeasurePointResult>();
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(1, 0d, 0d)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(2, 0d, 143.650)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(3, -124.405, -71.825)));
            subMeasuresResults.Add(measure.ExecuteSubMeasure(bowSettings, CreateMeasureContext(4, 124.405, 31.825)));
            var measureResult = measure.ComputeMeasureFromSubMeasures(bowSettings, CreateMeasureContext(1, 0d, 0d), subMeasuresResults);
            var bowPointData1 = subMeasuresResults[0].Datas[0] as BowPointData;
            var bowPointData2 = subMeasuresResults[1].Datas[0] as BowPointData;
            var bowPointData3 = subMeasuresResults[2].Datas[0] as BowPointData;
            var bowTotalPointData = measureResult.Datas[0] as BowTotalPointData;
            Assert.IsNotNull(bowPointData1);
            Assert.IsNotNull(bowPointData2);
            Assert.IsNotNull(bowPointData3);

            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.NotMeasured, bowTotalPointData.State);
            Assert.IsTrue(bowTotalPointData.Message.Contains("Plane angle exceeds the configuration limit"));
        }

        [TestMethod]
        public void MeasureBow_NonOpenChuckInitiallyClampedUnclampsWhenAirGap()
        {
            // Given air gaps on measure point and reference plane points
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // Given chuck is a non open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            // Then clamps/unclamps 3 times
            SimulatedChuck.Verify(_ => _.ClampWafer(300.Millimeters()), Times.Exactly(4));
            SimulatedChuck.Verify(_ => _.ReleaseWafer(300.Millimeters()), Times.Exactly(4));

            // Then result is expected
            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.Success, bowTotalPointData.State);
            Assert.AreEqual(1.0, bowTotalPointData.Bow.Millimeters, Precision);
        }

        [TestMethod]
        public void MeasureBow_NonOpenChuckInitiallyUnclampedDoesNothingSpecialWhenAirGap()
        {
            // Given air gaps on measure point and reference plane points
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // Given chuck is a non open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            // Given chuck is initially unclamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), false);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            // Then clamps/unclamps 3 times
            SimulatedChuck.Verify(_ => _.ClampWafer(300.Millimeters()), Times.Never());
            SimulatedChuck.Verify(_ => _.ReleaseWafer(300.Millimeters()), Times.Never());

            // Then result is expected
            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.Success, bowTotalPointData.State);
            Assert.AreEqual(1.0, bowTotalPointData.Bow.Millimeters, Precision);
        }

        [TestMethod]
        public void MeasureBow_OpenChuckInitiallyClampedDoesNothingSpecialWhenAirGap()
        {
            // Given air gaps on measure point and reference plane points
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(4.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            // Then clamps/unclamps 3 times
            SimulatedChuck.Verify(_ => _.ClampWafer(300.Millimeters()), Times.Never());
            SimulatedChuck.Verify(_ => _.ReleaseWafer(300.Millimeters()), Times.Never());

            // Then result is expected
            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.Success, bowTotalPointData.State);
            Assert.AreEqual(1.0, bowTotalPointData.Bow.Millimeters, Precision);
        }

        [TestMethod]
        public void MeasureBow_AirGapFailureReturnsNotMeasured()
        {
            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given chuck is initially clamped
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
            // Given air gaps on measure point and reference plane points, with one null signal
            var signals = new List<IProbeSignal>()
            {
                // Measure point signal
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(3.Millimeters()),
                // Reference plane points signals
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(5.Millimeters()),
                LiseTestUtils.CreateNullLiseSignal(),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(5.Millimeters())
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            var bowTotalPointData = PrepareAndExecuteMeasureBow();

            Assert.IsNotNull(bowTotalPointData);
            Assert.AreEqual(MeasureState.NotMeasured, bowTotalPointData.State);
            Assert.IsTrue(bowTotalPointData.Message.Contains("Air Gap Error"));
        }
    }
}
