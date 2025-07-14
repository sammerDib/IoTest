using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Thickness;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Measure.Test.ThicknessTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureThicknessLayersUnificationTest
    {
        private const string LiseUpId = "LiseUpId";
        private const string LiseDownId = "LiseDownId";
        private const string LiseDualId = "LiseDualId";

        [TestMethod]
        public void One_physical_layer_measured()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer750 };
            var layerGroup750With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId), new List<LayerSettings>() { layer750 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var layersUnified = new UnifiedLayers(thicknessSettings);

            //Then
            layersUnified.LayersToMeasure.Should().NotBeNull().And.HaveCount(1).And.BeEquivalentTo(thicknessSettings.LayersToMeasure);
            layersUnified.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.BeEquivalentTo(thicknessSettings.PhysicalLayers);
            layersUnified.LayersByProbeSettings.Should().NotBeNull().And.HaveCount(1);
            layersUnified.OffsetByLayerToMeasureName.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public void Three_physical_layers_measured()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 1);
            var layer150 = CreateLayer(150.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer50, layer100, layer150 };
            var layerGroup50With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8), new List<LayerSettings>() { layer50 }, 1, 1.8);
            var layerGroup100With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8), new List<LayerSettings>() { layer100 }, 2, 1.8);
            var layerGroup150With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8), new List<LayerSettings>() { layer150 }, 3, 1.8);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With1Layer, layerGroup100With1Layer, layerGroup150With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var layersUnified = new UnifiedLayers(thicknessSettings);

            //Then
            var expectedProbeUpSettings = CreateProbeSettings(LiseUpId);
            var expectedThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            var expectedGain = 1.8;
            var expectedMultipleLayersOffset = 0.Millimeters();
            var expectedLayer50 = CreateLayer(50.Micrometers(), 1);
            var expectedLayer100 = CreateLayer(100.Micrometers(), 1);
            var expectedLayer150 = CreateLayer(150.Micrometers(), 1);
            layersUnified.LayersToMeasure.Should().NotBeNull().And.HaveCount(3).And.BeEquivalentTo(thicknessSettings.LayersToMeasure);
            layersUnified.PhysicalLayers.Should().NotBeNull().And.HaveCount(3).And.BeEquivalentTo(thicknessSettings.PhysicalLayers);
            layersUnified.LayersByProbeSettings.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.Key.ProbeId.Should().Be(LiseUpId);
                    element.Value.Should().NotBeNull().And.HaveCount(3).And.SatisfyRespectively(
                        firstElement =>
                        {
                            firstElement.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                                physLayer => { physLayer.Should().Be(expectedLayer50); });
                            firstElement.Name.Should().NotBeNull();
                            firstElement.ThicknessTolerance.Should().BeEquivalentTo(expectedThicknessTolerance);
                            firstElement.ProbeSettings.Should().BeEquivalentTo(expectedProbeUpSettings);
                            firstElement.RefractiveIndex.Should().Be(expectedGain);
                            firstElement.MultipleLayersOffset.Should().BeEquivalentTo(expectedMultipleLayersOffset);
                            firstElement.IsWaferTotalThickness.Should().BeFalse();
                        },
                        secondElement =>
                        {
                            secondElement.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                                physLayer => { physLayer.Should().Be(expectedLayer100); });
                            secondElement.Name.Should().NotBeNull();
                            secondElement.ThicknessTolerance.Should().BeEquivalentTo(expectedThicknessTolerance);
                            secondElement.ProbeSettings.Should().BeEquivalentTo(expectedProbeUpSettings);
                            secondElement.RefractiveIndex.Should().Be(expectedGain);
                            secondElement.MultipleLayersOffset.Should().BeEquivalentTo(expectedMultipleLayersOffset);
                            secondElement.IsWaferTotalThickness.Should().BeFalse();
                        },
                        thirdElement =>
                        {
                            thirdElement.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                                physLayer => { physLayer.Should().Be(expectedLayer150); });
                            thirdElement.Name.Should().NotBeNull();
                            thirdElement.ThicknessTolerance.Should().BeEquivalentTo(expectedThicknessTolerance);
                            thirdElement.ProbeSettings.Should().BeEquivalentTo(expectedProbeUpSettings);
                            thirdElement.RefractiveIndex.Should().Be(expectedGain);
                            thirdElement.MultipleLayersOffset.Should().BeEquivalentTo(expectedMultipleLayersOffset);
                            thirdElement.IsWaferTotalThickness.Should().BeFalse();
                        }
                    );
                });
            layersUnified.OffsetByLayerToMeasureName.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public void One_measured_group_of_two_physical_layers()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer50, layer100 };
            var layerGroup50With2Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8), new List<LayerSettings>() { layer50, layer100 }, 1, 1.8);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With2Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var layersUnified = new UnifiedLayers(thicknessSettings);

            //Then
            var expectedProbeUpSettings = CreateProbeSettings(LiseUpId);
            var expectedThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            var expectedGain = 1.8;
            var expectedMultipleLayersOffset = 0.Millimeters();
            var expectedLayer150 = CreateLayer(150.Micrometers(), 1);
            var expectedLayers = new List<LayerSettings>() { expectedLayer150 };
            var expectedLayerGroup150With1Layer = CreateLayerGroup(CreateProbeSettings(LiseUpId, 1.8), new List<LayerSettings>() { expectedLayer150 }, 1, expectedGain);
            var expectedLayerGroupsToMeasure = new List<Layer> { expectedLayerGroup150With1Layer };
            var expectedThicknessSettings = CreateThicknessSettings(expectedLayers, expectedLayerGroupsToMeasure);
            expectedLayer150.MaterialName = "Composites";
            expectedLayer150.RefractiveIndex = expectedGain;
            expectedLayer150.Name = expectedLayerGroup150With1Layer.Name;
            layersUnified.LayersToMeasure.Should().NotBeNull().And.HaveCount(1).And.BeEquivalentTo(expectedThicknessSettings.LayersToMeasure);
            layersUnified.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.BeEquivalentTo(expectedThicknessSettings.PhysicalLayers);
            layersUnified.LayersByProbeSettings.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.Key.ProbeId.Should().Be(LiseUpId);
                    element.Value.Should().NotBeNull().And.HaveCount(1).And.SatisfyRespectively(
                        firstElement =>
                        {
                            firstElement.PhysicalLayers.Should().NotBeNull().And.HaveCount(1).And.SatisfyRespectively(
                                physLayer => { physLayer.Should().Be(expectedLayer150); });
                            firstElement.Name.Should().NotBeNull();
                            firstElement.ThicknessTolerance.Should().BeEquivalentTo(expectedThicknessTolerance);
                            firstElement.ProbeSettings.Should().BeEquivalentTo(expectedProbeUpSettings);
                            firstElement.RefractiveIndex.Should().Be(expectedGain);
                            firstElement.MultipleLayersOffset.Should().BeEquivalentTo(expectedMultipleLayersOffset);
                            firstElement.IsWaferTotalThickness.Should().BeFalse();
                        }
                    );
                });
            layersUnified.OffsetByLayerToMeasureName.Should().NotBeNull().And.BeEmpty();
        }
    }
}