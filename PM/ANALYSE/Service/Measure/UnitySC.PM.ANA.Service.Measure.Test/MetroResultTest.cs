using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;
using static UnitySC.PM.ANA.Service.Measure.Test.ThicknessTestUtils;
using UnitySC.PM.ANA.Hardware.Probe.Lise;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MetroResultTest : TestWithMockedHardware<MetroResultTest>,
        ITestWithProbeLise, ITestWithCamera, ITestWithAxes, ITestWithChuck
    {
        #region parameters

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
        public Mock<IAxes> SimulatedAxes { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion parameters

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
        public void For2layersGroups_WithProbeLiseUp_CreateMetroResult_NominalCase()
        {
            //Given datas input

            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var fourthLayer100 = CreateLayer(100.Micrometers(), 4);
            var fifthLayer550 = CreateLayer(550.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer200, fourthLayer100, fifthLayer550 };

            var layerGroup750With2Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId), new List<LayerSettings>() { thirdLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var layerGroups = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.CreateMetroMeasureResult(thicknessSettings);
            var res = measureResult as ThicknessResult;
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);
            Assert.AreEqual(false, res.Settings.HasWaferThicknesss);

            //Respresent group of layer with id 1 and 2
            var layer1 = res.Settings.ThicknessLayers[0];
            Assert.AreEqual(layerGroup750With2Layer.Name, layer1.Name);
            Assert.AreEqual(firstLayer200.Thickness + secondLayer550.Thickness, layer1.Target);
            Assert.AreEqual(layerGroup750With2Layer.ThicknessTolerance, layer1.Tolerance);
            Assert.AreEqual(true, layer1.IsMeasured);

            //layer with id 3
            var layer2 = res.Settings.ThicknessLayers[1];
            Assert.AreEqual(thirdLayer200.Name, layer2.Name);
            Assert.AreEqual(thirdLayer200.Thickness, layer2.Target);
            Assert.AreEqual(layerGroup200With1Layer.ThicknessTolerance, layer2.Tolerance);
            Assert.AreEqual(true, layer2.IsMeasured);

            //layer with id 4
            var layer4 = res.Settings.ThicknessLayers[2];
            Assert.AreEqual(fourthLayer100.Name, layer4.Name);
            Assert.AreEqual(fourthLayer100.Thickness, layer4.Target);
            //It is not a layer to be measured so it has no tolerance
            var expectedDefaultTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            Assert.AreEqual(expectedDefaultTolerance, layer4.Tolerance);
            Assert.AreEqual(false, layer4.IsMeasured);

            //We expect to find our four layers to measure, knowing that the first two were merged
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);
        }

        [TestMethod]
        public void For2layersGoups_WithProbeLiseDown_CreateMetroResult_NominalCase()
        {
            //Given datas input
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var fourthLayer100 = CreateLayer(100.Micrometers(), 4);
            var fifthLayer550 = CreateLayer(550.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer200, fourthLayer100, fifthLayer550 };

            var layerGroup750With2Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { thirdLayer200 }, 2);
            var totalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers,CreateProbeSettings(LiseBottomId));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
                totalThickness,
            };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.CreateMetroMeasureResult(thicknessSettings);
            var res = measureResult as ThicknessResult;

            //Respresent group of layer with id 1 and 2
            var layer1 = res.Settings.ThicknessLayers[0];
            Assert.AreEqual(layerGroup750With2Layer.Name, layer1.Name);
            Assert.AreEqual(firstLayer200.Thickness + secondLayer550.Thickness, layer1.Target);
            Assert.AreEqual(layerGroup750With2Layer.ThicknessTolerance, layer1.Tolerance);
            Assert.AreEqual(true, layer1.IsMeasured);

            //layer with id 3
            var layer2 = res.Settings.ThicknessLayers[1];
            Assert.AreEqual(thirdLayer200.Name, layer2.Name);
            Assert.AreEqual(thirdLayer200.Thickness, layer2.Target);
            Assert.AreEqual(layerGroup200With1Layer.ThicknessTolerance, layer2.Tolerance);
            Assert.AreEqual(true, layer2.IsMeasured);

            //layer with id 4
            var layer4 = res.Settings.ThicknessLayers[2];
            Assert.AreEqual(fourthLayer100.Name, layer4.Name);
            Assert.AreEqual(fourthLayer100.Thickness, layer4.Target);
            //It is not a layer to be measured so it has no tolerance
            var expectedDefaultTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            Assert.AreEqual(expectedDefaultTolerance, layer4.Tolerance);
            Assert.AreEqual(false, layer4.IsMeasured);

            var layer5 = res.Settings.ThicknessLayers[3];
            Assert.AreEqual(false, layer5.IsMeasured);

            //We expect to find our four layers to measure, knowing that the first two were merged
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);
            Assert.IsTrue(res.Settings.HasWaferThicknesss);
        }

        [TestMethod]
        public void For2layersGoups_WithDifferentProbes_CreateMetroResult_NominalCase()
        {
            //Given datas input

            var signalUp = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);

            var signalDown = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDown, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var fourthLayer100 = CreateLayer(100.Micrometers(), 4);
            var fifthLayer550 = CreateLayer(550.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer200, fourthLayer100, fifthLayer550 };

            var layerGroup750With2Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { thirdLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var layerGroups = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.CreateMetroMeasureResult(thicknessSettings);
            var res = measureResult as ThicknessResult;
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);

            //Respresent group of layer with id 1 and 2
            var layer1 = res.Settings.ThicknessLayers[0];
            Assert.AreEqual(layerGroup750With2Layer.Name, layer1.Name);
            Assert.AreEqual(firstLayer200.Thickness + secondLayer550.Thickness, layer1.Target);
            Assert.AreEqual(layerGroup750With2Layer.ThicknessTolerance, layer1.Tolerance);

            //layer with id 3
            var layer2 = res.Settings.ThicknessLayers[1];
            Assert.AreEqual(thirdLayer200.Name, layer2.Name);
            Assert.AreEqual(thirdLayer200.Thickness, layer2.Target);
            Assert.AreEqual(layerGroup200With1Layer.ThicknessTolerance, layer2.Tolerance);

            //layer with id 4
            var layer4 = res.Settings.ThicknessLayers[2];
            Assert.AreEqual(fourthLayer100.Name, layer4.Name);
            Assert.AreEqual(fourthLayer100.Thickness, layer4.Target);
            //It is not a layer to be measured so it has no tolerance
            var expectedDefaultTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            Assert.AreEqual(expectedDefaultTolerance, layer4.Tolerance);

            //We expect to find our four layers to measure, knowing that the first two were merged
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);
        }

        [TestMethod]
        public void For2layersGoups_WithDualLise_CreateMetroResult_NominalCase()
        {
            //Given datas input

            var signalUp = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);

            var signalDown = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDown, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var fourthLayer100 = CreateLayer(100.Micrometers(), 4);
            var fifthLayer550 = CreateLayer(550.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer200, fourthLayer100, fifthLayer550 };

            var layerGroup750With2Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId), new List<LayerSettings>() { thirdLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var layerGroups = new List<Layer>
            {
                layerGroup750With2Layer,
                layerGroup200With1Layer,
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.CreateMetroMeasureResult(thicknessSettings);
            var res = measureResult as ThicknessResult;
            Assert.AreEqual(4, res.Settings.ThicknessLayers.Count);
        }
    }
}
