using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using Moq;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureEdgeTrimTest : TestWithMockedHardware<MeasureEdgeTrimTest>, ITestWithAxes, ITestWithProbeLise
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
                        MeasureType.EdgeTrim
                   },
                   Measures = new List<MeasureConfigurationBase>() {
                       new MeasureEdgeTrimConfiguration()
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

        private EdgeTrimSettings CreateEdgeTrimSettings()
        {
            return new EdgeTrimSettings()
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

                PreEdgeScanSize = 0.25.Millimeters(),
                PostEdgeScanSize = 0.3.Millimeters(),
                StepSize = 50.Micrometers(),

                TopEdgeExclusionSize = 0.075.Millimeters(),
                BottomEdgeExclusionSize = 0.075.Millimeters(),

                HeightTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                HeightTarget = 150.Micrometers(),

                IsWidthMeasured = true,
                WidthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                WidthTarget = 200.Micrometers(),
            };
        }

        private EdgeTrimPointData ExecuteMeasure(EdgeTrimSettings edgeTrimSettings)
        {
            var edgeTrimContext = CreateContext();

            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.EdgeTrim);

            var measureResult = measureLoad.Execute(edgeTrimSettings, edgeTrimContext) as EdgeTrimPointResult;
            return measureResult.Datas[0] as EdgeTrimPointData;
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
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(560.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(610.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var edgeTrimSettings = CreateEdgeTrimSettings();
            var edgeTrimPointData = ExecuteMeasure(edgeTrimSettings);

            // Then
            Assert.IsNotNull(edgeTrimPointData);
            Assert.AreEqual(MeasureState.Success, edgeTrimPointData.State);
            Assert.IsTrue(edgeTrimSettings.HeightTolerance.IsInTolerance(edgeTrimPointData.Height, edgeTrimSettings.HeightTarget));
            Assert.IsTrue(edgeTrimSettings.WidthTolerance.IsInTolerance(edgeTrimPointData.Width, edgeTrimSettings.WidthTarget));
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
            var edgeTrimSettings = CreateEdgeTrimSettings();
            var edgeTrimPointData = ExecuteMeasure(edgeTrimSettings);

            // Then
            Assert.IsNotNull(edgeTrimPointData);
            Assert.AreEqual(MeasureState.NotMeasured, edgeTrimPointData.State);
        }

        [TestMethod]
        public void HeightOutOfTolerance()
        {
            // Given
            var signals = new List<IProbeSignal>()
            {
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(510.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(510.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(510.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(510.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(510.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(560.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(610.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var edgeTrimSettings = CreateEdgeTrimSettings();
            var edgeTrimPointData = ExecuteMeasure(edgeTrimSettings);

            // Then
            Assert.IsNotNull(edgeTrimPointData);
            Assert.AreEqual(MeasureState.Error, edgeTrimPointData.State);
            Assert.IsFalse(edgeTrimSettings.HeightTolerance.IsInTolerance(edgeTrimPointData.Height, edgeTrimSettings.HeightTarget));
            Assert.IsTrue(edgeTrimSettings.WidthTolerance.IsInTolerance(edgeTrimPointData.Width, edgeTrimSettings.WidthTarget));
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
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(500.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(560.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(610.Micrometers()),

                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
                LiseTestUtils.CreateLiseSignalWithOnlyAirGap(650.Micrometers()),
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When
            var edgeTrimSettings = CreateEdgeTrimSettings();
            var edgeTrimPointData = ExecuteMeasure(edgeTrimSettings);

            // Then
            Assert.IsNotNull(edgeTrimPointData);
            Assert.AreEqual(MeasureState.Error, edgeTrimPointData.State);
            Assert.IsTrue(edgeTrimSettings.HeightTolerance.IsInTolerance(edgeTrimPointData.Height, edgeTrimSettings.HeightTarget));
            Assert.IsFalse(edgeTrimSettings.WidthTolerance.IsInTolerance(edgeTrimPointData.Width, edgeTrimSettings.WidthTarget));
        }
    }
}
