using System.Collections.Generic;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy.Test
{
    [TestClass]
    public class UnitTestProbeLise
    {
        #region Fields

        private readonly double _positionningAccuracy = 0.001; // mm

        #endregion Fields

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            Thread.Sleep(4000); // Wait for ACS controller to park LOH and UOH at start
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var axesState = axesSupervisor.GetCurrentState()?.Result;

            if (axesState.Landed == true)
                axesSupervisor.StopLanding();

            axesState = axesSupervisor.GetCurrentState()?.Result;
        }

        private static DualLiseCalibParams CreateDualLiseCalibParams()
        {
            var inputCalib = new DualLiseCalibParams();
            var thickness = 750.46.Micrometers();
            var tolerance = 5.0.Micrometers();
            var refractionIndex = (float)1.4621;
            inputCalib.ProbeCalibrationReference = new OpticalReferenceDefinition() { RefThickness = thickness, RefTolerance = tolerance, RefRefrIndex = refractionIndex };

            inputCalib.TopLiseAirgapThreshold = 0.6;
            inputCalib.BottomLiseAirgapThreshold = 0.7;
            inputCalib.CalibrationMode = 3;
            inputCalib.NbRepeatCalib = 16;

            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            var currentPos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            inputCalib.ZTopUsedForCalib = currentPos.ZTop;
            inputCalib.ZBottomUsedForCalib = currentPos.ZBottom;

            return inputCalib;
        }

        private static SingleLiseInputParams CreateProbeInputParametersLise()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 16;
            double gain = 1.8;

            var probeInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);

            return probeInputParameters;
        }

        private static DualLiseInputParams CreateProbeInputParametersLiseDoubleForCalib()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 0);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 6;
            double gain = 1.8;

            var probeUpInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeDownInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeInputParameters = new DualLiseInputParams(sample, probeUpInputParameters, probeDownInputParameters);

            return probeInputParameters;
        }

        private static DualLiseInputParams CreateProbeInputParametersLiseDouble()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            ProbeSample sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 6;
            double gain = 1.8;

            var probeUpInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeDownInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeInputParameters = new DualLiseInputParams(sample, probeUpInputParameters, probeDownInputParameters);

            return probeInputParameters;
        }

        private void GotoHome()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            axesSupervisor.GoToHome(AxisSpeed.Measure);
            Thread.Sleep(2000);
            axesSupervisor.WaitMotionEnd(15000);
        }

        private void GotoRef()
        {
            var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            var referencePosition = new XYZTopZBottomPosition(new StageReferential(), -208.8955, 102.7758, 13.0992, 0.2366);
            axesSupervisor.GotoPosition(referencePosition, AxisSpeed.Normal);
            Thread.Sleep(5000);
            axesSupervisor.WaitMotionEnd(15000);

            var realPos = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
            Assert.IsTrue(realPos.X.Near(referencePosition.X, _positionningAccuracy), "RealPos.X: " + realPos.X + "  referencePosition.X: " + referencePosition.X);
            Assert.IsTrue(realPos.Y.Near(referencePosition.Y, _positionningAccuracy));
            Assert.IsTrue(realPos.ZTop.Near(referencePosition.ZTop, _positionningAccuracy));
            Assert.IsTrue(realPos.ZBottom.Near(referencePosition.ZBottom, _positionningAccuracy));

            var probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            var _objectivesSelectors = probesSupervisor.ObjectivesSelectors;
            var objectiveSelector = _objectivesSelectors.Find(selector => selector.FindObjective("ID-5XNIR01") != null);
            Assert.IsNotNull(objectiveSelector);
            Assert.IsTrue(probesSupervisor.SetNewObjectiveToUse(objectiveSelector.DeviceID, "ID-5XNIR01")?.Result);
        }

        [TestMethod]
        public void TestDoMeasureSimpleAndDouble()
        {
            // Arrange
            var probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            var allProbes = probesSupervisor.Probes;

            GotoHome();

            foreach (var probe in allProbes)
            {
                GotoRef();
                if (probe is ProbeLiseVM)
                {
                    var liseProbeInputParameters = CreateProbeInputParametersLise();
                    const int totalMeasureNb = 10;
                    for (int measureNb = 0; measureNb < totalMeasureNb; measureNb++)
                    {
                        // Act
                        var response = probesSupervisor.DoMeasure(probe.Configuration.DeviceID, liseProbeInputParameters);

                        Assert.IsNotNull((LiseResult)response.Result);
                        var layersThickness = ((LiseResult)response.Result).LayersThickness;

                        // Assert
                        Assert.IsNotNull(response.Result);
                        Assert.AreEqual(1, layersThickness.Count);
                        double thickness = layersThickness[0].Thickness.Nanometers;
                        double quality = layersThickness[0].Quality;
                        const double expectedThickness = 750.0;

                        Assert.IsTrue(thickness.Near(expectedThickness, 5.0));
                        Assert.IsTrue(quality > 0.0);
                    }
                }

                if (probe is ProbeLiseDoubleVM)
                {
                    var liseDoubleProbeInputParameters = CreateProbeInputParametersLiseDouble();
                    int totalMeasureNb = 10;
                    for (int measureNb = 0; measureNb < totalMeasureNb; measureNb++)
                    {
                        // Act
                        var response = probesSupervisor.DoMeasure(probe.Configuration.DeviceID, liseDoubleProbeInputParameters);
                        var layersThickness = ((LiseResult)response.Result).LayersThickness;

                        // Assert
                        Assert.IsNotNull(response.Result);
                        Assert.AreEqual(1, layersThickness.Count);
                        double thickness = layersThickness[0].Thickness.Nanometers;
                        double quality = layersThickness[0].Quality;
                        const double expectedThickness = 750.0;

                        Assert.IsTrue(thickness.Near(expectedThickness, 5.0));

                        if (probe.Configuration.ModulePosition == Shared.Hardware.Service.Interface.ModulePositions.Up)
                            Assert.IsTrue(quality > liseDoubleProbeInputParameters.ProbeUpParams.QualityThreshold);
                        else
                            Assert.IsTrue(quality > liseDoubleProbeInputParameters.ProbeDownParams.QualityThreshold);
                    }
                }
            }
        }

        [TestMethod]
        public void TestStartAcquisitionSimpleAndDouble()
        {
            // Arrange

            var probesSupervisorMock = new Mock<ProbesSupervisor>(ClassLocator.Default.GetInstance<ILogger<ProbesSupervisor>>(), ClassLocator.Default.GetInstance<IProbesFactory>(), ClassLocator.Default.GetInstance<IMessenger>());
            var probesSupervisor = probesSupervisorMock.Object;
            var allProbes = probesSupervisor.Probes;
            var liseProbeInputParameters = CreateProbeInputParametersLise();
            var liseDoubleProbeInputParameters = CreateProbeInputParametersLiseDouble();
            bool probeLiseFound = false;

            foreach (var probe in allProbes)
            {
                if (probe is ProbeLiseVM)
                {
                    probeLiseFound = true;
                    probesSupervisor.StartContinuousAcquisition(probe.DeviceID, liseProbeInputParameters);
                    Thread.Sleep(2000);

                    probesSupervisorMock.Verify(x => x.ProbeRawMeasuresCallback(It.Is<ProbeLiseSignal>(i => i.RawValues.Count > 3000)), Times.AtLeastOnce());

                    //Assert.IsTrue((probe as ProbeLiseVM).LastProbeRawSignalReceived.RawValues.Count > 3000);
                    probesSupervisor.StopContinuousAcquisition(probe.DeviceID);
                }

                if (probe is ProbeLiseDoubleVM)
                {
                    probeLiseFound = true;
                    probesSupervisor.StartContinuousAcquisition(probe.DeviceID, liseDoubleProbeInputParameters);
                    Thread.Sleep(2000);

                    probesSupervisorMock.Verify(x => x.ProbeRawMeasuresCallback(It.Is<ProbeLiseSignal>(i => i.RawValues.Count > 3000)), Times.AtLeastOnce());

                    //Assert.IsTrue((probe as ProbeLiseVM).LastProbeRawSignalReceived.RawValues.Count > 3000);
                    probesSupervisor.StopContinuousAcquisition(probe.DeviceID);
                }

                Assert.IsTrue(probeLiseFound);
            }
        }

        [TestMethod]
        public void TestDoMultipleMeasures()
        {
            var probesSupervisorMock = new Mock<ProbesSupervisor>(ClassLocator.Default.GetInstance<ILogger<ProbesSupervisor>>(), ClassLocator.Default.GetInstance<IProbesFactory>(), ClassLocator.Default.GetInstance<IMessenger>());
            var probesSupervisor = probesSupervisorMock.Object;
            bool probeLiseFound = false;
            var liseProbeInputParameters = CreateProbeInputParametersLise();
            foreach (var probe in probesSupervisor.Probes)
            {
                GotoRef();
                if (probe is ProbeLiseVM)
                {
                    probeLiseFound = true;
                    probesSupervisor.DoMultipleMeasures(probe.DeviceID, liseProbeInputParameters, 10);
                    Thread.Sleep(1000);
                }
                //probesSupervisorMock.Verify(x => x.ProbeNewThicknessMeasuresCallback(probe.DeviceID, It.Is<List<IProbeResult>>(i => i.Count> 50)), Times.AtLeastOnce());
            }
            Assert.IsTrue(probeLiseFound);
        }

        [TestMethod]
        public void TestCalibration()
        {
            GotoHome();

            var probesSupervisorMock = new Mock<ProbesSupervisor>(ClassLocator.Default.GetInstance<ILogger<ProbesSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>());
            var probesSupervisor = probesSupervisorMock.Object;
            var allProbes = probesSupervisor.Probes;
            var dualLiseCalibParams = CreateDualLiseCalibParams();
            var liseDoubleProbeInputParameters = CreateProbeInputParametersLiseDoubleForCalib();
            bool probeLiseFound = false;

            foreach (var probe in allProbes)
            {
                if (probe is ProbeLiseDoubleVM)
                {
                    probeLiseFound = true;
                    probesSupervisor.StartCalibration(probe.DeviceID, dualLiseCalibParams, liseDoubleProbeInputParameters);
                    Thread.Sleep(1000);
                }
            }
            Assert.IsTrue(probeLiseFound);
            Thread.Sleep(10000);
        }

        [TestMethod]
        public void TestObjectiveSelector()
        {
            var probesSupervisorMock = new Mock<ProbesSupervisor>(ClassLocator.Default.GetInstance<ILogger<ProbesSupervisor>>(), ClassLocator.Default.GetInstance<IProbesFactory>(), ClassLocator.Default.GetInstance<IMessenger>());
            var probesSupervisor = probesSupervisorMock.Object;
            var _objectivesSelectors = probesSupervisor.ObjectivesSelectors;

            foreach (var objectiveSelector in _objectivesSelectors)
                foreach (var objective in objectiveSelector.Objectives)
                    probesSupervisor.SetNewObjectiveToUse(objectiveSelector.DeviceID, objective.DeviceID);
        }

        [TestMethod]
        public void TestObjectiveSelectorWaitMotionEndCallback()
        {
            var probesSupervisorMock = new Mock<ProbesSupervisor>(ClassLocator.Default.GetInstance<ILogger<ProbesSupervisor>>(), ClassLocator.Default.GetInstance<IProbesFactory>(), ClassLocator.Default.GetInstance<IMessenger>());
            var probesSupervisor = probesSupervisorMock.Object;
            var _objectivesSelectors = probesSupervisor.ObjectivesSelectors;
            int nbOfObjectivesChanged = 0;

            foreach (var objectiveSelector in _objectivesSelectors)
                foreach (var objective in objectiveSelector.Objectives)
                {
                    probesSupervisor.SetNewObjectiveToUse(objectiveSelector.DeviceID, objective.DeviceID);
                    nbOfObjectivesChanged++;
                    Thread.Sleep(3000);
                }
            probesSupervisorMock.Verify(x => x.ProbeNewObjectiveInUseCallback(It.Is<ObjectiveResult>(i => i.Success == true)), Times.AtLeast(nbOfObjectivesChanged));
        }
    }
}
