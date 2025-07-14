using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Measure.Test.ThicknessTestUtils;
using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureThicknessTest : TestWithMockedHardware<MeasureThicknessTest>,
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
                       new MeasureThicknessConfiguration()
                   }
               });
        }

        [TestMethod]
        public void ExecuteThicknessMeasure_CompleteCase()
        {
            //Given
            var signalUp = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);
            var signalBottom = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalBottom, this);

            var firstLayer100 = CreateLayer(100.Micrometers(), 1);
            var secondLayer100 = CreateLayer(100.Micrometers(), 2);
            var thirdLayer550 = CreateLayer(550.Micrometers(), 3);
            var fourthLayer200 = CreateLayer(200.Micrometers(), 4);
            var layers = new List<LayerSettings>() { firstLayer100, secondLayer100, thirdLayer550, fourthLayer200 };

            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers, CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId));

            var offset = 100.Micrometers();
            var layerGroups750With3LayersAndOffset = CreateLayerGroupWithOffset(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer100, secondLayer100, thirdLayer550 }, 1, offset);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { fourthLayer200 }, 4);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroups750With3LayersAndOffset,
                layerGroup200With1Layer,
                layerTotalThickness
            };

            var layerGroups = new List<Layer>
            {
                layerGroups750With3LayersAndOffset,
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);

            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;
            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;

            //Then
            Assert.AreEqual(measureResult.State, MeasureState.Success);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            // layerGroup
            Assert.AreEqual(layerGroups750With3LayersAndOffset.Name, thicknessPointData.ThicknessLayerResults[0].Name);
            Assert.AreEqual(750 + offset.Micrometers, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);

            // layer2
            Assert.AreEqual(fourthLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
            Assert.AreEqual(layerGroup200With1Layer.PhysicalLayers[0].Thickness.Micrometers, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);

            // totalThickness
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
            Assert.AreEqual(750 + offset.Micrometers + 200, thicknessPointData.WaferThicknessResult.Length.Micrometers, 5);
        }

        #region testFor1Layer

        [TestMethod]
        public void Measuring_one_layer_with_lise_up_succeeds_and_returns_good_thickness_when_provided_layer_is_correct()
        {
            //Given: The LISE signal corresponds to the layer to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { firstLayer750 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer
            };

            var layerGroups = new List<Layer>
            {
                layerGroup750With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);
        }

        [TestMethod]
        public void Measuring_one_layer_with_lise_bottom_succeeds_and_returns_good_thickness_when_provided_layer_is_correct()
        {
            //Given: The LISE signal corresponds to the layer to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { firstLayer750 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);
        }

        [TestMethod]
        public void Measuring_one_layer_fails_when_provided_layer_is_incorrect()
        {
            //Given: The LISE signals doesn't corresponds to the layer to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { firstLayer750 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state fails and no thickness is measured
            Assert.AreEqual(MeasureState.NotMeasured, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.NotMeasured, thicknessPointData.State);
            Assert.AreEqual(0, thicknessPointData.ThicknessLayerResults.Count);
        }

        [TestMethod]
        public void Measuring_one_refractive_index_unknown_with_dual_lise_succeeds_and_returns_good_thickness_when_calibration_is_correct()
        {
            //Given: Correct calibration
            var layersUp = new List<ProbeSampleLayer> { Layer750 };
            var layersDown = new List<ProbeSampleLayer> { Layer750 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);

            var sampleSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp + 750.Micrometers(), AirGapDown);

            var signalsLiseUp = new List<IProbeSignal>
            {
                sampleSignalForCalibration.SignalLiseUp,
                sampleAndSignalForMeasure.SignalLiseUp
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsLiseUp, this);

            var signalsLiseBottom = new List<IProbeSignal>
            {
                sampleSignalForCalibration.SignalLiseDown,
                sampleAndSignalForMeasure.SignalLiseDown
            };
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalsLiseBottom, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var firstUnknownLayer = CreateLayer(0.Micrometers(), 2);
            firstUnknownLayer.RefractiveIndex = double.NaN;

            var layers = new List<LayerSettings>() { CreateLayer(750.Micrometers(), 1), firstUnknownLayer, CreateLayer(750.Micrometers(), 3) };

            var layerToMeasureWithUnknowLayer = CreateLayerGroup(CreateDualProbeSettings(DualLiseId, 
                CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)),
                new List<LayerSettings>() { firstUnknownLayer },
                2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerToMeasureWithUnknowLayer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single unknown layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
        }

        #endregion testFor1Layer

        #region testForMultipleLayers

        [TestMethod]
        public void Measuring_one_layer_with_lise_up_succeeds_and_returns_good_thickness_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layer to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[0].Name);
        }

        [TestMethod]
        public void Measuring_one_layer_with_lise_bottom_succeeds_and_returns_good_thickness_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layer to be measured
            var signalFromBottom = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalFromBottom, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[0].Name);
        }

        [TestMethod]
        public void Measuring_layerWithUnknownRI_between_multiple_layers_with_dual_lise_succeeds_and_returns_good_thickness_when_calibration_and_provided_layers_are_correct()
        {
            //Given: Correct calibration & the LISE signal corresponds to the layers to be measured
            var layersUp = new List<ProbeSampleLayer> { Layer200 };
            var layersDown = new List<ProbeSampleLayer> { Layer200 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);

            var sampleSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);

            List<IProbeSignal> signalsLiseUp = new List<IProbeSignal>();
            signalsLiseUp.Add(sampleSignalForCalibration.SignalLiseUp);
            signalsLiseUp.Add(sampleAndSignalForMeasure.SignalLiseUp);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsLiseUp, this);

            List<IProbeSignal> signalsLiseBottom = new List<IProbeSignal>();
            signalsLiseBottom.Add(sampleSignalForCalibration.SignalLiseDown);
            signalsLiseBottom.Add(sampleAndSignalForMeasure.SignalLiseDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalsLiseBottom, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondUnknownLayer = CreateLayer(350.Micrometers(), 2);
            secondUnknownLayer.RefractiveIndex = double.NaN;
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var layers = new List<LayerSettings>() { firstLayer200, secondUnknownLayer, thirdLayer200 };

            var layerGroupWithUnknowLayer = CreateLayerGroup(
                CreateDualProbeSettings(
                    DualLiseId, 
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), 
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)), 
                new List<LayerSettings>() { secondUnknownLayer }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroupWithUnknowLayer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single unknown layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750 - 200 - 200, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
        }

        [TestMethod]
        public void Measuring_multiple_layers_with_lise_up_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer,
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
        }

        [TestMethod]
        public void Measuring_multiple_layers_with_lise_bottom_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signalFromBottom = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalFromBottom, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer,
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
        }

        [TestMethod]
        public void Measuring_multiple_layers_with_differents_probes_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signals corresponds to the layers to be measured
            var signalFromTop = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalFromTop, this);

            var signalFromBottom = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalFromBottom, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer,
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
        }

        [TestMethod]
        public void Measuring_multiple_layers_with_differents_probes_up_settings_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured but probes used to measure each layer are differents
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200, };

            var layerGroup750ProbeUpSettings1 = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.4, ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200ProbeUpSettings2 = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8, TestWithObjectiveHelper.objectiveUp20XId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750ProbeUpSettings1,
                layerGroup200ProbeUpSettings2
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
        }

        [TestMethod]
        public void Measuring_multiple_layers_with_differents_probes_bottom_settings_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured but probe settings used to measure each layer are differents
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750ProbeUpSettings1 = CreateLayerGroup(CreateProbeSettings(LiseBottomId, 1.4, ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200ProbeUpSettings2 = CreateLayerGroup(CreateProbeSettings(LiseBottomId, 1.8, ObjectiveBottomId), new List<LayerSettings>() { secondLayer200 }, 2);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750ProbeUpSettings1,
                layerGroup200ProbeUpSettings2
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);
        }

        [TestMethod]
        public void Measuring_multiple_layers_fails_when_provided_layers_are_incorrect()
        {
            //Given: The LISE signals doesn't corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 2);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer200 };

            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer200 }, 1);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup200With1Layer,
                layerGroup200With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Measure is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state fails and no thickness is measured
            Assert.AreEqual(MeasureState.NotMeasured, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.NotMeasured, thicknessPointData.State);
            Assert.AreEqual(0, thicknessPointData.ThicknessLayerResults.Count);
        }

        #endregion testForMultipleLayers

        #region testTotalThickness

        [TestMethod]
        public void Measuring_total_thickness_with_lise_up_succeeds_and_returns_good_total_thickness_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { secondLayer200 }, 2);
            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers, CreateProbeSettings(LiseUpId, objective: ObjectiveUpId));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Only total thickness is measured
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(0, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750 + 200, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        [TestMethod]
        public void Measuring_total_thickness_with_lise_bottom_succeeds_and_returns_good_total_thickness_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers, CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Only total thickness is measured
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(0, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750 + 200, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        [TestMethod]
        public void Measuring_total_thickness_with_dual_lise_succeeds_and_returns_good_total_thickness_when_calibration_and_provided_layers_are_correct()
        {
            //Given: Correct calibration
            var layersUp = new List<ProbeSampleLayer> { Layer200 };
            var layersDown = new List<ProbeSampleLayer> { Layer200 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);

            var sampleSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);

            List<IProbeSignal> signalsLiseUp = new List<IProbeSignal>();
            signalsLiseUp.Add(sampleSignalForCalibration.SignalLiseUp);
            signalsLiseUp.Add(sampleAndSignalForMeasure.SignalLiseUp);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsLiseUp, this);

            List<IProbeSignal> signalsLiseBottom = new List<IProbeSignal>();
            signalsLiseBottom.Add(sampleSignalForCalibration.SignalLiseDown);
            signalsLiseBottom.Add(sampleAndSignalForMeasure.SignalLiseDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalsLiseBottom, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayerWithUnknownRI = CreateLayer(350.Micrometers(), 2);
            secondLayerWithUnknownRI.RefractiveIndex = double.NaN;
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayerWithUnknownRI, thirdLayer200 };

            var layerToMeasure = CreateLayerGroup(
                CreateDualProbeSettings(
                    DualLiseId, 
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), 
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)), 
                new List<LayerSettings>() { secondLayerWithUnknownRI }, 2);
            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(
                layers, 
                CreateDualProbeSettings(
                    DualLiseId, 
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), 
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)));

            var layersToMeasure = new List<Layer>
            {
                layerToMeasure,
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layersToMeasure);

            //When: Single unknown layer measurement is execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        [TestMethod]
        public void Measuring_thicknesses_and_total_thickness_with_lise_up_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { secondLayer200 }, 2);
            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers, CreateProbeSettings(LiseUpId, objective: ObjectiveUpId));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer,
                layerGroup200With1Layer,
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750 + 200, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        [TestMethod]
        public void Measuring_thicknesses_and_total_thickness_with_lise_bottom_succeeds_and_returns_good_thicknesses_when_provided_layers_are_correct()
        {
            //Given: The LISE signal corresponds to the layers to be measured
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 200.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signal, this);

            var firstLayer750 = CreateLayer(750.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var layers = new List<LayerSettings>() { firstLayer750, secondLayer200 };

            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { firstLayer750 }, 1);
            var layerGroup200With1Layer = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { secondLayer200 }, 2);
            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(layers, CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With1Layer,
                layerGroup200With1Layer,
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: All layer measurements are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thicknesses are correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
            Assert.AreEqual(firstLayer750.Name, thicknessPointData.ThicknessLayerResults[0].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[1].State);
            Assert.AreEqual(200, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
            Assert.AreEqual(secondLayer200.Name, thicknessPointData.ThicknessLayerResults[1].Name);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750 + 200, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        [TestMethod]
        public void Measuring_thicknesses_and_total_thickness_with_dual_lise_succeeds_and_returns_good_thicknesses_when_calibration_and_provided_layers_are_correct()
        {
            //Given: Correct calibration & the LISE signal corresponds to the layers to be measured
            var layersUp = new List<ProbeSampleLayer> { Layer200 };
            var layersDown = new List<ProbeSampleLayer> { Layer200 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);

            var sampleSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);

            List<IProbeSignal> signalsLiseUp = new List<IProbeSignal>();
            signalsLiseUp.Add(sampleSignalForCalibration.SignalLiseUp);
            signalsLiseUp.Add(sampleAndSignalForMeasure.SignalLiseUp);
            signalsLiseUp.Add(sampleSignalForCalibration.SignalLiseUp);
            signalsLiseUp.Add(sampleAndSignalForMeasure.SignalLiseUp);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalsLiseUp, this);

            List<IProbeSignal> signalsLiseBottom = new List<IProbeSignal>();
            signalsLiseBottom.Add(sampleSignalForCalibration.SignalLiseDown);
            signalsLiseBottom.Add(sampleAndSignalForMeasure.SignalLiseDown);
            signalsLiseBottom.Add(sampleSignalForCalibration.SignalLiseDown);
            signalsLiseBottom.Add(sampleAndSignalForMeasure.SignalLiseDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalsLiseBottom, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondUnknownLayer = CreateLayer(350.Micrometers(), 2);
            secondUnknownLayer.RefractiveIndex = double.NaN;
            var thirdLayer200 = CreateLayer(200.Micrometers(), 3);
            var layers = new List<LayerSettings>() { firstLayer200, secondUnknownLayer, thirdLayer200 };

            var layerGroup200With1Layer = CreateLayerGroup(
                CreateDualProbeSettings(
                    DualLiseId, 
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), 
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)),
                new List<LayerSettings>() { firstLayer200 }, 1);
            var layerGroupWithUnknowLayer = CreateLayerGroup(
                CreateDualProbeSettings(
                    DualLiseId, 
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), 
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)),
                new List<LayerSettings>() { secondUnknownLayer }, 2);
            var layerTotalThickness = CreateFakeLayerGroupToMeasureTotalThickness(
                layers,
                CreateDualProbeSettings(
                    DualLiseId,
                    CreateProbeSettings(LiseUpId, objective: ObjectiveUpId),
                    CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId)));

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroupWithUnknowLayer,
                layerTotalThickness
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When: Single unknown layer measurement and total thickness are execute
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);
            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then: The state is success and thickness is correct
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);
            Assert.AreEqual(MeasureState.Success, thicknessPointData.State);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.ThicknessLayerResults[0].State);
            Assert.AreEqual(750 - 200 - 200, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);

            Assert.AreEqual(MeasureState.Success, thicknessPointData.WaferThicknessResult.State);
            Assert.AreEqual(750, thicknessPointData.WaferThicknessResult.Length.Micrometers, 1);
            Assert.AreEqual(layerTotalThickness.Name, thicknessPointData.WaferThicknessResult.Name);
        }

        #endregion testTotalThickness

        #region testLayersGrouping

        [TestMethod]
        public void Measuring_grouped_layers_thicknesses_succeeds_and_returns_good_thickness_when_provided_grouped_layers_are_correct()
        {
            //Given
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 550.Micrometers(), 400.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer550 = CreateLayer(550.Micrometers(), 3);
            var fourthLayer200 = CreateLayer(200.Micrometers(), 4);
            var fifthLayer200 = CreateLayer(200.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer550, fourthLayer200, fifthLayer200 };

            var layerGroup750With2Layers = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup400With2Layers = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { fourthLayer200, fifthLayer200 }, 4);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layers,
                layerGroup400With2Layers
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);

            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then
            Assert.AreEqual(measureResult.State, MeasureState.Success);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(layerGroup750With2Layers.Name, thicknessPointData.ThicknessLayerResults[0].Name);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);

            Assert.AreEqual(layerGroup400With2Layers.Name, thicknessPointData.ThicknessLayerResults[1].Name);
            Assert.AreEqual(400, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
        }

        [TestMethod]
        public void Measuring_grouped_layers_thicknesses_with_different_probes_succeeds_and_returns_good_thickness_when_provided_grouped_layers_are_correct()
        {
            //Given
            var signalUp = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 550.Micrometers(), 400.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);
            var signalBottom = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 400.Micrometers(), 550.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalBottom, this);

            var firstLayer200 = CreateLayer(200.Micrometers(), 1);
            var secondLayer550 = CreateLayer(550.Micrometers(), 2);
            var thirdLayer550 = CreateLayer(550.Micrometers(), 3);
            var fourthLayer200 = CreateLayer(200.Micrometers(), 4);
            var fifthLayer200 = CreateLayer(200.Micrometers(), 5);
            var layers = new List<LayerSettings>() { firstLayer200, secondLayer550, thirdLayer550, fourthLayer200, fifthLayer200 };

            var layerGroup750With2Layers = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { firstLayer200, secondLayer550 }, 1);
            var layerGroup550With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { thirdLayer550 }, 3);
            var layerGroup400With2Layers = CreateLayerGroup(CreateProbeSettings(LiseBottomId, objective: ObjectiveBottomId), new List<LayerSettings>() { fourthLayer200, fifthLayer200 }, 4);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2Layers,
                layerGroup400With2Layers
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);

            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then
            Assert.AreEqual(measureResult.State, MeasureState.Success);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(layerGroup750With2Layers.Name, thicknessPointData.ThicknessLayerResults[0].Name);
            Assert.AreEqual(750, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);

            Assert.AreEqual(layerGroup400With2Layers.Name, thicknessPointData.ThicknessLayerResults[1].Name);
            Assert.AreEqual(400, thicknessPointData.ThicknessLayerResults[1].Length.Micrometers, 1);
        }

        [TestMethod]
        public void Measuring_grouped_layers_thickness_with_offset_succeeds_and_returns_good_thickness_when_provided_grouped_layers_are_correct()
        {
            //Given
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 550.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var firstLayer550 = CreateLayer(550.Micrometers(), 1);
            var secondLayer200 = CreateLayer(200.Micrometers(), 2);
            var thirdLayer550 = CreateLayer(550.Micrometers(), 3);
            var layers = new List<LayerSettings>() { firstLayer550, secondLayer200, thirdLayer550 };

            var offset = 100.Micrometers();
            var layerGroup750With2LayersAndOffset = CreateLayerGroupWithOffset(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { secondLayer200, thirdLayer550 }, 2, offset);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup750With2LayersAndOffset
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);

            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(1, thicknessPointData.ThicknessLayerResults.Count);

            Assert.AreEqual(layerGroup750With2LayersAndOffset.Name, thicknessPointData.ThicknessLayerResults[0].Name);
            Assert.AreEqual(750 + offset.Micrometers, thicknessPointData.ThicknessLayerResults[0].Length.Micrometers, 1);
        }

        [TestMethod]
        public void Measurement_of_a_layer_smaller_than_40_microns_Joining_with_the_adjacent_layer_to_form_a_single_layer_the_grouping_will_not_be_measured_The_measurement_is_a_success()
        {
            //Given
            var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 550.Micrometers() }, MaterialRefractionIndex);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signal, this);

            var layer740 = CreateLayer(740.Micrometers(), 1);
            var layer10 = CreateLayer(10.Micrometers(), 2);
            var layer550 = CreateLayer(550.Micrometers(), 4);

            var layers = new List<LayerSettings>() { layer740, layer10, layer550 };

            var layerGroup740With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { layer740 }, 1);
            var layerGroup550With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, objective: ObjectiveUpId), new List<LayerSettings>() { layer550 }, 4);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup740With1Layer,
                layerGroup550With1Layer
            };

            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureLoad = loader.GetMeasure(MeasureType.Thickness);

            var measureResult = measureLoad.Execute(thicknessSettings, CreateContext()) as ThicknessPointResult;

            //Then
            Assert.AreEqual(MeasureState.Success, measureResult.State);

            var thicknessPointData = measureResult.Datas[0] as ThicknessPointData;
            Assert.AreEqual(2, thicknessPointData.ThicknessLayerResults.Count);
        }

        #endregion testLayersGrouping
    }
}
