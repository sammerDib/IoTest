using System.Collections.Generic;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Warp;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureWarpTest : TestWithMockedHardware<MeasureWarpTest>, ITestWithAxes, ITestWithProbeLise, ITestWithChuck, ITestWithCamera
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

        private WarpSettings CreateWarpSettings(bool withDualLise)
        {
            ProbeSettings probeSettings = null;
            if (withDualLise)
            {
                probeSettings = new DualLiseSettings()
                {
                    ProbeId = DualLiseId,
                    LiseUp = new SingleLiseSettings()
                    {
                        ProbeId = LiseUpId,
                        LiseGain = 1.8,
                        ProbeObjectiveContext = new Interface.Context.ObjectiveContext()
                        {
                            ObjectiveId = ObjectiveUpId
                        }
                    },
                    LiseDown = new SingleLiseSettings()
                    {
                        ProbeId = LiseBottomId,
                        LiseGain = 1.8,
                        ProbeObjectiveContext = new Interface.Context.ObjectiveContext()
                        {
                            ObjectiveId = ObjectiveBottomId
                        }
                    }
                };
            }
            else
            {
                probeSettings = new SingleLiseSettings()
                {
                    ProbeId = LiseUpId,
                    LiseGain = 1.8,
                    ProbeObjectiveContext = new Interface.Context.ObjectiveContext()
                    {
                        ObjectiveId = ObjectiveUpId
                    }
                };
            }

            var newWarpSettings = new WarpSettings()
            {
                ProbeSettings = probeSettings,
                NbOfRepeat = 1,

                WarpMax = 20.Micrometers(),

                SubMeasurePoints = new List<int>(),
                IsMeasureWithSubMeasurePoints = true,

                WaferCharacteristic = new WaferDimensionalCharacteristic()
                {
                    Diameter = 300.Millimeters(),
                },

                PhysicalLayers = new List<LayerSettings>()
            };

            return newWarpSettings;
        }

        private WarpTotalPointData PrepareAndExecuteMeasureWarp(bool withDualLise)
        {
            var warpSettings = CreateWarpSettings(withDualLise);

            //When measure is executed
            var measure = new MeasureWarp();
            var subMeasuresResults = new List<MeasurePointResult>
            {
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(1, 0d, 0d)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(2, 143.650, 0d)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(3, 101.576, 101.576)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(4, 0d, 143.650)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(5, -101.576, 101.576)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(6, 143.650, 0d)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(7, -101.576, -101.576)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(8, 0d, 143.650)),
                measure.ExecuteSubMeasure(warpSettings, CreateMeasureContext(9, 101.576, 101.576))
            };
            var measureResult = measure.ComputeMeasureFromSubMeasures(warpSettings, CreateMeasureContext(1, 0d, 0d), subMeasuresResults);
            var warpTotalPointData = measureResult.Datas[0] as WarpTotalPointData;
            Assert.IsNotNull(warpTotalPointData);

            return warpTotalPointData;
        }

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   Measures = new List<MeasureConfigurationBase>() {
                       new MeasureWarpConfiguration() {
                           DefaultReferencePlanePointsAngularPositions = new List<Angle> ()
                           {
                               0.Degrees(),
                               45.Degrees(),
                               90.Degrees(),
                               135.Degrees(),
                               180.Degrees(),
                               225.Degrees(),
                               270.Degrees(),
                               310.Degrees()
                           },
                           DefaultReferencePlanePointsDistanceFromWaferEdge = 6.5.Millimeters(),
                           ReferencePlanePointsRotationWhenDefaultUnreachable = 1.Degrees(),
                           ReleaseWaferTimeoutMilliseconds = 1200,
                           DualLiseTotalThicknessValidityFactor = 1.5
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
            var newMeasurePoint = new MeasurePoint(siteId, x, y, false)
            {
                IsSubMeasurePoint = true
            };
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
        public void MeasureWarp_GetMeasureTools_NominalCase()
        {
            // Given default settings
            var measureSettings = CreateWarpSettings(true);

            // Given default config (see SpecializeRegister)

            // When geting measure tools
            var measure = new MeasureWarp();
            var measureTools = measure.GetMeasureTools(measureSettings) as WarpMeasureTools;

            // Then default reference points are as expected
            Assert.AreEqual(8, measureTools.DefaultReferencePlanePositions.Count);
            AssertXYPositionIs(143.5, 0d, measureTools.DefaultReferencePlanePositions[0]);
            AssertXYPositionIs(101.470, 101.470, measureTools.DefaultReferencePlanePositions[1]);
            AssertXYPositionIs(0d, 143.5, measureTools.DefaultReferencePlanePositions[2]);
            AssertXYPositionIs(-101.470, 101.470, measureTools.DefaultReferencePlanePositions[3]);
            AssertXYPositionIs(-143.5, 0d, measureTools.DefaultReferencePlanePositions[4]);
            AssertXYPositionIs(-101.470, -101.470, measureTools.DefaultReferencePlanePositions[5]);
            AssertXYPositionIs(0d, -143.5, measureTools.DefaultReferencePlanePositions[6]);
            AssertXYPositionIs(92.240, -109.927, measureTools.DefaultReferencePlanePositions[7]);
        }

        [TestMethod]
        public void MeasureWarp_GetMeasureTools_UnreachableDefaultPinsAreAvoided()
        {
            // Given settings that refer to unreachable default point (under pins)
            var measureSettings = CreateWarpSettings(true);
            measureSettings.ProbeSettings.ProbeId = LiseBottomId;
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;
            // Given default config (see SpecializeRegister)

            // When geting measure tools
            var measure = new MeasureWarp();
            var measureTools = measure.GetMeasureTools(measureSettings) as WarpMeasureTools;

            // Then default reference points are rotated as expected
            Assert.AreEqual(8, measureTools.DefaultReferencePlanePositions.Count);
            AssertXYPositionIsWithRotation(143.5, 0d, 1.Degrees(), measureTools.DefaultReferencePlanePositions[0]);
            AssertXYPositionIsWithRotation(101.470, 101.470, 1.Degrees(), measureTools.DefaultReferencePlanePositions[1]);
            AssertXYPositionIsWithRotation(0d, 143.5, 1.Degrees(), measureTools.DefaultReferencePlanePositions[2]);
            AssertXYPositionIsWithRotation(-101.470, 101.470, 1.Degrees(), measureTools.DefaultReferencePlanePositions[3]);
            AssertXYPositionIsWithRotation(-143.5, 0d, 1.Degrees(), measureTools.DefaultReferencePlanePositions[4]);
            AssertXYPositionIsWithRotation(-101.470, -101.470, 1.Degrees(), measureTools.DefaultReferencePlanePositions[5]);
            AssertXYPositionIsWithRotation(0d, -143.5, 1.Degrees(), measureTools.DefaultReferencePlanePositions[6]);
            AssertXYPositionIsWithRotation(92.240, -109.927, 1.Degrees(), measureTools.DefaultReferencePlanePositions[7]);
        }

        [TestMethod, Ignore("Warp measure with DualLise in simulated mode is irrelevant and always returns NotMeasured")]
        public void MeasureWarp_DualLise_NominalCase()
        {
            var token = new CancellationToken();
            uint minuteBetweenTwoDualLiseCalibration = 10;
            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble", token, minuteBetweenTwoDualLiseCalibration);
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown }, this);

            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;

            // Given chuck is initially clamped
            var clampStates = new Dictionary<Length, bool> { { new Length(300, LengthUnit.Millimeter), true } };
            var presenceStates = new Dictionary<Length, MaterialPresence> { { new Length(300, LengthUnit.Millimeter), MaterialPresence.Present } };
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var length1microm = 1.Micrometers();
            var airGapForCalibration = 5.Millimeters();
            // Given air gaps on measure point and reference plane points
            var signalsUp = new List<IProbeSignal>()
            {
                // Reference plane points signalsUp
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 2 * length1microm, AirGapDown + 2 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 4 * length1microm, AirGapDown + 4 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 6 * length1microm, AirGapDown + 6 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 8 * length1microm, AirGapDown + 8 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 10 * length1microm, AirGapDown + 10 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 12 * length1microm, AirGapDown + 12 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 14 * length1microm, AirGapDown + 14 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 16 * length1microm, AirGapDown + 16 * length1microm).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 18 * length1microm, AirGapDown + 18 * length1microm).SignalLiseUp
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsUp, this);

            // Given air gaps on measure point and reference plane points
            var signalsDown = new List<IProbeSignal>()
            {
                // Reference plane points signalsDown
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, airGapForCalibration, airGapForCalibration).SignalLiseDown,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown).SignalLiseDown
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalsDown, this);

            var warpTotalPointData = PrepareAndExecuteMeasureWarp(true);

            Assert.IsNotNull(warpTotalPointData);
            Assert.AreEqual(MeasureState.Success, warpTotalPointData.State);
            Assert.AreEqual(7.671, warpTotalPointData.Warp.Micrometers, Precision);
            Assert.AreEqual(15.6, warpTotalPointData.TTV.Micrometers, Precision);
        }

        [TestMethod]
        public void MeasureWarp_ToplLise_NominalCase()
        {
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);

            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;

            // Given chuck is initially clamped
            var clampStates = new Dictionary<Length, bool> { { new Length(300, LengthUnit.Millimeter), true } };
            var presenceStates = new Dictionary<Length, MaterialPresence> { { new Length(300, LengthUnit.Millimeter), MaterialPresence.Present } };
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var length1microm = 1.Micrometers();
            var airGapForCalibration = 5.Millimeters();

            // Given air gaps on measure point and reference plane points
            var signalsUp = new List<IProbeSignal>()
            {
                // Reference plane points signalsUp
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 2 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 4 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 6 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 8 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 10 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 12 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 14 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 16 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 18 * length1microm, AirGapDown).SignalLiseUp
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsUp, this);

            var warpTotalPointData = PrepareAndExecuteMeasureWarp(false);

            Assert.IsNotNull(warpTotalPointData);
            Assert.AreEqual(MeasureState.Success, warpTotalPointData.State);
            Assert.AreEqual(15.602, warpTotalPointData.Warp.Micrometers, Precision);
        }

        [TestMethod]
        public void MeasureWarp_ToplLise_OutOfTolerance()
        {
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);

            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;

            // Given chuck is initially clamped
            var clampStates = new Dictionary<Length, bool> { { new Length(300, LengthUnit.Millimeter), true } };
            var presenceStates = new Dictionary<Length, MaterialPresence> { { new Length(300, LengthUnit.Millimeter), MaterialPresence.Present } };
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var length1microm = 1.Micrometers();
            var airGapForCalibration = 5.Millimeters();

            // Given air gaps on measure point and reference plane points
            var signalsUp = new List<IProbeSignal>()
            {
                // Reference plane points signalsUp
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 2 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 8 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 12 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 16 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 20 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 24 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 28 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 32 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 36 * length1microm, AirGapDown).SignalLiseUp
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsUp, this);

            var warpTotalPointData = PrepareAndExecuteMeasureWarp(false);

            Assert.IsNotNull(warpTotalPointData);
            Assert.AreEqual(MeasureState.Error, warpTotalPointData.State);
            Assert.IsTrue(warpTotalPointData.Warp.Micrometers > 20d);
        }

        [TestMethod]
        public void MeasureWarp_ToplLise_NotEnoughSubMeasures()
        {
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);

            // Given chuck is an open chuck
            SimulatedChuck.Object.Configuration.IsOpenChuck = true;

            // Given chuck is initially clamped
            var clampStates = new Dictionary<Length, bool> { { new Length(300, LengthUnit.Millimeter), true } };
            var presenceStates = new Dictionary<Length, MaterialPresence> { { new Length(300, LengthUnit.Millimeter), MaterialPresence.Present } };
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));

            var length1microm = 1.Micrometers();
            var airGapForCalibration = 5.Millimeters();

            // Given air gaps on measure point and reference plane points
            var signalsUp = new List<IProbeSignal>()
            {
                // Reference plane points signalsUp
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 2 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 4 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 6 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 8 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 10 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 12 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 14 * length1microm, AirGapDown).SignalLiseUp,
                CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 16 * length1microm, AirGapDown).SignalLiseUp
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsUp, this);

            var warpTotalPointData = PrepareAndExecuteMeasureWarp(false);

            Assert.IsNotNull(warpTotalPointData);
            Assert.AreEqual(MeasureState.NotMeasured, warpTotalPointData.State);
            Assert.IsTrue(warpTotalPointData.Warp is null);
        }
    }
}
