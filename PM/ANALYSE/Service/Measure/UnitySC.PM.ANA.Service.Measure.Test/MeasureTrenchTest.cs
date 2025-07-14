using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Tolerances;

using UnitySC.Shared.Tools.Units;

using UnitySC.Shared.Tools;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureTrenchTest : TestWithMockedHardware<MeasureTrenchTest>, ITestWithAxes, ITestWithProbeLise
    {
        #region params
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
        #endregion

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   AuthorizedMeasures = new List<MeasureType> {
                        MeasureType.Trench
                   },
                   Measures = new List<MeasureConfigurationBase>() {
                       new MeasureTrenchConfiguration()
                   }
               });
        }

        private MeasureContext CreateContext()
        {
            return new MeasureContext(
                new MeasurePoint(1, 150, 0, false),
                null,
                null);
        }

        private TrenchSettings CreateTrenchSettings()
        {
            return new TrenchSettings()
            {
                NbOfRepeat = 1,

                ProbeSettings = new SingleLiseSettings()
                {
                    ProbeId = LiseUpId,
                    LiseGain = 1.8,
                    ProbeObjectiveContext = new Interface.Context.ObjectiveContext()
                    {
                        ObjectiveId = ObjectiveUpId
                    }
                },

                ScanSize = 0.65.Millimeters(),
                StepSize = 50.Micrometers(),
                ScanAngle = 0.0.Degrees(),

                TopEdgeExclusionSize = 0.075.Millimeters(),
                BottomEdgeExclusionSize = 0.075.Millimeters(),

                DepthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                DepthTarget = 300.Micrometers(),

                IsWidthMeasured = true,
                WidthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                WidthTarget = 150.Micrometers(),
            };
        }

        private TrenchPointData ExecuteMeasure(TrenchSettings trenchSettings)
        {
            var trenchContext = CreateContext();

            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Trench);

            var measureResult = measureLoad.Execute(trenchSettings, trenchContext) as TrenchPointResult;
            return measureResult.Datas[0] as TrenchPointData;
        }

        [TestMethod]
        public void NominalCase()
        {
            // Given
            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(600.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(700.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var trenchSettings = CreateTrenchSettings();
            var trenchPointData = ExecuteMeasure(trenchSettings);

            // Then
            Assert.IsNotNull(trenchPointData);
            Assert.AreEqual(MeasureState.Success, trenchPointData.State);
            Assert.IsTrue(trenchSettings.DepthTolerance.IsInTolerance(trenchPointData.Depth, trenchSettings.DepthTarget));
            Assert.IsTrue(trenchSettings.WidthTolerance.IsInTolerance(trenchPointData.Width, trenchSettings.WidthTarget));
        }

        [TestMethod]
        public void NotEnoughSignal()
        {
            // Given
            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var trenchSettings = CreateTrenchSettings();
            var trenchPointData = ExecuteMeasure(trenchSettings);

            // Then
            Assert.IsNotNull(trenchPointData);
            Assert.AreEqual(MeasureState.NotMeasured, trenchPointData.State);
        }

        [TestMethod]
        public void DepthOutOfTolerance()
        {
            // Given
            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(600.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(795.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(795.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(795.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(795.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(700.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var trenchSettings = CreateTrenchSettings();
            var trenchPointData = ExecuteMeasure(trenchSettings);

            // Then
            Assert.IsNotNull(trenchPointData);
            Assert.AreEqual(MeasureState.Error, trenchPointData.State);
            Assert.IsFalse(trenchSettings.DepthTolerance.IsInTolerance(trenchPointData.Depth, trenchSettings.DepthTarget));
            Assert.IsTrue(trenchSettings.WidthTolerance.IsInTolerance(trenchPointData.Width, trenchSettings.WidthTarget));
        }

        [TestMethod]
        public void WidthOutOfTolerance()
        {
            // Given
            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(800.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var trenchSettings = CreateTrenchSettings();
            var trenchPointData = ExecuteMeasure(trenchSettings);

            // Then
            Assert.IsNotNull(trenchPointData);
            Assert.AreEqual(MeasureState.Error, trenchPointData.State);
            Assert.IsTrue(trenchSettings.DepthTolerance.IsInTolerance(trenchPointData.Depth, trenchSettings.DepthTarget));
            Assert.IsFalse(trenchSettings.WidthTolerance.IsInTolerance(trenchPointData.Width, trenchSettings.WidthTarget));
        }
    }
}
