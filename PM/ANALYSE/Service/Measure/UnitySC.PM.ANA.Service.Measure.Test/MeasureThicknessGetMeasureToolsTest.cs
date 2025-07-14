using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Measure.Test.ThicknessTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureThicknessGetMeasureToolsTest : TestWithMockedHardware<MeasureThicknessTest>, ITestWithProbeLise
    {
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }

        private List<string> GetCompatibleObjectivesUp()
        {
            return new List<string>() { ObjectiveUpId, TestWithObjectiveHelper.objectiveUp20XId };
        }

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

        // |------------------------|
        // |        layer750        | -> Measured, Top & Bottom & Dual
        // |------------------------|
        [TestMethod]
        public void Only_one_measurable_layer_returns_all_probes_and_objectives()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer750 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(element =>
            {
                element.NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
                element.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                element.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
            });
        }

        // |------------------------|
        // |        layer750        | -> Measured, Top & Bottom
        // |------------------------|
        // |------------------------|
        // |        layer100        | -> Measured, Top & Bottom
        // |------------------------|
        // |------------------------|
        // |        layer200        | -> Measured, Top & Bottom
        // |------------------------|
        [TestMethod]
        public void Three_measurable_layers_returns_top_and_bottom_probes_and_objective()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layer200 = CreateLayer(200.Micrometers(), 3);
            var layers = new List<LayerSettings>() { layer750, layer100, layer200 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroup100With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100 }, 2);
            var layerGroup200With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer200 }, 3);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer, layerGroup100With1Layer, layerGroup200With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(3).And.AllSatisfy(element =>
            {
                element.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                element.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                element.DualProbes.Should().BeEmpty();
            });
            measureTools.MeasureToolsForLayers[0].NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
            measureTools.MeasureToolsForLayers[1].NameLayerToMeasure.Should().Be(layerGroup100With1Layer.Name);
            measureTools.MeasureToolsForLayers[2].NameLayerToMeasure.Should().Be(layerGroup200With1Layer.Name);
        }

        // |========================|
        // |  unmeasurableLayer750  | -> Measured, Dual
        // |========================|
        [TestMethod]
        public void Only_one_unmeasurable_layer_returns_dual_probe_and_objectives()
        {
            //Given
            var layer750 = CreateUnknownLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer750 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };
            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(element =>
            {
                element.NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
                element.UpProbes.Should().BeEmpty();
                element.DownProbes.Should().BeEmpty();
                element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => probe.Should().BeEquivalentTo(expectedDualProbeMaterial));
            });
        }

        // |------------------------|
        // |        layer750        | -> Measured, Top
        // |------------------------|
        // |========================|
        // |  unmeasurableLayer100  | -> Not Measured
        // |========================|
        [TestMethod]
        public void One_measurable_layer_above_one_unmeasurable_layer_returns_top_probe_and_objective()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layer100 = CreateUnknownLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(element =>
            {
                element.NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
                element.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                element.DownProbes.Should().BeEmpty();
                element.DualProbes.Should().BeEmpty();
            });
            measureTools.MeasureToolsForLayers[0].NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
        }

        // |========================|
        // |  unmeasurableLayer750  | -> Not Measured
        // |========================|
        // |------------------------|
        // |        layer100        | -> Measured, Bottom
        // |------------------------|
        [TestMethod]
        public void One_measurable_layer_below_one_unmeasurable_layer_returns_bottom_probe_and_objective()
        {
            //Given
            var layer750 = CreateUnknownLayer(750.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup100With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup100With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(element =>
            {
                element.NameLayerToMeasure.Should().Be(layerGroup100With1Layer.Name);
                element.UpProbes.Should().BeEmpty();
                element.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                element.DualProbes.Should().BeEmpty();
            });
        }

        // |------------------------|
        // |        layer750        | -> Measured, Top
        // |------------------------|
        // |========================|
        // |  unmeasurableLayer100  | -> Measured, Dual
        // |========================|
        [TestMethod]
        public void One_measurable_layer_above_one_measured_unmeasurable_layer_returns_bottom_probe_and_objective()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layer100 = CreateUnknownLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroup100With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100 }, 2);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer, layerGroup100With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(2).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
                    firstElement.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroup100With1Layer.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().BeEmpty();
                    secondElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        // |========================|
        // |  unmeasurableLayer100  | -> Measured, No probe
        // |========================|
        // |------------------------|
        // |        layer750        | -> Measured, No probe
        // |------------------------|
        // |========================|
        // |  unmeasurableLayer100  | -> Measured, No probe
        // |========================|
        [TestMethod]
        public void One_measurable_layer_between_two_measured_unmeasurable_layers_returns_no_probe_and_objective()
        {
            //Given
            var layer750 = CreateUnknownLayer(750.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layer200 = CreateUnknownLayer(100.Micrometers(), 3);
            var layers = new List<LayerSettings>() { layer750, layer100, layer200 };
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroup100With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100 }, 2);
            var layerGroup200With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer200 }, 3);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With1Layer, layerGroup100With1Layer, layerGroup200With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(3).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup750With1Layer.Name);
                    firstElement.UpProbes.Should().BeEmpty();
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroup100With1Layer.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().BeEmpty();
                    secondElement.DualProbes.Should().BeEmpty();
                },
                thirdElement =>
                {
                    thirdElement.NameLayerToMeasure.Should().Be(layerGroup200With1Layer.Name);
                    thirdElement.UpProbes.Should().BeEmpty();
                    thirdElement.DownProbes.Should().BeEmpty();
                });
        }

        // |------------------------| -----}
        // |        layer750        |      }
        // |------------------------|      } -> Grouped -> Measured, Top & Bottom & Dual
        // |------------------------|      }
        // |        layer100        |      }
        // |------------------------| -----}
        [TestMethod]
        public void Two_measurable_layer_grouped_returns_all_probes_and_objectives()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup750With2Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750, layer100 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With2Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.NameLayerToMeasure.Should().Be(layerGroup750With2Layer.Name);
                    element.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    element.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        // |------------------------| -----}
        // |        layer750        |      }
        // |------------------------|      } -> Grouped -> Measured, Dual
        // |========================|      }
        // |  unmeasurableLayer100  |      }
        // |========================| -----}
        [TestMethod]
        public void One_measurable_layer_grouped_with_one_unmeasurable_layers_returns_dual_probe_and_objectives()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layer100 = CreateUnknownLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup750With2Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750, layer100 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With2Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.NameLayerToMeasure.Should().Be(layerGroup750With2Layer.Name);
                    element.UpProbes.Should().BeEmpty();
                    element.DownProbes.Should().BeEmpty();
                    element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => probe.Should().BeEquivalentTo(expectedDualProbeMaterial));
                });
        }

        // |========================| -----}
        // |  unmeasurableLayer100  |      }
        // |========================|      }  -> Grouped -> Measured, Dual
        // |------------------------|      }
        // |        layer750        |      }
        // |------------------------| -----}
        [TestMethod]
        public void One_unmeasurable_layer_grouped_with_one_measurable_layers_returns_dual_probe_and_objectives()
        {
            //Given
            var layer750 = CreateUnknownLayer(750.Micrometers(), 1);
            var layer100 = CreateLayer(100.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer750, layer100 };
            var layerGroup750With2Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750, layer100 }, 1);
            var layerGroupsToMeasure = new List<Layer> { layerGroup750With2Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.NameLayerToMeasure.Should().Be(layerGroup750With2Layer.Name);
                    element.UpProbes.Should().BeEmpty();
                    element.DownProbes.Should().BeEmpty();
                    element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => probe.Should().BeEquivalentTo(expectedDualProbeMaterial));
                });
        }

        // |------------------------|
        // |        layer50         | -> Not Measured
        // |------------------------|
        // |========================| -----}
        // |  unmeasurableLayer100  |      }
        // |========================|      }
        // |------------------------|      }
        // |        layer150        |      } -> Grouped -> Measured, Dual
        // |------------------------|      }
        // |========================|      }
        // |  unmeasurableLayer200  |      }
        // |========================| -----}
        // |------------------------|
        // |        layer250        | -> Measured, Bottom
        // |------------------------|
        [TestMethod]
        public void One_measurable_layer_between_two_measured_unmeasurable_layers_grouped_with_one_measurable_layer_below_returns_respectively_dual_and_bottom_probe_and_objective()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer100 = CreateUnknownLayer(100.Micrometers(), 2);
            var layer150 = CreateLayer(750.Micrometers(), 3);
            var layer200 = CreateUnknownLayer(100.Micrometers(), 4);
            var layer250 = CreateLayer(100.Micrometers(), 5);
            var layers = new List<LayerSettings>() { layer50, layer100, layer150, layer200, layer250 };
            var layerGroup100With3Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100, layer150, layer200 }, 2);
            var layerGroup250With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer250 }, 3);
            var layerGroupsToMeasure = new List<Layer> { layerGroup100With3Layer, layerGroup250With1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(2).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup100With3Layer.Name);
                    firstElement.UpProbes.Should().BeEmpty();
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => probe.Should().BeEquivalentTo(expectedDualProbeMaterial));
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroup250With1Layer.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    secondElement.DualProbes.Should().BeEmpty();
                });
        }

        //                            --------------------------------------------------------}
        // |------------------------|                                                         }
        // |        layer750        |                                                         } -> Measured TotalThickness, Top & Bottom & Dual
        // |------------------------|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void Total_thickness_with_one_measurable_layer_returns_all_probe_and_objectives()
        {
            //Given
            var layer750 = CreateLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer750 };
            var layerGroupTotalWith1Layer = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroupTotalWith1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.NameLayerToMeasure.Should().Be(layerGroupTotalWith1Layer.Name);
                    element.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    element.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |========================|                                                         }
        // |  unmeasurableLayer750  |                                                         } -> Measured TotalThickness, Dual
        // |========================|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void Total_thickness_with_one_unmeasurable_layer_returns_dual_probe_and_objectives()
        {
            //Given
            var layer750 = CreateUnknownLayer(750.Micrometers(), 1);
            var layers = new List<LayerSettings>() { layer750 };
            var layerGroupTotalWith1Layer = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroupTotalWith1Layer };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(
                element =>
                {
                    element.NameLayerToMeasure.Should().Be(layerGroupTotalWith1Layer.Name);
                    element.UpProbes.Should().BeEmpty();
                    element.DownProbes.Should().BeEmpty();
                    element.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |------------------------|                                                         }
        // |        layer50         | -> Measured, Top & Bottom                               }
        // |------------------------|                                                         } -> Measured TotalThickness, Dual & Top & Bottom
        // |------------------------|                                                         }
        // |        layer750        |                                                         }
        // |------------------------|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void Two_measurable_layers_with_one_measured_layer_and_total_thickness_returns_respectively_top_and_bottom_probes_and_all_probes_and_objectives()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer750 = CreateLayer(750.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer50, layer750 };
            var layerGroup50With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer50 }, 1);
            var layerGroupTotalWith2Layers = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With1Layer, layerGroupTotalWith2Layers };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(2).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    firstElement.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    firstElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroupTotalWith2Layers.Name);
                    secondElement.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    secondElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    secondElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |========================|                                                         }
        // |  unmeasurableLayer50   | -> Measured, No Probe                                   }
        // |========================|                                                         } -> Measured TotalThickness, Dual
        // |========================|                                                         }
        // |  unmeasurableLayer750  |                                                         }
        // |========================|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void Two_unmeasurable_layers_with_one_measured_layer_and_total_thickness_returns_respectively_no_probe_and_all_probes_and_objectives()
        {
            //Given
            var layer50 = CreateUnknownLayer(50.Micrometers(), 1);
            var layer750 = CreateUnknownLayer(750.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer50, layer750 };
            var layerGroup50With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer50 }, 1);
            var layerGroupTotalWith2Layers = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With1Layer, layerGroupTotalWith2Layers };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(2).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    firstElement.UpProbes.Should().BeEmpty();
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroupTotalWith2Layers.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().BeEmpty();
                    secondElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |------------------------|                                                         }
        // |        layer50         | -> Measured, Top Probe                                  }
        // |------------------------|                                                         } -> Measured TotalThickness, Dual
        // |========================|                                                         }
        // |  unmeasurableLayer750  |                                                         }
        // |========================|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void One_measurable_layer_measured_with_one_unmeasurable_layer_below_and_total_thickness_returns_respectively_top_probe_and_all_probes_and_objectives()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer750 = CreateUnknownLayer(750.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer50, layer750 };
            var layerGroup50With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer50 }, 1);
            var layerGroupTotalWith2Layers = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With1Layer, layerGroupTotalWith2Layers };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(2).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    firstElement.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroupTotalWith2Layers.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().BeEmpty();
                    secondElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |========================|                                                         }
        // |  unmeasurableLayer50   | -> Measured, No Probe                                   }
        // |========================|                                                         } -> Measured TotalThickness, Dual
        // |------------------------|                                                         }
        // |        layer750        | -> Measured, Bottom Probe                               }
        // |------------------------|                                                         }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void One_unmeasurable_layer_measured_with_one_measurable_layer_measured_below_and_total_thickness_returns_respectively_no_probe_and_top_probe_and_all_probes_and_objectives()
        {
            //Given
            var layer50 = CreateUnknownLayer(50.Micrometers(), 1);
            var layer750 = CreateLayer(750.Micrometers(), 2);
            var layers = new List<LayerSettings>() { layer50, layer750 };
            var layerGroup50With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer50 }, 1);
            var layerGroup750With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer750 }, 1);
            var layerGroupTotalWith2Layers = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);
            var layerGroupsToMeasure = new List<Layer> { layerGroup50With1Layer, layerGroup750With1Layer, layerGroupTotalWith2Layers };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(3).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    firstElement.UpProbes.Should().BeEmpty();
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    secondElement.DualProbes.Should().BeEmpty();
                },
                thirdElement =>
                {
                    thirdElement.NameLayerToMeasure.Should().Be(layerGroupTotalWith2Layers.Name);
                    thirdElement.UpProbes.Should().BeEmpty();
                    thirdElement.DownProbes.Should().BeEmpty();
                    thirdElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }

        //                            --------------------------------------------------------}
        // |------------------------|                                                         }
        // |        layer50         | -> Measured, Top                                        }
        // |------------------------|                                                         }
        // |========================|                                                         }
        // |  unmeasurableLayer100  | -> Measured, No probe                                   }
        // |========================|                                                         }
        // |------------------------|                                                         }
        // |        layer150        | -> Measured, No probe                                   }
        // |------------------------|                                                         }
        // |------------------------| -----}                                                  }
        // |        layer200        |      }                                                  }
        // |------------------------|      } -> Grouped -> Measured, No probe                 }
        // |========================|      }                                                  }
        // |  unmeasurableLayer250  |      }                                                  } -> Measured TotalThickness, Dual
        // |========================| -----}                                                  }
        // |========================|                                                         }
        // |  unmeasurableLayer300  | -> Measured, No probe                                   }
        // |========================|                                                         }
        // |------------------------|                                                         }
        // |        layer350        | -> Measured, Bottom                                     }
        // |------------------------|                                                         }
        // |------------------------|                                                         }
        // |        layer400        | -> Not Measured                                         }
        // |------------------------|                                                         }
        // |------------------------| -----}                                                  }
        // |        layer450        |      }                                                  }
        // |------------------------|      } > Grouped -> Measured, Bottom                    }
        // |------------------------|      }                                                  }
        // |        layer500        |      }                                                  }
        // |------------------------| -----}                                                  }
        //                            --------------------------------------------------------}
        [TestMethod]
        public void CompleteCase()
        {
            //Given
            var layer50 = CreateLayer(50.Micrometers(), 1);
            var layer100 = CreateUnknownLayer(100.Micrometers(), 2);
            var layer150 = CreateLayer(150.Micrometers(), 3);
            var layer200 = CreateLayer(200.Micrometers(), 4);
            var layer250 = CreateUnknownLayer(250.Micrometers(), 5);
            var layer300 = CreateUnknownLayer(300.Micrometers(), 6);
            var layer350 = CreateLayer(350.Micrometers(), 7);
            var layer400 = CreateLayer(400.Micrometers(), 8);
            var layer450 = CreateLayer(450.Micrometers(), 9);
            var layer500 = CreateLayer(500.Micrometers(), 10);

            var layers = new List<LayerSettings>() { layer50, layer100, layer150, layer200, layer250, layer300, layer350, layer400, layer450, layer500 };
            var layerGroup50With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer50 }, 1);
            var layerGroup100With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer100 }, 2);
            var layerGroup150With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer150 }, 3);
            var layerGroup200With2Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer200, layer250 }, 4);
            var layerGroup300With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer300 }, 5);
            var layerGroup350With1Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer350 }, 6);
            var layerGroup450With2Layer = CreateLayerGroup(null, new List<LayerSettings>() { layer450, layer500 }, 7);
            var layerGroupTotalWithAllLayesr = CreateFakeLayerGroupToMeasureTotalThickness(layers, null);

            var layerGroupsToMeasure = new List<Layer>
            {
                layerGroup50With1Layer, layerGroup100With1Layer, layerGroup150With1Layer, layerGroup200With2Layer,
                layerGroup300With1Layer, layerGroup350With1Layer, layerGroup450With2Layer, layerGroupTotalWithAllLayesr
            };
            var thicknessSettings = CreateThicknessSettings(layers, layerGroupsToMeasure);

            //When
            var loader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            var measureThickness = loader.GetMeasure(MeasureType.Thickness);
            var measureTools = measureThickness.GetMeasureTools(thicknessSettings) as ThicknessMeasureTools;

            //Then
            var expectedUpProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseUpId, CompatibleObjectives = GetCompatibleObjectivesUp() };
            var expectedBottomProbeMaterial = new ProbeWithObjectivesMaterial() { ProbeId = LiseBottomId, CompatibleObjectives = new List<string>() { ObjectiveBottomId } };
            var expectedDualProbeMaterial = new DualProbeWithObjectivesMaterial() { ProbeId = "ProbeLiseDouble", UpProbe = expectedUpProbeMaterial, DownProbe = expectedBottomProbeMaterial };

            measureTools.Should().NotBeNull();
            measureTools.MeasureToolsForLayers.Should().NotBeNull().And.HaveCount(8).And.SatisfyRespectively(
                firstElement =>
                {
                    firstElement.NameLayerToMeasure.Should().Be(layerGroup50With1Layer.Name);
                    firstElement.UpProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedUpProbeMaterial); });
                    firstElement.DownProbes.Should().BeEmpty();
                    firstElement.DualProbes.Should().BeEmpty();
                },
                secondElement =>
                {
                    secondElement.NameLayerToMeasure.Should().Be(layerGroup100With1Layer.Name);
                    secondElement.UpProbes.Should().BeEmpty();
                    secondElement.DownProbes.Should().BeEmpty();
                    secondElement.DualProbes.Should().BeEmpty();
                },
                thirdElement =>
                {
                    thirdElement.NameLayerToMeasure.Should().Be(layerGroup150With1Layer.Name);
                    thirdElement.UpProbes.Should().BeEmpty();
                    thirdElement.DownProbes.Should().BeEmpty();
                },
                fourthElement =>
                {
                    fourthElement.NameLayerToMeasure.Should().Be(layerGroup200With2Layer.Name);
                    fourthElement.UpProbes.Should().BeEmpty();
                    fourthElement.DownProbes.Should().BeEmpty();
                    fourthElement.DualProbes.Should().BeEmpty();
                },
                fifthElement =>
                {
                    fifthElement.NameLayerToMeasure.Should().Be(layerGroup300With1Layer.Name);
                    fifthElement.UpProbes.Should().BeEmpty();
                    fifthElement.DownProbes.Should().BeEmpty();
                    fifthElement.DualProbes.Should().BeEmpty();
                },
                sixthElement =>
                {
                    sixthElement.NameLayerToMeasure.Should().Be(layerGroup350With1Layer.Name);
                    sixthElement.UpProbes.Should().BeEmpty();
                    sixthElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    sixthElement.DualProbes.Should().BeEmpty();
                },
                seventhElement =>
                {
                    seventhElement.NameLayerToMeasure.Should().Be(layerGroup450With2Layer.Name);
                    seventhElement.UpProbes.Should().BeEmpty();
                    seventhElement.DownProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedBottomProbeMaterial); });
                    seventhElement.DualProbes.Should().BeEmpty();
                },
                eighthElement =>
                {
                    eighthElement.NameLayerToMeasure.Should().Be(layerGroupTotalWithAllLayesr.Name);
                    eighthElement.UpProbes.Should().BeEmpty();
                    eighthElement.DownProbes.Should().BeEmpty();
                    eighthElement.DualProbes.Should().NotBeNull().And.HaveCount(1).And.AllSatisfy(probe => { probe.Should().BeEquivalentTo(expectedDualProbeMaterial); });
                });
        }
    }
}
